import { useState, useEffect } from 'react'
import './App.css'

interface ApiResponse {
  message: string;
  version: string;
  timestamp: string;
}

function App() {
  const [apiMessage, setApiMessage] = useState<string>('Loading...');
  const [error, setError] = useState<string>('');

  useEffect(() => {
    // Call the backend API
    fetch('https://localhost:5001/api/hello')
      .then(response => response.json())
      .then((data: ApiResponse) => {
        setApiMessage(data.message);
        setError('');
      })
      .catch(() => {
        setError('Unable to connect to API. Make sure the backend is running.');
        setApiMessage('');
      });
  }, []);

  return (
    <div className="app">
      <h1>GSC Tracking - Small Engine Repair</h1>
      <p>Welcome to the GSC Tracking business management application!</p>
      
      <div className="card">
        <h2>Backend API Status</h2>
        {error ? (
          <p className="error">{error}</p>
        ) : (
          <p className="success">{apiMessage}</p>
        )}
      </div>

      <p className="info">
        This is a Hello World setup. The full application will manage customers, jobs, and finances.
      </p>
    </div>
  )
}

export default App
