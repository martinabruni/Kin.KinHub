-- ============================================================
-- KinHub Database Creation Script
-- Target: SQL Server
-- Run against an empty KinHub database
-- ============================================================

-- ------------------------------------------------------------
-- Schemas
-- ------------------------------------------------------------

IF NOT EXISTS (SELECT 1
FROM sys.schemas
WHERE name = N'identity')
    EXEC (N'CREATE SCHEMA [identity]');
GO

IF NOT EXISTS (SELECT 1
FROM sys.schemas
WHERE name = N'core')
    EXEC (N'CREATE SCHEMA [core]');
GO

-- ============================================================
-- IDENTITY SCHEMA
-- ============================================================

-- identity.KinUserEntity
CREATE TABLE [identity].[KinUserEntity]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Email] NVARCHAR(256) NOT NULL,
    [DisplayName] NVARCHAR(200) NULL,
    [IsEmailVerified] BIT NOT NULL DEFAULT 0,
    [Status] NVARCHAR(50) NOT NULL DEFAULT N'Active',
    -- Active | Locked | Suspended
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_identity_KinUserEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_identity_KinUserEntity_Email] UNIQUE ([Email])
);
GO

-- identity.ProviderEntity  (config table — pre-populated)
-- Id: 1=KinHub, 2=Google, 3=GitHub, 4=Microsoft
CREATE TABLE [identity].[ProviderEntity]
(
    [Id] INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Label] NVARCHAR(200) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_identity_ProviderEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_identity_ProviderEntity_Name] UNIQUE ([Name])
);
GO

-- identity.UserCredentialEntity
CREATE TABLE [identity].[UserCredentialEntity]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_identity_UserCredentialEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_identity_UserCredentialEntity_UserId] FOREIGN KEY ([UserId])
        REFERENCES [identity].[KinUserEntity] ([Id]),
    CONSTRAINT [UQ_identity_UserCredentialEntity_UserId] UNIQUE ([UserId])
);
GO

-- identity.UserProviderEntity
CREATE TABLE [identity].[UserProviderEntity]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [ProviderId] INT NOT NULL,
    [ProviderUserId] NVARCHAR(256) NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_identity_UserProviderEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_identity_UserProviderEntity_UserId] FOREIGN KEY ([UserId])
        REFERENCES [identity].[KinUserEntity] ([Id]),
    CONSTRAINT [FK_identity_UserProviderEntity_ProviderId] FOREIGN KEY ([ProviderId])
        REFERENCES [identity].[ProviderEntity] ([Id]),
    CONSTRAINT [UQ_identity_UserProviderEntity_UserProvider] UNIQUE ([UserId], [ProviderId]),
    CONSTRAINT [UQ_identity_UserProviderEntity_ProviderUser] UNIQUE ([ProviderId], [ProviderUserId])
);
GO

-- identity.RefreshTokenEntity
CREATE TABLE [identity].[RefreshTokenEntity]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Token] NVARCHAR(512) NOT NULL,
    [ExpiresAtUtc] DATETIME2 NOT NULL,
    [Revoked] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_identity_RefreshTokenEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_identity_RefreshTokenEntity_UserId] FOREIGN KEY ([UserId])
        REFERENCES [identity].[KinUserEntity] ([Id]),
    CONSTRAINT [UQ_identity_RefreshTokenEntity_Token] UNIQUE ([Token])
);
GO

CREATE INDEX [IX_identity_RefreshTokenEntity_UserId]
    ON [identity].[RefreshTokenEntity] ([UserId]);
GO

-- ============================================================
-- CORE SCHEMA
-- ============================================================

-- core.KinHubServiceEntity  (config table — pre-populated)
CREATE TABLE [core].[KinHubServiceEntity]
(
    [Id] INT NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [BaseUrl] NVARCHAR(500) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsAdminOnly] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_core_KinHubServiceEntity] PRIMARY KEY ([Id])
);
GO

-- core.FamilyRoleEntity  (config table — pre-populated)
-- Id: 1=Admin, 2=Member
CREATE TABLE [core].[FamilyRoleEntity]
(
    [Id] INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_core_FamilyRoleEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_core_FamilyRoleEntity_Name] UNIQUE ([Name])
);
GO

-- core.FamilyEntity
CREATE TABLE [core].[FamilyEntity]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Name] NVARCHAR(200) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AdminCodeHash] NVARCHAR(MAX) NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_core_FamilyEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_core_FamilyEntity_UserId] FOREIGN KEY ([UserId])
        REFERENCES [identity].[KinUserEntity] ([Id])
);
GO

CREATE INDEX [IX_core_FamilyEntity_UserId]
    ON [core].[FamilyEntity] ([UserId]);
GO

-- core.FamilyMemberEntity
CREATE TABLE [core].[FamilyMemberEntity]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Name] NVARCHAR(200) NOT NULL,
    [FamilyId] UNIQUEIDENTIFIER NOT NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_core_FamilyMemberEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_core_FamilyMemberEntity_FamilyId] FOREIGN KEY ([FamilyId])
        REFERENCES [core].[FamilyEntity] ([Id]),
    CONSTRAINT [UQ_core_FamilyMemberEntity_FamilyName] UNIQUE ([FamilyId], [Name])
);
GO

CREATE INDEX [IX_core_FamilyMemberEntity_FamilyId]
    ON [core].[FamilyMemberEntity] ([FamilyId]);
GO

-- core.FamilyServiceEntity
CREATE TABLE [core].[FamilyServiceEntity]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [FamilyId] UNIQUEIDENTIFIER NOT NULL,
    [ServiceId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_core_FamilyServiceEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_core_FamilyServiceEntity_FamilyId] FOREIGN KEY ([FamilyId])
        REFERENCES [core].[FamilyEntity] ([Id]),
    CONSTRAINT [FK_core_FamilyServiceEntity_ServiceId] FOREIGN KEY ([ServiceId])
        REFERENCES [core].[KinHubServiceEntity] ([Id]),
    CONSTRAINT [UQ_core_FamilyServiceEntity_FamilyService] UNIQUE ([FamilyId], [ServiceId])
);
GO

CREATE INDEX [IX_core_FamilyServiceEntity_FamilyId]
    ON [core].[FamilyServiceEntity] ([FamilyId]);
GO

-- core.MemberRoleEntity
CREATE TABLE [core].[MemberRoleEntity]
(
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [MemberId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_core_MemberRoleEntity] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_core_MemberRoleEntity_MemberId] FOREIGN KEY ([MemberId])
        REFERENCES [core].[FamilyMemberEntity] ([Id]),
    CONSTRAINT [FK_core_MemberRoleEntity_RoleId] FOREIGN KEY ([RoleId])
        REFERENCES [core].[FamilyRoleEntity] ([Id]),
    CONSTRAINT [UQ_core_MemberRoleEntity_MemberRole] UNIQUE ([MemberId], [RoleId])
);
GO

CREATE INDEX [IX_core_MemberRoleEntity_MemberId]
    ON [core].[MemberRoleEntity] ([MemberId]);
GO

-- ============================================================
-- SEED DATA
-- ============================================================

DECLARE @seed DATETIME2 = '2026-01-01T00:00:00';

-- identity.ProviderEntity
INSERT INTO [identity].[ProviderEntity]
    ([Id], [Name], [Label], [IsActive], [CreatedAt], [UpdatedAt])
VALUES
    (1, N'kinhub', N'Accedi con KinHub', 1, @seed, @seed),
    (2, N'google', N'Accedi con Google', 0, @seed, @seed),
    (3, N'github', N'Accedi con GitHub', 0, @seed, @seed),
    (4, N'microsoft', N'Accedi con Microsoft', 0, @seed, @seed);
GO

-- core.FamilyRoleEntity
INSERT INTO [core].[FamilyRoleEntity]
    ([Id], [Name], [IsActive], [CreatedAt], [UpdatedAt])
VALUES
    (1, N'admin', 1, '2026-01-01T00:00:00', '2026-01-01T00:00:00'),
    (2, N'member', 1, '2026-01-01T00:00:00', '2026-01-01T00:00:00');
GO

-- core.KinHubServiceEntity
INSERT INTO [core].[KinHubServiceEntity]
    ([Id], [Name], [BaseUrl], [IsActive], [IsAdminOnly], [CreatedAt], [UpdatedAt])
VALUES
    (1, N'KinConsole', N'/kin-console', 1, 1, '2026-01-01T00:00:00', '2026-01-01T00:00:00'),
    (2, N'KinRecipe', N'/kin-recipe', 0, 0, '2026-01-01T00:00:00', '2026-01-01T00:00:00');
GO
