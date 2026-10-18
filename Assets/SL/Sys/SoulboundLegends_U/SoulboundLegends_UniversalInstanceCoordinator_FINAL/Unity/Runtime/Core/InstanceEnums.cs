namespace SoulboundLegends.InstanceCoordinator
{
    public enum InstanceLifecycleState { Requested, Validated, CapacityChecked, ServerSelected, InstanceCreated, PlayersAssigned, Running, Draining, Destroyed, CleanupComplete, Failed }
    public enum InstanceFailureState { None, ValidationFailed, CapacityUnavailable, ServerUnavailable, CreationFailed, AssignmentFailed, TransferFailed, RecoveryFailed, CleanupFailed, Unauthorized, DependencyUnavailable, Timeout, Unknown }
    public enum InstanceRequestPriority { Low=0, Normal=100, High=200, Critical=300 }
    public enum TransferState { Requested, Reserved, Loading, Authenticated, Confirmed, RolledBack, Failed }
    public enum ServerHealth { Unknown, Healthy, Degraded, Unhealthy, Offline }
}
