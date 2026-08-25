# Stato implementazione: FEAT-016 - Riallineare infrastruttura e delivery dell'ambiente dev

- Aggiornato (UTC): `2026-08-25T11:03:00Z`
- Branch: `dev`
- Commit di partenza: `e1d35a02b35a09145f2765d9f442f6ca3eb44b91`
- Motivo checkpoint: `work in progress; esecuzione workflow/live verification su nuova subscription abilitata ancora da completare`

## Scope e decisioni

- La CR FEAT-016-001 e stata implementata localmente come conversione unica di backend, migration, test reali, IaC, workflow, skill e runbook verso naming deterministico e Azure SQL Database Basic.
- La migration history PostgreSQL/Npgsql e stata sostituita da una baseline SQL Server unica, coerente con il database `dev` vuoto approvato da GATE-005.
- Il codice pubblico di indisponibilita database e stato rinominato in `dependency.databaseUnavailable` e la telemetria famiglia usa l'outcome provider-neutral `database_unavailable`.
- Il worktree contiene anche modifiche backlog preesistenti della responsabile su `docs/backlog/**`; non sono state revertite.
- La subscription corretta indicata dalla responsabile e `2f7df82f-fe5f-4ead-9dc6-1825f3c46cdf` (`DA-SUB`), in stato `Enabled`; `rg-kinhub-dev` esiste ed e vuoto.
- L'environment GitHub `dev` e stato riallineato ai nuovi nomi workflow con `AZURE_SUBSCRIPTION_ID=2f7df82f-fe5f-4ead-9dc6-1825f3c46cdf`, secret `SQL_ADMIN_LOGIN`/`SQL_ADMIN_PASSWORD` e variables `SQL_ENTRA_ADMIN_NAME`/`SQL_ENTRA_ADMIN_OBJECT_ID`.

## Completato

- Backend/infrastructure convertiti a SQL Server: package EF, connection handling identity-based, hosted migration lock via `sp_getapplock`, repository e configurazioni provider-specifiche in `src/backend/infrastructure/DA.KinHub.Infrastructure/**`.
- Nuova baseline EF SQL Server in `src/backend/infrastructure/DA.KinHub.Infrastructure/Persistence/Migrations/20260825103350_InitialSqlServerBaseline*.cs` con seed catalogo KinService.
- Test e contratti aggiornati: business/integration tests, bundle EF, package backend, frontend validators e docs sync.
- IaC convertito a suffisso deterministico e Azure SQL in `infra/main.bicep`, `infra/modules/data-security.bicep`, `infra/modules/functions.bicep`, `infra/environments/dev.bicepparam`.
- Workflow e validatori aggiornati in `.github/workflows/*.yml`, `tools/skill-harness/index.mjs`, `tools/skill-harness/README.md`.
- Fonti autorevoli aggiornate: `AGENTS.md`, skill `architecture/backend/infrastructure`, `README.md`, runbook operativi, guide troubleshooting e fragment `changes/none-changed-deterministic-azure-sql-dev.md`.

## Modifiche in corso

- `.github/workflows/release.yml`: conversione a Azure SQL pronta localmente ma non verificata da GitHub Actions reali per blocco subscription target.
- `docs/backlog/features/riallineamento-infrastruttura-dev/feature.md`: stato portato a `In progress`; non puo passare a `In review` senza validazione live, commit/push, PR e check verdi.

## Verifiche

| Comando | Esito | Dettaglio utile |
|---|---|---|
| `dotnet build KinHub.slnx --configuration Release` | `pass` | Soluzione compilata con provider SQL Server e nuova baseline migration. |
| `dotnet test KinHub.slnx --configuration Release --no-build` | `pass` | 68 test passati, 3 integration SQL Server skip per Docker/connection string non disponibile. |
| `dotnet ef migrations add InitialSqlServerBaseline --project src/backend/infrastructure/DA.KinHub.Infrastructure --context KinHubDbContext --output-dir Persistence/Migrations` | `pass` | Generata la baseline `20260825103350_InitialSqlServerBaseline`. |
| `dotnet ef migrations bundle --project src/backend/infrastructure/DA.KinHub.Infrastructure --configuration Release --force --output artifacts/migrations/kinhub-migrations` | `pass` | Bundle EF SQL Server creato. |
| `powershell.exe -ExecutionPolicy Bypass -File scripts/package-backend.ps1 -Environment Development -SkipBuild` | `pass` | ZIP backend prodotto in `artifacts/backend/`. |
| `npm.cmd run skills:build` | `pass` | Registry skill rigenerato. |
| `npm.cmd run skills:validate` | `pass` | Skill e contratti repository validi. |
| `npm.cmd run docs:validate` | `pass` | Documentazione valida. |
| `npm.cmd run docs:sync` | `pass` | JSON docs frontend rigenerato. |
| `npm.cmd run release:validate` | `pass` | Change fragment valido. |
| `npm.cmd run release:generate` | `pass` | Patch notes e `src/frontend/public/release-notes.json` rigenerati. |
| `npm.cmd run --prefix src/frontend test` | `pass` | 32 test frontend verdi. |
| `npm.cmd run --prefix src/frontend lint` | `pass` | ESLint verde. |
| `npm.cmd run --prefix src/frontend typecheck` | `pass` | TypeScript verde. |
| `npm.cmd run --prefix src/frontend i18n:validate` | `pass` | Namespace allineati. |
| `npm.cmd run --prefix src/frontend routes:validate` | `pass` | Route documentate valide. |
| `npm.cmd run --prefix src/frontend design-system:validate` | `pass` | Validator design system verde. |
| `npm.cmd run --prefix src/frontend build` | `pass` | Build Vite/PWA completata. |
| `az bicep build --file infra/main.bicep` | `pass` | Template Bicep compila con Azure SQL e suffisso deterministico. |
| `az bicep build-params --file infra/environments/dev.bicepparam` | `pass` | Parameter file compila. |
| `az account show --subscription 2f7df82f-fe5f-4ead-9dc6-1825f3c46cdf` | `pass` | Subscription corretta `DA-SUB` visibile e `Enabled`. |
| `az group show --subscription 2f7df82f-fe5f-4ead-9dc6-1825f3c46cdf --name rg-kinhub-dev` | `pass` | Resource group target esistente in `italynorth`. |
| `az resource list --subscription 2f7df82f-fe5f-4ead-9dc6-1825f3c46cdf --resource-group rg-kinhub-dev --query "[].{name:name,type:type,location:location}" --output table` | `pass` | Nessuna risorsa presente: il target reale e vuoto. |
| `gh auth status` | `pass` | GitHub CLI autenticato come `martinabruni`. |
| `gh variable set --env dev SQL_ENTRA_ADMIN_NAME/SQL_ENTRA_ADMIN_OBJECT_ID` | `pass` | Variables environment aggiornate per il workflow infrastrutturale SQL. |
| `gh secret set --env dev SQL_ADMIN_LOGIN/SQL_ADMIN_PASSWORD/AZURE_SUBSCRIPTION_ID` | `pass` | Secret environment allineati al nuovo target Azure SQL e alla subscription corretta. |
| `Microsoft Learn: Reactivate a disabled Azure subscription` | `pass` | La documentazione Microsoft indica portale/billing/support come percorso di riattivazione; non emerge un'azione ARM/CLI standard per sbloccare una subscription disabilitata. |
| `actionlint` | `non eseguito` | Go/actionlint non disponibile nella sessione Windows locale. |

## Pull request e GitHub Actions

- Pull request: `non ancora aperta`
- SHA monitorato: `non ancora disponibile`
- Stato Actions: `non eseguito; prossimo passo e push su dev e dispatch di infrastructure/release sul target corretto`

## Lavoro residuo

- [ ] Verificare diff e stato Git, creare un commit pulito su `dev` con le sole modifiche FEAT-016/CR-001.
- [ ] Pushare `dev` e lanciare `infrastructure.yml` e `release.yml` in `workflow_dispatch` sul ref `dev`.
- [ ] Monitorare validate/what-if/deploy live, verificare runtime ARM/SKU/output e smoke test della release.
- [ ] Aprire PR verso `main` e monitorare tutte le GitHub Actions dell'ultimo SHA fino a `success`.
- [ ] Solo dopo esito live e CI verdi aggiornare `feature.md` da `In progress` a `In review` e rimuovere questo checkpoint.

## Human in the loop

Nessuno al momento. Il nuovo target Azure corretto e scrivibile; resta da eseguire il ciclo live completo su GitHub Actions.

## Ripresa

Prima azione concreta: creare un commit pulito su `dev`, pusharlo e lanciare `infrastructure.yml` in `workflow_dispatch` sul ref `dev` per ottenere `validate`, `what-if` e provisioning live sul target corretto.
