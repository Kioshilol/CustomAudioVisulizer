using System;

namespace CustomAudioVisulizer
{
    public interface IAudioRecorderService
    {
        void StartRecording();

        void StopRecording();

        Action<double> AmplitudeUpdateAction { get; set; }
    }
}