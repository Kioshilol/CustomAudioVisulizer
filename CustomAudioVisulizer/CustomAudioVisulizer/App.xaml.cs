using System.ComponentModel.Design;
using CustomAudioVisulizer.Services.SystemTimer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace CustomAudioVisulizer
{
    public partial class App : Application
    {
        private static ServiceContainer _serviceContainer;
        
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            _serviceContainer = new ServiceContainer();
            _serviceContainer.AddService(typeof(ISystemTimer), new SystemTimer());
        }

        public static T GetService<T>()
        {
            return _serviceContainer.GetService<T>();
        }
        
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}