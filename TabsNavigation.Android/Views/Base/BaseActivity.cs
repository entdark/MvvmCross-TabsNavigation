using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;

namespace TabsNavigation.Android.Views.Base;

public class BaseActivity<TViewModel> : MvxActivity<TViewModel>, IBaseActivity where TViewModel : class, IMvxViewModel
{
    protected int LayoutId { get; init; }

    public BaseActivity(int layoutId)
    {
        LayoutId = layoutId;
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var view = this.BindingInflate(LayoutId, null);
        SetContentView(view);
    }
}
