using System;
using MonoTouch.UIKit;
namespace MonoMobile.Extensions
{
	public class Device
	{
		public Device ()
		{
		}
		
		public string Name
        {
            get { return UIDevice.CurrentDevice.Name; }
        }

        public string MonoMobileVersion
        {
            get { return ExtensionHelper.MonoMobile; }
        }

        public string Platform
        {
            get { return UIDevice.CurrentDevice.Model;  }
        }

        public string UUID
        {
            get { return UIDevice.CurrentDevice.UniqueIdentifier; }
        }

        public string Version
        {
            get { return UIDevice.CurrentDevice.SystemVersion; }
        }
	}
}

