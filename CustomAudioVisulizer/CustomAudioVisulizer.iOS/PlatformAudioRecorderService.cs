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
        private readonly ISystemTimer _systemTimer = App.GetService<ISystemTimer>();
        
        private AVAudioRecorder _recorder;

        public void StartRecording()
        {
            PrepareRecorder();
            _recorder.Record();
        }

        public void StopRecording()
        {
            _recorder.Stop();
            _systemTimer.Stop();
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

            _systemTimer.Initialize(TimerOnElapsed, 100);
            _systemTimer.Start();
        }

        private Task TimerOnElapsed()
        {
            _recorder.UpdateMeters();
            var power = _recorder.AveragePower(0);
            var amplitude = 1.1 * Math.Pow(10.0, power / 20.0);
            var clampedAmplitude = Math.Min(Math.Max(amplitude, 0), 1);
            Console.WriteLine(clampedAmplitude);
            AmplitudeUpdateAction?.Invoke(clampedAmplitude);
            return Task.CompletedTask;
        }
    }
}