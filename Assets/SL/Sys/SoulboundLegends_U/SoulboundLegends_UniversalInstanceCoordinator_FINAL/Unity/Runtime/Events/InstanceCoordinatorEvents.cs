using System;

namespace SoulboundLegends.InstanceCoordinator
{
    public sealed class InstanceLifecycleEventArgs : EventArgs { public InstanceRecord Instance { get; init; } public InstanceLifecycleEventArgs(InstanceRecord instance) => Instance = instance; }
    public sealed class TransferEventArgs : EventArgs { public TransferRecord Transfer { get; init; } public TransferEventArgs(TransferRecord transfer) => Transfer = transfer; }
}
