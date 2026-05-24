import { useState } from 'react';
import { Card, Button, Badge, Modal, Form, Alert } from 'react-bootstrap';
import { applyToJob, reportJob } from '../services/api';

export default function JobCard({ job }) {
  const [showApply, setShowApply] = useState(false);
  const [letter, setLetter] = useState('');
  const [cvFile, setCvFile] = useState(null);
  const [sending, setSending] = useState(false);
  const [sent, setSent] = useState(false);
  const [error, setError] = useState('');

  const handleApply = async (e) => {
    e.preventDefault();
    setSending(true);
    setError('');
    try {
      const form = new FormData();
      form.append('jobId', job.id);
      form.append('letter', letter);
      if (cvFile) form.append('cv', cvFile);
      await applyToJob(form);
      setSent(true);
    } catch {
      setError('שגיאה בשליחת הבקשה. נסה שוב.');
    } finally {
      setSending(false);
    }
  };

  const handleReport = async () => {
    if (window.confirm('האם אתה בטוח שברצונך לדווח על מודעה זו?')) {
      await reportJob(job.id);
      alert('הדיווח נשלח בהצלחה');
    }
  };

  return (
    <>
      <Card className="mb-3 shadow-sm" dir="rtl">
        <Card.Body>
          <div className="d-flex justify-content-between align-items-start">
            <div>
              <Card.Title className="text-primary">{job.postTitle}</Card.Title>
              <Card.Subtitle className="mb-2 text-muted">
                {job.nameOfPublisher}
              </Card.Subtitle>
            </div>
            <div className="text-end">
              <Badge bg="secondary" className="me-1">{job.typOfJob}</Badge>
              <Badge bg="info" text="dark">{job.categoryPosition}</Badge>
            </div>
          </div>

          <div className="mb-2">
            <small className="text-muted">
              <i className="bi bi-geo-alt-fill me-1" />
              {job.address}, {job.region}
            </small>
            {job.salary && (
              <small className="text-muted ms-3">
                <i className="bi bi-currency-dollar me-1" />
                {job.salary}
              </small>
            )}
          </div>

          <Card.Text className="text-truncate">{job.description}</Card.Text>

          {job.experience && (
            <p className="text-muted small mb-2">
              <strong>דרישות: </strong>{job.experience}
            </p>
          )}

          <div className="d-flex gap-2">
            <Button variant="primary" size="sm" onClick={() => setShowApply(true)}>
              הגש מועמדות
            </Button>
            <Button variant="outline-danger" size="sm" onClick={handleReport}>
              דווח
            </Button>
          </div>
        </Card.Body>
      </Card>

      <Modal show={showApply} onHide={() => { setShowApply(false); setSent(false); }} dir="rtl">
        <Modal.Header closeButton>
          <Modal.Title>הגשת מועמדות — {job.postTitle}</Modal.Title>
        </Modal.Header>
        {sent ? (
          <Modal.Body>
            <Alert variant="success">המועמדות נשלחה בהצלחה!</Alert>
          </Modal.Body>
        ) : (
          <Form onSubmit={handleApply}>
            <Modal.Body>
              {error && <Alert variant="danger">{error}</Alert>}
              <Form.Group className="mb-3">
                <Form.Label>מכתב פנייה</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={4}
                  value={letter}
                  onChange={(e) => setLetter(e.target.value)}
                  placeholder="כתוב כאן את מכתב הפנייה שלך..."
                  required
                />
              </Form.Group>
              <Form.Group className="mb-3">
                <Form.Label>קובץ קורות חיים (אופציונלי)</Form.Label>
                <Form.Control
                  type="file"
                  accept=".pdf,.doc,.docx"
                  onChange={(e) => setCvFile(e.target.files[0])}
                />
              </Form.Group>
            </Modal.Body>
            <Modal.Footer>
              <Button variant="secondary" onClick={() => setShowApply(false)}>ביטול</Button>
              <Button type="submit" variant="primary" disabled={sending}>
                {sending ? 'שולח...' : 'שלח'}
              </Button>
            </Modal.Footer>
          </Form>
        )}
      </Modal>
    </>
  );
}
