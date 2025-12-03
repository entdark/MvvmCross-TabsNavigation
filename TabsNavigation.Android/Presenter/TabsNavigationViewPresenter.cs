using System.Reflection;
using Android.Views;
using Microsoft.Extensions.Logging;
using MvvmCross.Exceptions;
using MvvmCross.Logging;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Fragments;
using MvvmCross.Presenters;
using MvvmCross.ViewModels;
using TabsNavigation.Android.Controls;
using TabsNavigation.Android.Presenter.Attributes;
using TabsNavigation.Android.Views.Base;
using TabsNavigation.Core.Models.Navigation.Hints;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace TabsNavigation.Android.Presenter;

public class TabsNavigationViewPresenter : MvxAndroidViewPresenter, ITabsNavigationViewPresenter
{
	public bool PreferShowHideOverReplace { get; init; }

	public TabsNavigationViewPresenter(IEnumerable<Assembly> androidViewAssemblies) : base(androidViewAssemblies)
    {
    }

    public override void RegisterAttributeTypes()
    {
        base.RegisterAttributeTypes();
        AttributeTypesToActionsDictionary.Register<RootFragmentPresentationAttribute>(ShowRootFragment, CloseRootFragment);
        AttributeTypesToActionsDictionary.Register<TabFragmentPresentationAttribute>(ShowTabFragment, CloseTabFragment);
        AttributeTypesToActionsDictionary.Register<PushFragmentPresentationAttribute>(ShowPushFragment, ClosePushFragment);
    }

    protected Task<bool> ShowPushFragment(Type view, PushFragmentPresentationAttribute attribute, MvxViewModelRequest request)
    {
        if (attribute.TabsFragmentHostViewType != null)
        {
            var fragment = GetFragmentByViewType(attribute.TabsFragmentHostViewType);
            if (fragment is ITabsView tabsFragment)
            {
                var tabFragment = tabsFragment.CurrentTabFragment;
                PerformShowFragmentTransaction(tabFragment.ChildFragmentManager, attribute, request);
                return Task.FromResult(true);
            }
        }
        return base.ShowFragment(view, attribute, request);
    }

    protected Task<bool> ClosePushFragment(IMvxViewModel viewModel, PushFragmentPresentationAttribute attribute)
    {
        if (attribute.TabsFragmentHostViewType != null)
        {
            var fragment = GetFragmentByViewType(attribute.TabsFragmentHostViewType);
            if (fragment is ITabsView tabsFragment)
            {
                var tabFragment = tabsFragment.CurrentTabFragment;
                if (TryPerformCloseFragmentTransaction(tabFragment.ChildFragmentManager, attribute))
                {
                    return Task.FromResult(true);
                }
            }
        }
        return base.CloseFragment(viewModel, attribute);
    }

    protected virtual async Task<bool> ShowTabFragment(
        Type view,
        TabFragmentPresentationAttribute attribute,
        MvxViewModelRequest request)
    {
        var actualViewType = attribute.ViewType;
        var actualViewModelType = attribute.ViewModelType;
        var tabViewModel = new WrapperTabViewModel(tabFragment =>
        {
            var actualAttribute = new RootFragmentPresentationAttribute()
            {
                ViewType = actualViewType,
                ViewModelType = actualViewModelType
            };
            PerformShowFragmentTransaction(tabFragment.ChildFragmentManager, actualAttribute, request);
        });
        var tabViewModelRequest = new MvxViewModelInstanceRequest(tabViewModel);
        attribute.ViewType = typeof(WrapperTabFragment);
        attribute.ViewModelType = typeof(WrapperTabViewModel);
        attribute.Tag = $"{Guid.NewGuid()}.{nameof(WrapperTabFragment)}";
        var showViewPagerFragment = await ShowViewPagerFragment(view, attribute, tabViewModelRequest).ConfigureAwait(true);
        if (!showViewPagerFragment)
            return false;

        TabsViewPager viewPager = null;
        TabsBottomNavigationView bottomNavigationView = null;

        // check for a ViewPager inside a Fragment
        if (attribute.FragmentHostViewType != null)
        {
            var fragment = GetFragmentByViewType(attribute.FragmentHostViewType);

            viewPager = fragment?.View.FindViewById<TabsViewPager>(attribute.ViewPagerResourceId);
            bottomNavigationView = fragment?.View.FindViewById<TabsBottomNavigationView>(attribute.BottomNavigationViewResourceId);
        }

        // check for a ViewPager inside an Activity
        if (CurrentActivity.IsActivityAlive() && attribute?.ActivityHostViewModelType != null)
        {
            viewPager = CurrentActivity?.FindViewById<TabsViewPager>(attribute.ViewPagerResourceId);
            bottomNavigationView = CurrentActivity?.FindViewById<TabsBottomNavigationView>(attribute.BottomNavigationViewResourceId);
        }

        if (viewPager == null || bottomNavigationView == null)
            throw new MvxException("ViewPager or BottomNavigationView not found");

        bottomNavigationView.ViewPager ??= viewPager;
        if (!bottomNavigationView.TryRegisterViewModel(request.ViewModelType, attribute.Title, attribute.IconDrawableResourceId))
        {
            return false;
        }

        return true;
    }

    protected virtual Task<bool> CloseTabFragment(IMvxViewModel viewModel, TabFragmentPresentationAttribute attribute)
    {
        return base.CloseViewPagerFragment(viewModel, attribute);
    }

    protected Task<bool> ShowRootFragment(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
    {
        if (attribute.FragmentHostViewType != null)
        {
            ShowNestedFragment(view, attribute, request);
            return Task.FromResult(true);
        }
        if (attribute.ActivityHostViewModelType == null)
        {
            attribute.ActivityHostViewModelType = GetCurrentActivityViewModelType();
        }
        Type currentActivityViewModelType = GetCurrentActivityViewModelType();
        if (attribute.ActivityHostViewModelType != currentActivityViewModelType)
        {
            MvxLogHost.Default?.Log(LogLevel.Warning, "Activity host with ViewModelType {activityHostViewModelType} is not CurrentTopActivity. Showing Activity before showing Fragment for {viewModelType}", new object[2] { attribute.ActivityHostViewModelType, attribute.ViewModelType });
            PendingRequest = request;
            ShowHostActivity(attribute);
        }
        else if (CurrentActivity.IsActivityAlive())
        {
            if (CurrentActivity.FindViewById(attribute.FragmentContentId) == null)
            {
                throw new InvalidOperationException("FrameLayout to show Fragment not found");
            }
            CloseFragments(false);
            PerformShowFragmentTransaction(CurrentActivity.SupportFragmentManager, attribute, request);
        }
        return Task.FromResult(true);
    }

    protected Task<bool> CloseRootFragment(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
    {
        return base.CloseFragment(viewModel, attribute);
    }

    protected void CloseFragments(bool animated)
    {
        if (CurrentFragmentManager.Fragments?.FirstOrDefault() is ITabsView tabsFragment)
        {
            tabsFragment.CloseFragments(animated);
        }

        if (!animated)
            IBaseFragment.DisableAnimations = true;
        for (int i = 0, count = CurrentFragmentManager.BackStackEntryCount; i < count; ++i)
        {
            if (animated)
            {
                CurrentFragmentManager.PopBackStack();
            }
            else
            {
                CurrentFragmentManager.PopBackStackImmediate();
            }
        }
        IBaseFragment.DisableAnimations = false;
    }

    public override async Task<bool> ChangePresentation(MvxPresentationHint hint)
    {
        if (hint is MoveToTabPresentationHint moveToTabHint)
        {
            if (CurrentFragmentManager.Fragments?.FirstOrDefault() is ITabsView tabsFragment)
            {
                int oldCurrentTab = tabsFragment.CurrentTab;
                tabsFragment.MoveToTab(moveToTabHint.Tab);
                if (moveToTabHint.PopToRoot)
                {
                    tabsFragment.CloseFragments(true, tabsFragment.CurrentTab);
                    if (oldCurrentTab != tabsFragment.CurrentTab)
                        tabsFragment.CloseFragments(true, oldCurrentTab);
                }
                return true;
            }
            return false;
        }
        return await base.ChangePresentation(hint);
    }

    protected override void PerformShowFragmentTransaction(
        FragmentManager fragmentManager,
        MvxFragmentPresentationAttribute attribute,
        MvxViewModelRequest request)
    {
//      ValidateArguments(attribute, request);

        if (fragmentManager == null)
            throw new ArgumentNullException(nameof(fragmentManager));

        var fragmentName = attribute.Tag ?? attribute.ViewType.FragmentJavaName();

        IMvxFragmentView? fragmentView = null;
        if (attribute.IsCacheableFragment)
        {
            fragmentView = (IMvxFragmentView?)fragmentManager.FindFragmentByTag(fragmentName);
        }

        if (fragmentView == null && attribute.ViewType != null)
            fragmentView = CreateFragment(fragmentManager, attribute, attribute.ViewType);

        var fragment = fragmentView?.ToFragment();
        if (fragment == null)
            throw new MvxException($"Fragment {fragmentName} is null. Cannot perform Fragment Transaction.");

        // MvxNavigationService provides an already instantiated ViewModel here
        if (request is MvxViewModelInstanceRequest instanceRequest)
        {
            fragmentView!.ViewModel = instanceRequest.ViewModelInstance;
        }

        // save MvxViewModelRequest in the Fragment's Arguments
#pragma warning disable CA2000 // Dispose objects before losing scope
        var bundle = new Bundle();
#pragma warning restore CA2000 // Dispose objects before losing scope
        var serializedRequest = NavigationSerializer?.Serializer.SerializeObject(request);
        if (!string.IsNullOrEmpty(serializedRequest))
            bundle.PutString(ViewModelRequestBundleKey, serializedRequest);

        if (fragment.Arguments == null)
        {
            fragment.Arguments = bundle;
        }
        else
        {
            fragment.Arguments.Clear();
            fragment.Arguments.PutAll(bundle);
        }

        var ft = fragmentManager.BeginTransaction();

        OnBeforeFragmentChanging(ft, fragment, attribute, request);

        if (attribute.AddToBackStack)
            ft.AddToBackStack(fragmentName);

        OnFragmentChanging(ft, fragment, attribute, request);

        if (attribute.AddFragment && fragment.IsAdded)
        {
            ft.Show(fragment);
        }
        else if (PreferShowHideOverReplace || attribute.AddFragment)
        {
            if (PreferShowHideOverReplace && fragmentManager.Fragments?.Count > 0)
	            ft.Hide(fragmentManager.Fragments[^1]);
            ft.Add(attribute.FragmentContentId, fragment, fragmentName);
        }
        else
        {
            ft.Replace(attribute.FragmentContentId, fragment, fragmentName);
        }

        ft.CommitAllowingStateLoss();

        OnFragmentChanged(ft, fragment, attribute, request);
    }

    protected override bool TryPerformCloseFragmentTransaction(
        FragmentManager fragmentManager,
        MvxFragmentPresentationAttribute fragmentAttribute)
    {
//	    ValidateArguments(fragmentAttribute);

        if (fragmentManager == null)
	        throw new ArgumentNullException(nameof(fragmentManager));

        try
        {
	        var fragmentName = fragmentAttribute.Tag ?? fragmentAttribute.ViewType.FragmentJavaName();
	        if (fragmentManager.BackStackEntryCount > 0)
	        {
//			    PopOnBackstackEntries(fragmentName, fragmentManager, fragmentAttribute);
		        return base.TryPerformCloseFragmentTransaction(fragmentManager, fragmentAttribute);
	        }

	        Fragment? fragmentToPop = fragmentManager.FindFragmentByTag(fragmentName);
	        if (fragmentToPop != null)
	        {
		        PopFragment(fragmentManager, fragmentAttribute, fragmentToPop);
		        return true;
	        }
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (System.Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
        {
	        return false;
        }

        return false;
    }

    private void PopFragment(FragmentManager fragmentManager, MvxFragmentPresentationAttribute fragmentAttribute,
        Fragment fragmentToPop)
    {
        var ft = fragmentManager.BeginTransaction();

        if (!fragmentAttribute.EnterAnimation.Equals(int.MinValue) &&
            !fragmentAttribute.ExitAnimation.Equals(int.MinValue))
        {
	        if (!fragmentAttribute.PopEnterAnimation.Equals(int.MinValue) &&
	            !fragmentAttribute.PopExitAnimation.Equals(int.MinValue))
	        {
		        ft.SetCustomAnimations(
			        fragmentAttribute.EnterAnimation,
			        fragmentAttribute.ExitAnimation,
			        fragmentAttribute.PopEnterAnimation,
			        fragmentAttribute.PopExitAnimation);
	        }
	        else
	        {
		        ft.SetCustomAnimations(
			        fragmentAttribute.EnterAnimation,
			        fragmentAttribute.ExitAnimation);
	        }
        }

        if (fragmentAttribute.TransitionStyle != int.MinValue)
	        ft.SetTransitionStyle(fragmentAttribute.TransitionStyle);

        ft.Remove(fragmentToPop);
        if (PreferShowHideOverReplace && fragmentManager.Fragments?.Count > 1)
	        ft.Show(fragmentManager.Fragments[^2]);
        ft.CommitAllowingStateLoss();

        OnFragmentPopped(ft, fragmentToPop, fragmentAttribute);
    }

    private class WrapperTabViewModel : MvxViewModel
    {
        public Action<WrapperTabFragment> ShowActualViewModel { get; set; }

        public WrapperTabViewModel()
        {
        }

        public WrapperTabViewModel(Action<WrapperTabFragment> showActualViewModel)
        {
            ShowActualViewModel = showActualViewModel;
        }
    }

    private class WrapperTabFragment : MvxFragment<WrapperTabViewModel>, ITabFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.EnsureBindingContextIsSet(inflater);
            var view = this.BindingInflate(Resource.Layout.tab_page, null);

            if (ViewModel.ShowActualViewModel != null)
            {
                ViewModel.ShowActualViewModel(this);
                ViewModel.ShowActualViewModel = null;
            }

            return view;
        }
    }
}
