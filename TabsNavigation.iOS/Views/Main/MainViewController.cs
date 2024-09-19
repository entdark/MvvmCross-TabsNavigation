using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using TabsNavigation.Core.ViewModels.Main;

namespace TabsNavigation.iOS.Views.Main;

[MvxRootPresentation]
public partial class MainViewController : MvxTabBarViewController<MainViewModel>
{
    public MainViewController() : base(nameof(MainViewController), null)
    {
    }

    protected override void SetTitleAndTabBarItem(UIViewController viewController, MvxTabPresentationAttribute attribute)
    {
        base.SetTitleAndTabBarItem(viewController, attribute);
        viewController.TabBarItem.Image = UIImage.GetSystemImage(attribute.TabIconName);
        viewController.TabBarItem.SelectedImage = UIImage.GetSystemImage(attribute.TabSelectedIconName);
    }
}

