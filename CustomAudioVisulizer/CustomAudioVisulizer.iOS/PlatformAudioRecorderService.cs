using System;
using System.IO;
using System.Threading.Tasks;
using CustomAudioVisulizer.iOS;
using CustomAudioVisulizer.Services.SystemTimer;
using AVFoundation;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformAudioRecorderService))]
namespace CustomAudioVisulizer.iOS
{
    public class PlatformAudioRecorderService : IAudioRecorderService
    {
        private const int MaximumAmplitude = 60;
        
        private readonly Lazy<ISystemTimer> _systemTimer = new Lazy<ISystemTimer>(() => App.GetService<ISystemTimer>());
        
        private AVAudioRecorder _recorder;

        public void StartRecording()
        {
            PrepareRecorder();
            _recorder.Record();
        }

        public void StopRecording()
        {
            _recorder.Stop();
            _systemTimer.Value.Stop();
        }

        public Action<double> AmplitudeUpdateAction { get; set; }

        private void PrepareRecorder()
        {
            var audioSession = AVAudioSession.SharedInstance();
            audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord, AVAudioSessionCategoryOptions.MixWithOthers);
            audioSession.SetActive(true);

            NSObject[] values = {
                NSNumber.FromInt32((int)AudioToolbox.AudioFormatType.MPEG4AAC),
                NSNumber.FromInt32(16),
                NSNumber.FromBoolean(true),
                NSNumber.FromFloat(12000.0f),
                NSNumber.FromInt32(1),
            };
            
            NSObject[] keys = {
                AVAudioSettings.AVFormatIDKey,
                AVAudioSettings.AVLinearPCMBitDepthKey,
                AVAudioSettings.AVLinearPCMIsFloatKey,
                AVAudioSettings.AVSampleRateKey,
                AVAudioSettings.AVNumberOfChannelsKey,
            };
            
            var settings = NSDictionary.FromObjectsAndKeys(values, keys);
            var audioSettings = new AudioSettings(settings);

            var path = Path.GetTempPath();
            var guid = Guid.NewGuid().ToString();
            var url = NSUrl.FromFilename(path + guid + ".m4a");
            
            _recorder = AVAudioRecorder.Create(url, audioSettings, out _);
            _recorder.PrepareToRecord();
            _recorder.MeteringEnabled = true;

            _systemTimer.Value.Initialize(TimerOnElapsed, 100);
            _systemTimer.Value.Start();
        }

        private Task TimerOnElapsed()
        {
            _recorder.UpdateMeters();
            var power = Math.Abs(_recorder.AveragePower(0));
            power = Math.Min(power, MaximumAmplitude);
            var powerMultiplier = power / MaximumAmplitude;
            powerMultiplier = 1 - powerMultiplier;
            
            Console.WriteLine(powerMultiplier);
            AmplitudeUpdateAction?.Invoke(powerMultiplier);
            return Task.CompletedTask;
        }
    }
}