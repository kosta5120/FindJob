import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Row, Col, Form, Button, Card, Badge, Spinner } from 'react-bootstrap';
import { getCategories, getJobTypes, getRegions } from '../services/api';

export default function HomePage() {
  const [what, setWhat] = useState('');
  const [where, setWhere] = useState('');
  const [categories, setCategories] = useState([]);
  const [jobTypes, setJobTypes] = useState([]);
  const [regions, setRegions] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    Promise.all([getCategories(), getJobTypes(), getRegions()])
      .then(([cats, types, regs]) => {
        setCategories(cats.data);
        setJobTypes(types.data);
        setRegions(regs.data);
      })
      .finally(() => setLoading(false));
  }, []);

  const handleSearch = (e) => {
    e.preventDefault();
    const params = new URLSearchParams();
    if (what) params.set('what', what);
    if (where) params.set('where', where);
    navigate(`/jobs?${params.toString()}`);
  };

  const browseByCategory = (categoryId) => navigate(`/jobs?category=${categoryId}`);
  const browseByType = (typeId) => navigate(`/jobs?jobType=${typeId}`);
  const browseByRegion = (regionId) => navigate(`/jobs?region=${regionId}`);

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '60vh' }}>
        <Spinner animation="border" variant="primary" />
      </div>
    );
  }

  return (
    <div dir="rtl">
      {/* Hero Section */}
      <div className="bg-primary text-white py-5">
        <Container>
          <h1 className="display-5 fw-bold text-center mb-4">מצא את המשרה המושלמת</h1>
          <Form onSubmit={handleSearch}>
            <Row className="g-2 justify-content-center">
              <Col md={4}>
                <Form.Control
                  size="lg"
                  type="text"
                  placeholder="מה? (תפקיד, קטגוריה...)"
                  value={what}
                  onChange={(e) => setWhat(e.target.value)}
                />
              </Col>
              <Col md={4}>
                <Form.Control
                  size="lg"
                  type="text"
                  placeholder="איפה? (עיר, אזור...)"
                  value={where}
                  onChange={(e) => setWhere(e.target.value)}
                />
              </Col>
              <Col md={2}>
                <Button type="submit" size="lg" variant="warning" className="w-100 fw-bold">
                  חפש
                </Button>
              </Col>
            </Row>
          </Form>
        </Container>
      </div>

      <Container className="py-5">
        {/* Categories */}
        <h2 className="mb-4 text-center">קטגוריות</h2>
        <Row className="g-3 mb-5">
          {categories.map((cat) => (
            <Col key={cat.id} xs={6} md={4} lg={3}>
              <Card
                className="h-100 text-center cursor-pointer border-0 shadow-sm"
                style={{ cursor: 'pointer' }}
                onClick={() => browseByCategory(cat.id)}
              >
                <Card.Body className="py-3">
                  <div className="text-primary fw-semibold">{cat.categoryWork}</div>
                </Card.Body>
              </Card>
            </Col>
          ))}
        </Row>

        {/* Job Types */}
        <h2 className="mb-4 text-center">סוגי משרות</h2>
        <div className="d-flex flex-wrap gap-2 justify-content-center mb-5">
          {jobTypes.map((t) => (
            <Button
              key={t.idTypePosition}
              variant="outline-primary"
              onClick={() => browseByType(t.idTypePosition)}
            >
              {t.jobType}
            </Button>
          ))}
        </div>

        {/* Regions */}
        <h2 className="mb-4 text-center">אזורים</h2>
        <div className="d-flex flex-wrap gap-2 justify-content-center">
          {regions.map((r) => (
            <Badge
              key={r.idRegion}
              bg="secondary"
              style={{ cursor: 'pointer', fontSize: '1rem', padding: '0.5rem 1rem' }}
              onClick={() => browseByRegion(r.idRegion)}
            >
              {r.region}
            </Badge>
          ))}
        </div>
      </Container>
    </div>
  );
}
