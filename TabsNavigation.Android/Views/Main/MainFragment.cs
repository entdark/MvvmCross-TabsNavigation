using TabsNavigation.Android.Views.Base;
using TabsNavigation.Core.ViewModels.Main;

namespace TabsNavigation.Android.Views.Main;

public class MainFragment : TabsFragment<MainViewModel>
{
    public MainFragment() : base(Resource.Layout.main_page, Resource.Id.tabs_viewpager, Resource.Id.tabs_navigationview, 3)
    {
    }
}
