import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { useUiStore } from "@/stores/uiStore";
import { useAuthStore } from "@/stores/authStore";
import { useUpdateUserEmail } from "@/api/identity/useUpdateUserEmail";
import { useUpdateUserPassword } from "@/api/identity/useUpdateUserPassword";
import { useDeleteUser } from "@/api/identity/useDeleteUser";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";

export function ProfileAccountPage() {
  const { t } = useTranslation();
  const showSnackbar = useUiStore((s) => s.showSnackbar);
  const logout = useAuthStore((s) => s.logout);
  const navigate = useNavigate();

  const updateEmail = useUpdateUserEmail();
  const updatePassword = useUpdateUserPassword();
  const deleteUser = useDeleteUser();

  const [editingEmail, setEditingEmail] = useState(false);
  const [newEmail, setNewEmail] = useState("");

  const [editingPassword, setEditingPassword] = useState(false);
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");

  const [confirmDelete, setConfirmDelete] = useState(false);

  function handleSaveEmail() {
    updateEmail.mutate(
      { newEmail },
      {
        onSuccess: () => {
          showSnackbar(t("app.profile.account.changeEmailSuccess"));
          setEditingEmail(false);
          setNewEmail("");
        },
        onError: (err) => {
          showSnackbar(err.message ?? t("errors.generic"));
        },
      },
    );
  }

  function handleSavePassword() {
    updatePassword.mutate(
      { currentPassword, newPassword },
      {
        onSuccess: () => {
          showSnackbar(t("app.profile.account.changePasswordSuccess"));
          setEditingPassword(false);
          setCurrentPassword("");
          setNewPassword("");
        },
        onError: (err) => {
          showSnackbar(err.message ?? t("errors.generic"));
        },
      },
    );
  }

  function handleDeleteAccount() {
    deleteUser.mutate(undefined, {
      onSuccess: () => {
        showSnackbar(t("app.profile.account.deleteAccountSuccess"));
        logout();
        navigate("/login", { replace: true });
      },
      onError: (err) => {
        showSnackbar(err.message ?? t("errors.generic"));
      },
    });
  }

  return (
    <div className="flex flex-col gap-6">
      {/* Change email */}
      <div className="rounded-xl border border-[var(--border)] bg-[var(--card)] px-5 py-4">
        <div className="flex items-center justify-between">
          <span className="text-sm text-[var(--fg)]">
            {t("app.profile.account.changeEmail")}
          </span>
          {!editingEmail && (
            <Button
              size="sm"
              variant="secondary"
              onClick={() => setEditingEmail(true)}
            >
              {t("app.profile.account.edit")}
            </Button>
          )}
        </div>
        {editingEmail && (
          <div className="mt-3 flex flex-wrap items-end gap-2">
            <Input
              label={t("app.profile.account.newEmail")}
              type="email"
              value={newEmail}
              onChange={(e) => setNewEmail(e.target.value)}
              className="w-64"
              autoFocus
            />
            <Button
              size="sm"
              loading={updateEmail.isPending}
              onClick={handleSaveEmail}
            >
              {t("app.profile.account.save")}
            </Button>
            <Button
              size="sm"
              variant="ghost"
              onClick={() => {
                setEditingEmail(false);
                setNewEmail("");
              }}
            >
              {t("app.profile.account.cancel")}
            </Button>
          </div>
        )}
      </div>

      {/* Change password */}
      <div className="rounded-xl border border-[var(--border)] bg-[var(--card)] px-5 py-4">
        <div className="flex items-center justify-between">
          <span className="text-sm text-[var(--fg)]">
            {t("app.profile.account.changePassword")}
          </span>
          {!editingPassword && (
            <Button
              size="sm"
              variant="secondary"
              onClick={() => setEditingPassword(true)}
            >
              {t("app.profile.account.edit")}
            </Button>
          )}
        </div>
        {editingPassword && (
          <div className="mt-3 flex flex-col gap-3">
            <Input
              label={t("app.profile.account.currentPassword")}
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              className="w-64"
              autoFocus
            />
            <Input
              label={t("app.profile.account.newPassword")}
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              className="w-64"
            />
            <div className="flex gap-2">
              <Button
                size="sm"
                loading={updatePassword.isPending}
                onClick={handleSavePassword}
              >
                {t("app.profile.account.save")}
              </Button>
              <Button
                size="sm"
                variant="ghost"
                onClick={() => {
                  setEditingPassword(false);
                  setCurrentPassword("");
                  setNewPassword("");
                }}
              >
                {t("app.profile.account.cancel")}
              </Button>
            </div>
          </div>
        )}
      </div>

      {/* Delete account */}
      <div className="flex items-center justify-between rounded-xl border border-[var(--border)] bg-[var(--card)] px-5 py-4">
        <span className="text-sm text-[var(--fg)]">
          {t("app.profile.account.deleteAccount")}
        </span>
        {confirmDelete ? (
          <Button
            size="sm"
            variant="danger"
            loading={deleteUser.isPending}
            onClick={handleDeleteAccount}
          >
            {t("app.profile.account.deleteAccountConfirm")}
          </Button>
        ) : (
          <Button
            size="sm"
            variant="danger"
            onClick={() => setConfirmDelete(true)}
          >
            {t("app.profile.account.deleteAccount")}
          </Button>
        )}
      </div>
    </div>
  );
}
