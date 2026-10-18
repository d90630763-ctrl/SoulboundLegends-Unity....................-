using UnityEngine;

namespace SoulboundLegends.Runtime
{
    /// <summary>Keyboard test combat. Mobile UI can call AttackNearest() through a Unity Button event.</summary>
    public sealed class SLRuntimeCombatInput : MonoBehaviour
    {
        public float attackRange = 3.5f;
        public float damage = 50f;
        public KeyCode attackKey = KeyCode.J;
        void Update() { if (Input.GetKeyDown(attackKey)) AttackNearest(); }
        public void AttackNearest()
        {
            var rig = GetComponent<SLProceduralActorRig>(); if (rig != null) rig.PlayAction("Attack", .42f);
            var actors = FindObjectsByType<SLRuntimeCombatant>(FindObjectsSortMode.None);
            SLRuntimeCombatant best = null; float bestDistance = attackRange;
            foreach (var actor in actors)
            {
                if (!actor || actor.IsDead || actor.transform == transform) continue;
                float d = Vector3.Distance(transform.position, actor.transform.position);
                if (d < bestDistance) { best = actor; bestDistance = d; }
            }
            if (best) best.ApplyDamage(damage);
        }
        public void HeavyAttack() { AttackNearest(); var rig=GetComponent<SLProceduralActorRig>(); if(rig)rig.PlayCombo("heavy attack"); }
        public void Block() { var rig=GetComponent<SLProceduralActorRig>(); if(rig)rig.PlayAction("Block",.7f); }
        public void Dodge() { var rig=GetComponent<SLProceduralActorRig>(); if(rig)rig.PlayAction("Dodge",.38f); }
        public void Jump() { var rig=GetComponent<SLProceduralActorRig>(); if(rig)rig.PlayAction("Jump",.55f); }
    }
}
