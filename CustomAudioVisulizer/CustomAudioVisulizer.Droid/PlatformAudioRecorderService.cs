using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Android.Media;
using CustomAudioVisulizer.Droid;
using CustomAudioVisulizer.Services.SystemTimer;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformAudioRecorderService))]
namespace CustomAudioVisulizer.Droid
{
    public class PlatformAudioRecorderService : IAudioRecorderService
    {
        private const int MaximumAmplitude = 10000;
        private readonly MediaRecorder  _mediaPlayer = new MediaRecorder();

        private readonly Lazy<ISystemTimer> _systemTimer = new Lazy<ISystemTimer>(App.GetService<ISystemTimer>);

        public void StartRecording()
        {
            _mediaPlayer.Reset();
            _mediaPlayer.SetAudioSource(AudioSource.Mic);
            _mediaPlayer.SetOutputFormat(OutputFormat.ThreeGpp);
            _mediaPlayer.SetAudioEncoder(AudioEncoder.AmrNb);
            _mediaPlayer.SetOutputFile(GetFileNameForRecording());
            _mediaPlayer.Prepare();
            _mediaPlayer.Start();

            _systemTimer.Value.Initialize(TimerOnElapsed, 100);
            _systemTimer.Value.Start();
        }

        public void StopRecording()
        {
            _mediaPlayer.Stop();
            _systemTimer.Value.Stop();
        }

        public Action<double> AmplitudeUpdateAction { get; set; }

        private Task TimerOnElapsed()
        {
            var amplitude = _mediaPlayer.MaxAmplitude;
            var maxAmplitude = (double)Math.Min(MaximumAmplitude, amplitude);
            var amplitudeMultiplier = maxAmplitude  / MaximumAmplitude;
            
            AmplitudeUpdateAction?.Invoke(amplitudeMultiplier);
            return Task.CompletedTask;
        }
        
        private string GetFileNameForRecording()
        {
            var dir = MainActivity.CurrentActivity.GetExternalFilesDir(Android.OS.Environment.DirectoryMusic).AbsolutePath;
            var guid = Guid.NewGuid() + ".mp3";
            var file = Path.Combine(dir, guid);
            return file;
        }
    }
}