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
