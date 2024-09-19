using MvvmCross.Commands;
using TabsNavigation.Core.ViewModels.Base;
using TabsNavigation.Core.ViewModels.Pages;

namespace TabsNavigation.Core.ViewModels.Tabs;

public class Tab1ViewModel : BaseViewModel
{
    public IMvxCommand GoToInnerPageCommand { get; init; }

    public Tab1ViewModel()
    {
        GoToInnerPageCommand = new MvxAsyncCommand(async () => await NavigationService.Navigate<InnerPageViewModel>());
    }
}

public class Tab2ViewModel : BaseViewModel
{
    public IMvxCommand GoToOuterPageCommand { get; init; }

    public Tab2ViewModel()
    {
        GoToOuterPageCommand = new MvxAsyncCommand(async () => await NavigationService.Navigate<OuterPageViewModel>());
    }
}

public class Tab3ViewModel : BaseViewModel
{
    public IMvxCommand GoToTablessPageCommand { get; init; }

    public Tab3ViewModel()
    {
        GoToTablessPageCommand = new MvxAsyncCommand(async () => await NavigationService.Navigate<TablessRootViewModel>());
    }
}