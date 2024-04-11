using System;
using AVFoundation;
using Foundation;

namespace App1.iOS
{
    public class PlatformAudioRecorderService : IAudioRecorderService
    {
        private readonly AVAudioEngine _audioEngine = new AVAudioEngine();

        public NSDictionary Settings { get; set; }


        public void StartRecording()
        {
            PrepareRecorder();
        }

        public void StopRecording()
        {
            _audioEngine.InputNode.RemoveTapOnBus(0);
            _audioEngine.Stop();
            AVAudioSession.SharedInstance().SetActive(false);
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
                NSNumber.FromInt32((int)AVAudioCommonFormat.PCMFloat32),
                NSNumber.FromInt32(16),
                NSNumber.FromBoolean(true),
                NSNumber.FromFloat(rate),
                NSNumber.FromInt32(1),
            };
            
            NSObject[] keys = {
                AVAudioSettings.AVFormatIDKey,
                AVAudioSettings.AVLinearPCMBitDepthKey,
                AVAudioSettings.AVLinearPCMIsFloatKey,
                AVAudioSettings.AVSampleRateKey,
                AVAudioSettings.AVNumberOfChannelsKey,
            };
            
            Settings = NSDictionary.FromObjectsAndKeys(values, keys);
            var audioSettings = new AudioSettings(Settings);
            var audioFormat = new AVAudioFormat(audioSettings);

            _audioEngine.Reset();
            Console.WriteLine("buffer");
            _audioEngine.InputNode.RemoveTapOnBus(0);
            _audioEngine.InputNode.InstallTapOnBus(0, 1024, audioFormat, TapBlock);
            
            _audioEngine.Init();
            _audioEngine.InputNode.Init();
            _audioEngine.Prepare();
            _audioEngine.StartAndReturnError(out var error);
        }

        private void TapBlock(AVAudioPcmBuffer buffer, AVAudioTime when)
        {
        }
    }
}