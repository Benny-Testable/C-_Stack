import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { loginStudent } from '../services/studentApi'
import { loginAdmin } from '../services/adminApi'

export default function Login() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('ava.nguyen@example.test')
  const [password, setPassword] = useState('TEST_ONLY_FAKE_PASSWORD')
  const [role, setRole] = useState('student')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  function validateLogin(nextEmail, nextPassword) {
    const problems = []
    if (!nextEmail || nextEmail.trim().length === 0) {
      problems.push('Email is required')
    } else if (!nextEmail.includes('@') || !nextEmail.includes('.')) {
      problems.push('Email format is invalid')
    } else if (nextEmail.length > 120) {
      problems.push('Email is too long')
    }
    if (!nextPassword || nextPassword.trim().length === 0) {
      problems.push('Password is required')
    } else if (nextPassword.length < 8) {
      problems.push('Password is too short')
    } else if (nextPassword.length > 80) {
      problems.push('Password is too long')
    }
    return problems
  }

  async function onSubmit(event) {
    event.preventDefault()
    const problems = validateLogin(email, password)
    if (problems.length > 0) {
      setError(problems.join('; '))
      return
    }
    setLoading(true)
    setError('')
    const result = role === 'admin'
      ? await loginAdmin(email, password)
      : await loginStudent(email, password)
    setLoading(false)
    if (!result.success) {
      setError(result.message)
      return
    }
    localStorage.setItem('cmgroups.token', result.data.token || '')
    localStorage.setItem('cmgroups.role', result.data.role || role)
    localStorage.setItem('cmgroups.userId', String(result.data.userId || ''))
    localStorage.setItem('cmgroups.name', result.data.displayName || '')
    navigate(role === 'admin' ? '/admin' : '/student')
  }

  return (
    <section className="panel narrow">
      <h1>Sign in</h1>
      <p>Use a synthetic seed account. Passwords in this repository are fake test values.</p>
      <form onSubmit={onSubmit}>
        <label>
          Role
          <select value={role} onChange={(event) => setRole(event.target.value)}>
            <option value="student">Student</option>
            <option value="admin">Admin</option>
          </select>
        </label>
        <label>
          Email
          <input value={email} onChange={(event) => setEmail(event.target.value)} />
        </label>
        <label>
          Password
          <input type="password" value={password} onChange={(event) => setPassword(event.target.value)} />
        </label>
        {error ? <p className="error">{error}</p> : null}
        <button type="submit" disabled={loading}>{loading ? 'Signing in...' : 'Sign in'}</button>
      </form>
    </section>
  )
}
