using UnityEngine;

namespace SoulboundLegends.RiftSystem
{
    public sealed class RiftServerApi : MonoBehaviour
    {
        [SerializeField] private RiftSystem riftSystem;

        public RiftRequestResult Enter(string playerId, string riftId, string nonce) =>
            riftSystem.EnterRift(playerId, riftId, nonce);

        public RiftRequestResult Exit(string playerId, string riftId, string nonce) =>
            riftSystem.ExitRift(playerId, riftId, nonce);

        public RiftRequestResult DefeatBoss(string playerId, string riftId, string bossSpawnId, string nonce) =>
            riftSystem.RequestBossDefeat(playerId, riftId, bossSpawnId, nonce);

        public bool Restore(string playerId, string riftId) =>
            riftSystem.TryRestorePlayer(playerId, riftId);
    }
}
