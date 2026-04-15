import { useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import { Card } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Spinner } from "@/components/ui/Spinner";
import { useGetRecipes } from "@/api/core/useGetRecipes";
import { useCreateRecipeIngredient } from "@/api/core/useCreateRecipeIngredient";
import { useUpdateRecipeIngredient } from "@/api/core/useUpdateRecipeIngredient";
import { useDeleteRecipeIngredient } from "@/api/core/useDeleteRecipeIngredient";
import { useCreateRecipeStep } from "@/api/core/useCreateRecipeStep";
import { useUpdateRecipeStep } from "@/api/core/useUpdateRecipeStep";
import { useDeleteRecipeStep } from "@/api/core/useDeleteRecipeStep";
import { useGetMissingIngredients } from "@/api/core/useGetMissingIngredients";
import { useGetFridges } from "@/api/core/useGetFridges";
import { useUiStore } from "@/stores/uiStore";
import type {
  RecipeIngredientResponse,
  RecipeStepResponse,
} from "@/types/core";

export function RecipeDetailPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { recipeBookId = "", recipeId = "" } = useParams<{
    recipeBookId: string;
    recipeId: string;
  }>();

  const { data: recipes, isLoading } = useGetRecipes(recipeBookId);
  const recipe = recipes?.find((r) => r.id === recipeId);

  const { data: fridges } = useGetFridges();
  const [selectedFridgeId, setSelectedFridgeId] = useState("");
  const [missingResult, setMissingResult] = useState<string[] | null>(null);
  const getMissing = useGetMissingIngredients(recipeBookId, recipeId);
  const showSnackbar = useUiStore((s) => s.showSnackbar);

  // Ingredient form state
  const [showIngredientForm, setShowIngredientForm] = useState(false);
  const [editingIngredient, setEditingIngredient] =
    useState<RecipeIngredientResponse | null>(null);

  // Step form state
  const [showStepForm, setShowStepForm] = useState(false);
  const [editingStep, setEditingStep] = useState<RecipeStepResponse | null>(
    null,
  );

  const deleteIngredient = useDeleteRecipeIngredient(recipeBookId, recipeId);
  const deleteStep = useDeleteRecipeStep(recipeBookId, recipeId);

  function handleCheckMissing() {
    if (!selectedFridgeId) return;
    getMissing.mutate(selectedFridgeId, {
      onSuccess: (data) => setMissingResult(data.missingIngredients),
      onError: (err) => showSnackbar(err.message ?? t("errors.generic")),
    });
  }

  if (isLoading) {
    return (
      <div className="flex justify-center pt-16">
        <Spinner size="lg" className="text-[var(--accent)]" />
      </div>
    );
  }

  if (!recipe) {
    return (
      <div className="flex flex-col gap-4">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => navigate(`/recipe-books/${recipeBookId}/recipes`)}
          className="self-start"
        >
          ← {t("app.recipeDetail.back")}
        </Button>
        <p className="text-[var(--muted)]">{t("errors.notFound")}</p>
      </div>
    );
  }

  const sortedSteps = [...recipe.steps].sort((a, b) => a.order - b.order);

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => navigate(`/recipe-books/${recipeBookId}/recipes`)}
        >
          ← {t("app.recipeDetail.back")}
        </Button>
        <h1 className="text-2xl font-bold text-[var(--fg)]">{recipe.name}</h1>
      </div>

      {recipe.backstory && (
        <p className="text-[var(--muted)]">{recipe.backstory}</p>
      )}

      <div className="flex gap-6 text-sm text-[var(--muted)]">
        <span>
          {t("app.recipeDetail.portions")}: <strong>{recipe.portions}</strong>
        </span>
        <span>
          {t("app.recipeDetail.cookingTime")}:{" "}
          <strong>{recipe.finalTime}</strong>
        </span>
      </div>

      {/* Missing ingredients check */}
      {!!fridges?.length && (
        <Card title={t("app.recipeDetail.missingIngredients")}>
          <div className="flex flex-col gap-3">
            <div className="flex gap-2">
              <select
                value={selectedFridgeId}
                onChange={(e) => {
                  setSelectedFridgeId(e.target.value);
                  setMissingResult(null);
                }}
                className="rounded-md border border-[var(--border)] bg-[var(--card)] px-3 py-2 text-sm text-[var(--fg)] focus:outline-none focus:ring-2 focus:ring-[var(--accent)]"
              >
                <option value="">{t("app.recipeDetail.selectFridge")}</option>
                {fridges.map((f) => (
                  <option key={f.id} value={f.id}>
                    {f.name}
                  </option>
                ))}
              </select>
              <Button
                size="sm"
                disabled={!selectedFridgeId}
                loading={getMissing.isPending}
                onClick={handleCheckMissing}
              >
                {t("app.recipeDetail.check")}
              </Button>
            </div>
            {missingResult !== null && (
              <p className="text-sm">
                {missingResult.length === 0 ? (
                  <span className="text-green-600">
                    {t("app.recipeDetail.noMissing")}
                  </span>
                ) : (
                  <>
                    <span className="font-medium text-[var(--danger)]">
                      {t("app.recipeDetail.missing")}{" "}
                    </span>
                    {missingResult.join(", ")}
                  </>
                )}
              </p>
            )}
          </div>
        </Card>
      )}

      {/* Ingredients */}
      <Card
        title={t("app.recipeDetail.ingredients")}
        footer={
          <Button size="sm" onClick={() => setShowIngredientForm(true)}>
            {t("app.recipeDetail.addIngredient")}
          </Button>
        }
      >
        {showIngredientForm && !editingIngredient && (
          <AddIngredientForm
            recipeBookId={recipeBookId}
            recipeId={recipeId}
            onClose={() => setShowIngredientForm(false)}
          />
        )}
        {recipe.ingredients.length === 0 && !showIngredientForm && (
          <p className="text-sm text-[var(--muted)]">—</p>
        )}
        <ul className="flex flex-col gap-2">
          {recipe.ingredients.map((ing) =>
            editingIngredient?.id === ing.id ? (
              <EditIngredientForm
                key={ing.id}
                ingredient={ing}
                recipeBookId={recipeBookId}
                recipeId={recipeId}
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
                    {t("app.recipeDetail.edit")}
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
                    {t("app.recipeDetail.delete")}
                  </Button>
                </div>
              </li>
            ),
          )}
        </ul>
      </Card>

      {/* Steps */}
      <Card
        title={t("app.recipeDetail.steps")}
        footer={
          <Button size="sm" onClick={() => setShowStepForm(true)}>
            {t("app.recipeDetail.addStep")}
          </Button>
        }
      >
        {showStepForm && !editingStep && (
          <AddStepForm
            recipeBookId={recipeBookId}
            recipeId={recipeId}
            nextOrder={sortedSteps.length + 1}
            onClose={() => setShowStepForm(false)}
          />
        )}
        {sortedSteps.length === 0 && !showStepForm && (
          <p className="text-sm text-[var(--muted)]">—</p>
        )}
        <ol className="flex flex-col gap-2">
          {sortedSteps.map((step) =>
            editingStep?.id === step.id ? (
              <EditStepForm
                key={step.id}
                step={step}
                recipeBookId={recipeBookId}
                recipeId={recipeId}
                onClose={() => setEditingStep(null)}
              />
            ) : (
              <li
                key={step.id}
                className="flex items-start justify-between gap-2 rounded-md border border-[var(--border)] px-3 py-2 text-sm"
              >
                <span className="text-[var(--fg)]">
                  <strong className="mr-2 text-[var(--accent)]">
                    {step.order}.
                  </strong>
                  {step.description}
                </span>
                <div className="flex shrink-0 gap-1">
                  <Button
                    size="sm"
                    variant="ghost"
                    onClick={() => setEditingStep(step)}
                  >
                    {t("app.recipeDetail.edit")}
                  </Button>
                  <Button
                    size="sm"
                    variant="danger"
                    loading={deleteStep.isPending}
                    onClick={() =>
                      deleteStep.mutate(step.id, {
                        onError: (err) =>
                          showSnackbar(err.message ?? t("errors.generic")),
                      })
                    }
                  >
                    {t("app.recipeDetail.delete")}
                  </Button>
                </div>
              </li>
            ),
          )}
        </ol>
      </Card>
    </div>
  );
}

function AddIngredientForm({
  recipeBookId,
  recipeId,
  onClose,
}: {
  recipeBookId: string;
  recipeId: string;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const createIngredient = useCreateRecipeIngredient(recipeBookId, recipeId);
  const [name, setName] = useState("");
  const [unit, setUnit] = useState("");
  const [qty, setQty] = useState("1");
  const [error, setError] = useState<string | null>(null);

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    createIngredient.mutate(
      {
        name: name.trim(),
        measureUnit: unit.trim(),
        quantity: Number(qty),
        recipeId,
      },
      {
        onSuccess: onClose,
        onError: (err) => setError(err.message ?? t("errors.generic")),
      },
    );
  }

  return (
    <form onSubmit={handleSubmit} className="mb-4 flex flex-col gap-3">
      <div className="grid grid-cols-3 gap-2">
        <Input
          label={t("app.recipeDetail.ingredientName")}
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
        <Input
          label={t("app.recipeDetail.measureUnit")}
          value={unit}
          onChange={(e) => setUnit(e.target.value)}
          required
        />
        <Input
          label={t("app.recipeDetail.quantity")}
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
        <Button type="submit" size="sm" loading={createIngredient.isPending}>
          {t("app.recipeDetail.save")}
        </Button>
        <Button type="button" size="sm" variant="secondary" onClick={onClose}>
          {t("app.recipeDetail.cancel")}
        </Button>
      </div>
    </form>
  );
}

function EditIngredientForm({
  ingredient,
  recipeBookId,
  recipeId,
  onClose,
}: {
  ingredient: RecipeIngredientResponse;
  recipeBookId: string;
  recipeId: string;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const updateIngredient = useUpdateRecipeIngredient(
    recipeBookId,
    recipeId,
    ingredient.id,
  );
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
    <li className="mb-2">
      <form onSubmit={handleSubmit} className="flex flex-col gap-3">
        <div className="grid grid-cols-3 gap-2">
          <Input
            label={t("app.recipeDetail.ingredientName")}
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
          <Input
            label={t("app.recipeDetail.measureUnit")}
            value={unit}
            onChange={(e) => setUnit(e.target.value)}
            required
          />
          <Input
            label={t("app.recipeDetail.quantity")}
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
            {t("app.recipeDetail.save")}
          </Button>
          <Button type="button" size="sm" variant="secondary" onClick={onClose}>
            {t("app.recipeDetail.cancel")}
          </Button>
        </div>
      </form>
    </li>
  );
}

function AddStepForm({
  recipeBookId,
  recipeId,
  nextOrder,
  onClose,
}: {
  recipeBookId: string;
  recipeId: string;
  nextOrder: number;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const createStep = useCreateRecipeStep(recipeBookId, recipeId);
  const [order, setOrder] = useState(String(nextOrder));
  const [description, setDescription] = useState("");
  const [error, setError] = useState<string | null>(null);

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    createStep.mutate(
      { order: Number(order), description: description.trim(), recipeId },
      {
        onSuccess: onClose,
        onError: (err) => setError(err.message ?? t("errors.generic")),
      },
    );
  }

  return (
    <form onSubmit={handleSubmit} className="mb-4 flex flex-col gap-3">
      <div className="grid grid-cols-4 gap-2">
        <Input
          label={t("app.recipeDetail.stepOrder")}
          type="number"
          min={1}
          value={order}
          onChange={(e) => setOrder(e.target.value)}
          required
          className="col-span-1"
        />
        <Input
          label={t("app.recipeDetail.stepDescription")}
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          required
          className="col-span-3"
        />
      </div>
      {error && <p className="text-sm text-[var(--danger)]">{error}</p>}
      <div className="flex gap-2">
        <Button type="submit" size="sm" loading={createStep.isPending}>
          {t("app.recipeDetail.save")}
        </Button>
        <Button type="button" size="sm" variant="secondary" onClick={onClose}>
          {t("app.recipeDetail.cancel")}
        </Button>
      </div>
    </form>
  );
}

function EditStepForm({
  step,
  recipeBookId,
  recipeId,
  onClose,
}: {
  step: RecipeStepResponse;
  recipeBookId: string;
  recipeId: string;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const updateStep = useUpdateRecipeStep(recipeBookId, recipeId, step.id);
  const [order, setOrder] = useState(String(step.order));
  const [description, setDescription] = useState(step.description);
  const [error, setError] = useState<string | null>(null);

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    updateStep.mutate(
      { order: Number(order), description: description.trim() },
      {
        onSuccess: onClose,
        onError: (err) => setError(err.message ?? t("errors.generic")),
      },
    );
  }

  return (
    <li>
      <form onSubmit={handleSubmit} className="flex flex-col gap-3">
        <div className="grid grid-cols-4 gap-2">
          <Input
            label={t("app.recipeDetail.stepOrder")}
            type="number"
            min={1}
            value={order}
            onChange={(e) => setOrder(e.target.value)}
            required
            className="col-span-1"
          />
          <Input
            label={t("app.recipeDetail.stepDescription")}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
            className="col-span-3"
          />
        </div>
        {error && <p className="text-sm text-[var(--danger)]">{error}</p>}
        <div className="flex gap-2">
          <Button type="submit" size="sm" loading={updateStep.isPending}>
            {t("app.recipeDetail.save")}
          </Button>
          <Button type="button" size="sm" variant="secondary" onClick={onClose}>
            {t("app.recipeDetail.cancel")}
          </Button>
        </div>
      </form>
    </li>
  );
}
