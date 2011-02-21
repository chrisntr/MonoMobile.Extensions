using System;

namespace MonoMobile.Extensions
{
    public interface INotification
    {
        void Alert(string message, Action alertCallback);
        void Alert(string message, Action alertCallback, string title);
        void Alert(string message, Action alertCallback, string title, string buttonName);

        void Confirm(string message, Action confirmCallback);
        void Confirm(string message, Action confirmCallback, string title);
        void Confirm(string message, Action confirmCallback, string title, string buttonLabels);

        void Beep();
        void Beep(int times);

        void Vibrate();
        void Vibrate(int milliseconds);
    }
}