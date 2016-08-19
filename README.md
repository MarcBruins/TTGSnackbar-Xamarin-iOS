# TTGSnackbar-Xamarin-iOS
A C#/Xamarin based implementation of the Android Snackbar for iOS.

![Screenshot](http://7nj2iz.com1.z0.glb.clouddn.com/TTGSnackbar_8.gif)
# About
TTGSnackbar is useful for showing a brief message at the bottom of the screen with an action button.  
It appears above all other elements on screen and only one can be displayed at a time.  
It disappears after a timeout or after user click the action button.

### Requirement
iOS 8+

# Usage
## Show a simple message
![Example](http://7nj2iz.com1.z0.glb.clouddn.com/TTGSnackbar_2.png)
```
var snackbar = new TTGSnackbar("Message", TTGSnackbarDuration.Short);
snackbar.Show();
```
## Show a simple message with an action button
![Example](http://7nj2iz.com1.z0.glb.clouddn.com/TTGSnackbar_3.png)
```
var snackbar = new TTGSnackbar("Message", TTGSnackbarDuration.Long, "Cancel", () => { Console.WriteLine("clicked");});   
snackbar.Show();
```

## Show a simple message with a long running action
![Example](http://7nj2iz.com1.z0.glb.clouddn.com/TTGSnackbar_5.png)
```
var snackbar = new TTGSnackbar("Message", TTGSnackbarDuration.Forever, "Cancel", async (s) => {
				await Task.Delay(3000);
				s.dismiss();
			});
snackbar.Show();
```

## Show a simple message with two action buttons
![Example](http://7nj2iz.com1.z0.glb.clouddn.com/TTGSnackbar_9.png)
```
var snackbar = new TTGSnackbar("Message", TTGSnackbarDuration.Middle);

// Action 1
snackbar.ActionText = "Yes";
snackbar.ActionTextColor = UIColor.Green;
snackbar.ActionBlock = (t) => { Console.WriteLine("clicked yes"); };

// Action 2
snackbar.SecondActionText = "No";
snackbar.SecondActionTextColor = UIColor.Purple;
snackbar.SecondActionBlock = (t) => { Console.WriteLine("clicked no"); };

snackbar.Show();
```

# Customization
### Message
`message: String` defines the message to diaplay.

### Message text color
`messageTextColor: UIColor` defines the message text color.

### Message text font
`messageTextFont: UIFont` defines the message text font.

### Display duration
`duration: TTGSnackbarDuration`defines the display duration.  
`TTGSnackbarDuration` : `Short`, `Middle`, `Long` and `Forever`.  
When you set `Forever`, the snackbar will show an activity indicator after user click the action button and you must dismiss the snackbar manually.

### Action title
`actionText: String` defines the action button title.

### Action title color
`actionTextColor: UIColor` defines the action button title color.

### Action title font
`actionTextFont: UIFont` defines the action button title font.

### Action callback
`actionBlock: TTGActionBlock?` will be called when user click the action button.
```
// TTGActionBlock definition.
public Action<TTGSnackbar> ActionBlock
```

### Second action title, color, font and callback
```
secondActionText: String  
secondActionTextColor: UIColor  
secondActionTextFont: UIFont  
secondActionBlock: TTGActionBlock?
```

### Dismiss callback
`dismissBlock: TTGDismissBlock?` will be called when snackbar  dismiss automatically or when user click action button to dismiss the snackbar.
```
// TTGDismissBlock definition.
public typealias TTGDismissBlock = (snackbar: TTGSnackbar) -> Void
```

### Animation type
`animationType: TTGSnackbarAnimationType` defines the style of snackbar when it show and dismiss.  

`TTGSnackbarAnimationType` : `FadeInFadeOut`, `SlideFromBottomToTop`, `SlideFromBottomBackToBottom`, `SlideFromLeftToRight`,  `SlideFromRightToLeft` and `Flip`.

The default value of `animationType` is `SlideFromBottomBackToBottom`, which is the same as Snackbar in Android.

### Animation duration
`animationDuration: NSTimeInterval` defines the duration of show and hide animation.

### Margins
`leftMargin: CGFloat`, `rightMargin: CGFloat` and `bottomMargin: CGFloat` define the margins of snackbar.

### Snackbar height
`height: CGFloat` defines the height of snackbar.

### Corner radius
`cornerRadius: CGFloat` defines the corner radius of snackbar.


# Credits

This is a port of https://github.com/zekunyan/TTGSnackbar/. All credits go to zekunynan.
