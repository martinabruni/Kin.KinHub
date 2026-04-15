// Core API types

export interface CreateFamilyRequest {
  familyName: string;
  ownerProfileName: string;
  adminCode: string;
  additionalMembers?: string[];
}

export interface CreateFamilyResponse {
  familyId: string;
  adminMemberId: string;
}

export interface FamilyMemberDto {
  id: string;
  name: string;
  role: string;
}

export interface FamilyDetailResponse {
  familyId: string;
  name: string;
  members: FamilyMemberDto[];
}

export interface AddFamilyMemberRequest {
  name: string;
}

export interface AddFamilyMemberResponse {
  memberId: string;
}

export interface VerifyAdminCodeRequest {
  adminCode: string;
}

export interface KinHubServiceDto {
  id: number;
  name: string;
  baseUrl: string;
  isActive: boolean;
  isAdminOnly: boolean;
}

export interface FamilyServiceDto {
  id: string;
  serviceId: number;
  serviceName: string;
  isActive: boolean;
}

export interface ToggleFamilyServiceRequest {
  serviceId: number;
  isActive: boolean;
}

// Recipe Book
export interface RecipeBookResponse {
  id: string;
  name: string;
  description?: string;
  familyId: string;
}

export interface CreateRecipeBookRequest {
  name: string;
  description?: string;
}

export interface UpdateRecipeBookRequest {
  name: string;
  description?: string;
}

// Recipe
export interface RecipeIngredientResponse {
  id: string;
  name: string;
  measureUnit: string;
  quantity: number;
  recipeId: string;
}

export interface RecipeStepResponse {
  id: string;
  order: number;
  description: string;
  recipeId: string;
}

export interface RecipeResponse {
  id: string;
  name: string;
  backstory?: string;
  finalTime: string;
  portions: number;
  recipeBookId: string;
  ingredients: RecipeIngredientResponse[];
  steps: RecipeStepResponse[];
}

export interface CreateRecipeRequest {
  name: string;
  backstory?: string;
  finalTime: string;
  portions: number;
  recipeBookId: string;
}

export interface UpdateRecipeRequest {
  name: string;
  backstory?: string;
  finalTime: string;
  portions: number;
}

// Recipe Ingredient
export interface CreateRecipeIngredientRequest {
  name: string;
  measureUnit: string;
  quantity: number;
  recipeId: string;
}

export interface UpdateRecipeIngredientRequest {
  name: string;
  measureUnit: string;
  quantity: number;
}

// Recipe Step
export interface CreateRecipeStepRequest {
  order: number;
  description: string;
  recipeId: string;
}

export interface UpdateRecipeStepRequest {
  order: number;
  description: string;
}

// Fridge
export interface FridgeResponse {
  id: string;
  name: string;
  familyId: string;
}

export interface CreateFridgeRequest {
  name: string;
}

export interface UpdateFridgeRequest {
  name: string;
}

// Fridge Ingredient
export interface FridgeIngredientResponse {
  id: string;
  name: string;
  measureUnit: string;
  quantity: number;
  fridgeId: string;
}

export interface CreateFridgeIngredientRequest {
  name: string;
  measureUnit: string;
  quantity: number;
  fridgeId: string;
}

export interface UpdateFridgeIngredientRequest {
  name: string;
  measureUnit: string;
  quantity: number;
}

// Missing ingredients (AI)
export interface MissingIngredientsResponse {
  missingIngredients: string[];
}
