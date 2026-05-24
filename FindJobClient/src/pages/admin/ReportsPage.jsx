import { useState, useEffect } from 'react';
import { Container, Table, Button, Badge, Modal, Spinner, Alert } from 'react-bootstrap';
import { getReports, deleteReport, deleteJobAndReport } from '../../services/api';

export default function ReportsPage() {
  const [reports, setReports] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [preview, setPreview] = useState(null);

  const load = async () => {
    setLoading(true);
    try {
      const res = await getReports();
      setReports(res.data);
    } catch {
      setError('שגיאה בטעינת הדיווחים');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const handleDismiss = async (id) => {
    if (window.confirm('להסיר את הדיווח מבלי למחוק את המשרה?')) {
      await deleteReport(id);
      load();
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('למחוק את המשרה לצמיתות?')) {
      await deleteJobAndReport(id);
      load();
    }
  };

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
        <h2 className="mb-4">משרות מדווחות</h2>
        {error && <Alert variant="danger">{error}</Alert>}
        {reports.length === 0 ? (
          <Alert variant="success">אין משרות מדווחות כרגע.</Alert>
        ) : (
          <Table striped bordered hover responsive>
            <thead className="table-dark">
              <tr>
                <th>#</th>
                <th>כותרת</th>
                <th>מפרסם</th>
                <th>אימייל</th>
                <th>תאריך</th>
                <th>דיווחים</th>
                <th>פעולות</th>
              </tr>
            </thead>
            <tbody>
              {reports.map((r) => (
                <tr key={r.id}>
                  <td>{r.post?.id}</td>
                  <td>{r.post?.postTitle}</td>
                  <td>{r.post?.nameOfPublisher}</td>
                  <td>{r.post?.email}</td>
                  <td>{r.post?.dateTime ? new Date(r.post.dateTime).toLocaleDateString('he-IL') : '-'}</td>
                  <td>
                    <Badge bg="danger">{r.numberOfReports}</Badge>
                  </td>
                  <td>
                    <div className="d-flex gap-1">
                      <Button size="sm" variant="info" onClick={() => setPreview(r.post)}>
                        תצוגה
                      </Button>
                      <Button size="sm" variant="warning" onClick={() => handleDismiss(r.id)}>
                        הסר דיווח
                      </Button>
                      <Button size="sm" variant="danger" onClick={() => handleDelete(r.id)}>
                        מחק משרה
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
        )}
      </Container>

      <Modal show={!!preview} onHide={() => setPreview(null)} size="lg" dir="rtl">
        <Modal.Header closeButton>
          <Modal.Title>{preview?.postTitle}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {preview && (
            <dl className="row">
              <dt className="col-sm-4">מפרסם</dt>
              <dd className="col-sm-8">{preview.nameOfPublisher}</dd>
              <dt className="col-sm-4">אימייל</dt>
              <dd className="col-sm-8">{preview.email}</dd>
              <dt className="col-sm-4">קטגוריה</dt>
              <dd className="col-sm-8">{preview.categoryPosition} — {preview.subcategory}</dd>
              <dt className="col-sm-4">סוג משרה</dt>
              <dd className="col-sm-8">{preview.typOfJob}</dd>
              <dt className="col-sm-4">מיקום</dt>
              <dd className="col-sm-8">{preview.address}, {preview.region}</dd>
              <dt className="col-sm-4">שכר</dt>
              <dd className="col-sm-8">{preview.salary || '-'}</dd>
              <dt className="col-sm-4">תיאור</dt>
              <dd className="col-sm-8">{preview.description}</dd>
              <dt className="col-sm-4">דרישות</dt>
              <dd className="col-sm-8">{preview.experience || '-'}</dd>
            </dl>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setPreview(null)}>סגור</Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
}
