using System;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using TabsNavigation.Core.ViewModels.Tabs;

namespace TabsNavigation.iOS.Views.Tabs;

[MvxTabPresentation(TabName = "Tab stack", TabIconName = "square.stack.3d.up", TabSelectedIconName = "square.stack.3d.up.fill", WrapInNavigationController = true)]
public class Tab1ViewController : TabViewController<Tab1ViewModel>
{
    public Tab1ViewController() : base("Open page in the TABS stack")
    {

    }

    protected override void BindButton(MvxFluentBindingDescriptionSet<IMvxIosView<Tab1ViewModel>, Tab1ViewModel> set, UIButton button)
    {
        base.BindButton(set, button);
        set.Bind(button).To(vm => vm.GoToInnerPageCommand);
    }
}

[MvxTabPresentation(TabName = "Main stack", TabIconName = "rectangle.stack", TabSelectedIconName = "rectangle.stack.fill", WrapInNavigationController = true)]
public class Tab2ViewController : TabViewController<Tab2ViewModel>
{
    public Tab2ViewController() : base("Open page in the MAIN stack")
    {

    }

    protected override void BindButton(MvxFluentBindingDescriptionSet<IMvxIosView<Tab2ViewModel>, Tab2ViewModel> set, UIButton button)
    {
        base.BindButton(set, button);
        set.Bind(button).To(vm => vm.GoToOuterPageCommand);
    }
}

[MvxTabPresentation(TabName = "Tabless root", TabIconName = "rectangle", TabSelectedIconName = "rectangle.fill", WrapInNavigationController = true)]
public class Tab3ViewController : TabViewController<Tab3ViewModel>
{
    public Tab3ViewController() : base("Open new TABLESS ROOT page")
    {

    }

    protected override void BindButton(MvxFluentBindingDescriptionSet<IMvxIosView<Tab3ViewModel>, Tab3ViewModel> set, UIButton button)
    {
        base.BindButton(set, button);
        set.Bind(button).To(vm => vm.GoToTablessPageCommand);
    }
}