using './main.bicep'

param staticWebAppLocation = 'westeurope'
param staticWebAppName = 'kin-kinhub-swa'
param functionAppName = 'kin-kinhub-func'
param storageAccountName = 'kinkinhubstorage'
param keyVaultName = 'kvkinhubdev'
param sqlServerName = 'sql-kinhub-dev'
param sqlDatabaseName = 'sql-db-kinhub-dev'

// Fill secure values before deployment.
param sqlAdministratorLogin = ''
param sqlAdministratorPassword = ''
param iftttWebhookUrl = ''
