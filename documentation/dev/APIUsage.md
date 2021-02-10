# API Usage
## Client Api (LCU)
- GET /lol-summoner/v1/current-summoner
    - Using Summonername and Summonericon for more friendly User Interface (later implementation)
- /wamp (WebSocket) + OnJsonApiEvent_lol-gameflow_v1_session
    - Detect the moment when Player enters InGame
    - Opens oportunities to trigger championselect events
## In Game API (127.0.0.1:2999/*)
- /liveclientdata/eventdata
    - Track Champion and EpicMonster Kills to trigger sound events
    - No Socket possible, so a frequent request is needed
    - Frequency: 0.2 seconds
- /liveclientdata/activeplayer + /liveclientdata/playerlist
    - Identify active Player and its team
    - Request on new game 