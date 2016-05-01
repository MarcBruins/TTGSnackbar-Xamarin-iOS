using System;
using System.Threading.Tasks;
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
			var snackbar = new TTGSnackbar("Hello Xamarin snackbar", TTGSnackbarDuration.Forever);

			// Action 1
			snackbar.ActionText = "Yes";
			snackbar.ActionTextColor = UIColor.Green;
			snackbar.ActionBlock = (t) => { Console.WriteLine("clicked yes"); };

			// Action 2
			snackbar.SecondActionText = "No";
			snackbar.SecondActionTextColor = UIColor.Magenta;
			snackbar.SecondActionBlock = (t) => { Console.WriteLine("clicked no"); };

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

