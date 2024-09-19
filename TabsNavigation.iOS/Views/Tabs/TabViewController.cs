using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using TabsNavigation.Core.ViewModels.Base;
using TabsNavigation.iOS.Views.Base;

namespace TabsNavigation.iOS.Views.Tabs;

public abstract partial class TabViewController<TViewModel> : BaseViewController<TViewModel> where TViewModel : class, IBaseViewModel
{
    private readonly string buttonTitle;

    public TabViewController(string buttonTitle) : base("TabViewController", null)
    {
        this.buttonTitle = buttonTitle;
    }

    public override void LoadView()
    {
        base.LoadView();
        NavigationButton.SetTitle(buttonTitle, UIControlState.Normal);
    }

    protected override void BindView(MvxFluentBindingDescriptionSet<IMvxIosView<TViewModel>, TViewModel> set)
    {
        base.BindView(set);
        this.BindButton(set, NavigationButton);
    }

    protected virtual void BindButton(MvxFluentBindingDescriptionSet<IMvxIosView<TViewModel>, TViewModel> set, UIButton button)
    {
    }
}

