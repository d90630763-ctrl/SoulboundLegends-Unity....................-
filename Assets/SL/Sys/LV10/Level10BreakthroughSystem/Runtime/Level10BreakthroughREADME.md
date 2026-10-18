# Level 10 Breakthrough System

Purpose:
- Gives a configurable progression boost, normally +0.5 level.
- Hard-caps the result at Level 10.
- Becomes unavailable at Level 10.
- Supports server authorization.
- Keeps the progression value configurable for live updates.

Integration:
- Connect to the main XP/Level system through your final integration layer.
- The final server should validate charges and progression before calling the authoritative grant.
- Do not trust a client-provided level, charge count, or authorization result.
