import { useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { useParams, useNavigate } from "react-router-dom";
import { Card } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Spinner } from "@/components/ui/Spinner";
import { useGetFridges } from "@/api/core/useGetFridges";
import { useCreateFridge } from "@/api/core/useCreateFridge";
import { useUpdateFridge } from "@/api/core/useUpdateFridge";
import { useDeleteFridge } from "@/api/core/useDeleteFridge";
import { useGetFridgeIngredients } from "@/api/core/useGetFridgeIngredients";
import { useCreateFridgeIngredient } from "@/api/core/useCreateFridgeIngredient";
import { useUpdateFridgeIngredient } from "@/api/core/useUpdateFridgeIngredient";
import { useDeleteFridgeIngredient } from "@/api/core/useDeleteFridgeIngredient";
import { useUiStore } from "@/stores/uiStore";
import type { FridgeResponse, FridgeIngredientResponse } from "@/types/core";

export function FridgePage() {
  const { t } = useTranslation();
  const { fridgeId: paramFridgeId } = useParams<{ fridgeId?: string }>();
  const navigate = useNavigate();
  const { data: fridges, isLoading } = useGetFridges();
  const createFridge = useCreateFridge();
  const deleteFridge = useDeleteFridge();
  const showSnackbar = useUiStore((s) => s.showSnackbar);

  const [showForm, setShowForm] = useState(false);
  const [editingFridge, setEditingFridge] = useState<FridgeResponse | null>(
    null,
  );
  const [selectedFridgeId, setSelectedFridgeId] = useState<string | null>(
    paramFridgeId ?? null,
  );
  const [name, setName] = useState("");
  const [formError, setFormError] = useState<string | null>(null);

  function openCreate() {
    setEditingFridge(null);
    setName("");
    setFormError(null);
    setShowForm(true);
  }

  function openEdit(fridge: FridgeResponse) {
    setEditingFridge(fridge);
    setName(fridge.name);
    setFormError(null);
    setShowForm(true);
  }

  function handleClose() {
    setShowForm(false);
    setEditingFridge(null);
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    createFridge.mutate(
      { name: name.trim() },
      {
        onSuccess: handleClose,
        onError: (err) => setFormError(err.message ?? t("errors.generic")),
      },
    );
  }

  if (isLoading) {
    return (
      <div className="flex justify-center pt-16">
        <Spinner size="lg" className="text-[var(--accent)]" />
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => navigate("/recipe-books")}
        >
          ← {t("app.fridges.back")}
        </Button>
        <h1 className="text-2xl font-bold text-[var(--fg)]">
          {t("app.fridges.title")}
        </h1>
        <Button className="ml-auto" onClick={openCreate}>
          {t("app.fridges.create")}
        </Button>
      </div>

      {showForm && !editingFridge && (
        <Card>
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            <Input
              label={t("app.fridges.name")}
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
            {formError && (
              <p className="text-sm text-[var(--danger)]">{formError}</p>
            )}
            <div className="flex gap-2">
              <Button type="submit" loading={createFridge.isPending}>
                {t("app.fridges.save")}
              </Button>
              <Button type="button" variant="secondary" onClick={handleClose}>
                {t("app.fridges.cancel")}
              </Button>
            </div>
          </form>
        </Card>
      )}

      {!fridges?.length && !showForm && (
        <p className="text-[var(--muted)]">{t("app.fridges.empty")}</p>
      )}

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {fridges?.map((fridge) =>
          editingFridge?.id === fridge.id ? (
            <EditFridgeForm
              key={fridge.id}
              fridge={fridge}
              onClose={handleClose}
            />
          ) : (
            <Card key={fridge.id}>
              <div className="flex flex-col gap-2">
                <h3 className="font-semibold text-[var(--fg)]">
                  {fridge.name}
                </h3>
                <div className="mt-2 flex gap-2">
                  <Button
                    size="sm"
                    onClick={() =>
                      setSelectedFridgeId(
                        selectedFridgeId === fridge.id ? null : fridge.id,
                      )
                    }
                  >
                    {t("app.fridges.viewIngredients")}
                  </Button>
                  <Button
                    size="sm"
                    variant="secondary"
                    onClick={() => openEdit(fridge)}
                  >
                    {t("app.fridges.edit")}
                  </Button>
                  <Button
                    size="sm"
                    variant="danger"
                    loading={deleteFridge.isPending}
                    onClick={() =>
                      deleteFridge.mutate(fridge.id, {
                        onError: (err) =>
                          showSnackbar(err.message ?? t("errors.generic")),
                      })
                    }
                  >
                    {t("app.fridges.delete")}
                  </Button>
                </div>
              </div>
            </Card>
          ),
        )}
      </div>

      {selectedFridgeId && (
        <FridgeIngredientPanel fridgeId={selectedFridgeId} />
      )}
    </div>
  );
}

function EditFridgeForm({
  fridge,
  onClose,
}: {
  fridge: FridgeResponse;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const updateFridge = useUpdateFridge(fridge.id);
  const [name, setName] = useState(fridge.name);
  const [error, setError] = useState<string | null>(null);

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    updateFridge.mutate(
      { name: name.trim() },
      {
        onSuccess: onClose,
        onError: (err) => setError(err.message ?? t("errors.generic")),
      },
    );
  }

  return (
    <Card>
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <Input
          label={t("app.fridges.name")}
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
        {error && <p className="text-sm text-[var(--danger)]">{error}</p>}
        <div className="flex gap-2">
          <Button type="submit" loading={updateFridge.isPending}>
            {t("app.fridges.save")}
          </Button>
          <Button type="button" variant="secondary" onClick={onClose}>
            {t("app.fridges.cancel")}
          </Button>
        </div>
      </form>
    </Card>
  );
}

function FridgeIngredientPanel({ fridgeId }: { fridgeId: string }) {
  const { t } = useTranslation();
  const { data: ingredients, isLoading } = useGetFridgeIngredients(fridgeId);
  const createIngredient = useCreateFridgeIngredient(fridgeId);
  const deleteIngredient = useDeleteFridgeIngredient(fridgeId);
  const showSnackbar = useUiStore((s) => s.showSnackbar);

  const [showForm, setShowForm] = useState(false);
  const [editingIngredient, setEditingIngredient] =
    useState<FridgeIngredientResponse | null>(null);
  const [name, setName] = useState("");
  const [unit, setUnit] = useState("");
  const [qty, setQty] = useState("1");
  const [formError, setFormError] = useState<string | null>(null);

  function handleCreateSubmit(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    createIngredient.mutate(
      {
        name: name.trim(),
        measureUnit: unit.trim(),
        quantity: Number(qty),
        fridgeId,
      },
      {
        onSuccess: () => {
          setName("");
          setUnit("");
          setQty("1");
          setShowForm(false);
        },
        onError: (err) => setFormError(err.message ?? t("errors.generic")),
      },
    );
  }

  if (isLoading) {
    return (
      <div className="flex justify-center py-4">
        <Spinner className="text-[var(--accent)]" />
      </div>
    );
  }

  return (
    <Card
      title={t("app.fridges.ingredients")}
      footer={
        <Button size="sm" onClick={() => setShowForm(true)}>
          {t("app.fridges.addIngredient")}
        </Button>
      }
    >
      {showForm && !editingIngredient && (
        <form
          onSubmit={handleCreateSubmit}
          className="mb-4 flex flex-col gap-3"
        >
          <div className="grid grid-cols-3 gap-2">
            <Input
              label={t("app.fridges.ingredientName")}
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
            <Input
              label={t("app.fridges.measureUnit")}
              value={unit}
              onChange={(e) => setUnit(e.target.value)}
              required
            />
            <Input
              label={t("app.fridges.quantity")}
              type="number"
              min={0}
              step="0.01"
              value={qty}
              onChange={(e) => setQty(e.target.value)}
              required
            />
          </div>
          {formError && (
            <p className="text-sm text-[var(--danger)]">{formError}</p>
          )}
          <div className="flex gap-2">
            <Button
              type="submit"
              size="sm"
              loading={createIngredient.isPending}
            >
              {t("app.fridges.save")}
            </Button>
            <Button
              type="button"
              size="sm"
              variant="secondary"
              onClick={() => setShowForm(false)}
            >
              {t("app.fridges.cancel")}
            </Button>
          </div>
        </form>
      )}

      {!ingredients?.length && !showForm && (
        <p className="text-sm text-[var(--muted)]">—</p>
      )}

      <ul className="flex flex-col gap-2">
        {ingredients?.map((ing) =>
          editingIngredient?.id === ing.id ? (
            <EditFridgeIngredientForm
              key={ing.id}
              ingredient={ing}
              fridgeId={fridgeId}
              onClose={() => setEditingIngredient(null)}
            />
          ) : (
            <li
              key={ing.id}
              className="flex items-center justify-between gap-2 rounded-md border border-[var(--border)] px-3 py-2 text-sm"
            >
              <span className="text-[var(--fg)]">
                {ing.name}{" "}
                <span className="text-[var(--muted)]">
                  {ing.quantity} {ing.measureUnit}
                </span>
              </span>
              <div className="flex gap-1">
                <Button
                  size="sm"
                  variant="ghost"
                  onClick={() => setEditingIngredient(ing)}
                >
                  {t("app.fridges.edit")}
                </Button>
                <Button
                  size="sm"
                  variant="danger"
                  loading={deleteIngredient.isPending}
                  onClick={() =>
                    deleteIngredient.mutate(ing.id, {
                      onError: (err) =>
                        showSnackbar(err.message ?? t("errors.generic")),
                    })
                  }
                >
                  {t("app.fridges.delete")}
                </Button>
              </div>
            </li>
          ),
        )}
      </ul>
    </Card>
  );
}

function EditFridgeIngredientForm({
  ingredient,
  fridgeId,
  onClose,
}: {
  ingredient: FridgeIngredientResponse;
  fridgeId: string;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const updateIngredient = useUpdateFridgeIngredient(fridgeId, ingredient.id);
  const [name, setName] = useState(ingredient.name);
  const [unit, setUnit] = useState(ingredient.measureUnit);
  const [qty, setQty] = useState(String(ingredient.quantity));
  const [error, setError] = useState<string | null>(null);

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    updateIngredient.mutate(
      { name: name.trim(), measureUnit: unit.trim(), quantity: Number(qty) },
      {
        onSuccess: onClose,
        onError: (err) => setError(err.message ?? t("errors.generic")),
      },
    );
  }

  return (
    <li>
      <form onSubmit={handleSubmit} className="flex flex-col gap-3">
        <div className="grid grid-cols-3 gap-2">
          <Input
            label={t("app.fridges.ingredientName")}
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
          <Input
            label={t("app.fridges.measureUnit")}
            value={unit}
            onChange={(e) => setUnit(e.target.value)}
            required
          />
          <Input
            label={t("app.fridges.quantity")}
            type="number"
            min={0}
            step="0.01"
            value={qty}
            onChange={(e) => setQty(e.target.value)}
            required
          />
        </div>
        {error && <p className="text-sm text-[var(--danger)]">{error}</p>}
        <div className="flex gap-2">
          <Button type="submit" size="sm" loading={updateIngredient.isPending}>
            {t("app.fridges.save")}
          </Button>
          <Button type="button" size="sm" variant="secondary" onClick={onClose}>
            {t("app.fridges.cancel")}
          </Button>
        </div>
      </form>
    </li>
  );
}
