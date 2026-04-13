import { useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useGetServices } from "@/api/core/useGetServices";
import { Spinner } from "@/components/ui/Spinner";

export function ServicePage() {
  const { serviceId } = useParams<{ serviceId: string }>();
  const { t } = useTranslation();
  const { data: services, isLoading } = useGetServices();

  if (isLoading) {
    return (
      <div className="flex justify-center pt-16">
        <Spinner size="lg" className="text-[var(--accent)]" />
      </div>
    );
  }

  const service = services?.find((s) => String(s.id) === serviceId);

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-[var(--fg)]">
        {service?.name ?? serviceId}
      </h1>
      <p className="text-[var(--muted)]">{t("app.services.comingSoon")}</p>
    </div>
  );
}
