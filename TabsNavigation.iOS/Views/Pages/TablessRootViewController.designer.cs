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
	[Register ("TablessRootViewController")]
	partial class TablessRootViewController
	{
		[Outlet]
		UIKit.UIButton GoDeeperButton { get; set; }

		[Outlet]
		UIKit.UIButton NavigationButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (GoDeeperButton != null) {
				GoDeeperButton.Dispose ();
				GoDeeperButton = null;
			}

			if (NavigationButton != null) {
				NavigationButton.Dispose ();
				NavigationButton = null;
			}
		}
	}
}
