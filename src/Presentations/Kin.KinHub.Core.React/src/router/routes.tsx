import { createBrowserRouter, Navigate } from 'react-router-dom'
import { ProtectedRoute } from '@/components/ProtectedRoute'
import { Layout } from '@/components/Layout'
import { LoginPage } from '@/features/auth/pages/LoginPage'
import { RegisterPage } from '@/features/auth/pages/RegisterPage'
import { DashboardPage } from '@/features/dashboard/pages/DashboardPage'
import { FamilyPage } from '@/features/family/pages/FamilyPage'
import { ServicesPage } from '@/features/family/pages/ServicesPage'
import { RecipeBooksPage } from '@/features/recipes/pages/RecipeBooksPage'
import { RecipeBookDetailPage } from '@/features/recipes/pages/RecipeBookDetailPage'
import { RecipeDetailPage } from '@/features/recipes/pages/RecipeDetailPage'
import { FridgesPage } from '@/features/fridges/pages/FridgesPage'
import { FridgeDetailPage } from '@/features/fridges/pages/FridgeDetailPage'
import { AIAssistantPage } from '@/features/ai-assistant/pages/AIAssistantPage'
import { ProfilePage } from '@/features/profile/pages/ProfilePage'

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
    element: <ProtectedRoute />,
    children: [
      {
        element: <Layout />,
        children: [
          { index: true, element: <Navigate to="/dashboard" replace /> },
          { path: '/dashboard', element: <DashboardPage /> },
          { path: '/family', element: <FamilyPage /> },
          { path: '/services', element: <ServicesPage /> },
          { path: '/recipe-books', element: <RecipeBooksPage /> },
          { path: '/recipe-books/:id', element: <RecipeBookDetailPage /> },
          { path: '/recipe-books/:id/recipes/:recipeId', element: <RecipeDetailPage /> },
          { path: '/fridges', element: <FridgesPage /> },
          { path: '/fridges/:id', element: <FridgeDetailPage /> },
          { path: '/ai-assistant', element: <AIAssistantPage /> },
          { path: '/profile', element: <ProfilePage /> },
        ],
      },
    ],
  },
  { path: '*', element: <Navigate to="/" replace /> },
])
