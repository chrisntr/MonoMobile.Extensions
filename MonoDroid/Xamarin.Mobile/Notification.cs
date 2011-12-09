using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Java.Lang;

namespace MonoMobile.Extensions
{
    public class Notification : INotification
    {
        private Activity Context { get; set; }

        public Notification(Activity context)
        {
            Context = context;
        }

        public void Alert(string message, Action alertCallback)
        {
            Alert(message, alertCallback, "Alert", "OK");
        }

        public void Alert(string message, Action alertCallback, string title)
        {
            Alert(message, alertCallback, title, "OK");
        }

        public void Alert(string message, Action alertCallback, string title, string buttonName)
        {
            Context.RunOnUiThread(() =>
            {
                var dlg = new AlertDialog.Builder(Context);
                dlg.SetMessage(message);
                dlg.SetTitle(title);
                dlg.SetCancelable(false);
                dlg.SetPositiveButton(buttonName, new DialogInterface(alertCallback));
                dlg.Create();
                dlg.Show();                              
            });

            
            //    final PhonegapActivity ctx = this.ctx;
            //final Notification notification = this;

            //Runnable runnable = new Runnable() {
            //    public void run() {

            //        AlertDialog.Builder dlg = new AlertDialog.Builder(ctx);
            //        dlg.setMessage(message);
            //        dlg.setTitle(title);
            //        dlg.setCancelable(false);
            //        dlg.setPositiveButton(buttonLabel,
            //                new AlertDialog.OnClickListener() {
            //            public void onClick(DialogInterface dialog, int which) {
            //                dialog.dismiss();
            //                notification.success(new PluginResult(PluginResult.Status.OK, 0), callbackId);
            //            }
            //        });
            //        dlg.create();
            //        dlg.show();
            //    };
            //};
            //this.ctx.runOnUiThread(runnable);
        }

        public class DialogInterface : IDialogInterfaceOnClickListener
        {
            private readonly Action _alertCallback;
            public DialogInterface(Action alertCallback)
            {
                _alertCallback = alertCallback;
            }

            public void OnClick(IDialogInterface dialog, DialogInterfaceButton which)
            {
                dialog.Dismiss();
                _alertCallback();
            }

            public IntPtr Handle
            {
                get { return Handle; }
            }
        }

        public void Confirm(string message, Action confirmCallback)
        {
            throw new NotImplementedException();
        }

        public void Confirm(string message, Action confirmCallback, string title)
        {
            throw new NotImplementedException();
        }

        public void Confirm(string message, Action confirmCallback, string title, string buttonLabels)
        {
            throw new NotImplementedException();
        }

        public void Beep()
        {
            Beep(1);
        }

        public void Beep(int count)
        {
            Android.Net.Uri ringtone = RingtoneManager.GetActualDefaultRingtoneUri(Context, RingtoneType.Notification);
            Ringtone notification = RingtoneManager.GetRingtone(Context, ringtone);

            // If phone is not set to silent mode
            if (notification != null)
            {
                for (int i = 0; i < count; ++i)
                {
                    notification.Play();
                    long timeout = 5000;
                    while (notification.IsPlaying && (timeout > 0))
                    {
                        timeout = timeout - 100;
                        try
                        {
                            Thread.Sleep(100);
                        }
                        catch (InterruptedException e)
                        {
                        }
                    }
                }
            }
        }

        public void Vibrate()
        {
            Vibrate(500);
        }

        public void Vibrate(int milliseconds)
        {
            Vibrator vibrator = (Vibrator) Context.GetSystemService(Android.Content.Context.VibratorService);
            vibrator.Vibrate(milliseconds);
        }
    }
}
