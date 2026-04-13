using './main.bicep'

param staticWebAppLocation = 'westeurope'
param staticWebAppName = 'kin-kinhub-swa'
param webAppName = 'kin-kinhub-api'
param keyVaultName = 'kvkinhubdev'
param sqlServerName = 'sql-kinhub-dev'
param sqlDatabaseName = 'sql-db-kinhub-dev'

// Fill secure values before deployment.
param sqlAdministratorLogin = ''
param sqlAdministratorPassword = ''
param jwtSecret = ''
