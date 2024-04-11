using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace App1
{
    public partial class MainPage : ContentPage
    {
        private readonly IAudioRecorderService _audioRecorderService;

        private double[] _amplitudes = new double[10];
        
        public MainPage()
        {
            InitializeComponent();

            _audioRecorderService = DependencyService.Resolve<IAudioRecorderService>();
        }

        private void OnStartRecording(object sender, EventArgs e)
        {
            _audioRecorderService.StartRecording();
            _audioRecorderService.AmplitudeUpdateAction = OnUpdateAmplitude2;
        }

        private int _counter = 9;
        
        private void OnUpdateAmplitude2(double amplitude)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var height = 3 * amplitude * 10;
                height = Math.Max(1, height);
                var boxView = new BoxView
                {
                    BackgroundColor = Color.White,
                    WidthRequest = 2,
                    HeightRequest = height,
                    VerticalOptions = LayoutOptions.Center
                };

                var width = 6 * MainStack.Children.Count;
                var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
                if (width > screenWidth)
                {
                    MainStack.Children.RemoveAt(0);
                }
            
                MainStack.Children.Add(boxView);
            });
        }
        
        private void OnUpdateAmplitude(double amplitude)
        {
            if (_counter < 0)
            {
                _counter = 9;
                return;
            }
            
            _amplitudes[_counter] = amplitude;
            for (var i = _amplitudes.Length - 1; i > 0; i--)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var height = 3 * 1 * 10;
                    MainStack.Children[i].HeightRequest = height;
                });
            }

            _counter--;
        }

        private void OnStopRecording(object sender, EventArgs e)
        {
            _audioRecorderService.StopRecording();
            MainStack.Children.Clear();
        }
    }
}