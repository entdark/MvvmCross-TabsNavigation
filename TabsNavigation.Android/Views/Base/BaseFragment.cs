using Android.Views;
using Android.Views.Animations;
using Google.Android.Material.Transition;
using MvvmCross;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views.Fragments;
using MvvmCross.Presenters;
using MvvmCross.Presenters.Attributes;
using MvvmCross.ViewModels;
using TabsNavigation.Android.Callbacks;
using TabsNavigation.Android.Presenter;
using TabsNavigation.Android.Presenter.Attributes;
using TabsNavigation.Android.Views.Main;
using TabsNavigation.Core.Navigation;
using TabsNavigation.Core.ViewModels.Base;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace TabsNavigation.Android.Views.Base;

public abstract class BaseFragment<TViewModel> : MvxFragment<TViewModel>, IBaseFragment, IMvxOverridePresentationAttribute where TViewModel : class, IBaseViewModel
{
    private const string BundleRegisterBackPressedCallback = nameof(BaseFragment<TViewModel>) + nameof(BundleRegisterBackPressedCallback);
    private const string BundleIsHidden = nameof(BaseFragment<TViewModel>) + nameof(BundleIsHidden);

    private OnBackPressedCallback onBackPressedCallback;

    private bool isHidden = false;
    private bool postponeIsHiddenChanged = false;

    protected int LayoutId { get; init; }

    protected Toolbar Toolbar { get; private set; }

    private ITabsNavigationViewPresenter presenter;
    protected ITabsNavigationViewPresenter Presenter => presenter ??= Mvx.IoCProvider.Resolve<ITabsNavigationViewPresenter>();

    public bool RegisterBackPressedCallback { get; set; }

    public override Java.Lang.Object EnterTransition
    {
	    get => isHidden ? base.ExitTransition : base.EnterTransition;
	    set => base.EnterTransition = value;
    }

    public BaseFragment(int layoutId)
    {
        LayoutId = layoutId;
    }

    public override void OnHiddenChanged(bool hidden)
    {
	    base.OnHiddenChanged(hidden);

	    if (Presenter.PreferShowHideOverReplace)
	    {
		    if (hidden)
			    isHidden = hidden;
		    else
			    postponeIsHiddenChanged = true;
	    }
    }

    public override void OnResume()
    {
	    base.OnResume();

	    if (Presenter.PreferShowHideOverReplace)
	    {
		    if (postponeIsHiddenChanged)
		    {
			    isHidden = false;
			    postponeIsHiddenChanged = false;
		    }
	    }
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        this.EnsureBindingContextIsSet(inflater);
        return this.BindingInflate(LayoutId, null);
    }

    public override void OnViewCreated(View view, Bundle savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);

        if (savedInstanceState != null)
            OnRestoreInstanceState(savedInstanceState);

        Toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
        if (Toolbar != null)
        {
            Toolbar.NavigationClick += NavigationClick;
        }

        SetUpNavigation(true);

        if (RegisterBackPressedCallback)
        {
            onBackPressedCallback?.Remove();
            onBackPressedCallback = new OnBackPressedCallback(OnBackPressedCallback);
            Activity.OnBackPressedDispatcher.AddCallback(this, onBackPressedCallback);
        }
    }

    public override void OnDestroyView()
    {
        if (Toolbar != null)
        {
            Toolbar.NavigationClick -= NavigationClick;
        }
        onBackPressedCallback?.Remove();

        base.OnDestroyView();
    }

    public override void OnSaveInstanceState(Bundle outState)
    {
        base.OnSaveInstanceState(outState);
        outState.PutBoolean(BundleRegisterBackPressedCallback, RegisterBackPressedCallback);
        outState.PutBoolean(BundleIsHidden, isHidden);
    }

    public virtual void OnRestoreInstanceState(Bundle savedInstanceState)
    {
        if (savedInstanceState == null)
            return;

        RegisterBackPressedCallback = savedInstanceState.GetBoolean(BundleRegisterBackPressedCallback, RegisterBackPressedCallback);
        isHidden = savedInstanceState.GetBoolean(BundleIsHidden, isHidden);
    }

    public override Animation OnCreateAnimation(int transit, bool enter, int nextAnim)
    {
        if (IBaseFragment.DisableAnimations)
            return new NullAnimation();
        return base.OnCreateAnimation(transit, enter, nextAnim);
    }

    private class NullAnimation : Animation
    {
        public NullAnimation()
        {
            Duration = 0L;
        }
    }

    public virtual bool OnBackPressed()
    {
        return false;
    }

    protected virtual void OnBackPressedCallback()
    {
        if (ParentFragmentManager is { IsStateSaved : false } && ParentFragmentManager.PopBackStackImmediate())
            return;

        Mvx.IoCProvider.Resolve<INavigationService>().Close(ViewModel);
    }

    protected void ToggleBackPressedCallback(bool enable)
    {
        if (onBackPressedCallback != null)
            onBackPressedCallback.Enabled = enable;
    }

    protected virtual void NavigationClick(object sender, Toolbar.NavigationClickEventArgs ev)
    {
        Activity?.OnBackPressedDispatcher?.OnBackPressed();
    }

    protected virtual void SetUpNavigation(bool showUpNavigation)
    {
        if (Toolbar != null)
        {
            Toolbar.NavigationIcon = showUpNavigation ? Context.GetDrawable(Resource.Drawable.abc_ic_ab_back_material) : null;
        }
    }

    public virtual MvxBasePresentationAttribute PresentationAttribute(MvxViewModelRequest request)
    {
        return null;
    }
}

public abstract class BasePushFragment<TViewModel> : BaseFragment<TViewModel> where TViewModel : class, IBaseViewModel
{
	public BasePushFragment(int layoutId) : base(layoutId)
	{
		if (Presenter.PreferShowHideOverReplace)
		{
			RegisterBackPressedCallback = true;
		}
        EnterTransition = new MaterialSharedAxis(MaterialSharedAxis.X, true);
        ExitTransition = new MaterialSharedAxis(MaterialSharedAxis.X, false);
    }

    public override MvxBasePresentationAttribute PresentationAttribute(MvxViewModelRequest request)
    {
        return new PushFragmentPresentationAttribute();
    }
}

public abstract class BaseTabPushFragment<TViewModel> : BasePushFragment<TViewModel> where TViewModel : class, IBaseViewModel
{
    public BaseTabPushFragment(int layoutId) : base(layoutId)
    {
    }

    public override MvxBasePresentationAttribute PresentationAttribute(MvxViewModelRequest request)
    {
        return new PushFragmentPresentationAttribute()
        {
            TabsFragmentHostViewType = typeof(MainFragment)
        };
    }
}

public abstract class BaseRootFragment<TViewModel> : BaseFragment<TViewModel> where TViewModel : class, IBaseViewModel
{
    public BaseRootFragment(int layoutId) : base(layoutId)
    {
        EnterTransition = new MaterialFadeThrough();
        ExitTransition = new MaterialSharedAxis(MaterialSharedAxis.X, false);
    }

    protected override void SetUpNavigation(bool showUpNavigation)
    {
        base.SetUpNavigation(false);
    }

    public override MvxBasePresentationAttribute PresentationAttribute(MvxViewModelRequest request)
    {
        return new RootFragmentPresentationAttribute();
    }
}

public abstract class BaseTabFragment<TViewModel> : BaseFragment<TViewModel> where TViewModel : class, IBaseViewModel
{
    private readonly string tabTitle;
    private readonly int tabIconDrawableResourceId;

    public BaseTabFragment(int layoutId, string tabTitle, int tabIconDrawableResourceId = int.MinValue) : base(layoutId)
    {
        this.tabTitle = tabTitle;
        this.tabIconDrawableResourceId = tabIconDrawableResourceId;

        ExitTransition = new MaterialSharedAxis(MaterialSharedAxis.X, false);
    }

    public override MvxBasePresentationAttribute PresentationAttribute(MvxViewModelRequest request)
    {
        return new TabFragmentPresentationAttribute(tabTitle, tabIconDrawableResourceId);
    }
}
