using Microsoft.Owin.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Timers;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System;

namespace HelloWorldNet
{
    class Program
    {
        static AutoResetEvent appServiceExit;
        static AppServiceConnection connection = null;

        static void Main(string[] args)
        {
            appServiceExit = new AutoResetEvent(false);
            Thread appServiceThread = new Thread(new ThreadStart(ThreadProc));
            appServiceThread.Start();
            appServiceExit.WaitOne();
        }

        static async void ThreadProc()
        {
            connection = new AppServiceConnection();
            connection.AppServiceName = "HelloWorldService";
            connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;

            //we open the connection
            AppServiceConnectionStatus status = await connection.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                //if the connection fails, we terminate the Win32 process
                appServiceExit.Set();
            }
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            //when the connection with the App Service is closed, we terminate the Win32 process
            appServiceExit.Set();
        }

        private static async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {          
            string name = args.Request.Message["Name"].ToString();
            string surname = args.Request.Message["Surname"].ToString();

            string message = $"Hello {name} {surname}";

            ValueSet valueSet = new ValueSet
            {
                { "Message", message }
            };

            await args.Request.SendResponseAsync(valueSet);
        }
    }
}
