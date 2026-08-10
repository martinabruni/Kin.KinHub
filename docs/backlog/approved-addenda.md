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

### Decisione aperta

- **GATE-004**: deve essere confermato l'esito del bootstrap quando la creazione o una sincronizzazione richiesta non dispone di un claim `name` utilizzabile. La scelta deve precisare se mantenere il valore precedente e `NeedBootstrap = true`, completare con il fallback applicativo oppure fallire il bootstrap; non è ammessa una cancellazione o sostituzione silenziosa del dato.
