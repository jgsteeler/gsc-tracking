import { lazy, Suspense } from 'react'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { Auth0ProviderWithNavigate } from './components/Auth0ProviderWithNavigate'
import { ProtectedRoute } from './components/ProtectedRoute'
import { MainLayout } from './layouts/MainLayout'
import { Toaster } from './components/ui/toaster'

const Landing = lazy(() => import('./pages/Landing'))
const Dashboard = lazy(() => import('./pages/Dashboard'))
const Customers = lazy(() => import('./pages/Customers'))
const Jobs = lazy(() => import('./pages/Jobs'))
const JobDetails = lazy(() => import('./pages/JobDetails'))

function App() {
  return (
    <BrowserRouter>
      <Auth0ProviderWithNavigate>
        <Suspense fallback={<div>Loading...</div>}>
          <Routes>
            <Route path="/landing" element={<Landing />} />
            <Route path="/" element={
              <ProtectedRoute>
                <MainLayout />
              </ProtectedRoute>
            }>
              <Route index element={<Dashboard />} />
              <Route path="customers" element={<Customers />} />
              <Route path="jobs" element={<Jobs />} />
              <Route path="jobs/:id" element={<JobDetails />} />
            </Route>
          </Routes>
        </Suspense>
        <Toaster />
      </Auth0ProviderWithNavigate>
    </BrowserRouter>
  )
}

export default App
