using Microsoft.ReactNative;
using Microsoft.ReactNative.Managed;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace ServiceChannel
{
    [ReactModule]
    class ServiceChannelModule
    {
        private ReactContext _context;
        private AppServiceConnection _connection;

        [ReactInitializer]
        public void Initialize(ReactContext context)
        {
            _context = context;
        
        }

        [ReactMethod("launchFullTrustProcess")]
        public async void LaunchFullTrustProcessAsync(IReactPromise<bool> promise)
        {
            try
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                promise.Resolve(true);
            }
            catch (Exception exc)
            {
                promise.Reject(new ReactError { Exception = exc });
            }
        }


        [ReactMethod("sendMessageWithAppService")]
        public async void SendMessageWithAppServiceAsync(string name, string surname, IReactPromise<string> promise)
        {
            var propertyName = ReactPropertyBagHelper.GetName(ReactPropertyBagHelper.GlobalNamespace, "AppServiceConnection");
            var content = _context.Handle.Properties.Get(propertyName);
            _connection = content as AppServiceConnection;

            ValueSet valueSet = new ValueSet
            {
                { "Name", name },
                { "Surname", surname }
            };

            var result = await _connection.SendMessageAsync(valueSet);

            string message = result.Message["Message"].ToString();
            promise.Resolve(message);
        }
    }
}

