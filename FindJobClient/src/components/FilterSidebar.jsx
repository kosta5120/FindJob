import { Form, Card, Button } from 'react-bootstrap';

export default function FilterSidebar({ filters, onChange, onReset, categories, jobTypes, regions, cities }) {
  const update = (key, value) => onChange({ ...filters, [key]: value });

  return (
    <Card dir="rtl" className="sticky-top" style={{ top: '80px' }}>
      <Card.Header className="bg-primary text-white fw-bold">סינון משרות</Card.Header>
      <Card.Body>
        <Form.Group className="mb-3">
          <Form.Label className="fw-semibold">קטגוריה</Form.Label>
          <Form.Select value={filters.category || ''} onChange={(e) => update('category', e.target.value)}>
            <option value="">כל הקטגוריות</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>{c.categoryWork}</option>
            ))}
          </Form.Select>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label className="fw-semibold">סוג משרה</Form.Label>
          <Form.Select value={filters.jobType || ''} onChange={(e) => update('jobType', e.target.value)}>
            <option value="">כל הסוגים</option>
            {jobTypes.map((t) => (
              <option key={t.idTypePosition} value={t.idTypePosition}>{t.jobType}</option>
            ))}
          </Form.Select>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label className="fw-semibold">אזור</Form.Label>
          <Form.Select value={filters.region || ''} onChange={(e) => { update('region', e.target.value); update('city', ''); }}>
            <option value="">כל האזורים</option>
            {regions.map((r) => (
              <option key={r.idRegion} value={r.idRegion}>{r.region}</option>
            ))}
          </Form.Select>
        </Form.Group>

        {cities.length > 0 && (
          <Form.Group className="mb-3">
            <Form.Label className="fw-semibold">עיר</Form.Label>
            <Form.Select value={filters.city || ''} onChange={(e) => update('city', e.target.value)}>
              <option value="">כל הערים</option>
              {cities.map((c) => (
                <option key={c.idCity} value={c.idCity}>{c.city}</option>
              ))}
            </Form.Select>
          </Form.Group>
        )}

        <Button variant="outline-secondary" size="sm" className="w-100" onClick={onReset}>
          נקה סינון
        </Button>
      </Card.Body>
    </Card>
  );
}
