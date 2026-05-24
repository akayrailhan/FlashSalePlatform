import { useMemo } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';

import Checkout from './components/Checkout';
import Dashboard from './components/Dashboard';
import Login from './components/Login';
import MyTickets from './components/MyTickets';

function App() {
  const toastStyles = useMemo(
    () => ({
      style: {
        background: '#0f172a',
        color: '#f8fafc',
        border: '1px solid #dc2626',
      },
      iconTheme: {
        primary: '#dc2626',
        secondary: '#0f172a',
      },
    }),
    []
  );

  return (
    <Router>
      <Toaster position="top-right" toastOptions={toastStyles} />

      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/login" element={<Login />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/my-tickets" element={<MyTickets />} />
        <Route path="/checkout/:id" element={<Checkout />} />
      </Routes>
    </Router>
  );
}

export default App;