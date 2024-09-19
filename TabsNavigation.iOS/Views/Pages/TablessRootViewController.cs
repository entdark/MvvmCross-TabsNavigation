using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using TabsNavigation.Core.ViewModels.Pages;
using TabsNavigation.iOS.Views.Base;

namespace TabsNavigation.iOS.Views.Pages;

[MvxRootPresentation(WrapInNavigationController = true)]
public partial class TablessRootViewController : BaseViewController<TablessRootViewModel>
{
    public TablessRootViewController() : base(nameof(TablessRootViewController), null)
    {
    }

    protected override void BindView(MvxFluentBindingDescriptionSet<IMvxIosView<TablessRootViewModel>, TablessRootViewModel> set)
    {
        base.BindView(set);
        set.Bind(GoDeeperButton).To(vm => vm.GoToOuterPageCommand);
        set.Bind(NavigationButton).To(vm => vm.GoToTabsCommand);
    }
}

