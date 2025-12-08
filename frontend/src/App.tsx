import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { MainLayout } from './layouts/MainLayout'
import { Dashboard } from './pages/Dashboard'
import { Customers } from './pages/Customers'
import { Jobs } from './pages/Jobs'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<Dashboard />} />
          <Route path="customers" element={<Customers />} />
          <Route path="jobs" element={<Jobs />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
