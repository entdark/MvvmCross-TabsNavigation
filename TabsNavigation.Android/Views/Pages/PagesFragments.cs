using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Presenters.Attributes;
using MvvmCross.ViewModels;
using TabsNavigation.Android.Views.Base;
using TabsNavigation.Core.ViewModels.Pages;

namespace TabsNavigation.Android.Views.Pages;

public class InnerFragment : BaseTabPushFragment<InnerPageViewModel>
{
    public InnerFragment() : base(Resource.Layout.inner_page)
    {
    }

    //prevent closing the whole stack by giving unique back stack tags when closing by ViewModel
    //when the same Fragments of that ViewModel is added to the stack in a row
    public override MvxBasePresentationAttribute PresentationAttribute(MvxViewModelRequest request)
    {
        var attribute = base.PresentationAttribute(request);
        if (attribute is MvxFragmentPresentationAttribute fragmentAttribute
            && request is MvxViewModelInstanceRequest { ViewModelInstance: InnerPageViewModel { } viewModel })
        {
            fragmentAttribute.Tag = nameof(InnerFragment) + viewModel.Depth;
        }
        return attribute;
    }
}

public class OuterFragment : BasePushFragment<OuterPageViewModel>
{
    public OuterFragment() : base(Resource.Layout.outer_page)
    {
    }

    //prevent closing the whole stack by giving unique back stack tags when closing by ViewModel
    //when the same Fragments of that ViewModel is added to the stack in a row
    public override MvxBasePresentationAttribute PresentationAttribute(MvxViewModelRequest request)
    {
        var attribute = base.PresentationAttribute(request);
        if (attribute is MvxFragmentPresentationAttribute fragmentAttribute
            && request is MvxViewModelInstanceRequest { ViewModelInstance: OuterPageViewModel { } viewModel })
        {
            fragmentAttribute.Tag = nameof(OuterFragment) + viewModel.Depth;
        }
        return attribute;
    }
}

public class TablessRootFragment : BaseRootFragment<TablessRootViewModel>
{
    public TablessRootFragment() : base(Resource.Layout.tabless_root_page)
    {
    }
}