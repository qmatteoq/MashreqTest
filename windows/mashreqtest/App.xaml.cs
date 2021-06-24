using Microsoft.ReactNative;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace mashreqtest
{
    sealed partial class App : ReactApplication
    {
        private BackgroundTaskDeferral appServiceDeferral;

        public App()
        {
#if BUNDLE
            JavaScriptBundleFile = "index.windows";
            InstanceSettings.UseWebDebugger = false;
            InstanceSettings.UseFastRefresh = false;
#else
            JavaScriptMainModuleName = "index";
            InstanceSettings.UseWebDebugger = true;
            InstanceSettings.UseFastRefresh = true;
#endif

#if DEBUG
            InstanceSettings.UseDeveloperSupport = true;
#else
            InstanceSettings.UseDeveloperSupport = false;
#endif

            Microsoft.ReactNative.Managed.AutolinkedNativeModules.RegisterAutolinkedNativeModulePackages(PackageProviders); // Includes any autolinked modules

            PackageProviders.Add(new Microsoft.ReactNative.Managed.ReactPackageProvider());
            //PackageProviders.Add(new ServiceChannel.ReactPackageProvider());
            PackageProviders.Add(new ReactPackageProvider());

            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            base.OnLaunched(e);
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(MainPage));
            Window.Current.Activate();
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            var name = ReactPropertyBagHelper.GetName(ReactPropertyBagHelper.GlobalNamespace, "FilePath");
            InstanceSettings.Properties.Set(name, args.Files.FirstOrDefault().Path);
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
            {
                appServiceDeferral = args.TaskInstance.GetDeferral();

                var name = ReactPropertyBagHelper.GetName(ReactPropertyBagHelper.GlobalNamespace, "AppServiceConnection");

                InstanceSettings.Properties.Set(name, details.AppServiceConnection);
            }
        }
    }
}
