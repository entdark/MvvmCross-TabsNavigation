using MvvmCross.Platforms.Android.Presenters.Attributes;
using TabsNavigation.Android.Views;

namespace TabsNavigation.Android.Presenter.Attributes;

public class PushFragmentPresentationAttribute : MvxFragmentPresentationAttribute
{
    public PushFragmentPresentationAttribute() : this(
        typeof(MainActivityViewModel),
        Resource.Id.content,
        true
    )
    { }

    public PushFragmentPresentationAttribute(
        Type activityHostViewModelType = null,
        int fragmentContentId = global::Android.Resource.Id.Content,
        bool addToBackStack = false,
        int enterAnimation = int.MinValue,
        int exitAnimation = int.MinValue,
        int popEnterAnimation = int.MinValue,
        int popExitAnimation = int.MinValue,
        int transitionStyle = int.MinValue,
        Type fragmentHostViewType = null,
        Type tabsFramentHostViewType = null,
        bool isCacheableFragment = false,
        string tag = null,
        string popBackStackImmediateName = "",
        MvxPopBackStack popBackStackImmediateFlag = MvxPopBackStack.Inclusive,
        bool addFragment = false) : base(activityHostViewModelType, fragmentContentId, addToBackStack,
            enterAnimation, exitAnimation, popEnterAnimation, popExitAnimation,
            transitionStyle, fragmentHostViewType, isCacheableFragment, tag,
            popBackStackImmediateName, popBackStackImmediateFlag, addFragment)
    {
        TabsFragmentHostViewType = tabsFramentHostViewType;
    }

    public PushFragmentPresentationAttribute(
        Type activityHostViewModelType = null,
        string fragmentContentResourceName = null,
        bool addToBackStack = false,
        string enterAnimation = null,
        string exitAnimation = null,
        string popEnterAnimation = null,
        string popExitAnimation = null,
        string transitionStyle = null,
        Type fragmentHostViewType = null,
        Type tabsFramentHostViewType = null,
        bool isCacheableFragment = false,
        string tag = null,
        string popBackStackImmediateName = "",
        MvxPopBackStack popBackStackImmediateFlag = MvxPopBackStack.Inclusive,
        bool addFragment = false) : base(activityHostViewModelType, fragmentContentResourceName, addToBackStack,
            enterAnimation, exitAnimation, popEnterAnimation, popExitAnimation,
            transitionStyle, fragmentHostViewType, isCacheableFragment, tag,
            popBackStackImmediateName, popBackStackImmediateFlag, addFragment)
    {
        TabsFragmentHostViewType = tabsFramentHostViewType;
    }

    /// <summary>
    /// Fragment parent View Type. When set ChildFragmentManager of this Fragment will be used
    /// </summary>
    public Type TabsFragmentHostViewType { get; set; }
}
