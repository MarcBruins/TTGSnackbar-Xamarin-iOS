using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace TTGSnackbar
{

	/**
	Snackbar display duration types.
	- Short:   1 second
	- Middle:  3 seconds
	- Long:    5 seconds
	- Forever: Not dismiss automatically. Must be dismissed manually.
	*/

	public enum TTGSnackbarDuration
	{
     	Short = 1,
    	Middle = 3,
   		Long = 5,
    	Forever = 9999999 // Must dismiss manually.
	}

	/**
	Snackbar animation types.
	- FadeInFadeOut:               Fade in to show and fade out to dismiss.
	- SlideFromBottomToTop:        Slide from the bottom of screen to show and slide up to dismiss.
	- SlideFromBottomBackToBottom: Slide from the bottom of screen to show and slide back to bottom to dismiss.
	- SlideFromLeftToRight:        Slide from the left to show and slide to rigth to dismiss.
	- SlideFromRightToLeft:        Slide from the right to show and slide to left to dismiss.
	- Flip:                        Flip to show and dismiss.
	*/

	public enum TTGSnackbarAnimationType
	{
	    FadeInFadeOut,
	    SlideFromBottomToTop,
	    SlideFromBottomBackToBottom,
	    SlideFromLeftToRight,
	    SlideFromRightToLeft,
	    Flip,
	}

	public class TTGSnackbar : UIView
	{

		/// Snackbar action button max width.
		private const float snackbarActionButtonMaxWidth = 64;

		// Snackbar action button min width.
		private const float snackbarActionButtonMinWidth = 44;

		// Action callback closure definition.
		public Action TTGActionBlock { get; set; }

		// Dismiss callback closure definition.
		public Action TTGDismissBlock { get; set; }

		// Action callback.
		public Action ActionBlock { get; set; }

		// Second action block
		public Action SecondActionBlock { get; set; }

		// Snackbar display duration. Default is Short - 1 second.
		public TTGSnackbarDuration Duration = TTGSnackbarDuration.Short;

		// Snackbar animation type. Default is SlideFromBottomBackToBottom.
		public TTGSnackbarAnimationType AnimationType = TTGSnackbarAnimationType.SlideFromBottomBackToBottom;

		// Show and hide animation duration. Default is 0.3
		public float AnimationDuration = 0.3f;

		private float _cornerRadius = 4;
		public float CornerRadius
		{
			get { return _cornerRadius; }
			set
			{
				_cornerRadius = value;
				if (_cornerRadius > Height)
				{
					_cornerRadius = Height / 2;
				}
				if (_cornerRadius < 0)
					_cornerRadius = 0;
				
				this.Layer.CornerRadius = _cornerRadius;
				this.Layer.MasksToBounds = true;
			}
		}

		/// Left margin. Default is 4
		private float _leftMargin = 4; 
		public float LeftMargin{ get { return _leftMargin;} 
			set {_leftMargin = value; leftMarginConstraint.Constant = _leftMargin; this.LayoutIfNeeded(); } }

		private float _rightMargin = 4;
		public float RightMargin
		{
			get { return _rightMargin; }
			set { _rightMargin = value; rightMarginConstraint.Constant = _leftMargin; this.LayoutIfNeeded(); }
		}

		/// Bottom margin. Default is 4
		private float _bottomMargin = 4;
		public float BottomMargin
		{
			get { return _bottomMargin; }
			set { _bottomMargin = value; bottomMarginConstraint.Constant = _bottomMargin; this.LayoutIfNeeded(); }
		}

		private float _height = 44;
		public float Height
		{
			get { return _height;}
			set { _height = value; heightConstraint.Constant = _height; this.LayoutIfNeeded();}
		}

		private string _message;
		public string Message
		{
			get { return _message;}
			set { _message = value; this.messageLabel.Text = _message;}
		}

		private UIColor _messageTextColor;
		public UIColor MessageTextColor
		{
			get { return _messageTextColor; }
			set { _messageTextColor = value; this.messageLabel.TextColor = _messageTextColor; }
		}

		private UIFont _messageTextFont;
		public UIFont MessageTextFont
		{
			get { return _messageTextFont;}
			set { _messageTextFont = value; this.messageLabel.Font = _messageTextFont;}
		}

		private UITextAlignment _messageTextAlign;
		public UITextAlignment MessageTextAlign
		{
			get { return _messageTextAlign; }
			set { _messageTextAlign = value; this.messageLabel.TextAlignment = _messageTextAlign; }
		}

		private string _actionText;
		public string ActionText
		{
			get { return _actionText; }
			set { _actionText = value; this.actionButton.SetTitle(_actionText, UIControlState.Normal); }
		}

		private string _secondActionText;
		public string SecondActionText
		{
			get { return _secondActionText; }
			set { _secondActionText = value; this.secondActionButton.SetTitle(_actionText, UIControlState.Normal); }
		}

		// Action button title color. Default is white.
		private UIColor _actionTextColor = UIColor.White;
		public UIColor ActionTextColor
		{
			get { return _actionTextColor;}
			set { _actionTextColor = value; this.actionButton.SetTitleColor(_actionTextColor, UIControlState.Normal); }
		}

		// Second action button title color. Default is white.
		private UIColor _secondActionTextColor = UIColor.White;
		public UIColor SecondActionTextColor
		{
			get { return _secondActionTextColor; }
			set { _secondActionTextColor = value; this.secondActionButton.SetTitleColor(_secondActionTextColor, UIControlState.Normal); }
		}

		// First action text font. Default is Bold system font (14).
		private UIFont _actionTextFont = UIFont.BoldSystemFontOfSize(14);
		public UIFont ActionTextFont
		{
			get { return _actionTextFont;}
			set { _actionTextFont = value; this.actionButton.TitleLabel.Font = _actionTextFont;}
		}

		// First action text font. Default is Bold system font (14).
		private UIFont _secondActionTextFont = UIFont.BoldSystemFontOfSize(14);
		public UIFont SecondActionTextFont
		{
			get { return _secondActionTextFont; }
			set { _secondActionTextFont = value; this.secondActionButton.TitleLabel.Font = _secondActionTextFont; }
		}

		private UILabel messageLabel;
		private UIView seperateView;
		private UIButton actionButton;
		private UIButton secondActionButton;
		private UIActivityIndicatorView activityIndicatorView;

		// Timer to dismiss the snackbar.
		private NSTimer dismissTimer;

		// Constraints.
		private NSLayoutConstraint heightConstraint;
		private NSLayoutConstraint leftMarginConstraint;
		private NSLayoutConstraint rightMarginConstraint;
		private NSLayoutConstraint bottomMarginConstraint;
		private NSLayoutConstraint actionButtonWidthConstraint;
		private NSLayoutConstraint secondActionButtonWidthConstraint;


		/**
		   Show a single message like a Toast.
		   
		   - parameter message:  Message text.
		   - parameter duration: Duration type.
		   
		   - returns: Void
		   */
		public TTGSnackbar(string message, TTGSnackbarDuration duration)
		{
			Frame = CoreGraphics.CGRect.FromLTRB(0, 0, 320, Height); //todo check if this is correct

			this.Duration = duration;

			this.Message = message;

			configure();
		}

		/**
		Show a message with action button.
		
		- parameter message:     Message text.
		- parameter duration:    Duration type.
		- parameter actionText:  Action button title.
		- parameter actionBlock: Action callback closure.
		
		- returns: Void
		*/
		public TTGSnackbar(string message, TTGSnackbarDuration duration, string actionText, Action ttgAction)
		{
			Frame = CoreGraphics.CGRect.FromLTRB(0, 0, 320, Height); //todo check if this is correct

			this.Duration = duration;

			this.Message = message;

			this.ActionText = actionText;

			this.ActionBlock = ttgAction;

			configure();
		}

		/**
		Show a custom message with action button.
		 
		- parameter message:          Message text.
		- parameter duration:         Duration type.
		- parameter actionText:       Action button title.
		- parameter messageFont:      Message label font.
		- parameter actionButtonFont: Action button font.
		- parameter actionBlock:      Action callback closure.
		- returns: Void
		*/
		public TTGSnackbar(string message, TTGSnackbarDuration duration, string actionText, UIFont messageFont, UIFont actionTextFont, Action ttgAction)
		{
			Frame = CoreGraphics.CGRect.FromLTRB(0, 0, 320, Height); //todo check if this is correct

			this.Duration = duration;

			this.Message = message;

			this.ActionText = actionText;

			this.ActionBlock = ttgAction;

			this.MessageTextFont = messageFont;

			this.ActionTextFont = actionTextFont;

			configure();
		}


	/**
    Show the snackbar.
    */
	public void Show()
	{
		// Only show once
			if (this.Superview != null)
				return;

			// Create dismiss timer
			dismissTimer = NSTimer.CreateScheduledTimer((int)Duration, (t) => dismiss());

			// Show or hide action button
			if (actionButton.Hidden)
				ActionText = String.Empty;
			else
				ActionBlock = null;

			if (secondActionButton.Hidden)
				SecondActionText = String.Empty;
			else
				SecondActionBlock = null;

			seperateView.Hidden = actionButton.Hidden;

			actionButtonWidthConstraint.Constant = actionButton.Hidden ? 0 : (secondActionButton.Hidden ? TTGSnackbar.snackbarActionButtonMaxWidth : TTGSnackbar.snackbarActionButtonMinWidth);

			secondActionButtonWidthConstraint.Constant = secondActionButton.Hidden ? 0 : (actionButton.Hidden ? TTGSnackbar.snackbarActionButtonMaxWidth : TTGSnackbar.snackbarActionButtonMinWidth);


			this.LayoutIfNeeded();

			if (Superview == UIApplication.SharedApplication.KeyWindow)
			{
				Superview.Add(this);

				heightConstraint = NSLayoutConstraint.Create(
					this,
					NSLayoutAttribute.Height,
					NSLayoutRelation.Equal,
					null,
					NSLayoutAttribute.NoAttribute,
					1,
					Height);

				leftMarginConstraint = NSLayoutConstraint.Create(
					this,
					NSLayoutAttribute.Left,
					NSLayoutRelation.Equal,
					Superview,
					NSLayoutAttribute.NoAttribute,
					1,
					Height);

				rightMarginConstraint = NSLayoutConstraint.Create(
					this,
					NSLayoutAttribute.Right,
					NSLayoutRelation.Equal,
					Superview,
					NSLayoutAttribute.NoAttribute,
					1,
					Height);

				bottomMarginConstraint = NSLayoutConstraint.Create(
					this,
					NSLayoutAttribute.Bottom,
					NSLayoutRelation.Equal,
					Superview,
					NSLayoutAttribute.NoAttribute,
					1,
					Height);

				// Avoid the "UIView-Encapsulated-Layout-Height" constraint conflicts
				// http://stackoverflow.com/questions/25059443/what-is-nslayoutconstraint-uiview-encapsulated-layout-height-and-how-should-i
				leftMarginConstraint.Priority = 999;
				rightMarginConstraint.Priority = 999;

				this.AddConstraint(heightConstraint);
				Superview.AddConstraint(leftMarginConstraint);
				Superview.AddConstraint(rightMarginConstraint);
				Superview.AddConstraint(bottomMarginConstraint);

				// Show 
				showWithAnimation();
			}
			else {
				Console.WriteLine("TTGSnackbar needs a keyWindows to display.");
			}
	}

	/**
    Dismiss the snackbar manually.
    */
	public void dismiss()
	{
			this.dismissAnimated(true);

		// On main thread
		//dispatch_async(dispatch_get_main_queue()) {
		//	()->Void in
  //          self.dismissAnimated(true)

		//}


	}

	/**
    Init configuration.
    */
	private void configure()
	{
			this.TranslatesAutoresizingMaskIntoConstraints = false;

			this.BackgroundColor = UIColor.FromRGBA(255, 255, 255, 255 * 0.8f);

			this.Layer.CornerRadius = CornerRadius;

			this.Layer.MasksToBounds = true;


			messageLabel = new UILabel();

			messageLabel.TranslatesAutoresizingMaskIntoConstraints = false;

			messageLabel.TextColor = UIColor.whiteColor();

			messageLabel.Font = messageTextFont;

			messageLabel.BackgroundColor = UIColor.clearColor();

			messageLabel.LineBreakMode = .ByCharWrapping;

			messageLabel.NumberOfLines = 2;

			messageLabel.TextAlignment = .Left;

			messageLabel.Text = message;

			this.AddSubview(messageLabel);


			actionButton = new UIButton();

			actionButton.TranslatesAutoresizingMaskIntoConstraints = false;

			actionButton.BackgroundColor = UIColor.Clear();

			actionButton.?TitleLabel.Font = ActionTextFont;

			actionButton.TitleLabel.AdjustsFontSizeToFitWidth = true;

			actionButton.SetTitle(actionText, forState: .Normal);

			actionButton.SetTitleColor(actionTextColor, forState: .Normal);

			actionButton.AddTarget(self, action: #selector(doAction(_:)), forControlEvents: .TouchUpInside);
			self.addSubview(actionButton);


		secondActionButton = UIButton()

		secondActionButton.translatesAutoresizingMaskIntoConstraints = false

		secondActionButton.backgroundColor = UIColor.clearColor()

		secondActionButton.titleLabel?.font = secondActionTextFont

		secondActionButton.titleLabel?.adjustsFontSizeToFitWidth = true

		secondActionButton.setTitle(secondActionText, forState: .Normal)

		secondActionButton.setTitleColor(secondActionTextColor, forState: .Normal)

		secondActionButton.addTarget(self, action: #selector(doAction(_:)), forControlEvents: .TouchUpInside)
        self.addSubview(secondActionButton)


		seperateView = UIView()

		seperateView.translatesAutoresizingMaskIntoConstraints = false

		seperateView.backgroundColor = UIColor.grayColor()

		self.addSubview(seperateView)


		activityIndicatorView = UIActivityIndicatorView(activityIndicatorStyle: .White)

		activityIndicatorView.translatesAutoresizingMaskIntoConstraints = false

		activityIndicatorView.stopAnimating()

		self.addSubview(activityIndicatorView)

		// Add constraints
		let hConstraints: [NSLayoutConstraint] = NSLayoutConstraint.constraintsWithVisualFormat(
		"H:|-4-[messageLabel]-2-[seperateView(0.5)]-2-[actionButton]-0-[secondActionButton]-4-|",
				options: NSLayoutFormatOptions(rawValue: 0),
				metrics: nil,
				views: ["messageLabel": messageLabel, "seperateView": seperateView, "actionButton": actionButton, "secondActionButton": secondActionButton])


		let vConstraintsForMessageLabel: [NSLayoutConstraint] = NSLayoutConstraint.constraintsWithVisualFormat(
		"V:|-0-[messageLabel]-0-|",
				options: NSLayoutFormatOptions(rawValue: 0),
				metrics: nil,
				views: ["messageLabel": messageLabel])


		let vConstraintsForSeperateView: [NSLayoutConstraint] = NSLayoutConstraint.constraintsWithVisualFormat(
		"V:|-4-[seperateView]-4-|",
				options: NSLayoutFormatOptions(rawValue: 0),
				metrics: nil,
				views: ["seperateView": seperateView])


		let vConstraintsForActionButton: [NSLayoutConstraint] = NSLayoutConstraint.constraintsWithVisualFormat(
		"V:|-0-[actionButton]-0-|",
				options: NSLayoutFormatOptions(rawValue: 0),
				metrics: nil,
				views: ["actionButton": actionButton])


		let vConstraintsForSecondActionButton: [NSLayoutConstraint] = NSLayoutConstraint.constraintsWithVisualFormat(
		"V:|-0-[secondActionButton]-0-|",
				options: NSLayoutFormatOptions(rawValue: 0),
				metrics: nil,
				views: ["secondActionButton": secondActionButton])


		actionButtonWidthConstraint = NSLayoutConstraint.init(
		item: actionButton, attribute: .Width, relatedBy: .Equal,
				toItem: nil, attribute: .NotAnAttribute, multiplier: 1, constant: TTGSnackbar.snackbarActionButtonMinWidth)


		secondActionButtonWidthConstraint = NSLayoutConstraint.init(
		item: secondActionButton, attribute: .Width, relatedBy: .Equal,
				toItem: nil, attribute: .NotAnAttribute, multiplier: 1, constant: TTGSnackbar.snackbarActionButtonMinWidth)


		let vConstraintsForActivityIndicatorView: [NSLayoutConstraint] = NSLayoutConstraint.constraintsWithVisualFormat(
		"V:|-2-[activityIndicatorView]-2-|",
				options: NSLayoutFormatOptions(rawValue: 0),
				metrics: nil,
				views: ["activityIndicatorView": activityIndicatorView])


		let hConstraintsForActivityIndicatorView: [NSLayoutConstraint] = NSLayoutConstraint.constraintsWithVisualFormat(
		"H:[activityIndicatorView(activityIndicatorWidth)]-2-|",
				options: NSLayoutFormatOptions(rawValue: 0),
				metrics: ["activityIndicatorWidth": height - 4],
				views: ["activityIndicatorView": activityIndicatorView])


		actionButton.addConstraint(actionButtonWidthConstraint!)

		secondActionButton.addConstraint(secondActionButtonWidthConstraint!)


		self.addConstraints(hConstraints)

		self.addConstraints(vConstraintsForMessageLabel)

		self.addConstraints(vConstraintsForSeperateView)

		self.addConstraints(vConstraintsForActionButton)

		self.addConstraints(vConstraintsForSecondActionButton)

		self.addConstraints(vConstraintsForActivityIndicatorView)

		self.addConstraints(hConstraintsForActivityIndicatorView)

	}

	/**
    Invalid the dismiss timer.
    */
	private func invalidDismissTimer()
	{
		dismissTimer?.invalidate()

		dismissTimer = nil

	}

	/**
    Dismiss.
    
    - parameter animated: If dismiss with animation.
    */
	private func dismissAnimated(animated: Bool)
	{
		invalidDismissTimer()

		activityIndicatorView.stopAnimating()


		let superViewWidth = CGRectGetWidth((superview?.frame)!)


		if !animated {
			dismissBlock ? (snackbar: self)
            self.removeFromSuperview()

			return

		}

		var animationBlock: (()->Void) ? = nil


		switch animationType {
			case .FadeInFadeOut:
				animationBlock = {
					self.alpha = 0.0
	
			}
			case .SlideFromBottomBackToBottom:
				bottomMarginConstraint?.constant = height

		case .SlideFromBottomToTop:
				animationBlock = {
					self.alpha = 0.0

			}
				bottomMarginConstraint?.constant = -height - bottomMargin

		case .SlideFromLeftToRight:
				leftMarginConstraint?.constant = leftMargin + superViewWidth

			rightMarginConstraint?.constant = -rightMargin + superViewWidth

		case .SlideFromRightToLeft:
				leftMarginConstraint?.constant = leftMargin - superViewWidth

			rightMarginConstraint?.constant = -rightMargin - superViewWidth

		case .Flip:
				animationBlock = {
					self.layer.transform = CATransform3DMakeRotation(CGFloat(M_PI_2), 1, 0, 0)

			}
		}

		self.setNeedsLayout()

		UIView.animateWithDuration(animationDuration, delay: 0, usingSpringWithDamping: 1, initialSpringVelocity: 0.2, options: .CurveEaseIn,
				animations: {
			()->Void in
                    animationBlock ? ()

					self.layoutIfNeeded()

				}) {
			(finished)->Void in
            self.dismissBlock ? (snackbar: self)
            self.removeFromSuperview()

		}
	}

	/**
    Show.
    */
	private func showWithAnimation()
	{
		var animationBlock: (()->Void) ? = nil

		let superViewWidth = CGRectGetWidth((superview?.frame)!)


		switch animationType {
			case .FadeInFadeOut:
				self.alpha = 0.0
	
			self.layoutIfNeeded()
	            // Animation
				animationBlock = {
					self.alpha = 1.0
	
			}
			case .SlideFromBottomBackToBottom, .SlideFromBottomToTop:
				bottomMarginConstraint?.constant = height

			self.layoutIfNeeded()

		case .SlideFromLeftToRight:
				leftMarginConstraint?.constant = leftMargin - superViewWidth

			rightMarginConstraint?.constant = -rightMargin - superViewWidth

			bottomMarginConstraint?.constant = -bottomMargin

			self.layoutIfNeeded()

		case .SlideFromRightToLeft:
				leftMarginConstraint?.constant = leftMargin + superViewWidth

			rightMarginConstraint?.constant = -rightMargin + superViewWidth

			bottomMarginConstraint?.constant = -bottomMargin

			self.layoutIfNeeded()

		case .Flip:
				self.layer.transform = CATransform3DMakeRotation(CGFloat(M_PI_2), 1, 0, 0)

			self.layoutIfNeeded()
			// Animation
				animationBlock = {
					self.layer.transform = CATransform3DMakeRotation(0, 1, 0, 0)

			}
		}

		// Final state
		bottomMarginConstraint?.constant = -bottomMargin

		leftMarginConstraint?.constant = leftMargin

		rightMarginConstraint?.constant = -rightMargin


		UIView.animateWithDuration(animationDuration, delay: 0, usingSpringWithDamping: 0.7, initialSpringVelocity: 5, options: .CurveEaseInOut,
				animations: {
			()->Void in
                    animationBlock ? ()

					self.layoutIfNeeded()

				}, completion: nil)
    }

	/**
    Action button.
    */
	func doAction(button: UIButton)
	{
		// Call action block first
		button == actionButton ? actionBlock ? (snackbar: self) : secondActionBlock ? (snackbar: self)

        // Show activity indicator
        if duration == .Forever && actionButton.hidden == false {
			actionButton.hidden = true

			secondActionButton.hidden = true

			seperateView.hidden = true

			activityIndicatorView.hidden = false

			activityIndicatorView.startAnimating()

		}
		else {
			dismissAnimated(true)

		}
	}
	}
}

