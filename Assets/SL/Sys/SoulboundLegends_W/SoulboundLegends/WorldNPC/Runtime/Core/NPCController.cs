using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SoulboundLegends.WorldNPC
{
    [DisallowMultipleComponent]
    public class NPCController : MonoBehaviour
    {
        public NPCGeneratedIdentity identity = new();
        public NPCNavigationData navigation = new();
        public List<NPCRoutineStep> routine = new();
        public List<NPCServiceBinding> services = new();
        public List<string> approachDialogueTags = new();
        public NPCVoiceProfile voiceProfile;
        public NPCSystemRegistry registry;
        public float interactionDistance = 3f;
        public bool pauseRoutineDuringInteraction = true;

        private NavMeshAgent agent;
        private bool interacting;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (!registry) registry = GetComponentInParent<NPCSystemRegistry>();
        }

        private void Update()
        {
            if (!interacting) RunRoutineSafely();
        }

        private void RunRoutineSafely()
        {
            // Full routine scheduling can be driven by a connected Navigation System.
            // This component intentionally does not override a connected provider.
        }

        public void SetInteracting(bool value) { interacting = value; if (agent && pauseRoutineDuringInteraction) { if (value) agent.isStopped = true; else agent.isStopped = false; } }
        public bool HasService(NPCServiceType type) => services.Exists(s => s != null && s.enabled && s.service == type);
        public void FacePlayer(Transform player)
        {
            if (!player) return;
            Vector3 d = player.position - transform.position; d.y = 0;
            if (d.sqrMagnitude > .001f) transform.rotation = Quaternion.LookRotation(d.normalized, Vector3.up);
        }
    }
}
