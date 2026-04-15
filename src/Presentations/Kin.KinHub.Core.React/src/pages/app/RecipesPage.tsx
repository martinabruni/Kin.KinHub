import { useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import { Card } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Spinner } from "@/components/ui/Spinner";
import { useGetRecipes } from "@/api/core/useGetRecipes";
import { useCreateRecipe } from "@/api/core/useCreateRecipe";
import { useUpdateRecipe } from "@/api/core/useUpdateRecipe";
import { useDeleteRecipe } from "@/api/core/useDeleteRecipe";
import { useUiStore } from "@/stores/uiStore";
import type { RecipeResponse } from "@/types/core";

export function RecipesPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { recipeBookId = "" } = useParams<{ recipeBookId: string }>();
  const { data: recipes, isLoading } = useGetRecipes(recipeBookId);
  const createRecipe = useCreateRecipe(recipeBookId);
  const deleteRecipe = useDeleteRecipe(recipeBookId);
  const showSnackbar = useUiStore((s) => s.showSnackbar);

  const [showForm, setShowForm] = useState(false);
  const [editingRecipe, setEditingRecipe] = useState<RecipeResponse | null>(
    null,
  );
  const [name, setName] = useState("");
  const [backstory, setBackstory] = useState("");
  const [finalTime, setFinalTime] = useState("00:30:00");
  const [portions, setPortions] = useState("4");
  const [formError, setFormError] = useState<string | null>(null);

  function openCreate() {
    setEditingRecipe(null);
    setName("");
    setBackstory("");
    setFinalTime("00:30:00");
    setPortions("4");
    setFormError(null);
    setShowForm(true);
  }

  function openEdit(recipe: RecipeResponse) {
    setEditingRecipe(recipe);
    setName(recipe.name);
    setBackstory(recipe.backstory ?? "");
    setFinalTime(recipe.finalTime);
    setPortions(String(recipe.portions));
    setFormError(null);
    setShowForm(true);
  }

  function handleClose() {
    setShowForm(false);
    setEditingRecipe(null);
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    createRecipe.mutate(
      {
        name: name.trim(),
        backstory: backstory.trim() || undefined,
        finalTime,
        portions: Number(portions),
        recipeBookId,
      },
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
          ← {t("app.recipes.back")}
        </Button>
        <h1 className="text-2xl font-bold text-[var(--fg)]">
          {t("app.recipes.title")}
        </h1>
        <div className="ml-auto">
          <Button onClick={openCreate}>{t("app.recipes.create")}</Button>
        </div>
      </div>

      {showForm && !editingRecipe && (
        <Card>
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            <Input
              label={t("app.recipes.name")}
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
            <Input
              label={t("app.recipes.backstory")}
              value={backstory}
              onChange={(e) => setBackstory(e.target.value)}
            />
            <div className="grid gap-4 sm:grid-cols-2">
              <Input
                label={t("app.recipes.finalTime")}
                value={finalTime}
                onChange={(e) => setFinalTime(e.target.value)}
                placeholder="00:30:00"
                required
              />
              <Input
                label={t("app.recipes.portions")}
                type="number"
                min={1}
                value={portions}
                onChange={(e) => setPortions(e.target.value)}
                required
              />
            </div>
            {formError && (
              <p className="text-sm text-[var(--danger)]">{formError}</p>
            )}
            <div className="flex gap-2">
              <Button type="submit" loading={createRecipe.isPending}>
                {t("app.recipes.save")}
              </Button>
              <Button type="button" variant="secondary" onClick={handleClose}>
                {t("app.recipes.cancel")}
              </Button>
            </div>
          </form>
        </Card>
      )}

      {!recipes?.length && !showForm && (
        <p className="text-[var(--muted)]">{t("app.recipes.empty")}</p>
      )}

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {recipes?.map((recipe) =>
          editingRecipe?.id === recipe.id ? (
            <EditRecipeForm
              key={recipe.id}
              recipe={recipe}
              recipeBookId={recipeBookId}
              onClose={handleClose}
            />
          ) : (
            <Card
              key={recipe.id}
              onClick={() =>
                navigate(`/recipe-books/${recipeBookId}/recipes/${recipe.id}`)
              }
            >
              <div className="flex flex-col gap-3">
                <div>
                  <h3 className="font-semibold text-[var(--fg)]">
                    {recipe.name}
                  </h3>
                  {recipe.backstory && (
                    <p className="mt-1 text-sm text-[var(--muted)] line-clamp-2">
                      {recipe.backstory}
                    </p>
                  )}
                  <p className="mt-1 text-sm text-[var(--muted)]">
                    {t("app.recipeDetail.portions")}: {recipe.portions} &middot;{" "}
                    {t("app.recipeDetail.cookingTime")}: {recipe.finalTime}
                  </p>
                </div>
                <div className="flex flex-wrap gap-2">
                  <Button
                    size="sm"
                    variant="secondary"
                    onClick={(e) => {
                      e.stopPropagation();
                      openEdit(recipe);
                    }}
                  >
                    {t("app.recipes.edit")}
                  </Button>
                  <Button
                    size="sm"
                    variant="danger"
                    loading={deleteRecipe.isPending}
                    onClick={(e) => {
                      e.stopPropagation();
                      deleteRecipe.mutate(recipe.id, {
                        onError: (err) =>
                          showSnackbar(err.message ?? t("errors.generic")),
                      });
                    }}
                  >
                    {t("app.recipes.delete")}
                  </Button>
                </div>
              </div>
            </Card>
          ),
        )}
      </div>
    </div>
  );
}

function EditRecipeForm({
  recipe,
  recipeBookId,
  onClose,
}: {
  recipe: RecipeResponse;
  recipeBookId: string;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const updateRecipe = useUpdateRecipe(recipeBookId, recipe.id);
  const [name, setName] = useState(recipe.name);
  const [backstory, setBackstory] = useState(recipe.backstory ?? "");
  const [finalTime, setFinalTime] = useState(recipe.finalTime);
  const [portions, setPortions] = useState(String(recipe.portions));
  const [error, setError] = useState<string | null>(null);

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    updateRecipe.mutate(
      {
        name: name.trim(),
        backstory: backstory.trim() || undefined,
        finalTime,
        portions: Number(portions),
      },
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
          label={t("app.recipes.name")}
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
        <Input
          label={t("app.recipes.backstory")}
          value={backstory}
          onChange={(e) => setBackstory(e.target.value)}
        />
        <div className="grid gap-4 sm:grid-cols-2">
          <Input
            label={t("app.recipes.finalTime")}
            value={finalTime}
            onChange={(e) => setFinalTime(e.target.value)}
            required
          />
          <Input
            label={t("app.recipes.portions")}
            type="number"
            min={1}
            value={portions}
            onChange={(e) => setPortions(e.target.value)}
            required
          />
        </div>
        {error && <p className="text-sm text-[var(--danger)]">{error}</p>}
        <div className="flex gap-2">
          <Button type="submit" loading={updateRecipe.isPending}>
            {t("app.recipes.save")}
          </Button>
          <Button type="button" variant="secondary" onClick={onClose}>
            {t("app.recipes.cancel")}
          </Button>
        </div>
      </form>
    </Card>
  );
}
