using Android.Content;
using Android.Provider;

namespace MonoMobile.Extensions
{
    public class Device : IDevice
    {
        public Device(Context context)
        {
            Context = context;
        }

        private Context Context { get; set; }

        public string Name
        {
            get { return Android.OS.Build.Product; }
        }

        public string MonoMobileVersion
        {
            get { return ExtensionHelper.MonoMobile; }
        }

        public string Platform
        {
            get { return "Android";  }
        }

        public string UUID
        {
            get { return Settings.Secure.GetString(Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId); }
        }

        public string Version
        {
            get { return Android.OS.Build.VERSION.Release; }
        }
    }
}
