using Microsoft.Maui.Devices;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using TabsNavigation.Core.ViewModels.Main;
using TabsNavigation.Core.ViewModels.Pages;

namespace TabsNavigation.Core;

public class App : MvxApplication
{
    public App()
    {
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterCustomAppStart<AppStart>();
    }
}

public class AppStart : MvxAppStart
{
    public bool InterceptReset { get; set; }

    public AppStart(IMvxApplication application, IMvxNavigationService navigationService) : base(application, navigationService)
    {
    }

    protected override async Task NavigateToFirstViewModel(object hint = null)
    {
        //hint is savedInstanceState on Android that means the app restored the whole navigation stack
        if (DeviceInfo.Platform == DevicePlatform.Android && hint != null)
            return;

        if (!AppSettings.IsTabsRoot)
            await NavigationService.Navigate<TablessRootViewModel>();
        else
            await NavigationService.Navigate<MainViewModel>();
    }
}