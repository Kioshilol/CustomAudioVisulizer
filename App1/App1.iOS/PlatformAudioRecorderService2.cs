using System;
using System.IO;
using System.Timers;
using App1.iOS;
using AVFoundation;
using Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformAudioRecorderService2))]
namespace App1.iOS
{
    public class PlatformAudioRecorderService2 : IAudioRecorderService
    {
        private AVAudioRecorder _recorder;
        private Timer _timer;
        
        public void StartRecording()
        {
            PrepareRecorder();
            _recorder.Record();
        }

        public void StopRecording()
        {
            _recorder.Stop();
            
            _timer.Stop();
            _timer.Elapsed -= TimerOnElapsed;
            _timer.Dispose();
        }

        public Action<double> AmplitudeUpdateAction { get; set; }

        private void PrepareRecorder()
        {
            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord, AVAudioSessionCategoryOptions.MixWithOthers);
            if (err != null)
            {
                Console.WriteLine("audioSession: {0}", err);
            }

            err = audioSession.SetActive(true);
            if (err != null)
            {
                Console.WriteLine("audioSession: {0}", err);
            }

            var rate = (float)audioSession.SampleRate;
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
            var audioFormat = new AVAudioFormat(audioSettings);

            Console.WriteLine("buffer");

            var path = Path.GetTempPath();
            var guid = Guid.NewGuid().ToString();
            var url = NSUrl.FromFilename(path + guid + ".m4a");
            _recorder = AVAudioRecorder.Create(url, audioSettings, out err);
            _recorder.PrepareToRecord();
            _recorder.MeteringEnabled = true;

            _timer = new Timer(100);
            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _recorder.UpdateMeters();
            var power = _recorder.AveragePower(0);
            var amplitude = 1.1 * Math.Pow(10.0, power / 20.0);
            var clampedAmplitude = Math.Min(Math.Max(amplitude, 0), 1);
            Console.WriteLine(clampedAmplitude);
            AmplitudeUpdateAction?.Invoke(clampedAmplitude);
        }
    }
}