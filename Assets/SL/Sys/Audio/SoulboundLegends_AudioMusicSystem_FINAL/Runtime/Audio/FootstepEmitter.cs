using UnityEngine;
namespace SoulboundLegends.AudioMusic
{
    public sealed class FootstepEmitter : MonoBehaviour
    {
        [SerializeField] SoulboundAudioSystem audioSystem;
        [SerializeField] MonoBehaviour surfaceProvider;
        [SerializeField] float minStepInterval=.18f;
        float nextTime;
        public void Step(MovementState movement){
            if(Time.time<nextTime||!audioSystem)return; nextTime=Time.time+minStepInterval;
            var surface=surfaceProvider is ISoulboundSurfaceProvider p?p.GetSurface(transform.position):SurfaceType.Auto;
            audioSystem.PlayFootstep(transform.position,movement,surface);
        }
    }
}
