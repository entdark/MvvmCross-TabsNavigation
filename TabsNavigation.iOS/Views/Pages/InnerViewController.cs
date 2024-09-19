using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using TabsNavigation.Core.ViewModels.Pages;
using TabsNavigation.iOS.Views.Base;

namespace TabsNavigation.iOS.Views.Pages;

public class InnerViewController : BaseInnerViewController<InnerPageViewModel>
{
}

public partial class BaseInnerViewController<TViewModel> : BaseViewController<TViewModel> where TViewModel : InnerPageViewModel
{
    public BaseInnerViewController() : base(nameof(InnerViewController), null)
    {
    }

    protected override void BindView(MvxFluentBindingDescriptionSet<IMvxIosView<TViewModel>, TViewModel> set)
    {
        base.BindView(set);
        set.Bind(GoBackButton).To(vm => vm.CloseCommand);
        BindStackLabel(set, StackLabel);
        BindDeeperButton(set, GoDeeperButton);
    }

    protected virtual void BindStackLabel(MvxFluentBindingDescriptionSet<IMvxIosView<TViewModel>, TViewModel> set, UILabel stackLabel)
    {
        set.Bind(stackLabel).For(v => v.Text).To("Format('Tab stack depth: {0}', Depth)");
    }

    protected virtual void BindDeeperButton(MvxFluentBindingDescriptionSet<IMvxIosView<TViewModel>, TViewModel> set, UIButton button)
    {
        set.Bind(button).To(vm => vm.GoToInnerPageCommand);
    }
}

public class OuterViewController : BaseInnerViewController<OuterPageViewModel>
{
    public OuterViewController()
    {
        HidesBottomBarWhenPushed = true;
    }

    protected override void BindStackLabel(MvxFluentBindingDescriptionSet<IMvxIosView<OuterPageViewModel>, OuterPageViewModel> set, UILabel stackLabel)
    {
        set.Bind(stackLabel).For(v => v.Text).To("Format('Main stack depth: {0}', Depth)");
    }

    protected override void BindDeeperButton(MvxFluentBindingDescriptionSet<IMvxIosView<OuterPageViewModel>, OuterPageViewModel> set, UIButton button)
    {
        set.Bind(button).To(vm => vm.GoToOuterPageCommand);
    }
}