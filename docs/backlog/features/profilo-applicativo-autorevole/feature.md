---
status: Open
---

# FEAT-017 - Creare il profilo prima di entrare in una famiglia

- **Codice**: `profilo-applicativo-autorevole`
- **Tipo**: `product`
- **Readiness**: `ready`
- **Wave**: 4
- **Risultato**: al primo accesso l'utente fornisce il proprio display name prima dell'onboarding familiare e il client mostra esclusivamente il nome profilo restituito dal backend.

## Contesto autonomo

Il bootstrap attuale crea idempotentemente un `ApplicationUser` dalla coppia verificata `(iss, oid)` e instrada subito verso una famiglia attiva o verso la scelta crea/unisciti. Il client usa ancora in alcune superfici il nome dell'account MSAL, mentre le proiezioni backend dei membri non dispongono di un profilo autorevole completo.

La feature introduce uno stato di profilo incompleto per il primo accesso. Dopo il collegamento dell'identità, KinHub chiede un solo dato, il `DisplayName`, e non permette azioni familiari finché il completamento non è persistito. Agli accessi successivi il bootstrap restituisce il profilo applicativo e prosegue verso famiglia o onboarding senza ripresentare il passaggio. Il disegno deve consentire l'aggiunta futura di altri campi profilo senza implementarli ora.

## Scope

### Incluso

- Stato autorevole di profilo applicativo incompleto/completo, separato dall'identità esterna e dalla membership.
- Passaggio post-login prioritario che raccoglie un solo `DisplayName` obbligatorio, con loading, validazione, invio, errore recuperabile e successo.
- Persistenza del display name fornito dall'utente e completamento idempotente del profilo.
- Blocco server-side della creazione famiglia finché il profilo non è completo, senza affidarsi al solo instradamento frontend; FEAT-005 applica lo stesso prerequisito al join prima di introdurlo.
- Estensione compatibile del bootstrap per restituire stato e profilo necessari al client senza esporre claim MSAL.
- Uso del `DisplayName` backend in menu utente, navigazione, membri, autori e altre superfici esistenti; fallback localizzati approvati quando un profilo storico non dispone del valore.
- Migration con verifica e rollback, contratti OpenAPI/client, localizzazione `it`/`en`, help/guida, accessibilità, telemetria redatta e test end-to-end.

### Escluso

- Avatar, immagine di copertina, data di compleanno o altri campi profilo futuri.
- Pagina profilo, modifica self-service del display name dopo il completamento iniziale o amministrazione dei profili.
- Copia o sincronizzazione del claim MSAL `name`, email, username o altri dati del token nel profilo.
- Modifiche a identità canonica, ruoli, membership, policy `Family`, disponibilità dei KinService o numero di famiglie attive.

## Tracciabilità

| Tipo | Riferimenti | Contributo della feature |
|---|---|---|
| Flussi | FLOW-001, FLOW-009, FLOW-011 | Inserisce il profilo obbligatorio tra riconoscimento e onboarding familiare |
| Requisiti | ADD-004-ADD-006; FR-001-FR-003, FR-031-FR-033 | Profilo iniziale, display name persistito e nome client autorevole |
| Regole/decisioni | BR-001, BR-002, BR-021, BR-024, BR-036; DEC-002, DEC-015-DEC-017, DEC-031 | Mantiene identità fail-closed, famiglia unica e localizzazione |
| Architettura | ADR-002, ADR-003, ADR-009-ADR-011, ADR-016; sezioni 4, 6.1, 8 e 9 | Estende profilo/bootstrap nei layer esistenti senza usare MSAL come modello applicativo |

## Dipendenze

### Feature prerequisite

| Feature | Tipo | Motivo | Output richiesto | Effetto sul parallelismo |
|---|---|---|---|---|
| FEAT-001 - Entrare nel percorso corretto dopo il login | hard | Serve il collegamento identità, il bootstrap protetto e la policy `ApiAccess` | `ApplicationUser`, risoluzione `(iss, oid)`, endpoint bootstrap e mapping errori | Inizio solo dopo integrazione FEAT-001 |
| FEAT-002 - Creare la propria famiglia | hard | Il nuovo gate deve proteggere il caso d'uso famiglia già esistente e il relativo onboarding | Contratto create, membership e stato onboarding corrente | Inizio solo dopo integrazione FEAT-002 |
| FEAT-014 - Usare un design system condiviso in tutta KinHub | hard | Il passaggio profilo deve usare form, stati e navigazione condivisi senza UI legacy | Primitive form/state, shell, floating navigation e regole i18n | Inizio solo dopo integrazione FEAT-014 |
| FEAT-004 - Consultare le impostazioni della famiglia | contract | La proiezione del membro corrente deve usare lo stesso profilo autorevole delle superfici globali | Contratto membro `displayName`/iniziali e fallback congelato in CP-006 | Lavoro parallelo solo con ownership file e contratto concordati |
| FEAT-015 - Raggiungere i servizi attivi della famiglia | contract | Home e gate KinService consumano lo stesso bootstrap esteso con lo stato profilo | Unione compatibile degli stati bootstrap e del context condiviso in CP-006 | Lavoro parallelo solo dopo congelamento del contratto bootstrap |

### Gate e assunzioni

| ID | Stato | Impatto | Evidenza per chiudere |
|---|---|---|---|
| GATE-004 | closed | Il claim `name` non è più necessario per creare o sincronizzare il profilo | `ADD-004`-`ADD-006` in `docs/backlog/approved-addenda.md` |
| TECH-010 | open, non bloccante | Allinea validazione e storage del display name senza modificare lo scope | Vincoli dominio/API/DB/UI documentati e testati con whitespace, limiti e nomi nelle lingue supportate |

### Parallelismo consentito

Può procedere con feature che non modificano bootstrap, `ApplicationUser`, migration shared, API client, context utente, floating navigation o proiezioni membro/autore. Con FEAT-004 e FEAT-015 richiede CP-006 congelato; le migration shared e le modifiche ai contratti centrali restano serializzate.

## Contratto di consegna

### Comportamento

- Un'identità nuova con `iss` e `oid` validi viene collegata idempotentemente a un record applicativo incompleto e riceve dal bootstrap lo stato profilo, non l'onboarding famiglia.
- Il form profilo mostra soltanto il display name. Un valore valido viene persistito una sola volta, completa il profilo e fa proseguire al bootstrap autorevole verso famiglia o onboarding.
- Input invalido, richiesta ripetuta, concorrenza o errore non creano profili duplicati e non consentono di saltare il passaggio; l'input non sensibile resta disponibile per riprovare.
- Create family rifiuta un profilo incompleto con Problem Details stabile anche se invocato fuori dalla PWA; il contratto di completezza viene congelato per FEAT-005.
- Un profilo completo non riceve nuovamente il form agli accessi successivi. Il bootstrap restituisce il suo display name insieme allo stato familiare compatibile.
- Il client non legge `account.name` o equivalenti MSAL per presentare l'utente. Quando un profilo storico non ha un display name usa esclusivamente i fallback localizzati backend/UI già approvati.

### Touchpoint previsti

- **Dominio/business**: `src/backend/domains/DA.KinHub.Domain/Identity`, `src/backend/business/DA.KinHub.Business/Identity` per completezza profilo, valore display name, bootstrap e guard del caso d'uso famiglia.
- **Persistenza/migration**: `src/backend/infrastructure/DA.KinHub.Infrastructure/Persistence`, `ApplicationUser`/configurazione/repository, migration shared con trattamento non distruttivo dei profili esistenti e procedura di verifica/rollback.
- **API/integrazioni**: Function bootstrap e completamento profilo protette da `ApiAccess`, route/OpenAPI condivisi, Problem Details e API client tipizzato.
- **Frontend/UX**: provider/context bootstrap, Home e gate KinService, form onboarding, `AccountProfileContext.tsx`, `Layout.tsx`, `FloatingBars.tsx`, `FamilySettingsPage.tsx`, proiezioni autori e risorse `it`/`en`; nessuna route dedicata è imposta se il passaggio resta nello scaffold Home esistente.
- **Infrastruttura/configurazione**: nessuna nuova risorsa; eventuali limiti configurabili seguono options tipizzate e validazione all'avvio.
- **Documentazione/operazioni**: help onboarding, guide utente bilingui, OpenAPI, migrazione database e change fragment.

### Errori, sicurezza e osservabilità

- Token o claim canonici mancanti falliscono chiusi; display name, email e claim di profilo non identificano né autorizzano l'utente.
- Validazione restituisce Problem Details stabile senza riecheggiare il display name nei log; conflitti e retry non sovrascrivono silenziosamente un profilo completo.
- Nessun display name, token o claim completo entra in log, metriche, trace, browser storage, service worker o cache applicative.
- Metriche e trace distinguono bootstrap profilo richiesto, completamento riuscito, validazione, conflitto e guasto tecnico usando sole dimensioni a bassa cardinalità.

## Criteri di accettazione

### AC-097 - Il primo accesso richiede il profilo

- **Dato** un'identità valida mai collegata prima a un profilo completo
- **Quando** termina il bootstrap post-login
- **Allora** KinHub mostra il passaggio profilo con il solo display name e non mostra ancora le azioni crea/unisciti o dati familiari
- **Fonte**: ADD-004, FLOW-001, NFR-001

### AC-098 - Il display name completa il profilo

- **Dato** un profilo incompleto e un display name valido
- **Quando** l'utente conferma, anche in presenza di retry di trasporto
- **Allora** il backend persiste un solo valore, completa il profilo e il bootstrap prosegue verso famiglia o onboarding secondo la membership corrente
- **Fonte**: ADD-005, FR-002, BR-001, NFR-006

### AC-099 - Il profilo incompleto non aggira il gate

- **Dato** un utente autenticato con profilo incompleto
- **Quando** tenta di creare una famiglia direttamente tramite API
- **Allora** l'operazione è rifiutata senza creare famiglia, membership o altri effetti parziali e la PWA resta sul passaggio profilo
- **Fonte**: ADD-004, FR-003, BR-002, NFR-005

### AC-100 - Il profilo completo viene riusato

- **Dato** un profilo già completato, con o senza membership attiva
- **Quando** l'utente accede nuovamente o aggiorna la PWA
- **Allora** il form profilo non ricompare e il bootstrap restituisce display name e stato familiare autorevoli senza sincronizzare dati da MSAL
- **Fonte**: ADD-004-ADD-006, FR-032, DEC-016

### AC-101 - Il client usa soltanto il nome backend

- **Dato** un nome MSAL diverso dal display name persistito
- **Quando** il client mostra menu utente, navigazione, membro corrente o autore
- **Allora** tutte le superfici mostrano il valore backend e nessuna usa il nome MSAL come fonte o fallback applicativo
- **Fonte**: ADD-006, ADR-003, NFR-005

### AC-102 - Errori, fallback e privacy restano sicuri

- **Dato** input invalido, errore di rete/backend, profilo storico senza display name o tema/lingua supportati
- **Quando** KinHub carica o completa il profilo e presenta il nome
- **Allora** stati e recupero sono accessibili e localizzati, il profilo incompleto non avanza, il profilo storico usa `Membro`/`Member` e `?`, e nessun display name compare in telemetria o storage browser
- **Fonte**: ADD-005, ADD-006, BR-021, NFR-004, NFR-008-NFR-010

## Strategia di verifica

| Livello | Verifica | Evidenza attesa |
|---|---|---|
| Unitario | Valore display name, transizione incompleto/completo, idempotenza e guard famiglia | Test dominio/business per validazione, retry, conflitto e profilo già completo |
| Integrazione | Migration, unicità `(iss, oid)`, persistenza, bootstrap, completamento e rifiuto create family | Test PostgreSQL e contratto HTTP con rollback e profili storici |
| Frontend/component | Stato profilo, form, errori, focus e tutte le proiezioni del nome | Test componenti/accessibilità che impostano un nome MSAL divergente e ne escludono il rendering |
| End-to-end/manuale | Prima registrazione -> profilo -> crea famiglia -> Home; accesso successivo e refresh | Evidenza desktop/mobile `it`/`en`, temi supportati e nessun bypass via URL/API |
| Validator repository | Backend build/test/publish/package; frontend test/lint/typecheck/build/design system/i18n/routes/docs; skill/release | Tutti i comandi applicabili completati e riportati |

## Definition of Done

- Tutti i criteri di accettazione sono verificati; le dipendenze hard sono integrate e CP-006 è rispettato.
- TECH-010 è chiusa con vincoli coerenti tra dominio, API, persistenza, OpenAPI e UI.
- Migration include trattamento dei profili esistenti, verifica e rollback documentati senza ricavare dati da MSAL.
- Bootstrap, endpoint profilo e create family hanno test di autorizzazione, idempotenza, concorrenza, Problem Details e parità OpenAPI-route; il contratto di completezza è documentato per FEAT-005.
- Tutte le superfici client sono verificate per escludere il nome MSAL; i dati personali non vengono persistiti nel browser o emessi in telemetria.
- Testi, help, guide `it`/`en`, accessibilità, temi, mobile/PWA, change fragment e artefatti docs generati sono aggiornati.
- Sono eseguiti e riportati i comandi applicabili di `AGENTS.md`, inclusi publish/package backend e validatori frontend/documentazione/skill.
- Non sono introdotti campi profilo futuri, modifica self-service, ruoli, nuove risorse o altri elementi out of scope.
