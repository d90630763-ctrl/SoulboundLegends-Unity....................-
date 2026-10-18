# Connection to the rest of SL

## Lobby
The Lobby Store may call:
`CommerceBridgeManager.Buy(productId)`

The Lobby should only display product information and purchase state.

## Master Authority
After Google Play verification, the Commerce backend calls Master Authority.
Master Authority determines what the purchased product grants.

Example:
`google product sbl_diamond_100` -> `entitlement package diamond_100` -> Master Authority -> +100 Diamonds

The client never sends `+100 Diamonds` as an authority instruction.

## In-game Store
The normal in-game Store does not need this bridge when it spends existing in-game currency.

## Auction House
The Auction House does not need this bridge when it trades game-owned items/currency.

## Luck Royale
Luck Royale does not need this bridge for ordinary in-game spins. If a player first buys premium currency with real money, that purchase uses this bridge; the subsequent spin is handled by the game's server-authoritative Luck Royale system.
