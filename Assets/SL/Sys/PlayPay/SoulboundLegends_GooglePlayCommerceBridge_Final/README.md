# SL — Google Play Commerce Bridge (Final)

## Purpose
This package is the real-money commerce bridge between the SL app and Google Play.

It does NOT implement the in-game Store, Auction House, Luck Royale, events, inventory, economy, or progression.
Those systems may request a paid product through this bridge, but Master Authority remains responsible for granting the actual entitlement.

## Purchase authority
APP/UI -> Google Play Billing -> purchase token -> secure backend verification -> Master Authority -> entitlement/economy

Never grant paid currency/items because the Unity client merely says a purchase succeeded.

## Current Google Play requirements
Google's current Billing guidance requires purchase verification before granting benefits. Pending purchases must not receive entitlement until they become PURCHASED. Google also recommends sending purchase tokens to a secure backend and processing purchases there.

This package therefore separates:
- Unity client: starts purchase and forwards purchase evidence.
- Commerce backend: verifies Google Play purchase state, idempotency, acknowledgement/consumption hooks.
- Master Authority: decides and commits the game's entitlement.

## Important
The server adapter in this package is intentionally a production-oriented integration boundary. You must supply your Google Play service-account credentials and connect the verification adapter to the Google Play Developer API before production release.
