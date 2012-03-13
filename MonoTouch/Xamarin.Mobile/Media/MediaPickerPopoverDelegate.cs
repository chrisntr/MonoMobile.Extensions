using System;
using MonoTouch.UIKit;

namespace Xamarin.Media
{
	internal class MediaPickerPopoverDelegate
		: UIPopoverControllerDelegate
	{
		internal MediaPickerPopoverDelegate (MediaPickerDelegate pickerDelegate, UIImagePickerController picker)
		{
			this.pickerDelegate = pickerDelegate;
			this.picker = picker;
		}
		
		public override bool ShouldDismiss (UIPopoverController popoverController)
		{
			return true;
		}
		
		public override void DidDismiss (UIPopoverController popoverController)
		{
			this.pickerDelegate.Canceled (this.picker);
		}
		
		private readonly MediaPickerDelegate pickerDelegate;
		private readonly UIImagePickerController picker;
	}
}

