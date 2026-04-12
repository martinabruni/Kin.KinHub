import { createBrowserRouter, Navigate } from 'react-router-dom'
import { PrivateRoute } from './PrivateRoute'
import { LoginPage } from '@/pages/auth/LoginPage'
import { RegisterPage } from '@/pages/auth/RegisterPage'
import { DashboardPage } from '@/pages/app/DashboardPage'
import { FamilyPage } from '@/pages/app/FamilyPage'
import { ProfilePage } from '@/pages/app/ProfilePage'

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/register',
    element: <RegisterPage />,
  },
  {
    element: <PrivateRoute />,
    children: [
      { path: '/', element: <DashboardPage /> },
      { path: '/family', element: <FamilyPage /> },
      { path: '/profile', element: <ProfilePage /> },
    ],
  },
  {
    path: '*',
    element: <Navigate to="/" replace />,
  },
])
