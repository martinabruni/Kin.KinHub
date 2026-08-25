using '../main.bicep'

param applicationName = 'kinhub'
param environmentName = 'dev'
param location = 'italynorth'
param staticWebAppLocation = 'westeurope'
param runtimeName = 'dotnet-isolated'
param runtimeVersion = '10.0'
param instanceMemoryMB = 2048
param maximumInstanceCount = 20
param alwaysReadyInstanceCount = 0
param deploymentBlobContainerName = 'function-packages'
param azureTenantId = '<AZURE_TENANT_ID>'
param entraInstance = 'https://<ENTRA_TENANT_SUBDOMAIN>.ciamlogin.com/'
param entraTenantId = '<ENTRA_TENANT_ID>'
param entraBackendAudience = '<ENTRA_BACKEND_CLIENT_ID>'
param entraApiScopeName = 'access_as_user'
param sqlEntraAdministratorName = '<SQL_ENTRA_ADMIN_NAME>'
param sqlEntraAdministratorObjectId = '<SQL_ENTRA_ADMIN_OBJECT_ID>'
param sqlAdministratorLogin = '<SQL_ADMIN_LOGIN>'
param sqlAdministratorPassword = '<SQL_ADMIN_PASSWORD>'
param allowedOrigins = [
  'http://localhost:5173'
]
param enableVnetIntegration = false
param virtualNetworkSubnetResourceId = ''
param enablePurgeProtection = true
param logRetentionDays = 30
param logDailyCapGb = 1
param tags = {
  workload: 'kinhub'
  environment: 'dev'
  owner: 'martinabruni'
  costClassification: 'personal-low-cost'
}
