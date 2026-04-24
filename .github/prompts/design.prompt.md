# KinHub – UI/UX Design Prompt

## Context

You are a UI/UX designer and frontend engineer. Your goal is to design and produce a **complete, pixel-perfect React application** for **KinHub**, a modern family hub web app.

The app connects families through shared tools: recipe management, fridge/pantry tracking, an AI recipe assistant, and family configuration. It is built with:

- **React 18 + TypeScript + Vite**
- **shadcn/ui** as the component library (Radix UI primitives + Tailwind CSS)
- **React Router v6** for routing
- **TanStack Query** for server state / API calls
- **JWT authentication** (access token + refresh token stored in memory/localStorage)
- **react-i18next** for internationalization (EN + IT, centralized locale files in `src/i18n/locales/`)
- **next-themes** for dark/light mode theming
- **One API provider per controller** — each controller maps to a `[Feature]Provider.tsx` that wraps TanStack Query and exposes a typed context + `use*()` hook

The backend REST API is running at `http://localhost:5000` (configurable via env var `VITE_API_URL`).

---

## App Overview

**KinHub** = a family management hub. Each user belongs to a family. The family admin can toggle modular services (like Recipes) on/off.

Current active features:

- Authentication (JWT)
- Family management
- Recipe Books → Recipes → Ingredients + Steps
- Fridge management → Fridge Ingredients
- AI Recipe Assistant (suggest, parse, adapt)

---

## Design Principles

- **Warm, modern, friendly** — this is a family app, not a corporate tool
- **Mobile-first** but fully responsive on desktop
- Comfortable whitespace, soft shadows, rounded corners
- Dark mode support from day one (shadcn/ui theming via CSS variables)
- Accessible (WCAG AA): good contrast, keyboard navigation, focus rings
- Fast perceived performance: skeleton loaders on data fetches

### Suggested Color Palette

| Token          | Light            | Dark      |
| -------------- | ---------------- | --------- |
| `--primary`    | Indigo `#6366f1` | `#818cf8` |
| `--background` | `#fafaf9`        | `#09090b` |
| `--card`       | `#ffffff`        | `#18181b` |
| `--muted`      | `#f4f4f5`        | `#27272a` |
| `--accent`     | Amber `#f59e0b`  | `#fbbf24` |

Typography: **Inter** (sans-serif), loaded from Google Fonts or Fontsource.

---

## Application Structure

### Routing Map

```
/                        → redirect to /dashboard (if authenticated) or /login
/login                   → Login page
/register                → Register page
/dashboard               → Home dashboard
/profile                 → User profile settings
/family                  → Family overview & settings
/services                → Family services toggle
/recipe-books            → List of recipe books
/recipe-books/:id        → Recipe book detail (list of recipes)
/recipe-books/:id/recipes/:recipeId  → Recipe detail (ingredients + steps)
/fridges                 → List of fridges
/fridges/:id             → Fridge detail (ingredients list)
/ai-assistant            → AI Recipe Assistant (suggest / parse / adapt)
```

---

## Pages — Detailed Specifications

> For every page: use `Skeleton` cards/rows during loading, `sonner` toast for mutations, and translate all visible strings via `react-i18next`.

---

### 1. Login (`/login`)

**Layout**

- Full-screen centered layout with a soft gradient background (light: indigo-50 to white; dark: zinc-950 to zinc-900)
- Single `Card` centered, max-width `420px`, `p-8`, `rounded-2xl`, `shadow-xl`

**Content (top to bottom)**

1. App logo icon (e.g. `HomeIcon` from Lucide, 40px, primary color) + app name "KinHub" in `text-2xl font-bold`
2. Tagline: `text-muted-foreground text-sm` — "Your family, all in one place."
3. `Separator` with `my-6`
4. `Form` with:
   - `Label` + `Input` for **Email** (type=email, autocomplete=email)
   - `Label` + `Input` for **Password** (type=password, show/hide toggle button inside input)
5. Inline field-level error messages in `text-destructive text-xs` below each input
6. `Button` variant=`default`, full width, size=`lg`: "Sign in" (shows `Loader2` spinner when loading)
7. Subtle link: `text-sm text-muted-foreground` — "Don't have an account? **Register**" → `/register`

**Interactions**

- Pressing Enter in any field submits the form
- On 401/400: show field errors extracted from API `errors` array
- On success: smooth fade-out then redirect to `/dashboard`

---

### 2. Register (`/register`)

**Layout** — identical card layout to Login

**Content**

1. Logo + "Create your account" heading
2. `Form` with: Email, Password, Confirm Password
3. Password strength indicator (4 colored segments below the password field)
4. Inline validation: password mismatch shown immediately on blur
5. "Create account" `Button`, full width, primary
6. Link back to `/login`

**Post-success**: success toast "Account created! Please sign in." then redirect to `/login`.

---

### 3. Dashboard (`/dashboard`)

**Layout**

- Page title: "Dashboard" (hidden on mobile; shown in sidebar active state)
- Greeting header: `text-3xl font-bold` "Hello, {firstName or email}! 👋" + `text-muted-foreground text-sm` current date
- `Separator` `my-6`

**Sections (stacked vertically)**

**A. Family Card**

- Full-width `Card` with gradient left border (4px, primary color)
- Left: `Avatar` (family initials, size lg) + family name (`text-xl font-semibold`) + member count badge
- Right: admin badge if user is admin; "Manage Family →" link
- If no family: replace with a full-width CTA card ("You're not part of a family yet") with two `Button`s: "Create a family" and "Join with invite code"

**B. Enabled Services Grid**

- Section heading: "Your services" `text-lg font-semibold mb-4`
- Responsive grid: `grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4`
- Each service = `Card` with:
  - Icon (Lucide, 32px, primary color) top-left
  - Service name `font-semibold`
  - Short description `text-muted-foreground text-sm`
  - Bottom: "Open →" link
  - Hover: lift shadow + border-primary

**C. Recent Activity** (optional, placeholder)

- `text-muted-foreground text-sm italic` "Recent activity coming soon"

---

### 4. Family Overview (`/family`)

**Layout**

- Page header: "Family" + Edit name pencil icon (opens `Dialog` with a single name input)
- Two-column layout on desktop: left 2/3 members, right 1/3 settings/danger

**A. Members Section**

- Section heading + "Add Member" `Button` (outline, small, top-right)
- `Table` with columns: Avatar | Name | Role (Badge: "Admin" / "Member") | Actions
- Actions column: `DropdownMenu` with Edit + Remove (Remove triggers `AlertDialog`)
- Empty state: "No members yet"
- On mobile: cards instead of table (stacked vertically)

**B. Admin Code Section** (admin only)

- `Card` with `CardHeader` "Admin Code" + lock icon
- Shows masked code (•••••) with "Reveal" toggle
- "Verify code" input + button
- "Regenerate" `Button` variant=destructive (triggers `AlertDialog` confirmation)

**C. Danger Zone**

- `Card` with red/destructive border
- "Leave Family" `Button` variant=outline (for non-admin)
- "Delete Family" `Button` variant=destructive (for admin, triggers `AlertDialog`)

---

### 5. Services (`/services`)

**Layout**

- Page heading "Family Services" + subtitle "Enable or disable features for your family"
- Responsive grid `grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6`

**Service Card**

- `Card` `p-6` with:
  - Top row: service icon (Lucide, 28px) + service name `font-semibold` + `Switch` (right-aligned)
  - Body: description `text-muted-foreground text-sm`
  - Bottom: status badge "Active" (green) / "Inactive" (muted)
- When Switch toggled: optimistic update + loading spinner on the switch; revert on error + toast

---

### 6. Recipe Books (`/recipe-books`)

**Layout**

- Page header row: "Recipe Books" `text-2xl font-bold` + "New Recipe Book" `Button` (right-aligned, `+ icon`)
- Search/filter `Input` with search icon (full width on mobile, max-w-sm on desktop)
- Responsive grid `grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6`

**Recipe Book Card**

- `Card` with `aspect-[4/3]` cover area at top:
  - Gradient background (deterministic from book ID, e.g. hsl based on hash) or emoji cover
  - Book title overlaid at bottom in white `font-bold text-lg drop-shadow`
- `CardContent`:
  - Recipe count badge + "recipes"
  - Last updated `text-xs text-muted-foreground`
- `CardFooter`: `DropdownMenu` dots button (Edit, Delete)
- Card hover: scale(1.02) + shadow-lg transition

**Empty State**

- Centered illustration (📚 emoji large) + "No recipe books yet" + "Create your first one" `Button`

---

### 7. Recipe Book Detail (`/recipe-books/:id`)

**Layout**

- `Breadcrumb`: Recipe Books › {Book name}
- Book header: large cover gradient strip (height 120px) with book name `text-3xl font-bold` centered
- Action row: "Add Recipe" `Button` + `DropdownMenu` (Edit book, Delete book)
- `Separator`
- Recipes grid: `grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5`

**Recipe Card**

- `Card` with:
  - Cover color strip (top, 60px, gradient from recipe name hash)
  - Recipe name `font-semibold text-base`
  - Row: serving size icon + prep time icon in `text-muted-foreground text-xs`
  - `DropdownMenu` dots (Edit, Delete)
- Click card body → navigate to recipe detail

**Add Recipe Dialog**

- `Dialog` with `DialogHeader` "New Recipe"
- Fields: Name, Description (optional), Serving size (number), Prep time (minutes)
- "Create" `Button`

---

### 8. Recipe Detail (`/recipe-books/:id/recipes/:recipeId`)

**Layout**

- `Breadcrumb`: Recipe Books › {Book} › {Recipe}
- **Hero section**: gradient banner (height 160px) with recipe name centered `text-4xl font-bold text-white`
- Meta row below banner: serving size chip + prep time chip + "Edit" + "Delete" (action `DropdownMenu`)

**Two-column grid on desktop** (`grid-cols-5 gap-8`):

**Left column (col-span-3) — Ingredients**

- Section header "Ingredients" `text-lg font-semibold` + "Add" `Button` (small)
- List of `Card` rows: quantity + unit + ingredient name + delete `Button` (trash icon, xs)
- Inline "Add ingredient" form (appears below list, hidden by default): Name, Quantity, Unit inputs + Save/Cancel

**Right column (col-span-2) — Steps**

- Section header "Steps" + "Add" `Button`
- Ordered list (`ol`) of step cards: step number badge (primary circle) + step description + delete button
- Steps are drag-reorderable (use `@dnd-kit/core` or similar)
- Inline "Add step" form at the bottom

**Missing Ingredients Check Bar** (sticky at bottom of page on mobile)

- `Card` with "Check Missing Ingredients" `Button` + fridge `Select` dropdown
- On result: show `Badge` list of missing ingredients with destructive color; "All ingredients available!" success badge if none missing

---

### 9. Fridges (`/fridges`)

**Layout** — same pattern as Recipe Books

**Fridge Card**

- `Card` with fridge icon (🧊 or `RefrigeratorIcon`) + fridge name + ingredient count
- `DropdownMenu` for Edit / Delete

---

### 10. Fridge Detail (`/fridges/:id`)

**Layout**

- Page header: fridge name + edit/delete `DropdownMenu`
- Search `Input` with clear button (filters ingredients client-side)
- "Add Ingredient" `Button` (top-right) → opens inline form row at top of table or a `Dialog`

**Ingredients Table** (`Table`)

- Columns: Name | Quantity | Unit | Actions
- Actions: edit (pencil icon, inline edit row) + delete (`AlertDialog`)
- Inline edit: clicking pencil turns the row into editable inputs with save/cancel buttons
- Sortable by name
- Pagination or virtual scroll if > 50 items

**Empty State**: fridge icon + "Your fridge is empty. Add your first ingredient."

---

### 11. AI Recipe Assistant (`/ai-assistant`)

**Layout**

- Page header: "AI Recipe Assistant" + sparkle icon ✨
- Subtitle: "Powered by GPT-4o — let AI help you cook smarter"
- `Tabs` component with 3 tabs (full width on mobile, auto on desktop)

**Tab 1 — Suggest Recipes 🍽️**

- `Select` "Choose a fridge…" (lists all user fridges)
- "Suggest Recipes" `Button` (primary, full width on mobile)
- Loading state: 3 `Skeleton` cards animating
- Results: `grid-cols-1 sm:grid-cols-2 gap-4`
  - Each result `Card`: recipe name `font-semibold`, AI-generated description, ingredient match score bar (green progress bar), "Save to Recipe Book" `Button` (opens a book selector `Dialog`)

**Tab 2 — Parse Recipe 📋**

- Label + `Textarea` (rows=10, resize=vertical) "Paste a recipe from any source…"
- "Parse Recipe" `Button`
- Loading: skeleton of name + 2 columns
- Result: structured `Card` with:
  - Name + description
  - Two-column: ingredients list + steps list
  - "Save to Recipe Book" `Button`

**Tab 3 — Adapt Recipe 🔄**

- `Select` "Choose a recipe…" (groups by recipe book)
- `Input` or multi-badge chip input: dietary constraints ("gluten-free", "vegan", "nut-free", custom)
- "Adapt Recipe" `Button`
- Result: two-column diff view:
  - Original ingredient (strikethrough in muted) → new ingredient (green highlight)
  - New steps shown with amber left border for changed steps
- "Save Adapted Recipe" `Button`

---

### 12. Profile (`/profile`)

**Layout**

- Page header "Profile"
- `Card` centered, max-width `560px`

**Sections (inside Card)**

**A. Identity**

- `Avatar` (initials, size=xl, `w-16 h-16`) centered
- Email below, `font-medium`
- `Badge` "Admin" or "Member" based on family role

**B. Update Email**

- `CardHeader` "Update Email" + `Separator`
- Fields: New email, Current password
- "Save" `Button`

**C. Update Password**

- `CardHeader` "Update Password" + `Separator`
- Fields: Current password, New password (with strength indicator), Confirm new password
- "Save" `Button`

**D. Preferences**

- Dark/light mode `Switch` + label
- Language selector (`Select`: English / Italiano)

**E. Danger Zone**

- Red `Card` border
- "Delete Account" `Button` variant=destructive → `AlertDialog` with typed confirmation ("Type DELETE to confirm")

---

## Layout & Navigation

### Desktop (≥ 1024px)

**Sidebar** (fixed left, `w-60`, collapsible to `w-16` icon-only mode):

- **Top**: App logo icon + "KinHub" text (hidden in collapsed mode)
- **Nav section** (with group label "Menu"):
  - Dashboard (`LayoutDashboard`)
  - Family (`Users`)
  - Services (`Grid2x2`)
  - Recipe Books (`BookOpen`)
  - Fridges (`Refrigerator`)
  - AI Assistant (`Sparkles`) — with "AI" badge in accent color
- **Bottom section**:
  - Dark/Light toggle `Button` icon-only (`Sun`/`Moon`)
  - Language toggle `Button`: "EN" / "IT" text
  - `Separator`
  - User `Avatar` (initials) + email (truncated) → click → `DropdownMenu` (Profile, Logout)
- Active nav item: `bg-primary/10 text-primary font-semibold rounded-lg`
- Hover: `bg-muted rounded-lg`
- Sidebar collapse toggle: chevron button at bottom-right of sidebar

**Main content area**:

- `ml-60` (or `ml-16` collapsed), `min-h-screen`, `bg-background`
- Content padded: `p-6 md:p-8`
- Max-width `max-w-7xl mx-auto`

### Mobile / Tablet (< 1024px)

**Top App Bar** (`h-14`, `sticky top-0 z-50 border-b bg-background/80 backdrop-blur`):

- Left: Hamburger `Button` icon → opens `Sheet` (slide from left)
- Center: current page title or "KinHub" logo
- Right: `Avatar` (user initials, sm) + dark mode toggle icon

**Sheet Navigation** (mobile):

- Full sidebar nav content mirrored inside the Sheet
- Closes on nav item click

**Bottom Navigation Bar** (`fixed bottom-0`, `h-16`, `border-t bg-background/80 backdrop-blur`):

- 5 icons centered: Dashboard, Recipe Books, Fridges, AI, Profile
- Active icon: primary color + small dot indicator
- Labels below icons in `text-[10px]`

---

## Key shadcn/ui Components to Use

| Component                  | Where                                           |
| -------------------------- | ----------------------------------------------- |
| `Button`                   | All CTAs                                        |
| `Card`                     | Recipe books, recipes, fridges, dashboard tiles |
| `Dialog`                   | Add/edit forms, confirmations                   |
| `Sheet`                    | Mobile nav drawer                               |
| `Table`                    | Family members, ingredient lists                |
| `Form` + `Input` + `Label` | All forms                                       |
| `Select`                   | Fridge selector, recipe selector in AI          |
| `Textarea`                 | Parse recipe raw text                           |
| `Tabs`                     | AI Assistant sections                           |
| `Badge`                    | Missing ingredients, roles                      |
| `Skeleton`                 | Loading states                                  |
| `Toaster` / `Sonner`       | Success / error notifications                   |
| `DropdownMenu`             | Item actions (edit, delete)                     |
| `Switch`                   | Service toggles                                 |
| `Avatar`                   | User initials                                   |
| `Breadcrumb`               | Deep navigation (recipe detail)                 |
| `AlertDialog`              | Destructive confirmations                       |
| `Separator`                | Section dividers                                |

---

## API Integration

All API calls go through a shared `apiClient` (Axios or fetch wrapper) that:

- Reads `VITE_API_URL` from env
- Attaches `Authorization: Bearer {accessToken}` header
- On 401: attempts token refresh via `POST /api/auth/refresh`, retries original request
- On refresh failure: clears tokens, redirects to `/login`

### API Endpoints Reference

| Method | URL                                         | Feature             |
| ------ | ------------------------------------------- | ------------------- |
| POST   | `/api/auth/login`                           | Login               |
| POST   | `/api/auth/register`                        | Register            |
| POST   | `/api/auth/logout`                          | Logout              |
| POST   | `/api/auth/refresh`                         | Refresh token       |
| GET    | `/api/auth/me`                              | Current user        |
| PUT    | `/api/auth/me/email`                        | Update email        |
| PUT    | `/api/auth/me/password`                     | Update password     |
| DELETE | `/api/auth/me`                              | Delete account      |
| POST   | `/api/families`                             | Create family       |
| GET    | `/api/families`                             | Get my family       |
| POST   | `/api/families/{id}/members`                | Add member          |
| PUT    | `/api/families/{id}/members/{memberId}`     | Update member       |
| DELETE | `/api/families/{id}/members/{memberId}`     | Remove member       |
| POST   | `/api/families/{id}/verify-admin-code`      | Verify admin code   |
| PATCH  | `/api/families/{id}`                        | Update family       |
| PATCH  | `/api/families/{id}/admin-code`             | Update admin code   |
| GET    | `/api/services`                             | List all services   |
| GET    | `/api/services/family/{familyId}`           | Get family services |
| POST   | `/api/services/family/{familyId}/toggle`    | Toggle service      |
| POST   | `/api/recipe-books`                         | Create recipe book  |
| GET    | `/api/recipe-books`                         | List recipe books   |
| GET    | `/api/recipe-books/{id}`                    | Get recipe book     |
| PUT    | `/api/recipe-books/{id}`                    | Update recipe book  |
| DELETE | `/api/recipe-books/{id}`                    | Delete recipe book  |
| POST   | `/api/recipe-books/{id}/recipes`            | Create recipe       |
| GET    | `/api/recipe-books/{id}/recipes`            | List recipes        |
| GET    | `/api/recipe-books/{id}/recipes/{recipeId}` | Get recipe          |
| PUT    | `/api/recipe-books/{id}/recipes/{recipeId}` | Update recipe       |
| DELETE | `/api/recipe-books/{id}/recipes/{recipeId}` | Delete recipe       |
| POST   | `/api/fridges`                              | Create fridge       |
| GET    | `/api/fridges`                              | List fridges        |
| GET    | `/api/fridges/{id}`                         | Get fridge          |
| PUT    | `/api/fridges/{id}`                         | Update fridge       |
| DELETE | `/api/fridges/{id}`                         | Delete fridge       |
| POST   | `/api/recipe-assistant/suggest`             | AI: suggest recipes |
| POST   | `/api/recipe-assistant/parse`               | AI: parse recipe    |
| POST   | `/api/recipe-assistant/adapt`               | AI: adapt recipe    |

---

## Project Setup Instructions

When the user says **"start implementation"**, scaffold the project with these exact steps:

### Step 1 — Scaffold Vite project

Run inside `src/Presentations/Kin.KinHub.Core.React/`:

```bash
npm create vite@latest . -- --template react-ts
```

### Step 2 — Install dependencies

```bash
npm install
npm install -D tailwindcss @tailwindcss/vite
npm install class-variance-authority clsx tailwind-merge lucide-react
npm install @radix-ui/react-slot
npm install react-router-dom
npm install @tanstack/react-query
npm install axios
npm install next-themes
npm install react-i18next i18next i18next-browser-languagedetector
```

### Step 3 — Init shadcn/ui

```bash
npx shadcn@latest init
```

Choose:

- Style: **New York**
- Base color: **Zinc**
- CSS variables: **Yes**

### Step 4 — Add shadcn components

```bash
npx shadcn@latest add button card dialog sheet table form input label select textarea tabs badge skeleton sonner dropdown-menu switch avatar breadcrumb alert-dialog separator
```

### Step 5 — Set up project structure

```
src/
  api/           # apiClient (Axios + JWT refresh interceptor)
  components/    # Shared UI: Layout, Sidebar, TopBar, ProtectedRoute
  features/      # One folder per feature; each has a Provider + hook + pages
    auth/
      AuthProvider.tsx        # Wraps all /api/auth/* calls
      useAuth.ts
      pages/                  # LoginPage.tsx, RegisterPage.tsx
    family/
      FamilyProvider.tsx      # Wraps /api/families/*
      useFamily.ts
      ServicesProvider.tsx    # Wraps /api/services/*
      useServices.ts
      pages/                  # FamilyPage.tsx, ServicesPage.tsx
    recipes/
      RecipeBookProvider.tsx  # Wraps /api/recipe-books/*
      useRecipeBooks.ts
      RecipeProvider.tsx      # Wraps /api/recipe-books/:id/recipes/*
      useRecipes.ts
      pages/
    fridges/
      FridgeProvider.tsx      # Wraps /api/fridges/*
      useFridges.ts
      pages/
    ai-assistant/
      RecipeAssistantProvider.tsx  # Wraps /api/recipe-assistant/*
      useRecipeAssistant.ts
      pages/
    profile/
      pages/                  # ProfilePage.tsx (uses AuthProvider)
  i18n/
    index.ts                  # i18next init (language detection, localStorage persistence)
    locales/
      en.json                 # All English UI strings
      it.json                 # All Italian UI strings
  lib/           # utils.ts (cn helper), constants.ts
  router/        # routes.tsx (React Router config)
  store/         # authContext.tsx
  types/         # API response types (strict, no `any`)
  main.tsx
  App.tsx
```

### Step 6 — Implement in this order

1. `apiClient` with JWT + refresh logic
2. Auth context + `ProtectedRoute` wrapper
3. i18n setup (`src/i18n/index.ts`, `en.json`, `it.json`)
4. Login + Register pages
5. App shell (Sidebar with dark/light toggle + language switcher, TopBar, layout)
6. Dashboard
7. Family pages (FamilyProvider, ServicesProvider)
8. Recipe Books + Recipe detail (RecipeBookProvider, RecipeProvider)
9. Fridges + Fridge detail (FridgeProvider)
10. AI Assistant (RecipeAssistantProvider)
11. Profile

---

## Deliverables

Produce the full working React application with:

- All pages listed above implemented with real API calls via dedicated providers
- One `[Feature]Provider.tsx` per controller, exposing typed context + `use*()` hook
- Centralized i18n: `src/i18n/locales/en.json` + `it.json`; language toggle in Sidebar/TopBar
- Dark/light mode toggle (next-themes) in Sidebar (desktop) and TopBar (mobile)
- Consistent shadcn/ui component usage throughout
- Responsive layout (mobile + desktop)
- Proper loading skeletons and error states
- Toast notifications for success/error feedback
- TypeScript strict mode, no `any` types
- Clean, readable code following React best practices
