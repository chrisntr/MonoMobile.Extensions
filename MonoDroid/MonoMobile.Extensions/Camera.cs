using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Android.Content;
using Android.App;
 
namespace MonoMobile.Extensions
{
    public class Camera
    {
        public class CameraOptions
        {
            // Set up Camera Option defaults - change to Android options
            //public UIImagePickerControllerSourceType SourceType = UIImagePickerControllerSourceType.Camera;
            public bool AllowEdit = false;
            public int Quality = 100;
            public DestinationType Destination = DestinationType.FileUri;
        }
         
        public enum DestinationType
        {   
            FileUri,
            DataUri
        }
 
        public Camera (Activity context)
        {
            _context2 = context;
             
        }
         
        Activity _context2;
         
        public void GetPicture(CameraOptions options, Action<string> successCallback, Action errorCallback)
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            intent.AddCategory(Intent.CategoryOpenable);
            var activity = new CameraActivity();
             
            Android.Util.Log.Info("MonoMobile.Extensions", "Got an activity...");
            activity.GetResult(intent, 3);
             
			// Work out how to get the activity for the result...
			
//      this.ctx.startActivityForResult((Plugin) this, Intent.createChooser(intent,
//              new String("Get Picture")), (srcType+1)*16 + returnType + 1);.
//          this.ctx.startActivityForResult((Plugin) this, Intent.createChooser(intent,
//                  new String("Get Picture")), (srcType+1)*16 + returnType + 1);
        }
         
        public void CameraActivityResult(int requestCode, Result resultCode, Intent data)
        {
			// Can't figure out how to call this without having the user manuall overriding the OnActivityResult manually...
            Android.Util.Log.Info("MonoMobile.Extensions", "Result returned {0}", requestCode); 
        }
         
        [Activity]
        public class CameraActivity : Activity
        {
            Camera _camera;
             
            protected override void OnCreate (Android.OS.Bundle savedInstanceState)
            {
                base.OnCreate (savedInstanceState);
                 
                Android.Util.Log.Info("MonoMobile.Extensions", "On Create...");
            }
             
            public void GetResult(Intent intent, int requestCode)
            {
                 
                Android.Util.Log.Info("MonoMobile.Extensions", "Starting Activity for result...request code = {0}", requestCode);
                this.StartActivityForResult(intent, requestCode);
            }
             
            protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
            {
                base.OnActivityResult (requestCode, resultCode, data);
                
                Android.Util.Log.Info("MonoMobile.Extensions", "ON Actitivty Result...");
                _camera.CameraActivityResult(requestCode, resultCode, data);
            }
        }
    }
}