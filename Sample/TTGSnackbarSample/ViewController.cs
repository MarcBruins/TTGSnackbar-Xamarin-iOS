using System;
using SnackBarTTG.Source;
using UIKit;

namespace TTGSnackbarSample
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}

		partial void buttonClicked(UIButton sender)
		{
			var snackbar = new TTGSnackbar("Message", TTGSnackbarDuration.Long, "Cancel", () => { cancel();});
			snackbar.AnimationType = TTGSnackbarAnimationType.SlideFromBottomToTop;
			snackbar.Show();
		}

		public void cancel()
		{
			//do something
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

