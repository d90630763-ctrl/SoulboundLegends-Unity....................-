# Persistent data coverage

Account/player identity, character customization, progression, attributes/statistics, skills, settings/HUD/controls/audio/graphics/localization, inventory/equipment, materials/consumables, currencies and ledger, purchases/refunds/rewards, trades, quests/objectives/rewards, discoveries/lore/monster weaknesses, achievements, social relationships/parties/mail, reputation/PK/bounty/wanted state, match history/participants/rewards/rank/disconnect records, cities, Rifts/participants, Supreme Ruins/divine-creature progress/inheritance history, idempotency, audit and migration history are represented by the schema.

Future connected systems can add dedicated tables through numbered migrations without modifying the core provider contract.
