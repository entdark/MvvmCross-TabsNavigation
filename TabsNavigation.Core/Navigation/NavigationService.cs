using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using TabsNavigation.Core.Models.Navigation;
using TabsNavigation.Core.Models.Navigation.Hints;

namespace TabsNavigation.Core.Navigation;

public class NavigationService : MvxNavigationService, INavigationService
{
    public NavigationService(IMvxViewModelLoader viewModelLoader, IMvxViewDispatcher viewDispatcher, IMvxIoCProvider iocProvider)
        : base(viewModelLoader, viewDispatcher, iocProvider) { }

    public async Task<bool> Navigate(Type[] viewModelTypes)
    {
        bool []results = await Task.WhenAll(viewModelTypes.Select(type => Navigate(type)));
        return results.All(result => result);
    }

    public async Task MoveToTab(NavigaionTab tab, bool popToRoot = true)
    {
        await this.ChangePresentation(new MoveToTabPresentationHint(tab) { PopToRoot = popToRoot });
    }
}
