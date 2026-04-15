import { createBrowserRouter, Navigate } from "react-router-dom";
import { PrivateRoute } from "./PrivateRoute";
import { AdminRoute } from "./AdminRoute";
import { LoginPage } from "@/pages/auth/LoginPage";
import { RegisterPage } from "@/pages/auth/RegisterPage";
import { SelectProfilePage } from "@/pages/auth/SelectProfilePage";
import { VerifyAdminPage } from "@/pages/auth/VerifyAdminPage";
import { DashboardPage } from "@/pages/app/DashboardPage";
import { FamilyPage } from "@/pages/app/FamilyPage";
import { ProfilePage } from "@/pages/app/ProfilePage";
import { ProfileInformationPage } from "@/pages/app/profile/ProfileInformationPage";
import { ProfileAccountPage } from "@/pages/app/profile/ProfileAccountPage";
import { KinConsolePage } from "@/pages/app/KinConsolePage";
import { ServicePage } from "@/pages/app/ServicePage";
import { RecipeBooksPage } from "@/pages/app/RecipeBooksPage";
import { RecipesPage } from "@/pages/app/RecipesPage";
import { RecipeDetailPage } from "@/pages/app/RecipeDetailPage";
import { FridgePage } from "@/pages/app/FridgePage";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/register",
    element: <RegisterPage />,
  },
  {
    path: "/select-profile",
    element: <SelectProfilePage />,
  },
  {
    path: "/verify-admin",
    element: <VerifyAdminPage />,
  },
  {
    element: <PrivateRoute />,
    children: [
      { path: "/", element: <DashboardPage /> },
      { path: "/family", element: <FamilyPage /> },
      { path: "/recipe-books", element: <RecipeBooksPage /> },
      {
        path: "/recipe-books/:recipeBookId/recipes",
        element: <RecipesPage />,
      },
      {
        path: "/recipe-books/:recipeBookId/recipes/:recipeId",
        element: <RecipeDetailPage />,
      },
      { path: "/fridges", element: <FridgePage /> },
      { path: "/fridges/:fridgeId", element: <FridgePage /> },
      {
        path: "/recipe-assistant",
        element: <Navigate to="/recipe-books" replace />,
      },
      { path: "/services/:serviceId", element: <ServicePage /> },
      {
        path: "/profile",
        element: <ProfilePage />,
        children: [
          { index: true, element: <ProfileInformationPage /> },
          {
            element: <AdminRoute />,
            children: [{ path: "account", element: <ProfileAccountPage /> }],
          },
        ],
      },
      {
        element: <AdminRoute />,
        children: [{ path: "/kin-console", element: <KinConsolePage /> }],
      },
    ],
  },
  {
    path: "*",
    element: <Navigate to="/" replace />,
  },
]);
