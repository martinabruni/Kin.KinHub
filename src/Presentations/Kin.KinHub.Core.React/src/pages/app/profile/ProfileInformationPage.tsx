import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useProfileStore } from "@/stores/profileStore";
import { useGetFamily } from "@/api/core/useGetFamily";
import { useUpdateProfileName } from "@/api/core/useUpdateProfileName";
import { useUpdateFamilyName } from "@/api/core/useUpdateFamilyName";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { Spinner } from "@/components/ui/Spinner";

export function ProfileInformationPage() {
  const { t } = useTranslation();
  const { selectedProfile, selectProfile } = useProfileStore();
  const { data: family, isLoading: familyLoading } = useGetFamily();
  const updateProfileName = useUpdateProfileName();
  const updateFamilyName = useUpdateFamilyName();

  const [editingProfileName, setEditingProfileName] = useState(false);
  const [profileNameValue, setProfileNameValue] = useState(
    selectedProfile?.name ?? "",
  );

  const [editingFamilyName, setEditingFamilyName] = useState(false);
  const [familyNameValue, setFamilyNameValue] = useState(family?.name ?? "");

  function handleSaveProfileName() {
    if (!selectedProfile) return;
    updateProfileName.mutate(
      { profileId: selectedProfile.id, name: profileNameValue },
      {
        onSuccess: () => {
          selectProfile({ ...selectedProfile, name: profileNameValue });
          setEditingProfileName(false);
        },
      },
    );
  }

  function handleSaveFamilyName() {
    if (!family) return;
    updateFamilyName.mutate(
      { familyId: family.familyId, name: familyNameValue },
      {
        onSuccess: () => {
          setEditingFamilyName(false);
        },
      },
    );
  }

  if (familyLoading) {
    return (
      <div className="flex justify-center py-8">
        <Spinner size="lg" className="text-[var(--accent)]" />
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-6">
      {/* Profile name */}
      <div>
        <p className="mb-1 text-xs font-medium uppercase tracking-wide text-[var(--muted)]">
          {t("app.profile.information.profileName")}
        </p>
        {editingProfileName ? (
          <div className="flex flex-wrap items-center gap-2">
            <Input
              value={profileNameValue}
              onChange={(e) => setProfileNameValue(e.target.value)}
              className="w-56"
              autoFocus
            />
            <Button
              size="sm"
              loading={updateProfileName.isPending}
              onClick={handleSaveProfileName}
            >
              {t("app.profile.information.save")}
            </Button>
            <Button
              size="sm"
              variant="ghost"
              onClick={() => {
                setProfileNameValue(selectedProfile?.name ?? "");
                setEditingProfileName(false);
              }}
            >
              {t("app.profile.information.cancel")}
            </Button>
          </div>
        ) : (
          <div className="flex items-center gap-3">
            <span className="text-sm text-[var(--fg)]">
              {selectedProfile?.name}
            </span>
            <Button
              size="sm"
              variant="secondary"
              onClick={() => {
                setProfileNameValue(selectedProfile?.name ?? "");
                setEditingProfileName(true);
              }}
            >
              {t("app.profile.information.edit")}
            </Button>
          </div>
        )}
      </div>

      {/* Family name (admin only) */}
      {selectedProfile?.role === "admin" && family && (
        <div>
          <p className="mb-1 text-xs font-medium uppercase tracking-wide text-[var(--muted)]">
            {t("app.profile.information.familyName")}
          </p>
          {editingFamilyName ? (
            <div className="flex flex-wrap items-center gap-2">
              <Input
                value={familyNameValue}
                onChange={(e) => setFamilyNameValue(e.target.value)}
                className="w-56"
                autoFocus
              />
              <Button
                size="sm"
                loading={updateFamilyName.isPending}
                onClick={handleSaveFamilyName}
              >
                {t("app.profile.information.save")}
              </Button>
              <Button
                size="sm"
                variant="ghost"
                onClick={() => {
                  setFamilyNameValue(family?.name ?? "");
                  setEditingFamilyName(false);
                }}
              >
                {t("app.profile.information.cancel")}
              </Button>
            </div>
          ) : (
            <div className="flex items-center gap-3">
              <span className="text-sm text-[var(--fg)]">{family.name}</span>
              <Button
                size="sm"
                variant="secondary"
                onClick={() => {
                  setFamilyNameValue(family?.name ?? "");
                  setEditingFamilyName(true);
                }}
              >
                {t("app.profile.information.edit")}
              </Button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
