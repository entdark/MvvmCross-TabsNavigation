// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace TabsNavigation.iOS.Views.Pages
{
	[Register ("InnerViewController")]
	partial class BaseInnerViewController<TViewModel>
    {
		[Outlet]
		UIKit.UIButton GoBackButton { get; set; }

		[Outlet]
		UIKit.UIButton GoDeeperButton { get; set; }

		[Outlet]
		UIKit.UILabel StackLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (StackLabel != null) {
				StackLabel.Dispose ();
				StackLabel = null;
			}

			if (GoDeeperButton != null) {
				GoDeeperButton.Dispose ();
				GoDeeperButton = null;
			}

			if (GoBackButton != null) {
				GoBackButton.Dispose ();
				GoBackButton = null;
			}
		}
	}
}
