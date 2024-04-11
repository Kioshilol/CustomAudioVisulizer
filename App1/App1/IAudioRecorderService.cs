using System;

namespace App1
{
    public interface IAudioRecorderService
    {
        void StartRecording();

        void StopRecording();

        Action<double> AmplitudeUpdateAction { get; set; }
    }
}