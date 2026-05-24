import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Modal, Button, Form, Alert } from 'react-bootstrap';
import { adminLogin } from '../services/api';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const [showLogin, setShowLogin] = useState(false);
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { isAdmin, login, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await adminLogin({ username, password });
      login(res.data.token);
      setShowLogin(false);
      navigate('/admin');
    } catch {
      setError('שם משתמש או סיסמה שגויים');
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark bg-primary" dir="rtl">
        <div className="container">
          <Link className="navbar-brand fw-bold" to="/">FindJob</Link>
          <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navMenu">
            <span className="navbar-toggler-icon" />
          </button>
          <div className="collapse navbar-collapse" id="navMenu">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                <Link className="nav-link" to="/post-job">פרסם משרה</Link>
              </li>
              <li className="nav-item">
                <Link className="nav-link" to="/jobs">כל המשרות</Link>
              </li>
            </ul>
            <ul className="navbar-nav">
              {isAdmin ? (
                <>
                  <li className="nav-item">
                    <Link className="nav-link" to="/admin">ניהול</Link>
                  </li>
                  <li className="nav-item">
                    <button className="nav-link btn btn-link text-white" onClick={logout}>יציאה</button>
                  </li>
                </>
              ) : (
                <li className="nav-item">
                  <button className="nav-link btn btn-link text-white" onClick={() => setShowLogin(true)}>
                    כניסת מנהל
                  </button>
                </li>
              )}
            </ul>
          </div>
        </div>
      </nav>

      <Modal show={showLogin} onHide={() => setShowLogin(false)} dir="rtl">
        <Modal.Header closeButton>
          <Modal.Title>כניסת מנהל</Modal.Title>
        </Modal.Header>
        <Form onSubmit={handleLogin}>
          <Modal.Body>
            {error && <Alert variant="danger">{error}</Alert>}
            <Form.Group className="mb-3">
              <Form.Label>שם משתמש</Form.Label>
              <Form.Control
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>סיסמה</Form.Label>
              <Form.Control
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </Form.Group>
          </Modal.Body>
          <Modal.Footer>
            <Button variant="secondary" onClick={() => setShowLogin(false)}>ביטול</Button>
            <Button type="submit" variant="primary" disabled={loading}>
              {loading ? 'מתחבר...' : 'כניסה'}
            </Button>
          </Modal.Footer>
        </Form>
      </Modal>
    </>
  );
}
