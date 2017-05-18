using System;
using Foundation;
using UIKit;

namespace TTGSnackBar
{
    public enum TTGSnackbarAnimationType
    {
        FadeInFadeOut,
        SlideFromBottomToTop,
        SlideFromBottomBackToBottom,
        SlideFromLeftToRight,
        SlideFromRightToLeft,
    }

    public enum TTGSnackbarLocation
    {
        Bottom,
        Top,
    }

    public class TTGSnackbar : UIView
    {

        /// Snackbar action button max width.
        private const float snackbarActionButtonMaxWidth = 64;

        // Snackbar action button min width.
        private const float snackbarActionButtonMinWidth = 44;

        // Snackbar icon imageView default width
        private const float snackbarIconImageViewWidth = 32;

        // Action callback.
        public Action<TTGSnackbar> ActionBlock { get; set; }

        // Second action block
        public Action<TTGSnackbar> SecondActionBlock { get; set; }

        // Dismiss block
        public Action<TTGSnackbar> DismissBlock { get; set; }

        // Snackbar display duration. Default is Short - 1 second.
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(3);

        // Snackbar animation type. Default is SlideFromBottomBackToBottom.
        public TTGSnackbarAnimationType AnimationType = TTGSnackbarAnimationType.SlideFromLeftToRight;

        // Snackbar location
        public TTGSnackbarLocation LocationType = TTGSnackbarLocation.Bottom;

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

        /// Top margin. Default is 4
        private float _topMargin = 20;
        public float TopMargin
        {
            get { return _topMargin; }
            set
            {
                _topMargin = value; if (topMarginConstraint != null) { topMarginConstraint.Constant = _topMargin; this.LayoutIfNeeded(); }
            }
        }

        /// Left margin. Default is 4
        private float _leftMargin = 4;
        public float LeftMargin
        {
            get { return _leftMargin; }
            set { _leftMargin = value; if (leftMarginConstraint != null) { leftMarginConstraint.Constant = _leftMargin; this.LayoutIfNeeded(); } }
        }

        private float _rightMargin = 4;
        public float RightMargin
        {
            get { return _rightMargin; }
            set { _rightMargin = value; if (rightMarginConstraint != null) { rightMarginConstraint.Constant = _leftMargin; this.LayoutIfNeeded(); } }
        }

        /// Bottom margin. Default is 4
        private float _bottomMargin = 4;
        public float BottomMargin
        {
            get { return _bottomMargin; }
            set { _bottomMargin = value; if (bottomMarginConstraint != null) { bottomMarginConstraint.Constant = _bottomMargin; this.LayoutIfNeeded(); } }
        }

        private float _height = 44;
        public float Height
        {
            get { return _height; }
            set { _height = value; if (heightConstraint != null) { heightConstraint.Constant = _height; this.LayoutIfNeeded(); } }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; if (this.messageLabel != null) { this.messageLabel.Text = _message; } }
        }

        private UIColor _messageTextColor = UIColor.White;
        public UIColor MessageTextColor
        {
            get { return _messageTextColor; }
            set { _messageTextColor = value; this.messageLabel.TextColor = _messageTextColor; }
        }

        private UIFont _messageTextFont = UIFont.BoldSystemFontOfSize(14);
        public UIFont MessageTextFont
        {
            get { return _messageTextFont; }
            set { _messageTextFont = value; this.messageLabel.Font = _messageTextFont; }
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
            set { _actionText = value; if (this.actionButton != null) { this.actionButton.SetTitle(_actionText, UIControlState.Normal); } }
        }

        private string _secondActionText;
        public string SecondActionText
        {
            get { return _secondActionText; }
            set { _secondActionText = value; if (this.secondActionButton != null) { this.secondActionButton.SetTitle(_secondActionText, UIControlState.Normal); } }
        }

        // Action button title color. Default is white.
        private UIColor _actionTextColor = UIColor.White;
        public UIColor ActionTextColor
        {
            get { return _actionTextColor; }
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
            get { return _actionTextFont; }
            set { _actionTextFont = value; this.actionButton.TitleLabel.Font = _actionTextFont; }
        }

        // First action text font. Default is Bold system font (14).
        private UIFont _secondActionTextFont = UIFont.BoldSystemFontOfSize(14);
        public UIFont SecondActionTextFont
        {
            get { return _secondActionTextFont; }
            set { _secondActionTextFont = value; this.secondActionButton.TitleLabel.Font = _secondActionTextFont; }
        }

        private UIImage _icon;
        public UIImage Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                iconImageView.Image = _icon;
            }
        }

        private UIViewContentMode _iconContentMode = UIViewContentMode.Center;
        public UIViewContentMode IconContentMode
        {
            get { return _iconContentMode; }
            set
            {
                _iconContentMode = value;
                iconImageView.ContentMode = _iconContentMode;
            }
        }

        public UIImageView iconImageView;
        public UILabel messageLabel;
        public UIView seperateView;
        public UIButton actionButton;
        public UIButton secondActionButton;
        public UIActivityIndicatorView activityIndicatorView;

        // Timer to dismiss the snackbar.
        private NSTimer dismissTimer;

        // Constraints.
        private NSLayoutConstraint heightConstraint;
        private NSLayoutConstraint leftMarginConstraint;
        private NSLayoutConstraint rightMarginConstraint;
        private NSLayoutConstraint bottomMarginConstraint;
        private NSLayoutConstraint topMarginConstraint;
        private NSLayoutConstraint actionButtonWidthConstraint;
        private NSLayoutConstraint secondActionButtonWidthConstraint;
        private NSLayoutConstraint iconImageViewWidthConstraint;

        /// <summary>
        /// Show a single message like an Android snackbar.
        /// - parameter message:  Message text.
        /// </summary>
        public TTGSnackbar(string message) : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            this.Message = message;
            configure();
        }

        /// <summary>
        /// Show a message with action button.
        /// - parameter message:     Message text.
        /// - parameter actionText:  Action button title.
        /// - parameter actionBlock: Action callback closure.
        /// - returns: Void
        /// </summary>
        public TTGSnackbar(string message, string actionText, Action<TTGSnackbar> ttgAction) : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            this.Message = message;
            this.ActionText = actionText;
            this.ActionBlock = ttgAction;

            configure();
        }

        /// <summary>
        /// Show a custom message with action button.
        /// - parameter message:          Message text.
        /// - parameter actionText:       Action button title.
        /// - parameter messageFont:      Message label font.
        /// - parameter actionButtonFont: Action button font.
        /// - parameter actionBlock:      Action callback closure.
        /// - returns: Void
        /// </summary>
        public TTGSnackbar(string message, string actionText, UIFont messageFont, UIFont actionTextFont, Action<TTGSnackbar> ttgAction) : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            this.Message = message;
            this.ActionText = actionText;
            this.ActionBlock = ttgAction;
            this.MessageTextFont = messageFont;
            this.ActionTextFont = actionTextFont;

            configure();
        }


        /// <summary>
        /// Show the snackbar.
        /// </summary>
        public void Show()
        {
            // Only show once
            if (this.Superview != null)
                return;

            // Create dismiss timer
            dismissTimer = NSTimer.CreateScheduledTimer(Duration, (t) => Dismiss());

            // Show or hide action button

            iconImageView.Hidden = (Icon == null);

            if (ActionBlock == null)
            {
                ActionText = String.Empty;
                actionButton.Hidden = true;
            }

            if (secondActionButton == null)
            {
                SecondActionText = String.Empty;
                secondActionButton.Hidden = true;
            }

            seperateView.Hidden = actionButton.Hidden;

            iconImageViewWidthConstraint.Constant = iconImageView.Hidden ? 0 : TTGSnackbar.snackbarIconImageViewWidth;
            actionButtonWidthConstraint.Constant = actionButton.Hidden ? 0 : (secondActionButton.Hidden ? TTGSnackbar.snackbarActionButtonMaxWidth : TTGSnackbar.snackbarActionButtonMinWidth);
            secondActionButtonWidthConstraint.Constant = secondActionButton.Hidden ? 0 : (actionButton.Hidden ? TTGSnackbar.snackbarActionButtonMaxWidth : TTGSnackbar.snackbarActionButtonMinWidth);

            this.LayoutIfNeeded();

            var localSuperView = UIApplication.SharedApplication.KeyWindow;
            if (localSuperView != null)
            {
                localSuperView.AddSubview(this);

                topMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Top,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Top,
                    1,
                    TopMargin);

                heightConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.GreaterThanOrEqual,
                    null,
                    NSLayoutAttribute.NoAttribute,
                    1,
                    Height);

                leftMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Left,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Left,
                    1,
                    LeftMargin);

                rightMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Right,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Right,
                    1,
                    -RightMargin);

                bottomMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Bottom,
                    NSLayoutRelation.Equal,
                    localSuperView,
                    NSLayoutAttribute.Bottom,
                    1,
                    -BottomMargin);

                // Avoid the "UIView-Encapsulated-Layout-Height" constraint conflicts
                // http://stackoverflow.com/questions/25059443/what-is-nslayoutconstraint-uiview-encapsulated-layout-height-and-how-should-i
                leftMarginConstraint.Priority = 999;
                rightMarginConstraint.Priority = 999;

                this.AddConstraint(heightConstraint);
                localSuperView.AddConstraint(leftMarginConstraint);
                localSuperView.AddConstraint(rightMarginConstraint);

                switch (LocationType)
                {
                    case TTGSnackbarLocation.Top:
                        localSuperView.AddConstraint(topMarginConstraint);
                        break;
                    default:
                        localSuperView.AddConstraint(bottomMarginConstraint);
                        break;
                }


                // Show 
                showWithAnimation();
            }
            else
            {
                Console.WriteLine("TTGSnackbar needs a keyWindows to display.");
            }
        }

        /// <summary>
        /// Dismiss the snackbar manually..
        /// </summary>
        public void Dismiss()
        {
            this.dismissAnimated(true);
        }

        /// <summary>
        /// Configure this instance.
        /// </summary>
        private void configure()
        {
            this.TranslatesAutoresizingMaskIntoConstraints = false;
            this.BackgroundColor = UIColor.DarkGray;
            this.Layer.CornerRadius = CornerRadius;
            this.Layer.MasksToBounds = true;

            iconImageView = new UIImageView();
            iconImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            iconImageView.BackgroundColor = UIColor.Clear;
            iconImageView.ContentMode = IconContentMode;

            this.AddSubview(iconImageView);

            messageLabel = new UILabel();
            messageLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            messageLabel.TextColor = UIColor.White;
            messageLabel.Font = MessageTextFont;
            messageLabel.BackgroundColor = UIColor.Clear;
            messageLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
            messageLabel.Lines = 2;
            messageLabel.TextAlignment = UITextAlignment.Left;
            messageLabel.Text = Message;

            this.AddSubview(messageLabel);

            actionButton = new UIButton();
            actionButton.TranslatesAutoresizingMaskIntoConstraints = false;
            actionButton.BackgroundColor = UIColor.Clear;
            actionButton.TitleLabel.Font = ActionTextFont;
            actionButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
            actionButton.SetTitle(ActionText, UIControlState.Normal);
            actionButton.SetTitleColor(ActionTextColor, UIControlState.Normal);
            actionButton.TouchUpInside += (s, e) => doAction(actionButton);

            this.AddSubview(actionButton);

            secondActionButton = new UIButton();
            secondActionButton.TranslatesAutoresizingMaskIntoConstraints = false;
            secondActionButton.BackgroundColor = UIColor.Clear;
            secondActionButton.TitleLabel.Font = SecondActionTextFont;
            secondActionButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
            secondActionButton.SetTitle(SecondActionText, UIControlState.Normal);
            secondActionButton.SetTitleColor(SecondActionTextColor, UIControlState.Normal);
            secondActionButton.TouchUpInside += (s, e) => doAction(secondActionButton);

            this.AddSubview(secondActionButton);

            seperateView = new UIView();
            seperateView.TranslatesAutoresizingMaskIntoConstraints = false;
            seperateView.BackgroundColor = UIColor.Gray;

            this.AddSubview(seperateView);

            activityIndicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
            activityIndicatorView.TranslatesAutoresizingMaskIntoConstraints = false;
            activityIndicatorView.StopAnimating();

            this.AddSubview(activityIndicatorView);

            // Add constraints

            var hConstraints = NSLayoutConstraint.FromVisualFormat(
                "H:|-0-[iconImageView]-2-[messageLabel]-2-[seperateView(0.5)]-2-[actionButton(>=44@999)]-0-[secondActionButton(>=44@999)]-0-|",
                0, new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] {
                        iconImageView,
                        messageLabel,
                        seperateView,
                        actionButton,
                        secondActionButton
                }, new NSObject[] {
                    new NSString("iconImageView"),
                    new NSString("messageLabel"),
                    new NSString("seperateView"),
                    new NSString("actionButton"),
                    new NSString("secondActionButton")
                })
            );

            var vConstraintsForIconImageView = NSLayoutConstraint.FromVisualFormat(
                "V:|-2-[iconImageView]-2-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { iconImageView }, new NSObject[] { new NSString("iconImageView") })
            );

            var vConstraintsForMessageLabel = NSLayoutConstraint.FromVisualFormat(
                "V:|-0-[messageLabel]-0-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { messageLabel }, new NSObject[] { new NSString("messageLabel") })
            );

            var vConstraintsForSeperateView = NSLayoutConstraint.FromVisualFormat(
                "V:|-4-[seperateView]-4-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { seperateView }, new NSObject[] { new NSString("seperateView") })
            );

            var vConstraintsForActionButton = NSLayoutConstraint.FromVisualFormat(
                "V:|-0-[actionButton]-0-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { actionButton }, new NSObject[] { new NSString("actionButton") })
            );

            var vConstraintsForSecondActionButton = NSLayoutConstraint.FromVisualFormat(
                "V:|-0-[secondActionButton]-0-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { secondActionButton }, new NSObject[] { new NSString("secondActionButton") })
            );

            iconImageViewWidthConstraint = NSLayoutConstraint.Create(iconImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, TTGSnackbar.snackbarIconImageViewWidth);

            actionButtonWidthConstraint = NSLayoutConstraint.Create(actionButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, TTGSnackbar.snackbarActionButtonMinWidth);

            secondActionButtonWidthConstraint = NSLayoutConstraint.Create(secondActionButton, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, TTGSnackbar.snackbarActionButtonMinWidth);

            var vConstraintsForActivityIndicatorView = NSLayoutConstraint.FromVisualFormat(
                "V:|-2-[activityIndicatorView]-2-|", 0, new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { activityIndicatorView }, new NSObject[] { new NSString("activityIndicatorView") })
            );

            var hConstraintsForActivityIndicatorView = NSLayoutConstraint.FromVisualFormat(
                "H:[activityIndicatorView]-2-|",
                0,
                new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] { activityIndicatorView },
                    new NSObject[] { new NSString("activityIndicatorView") })
            );

            iconImageView.AddConstraint(iconImageViewWidthConstraint);
            actionButton.AddConstraint(actionButtonWidthConstraint);
            secondActionButton.AddConstraint(secondActionButtonWidthConstraint);

            this.AddConstraints(hConstraints);
            this.AddConstraints(vConstraintsForIconImageView);
            this.AddConstraints(vConstraintsForMessageLabel);
            this.AddConstraints(vConstraintsForSeperateView);
            this.AddConstraints(vConstraintsForActionButton);
            this.AddConstraints(vConstraintsForSecondActionButton);
            this.AddConstraints(vConstraintsForActivityIndicatorView);
            this.AddConstraints(hConstraintsForActivityIndicatorView);
        }

        /// <summary>
        /// Invalid the dismiss timer.
        /// </summary>
        private void invalidDismissTimer()
        {
            if (dismissTimer != null)
            {
                dismissTimer.Invalidate();
                dismissTimer = null;
            }
        }

        /// <summary>
        /// If dismiss with animation.
        /// </summary>
        private void dismissAnimated(bool animated)
        {
            invalidDismissTimer();

            activityIndicatorView.StopAnimating();

            nfloat superViewWidth = 0;

            if (Superview != null)
                superViewWidth = Superview.Frame.Width;

            if (!animated)
            {
                DismissAndPerformAction();
                return;
            }

            Action animationBlock = () => { };

            switch (AnimationType)
            {
                case TTGSnackbarAnimationType.FadeInFadeOut:
                    animationBlock = () => { this.Alpha = 0; };
                    break;
                case TTGSnackbarAnimationType.SlideFromBottomBackToBottom:
                    animationBlock = () => { bottomMarginConstraint.Constant = Height; };
                    break;
                case TTGSnackbarAnimationType.SlideFromBottomToTop:
                    animationBlock = () => { this.Alpha = 0; bottomMarginConstraint.Constant = -Height - BottomMargin; };
                    break;
                case TTGSnackbarAnimationType.SlideFromLeftToRight:
                    animationBlock = () => { leftMarginConstraint.Constant = LeftMargin + superViewWidth; rightMarginConstraint.Constant = -RightMargin + superViewWidth; };
                    break;
                case TTGSnackbarAnimationType.SlideFromRightToLeft:
                    animationBlock = () =>
                    {
                        leftMarginConstraint.Constant = LeftMargin - superViewWidth;
                        rightMarginConstraint.Constant = -RightMargin - superViewWidth;
                    };
                    break;
            };

            this.SetNeedsLayout();

            UIView.Animate(AnimationDuration, 0, UIViewAnimationOptions.CurveEaseIn, animationBlock, DismissAndPerformAction);
        }

        void DismissAndPerformAction()
        {
            if (DismissBlock != null)
            {
                DismissBlock(this);
            }
            else if (ActionBlock != null)
            {
                ActionBlock(this);
            }

            this.RemoveFromSuperview();
        }

        /// <summary>
        /// Shows with animation.
        /// </summary>
        private void showWithAnimation()
        {
            Action animationBlock = () => { this.LayoutIfNeeded(); };
            var superViewWidth = Superview.Frame.Width;

            switch (AnimationType)
            {
                case TTGSnackbarAnimationType.FadeInFadeOut:
                    this.Alpha = 0;
                    this.SetNeedsLayout();

                    animationBlock = () => { this.Alpha = 1; };
                    break;
                case TTGSnackbarAnimationType.SlideFromBottomBackToBottom:
                case TTGSnackbarAnimationType.SlideFromBottomToTop:
                    bottomMarginConstraint.Constant = -BottomMargin;
                    this.LayoutIfNeeded();
                    break;
                case TTGSnackbarAnimationType.SlideFromLeftToRight:
                    leftMarginConstraint.Constant = LeftMargin - superViewWidth;
                    rightMarginConstraint.Constant = -RightMargin - superViewWidth;
                    bottomMarginConstraint.Constant = -BottomMargin;
                    this.LayoutIfNeeded();
                    break;
                case TTGSnackbarAnimationType.SlideFromRightToLeft:
                    leftMarginConstraint.Constant = LeftMargin + superViewWidth;
                    rightMarginConstraint.Constant = -RightMargin + superViewWidth;
                    bottomMarginConstraint.Constant = -BottomMargin;
                    this.LayoutIfNeeded();
                    break;
            };

            // Final state
            bottomMarginConstraint.Constant = -BottomMargin;
            leftMarginConstraint.Constant = LeftMargin;
            rightMarginConstraint.Constant = -RightMargin;
            topMarginConstraint.Constant = TopMargin;

            UIView.AnimateNotify(
                    AnimationDuration,
                    0,
                    0.7f,
                    5f,
                    UIViewAnimationOptions.CurveEaseInOut,
                      animationBlock,
                    null
                );
        }

        private void doAction(UIButton button)
        {
            // Call action block first
            if (button == actionButton)
            {
                ActionBlock(this);
            }
            else if (button == secondActionButton)
            {
                SecondActionBlock(this);
            }

            dismissAnimated(true);
        }
    }
}


