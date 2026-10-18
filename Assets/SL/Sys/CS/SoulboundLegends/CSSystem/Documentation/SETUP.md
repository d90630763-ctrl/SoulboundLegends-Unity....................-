# CS_World_System GameObject setup

Add these components to one GameObject:
- CSWorldSystem
- CSMatchManager
- CSTeamManager
- CSRoundManager
- CSScoringSystem
- CSSpawnManager
- CSSpectatorManager
- CSMatchmakingManager
- CSMapSiteManager
- CSBoundaryManager
- CSAFKDisconnectManager
- CSResultRewardManager
- CSServerValidationBridge
- CSIntegrationBridge
- CSGameBootstrap

Then assign the component references on CSWorldSystem.

The CSWorldSystem inspector has:
- Core CS settings
- 5 parent map definitions
- 7 sites per map
- Shared System Connections
- CS manager references

The General HUD is shared. Do not create a second CS HUD.
