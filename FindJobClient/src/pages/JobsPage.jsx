import { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Container, Row, Col, Spinner, Alert, Pagination } from 'react-bootstrap';
import JobCard from '../components/JobCard';
import FilterSidebar from '../components/FilterSidebar';
import { getJobs, getCategories, getJobTypes, getRegions, getCities } from '../services/api';

const PAGE_SIZE = 10;

export default function JobsPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [jobs, setJobs] = useState([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [categories, setCategories] = useState([]);
  const [jobTypes, setJobTypes] = useState([]);
  const [regions, setRegions] = useState([]);
  const [cities, setCities] = useState([]);

  const [filters, setFilters] = useState({
    what: searchParams.get('what') || '',
    where: searchParams.get('where') || '',
    category: searchParams.get('category') || '',
    jobType: searchParams.get('jobType') || '',
    region: searchParams.get('region') || '',
    city: searchParams.get('city') || '',
  });

  useEffect(() => {
    Promise.all([getCategories(), getJobTypes(), getRegions()]).then(([cats, types, regs]) => {
      setCategories(cats.data);
      setJobTypes(types.data);
      setRegions(regs.data);
    });
  }, []);

  useEffect(() => {
    if (filters.region) {
      getCities(filters.region).then((res) => setCities(res.data));
    } else {
      setCities([]);
    }
  }, [filters.region]);

  const fetchJobs = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const params = { ...filters, page, pageSize: PAGE_SIZE };
      const res = await getJobs(params);
      setJobs(res.data.items);
      setTotal(res.data.total);
    } catch {
      setError('שגיאה בטעינת המשרות');
    } finally {
      setLoading(false);
    }
  }, [filters, page]);

  useEffect(() => {
    fetchJobs();
  }, [fetchJobs]);

  const handleFilterChange = (newFilters) => {
    setFilters(newFilters);
    setPage(1);
    const params = {};
    Object.entries(newFilters).forEach(([k, v]) => { if (v) params[k] = v; });
    setSearchParams(params);
  };

  const handleReset = () => handleFilterChange({ what: '', where: '', category: '', jobType: '', region: '', city: '' });

  const totalPages = Math.ceil(total / PAGE_SIZE);

  return (
    <div dir="rtl">
      <Container className="py-4">
        <Row>
          <Col md={3}>
            <FilterSidebar
              filters={filters}
              onChange={handleFilterChange}
              onReset={handleReset}
              categories={categories}
              jobTypes={jobTypes}
              regions={regions}
              cities={cities}
            />
          </Col>
          <Col md={9}>
            <div className="d-flex justify-content-between align-items-center mb-3">
              <h5 className="mb-0">
                {loading ? 'טוען...' : `נמצאו ${total} משרות`}
              </h5>
              {(filters.what || filters.where) && (
                <small className="text-muted">
                  {filters.what && `מה: "${filters.what}"`}
                  {filters.what && filters.where && ' | '}
                  {filters.where && `איפה: "${filters.where}"`}
                </small>
              )}
            </div>

            {error && <Alert variant="danger">{error}</Alert>}

            {loading ? (
              <div className="text-center py-5">
                <Spinner animation="border" variant="primary" />
              </div>
            ) : jobs.length === 0 ? (
              <Alert variant="info">לא נמצאו משרות התואמות את החיפוש.</Alert>
            ) : (
              <>
                {jobs.map((job) => <JobCard key={job.id} job={job} />)}

                {totalPages > 1 && (
                  <div className="d-flex justify-content-center mt-4">
                    <Pagination>
                      <Pagination.Prev disabled={page === 1} onClick={() => setPage(page - 1)} />
                      {Array.from({ length: totalPages }, (_, i) => (
                        <Pagination.Item key={i + 1} active={page === i + 1} onClick={() => setPage(i + 1)}>
                          {i + 1}
                        </Pagination.Item>
                      ))}
                      <Pagination.Next disabled={page === totalPages} onClick={() => setPage(page + 1)} />
                    </Pagination>
                  </div>
                )}
              </>
            )}
          </Col>
        </Row>
      </Container>
    </div>
  );
}
