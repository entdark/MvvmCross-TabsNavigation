using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.ViewModels;
using TabsNavigation.Core.Models.Navigation.Hints;

namespace TabsNavigation.iOS.Presenter;

public class TabsNavigationViewPresenter : MvxIosViewPresenter
{
    public TabsNavigationViewPresenter(IUIApplicationDelegate applicationDelegate, UIWindow window) : base(applicationDelegate, window)
    {
    }

    public override async Task<bool> ChangePresentation(MvxPresentationHint hint)
    {
        if (hint is MoveToTabPresentationHint moveToTabHint)
        {
            if (TabBarViewController is UITabBarController tabBarViewController)
            {
                nint oldSelectedIndex = tabBarViewController.SelectedIndex;
                if (moveToTabHint.PopToRoot)
                    (tabBarViewController.SelectedViewController as UINavigationController)?.PopToRootViewController(true);
                tabBarViewController.SelectedIndex = moveToTabHint.Tab;
                if (moveToTabHint.PopToRoot && oldSelectedIndex != tabBarViewController.SelectedIndex)
                    (tabBarViewController.SelectedViewController as UINavigationController)?.PopToRootViewController(true);
                return true;
            }
            return false;
        }
        return await base.ChangePresentation(hint);
    }
}
