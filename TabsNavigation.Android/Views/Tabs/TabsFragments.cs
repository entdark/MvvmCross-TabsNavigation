using TabsNavigation.Android.Views.Base;
using TabsNavigation.Core.ViewModels.Tabs;

namespace TabsNavigation.Android.Views.Tabs;

public class Tab1Fragment : BaseTabFragment<Tab1ViewModel>
{
    public Tab1Fragment() : base(Resource.Layout.tab1_page, "Tab stack", Resource.Drawable.stacks_states)
    {
    }
}

public class Tab2Fragment : BaseTabFragment<Tab2ViewModel>
{
    public Tab2Fragment() : base(Resource.Layout.tab2_page, "Main stack", Resource.Drawable.web_stories_states)
    {
    }
}

public class Tab3Fragment : BaseTabFragment<Tab3ViewModel>
{
    public Tab3Fragment() : base(Resource.Layout.tab3_page, "Tabless root", Resource.Drawable.rectangle_states)
    {
    }
}