using MvvmCross.Navigation;
using TabsNavigation.Core.Models.Navigation;

namespace TabsNavigation.Core.Navigation;

public interface INavigationService : IMvxNavigationService
{
    Task<bool> Navigate(Type[] viewModelTypes);

    Task MoveToTab(NavigationTab tab, bool popToRoot = true);
}
