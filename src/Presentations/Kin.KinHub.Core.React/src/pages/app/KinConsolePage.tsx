import { useTranslation } from "react-i18next";
import { Card } from "@/components/ui/Card";
import { Spinner } from "@/components/ui/Spinner";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { useGetServices } from "@/api/core/useGetServices";
import { useGetFamilyServices } from "@/api/core/useGetFamilyServices";
import { useToggleFamilyService } from "@/api/core/useToggleFamilyService";
import { useProfileStore } from "@/stores/profileStore";
import type { KinHubServiceDto } from "@/types/core";

const KIN_CONSOLE_SERVICE_ID = 1;

export function KinConsolePage() {
  const { t } = useTranslation();
  const { selectedProfile } = useProfileStore();
  const familyId = selectedProfile!.familyId;

  const { data: allServices, isLoading: loadingServices } = useGetServices();
  const { data: familyServices, isLoading: loadingFamilyServices } =
    useGetFamilyServices(familyId);
  const toggleMutation = useToggleFamilyService(familyId);

  if (loadingServices || loadingFamilyServices) {
    return (
      <div className="flex justify-center pt-16">
        <Spinner size="lg" className="text-[var(--accent)]" />
      </div>
    );
  }

  function isServiceActive(service: KinHubServiceDto): boolean {
    if (service.id === KIN_CONSOLE_SERVICE_ID) return true;
    const familyService = familyServices?.find(
      (fs) => fs.serviceId === service.id,
    );
    return familyService?.isActive ?? false;
  }

  function handleToggle(service: KinHubServiceDto) {
    if (service.id === KIN_CONSOLE_SERVICE_ID) return;
    const currentActive = isServiceActive(service);
    toggleMutation.mutate({ serviceId: service.id, isActive: !currentActive });
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-[var(--fg)]">KinConsole</h1>
      <p className="text-sm text-[var(--muted)]">
        Gestisci i servizi attivi per la tua famiglia.
      </p>

      <div className="flex flex-col gap-3">
        {(allServices ?? []).map((service) => {
          const active = isServiceActive(service);
          const isKinConsole = service.id === KIN_CONSOLE_SERVICE_ID;

          return (
            <Card key={service.id}>
              <div className="flex items-center justify-between gap-4">
                <div className="flex flex-col gap-1">
                  <div className="flex items-center gap-2">
                    <span className="font-medium text-[var(--fg)]">
                      {service.name}
                    </span>
                    {service.isAdminOnly && (
                      <Badge variant="admin" label="Admin only" />
                    )}
                  </div>
                  <span className="text-xs text-[var(--muted)]">
                    {service.baseUrl}
                  </span>
                </div>

                <div className="flex items-center gap-3">
                  <Badge
                    variant={active ? "admin" : "member"}
                    label={active ? "Attivo" : "Inattivo"}
                  />
                  <Button
                    variant={active ? "ghost" : "primary"}
                    onClick={() => handleToggle(service)}
                    disabled={isKinConsole || toggleMutation.isPending}
                  >
                    {isKinConsole
                      ? "Sempre attivo"
                      : active
                        ? "Disattiva"
                        : "Attiva"}
                  </Button>
                </div>
              </div>
            </Card>
          );
        })}
      </div>
    </div>
  );
}
