using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CustomAudioVisulizer
{
    public partial class MainPage
    {
        private readonly IAudioRecorderService _audioRecorderService;

        private const double MaximumHeight = 60;
        private const double MinimumHeight = 6;
        
        public MainPage()
        {
            InitializeComponent();

            _audioRecorderService = DependencyService.Resolve<IAudioRecorderService>();
            _audioRecorderService.AmplitudeUpdateAction = OnUpdateAmplitude;
        }
        
        private void OnStartRecording(object _, EventArgs __)
        {
            var ___ = StartRecordingAsync();
        }
        
        private void OnStopRecording(object sender, EventArgs e)
        {
            _audioRecorderService.StopRecording();
            MainStack.Children.Clear();
        }

        private async Task StartRecordingAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status == PermissionStatus.Denied && 
                DeviceInfo.Platform == DevicePlatform.iOS)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    App.Current.MainPage.DisplayAlert("Permission", "please go to settings and enable microphone permission", "ok");
                });
                
                return;
            }
            
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
                
            });
            
            if (status == PermissionStatus.Denied &&
                DeviceInfo.Platform == DevicePlatform.Android &&
                !Permissions.ShouldShowRationale<Permissions.Microphone>())
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    App.Current.MainPage.DisplayAlert("Permission", "please go to settings and enable microphone permission", "ok");
                });
            }
            
            _audioRecorderService.StartRecording();
        }
        
        private void OnUpdateAmplitude(double value)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var height = value * MaximumHeight;
                height = Math.Max(MinimumHeight, height);
                
                var boxView = new BoxView
                {
                    BackgroundColor = Color.White,
                    WidthRequest = 2,
                    HeightRequest = height,
                    VerticalOptions = LayoutOptions.Center
                };
                
                MainStack.Children.RemoveAt(0);
                MainStack.Children.Add(boxView);
            });
        }
    }
}