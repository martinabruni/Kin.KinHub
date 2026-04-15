import { useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { Card } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Spinner } from "@/components/ui/Spinner";
import { useGetRecipeBooks } from "@/api/core/useGetRecipeBooks";
import { useCreateRecipeBook } from "@/api/core/useCreateRecipeBook";
import { useUpdateRecipeBook } from "@/api/core/useUpdateRecipeBook";
import { useDeleteRecipeBook } from "@/api/core/useDeleteRecipeBook";
import { useGetFridges } from "@/api/core/useGetFridges";
import { useCreateFridge } from "@/api/core/useCreateFridge";
import { useDeleteFridge } from "@/api/core/useDeleteFridge";
import { useUiStore } from "@/stores/uiStore";
import type { RecipeBookResponse } from "@/types/core";

export function RecipeBooksPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { data: books, isLoading } = useGetRecipeBooks();
  const createBook = useCreateRecipeBook();
  const deleteBook = useDeleteRecipeBook();
  const { data: fridges, isLoading: fridgesLoading } = useGetFridges();
  const createFridge = useCreateFridge();
  const deleteFridge = useDeleteFridge();
  const showSnackbar = useUiStore((s) => s.showSnackbar);

  const [showForm, setShowForm] = useState(false);
  const [editingBook, setEditingBook] = useState<RecipeBookResponse | null>(
    null,
  );
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [formError, setFormError] = useState<string | null>(null);

  // Fridge state
  const [showFridgeForm, setShowFridgeForm] = useState(false);
  const [fridgeName, setFridgeName] = useState("");
  const [fridgeFormError, setFridgeFormError] = useState<string | null>(null);

  function openCreate() {
    setEditingBook(null);
    setName("");
    setDescription("");
    setFormError(null);
    setShowForm(true);
  }

  function openEdit(book: RecipeBookResponse) {
    setEditingBook(book);
    setName(book.name);
    setDescription(book.description ?? "");
    setFormError(null);
    setShowForm(true);
  }

  function handleClose() {
    setShowForm(false);
    setEditingBook(null);
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setFormError(null);

    if (editingBook) {
      // handled inline in EditBookForm to pass the id
    } else {
      createBook.mutate(
        { name: name.trim(), description: description.trim() || undefined },
        {
          onSuccess: () => {
            handleClose();
          },
          onError: (err) => {
            setFormError(err.message ?? t("errors.generic"));
          },
        },
      );
    }
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
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-[var(--fg)]">
          {t("app.recipeBooks.title")}
        </h1>
        <Button onClick={openCreate}>{t("app.recipeBooks.create")}</Button>
      </div>

      {showForm && !editingBook && (
        <Card>
          <form onSubmit={handleSubmit} className="flex flex-col gap-4">
            <Input
              label={t("app.recipeBooks.name")}
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
            <Input
              label={t("app.recipeBooks.description")}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
            {formError && (
              <p className="text-sm text-[var(--danger)]">{formError}</p>
            )}
            <div className="flex gap-2">
              <Button type="submit" loading={createBook.isPending}>
                {t("app.recipeBooks.save")}
              </Button>
              <Button type="button" variant="secondary" onClick={handleClose}>
                {t("app.recipeBooks.cancel")}
              </Button>
            </div>
          </form>
        </Card>
      )}

      {!books?.length && !showForm && (
        <p className="text-[var(--muted)]">{t("app.recipeBooks.empty")}</p>
      )}

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {books?.map((book) =>
          editingBook?.id === book.id ? (
            <EditBookForm key={book.id} book={book} onClose={handleClose} />
          ) : (
            <Card key={book.id}>
              <div className="flex flex-col gap-2">
                <h3 className="font-semibold text-[var(--fg)]">{book.name}</h3>
                {book.description && (
                  <p className="text-sm text-[var(--muted)]">
                    {book.description}
                  </p>
                )}
                <div className="mt-2 flex gap-2">
                  <Button
                    size="sm"
                    onClick={() => navigate(`/recipe-books/${book.id}/recipes`)}
                  >
                    {t("app.recipeBooks.viewRecipes")}
                  </Button>
                  <Button
                    size="sm"
                    variant="secondary"
                    onClick={() => openEdit(book)}
                  >
                    {t("app.recipeBooks.edit")}
                  </Button>
                  <Button
                    size="sm"
                    variant="danger"
                    loading={deleteBook.isPending}
                    onClick={() =>
                      deleteBook.mutate(book.id, {
                        onError: (err) =>
                          showSnackbar(err.message ?? t("errors.generic")),
                      })
                    }
                  >
                    {t("app.recipeBooks.delete")}
                  </Button>
                </div>
              </div>
            </Card>
          ),
        )}
      </div>

      {/* Fridges section */}
      <div className="border-t border-[var(--border)] pt-6">
        <div className="flex items-center justify-between">
          <h2 className="text-xl font-bold text-[var(--fg)]">
            {t("app.fridges.title")}
          </h2>
          <Button
            size="sm"
            onClick={() => {
              setFridgeName("");
              setFridgeFormError(null);
              setShowFridgeForm(true);
            }}
          >
            {t("app.fridges.create")}
          </Button>
        </div>

        {showFridgeForm && (
          <Card className="mt-4">
            <form
              onSubmit={(e) => {
                e.preventDefault();
                setFridgeFormError(null);
                createFridge.mutate(
                  { name: fridgeName.trim() },
                  {
                    onSuccess: () => {
                      setShowFridgeForm(false);
                      setFridgeName("");
                    },
                    onError: (err) =>
                      setFridgeFormError(err.message ?? t("errors.generic")),
                  },
                );
              }}
              className="flex flex-col gap-4"
            >
              <Input
                label={t("app.fridges.name")}
                value={fridgeName}
                onChange={(e) => setFridgeName(e.target.value)}
                required
              />
              {fridgeFormError && (
                <p className="text-sm text-[var(--danger)]">
                  {fridgeFormError}
                </p>
              )}
              <div className="flex gap-2">
                <Button type="submit" loading={createFridge.isPending}>
                  {t("app.fridges.save")}
                </Button>
                <Button
                  type="button"
                  variant="secondary"
                  onClick={() => setShowFridgeForm(false)}
                >
                  {t("app.fridges.cancel")}
                </Button>
              </div>
            </form>
          </Card>
        )}

        {fridgesLoading ? (
          <div className="flex justify-center py-6">
            <Spinner className="text-[var(--accent)]" />
          </div>
        ) : !fridges?.length && !showFridgeForm ? (
          <p className="mt-3 text-[var(--muted)]">{t("app.fridges.empty")}</p>
        ) : (
          <div className="mt-4 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {fridges?.map((fridge) => (
              <Card key={fridge.id}>
                <div className="flex flex-col gap-2">
                  <h3 className="font-semibold text-[var(--fg)]">
                    {fridge.name}
                  </h3>
                  <div className="mt-2 flex gap-2">
                    <Button
                      size="sm"
                      onClick={() => navigate(`/fridges/${fridge.id}`)}
                    >
                      {t("app.fridges.viewIngredients")}
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
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

function EditBookForm({
  book,
  onClose,
}: {
  book: RecipeBookResponse;
  onClose: () => void;
}) {
  const { t } = useTranslation();
  const updateBook = useUpdateRecipeBook(book.id);
  const [name, setName] = useState(book.name);
  const [description, setDescription] = useState(book.description ?? "");
  const [error, setError] = useState<string | null>(null);

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    updateBook.mutate(
      { name: name.trim(), description: description.trim() || undefined },
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
          label={t("app.recipeBooks.name")}
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
        <Input
          label={t("app.recipeBooks.description")}
          value={description}
          onChange={(e) => setDescription(e.target.value)}
        />
        {error && <p className="text-sm text-[var(--danger)]">{error}</p>}
        <div className="flex gap-2">
          <Button type="submit" loading={updateBook.isPending}>
            {t("app.recipeBooks.save")}
          </Button>
          <Button type="button" variant="secondary" onClick={onClose}>
            {t("app.recipeBooks.cancel")}
          </Button>
        </div>
      </form>
    </Card>
  );
}
