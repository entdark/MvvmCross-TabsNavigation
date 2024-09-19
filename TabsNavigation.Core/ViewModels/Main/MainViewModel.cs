using MvvmCross.ViewModels;
using TabsNavigation.Core.ViewModels.Base;
using TabsNavigation.Core.ViewModels.Tabs;

namespace TabsNavigation.Core.ViewModels.Main;

public class MainViewModel : BaseViewModel
{
    private bool initialNavigationDone = false;

    private async Task ShowInitialViewModelsExecute()
    {
        if (initialNavigationDone)
            return;
        initialNavigationDone = true;
        await NavigationService.Navigate(new[] { typeof(Tab1ViewModel), typeof(Tab2ViewModel), typeof(Tab3ViewModel) });
    }

    public override Task Initialize()
    {
        //save so on the next load it shows tabs page
        AppSettings.IsTabsRoot = true;
        return base.Initialize();
    }

    public override void ViewCreated()
    {
        base.ViewCreated();
        //we can only show tabs after our host view is created
        Task.Run(ShowInitialViewModelsExecute);
    }

    protected override void SaveStateToBundle(IMvxBundle bundle)
    {
        base.SaveStateToBundle(bundle);
        bundle.Data[nameof(initialNavigationDone)] = initialNavigationDone.ToString();
    }

    protected override void ReloadFromBundle(IMvxBundle state)
    {
        base.ReloadFromBundle(state);
        if (state.Data.TryGetValue(nameof(initialNavigationDone), out string initDone))
            _ = bool.TryParse(initDone, out initialNavigationDone);
    }
}

