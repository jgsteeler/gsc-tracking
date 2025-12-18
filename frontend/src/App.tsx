import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Auth0ProviderWithNavigate } from './components/Auth0ProviderWithNavigate'
import { MainLayout } from './layouts/MainLayout'
import { Dashboard } from './pages/Dashboard'
import { Customers } from './pages/Customers'
import { Jobs } from './pages/Jobs'
import { JobDetails } from './pages/JobDetails'
import { Toaster } from './components/ui/toaster'

function App() {
  return (
    <BrowserRouter>
      <Auth0ProviderWithNavigate>
        <Routes>
          <Route path="/" element={<MainLayout />}>
            <Route index element={<Dashboard />} />
            <Route path="customers" element={<Customers />} />
            <Route path="jobs" element={<Jobs />} />
            <Route path="jobs/:id" element={<JobDetails />} />
          </Route>
        </Routes>
        <Toaster />
      </Auth0ProviderWithNavigate>
    </BrowserRouter>
  )
}

export default App
