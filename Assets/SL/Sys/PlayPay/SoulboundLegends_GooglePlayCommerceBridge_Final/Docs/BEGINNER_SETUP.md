# Beginner Setup

## What this system does
Think of it like a locked door between your game and Google Play:

1. Player presses Buy in your Lobby Store.
2. Lobby calls Commerce Bridge.
3. Commerce Bridge opens Google Play's purchase screen.
4. Google Play handles the money transaction.
5. Google Play returns a purchase token to the app.
6. App sends that token to your secure Oracle server.
7. Server asks Google Play for the real purchase status.
8. Server accepts it only when the purchase is genuinely PURCHASED.
9. Server asks Master Authority to grant the approved product entitlement.
10. The app refreshes its authoritative player state.

## Unity import
1. Open your SL Unity project.
2. In the Project window create: Assets/SL/.
3. Copy the folder `Unity/Runtime` into `Assets/SL/CommerceBridge/Runtime`.
4. Let Unity compile.
5. Create an empty GameObject in the startup scene.
6. Name it `GooglePlayCommerceBridge`.
7. Add the `CommerceBridgeManager` component.
8. Set the Backend URL to your secure HTTPS Oracle API.
9. Set the Product Catalog entries to exactly match the product IDs you create in Play Console.
10. Do not put Google service-account credentials in Unity.

## Google Play product setup
In Play Console:
1. Create the app with the same Android application ID you build from Unity.
2. Set up your Google Payments/merchant profile.
3. Add the one-time products you want to sell.
4. Give each product a stable product ID such as `sbl_diamond_100`.
5. Configure price and availability in Play Console.
6. Publish an app build containing Play Billing to a test track before testing purchases.
7. Add license testers.

Google's documentation says billing-related product configuration requires a Play Developer account and linked Google Payments profile. See the official Play Billing setup documentation.

## Testing
Use a test track and license tester. Test at minimum:
- successful purchase
- user cancellation
- pending purchase
- network loss after payment
- duplicate purchase callback
- app restart before verification finishes
- purchase made on another device/account session
- already-processed purchase token
- refund/revocation path

## Security rules
NEVER put these in Unity:
- Google service-account private key
- Play Console API credentials
- merchant credentials
- database passwords
- Oracle private keys

The client may contain public product IDs and your HTTPS API address.

## Important architecture rule
The Commerce Bridge does not directly add Diamonds, Gold, items, XP, or anything else to the player.
It sends a verified commerce event to Master Authority. Master Authority performs the actual entitlement/economy commit.
