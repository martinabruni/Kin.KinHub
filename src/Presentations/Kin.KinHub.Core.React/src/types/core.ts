// Core API types

export interface CreateFamilyRequest {
  familyName: string
  ownerProfileName: string
  additionalMembers?: string[]
}

export interface CreateFamilyResponse {
  familyId: string
  adminMemberId: string
}

export interface FamilyMemberDto {
  id: string
  name: string
  role: string
}

export interface FamilyDetailResponse {
  familyId: string
  name: string
  members: FamilyMemberDto[]
}

export interface AddFamilyMemberRequest {
  name: string
}

export interface AddFamilyMemberResponse {
  memberId: string
}
