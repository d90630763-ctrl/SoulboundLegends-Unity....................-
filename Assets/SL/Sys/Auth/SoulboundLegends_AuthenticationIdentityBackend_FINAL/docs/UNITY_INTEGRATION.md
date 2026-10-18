# Unity integration

Import `unity/com.soulboundlegends.authentication` as a local UPM package.

The Unity side contains only the API client and generic system connection component.

## Never put these in Unity
- PostgreSQL credentials
- JWT signing private key
- Google server/client secret
- recovery webhook secret

## Opening flow

1. Opening Screen finishes loading.
2. Login screen obtains provider credentials through the selected identity SDK.
3. Unity sends the provider ID token + nonce to this backend.
4. Backend returns access/refresh tokens and identity state.
5. Store tokens in an OS-protected secure storage implementation in the final game project.
6. If `characterExists` is false, route to Character Creation.
7. Character Creation obtains server-generated Player ID/Character ID from the Player Database/Character Generation system.
8. Route to Lobby.
9. Network Transport presents the access token before trusted gameplay communication.
10. Server Authority derives the player identity from the authenticated server context.

The supplied `AuthenticationSystemConnections` component intentionally uses generic capability names and `MonoBehaviour` slots so future Soulbound systems can be connected without rewriting the authentication core.
