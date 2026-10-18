using UnityEngine;

namespace SoulboundLegends.RiftSystem
{
    public sealed class RiftLiveOpsService : MonoBehaviour
    {
        [SerializeField] private RiftSystem riftSystem;

        public bool Spawn(RiftAdminCommand command)
        {
            if (!Authorize(command, "Spawn"))
                return false;

            return riftSystem.TryCreateRift(command.CityId, command.RiftTypeId, out _);
        }

        public bool SetDuration(RiftAdminCommand command)
        {
            if (!Authorize(command, "ForceSetDuration") || !command.DurationSeconds.HasValue)
                return false;

            return riftSystem.ForceSetDuration(command.AdministratorId, command.RiftId,
                command.DurationSeconds.Value, command.Reason);
        }

        public bool SetAttempts(RiftAdminCommand command)
        {
            if (!Authorize(command, "ForceSetAttempts") || !command.AttemptLimit.HasValue)
                return false;

            return riftSystem.ForceSetAttempts(command.AdministratorId, command.RiftId,
                command.AttemptLimit.Value, command.Reason);
        }

        public bool ForceEscalate(RiftAdminCommand command) =>
            Authorize(command, "ForceEscalate") &&
            riftSystem.ForceEscalate(command.AdministratorId, command.RiftId, command.Reason);

        public bool ForceEngulf(RiftAdminCommand command) =>
            Authorize(command, "ForceEngulf") &&
            riftSystem.ForceEngulf(command.AdministratorId, command.RiftId, command.Reason);

        public bool ForceClear(RiftAdminCommand command) =>
            Authorize(command, "ForceClear") &&
            riftSystem.ForceClear(command.AdministratorId, command.RiftId, command.Reason);

        public RiftStateSnapshot Inspect(string administratorId, string riftId, string reason)
        {
            var command = new RiftAdminCommand
            {
                AdministratorId = administratorId,
                RiftId = riftId,
                Reason = reason
            };

            return Authorize(command, "Inspect") ? riftSystem.GetSnapshot(riftId) : null;
        }

        private bool Authorize(RiftAdminCommand command, string action)
        {
            if (string.IsNullOrWhiteSpace(command.AdministratorId))
                return false;

            command.CommandId ??= System.Guid.NewGuid().ToString("N");

            if (!riftSystem.Connections.TryGet(RiftCapability.LiveOps, out IRiftLiveOpsProvider ops))
                return false;

            return ops.AuthorizeAdminCommand(command);
        }
    }
}
