# Master Authority 2.0 — Baby-Step Setup

## What this ZIP does
This is the central authorization gate for SL. It can oversee the Google Play Commerce Bridge, the In-Game Commerce & Auction System, and future systems through one Inspector list.

## 1. Install
1. Open your Unity SL project.
2. Copy `Unity/SL.MasterAuthority` into your Packages folder, or add it as a local package.
3. Let Unity compile.

## 2. Make the Authority object
1. In Hierarchy click `+`.
2. Create Empty GameObject.
3. Name it `MASTER AUTHORITY`.
4. Add Component → `Master Authority Client`.
5. Add Component → `Authority System Registry`.
6. Drag the Registry into the Client's `System Registry` field.
7. Keep this object alive across scenes.

## 3. Add systems with the + button
On `Authority System Registry` find **Connected Systems**.
Press **+**.
Drag the system's component into the new slot.
Press **+** again for another system.

For your current game, add:
- Google Play Commerce Authority adapter
- In-Game Commerce & Auction Authority adapter

Any future system that inherits `AuthorityManagedSystem` can be added to the same list. You do NOT create another Master Authority object.

## 4. Server
Set these environment variables on the secure server:
- `SOULBOUND_JWT_KEY`
- `SOULBOUND_ISSUER`
- `SOULBOUND_AUDIENCE`
- `SOULBOUND_DB`

Run `Server/sql/schema.sql` against PostgreSQL.

## 5. Important security rule
Adding a system to Unity makes the client route its requests through the authority gateway. The server still decides whether the system, capability, and action are allowed. Never put Google service-account credentials or database passwords in Unity.

## 6. Future systems
Do not make another general authorization server. Add the future system to `Connected Systems`, give it a unique System ID and capabilities, and register its server-side policy. The same Master Authority API remains the gate.

## 7. What this does NOT magically do
The universal gate cannot invent game rules for a brand-new system. A future system still needs its own server-side domain handler for its actual state changes. The Master Authority provides the common authentication, authorization, idempotency, rate limiting, audit/security boundary, and routing gate.
