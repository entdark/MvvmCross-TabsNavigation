﻿using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using TabsNavigation.Android.Views.Main;

namespace TabsNavigation.Android.Presenter.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TabFragmentPresentationAttribute : MvxViewPagerFragmentPresentationAttribute
{
    public TabFragmentPresentationAttribute(string title, int iconDrawableResourceId) : this(
        title,
        Resource.Id.tabs_viewpager,
        Resource.Id.tabs_navigationview,
        iconDrawableResourceId,
        fragmentHostViewType: typeof(MainFragment)
    )
    { }

    public TabFragmentPresentationAttribute(
        string title,
        int viewPagerResourceId,
        int bottomNavigationViewResourceId,
        int iconDrawableResourceId = int.MinValue,
        Type activityHostViewModelType = null,
        bool addToBackStack = false,
        Type fragmentHostViewType = null,
        bool isCacheableFragment = false) : base(title, viewPagerResourceId, activityHostViewModelType,
            addToBackStack, fragmentHostViewType, isCacheableFragment)
    {
        BottomNavigationViewResourceId = bottomNavigationViewResourceId;
        IconDrawableResourceId = iconDrawableResourceId;
    }

    public TabFragmentPresentationAttribute(
        string title,
        string viewPagerResourceName,
        string bottomNavigationViewResourceName,
        string iconDrawableResourceName = null,
        Type activityHostViewModelType = null,
        bool addToBackStack = false,
        Type fragmentHostViewType = null,
        bool isCacheableFragment = false) : base(title, viewPagerResourceName, activityHostViewModelType,
            addToBackStack, fragmentHostViewType, isCacheableFragment)
    {
        var context = Mvx.IoCProvider.Resolve<IMvxAndroidGlobals>().ApplicationContext;

        BottomNavigationViewResourceId = context.Resources!.GetIdentifier(bottomNavigationViewResourceName, "id", context.PackageName);
        IconDrawableResourceId = context.Resources!.GetIdentifier(iconDrawableResourceName, "id", context.PackageName);
    }

    /// <summary>
    /// The resource id used to get the BottomNavigationView from the view
    /// </summary>
    public int BottomNavigationViewResourceId { get; set; }

    /// <summary>
    /// The resource id used to get the menu item from the view
    /// </summary>
    public int IconDrawableResourceId { get; set; }
}
