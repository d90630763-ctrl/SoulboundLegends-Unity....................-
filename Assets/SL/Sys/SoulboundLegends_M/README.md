# SL — Master Authority System 2.0 FINAL

This ZIP is the central server-authorization layer for SL.

It is designed to oversee:
- Google Play Commerce Bridge
- In-Game Commerce & Auction System
- future systems through one Inspector `Connected Systems` [+] list

## Core rule
CLIENT REQUEST → MASTER AUTHORITY → DOMAIN SERVER VALIDATION → DATABASE COMMIT → RESULT

The client never becomes the source of truth for money, currency, inventory, auction ownership, rewards, purchases or permanent progression.

### Important
This is a reusable authorization/gateway foundation. It does not automatically know the private business rules of every future system. Each system still needs a server-side domain handler. The purpose is that you never need to build another general authorization server: future systems plug into the same Master Authority gate.

See `Docs/BEGINNER_SETUP.md` first.
