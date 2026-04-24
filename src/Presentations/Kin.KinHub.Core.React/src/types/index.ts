export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}

export interface User {
  id: string;
  email: string;
  familyId: string | null;
  familyRole: "Admin" | "Member" | null;
}

export interface Family {
  id: string;
  name: string;
  adminCode?: string;
  members: FamilyMember[];
}

export interface FamilyMember {
  id: string;
  name: string;
  role: "Admin" | "Member";
}

export interface Service {
  id: number;
  name: string;
  description: string;
  icon: string;
  isEnabled: boolean;
}

export interface RecipeBook {
  id: string;
  name: string;
  recipeCount: number;
  updatedAt: string;
}

export interface Recipe {
  id: string;
  recipeBookId: string;
  name: string;
  description?: string;
  servingSize: number;
  prepTimeMinutes: number;
  ingredients: Ingredient[];
  steps: Step[];
}

export interface Ingredient {
  id: string;
  name: string;
  quantity: number;
  unit: string;
}

export interface Step {
  id: string;
  order: number;
  description: string;
}

export interface Fridge {
  id: string;
  name: string;
  ingredientCount: number;
}

export interface FridgeIngredient {
  id: string;
  name: string;
  quantity: number;
  unit: string;
}

export interface AISuggestedRecipe {
  name: string;
  description: string;
  matchScore: number;
  ingredients: Ingredient[];
  steps: Step[];
}

export interface AIParsedRecipe {
  name: string;
  description: string;
  ingredients: Ingredient[];
  steps: Step[];
}

export interface AIAdaptedRecipe {
  originalIngredients: Ingredient[];
  adaptedIngredients: Ingredient[];
  originalSteps: Step[];
  adaptedSteps: Step[];
  changedStepIds: number[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
}
