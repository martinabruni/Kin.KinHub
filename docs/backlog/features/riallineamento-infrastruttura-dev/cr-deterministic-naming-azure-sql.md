# CR-FEAT-016-001 - Rendere univoci i nomi e adottare Azure SQL Basic

- **Feature interessata**: FEAT-016 `riallineamento-infrastruttura-dev`
- **Tipo**: revisione architetturale, infrastrutturale e del provider di persistenza
- **Stato**: approvata, non implementata
- **Readiness**: `ready`
- **Breaking change prodotto**: no
- **Breaking change tecnico**: si, per IaC, persistenza, migration e pipeline di release
- **Piano originario**: `feature.plan.md`
- **Linee guida originarie**: `infra-guidelines.md`

## Fonti autorevoli

| Fonte | Percorso o riferimento | Ruolo |
|---|---|---|
| Decisione della responsabile | Richiesta del 24 agosto 2026 | Approva il suffisso deterministico e la sostituzione di PostgreSQL con il profilo Azure SQL Basic a basso costo |
| Feature operativa | `feature.md` | Baseline implementata e vincoli di FEAT-016 da correggere |
| Piano infrastrutturale originario | `feature.plan.md` | Decisioni su naming esplicito, PostgreSQL, workflow e verifiche che questa CR sostituisce dove indicato |
| Linee guida infrastrutturali originarie | `infra-guidelines.md` | Guardrail di adozione, sicurezza, costo e delivery che restano validi salvo naming e provider database |
| Analisi funzionale | `docs/brainstorming/functional-analysis.md` | Comportamenti prodotto e requisiti che il cambio provider deve preservare |
| Architettura approvata | `docs/brainstorming/architecture.md` | Baseline PostgreSQL, managed identity, transazioni, schemi e ADR da superare esplicitamente |
| Istruzioni repository | `AGENTS.md` | Regole correnti PostgreSQL/Npgsql, migration, CI/CD e Definition of Done da aggiornare durante l'implementazione |
| Repository reale | `infra/`, `src/backend/`, `tests/`, `.github/workflows/` | Touchpoint e contratti provider-specifici esistenti |

## Motivazione

L'ambiente `dev` deve essere riprovisionato in una nuova subscription, dentro un resource group inizialmente vuoto. I nomi espliciti adottati dalla precedente subscription non sono una convenzione riutilizzabile e alcune risorse Azure richiedono unicita globale. La responsabile approva quindi un suffisso deterministico derivato dal contesto di deployment.

Il costo corrente di Azure Database for PostgreSQL Flexible Server non e proporzionato all'ambiente familiare `dev`. La responsabile approva la sostituzione con Azure SQL Database nel tier Basic a DTU, indicato come profilo da circa 5 euro al mese. Il riferimento economico e un obiettivo di budget, non un prezzo contrattuale: SKU, disponibilita regionale e stima corrente devono essere verificati prima di scrivere il Bicep definitivo.

## Comportamento attuale

- `infra/environments/dev.bicepparam` contiene nomi completi provenienti dalla subscription precedente.
- `feature.plan.md`, `infra-guidelines.md`, la skill infrastructure e i validatori vietano `uniqueString`, `namingPrefix` e nomi generati.
- Bicep crea Azure Database for PostgreSQL Flexible Server `Standard_B1ms`, database, amministratore Entra e firewall.
- Il backend usa EF Core con Npgsql, opzioni modellate su host/porta PostgreSQL, SQL provider-specifico e advisory lock PostgreSQL.
- Le migration contengono tipi, annotazioni e SQL PostgreSQL.
- La release usa output ARM `postgres*`, token `oss-rdbms`, `psql`, PL/pgSQL, grant PostgreSQL e una regola firewall temporanea.
- Test, health, telemetria, Problem Details, documentazione e skill espongono nomi e assunzioni PostgreSQL.

## Decisioni approvate

### CR-DEC-001 - Suffisso deterministico

Il suffisso autorevole e calcolato una sola volta nell'entry point Bicep con gli stessi seed dell'esempio approvato:

```bicep
var resourceNameSuffix = uniqueString(
  subscription().id,
  resourceGroup().id,
  applicationName,
  environmentName
)
```

Vincoli:

- `applicationName` identifica tecnicamente KinHub con valore normalizzato `kinhub`; `environmentName` resta `dev` nel parameter file corrente.
- Il suffisso si applica a tutte le risorse top-level create e gestite da Bicep nel resource group, con prefissi e suffissi semantici coerenti con il tipo di risorsa.
- La costruzione dei nomi e centralizzata in `infra/main.bicep`; i moduli ricevono nomi gia calcolati e non generano un secondo suffisso.
- Storage Account e ogni altra risorsa con vincoli speciali applicano lowercase, rimozione dei separatori e limite di lunghezza senza troncare o alterare il suffisso in modo non deterministico.
- Container, database, schema, role assignment e altre risorse figlie mantengono nomi funzionali stabili quando non richiedono unicita globale.
- Lo stesso template, con gli stessi seed e nello stesso scope, produce gli stessi nomi. Cambiare subscription, resource group, applicazione o ambiente produce intenzionalmente un nuovo suffisso.
- Il deployment ARM mantiene il nome stabile `kinhub-dev-infrastructure`: non e una risorsa applicativa da rinominare e resta la fonte degli output letti da `release.yml`.
- Il parameter file non duplica piu tutti i nomi completi; contiene soltanto decisioni ambientali che non possono essere derivate in modo sicuro.
- Non sono ammessi `newGuid()`, timestamp, casualita o discovery euristica per costruire i nomi.

Esempi di forma, da adattare ai limiti ufficiali del tipo di risorsa:

```text
kinhub-dev-<suffix>-func
kinhub-dev-<suffix>-sql
kinhubdev<suffix>
```

### CR-DEC-002 - Azure SQL Database Basic

Il termine colloquiale "SQL Server Basic" viene realizzato come:

- un Azure SQL logical server gestito da Bicep;
- un singolo Azure SQL Database nel purchasing model DTU e service tier Basic;
- collocazione `Italy North`, se il tier e disponibile nella regione target alla verifica tecnica;
- profilo minimo del tier Basic, senza elastic pool, zone redundancy, geo-replica, failover group o capacita serverless aggiuntive;
- dimensione massima coerente con il limite del tier Basic verificato al momento dell'implementazione;
- accesso pubblico controllato, TLS/cifratura obbligatori e firewall aperto solo alle origini necessarie;
- amministratore Microsoft Entra per bootstrap e migration;
- system-assigned managed identity della Function come identita runtime, senza username/password applicativi permanenti;
- migrazioni EF Core applicate dalla pipeline prima di One Deploy, mai durante il cold start di produzione;
- nessun backup/export applicativo aggiuntivo; restano accettati i soli comportamenti minimi obbligatori della piattaforma, da documentare senza presentarli come garanzia applicativa di ripristino.

Il costo di circa 5 euro/mese e una soglia orientativa. Il criterio verificabile e l'uso del tier Basic approvato, accompagnato dalla stima Azure corrente non sensibile; una differenza di disponibilita, modello o costo che richieda un altro SKU riapre una decisione umana e non autorizza una sostituzione automatica.

### CR-DEC-003 - Provider applicativo SQL Server

- EF Core 10 resta l'ORM e `Microsoft.EntityFrameworkCore.SqlServer` sostituisce Npgsql.
- Gli schemi logici `shared` e `kinlist`, le transazioni locali, i vincoli, la concorrenza ottimistica, la keyset pagination e i confini DDD restano invariati come comportamento.
- Configurazione, health e telemetria diventano provider-neutral dove il nome del prodotto non e parte necessaria del contratto.
- SQL provider-specifico, tipi, collation, precisione temporale, ordinamento `uniqueidentifier`, indici filtrati, lunghezze indicizzabili, upsert e lock devono essere adattati e verificati su SQL Server reale.
- I test di integrazione reali usano SQL Server, non un provider in-memory o SQLite come sostituto semantico.
- La migration history e la strategia di baseline/import dipendono da GATE-005; non si cancellano dati o si riscrive la storia senza la relativa decisione.

## Contratti invariati

- Nessun flusso, route, payload di successo, regola familiare o comportamento KinList cambia per effetto del provider.
- Una sola famiglia attiva, policy `Family`, scope `familyId`, isolamento, visibilita e autorizzazione restano server-side.
- Il database resta relazionale e condiviso da KinHub/KinService con separazione logica `shared`/`kinlist`.
- Domain e Business non dipendono dal provider EF o da Azure SQL.
- Runtime e pipeline usano identita separate e privilegi minimi; nessun segreto reale entra nel repository, nei log o negli artifact.
- Bicep resta resource-group scoped e il resource group non viene creato o modificato dal template.
- Provisioning `incremental`, validate, what-if, blocco dei cambi distruttivi e release build-once deploy-many restano obbligatori.
- Function App Flex, Static Web Apps, `/api`, Storage, Key Vault e osservabilita non cambiano architettura.

## Scope

### Incluso

- Convenzione centralizzata di naming deterministico per tutte le risorse top-level gestite nel resource group.
- Sostituzione della configurazione di nomi completi nel parameter file con input ambientali minimi.
- Sostituzione di Azure Database for PostgreSQL Flexible Server con Azure SQL logical server e singolo database Basic.
- Sostituzione di Npgsql con il provider EF Core SQL Server e adattamento dei contratti tecnici provider-specifici.
- Nuova serie di migration SQL Server o traduzione controllata della storia, secondo GATE-005.
- Autenticazione Entra/managed identity, utenti contenuti e grant minimi per migrator e runtime.
- Aggiornamento di output Bicep, app settings, options, health, error mapping, telemetria, workflow e test.
- Testcontainers SQL Server e test reali di transazioni, indici, concorrenza, paginazione, lock e migration.
- Aggiornamento di istruzioni, architettura tramite decisione superseding, runbook, skill, validatori, documentazione e change fragment.
- Provisioning e verifica live nella nuova subscription e nel resource group vuoto.

### Escluso

- Modifiche funzionali a KinHub, KinList, API o frontend.
- Azure SQL Managed Instance, SQL Server in VM/container, elastic pool, serverless, geo-replica, failover group o private networking.
- Doppia scrittura permanente o supporto contemporaneo PostgreSQL/SQL Server nel runtime.
- Cancellazione automatica del database PostgreSQL precedente o perdita implicita dei suoi dati.
- Aggiunta di nuovi ambienti oltre `dev`.
- Ridenominazione casuale a ogni deployment o discovery di risorse per prefisso.
- Garanzia di un prezzo mensile fisso o introduzione di alert/budget Azure non gia approvati.
- Riscrittura retroattiva di patch note, change fragment, piani completati o altra documentazione storica PostgreSQL.

## Dipendenze

### Feature prerequisite

La CR eredita le dipendenze hard gia soddisfatte o dichiarate da FEAT-016:

| Feature | Tipo | Motivo | Output richiesto | Effetto sul parallelismo |
|---|---|---|---|---|
| FEAT-001 - Entrare nel percorso corretto dopo il login | hard | Il nuovo provider deve preservare identita, policy, Problem Details, health e configurazione condivisa | Baseline backend/shared e contratti HTTP correnti | Nessun lavoro sul provider prima della baseline integrata |
| FEAT-015 - Raggiungere i servizi attivi della famiglia | hard | Migration e persistenza SQL Server devono includere catalogo e disponibilita gia integrati | Modello EF e migration applicative correnti | La conversione parte dal modello completo integrato |

### Gate bloccanti

| ID | Stato | Decisione richiesta | Impatto | Evidenza per chiudere |
|---|---|---|---|---|
| GATE-003 | closed | Rendere accessibili alla pipeline OIDC la nuova subscription e il resource group target | Senza accesso non sono possibili validate, what-if, provisioning e smoke test live | La responsabile conferma che l'accesso al target e consentito; l'implementazione deve comunque verificarlo con login OIDC, what-if e deployment live |
| GATE-005 | closed | Confermare se i dati PostgreSQL della subscription precedente devono essere conservati e migrati oppure se il nuovo database `dev` puo partire vuoto | Cambia migration history, strumenti di trasferimento, ordine di cutover, rollback e rischio dati | La responsabile approva un nuovo database `dev` vuoto; non e richiesto un trasferimento dati dalla subscription precedente |
| GATE-006 | closed | Confermare se il codice pubblico `dependency.postgresqlUnavailable` deve restare compatibile oppure essere sostituito con `dependency.databaseUnavailable` | Cambia un contratto Problem Details consumabile dalla PWA o da client esterni | La responsabile approva la rinomina coordinata a `dependency.databaseUnavailable` con aggiornamento coerente di API, client, OpenAPI, test e documentazione |

Non restano gate bloccanti aperti. La CR e `ready`; restano da eseguire le verifiche tecniche TECH-011-TECH-013 e l'intera consegna live in implementazione.

### Verifiche tecniche locali

| ID | Verifica | Trattamento | Evidenza attesa |
|---|---|---|---|
| TECH-011 | Confermare nome Bicep corrente, API version GA, SKU/capacity Basic DTU, max size, disponibilita in `Italy North` e stima mensile nella subscription target | Prima attivita infrastrutturale; nessun fallback automatico a uno SKU diverso | Documentazione/CLI Azure corrente, Bicep compilato, validate e stima non sensibile registrata |
| TECH-012 | Confermare il percorso supportato per amministratore Entra, token Azure SQL, creazione utenti contenuti mediante object ID/SID, client SQL del runner e grant post-migration | Attivita della pipeline; preservare identita separate e least privilege | Prova ripetibile in ambiente target con migrator DDL e runtime DML senza password persistenti |
| TECH-013 | Verificare collation, case sensitivity, ordinamento Guid, precisione date, tipi binari, indici filtrati/indicizzabili, upsert, lock e keyset pagination | Attivita backend e migration; adattare il provider senza cambiare semantica prodotto | Test SQL Server reali, migration bundle e piani/query rappresentativi verdi |

### Parallelismo consentito

Nessuno durante l'integrazione. Naming, provider EF, migration, Bicep, output ARM, workflow e documentazione autorevole formano un unico contratto instabile. Analisi e test di caratterizzazione possono essere preparati separatamente, ma le modifiche a `infra/`, persistenza e `.github/workflows/` devono essere serializzate nella stessa consegna.

## Contratto di consegna

### Comportamento

- Un deployment ripetuto nello stesso scope calcola gli stessi nomi e aggiorna le stesse risorse.
- Un deployment nella nuova subscription genera nomi globalmente distinti senza richiedere una lista manuale di nomi completi.
- Il primo provisioning del resource group vuoto crea Azure SQL Basic e le altre risorse senza dipendere da un deployment ARM precedente.
- La release recupera gli output SQL dal deployment stabile, prepara l'accesso temporaneo del runner, applica il migration bundle, assegna i privilegi runtime, distribuisce Function e frontend e chiude sempre l'accesso temporaneo.
- La Function usa managed identity per Azure SQL e non riceve password database.
- Readiness e smoke test distinguono un'app raggiungibile da un'app incapace di aprire una connessione al database.
- In caso di errore, what-if, migration e release falliscono esplicitamente senza eliminare il database precedente o dichiarare un cutover riuscito.

### Touchpoint previsti

- **Dominio/business**: nessuna modifica alle regole; aggiornare i nomi di errore/telemetria coerentemente con la rinomina approvata a `dependency.databaseUnavailable` senza introdurre dipendenze dal provider.
- **Persistenza/migration**: progetto Infrastructure, options/validator, DbContext e factory, hosted migration locale, configurazioni EF, repository con SQL specifico, intera migration history e runbook migration.
- **API/integrazioni**: readiness database, Problem Details, OpenAPI se cambia il codice pubblico e smoke test della dipendenza.
- **Frontend/UX**: nessuna UX; aggiornare il mapping del codice errore alla rinomina approvata.
- **Infrastruttura/configurazione**: `infra/main.bicep`, `infra/environments/dev.bicepparam`, `infra/modules/data-security.bicep`, `infra/modules/functions.bicep`, output ARM e configurazione runtime.
- **Delivery**: `.github/workflows/infrastructure.yml`, `.github/workflows/release.yml`, bundle EF, client SQL, token scope, firewall, utenti/grant e controlli distruttivi `Microsoft.Sql`.
- **Test**: package Testcontainers SQL Server, fixture reali, test di integrazione e contratti DI/configurazione/health.
- **Documentazione/conoscenza**: `AGENTS.md`, `README.md`, documenti architecture/development/operations applicabili, backlog, skill architecture/backend/infrastructure/implementation, registry generato e change fragment bilingue.

### Errori, sicurezza e osservabilita

- Non loggare access token Azure SQL, connection string, SID/object ID, nomi utenti, query parametrizzate con dati personali o output di migrazione sensibile.
- Il runtime ottiene solo permessi DML e uso degli schemi necessari; il migrator mantiene DDL soltanto nel job autorizzato.
- Le regole firewall del runner sono limitate al suo IP e rimosse con `always()` anche in caso di errore.
- L'accesso pubblico controllato non equivale a consentire `0.0.0.0` permanentemente a tutte le origini Azure senza una verifica esplicita.
- Il what-if blocca delete/replacement e cambi distruttivi su `Microsoft.Sql`, rete, Storage, Key Vault e risorse con dati.
- Health, metriche e trace usano un nome provider-neutral coerente con la rinomina approvata e non aumentano la cardinalita.
- Nessuna eccezione SQL interna o dettaglio di autenticazione viene restituito nei Problem Details.

## Sequenza di consegna

1. Registrare gli esiti chiusi di GATE-003, GATE-005 e GATE-006 nella CR e nell'indice backlog.
2. Eseguire TECH-011-TECH-013 e congelare naming map, SKU, configurazione Azure SQL, autenticazione e strategia migration.
3. Aggiungere test di caratterizzazione dei contratti provider-neutral e delle semantiche SQL a rischio.
4. Convertire provider EF, options, health, SQL specifico, migration e Testcontainers come un unico contratto verificabile.
5. Convertire Bicep al naming deterministico e ad Azure SQL Basic, mantenendo il deployment ARM stabile e output coerenti.
6. Convertire infrastructure/release workflow, principal/grant, firewall, bundle e readiness senza introdurre secret.
7. Aggiornare nello stesso change regole strutturali, ADR superseding, runbook, skill, validatori, registry e change fragment.
8. Eseguire build, test, migration bundle, packaging, validatori, Bicep build/build-params, actionlint e controlli di sicurezza.
9. Eseguire validate e what-if nella nuova subscription; applicare il provisioning soltanto senza modifiche distruttive inattese.
10. Eseguire la release sul nuovo database vuoto approvato e verificare runtime ARM, health, version, accesso applicativo e telemetria.

## Criteri di accettazione

### AC-CR-001 - Naming deterministico e univoco

- **Dato** lo stesso template KinHub, ambiente e resource group nella stessa subscription
- **Quando** Bicep viene compilato o distribuito piu volte
- **Allora** tutte le risorse top-level usano nomi derivati dal singolo `uniqueString(subscription().id, resourceGroup().id, applicationName, environmentName)`, rispettano i vincoli del provider e restano identiche tra i deployment
- **Fonte**: CR-DEC-001

### AC-CR-002 - Unicita tra scope differenti

- **Dato** uno dei seed subscription, resource group, applicazione o ambiente differente
- **Quando** viene calcolata la naming map
- **Allora** il suffisso cambia, mentre i nomi restano leggibili, deterministici e privi di casualita o timestamp
- **Fonte**: CR-DEC-001

### AC-CR-003 - Database Azure SQL Basic

- **Dato** il resource group `dev` vuoto e TECH-011 chiusa
- **Quando** il workflow Infrastructure applica il Bicep approvato
- **Allora** crea un logical server e un singolo Azure SQL Database Basic nella regione approvata, senza PostgreSQL, elastic pool, serverless, replica o alta disponibilita aggiuntiva
- **Fonte**: CR-DEC-002

### AC-CR-004 - Persistenza SQL Server equivalente

- **Dato** il modello applicativo e le invarianti correnti
- **Quando** build, migration e test reali vengono eseguiti con EF Core SQL Server
- **Allora** schemi, vincoli, transazioni, concorrenza, paginazione, ordinamenti, cleanup e query preservano i comportamenti approvati senza Npgsql o SQL PostgreSQL residuo nel codice attivo
- **Fonte**: CR-DEC-003; ADR-002 e ADR-004 da superare

### AC-CR-005 - Accesso identity-based e least privilege

- **Dato** Function, migrator e Azure SQL provisionati
- **Quando** migration e runtime aprono connessioni
- **Allora** il migrator applica DDL nel job, la Function opera con la propria managed identity e soli privilegi runtime, e nessuna password database permanente e richiesta dall'applicazione
- **Fonte**: CR-DEC-002; NFR-004, NFR-005

### AC-CR-006 - Release ripetibile su ambiente nuovo

- **Dato** il deployment ARM `kinhub-dev-infrastructure` riuscito
- **Quando** `release.yml` legge gli output e distribuisce la release
- **Allora** usa i nomi SQL deterministici, apre e chiude l'accesso temporaneo del runner, applica il bundle SQL Server prima di One Deploy e verifica anche la connessione database senza discovery euristica
- **Fonte**: FEAT-016 AC-093, AC-094, AC-096; CR-DEC-001-CR-DEC-003

### AC-CR-007 - Nessuna perdita dati implicita

- **Dato** il gate dati chiuso con approvazione di un database `dev` vuoto
- **Quando** avviene il cutover
- **Allora** il nuovo target parte da una baseline documentata senza trasferimento dati dalla subscription precedente e PostgreSQL non viene cancellato automaticamente come effetto collaterale del cutover
- **Fonte**: GATE-005; NFR-006

### AC-CR-008 - Contratti pubblici coordinati

- **Dato** il gate compatibilita chiuso con rinomina approvata
- **Quando** viene gestita un'indisponibilita del database
- **Allora** API, PWA, OpenAPI, test, telemetria e documentazione usano coerentemente il codice `dependency.databaseUnavailable` senza esporre dettagli SQL
- **Fonte**: GATE-006; NFR-007

### AC-CR-009 - Regole e fonti riallineate

- **Dato** l'implementazione della CR
- **Quando** vengono eseguiti i validatori repository e si leggono le fonti autorevoli correnti
- **Allora** `AGENTS.md`, ADR superseding, skill, runbook, backlog, Bicep e workflow concordano su naming deterministico e Azure SQL, mentre gli artefatti storici restano riconoscibili come tali
- **Fonte**: CR-DEC-001-CR-DEC-003; Definition of Done repository

### AC-CR-010 - Verifica live completa

- **Dato** la nuova subscription accessibile tramite OIDC
- **Quando** Infrastructure e Release terminano
- **Allora** what-if e deployment sono verdi, runtime ARM e SKU corrispondono al contratto, `health/live`, `health/ready`, `/api/version`, flusso Static Web Apps `/api` e telemetria confermano un'applicazione funzionante con Azure SQL
- **Fonte**: GATE-003; FEAT-016 AC-096

## Strategia di verifica

| Livello | Verifica | Evidenza attesa |
|---|---|---|
| Unitario | Naming map con seed uguali/diversi e vincoli per tipo; options e mapping errori | Test deterministici senza chiamate Azure |
| Integrazione SQL Server | Migration da database vuoto, modello, vincoli, query, transazioni, lock, concorrenza, keyset e cleanup | Testcontainers SQL Server eseguito realmente in CI e locale supportato |
| Contratto backend | DI, managed identity/token callback isolata, health, Problem Details e telemetria | Test senza segreti e senza dipendenza dalla rete per i componenti isolabili |
| Packaging | Migration bundle e ZIP Function con provider SQL Server | Bundle eseguibile e package backend valido |
| Infrastruttura statica | Bicep format/build/build-params, output e workflow | Compilazione, linter, actionlint e validatori repository verdi |
| Azure live | Validate, what-if, provisioning, migration, release e verifica costo/SKU | Run GitHub Actions e stato ARM sulla nuova subscription |
| End-to-end | Home/API protette, readiness database e smoke test Static Web Apps -> Function -> Azure SQL | Risposte attese, dati di prova non personali e telemetria redatta |

## Definition of Done

- GATE-003, GATE-005 e GATE-006 sono chiusi con evidenze registrate; TECH-011-TECH-013 sono verificate.
- AC-CR-001-AC-CR-010 sono soddisfatti senza cambiare lo scope prodotto.
- Non restano riferimenti PostgreSQL/Npgsql nei contratti attivi, salvo artefatti storici esplicitamente preservati.
- Naming, Bicep, parameter file, output ARM, options, migration, workflow, test e documentazione condividono una sola fonte di verita.
- `AGENTS.md` e le skill architecture/backend/infrastructure/implementation sono aggiornati nello stesso change strutturale e il registry e rigenerato.
- Sono aggiornati runbook migration/deployment/observability, documentazione locale, change fragment bilingue e artefatti derivati applicabili.
- Restore, build, test, publish, migration bundle, packaging, validatori docs/skills/release, Bicep e actionlint passano con esiti riportati.
- Infrastructure e Release sono eseguite nella nuova subscription; runtime, SKU, health, version, `/api` e telemetria sono verificati live.
- Nessun secret, token, connection string, dato personale o identificativo sensibile e versionato o stampato.
- FEAT-016 puo tornare da `Open` a `In progress` solo all'avvio dell'implementazione e a `In review` soltanto dopo la consegna completa; `Completed` richiede sempre il comando esplicito della responsabile.
