# Actions
This is a list of all Actions available in the app.
## InGame Actions
- PlayerAnyKill: Gets triggered on every kill a player performs. Gets triggered before PlayerMultiKills, PlayerFirstBlood or PlayerFirstKill get processed
- PlayerSingleKill: Gets triggered if the player scores a kill that is first of its kind and not already a multikill (Does not get triggered on FirstBlood(!))
- PlayerMultiKills (PlayerDoubleKill, PlayerTripleKill, PlayerQuodraKill, PlayerPentaKill): Gets triggered when the player scores a multikill of the respective kind
- PlayerFirstBlood: Gets triggered when the player scores a first blood, participating in the first blood does not trigger this Action
- PlayerDragonKill & PlayerBaronKill: Gets triggered if the player does the killing blow of the respective objective, assisting does not trigger this Action
## Client Actions
- EnterChampSelect: Gets triggered when the player enters the champion select
- EnterGame: Get triggered when the player enters the game, this might happen already in loading screen(!)
- EndGame: Whether win or loose, when the player is no longer in a game this Action gets active
