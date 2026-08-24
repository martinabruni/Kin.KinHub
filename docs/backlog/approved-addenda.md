# Addenda approvati del backlog

Questo documento registra requisiti approvati dalla responsabile dopo il consolidamento di `docs/brainstorming/functional-analysis.md` e `docs/brainstorming/architecture.md`. Gli addenda estendono il backlog senza modificare retroattivamente gli identificatori delle fonti consolidate e senza promuovere raccomandazioni di ricerca.

## Profilo applicativo nel bootstrap - 10 agosto 2026

### Requisiti approvati

- **ADD-001 - Display name persistito**: quando il bootstrap registra un nuovo `ApplicationUser`, KinHub persiste il valore del claim `name` in una proprietà e colonna aggiuntiva chiamata `DisplayName`.
- **ADD-002 - Sincronizzazione richiesta dal backend**: `ApplicationUser` possiede una proprietà e colonna booleana `NeedBootstrap`, con default `false`. Per un profilo già esistente il backend usa il valore per stabilire se sincronizzare nuovamente i dati di profilo durante il bootstrap; dopo una sincronizzazione persistita con successo il valore torna `false`.
- **ADD-003 - Nome client autorevole**: il client recupera `DisplayName` dal backend e usa quel valore per mostrare il nome dell'utente, senza usare il nome esposto da MSAL come fonte applicativa.

### Vincoli preservati

- `(iss, oid)` resta l'unica identità esterna canonica; `name` non diventa una chiave, un fallback identificativo o una prova di autorizzazione.
- Il bootstrap post-login continua a verificare autorevolmente membership e contesto famiglia a ogni esecuzione prevista. `NeedBootstrap` controlla soltanto la sincronizzazione dei dati di profilo e non consente di saltare i controlli di accesso esistenti.
- `DisplayName` è un dato personale: non viene inserito in log, metriche, trace, cursori o storage applicativo del browser.
- La migrazione inizializza `NeedBootstrap` a `false` anche per i profili esistenti; un profilo viene aggiornato soltanto dopo essere stato esplicitamente contrassegnato per la sincronizzazione.

### Fuori scope dell'addendum

- UI o API self-service per modificare `DisplayName`.
- UI o API amministrative per impostare `NeedBootstrap`.
- Sincronizzazione continua del nome a ogni richiesta o uso del nome MSAL come fallback applicativo.
- Modifiche a ruoli, membership, policy `Family` o disponibilità dei KinService.

### Decisione aperta al momento dell'addendum

- **GATE-004**: deve essere confermato l'esito del bootstrap quando la creazione o una sincronizzazione richiesta non dispone di un claim `name` utilizzabile. La scelta deve precisare se mantenere il valore precedente e `NeedBootstrap = true`, completare con il fallback applicativo oppure fallire il bootstrap; non è ammessa una cancellazione o sostituzione silenziosa del dato.

## Creazione esplicita del profilo prima della famiglia - 10 agosto 2026

Questa decisione sostituisce `ADD-001` e `ADD-002` e chiude `GATE-004`: il claim `name` non alimenta più il profilo applicativo. `ADD-003` resta valido ed è precisato da `ADD-006`.

### Requisiti approvati

- **ADD-004 - Profilo prima della famiglia**: al primo accesso riconosciuto, dopo il collegamento dell'identità `(iss, oid)` e prima di mostrare o consentire l'onboarding familiare, KinHub chiede all'utente di creare il proprio profilo applicativo. Finché il profilo non è completo, l'utente non può creare una famiglia né diventare membro tramite codice.
- **ADD-005 - Display name fornito dall'utente**: nella versione corrente la creazione del profilo richiede soltanto un `DisplayName` valido fornito dall'utente. KinHub lo persiste nel profilo applicativo; il claim MSAL `name` non viene copiato, sincronizzato o usato come fallback.
- **ADD-006 - Profilo backend autorevole nel client**: completato il profilo, il bootstrap restituisce al client il `DisplayName` persistito. Tutte le superfici client che mostrano il nome dell'utente usano il valore ricevuto dal backend e non il nome esposto da MSAL.

### Vincoli preservati

- `(iss, oid)` resta l'unica identità esterna canonica e può creare idempotentemente il record applicativo necessario a rappresentare lo stato di profilo incompleto; il completamento del profilo non modifica identità, membership o autorizzazioni.
- Il bootstrap post-login continua a verificare autorevolmente profilo e membership a ogni esecuzione prevista. Un profilo incompleto produce uno stato distinto e prioritario rispetto all'onboarding familiare.
- `DisplayName` è un dato personale: non viene inserito in log, metriche, trace, cursori o storage applicativo del browser.
- I profili già esistenti non vengono riscritti dal claim MSAL. Se non dispongono di un display name, le superfici condivise mantengono i fallback localizzati `Membro`/`Member` e `?` già approvati, senza reintrodurre il token come fonte applicativa.

### Fuori scope dell'addendum

- Avatar, immagine di copertina, data di compleanno e qualunque altro campo profilo futuro.
- Pagina profilo e modifica self-service del `DisplayName` dopo il completamento iniziale.
- Sincronizzazione del profilo da MSAL o uso di `name`, email o username come fallback applicativo.
- Modifiche a ruoli, policy `Family`, numero di famiglie o disponibilità dei KinService.

### Verifica tecnica locale

- **TECH-010**: la prima feature interessata deve definire e verificare normalizzazione, limiti e vincoli di persistenza del `DisplayName` in modo coerente tra dominio, API, database e UI, senza cambiare il requisito del solo campo obbligatorio né impedire i nomi nelle lingue supportate.
