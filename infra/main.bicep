targetScope = 'resourceGroup'

param applicationName string = 'kinhub'
@allowed(['dev', 'test', 'prod'])
param environmentName string = 'dev'
param location string = 'italynorth'
param staticWebAppLocation string = 'westeurope'
@allowed(['dotnet-isolated'])
param runtimeName string = 'dotnet-isolated'
param runtimeVersion string = '10.0'
@allowed([512, 2048, 4096])
param instanceMemoryMB int = 2048
@minValue(1)
@maxValue(1000)
param maximumInstanceCount int = 20
@minValue(0)
param alwaysReadyInstanceCount int = 0
param deploymentBlobContainerName string = 'function-packages'
param azureTenantId string
param entraInstance string
param entraTenantId string
param entraBackendAudience string
param entraApiScopeName string = 'access_as_user'
param sqlEntraAdministratorName string
param sqlEntraAdministratorObjectId string
param sqlAdministratorLogin string
@secure()
param sqlAdministratorPassword string
param allowedOrigins array = ['http://localhost:5173']
param enableVnetIntegration bool = false
param virtualNetworkSubnetResourceId string = ''
param enablePurgeProtection bool = true
@minValue(30)
@maxValue(730)
param logRetentionDays int = 30
param logDailyCapGb int = 1
param tags object = {
  workload: 'kinhub'
  environment: environmentName
  owner: 'martinabruni'
  costClassification: 'personal-low-cost'
}

var resourceNameSuffix = uniqueString(
  subscription().id,
  resourceGroup().id,
  applicationName,
  environmentName
)
var storageAccountName = toLower('${applicationName}${environmentName}${resourceNameSuffix}')
var keyVaultName = toLower('${applicationName}-${environmentName}${resourceNameSuffix}')
var logAnalyticsName = toLower('${applicationName}-${environmentName}-${resourceNameSuffix}-log')
var applicationInsightsName = toLower('${applicationName}-${environmentName}-${resourceNameSuffix}-appi')
var sqlServerName = toLower('${applicationName}-${environmentName}-${resourceNameSuffix}-sql')
var functionAppName = toLower('${applicationName}-${environmentName}-${resourceNameSuffix}-func')
var functionPlanName = toLower('${applicationName}-${environmentName}-${resourceNameSuffix}-fc')
var staticWebAppName = toLower('${applicationName}-${environmentName}-${resourceNameSuffix}-web')

module monitoring './modules/monitoring.bicep' = {
  name: 'monitoring'
  params: {
    location: location
    logAnalyticsName: logAnalyticsName
    applicationInsightsName: applicationInsightsName
    retentionDays: logRetentionDays
    dailyCapGb: logDailyCapGb
    tags: tags
  }
}

module dataSecurity './modules/data-security.bicep' = {
  name: 'data-security'
  params: {
    location: location
    storageAccountName: storageAccountName
    keyVaultName: keyVaultName
    deploymentContainerName: deploymentBlobContainerName
    sqlServerName: sqlServerName
    enablePurgeProtection: enablePurgeProtection
    azureTenantId: azureTenantId
    entraAdministratorPrincipalName: sqlEntraAdministratorName
    entraAdministratorObjectId: sqlEntraAdministratorObjectId
    administratorLogin: sqlAdministratorLogin
    administratorPassword: sqlAdministratorPassword
    tags: tags
  }
}

module functions './modules/functions.bicep' = {
  name: 'functions'
  params: {
    name: functionAppName
    planName: functionPlanName
    location: location
    tags: tags
    runtimeName: runtimeName
    runtimeVersion: runtimeVersion
    instanceMemoryMB: instanceMemoryMB
    maximumInstanceCount: maximumInstanceCount
    alwaysReadyInstanceCount: alwaysReadyInstanceCount
    storageAccountName: dataSecurity.outputs.storageAccountName
    storageAccountId: dataSecurity.outputs.storageAccountId
    storageBlobEndpoint: dataSecurity.outputs.storageBlobEndpoint
    deploymentContainerName: dataSecurity.outputs.deploymentContainerName
    applicationContainerName: dataSecurity.outputs.applicationContainerName
    applicationInsightsName: monitoring.outputs.applicationInsightsName
    applicationInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    entraInstance: entraInstance
    entraTenantId: entraTenantId
    entraBackendAudience: entraBackendAudience
    entraApiScopeName: entraApiScopeName
    environmentName: environmentName
    databaseHost: dataSecurity.outputs.sqlServerFqdn
    databaseName: dataSecurity.outputs.sqlDatabaseName
    allowedOrigins: allowedOrigins
    enableVnetIntegration: enableVnetIntegration
    virtualNetworkSubnetResourceId: virtualNetworkSubnetResourceId
  }
}

module staticWebApp './modules/static-web-app.bicep' = {
  name: 'static-web-app'
  params: {
    name: staticWebAppName
    location: staticWebAppLocation
    functionAppId: functions.outputs.id
    functionAppRegion: location
    tags: tags
  }
}

output functionAppName string = functions.outputs.name
output functionAppId string = functions.outputs.id
output functionAppHostname string = functions.outputs.hostname
output functionAppPrincipalId string = functions.outputs.principalId
output functionPlanId string = functions.outputs.planId
output storageAccountName string = dataSecurity.outputs.storageAccountName
output storageAccountId string = dataSecurity.outputs.storageAccountId
output deploymentContainerName string = dataSecurity.outputs.deploymentContainerName
output deploymentContainerUri string = dataSecurity.outputs.deploymentContainerUri
output staticWebAppName string = staticWebApp.outputs.name
output staticWebAppHostname string = staticWebApp.outputs.defaultHostname
output sqlServerName string = dataSecurity.outputs.sqlServerName
output sqlServerFqdn string = dataSecurity.outputs.sqlServerFqdn
output sqlDatabaseName string = dataSecurity.outputs.sqlDatabaseName
output keyVaultName string = dataSecurity.outputs.keyVaultName
