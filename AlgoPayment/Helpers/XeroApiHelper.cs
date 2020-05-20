//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Xero.Api.Core;
//using Xero.Api.Example.Applications.Partner;
//using Xero.Api.Example.Applications.Public;
//using Xero.Api.Infrastructure.Interfaces;
//using Xero.Api.Infrastructure.OAuth;
//using Xero.Api.Example.TokenStores;
//using Xero.Api.Serialization;
//using System.Configuration;

//namespace AlgoPayment.Helpers
//{
//    public class ApplicationSettings
//    {
//        public string BaseApiUrl { get; set; }
//        public Consumer Consumer { get; set; }
//        public object Authenticator { get; set; }

//    }
//    public static class XeroApiHelper
//    {
//        private static ApplicationSettings _applicationSettings;
//        static XeroApiHelper()
//        {
//            // Refer to README.md for details
//            //var callbackUrl = "";
//            var callbackUrl = "https://localhost:44378/";
//            var memoryStore = new MemoryTokenStore();
//            var requestTokenStore = new MemoryTokenStore();
//            var baseApiUrl = "https://localhost:44378/";
//            //var baseApiUrl = "";
//            // Consumer details for Application
//            var consumerKey = "1848379FED4C45BA813064A5DD05103F";
//            var consumerSecret = "MuKZc2SZ-ESxvZKDaxEtObegFYgQMrXawOzgGVXRwe9ViKgV";
//            // Signing certificate details for Partner Applications
//            //var signingCertificatePath = @"C:\Dev\your_public_privatekey.pfx";
//            //var signingCertificatePassword = "Your_signing_cert_password - leave empty if you didn't set one when creating the cert";
//            // Public Application Settings
//            var publicConsumer = new Consumer(consumerKey, consumerSecret);
//            var publicAuthenticator = new PublicMvcAuthenticator(baseApiUrl, baseApiUrl, callbackUrl, memoryStore,
//                publicConsumer, requestTokenStore);
//            var publicApplicationSettings = new ApplicationSettings
//            {
//                BaseApiUrl = baseApiUrl,
//                Consumer = publicConsumer,
//                Authenticator = publicAuthenticator
//            };
//            _applicationSettings = publicApplicationSettings;
//        }
//        public static ApiUser User()
//        {
//            return new ApiUser { Name = Environment.MachineName };
//        }
//        public static IConsumer Consumer()
//        {
//            return _applicationSettings.Consumer;
//        }
//        public static IMvcAuthenticator MvcAuthenticator()
//        {
//            return (IMvcAuthenticator)_applicationSettings.Authenticator;
//        }
//        public static IXeroCoreApi CoreApi()
//        {
//            if (_applicationSettings.Authenticator is IAuthenticator)
//            {
//                return new XeroCoreApi(_applicationSettings.BaseApiUrl, _applicationSettings.Authenticator as IAuthenticator,
//                    _applicationSettings.Consumer, User(), new DefaultMapper(), new DefaultMapper());
//            }

//            return null;
//        }
//    }
//}