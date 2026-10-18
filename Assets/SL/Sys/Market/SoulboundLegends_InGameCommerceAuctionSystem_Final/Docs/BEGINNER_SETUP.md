# Beginner Setup

## Unity
1. Open your SL Unity project.
2. Create `Assets/SL/InGameCommerce`.
3. Copy the contents of `Unity/Runtime` into that folder.
4. In Unity wait for compilation.
5. Create an empty GameObject named `InGameCommerceSystem`.
6. Add `InGameCommerceClient`.
7. Set the Oracle API Base URL to your secure server URL.
8. Set the player's authenticated session token from your login/authentication system.
9. Do not put database passwords, signing secrets, Google credentials, or admin keys in Unity.

## Server
1. Install the supported .NET SDK for your server environment.
2. Copy `Server/SoulboundInGameCommerce` to your Oracle server project area.
3. Configure environment variables from `.env.example`.
4. Create the database tables using `Server/sql/schema.sql`.
5. Connect the service to your existing Master Authority authentication/session system.
6. Run the API behind HTTPS.
7. Only authenticated player requests may reach player transaction endpoints.

## Connecting shops
Each shop sends a generic request containing a `merchantId`, such as:
- `city_general_shop`
- `town_general_shop`
- `kingdom_general_shop`
- `blacksmith_shop`
- `alchemy_shop`

The server owns the catalog and prices. The client must never send a trusted price.

## Connecting auctions
Auction UI calls:
- Create listing
- Place bid
- Cancel listing when allowed
- Get active listings
- Get my listings
- Get my bids
- Claim/finalize completed auction

The server owns the timer and winner calculation.
