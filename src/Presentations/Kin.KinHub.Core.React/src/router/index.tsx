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
