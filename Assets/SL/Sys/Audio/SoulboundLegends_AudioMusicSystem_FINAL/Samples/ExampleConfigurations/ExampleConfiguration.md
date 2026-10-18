# Example configuration

Create an AudioProfile:
- Footstep + Grass: add 3-4 grass clips.
- Footstep + Stone: add 3-4 stone clips.
- SkillImpact + Fire: add fire impact clips.
- BossIntro: add boss introduction clip.
- RiftWarning: add warning clip.
- GodInheritance: add inheritance clip.

Create a SoulboundMusicManager:
- + Add Music -> Village Peaceful Theme -> AdventureWorld/Village -> assign clip -> Loop -> 1.5 sec crossfade.
- + Add Music -> Boss Theme -> BossBattle -> assign clip -> high priority -> crossfade.
- + Add Music -> Victory -> Victory -> assign clip -> one-shot/appropriate transition.

Call PlayContext(MusicContext.Village, "Village") when the world system enters the village.
Call PlayContext(MusicContext.BossBattle) when authoritative boss-start state is received.
