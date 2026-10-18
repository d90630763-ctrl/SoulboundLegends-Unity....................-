using System;

namespace GameSystems.PartyArtifactLink
{
    public readonly struct PartyArtifactLinkEntityId : IEquatable<PartyArtifactLinkEntityId>
    {
        public readonly string Value;
        public PartyArtifactLinkEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(PartyArtifactLinkEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is PartyArtifactLinkEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator PartyArtifactLinkEntityId(string value) => new PartyArtifactLinkEntityId(value);
    }

    public interface IPartyArtifactLinkNetworkAdapter
    {
        void SendRequest(PartyArtifactLinkRequest request);
        void SendResult(PartyArtifactLinkResult result);
    }

    public interface IPartyArtifactLinkPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
