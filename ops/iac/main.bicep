targetScope = 'resourceGroup'

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Azure region for the Static Web App. Defaults to the main deployment location.')
param staticWebAppLocation string = location

@description('Static Web App name.')
param staticWebAppName string

@description('Function App name.')
param functionAppName string

@description('App Service plan name for the Function App.')
param appServicePlanName string = '${functionAppName}-plan'

@description('Storage account name for the Function App.')
@minLength(3)
@maxLength(24)
param storageAccountName string

@description('Application Insights name.')
param applicationInsightsName string = '${functionAppName}-appi'

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

@description('Business timezone identifier.')
param businessTimeZoneId string = 'Europe/Rome'

@description('IFTTT webhook URL used by the Function App.')
@secure()
param iftttWebhookUrl string

@description('Maximum number of IFTTT retries.')
param iftttMaxRetries int = 3

@description('Backoff schedule in minutes for IFTTT retries.')
param iftttRetryBackoffMinutes array = [
  1
  5
  15
]

var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
var sqlServerFullyQualifiedDomainName = '${sqlServer.name}${environment().suffixes.sqlServerHostname}'
var sqlConnectionString = 'Server=tcp:${sqlServerFullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
var sqlConnectionStringSecretName = 'sql-connection-string'
var iftttWebhookUrlSecretName = 'ifttt-webhook-url'
var keyVaultSecretsUserRoleDefinitionId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
var functionAppSettings = [
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: applicationInsights.properties.ConnectionString
  }
  {
    name: 'AzureWebJobsStorage'
    value: storageConnectionString
  }
  {
    name: 'Business__BusinessTimeZoneId'
    value: businessTimeZoneId
  }
  {
    name: 'FUNCTIONS_EXTENSION_VERSION'
    value: '~4'
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: 'dotnet-isolated'
  }
  {
    name: 'Ifttt__MaxRetries'
    value: string(iftttMaxRetries)
  }
  {
    name: 'Ifttt__RetryBackoffMinutes__0'
    value: string(iftttRetryBackoffMinutes[0])
  }
  {
    name: 'Ifttt__RetryBackoffMinutes__1'
    value: string(iftttRetryBackoffMinutes[1])
  }
  {
    name: 'Ifttt__RetryBackoffMinutes__2'
    value: string(iftttRetryBackoffMinutes[2])
  }
  {
    name: 'Ifttt__WebhookUrl'
    value: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=${iftttWebhookUrlSecretName})'
  }
  {
    name: 'SqlServer__ConnectionString'
    value: '@Microsoft.KeyVault(VaultName=${keyVault.name};SecretName=${sqlConnectionStringSecretName})'
  }
  {
    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
    value: storageConnectionString
  }
  {
    name: 'WEBSITE_CONTENTSHARE'
    value: toLower(take(replace(functionAppName, '-', ''), 63))
  }
]

resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

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

resource iftttWebhookUrlSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: iftttWebhookUrlSecretName
  properties: {
    value: iftttWebhookUrl
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  kind: 'functionapp'
  properties: {
    reserved: false
  }
}

resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: functionAppSettings
      ftpsState: 'Disabled'
      http20Enabled: true
      minTlsVersion: '1.2'
    }
  }
}

resource keyVaultSecretsUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, functionApp.id, keyVaultSecretsUserRoleDefinitionId)
  properties: {
    principalId: functionApp.identity.principalId
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

resource linkedFunctionApp 'Microsoft.Web/staticSites/userProvidedFunctionApps@2024-04-01' = {
  parent: staticWebApp
  name: 'linked-function'
  properties: {
    functionAppRegion: location
    functionAppResourceId: functionApp.id
  }
}

output functionAppDefaultHostname string = functionApp.properties.defaultHostName
output staticWebAppDefaultHostname string = staticWebApp.properties.defaultHostname
output linkedFunctionResourceId string = linkedFunctionApp.id
