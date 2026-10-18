using System;
using UnityEngine;

namespace SoulboundLegends.RiftSystem
{
    public sealed class RiftUiViewModel : MonoBehaviour, IRiftUiProvider
    {
        [SerializeField] private int priority = 0;

        public string ProviderId => name;
        public int Priority => priority;
        public System.Collections.Generic.IReadOnlyCollection<RiftCapability> Capabilities =>
            new[] { RiftCapability.Ui };
        public bool IsAvailable => isActiveAndEnabled;

        public event Action<RiftStateSnapshot> SnapshotReceived;

        public void PushSnapshot(RiftStateSnapshot snapshot) => SnapshotReceived?.Invoke(snapshot);

        public string GetRankDisplay(RiftStateSnapshot snapshot) => "???";

        public string GetObjectiveDisplay(RiftStateSnapshot snapshot) => "KILL THE FINAL BOSS.";

        public long GetServerRemainingMilliseconds(RiftStateSnapshot snapshot, long serverNow) =>
            Math.Max(0, snapshot.ExpirationUnixMilliseconds - serverNow);
    }
}
