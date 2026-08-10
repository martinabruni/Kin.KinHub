# Stato implementazione: FEAT-004 - Consultare le impostazioni della famiglia

- Aggiornato (UTC): `2026-08-06T13:09:35Z`
- Branch: `dev`
- Commit di partenza: `dc827da9fe93b031da4422b0ae6ef2fe093ab58e`
- Motivo checkpoint: `human in the loop; riallineamento richiesto sullo stato FEAT-004`

## Scope e decisioni

- `docs/backlog/features/impostazioni-famiglia/feature.md` e' in stato `In review`; non va portata autonomamente a `Completed`.
- Il read-side previsto da `feature.plan.md` e' presente: dettaglio famiglia, pagine membri/inviti, cursori opachi separati, fallback frontend e assenza del codice segreto nella proiezione pubblica.
- La UI non espone azioni invito o leave: il commit corrente aggiunge anche i flussi backend di creazione, join e revoca, ma non cambia il confine passivo della pagina FEAT-004.
- La feature e' verificata localmente, ma non e' dimostrata nel deployment dev: restano necessari runtime ARM, migration/grant, smoke test autenticati, fallback SPA, asset PWA e telemetria live.
- `feature.md` continua a riferire `bug-family-settings-deployment-404.md`, ma quel file risulta cancellato nel worktree gia' prima di questa sessione; la cancellazione e l'eventuale risoluzione del bug non sono state validate.

## Completato

- Domain/Business/Infrastructure per Family settings: `FamilyInvitation`, migration EF `20260730202708_AddFamilyInvitations`, repository di dettaglio e pagine keyset e codec Data Protection distinti per membri/inviti.
- API protette `GET /api/kinhub/families/details`, `GET /api/kinhub/families/members` e `GET /api/kinhub/families/invitations`, con policy `Family`, Problem Details, OpenAPI statico/runtime e telemetria a cardinalita' finita.
- Frontend `/settings/family` sotto `ProtectedRoute`, integrazione `SettingsPage`, client API tipizzato, stati loading/empty/error/403/offline/inconsistent, paginazione indipendente, help, route registry, guide it/en e change fragment.
- Artefatti documentali e release precedentemente rigenerati; il commit corrente `dc827da` aggiunge i flussi backend di invito senza aggiungere affordance non funzionanti alla pagina Family.

## Modifiche in corso

- `docs/backlog/features/impostazioni-famiglia/bug-family-settings-deployment-404.md`: cancellazione presente nel worktree; il riferimento in `feature.md` e' ora orfano finche' non viene confermata, ripristinata o sostituita.
- `implementation-progress.md`: checkpoint riallineato al commit e alla feature effettivamente analizzati.

## Verifiche

| Comando | Esito | Dettaglio utile |
|---|---|---|
| Skill `KinHub repository implementation workflow` | `pass` | Letta prima dell'aggiornamento del checkpoint. |
| `git status --short --branch` / `git log -1 --oneline` | `pass` | Branch `dev`, HEAD `dc827da`; unica modifica preesistente rilevata: cancellazione del bug report. |
| `dotnet build KinHub.slnx --configuration Release --no-restore` | `pass` | Build senza warning/errori. |
| `dotnet test KinHub.slnx --configuration Release --no-build` | `pass` | 64 test passati, 5 integration PostgreSQL skip per harness Docker/connection string non disponibile. |
| `npm.cmd run --prefix src/frontend test` | `pass` | 32 test passati in 10 file. |
| `npm.cmd run --prefix src/frontend lint` | `pass` | ESLint completato senza errori. |
| `npm.cmd run --prefix src/frontend typecheck` | `pass` | TypeScript completato senza errori. |
| `npm.cmd run --prefix src/frontend build` | `pass` | Build Vite/PWA completata; prebuild ha validato docs e rigenerato metadata localmente. |
| `npm.cmd run --prefix src/frontend i18n:validate` | `pass` | 4 namespace bilingui allineati. |
| `npm.cmd run --prefix src/frontend routes:validate` | `pass` | 9 route documentate. |
| `npm.cmd run --prefix src/frontend design-system:validate` | `pass` | Design system valido. |
| `npm.cmd run docs:validate` | `pass` | 7 pagine valide per 2 lingue. |
| `npm.cmd run skills:validate` | `pass` | 7 skill valide e route/OpenAPI coperte. |
| `powershell.exe -ExecutionPolicy Bypass -File scripts/package-backend.ps1 -Environment Development -SkipBuild` | `pass` | ZIP One Deploy e checksum prodotti per `dc827da9fe93`. |
| `dotnet ef` migration list/script/bundle | `non eseguito` | Nessuna verifica di artifact migration separata in questa sessione. |
| Smoke test browser e deployment dev | `non eseguito` | Nessuna evidenza live per route Family, `health/live`, `api/version`, Static Web Apps, service worker o telemetria. |
| `gh pr list --head dev --state all ...` | `non eseguito` | GitHub API ha restituito HTTP 502; PR e check non sono stati verificati. |

## Pull request e GitHub Actions

- Pull request: `non ancora aperta o non verificabile`
- SHA monitorato: `non ancora disponibile`
- Stato Actions: `non eseguito; API GitHub non disponibile (HTTP 502)`

## Lavoro residuo

- [x] Implementare localmente il read-side FEAT-004 e i relativi contratti backend/frontend.
- [x] Eseguire build, test, lint, typecheck, build PWA, validatori e packaging locale sul commit corrente.
- [ ] Decidere la cancellazione di `bug-family-settings-deployment-404.md` e riallineare il riferimento in `feature.md`, senza dichiarare risolto il bug in assenza di evidenza.
- [ ] Eseguire migration/grant e verifiche live nell'ambiente dev, incluse route Family, runtime, `health/live`, `health/ready`, `api/version`, fallback SPA, asset `staticwebapp.config.json`, service worker e telemetria redatta.
- [ ] Ripetere o abilitare le cinque integration PostgreSQL skip e completare la verifica separata di migration list/script/bundle.
- [ ] Verificare diff e stato Git, creare il commit su `dev`, pushare, aprire PR verso `main` e monitorare tutti i check dell'ultimo SHA fino a `success`.

## Human in the loop

Serve conferma sulla cancellazione del bug report referenziato dalla feature, accesso Azure alla subscription target per le verifiche live e accesso GitHub funzionante per commit/push, apertura PR e monitoraggio delle Actions. Non sono necessari secret nel checkpoint.

## Ripresa

Prima azione concreta: confermare o ripristinare il bug report cancellato, poi eseguire la verifica live del deployment dev sulle tre route Family e sull'asset PWA prima di preparare commit, push e PR.
