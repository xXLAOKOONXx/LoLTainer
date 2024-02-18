# Events
This is a list of all Events available in the app.
An event describes a specific situation on which all Actions added to the event will be triggered.
Below you can find all available Events grouped by the underlying APIs.
## InGame Events
- PlayerAnyKill: Gets triggered on every kill a player performs. Gets triggered before PlayerMultiKills, PlayerFirstBlood or PlayerFirstKill get processed
- PlayerSingleKill: Gets triggered if the player scores a kill that is first of its kind and not already a multikill (Does not get triggered on FirstBlood(!))
- PlayerMultiKills (PlayerDoubleKill, PlayerTripleKill, PlayerQuodraKill, PlayerPentaKill): Gets triggered when the player scores a multikill of the respective kind
- PlayerFirstBlood: Gets triggered when the player scores a first blood, participating in the first blood does not trigger this Action
- PlayerDragonKill & PlayerBaronKill: Gets triggered if the player does the killing blow of the respective objective, assisting does not trigger this Action
- BaronSteal, DragonSteal: Gets triggered if the player steals the respective objective, if this is set up and gets triggered the respective Kill Event will not get triggered
- NexusDestroyed (any | enemy | own): Gets triggered if the game ends, if no specific (team/enemy) NexusDestroyed Event is set up the AnyNexusDestroyed Event will get triggered. Pay attention to the description as the event pays attention on whos nexus falls, not who destroys the nexus
## Client Events
- EnterChampSelect: Gets triggered when the player enters the champion select
- EnterGame: Get triggered when the player enters the game, this might happen already in loading screen(!)
- EndGame: Whether win or loose, when the player is no longer in a game this Action gets active

  
## Future Events
Here goes a list of potential new Events:
- InGame
    - TeamAnyKill, TeamSingleKill, TeamMultiKills, TeamFirstBlood, TeamDragonKill, TeamBaronKill (TeamEvents will not get triggered if there is a PlayerEvent set for the event)
    - TeamAce
    - PlayerTurretKill, PlayerTurretFirstBlood, PlayerTurretAssist, PlayerTurretFirstBloodAssist, TeamTurretKill, TeamTurretFirstBlood
    - BaronSteal, DragonSteal (Team)
- Client
    - EnterQueue
    - ChampionPick
    - ChampionSelectCancel