using System;

namespace GameSystems.AudioWeatherWorldStreaming
{
    public readonly struct AudioWeatherWorldStreamingEntityId : IEquatable<AudioWeatherWorldStreamingEntityId>
    {
        public readonly string Value;
        public AudioWeatherWorldStreamingEntityId(string value) => Value = value ?? string.Empty;
        public bool Equals(AudioWeatherWorldStreamingEntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is AudioWeatherWorldStreamingEntityId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
        public static implicit operator AudioWeatherWorldStreamingEntityId(string value) => new AudioWeatherWorldStreamingEntityId(value);
    }

    public interface IAudioWeatherWorldStreamingNetworkAdapter
    {
        void SendRequest(AudioWeatherWorldStreamingRequest request);
        void SendResult(AudioWeatherWorldStreamingResult result);
    }

    public interface IAudioWeatherWorldStreamingPersistence
    {
        string CaptureState();
        void RestoreState(string serializedState);
    }
}
