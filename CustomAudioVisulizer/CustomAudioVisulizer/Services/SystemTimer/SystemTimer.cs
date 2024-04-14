using System;
using System.Threading.Tasks;
using System.Timers;

namespace CustomAudioVisulizer.Services.SystemTimer
{
    public class SystemTimer : ISystemTimer
    {
        private Timer? _timer;
        private Func<Task> _onElapsedAction;
        private bool _isStopped;
    
        public void Initialize(Func<Task> onElapsedAction, double timerDelayInMilliseconds)
        {
            Dispose();
    
            _onElapsedAction = onElapsedAction;
    
            _timer = new Timer(timerDelayInMilliseconds);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = false;
        }
    
        public void Start()
        {
            _timer?.Start();
            _isStopped = false;
        }
    
        public void Stop()
        {
            _timer?.Stop();
            _isStopped = true;
        }
    
        public void Reset()
        {
            if (_isStopped)
            {
                return;
            }
    
            _timer?.Stop();
            _timer?.Start();
        }
    
        public void Dispose()
        {
            if (_timer is null)
            {
                return;
            }
    
            _timer.Stop();
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Dispose();
        }
    
        private void OnTimerElapsed(object? _, ElapsedEventArgs __)
        {
            if (_isStopped)
            {
                return;
            }
    
            _onElapsedAction?.Invoke();
        }
    }
}

