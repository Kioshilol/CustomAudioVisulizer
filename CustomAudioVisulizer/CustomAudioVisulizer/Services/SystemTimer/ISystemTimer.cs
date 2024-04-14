using System;
using System.Threading.Tasks;

namespace CustomAudioVisulizer.Services.SystemTimer
{
    public interface ISystemTimer : IDisposable
    {
        void Initialize(Func<Task> onElapsedAction, double timerDelayInMilliseconds);

        void Start();

        void Stop();

        void Reset();
    }
}

