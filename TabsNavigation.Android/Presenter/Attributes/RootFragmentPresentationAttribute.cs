using MvvmCross.Platforms.Android.Presenters.Attributes;
using TabsNavigation.Android.Views;

namespace TabsNavigation.Android.Presenter.Attributes;

public class RootFragmentPresentationAttribute : MvxFragmentPresentationAttribute
{
    public RootFragmentPresentationAttribute() : base(
        typeof(MainActivityViewModel),
        Resource.Id.content,
        false
    )
    { }

    public RootFragmentPresentationAttribute(
        Type activityHostViewModelType = null,
        int fragmentContentId = global::Android.Resource.Id.Content,
        bool addToBackStack = false,
        int enterAnimation = int.MinValue,
        int exitAnimation = int.MinValue,
        int popEnterAnimation = int.MinValue,
        int popExitAnimation = int.MinValue,
        int transitionStyle = int.MinValue,
        Type fragmentHostViewType = null,
        bool isCacheableFragment = false,
        string tag = null,
        string popBackStackImmediateName = "",
        MvxPopBackStack popBackStackImmediateFlag = MvxPopBackStack.Inclusive,
        bool addFragment = false) : base(activityHostViewModelType, fragmentContentId, addToBackStack,
            enterAnimation, exitAnimation, popEnterAnimation, popExitAnimation,
            transitionStyle, fragmentHostViewType, isCacheableFragment, tag,
            popBackStackImmediateName, popBackStackImmediateFlag, addFragment)
    {
    }

    public RootFragmentPresentationAttribute(
        Type activityHostViewModelType = null,
        string fragmentContentResourceName = null,
        bool addToBackStack = false,
        string enterAnimation = null,
        string exitAnimation = null,
        string popEnterAnimation = null,
        string popExitAnimation = null,
        string transitionStyle = null,
        Type fragmentHostViewType = null,
        bool isCacheableFragment = false,
        string tag = null,
        string popBackStackImmediateName = "",
        MvxPopBackStack popBackStackImmediateFlag = MvxPopBackStack.Inclusive,
        bool addFragment = false) : base(activityHostViewModelType, fragmentContentResourceName, addToBackStack,
            enterAnimation, exitAnimation, popEnterAnimation, popExitAnimation,
            transitionStyle, fragmentHostViewType, isCacheableFragment, tag,
            popBackStackImmediateName, popBackStackImmediateFlag, addFragment)
    {
    }
}
