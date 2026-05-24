import { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Spinner, Alert } from 'react-bootstrap';
import { Bar, BarChart, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { getMonthlyStats, getCityStats } from '../../services/api';

const MONTH_NAMES = {
  1: 'ינואר', 2: 'פברואר', 3: 'מרץ', 4: 'אפריל',
  5: 'מאי', 6: 'יוני', 7: 'יולי', 8: 'אוגוסט',
  9: 'ספטמבר', 10: 'אוקטובר', 11: 'נובמבר', 12: 'דצמבר',
};

export default function AdminDashboard() {
  const [monthlyData, setMonthlyData] = useState([]);
  const [cityData, setCityData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    Promise.all([getMonthlyStats(), getCityStats()])
      .then(([monthly, cities]) => {
        setMonthlyData(
          monthly.data.map((d) => ({ month: MONTH_NAMES[d.month] || d.month, count: d.count }))
        );
        setCityData(cities.data);
      })
      .catch(() => setError('שגיאה בטעינת הסטטיסטיקות'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '50vh' }}>
        <Spinner animation="border" variant="primary" />
      </div>
    );
  }

  return (
    <div dir="rtl">
      <Container className="py-4">
        <h2 className="mb-4">לוח בקרה</h2>
        {error && <Alert variant="danger">{error}</Alert>}
        <Row className="g-4">
          <Col md={12}>
            <Card className="shadow-sm">
              <Card.Header className="bg-primary text-white">
                <h5 className="mb-0">משרות שפורסמו לפי חודש</h5>
              </Card.Header>
              <Card.Body>
                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={monthlyData} margin={{ top: 5, right: 20, bottom: 5, left: 0 }}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="month" />
                    <YAxis allowDecimals={false} />
                    <Tooltip />
                    <Bar dataKey="count" fill="#0d6efd" name="משרות" />
                  </BarChart>
                </ResponsiveContainer>
              </Card.Body>
            </Card>
          </Col>
          <Col md={12}>
            <Card className="shadow-sm">
              <Card.Header className="bg-success text-white">
                <h5 className="mb-0">כמות משרות לפי עיר</h5>
              </Card.Header>
              <Card.Body>
                <ResponsiveContainer width="100%" height={Math.max(300, cityData.length * 30)}>
                  <BarChart data={cityData} layout="vertical" margin={{ top: 5, right: 20, bottom: 5, left: 80 }}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis type="number" allowDecimals={false} />
                    <YAxis dataKey="city" type="category" width={80} />
                    <Tooltip />
                    <Bar dataKey="count" fill="#198754" name="משרות" />
                  </BarChart>
                </ResponsiveContainer>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
    </div>
  );
}
