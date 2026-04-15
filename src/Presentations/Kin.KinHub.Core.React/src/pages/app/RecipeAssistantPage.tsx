import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Card } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { Spinner } from "@/components/ui/Spinner";
import { useGetFridges } from "@/api/core/useGetFridges";
import { useGetRecipeBooks } from "@/api/core/useGetRecipeBooks";
import { useGetRecipes } from "@/api/core/useGetRecipes";
import { useCreateRecipe } from "@/api/core/useCreateRecipe";
import { useSuggestRecipes } from "@/api/core/useSuggestRecipes";
import { useParseRecipe } from "@/api/core/useParseRecipe";
import { useAdaptRecipe } from "@/api/core/useAdaptRecipe";
import { useUiStore } from "@/stores/uiStore";
import { cn } from "@/lib/cn";
import type {
  RecipeSuggestion,
  RecipeAssistantRecipe,
  RecipeAdaptationResult,
} from "@/types/core";

type Tab = "suggest" | "parse" | "adapt";

export function RecipeAssistantPage() {
  const { t } = useTranslation();

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-[var(--fg)]">
        {t("app.recipeAssistant.title")}
      </h1>
      <RecipeAssistantPanel />
    </div>
  );
}

export function RecipeAssistantPanel() {
  const { t } = useTranslation();
  const [activeTab, setActiveTab] = useState<Tab>("suggest");

  return (
    <div className="flex flex-col gap-6">
      <div className="flex gap-1 border-b border-[var(--border)]">
        {(["suggest", "parse", "adapt"] as Tab[]).map((tab) => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            className={cn(
              "px-4 py-2 text-sm font-medium transition-colors",
              activeTab === tab
                ? "border-b-2 border-[var(--accent)] text-[var(--accent)]"
                : "text-[var(--muted)] hover:text-[var(--fg)]",
            )}
          >
            {t(`app.recipeAssistant.tabs.${tab}`)}
          </button>
        ))}
      </div>

      {activeTab === "suggest" && <SuggestTab />}
      {activeTab === "parse" && <ParseTab />}
      {activeTab === "adapt" && <AdaptTab />}
    </div>
  );
}

function SuggestTab() {
  const { t } = useTranslation();
  const showSnackbar = useUiStore((s) => s.showSnackbar);
  const { data: fridges, isLoading } = useGetFridges();
  const [selectedFridgeId, setSelectedFridgeId] = useState("");
  const [result, setResult] = useState<RecipeSuggestion[] | null>(null);
  const suggest = useSuggestRecipes();

  function handleSubmit() {
    if (!selectedFridgeId) return;
    suggest.mutate(
      { fridgeId: selectedFridgeId },
      {
        onSuccess: (data) => setResult(data ?? []),
        onError: (err) => showSnackbar(err.message ?? t("errors.generic")),
      },
    );
  }

  if (isLoading) {
    return (
      <div className="flex justify-center pt-8">
        <Spinner size="lg" className="text-[var(--accent)]" />
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-4">
      <Card>
        <div className="flex flex-col gap-4">
          <p className="text-sm text-[var(--muted)]">
            {t("app.recipeAssistant.suggest.hint")}
          </p>
          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium text-[var(--fg)]">
              {t("app.recipeAssistant.suggest.selectFridge")}
            </label>
            <select
              value={selectedFridgeId}
              onChange={(e) => {
                setSelectedFridgeId(e.target.value);
                setResult(null);
              }}
              className="rounded-md border border-[var(--border)] bg-[var(--card)] px-3 py-2 text-sm text-[var(--fg)] focus:outline-none focus:ring-2 focus:ring-[var(--accent)]"
            >
              <option value="">
                {t("app.recipeAssistant.suggest.placeholderFridge")}
              </option>
              {fridges?.map((f) => (
                <option key={f.id} value={f.id}>
                  {f.name}
                </option>
              ))}
            </select>
          </div>
          <Button
            onClick={handleSubmit}
            disabled={!selectedFridgeId}
            loading={suggest.isPending}
          >
            {t("app.recipeAssistant.suggest.submit")}
          </Button>
        </div>
      </Card>

      {result !== null && result.length === 0 && (
        <p className="text-sm text-[var(--muted)]">
          {t("app.recipeAssistant.suggest.noResults")}
        </p>
      )}

      {result?.map((s, i) => (
        <SuggestionCard key={i} suggestion={s} />
      ))}
    </div>
  );
}

function SuggestionCard({ suggestion }: { suggestion: RecipeSuggestion }) {
  const { t } = useTranslation();
  const [open, setOpen] = useState(false);

  return (
    <Card>
      <div className="flex items-center justify-between">
        <div>
          <h3 className="font-semibold text-[var(--fg)]">
            {suggestion.recipe.name}
          </h3>
          <p className="text-sm text-[var(--muted)]">
            {t("app.recipeAssistant.suggest.match", {
              pct: suggestion.matchPercentage,
            })}
            {" · "}
            {suggestion.recipe.portions} {t("app.recipeAssistant.portions")}
            {" · "}
            {suggestion.recipe.finalTime}
          </p>
        </div>
        <Button variant="ghost" size="sm" onClick={() => setOpen((o) => !o)}>
          {open ? t("app.recipeAssistant.hide") : t("app.recipeAssistant.show")}
        </Button>
      </div>

      {open && (
        <div className="mt-4 flex flex-col gap-4">
          {suggestion.recipe.backstory && (
            <p className="text-sm text-[var(--muted)] italic">
              {suggestion.recipe.backstory}
            </p>
          )}
          <RecipeDetail recipe={suggestion.recipe} />
          {suggestion.missingIngredients.length > 0 && (
            <div>
              <p className="text-sm font-medium text-[var(--danger)]">
                {t("app.recipeAssistant.suggest.missing")}
              </p>
              <ul className="mt-1 list-disc pl-4 text-sm text-[var(--fg)]">
                {suggestion.missingIngredients.map((ing, i) => (
                  <li key={i}>
                    {ing.name} — {ing.quantity} {ing.unit}
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
    </Card>
  );
}

function ParseTab() {
  const { t } = useTranslation();
  const showSnackbar = useUiStore((s) => s.showSnackbar);
  const [rawText, setRawText] = useState("");
  const [result, setResult] = useState<
    RecipeAssistantRecipe | null | undefined
  >(undefined);
  const parse = useParseRecipe();

  function handleSubmit() {
    if (!rawText.trim()) return;
    parse.mutate(
      { rawText: rawText.trim() },
      {
        onSuccess: (data) => setResult(data),
        onError: (err) => showSnackbar(err.message ?? t("errors.generic")),
      },
    );
  }

  return (
    <div className="flex flex-col gap-4">
      <Card>
        <div className="flex flex-col gap-4">
          <p className="text-sm text-[var(--muted)]">
            {t("app.recipeAssistant.parse.hint")}
          </p>
          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium text-[var(--fg)]">
              {t("app.recipeAssistant.parse.label")}
            </label>
            <textarea
              value={rawText}
              onChange={(e) => {
                setRawText(e.target.value);
                setResult(undefined);
              }}
              rows={6}
              placeholder={t("app.recipeAssistant.parse.placeholder")}
              className="rounded-md border border-[var(--border)] bg-[var(--card)] px-3 py-2 text-sm text-[var(--fg)] placeholder:text-[var(--muted)] focus:outline-none focus:ring-2 focus:ring-[var(--accent)] resize-none"
            />
          </div>
          <Button
            onClick={handleSubmit}
            disabled={!rawText.trim()}
            loading={parse.isPending}
          >
            {t("app.recipeAssistant.parse.submit")}
          </Button>
        </div>
      </Card>

      {result === null && (
        <p className="text-sm text-[var(--muted)]">
          {t("app.recipeAssistant.parse.noResult")}
        </p>
      )}

      {result != null && (
        <div className="flex flex-col gap-4">
          <Card title={result.name}>
            {result.backstory && (
              <p className="mb-4 text-sm text-[var(--muted)] italic">
                {result.backstory}
              </p>
            )}
            <RecipeDetail recipe={result} />
          </Card>
          <SaveParsedRecipePanel
            recipe={result}
            onSaved={() => setResult(undefined)}
          />
        </div>
      )}
    </div>
  );
}

function SaveParsedRecipePanel({
  recipe,
  onSaved,
}: {
  recipe: RecipeAssistantRecipe;
  onSaved: () => void;
}) {
  const { t } = useTranslation();
  const showSnackbar = useUiStore((s) => s.showSnackbar);
  const { data: books } = useGetRecipeBooks();
  const [bookId, setBookId] = useState("");
  const createRecipe = useCreateRecipe(bookId);

  function handleSave() {
    if (!bookId) return;
    createRecipe.mutate(
      {
        name: recipe.name,
        backstory: recipe.backstory,
        finalTime: recipe.finalTime,
        portions: recipe.portions,
        recipeBookId: bookId,
      },
      {
        onSuccess: () => {
          showSnackbar(t("app.recipeAssistant.parse.createSuccess"));
          onSaved();
        },
        onError: (err) => showSnackbar(err.message ?? t("errors.generic")),
      },
    );
  }

  return (
    <Card title={t("app.recipeAssistant.parse.saveTitle")}>
      <div className="flex flex-col gap-3">
        <div className="flex flex-col gap-1">
          <label className="text-sm font-medium text-[var(--fg)]">
            {t("app.recipeAssistant.parse.selectBook")}
          </label>
          <select
            value={bookId}
            onChange={(e) => setBookId(e.target.value)}
            className="rounded-md border border-[var(--border)] bg-[var(--card)] px-3 py-2 text-sm text-[var(--fg)] focus:outline-none focus:ring-2 focus:ring-[var(--accent)]"
          >
            <option value="">
              {t("app.recipeAssistant.parse.placeholderBook")}
            </option>
            {books?.map((b) => (
              <option key={b.id} value={b.id}>
                {b.name}
              </option>
            ))}
          </select>
        </div>
        <Button
          onClick={handleSave}
          disabled={!bookId}
          loading={createRecipe.isPending}
        >
          {t("app.recipeAssistant.parse.createRecipe")}
        </Button>
      </div>
    </Card>
  );
}

function AdaptTab() {
  const { t } = useTranslation();
  const showSnackbar = useUiStore((s) => s.showSnackbar);
  const { data: recipeBooks } = useGetRecipeBooks();
  const [selectedBookId, setSelectedBookId] = useState("");
  const { data: recipes } = useGetRecipes(selectedBookId);
  const [selectedRecipeId, setSelectedRecipeId] = useState("");
  const [constraintInput, setConstraintInput] = useState("");
  const [constraints, setConstraints] = useState<string[]>([]);
  const [result, setResult] = useState<RecipeAdaptationResult | null>(null);
  const adapt = useAdaptRecipe();

  function addConstraint() {
    const trimmed = constraintInput.trim();
    if (!trimmed || constraints.includes(trimmed)) return;
    setConstraints((prev) => [...prev, trimmed]);
    setConstraintInput("");
  }

  function removeConstraint(c: string) {
    setConstraints((prev) => prev.filter((x) => x !== c));
  }

  function handleSubmit() {
    if (!selectedRecipeId || constraints.length === 0) return;
    adapt.mutate(
      { recipeId: selectedRecipeId, constraints },
      {
        onSuccess: (data) => setResult(data),
        onError: (err) => showSnackbar(err.message ?? t("errors.generic")),
      },
    );
  }

  return (
    <div className="flex flex-col gap-4">
      <Card>
        <div className="flex flex-col gap-4">
          <p className="text-sm text-[var(--muted)]">
            {t("app.recipeAssistant.adapt.hint")}
          </p>

          <div className="flex flex-col gap-1">
            <label className="text-sm font-medium text-[var(--fg)]">
              {t("app.recipeAssistant.adapt.selectBook")}
            </label>
            <select
              value={selectedBookId}
              onChange={(e) => {
                setSelectedBookId(e.target.value);
                setSelectedRecipeId("");
                setResult(null);
              }}
              className="rounded-md border border-[var(--border)] bg-[var(--card)] px-3 py-2 text-sm text-[var(--fg)] focus:outline-none focus:ring-2 focus:ring-[var(--accent)]"
            >
              <option value="">
                {t("app.recipeAssistant.adapt.placeholderBook")}
              </option>
              {recipeBooks?.map((rb) => (
                <option key={rb.id} value={rb.id}>
                  {rb.name}
                </option>
              ))}
            </select>
          </div>

          {selectedBookId && (
            <div className="flex flex-col gap-1">
              <label className="text-sm font-medium text-[var(--fg)]">
                {t("app.recipeAssistant.adapt.selectRecipe")}
              </label>
              <select
                value={selectedRecipeId}
                onChange={(e) => {
                  setSelectedRecipeId(e.target.value);
                  setResult(null);
                }}
                className="rounded-md border border-[var(--border)] bg-[var(--card)] px-3 py-2 text-sm text-[var(--fg)] focus:outline-none focus:ring-2 focus:ring-[var(--accent)]"
              >
                <option value="">
                  {t("app.recipeAssistant.adapt.placeholderRecipe")}
                </option>
                {recipes?.map((r) => (
                  <option key={r.id} value={r.id}>
                    {r.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          <div className="flex flex-col gap-2">
            <label className="text-sm font-medium text-[var(--fg)]">
              {t("app.recipeAssistant.adapt.constraints")}
            </label>
            <div className="flex gap-2">
              <input
                value={constraintInput}
                onChange={(e) => setConstraintInput(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    addConstraint();
                  }
                }}
                placeholder={t(
                  "app.recipeAssistant.adapt.constraintPlaceholder",
                )}
                className="flex-1 rounded-md border border-[var(--border)] bg-[var(--card)] px-3 py-2 text-sm text-[var(--fg)] placeholder:text-[var(--muted)] focus:outline-none focus:ring-2 focus:ring-[var(--accent)]"
              />
              <Button
                variant="secondary"
                size="sm"
                onClick={addConstraint}
                disabled={!constraintInput.trim()}
              >
                {t("app.recipeAssistant.adapt.addConstraint")}
              </Button>
            </div>
            {constraints.length > 0 && (
              <div className="flex flex-wrap gap-2">
                {constraints.map((c) => (
                  <span
                    key={c}
                    className="flex items-center gap-1 rounded-full bg-[var(--border)] px-3 py-1 text-xs text-[var(--fg)]"
                  >
                    {c}
                    <button
                      onClick={() => removeConstraint(c)}
                      className="text-[var(--muted)] hover:text-[var(--danger)]"
                      aria-label={`Remove ${c}`}
                    >
                      ×
                    </button>
                  </span>
                ))}
              </div>
            )}
          </div>

          <Button
            onClick={handleSubmit}
            disabled={!selectedRecipeId || constraints.length === 0}
            loading={adapt.isPending}
          >
            {t("app.recipeAssistant.adapt.submit")}
          </Button>
        </div>
      </Card>

      {result && (
        <div className="flex flex-col gap-4">
          <Card title={t("app.recipeAssistant.adapt.changes")}>
            <ul className="flex flex-col gap-1">
              {result.changes.map((c, i) => (
                <li key={i} className="text-sm text-[var(--fg)]">
                  <span className="font-medium">{c.type}:</span> {c.description}
                </li>
              ))}
            </ul>
          </Card>

          <div className="grid gap-4 md:grid-cols-2">
            <Card title={t("app.recipeAssistant.adapt.original")}>
              <RecipeDetail recipe={result.originalRecipe} />
            </Card>
            <Card title={t("app.recipeAssistant.adapt.adapted")}>
              <RecipeDetail recipe={result.adaptedRecipe} />
            </Card>
          </div>
        </div>
      )}
    </div>
  );
}

function RecipeDetail({ recipe }: { recipe: RecipeAssistantRecipe }) {
  const { t } = useTranslation();

  return (
    <div className="flex flex-col gap-4">
      <div className="flex gap-4 text-sm text-[var(--muted)]">
        <span>
          {t("app.recipeAssistant.portions")}: {recipe.portions}
        </span>
        <span>
          {t("app.recipeAssistant.cookingTime")}: {recipe.finalTime}
        </span>
      </div>

      <div>
        <p className="mb-2 text-sm font-medium text-[var(--fg)]">
          {t("app.recipeAssistant.ingredients")}
        </p>
        <ul className="flex flex-col gap-1">
          {recipe.ingredients.map((ing, i) => (
            <li key={i} className="text-sm text-[var(--fg)]">
              {ing.name} — {ing.quantity} {ing.unit}
            </li>
          ))}
        </ul>
      </div>

      <div>
        <p className="mb-2 text-sm font-medium text-[var(--fg)]">
          {t("app.recipeAssistant.steps")}
        </p>
        <ol className="flex flex-col gap-2">
          {[...recipe.steps]
            .sort((a, b) => a.order - b.order)
            .map((s) => (
              <li key={s.order} className="flex gap-2 text-sm text-[var(--fg)]">
                <span className="shrink-0 font-medium text-[var(--accent)]">
                  {s.order}.
                </span>
                <span>{s.description}</span>
              </li>
            ))}
        </ol>
      </div>
    </div>
  );
}
