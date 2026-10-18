# Finishing Animation System

Core behavior:
1. Detect nearby potential targets.
2. A finisher is eligible only when the target is at or below its configured HP percentage.
3. Range and optional line-of-sight are checked.
4. Ownership is checked for shop-unlocked finishers.
5. The server must authorize the final request.
6. The configured AnimationClip/Animator trigger, VFX, and audio are used by the final presentation integration.
7. The authoritative combat/death system performs the actual death. The finisher system does not trust the client to kill a target.
8. Finishers can also be triggered through the Skill System when `skillCanTriggerFinisher` is enabled.

Shop:
- Create a FinishingAnimationDefinition.
- Set `purchasable = true`.
- Give it a unique `shopProductId`.
- Connect the bridge to the authoritative Shop/Purchase/Inventory systems.
- After confirmed purchase, ownership is granted.

Important:
- The included ownership store is intentionally a bridge. The final integrated game must use the real server-backed player inventory/account ownership.
- Do not put premium currency or purchase authorization in client-only code.
- Mode restrictions (CS/BR/Siege), player level requirements, and other live-game rules should be supplied by the final Game Mode/Update systems.
