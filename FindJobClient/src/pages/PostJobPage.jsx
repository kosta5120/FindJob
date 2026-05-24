import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Row, Col, Form, Button, Card, Alert, Modal } from 'react-bootstrap';
import { getCategories, getSubcategories, getRegions, getCities, getJobTypes, createJob } from '../services/api';

const INITIAL = {
  postTitle: '',
  nameOfPublisher: '',
  email: '',
  description: '',
  experience: '',
  salary: '',
  numberOfPosition: 1,
  categoryId: '',
  subcategoryId: '',
  regionId: '',
  cityId: '',
  jobTypeId: '',
};

export default function PostJobPage() {
  const [form, setForm] = useState(INITIAL);
  const [categories, setCategories] = useState([]);
  const [subcategories, setSubcategories] = useState([]);
  const [regions, setRegions] = useState([]);
  const [cities, setCities] = useState([]);
  const [jobTypes, setJobTypes] = useState([]);
  const [submitting, setSubmitting] = useState(false);
  const [showSuccess, setShowSuccess] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    Promise.all([getCategories(), getRegions(), getJobTypes()]).then(([cats, regs, types]) => {
      setCategories(cats.data);
      setRegions(regs.data);
      setJobTypes(types.data);
    });
  }, []);

  const handleCategoryChange = async (categoryId) => {
    setForm((f) => ({ ...f, categoryId, subcategoryId: '' }));
    if (categoryId) {
      const res = await getSubcategories(categoryId);
      setSubcategories(res.data);
    } else {
      setSubcategories([]);
    }
  };

  const handleRegionChange = async (regionId) => {
    setForm((f) => ({ ...f, regionId, cityId: '' }));
    if (regionId) {
      const res = await getCities(regionId);
      setCities(res.data);
    } else {
      setCities([]);
    }
  };

  const set = (key) => (e) => setForm((f) => ({ ...f, [key]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSubmitting(true);
    try {
      await createJob(form);
      setShowSuccess(true);
    } catch {
      setError('שגיאה בשמירת המשרה. נסה שוב.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div dir="rtl">
      <Container className="py-5">
        <Row className="justify-content-center">
          <Col md={8}>
            <Card className="shadow-sm">
              <Card.Header className="bg-primary text-white">
                <h4 className="mb-0">פרסם משרה</h4>
              </Card.Header>
              <Card.Body className="p-4">
                {error && <Alert variant="danger">{error}</Alert>}
                <Form onSubmit={handleSubmit}>
                  <Row className="g-3">
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>כותרת המשרה *</Form.Label>
                        <Form.Control value={form.postTitle} onChange={set('postTitle')} required />
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>שם המפרסם *</Form.Label>
                        <Form.Control value={form.nameOfPublisher} onChange={set('nameOfPublisher')} required />
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>אימייל ליצירת קשר *</Form.Label>
                        <Form.Control type="email" value={form.email} onChange={set('email')} required />
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>שכר</Form.Label>
                        <Form.Control value={form.salary} onChange={set('salary')} placeholder="לדוגמה: 10,000-15,000 ₪" />
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>קטגוריה *</Form.Label>
                        <Form.Select value={form.categoryId} onChange={(e) => handleCategoryChange(e.target.value)} required>
                          <option value="">בחר קטגוריה</option>
                          {categories.map((c) => <option key={c.id} value={c.id}>{c.categoryWork}</option>)}
                        </Form.Select>
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>תת-קטגוריה</Form.Label>
                        <Form.Select value={form.subcategoryId} onChange={set('subcategoryId')} disabled={!form.categoryId}>
                          <option value="">בחר תת-קטגוריה</option>
                          {subcategories.map((s) => <option key={s.id} value={s.id}>{s.subcategory}</option>)}
                        </Form.Select>
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>אזור *</Form.Label>
                        <Form.Select value={form.regionId} onChange={(e) => handleRegionChange(e.target.value)} required>
                          <option value="">בחר אזור</option>
                          {regions.map((r) => <option key={r.idRegion} value={r.idRegion}>{r.region}</option>)}
                        </Form.Select>
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>עיר</Form.Label>
                        <Form.Select value={form.cityId} onChange={set('cityId')} disabled={!form.regionId}>
                          <option value="">בחר עיר</option>
                          {cities.map((c) => <option key={c.idCity} value={c.idCity}>{c.city}</option>)}
                        </Form.Select>
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>סוג משרה *</Form.Label>
                        <Form.Select value={form.jobTypeId} onChange={set('jobTypeId')} required>
                          <option value="">בחר סוג משרה</option>
                          {jobTypes.map((t) => <option key={t.idTypePosition} value={t.idTypePosition}>{t.jobType}</option>)}
                        </Form.Select>
                      </Form.Group>
                    </Col>
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>מספר משרות פתוחות</Form.Label>
                        <Form.Control type="number" min={1} value={form.numberOfPosition} onChange={set('numberOfPosition')} />
                      </Form.Group>
                    </Col>
                    <Col xs={12}>
                      <Form.Group>
                        <Form.Label>תיאור המשרה *</Form.Label>
                        <Form.Control as="textarea" rows={4} value={form.description} onChange={set('description')} required />
                      </Form.Group>
                    </Col>
                    <Col xs={12}>
                      <Form.Group>
                        <Form.Label>דרישות וניסיון נדרש</Form.Label>
                        <Form.Control as="textarea" rows={3} value={form.experience} onChange={set('experience')} />
                      </Form.Group>
                    </Col>
                    <Col xs={12}>
                      <Button type="submit" variant="primary" className="w-100" disabled={submitting}>
                        {submitting ? 'שומר...' : 'פרסם משרה'}
                      </Button>
                    </Col>
                  </Row>
                </Form>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>

      <Modal show={showSuccess} onHide={() => { setShowSuccess(false); navigate('/'); }} dir="rtl" centered>
        <Modal.Body className="text-center p-5">
          <div className="text-success display-4 mb-3">✓</div>
          <h4>המשרה פורסמה בהצלחה!</h4>
          <p className="text-muted">המשרה תופיע בתוצאות החיפוש כעת.</p>
          <Button variant="primary" onClick={() => { setShowSuccess(false); navigate('/'); }}>
            חזור לעמוד הבית
          </Button>
        </Modal.Body>
      </Modal>
    </div>
  );
}
