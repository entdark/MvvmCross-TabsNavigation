using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Presenters;
using MvvmCross.Presenters.Attributes;
using MvvmCross.ViewModels;
using TabsNavigation.Core.ViewModels.Base;

namespace TabsNavigation.iOS.Views.Base;

public abstract class BaseViewController<TViewModel> : MvxViewController<TViewModel>, IMvxOverridePresentationAttribute where TViewModel : class, IBaseViewModel
{
    protected BaseViewController() { }
    protected BaseViewController(IntPtr handle) : base(handle) { }
    protected BaseViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        using var set = this.CreateBindingSet();
        BindView(set);
    }

    protected virtual void BindView(MvxFluentBindingDescriptionSet<IMvxIosView<TViewModel>, TViewModel> set)
    {
    }

    public virtual MvxBasePresentationAttribute PresentationAttribute(MvxViewModelRequest request)
    {
        return null;
    }
}
