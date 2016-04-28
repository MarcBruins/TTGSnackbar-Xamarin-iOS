using System;
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

		/// Snackbar action button min width.
		private const float snackbarActionButtonMinWidth = 44;

		/// Action callback closure definition.
		public Action TTGActionBlock { get; set; }

		/// Dismiss callback closure definition.
		public Action TTGDismissBlock { get; set; }

		/// Action callback.
		public Action ActionBlock { get; set; }

		/// Second action block
		public Action SecondActionBlock { get; set; }

		/// Snackbar display duration. Default is Short - 1 second.
		public TTGSnackbarDuration Duration = TTGSnackbarDuration.Short;

		/// Snackbar animation type. Default is SlideFromBottomBackToBottom.
		public TTGSnackbarAnimationType AnimationType = TTGSnackbarAnimationType.SlideFromBottomBackToBottom;

		/// Show and hide animation duration. Default is 0.3
		public float AnimationDuration = 0.3f;

		private float _cornerRadius = 4;
		public float CornerRadius
		{
			get { return _cornerRadius; }
			set
			{
				if (_cornerRadius > value)
				{
					_cornerRadius = value / 2;
				}
				if (_cornerRadius < 0)
					_cornerRadius = 0;
				
				this.Layer.CornerRadius = _cornerRadius;
				this.Layer.MasksToBounds = true;
			}
		}


    /// Left margin. Default is 4
    public var leftMargin: CGFloat = 4 {
        didSet {
            leftMarginConstraint?.constant = leftMargin
			self.layoutIfNeeded()
        }
    }

    /// Right margin. Default is 4
    public var rightMargin: CGFloat = 4 {
        didSet {
            rightMarginConstraint?.constant = -rightMargin
			self.layoutIfNeeded()
        }
    }

    /// Bottom margin. Default is 4
    public var bottomMargin: CGFloat = 4 {
        didSet {
            bottomMarginConstraint?.constant = -bottomMargin
			self.layoutIfNeeded()
        }
    }

    /// Height: [44, +]. Default is 44
    public var height: CGFloat = 44 {
        didSet {
            if height< 44 {
                height = 44
            }
            heightConstraint?.constant = height
			self.layoutIfNeeded()
        }
    }

    /// Main text shown on the snackbar.
    public var message: String = "" {
        didSet {
            self.messageLabel.text = message
        }
    }

    /// Message text color. Default is white.
    public dynamic var messageTextColor: UIColor = UIColor.whiteColor() {
        didSet {
            self.messageLabel.textColor = messageTextColor
        }
    }

    /// Message text font. Default is Bold system font (14).
    public dynamic var messageTextFont: UIFont = UIFont.boldSystemFontOfSize(14) {
        didSet {
            self.messageLabel.font = messageTextFont
        }
    }

    /// Message text alignment. Default is left
    public dynamic var messageTextAlign: NSTextAlignment = .Left {
        didSet {
            self.messageLabel.textAlignment = messageTextAlign
        }
    }

    /// Action button title.
    public dynamic var actionText: String = "" {
        didSet {
            self.actionButton.setTitle(actionText, forState: UIControlState.Normal)
        }
    }

    /// Second action button title.
    public dynamic var secondActionText: String = "" {
        didSet {
            self.secondActionButton.setTitle(secondActionText, forState: UIControlState.Normal)
        }
    }

    /// Action button title color. Default is white.
    public dynamic var actionTextColor: UIColor = UIColor.whiteColor() {
        didSet {
            actionButton.setTitleColor(actionTextColor, forState: UIControlState.Normal)
        }
    }

    /// Second action button title color. Default is white.
    public dynamic var secondActionTextColor: UIColor = UIColor.whiteColor() {
        didSet {
            secondActionButton.setTitleColor(secondActionTextColor, forState: UIControlState.Normal)
        }
    }

    /// Action text font. Default is Bold system font (14).
    public dynamic var actionTextFont: UIFont = UIFont.boldSystemFontOfSize(14) {
        didSet {
            self.actionButton.titleLabel?.font = actionTextFont
        }
    }

    /// Second action text font. Default is Bold system font (14).
    public dynamic var secondActionTextFont: UIFont = UIFont.boldSystemFontOfSize(14) {
        didSet {
            self.secondActionButton.titleLabel?.font = secondActionTextFont
        }
    }

    // MARK: -
    // MARK: Private property.

    private var messageLabel: UILabel!
    private var seperateView: UIView!
    private var actionButton: UIButton!
    private var secondActionButton: UIButton!
    private var activityIndicatorView: UIActivityIndicatorView!

    /// Timer to dismiss the snackbar.
    private var dismissTimer: NSTimer? = nil

	// Constraints.
	private var heightConstraint: NSLayoutConstraint? = nil
	private var leftMarginConstraint: NSLayoutConstraint? = nil
	private var rightMarginConstraint: NSLayoutConstraint? = nil
	private var bottomMarginConstraint: NSLayoutConstraint? = nil
	private var actionButtonWidthConstraint: NSLayoutConstraint? = nil
	private var secondActionButtonWidthConstraint: NSLayoutConstraint? = nil

		public TTGSnackbar()
		{
		}
	}
}

