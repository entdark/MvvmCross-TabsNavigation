using Android.Views;
using AndroidX.ViewPager.Widget;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.ViewPager;
using TabsNavigation.Android.Controls;
using TabsNavigation.Core.ViewModels.Base;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace TabsNavigation.Android.Views.Base;

public abstract class TabsFragment<TViewModel> : BaseRootFragment<TViewModel>, ITabsView where TViewModel : class, IBaseViewModel
{
    private readonly int viewPagerId;
    private readonly int bottomNavigationViewId;
    private readonly int tabsCount;

    protected TabsViewPager ViewPager { get; private set; }
    protected TabsBottomNavigationView BottomNavigationView { get; private set; }

    public Fragment CurrentTabFragment => ViewPager?.CurrentFragment;
    public int CurrentTab => ViewPager?.CurrentItem ?? -1;

    public int DefaultTab { get; init; } = 0;

    public TabsFragment(int layoutId, int viewPagerId, int bottomNavigationViewId, int tabsCount) : base(layoutId)
    {
        this.viewPagerId = viewPagerId;
        this.bottomNavigationViewId = bottomNavigationViewId;
        this.tabsCount = tabsCount;
        RegisterBackPressedCallback = true;
    }

    public override void OnViewCreated(View view, Bundle savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);

        ViewPager = view.FindViewById<TabsViewPager>(viewPagerId);
        ViewPager.ScrollEnabled = false;
        ViewPager.OffscreenPageLimit = tabsCount;
        ViewPager.PageSelected += TabPageSelected;

        if (ViewPager.Adapter is not TabsViewPager.TabsAdapter)
            ViewPager.Adapter = new TabsViewPager.TabsAdapter(Context, ChildFragmentManager, new List<MvxViewPagerFragmentInfo>(), tabsCount);

        BottomNavigationView = view.FindViewById<TabsBottomNavigationView>(bottomNavigationViewId);
        BottomNavigationView.ViewPager = ViewPager;
    }

    public override void OnDestroyView()
    {
        if (ViewPager != null)
        {
            ViewPager.PageSelected -= TabPageSelected;
        }

        base.OnDestroyView();
    }

    private void TabPageSelected(object sender, ViewPager.PageSelectedEventArgs ev)
    {
        ToggleBackPressedCallback(true);
    }

    public void CloseFragments(bool animated, int tab = -1)
    {
        ViewPager.CloseTabsInnerFragments(animated, tab);
    }

    public void MoveToTab(int tab)
    {
        ViewPager.SetCurrentItem(tab, false);
    }

    protected override void OnBackPressedCallback()
    {
        var fragmentManager = ViewPager.CurrentFragment?.ChildFragmentManager;
        if (fragmentManager?.BackStackEntryCount > 0)
        {
            var fragment = fragmentManager.Fragments?.LastOrDefault();
            var baseFragment = fragment as IBaseFragment;
            var mvxFragment = fragment as IMvxFragmentView;
            bool handled = baseFragment?.OnBackPressed() ?? false;
            if (!handled)
            {
                if (!fragmentManager.IsStateSaved && fragmentManager.PopBackStackImmediate())
                    return;

                Mvx.IoCProvider.Resolve<IMvxNavigationService>().Close(mvxFragment.ViewModel);
            }
        }
        else
        {
            if (CurrentTab != DefaultTab)
            {
                MoveToTab(DefaultTab);
            }
            else
            {
                ToggleBackPressedCallback(false);
                Activity.OnBackPressedDispatcher.OnBackPressed();
            }
        }
    }
}
