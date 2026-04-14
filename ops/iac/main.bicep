targetScope = 'resourceGroup'

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Azure region for the Static Web App. Defaults to the main deployment location.')
param staticWebAppLocation string = location

@description('Static Web App name.')
param staticWebAppName string

@description('Web App name.')
param webAppName string

@description('App Service plan name for the Web App.')
param appServicePlanName string = '${webAppName}-plan'

@description('Identity Web App name.')
param identityWebAppName string

@description('App Service plan name for the Identity Web App.')
param identityAppServicePlanName string = '${identityWebAppName}-plan'

@description('Application Insights name.')
param applicationInsightsName string = '${webAppName}-appi'

@description('Key Vault name.')
param keyVaultName string

@description('SQL Server name.')
param sqlServerName string

@description('SQL Database name.')
param sqlDatabaseName string

@description('SQL administrator login.')
param sqlAdministratorLogin string

@description('SQL administrator password.')
@secure()
param sqlAdministratorPassword string

@description('JWT secret key.')
@secure()
param jwtSecret string

@description('JWT issuer.')
param jwtIssuer string = 'kinhub'

@description('JWT access token expiry in minutes.')
param jwtAccessTokenExpiryMinutes string = '15'

var sqlServerFullyQualifiedDomainName = '${sqlServer.name}${environment().suffixes.sqlServerHostname}'
var identityWebAppSettings = [
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: applicationInsights.properties.ConnectionString
  }
  {
    name: 'ConnectionStrings__KinHub'
    value: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=${sqlConnectionStringSecretName})'
  }
  {
    name: 'Jwt__Secret'
    value: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=${jwtSecretSecretName})'
  }
  {
    name: 'Jwt__Issuer'
    value: jwtIssuer
  }
]
var sqlConnectionString = 'Server=tcp:${sqlServerFullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
var sqlConnectionStringSecretName = 'sql-connection-string'
var jwtSecretSecretName = 'jwt-secret'
var keyVaultSecretsUserRoleDefinitionId = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  '4633458b-17de-408a-b874-0445c86b69e6'
)
var webAppSettings = [
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: applicationInsights.properties.ConnectionString
  }
  {
    name: 'ConnectionStrings__KinHub'
    value: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=${sqlConnectionStringSecretName})'
  }
  {
    name: 'Jwt__Secret'
    value: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=${jwtSecretSecretName})'
  }
  {
    name: 'Jwt__Issuer'
    value: jwtIssuer
  }
  {
    name: 'Jwt__AccessTokenExpiryMinutes'
    value: jwtAccessTokenExpiryMinutes
  }
]

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: null
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    enablePurgeProtection: true
    enableRbacAuthorization: true
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    publicNetworkAccess: 'Enabled'
    sku: {
      family: 'A'
      name: 'standard'
    }
    softDeleteRetentionInDays: 90
    tenantId: tenant().tenantId
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorPassword
    publicNetworkAccess: 'Enabled'
    version: '12.0'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
  }
}

resource sqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: sqlConnectionStringSecretName
  properties: {
    value: sqlConnectionString
  }
}

resource jwtSecretKvSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: jwtSecretSecretName
  properties: {
    value: jwtSecret
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: webAppName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: webAppSettings
      ftpsState: 'Disabled'
      http20Enabled: true
      linuxFxVersion: 'DOTNETCORE|8.0'
      minTlsVersion: '1.2'
    }
  }
}

resource keyVaultSecretsUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, webApp.id, keyVaultSecretsUserRoleDefinitionId)
  properties: {
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: keyVaultSecretsUserRoleDefinitionId
  }
}

resource identityAppServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: identityAppServicePlanName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource identityWebApp 'Microsoft.Web/sites@2024-04-01' = {
  name: identityWebAppName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: identityAppServicePlan.id
    siteConfig: {
      appSettings: identityWebAppSettings
      ftpsState: 'Disabled'
      http20Enabled: true
      linuxFxVersion: 'DOTNETCORE|8.0'
      minTlsVersion: '1.2'
    }
  }
}

resource identityKeyVaultSecretsUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, identityWebApp.id, keyVaultSecretsUserRoleDefinitionId)
  properties: {
    principalId: identityWebApp.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: keyVaultSecretsUserRoleDefinitionId
  }
}

resource staticWebApp 'Microsoft.Web/staticSites@2024-04-01' = {
  name: staticWebAppName
  location: staticWebAppLocation
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    allowConfigFileUpdates: true
    publicNetworkAccess: 'Enabled'
    stagingEnvironmentPolicy: 'Disabled'
  }
}

resource staticWebAppSettings 'Microsoft.Web/staticSites/config@2024-04-01' = {
  parent: staticWebApp
  name: 'appsettings'
  properties: {
    VITE_CORE_URL: 'https://${webApp.properties.defaultHostName}'
    VITE_IDENTITY_URL: 'https://${identityWebApp.properties.defaultHostName}'
  }
}

output webAppDefaultHostname string = webApp.properties.defaultHostName
output identityWebAppDefaultHostname string = identityWebApp.properties.defaultHostName
output staticWebAppDefaultHostname string = staticWebApp.properties.defaultHostname
