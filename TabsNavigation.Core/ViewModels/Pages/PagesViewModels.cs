using MvvmCross.Commands;
using MvvmCross.ViewModels;
using TabsNavigation.Core.ViewModels.Base;
using TabsNavigation.Core.ViewModels.Main;

namespace TabsNavigation.Core.ViewModels.Pages;

public class InnerPageViewModel : BaseViewModel<int>
{
    public IMvxCommand GoToInnerPageCommand { get; init; }

    private int depth;
    public int Depth
    {
        get => depth;
        set => SetProperty(ref depth, value);
    }

    public InnerPageViewModel()
    {
        GoToInnerPageCommand = new MvxAsyncCommand(async () => await NavigationService.Navigate<InnerPageViewModel, int>(Depth+1));
    }

    public override void Prepare()
    {
        base.Prepare();

        Depth = 1;
    }

    public override void Prepare(int parameter)
    {
        Depth = parameter;
    }

    protected override void SaveStateToBundle(IMvxBundle bundle)
    {
        base.SaveStateToBundle(bundle);
        bundle.Data[nameof(Depth)] = Depth.ToString();
    }

    protected override void ReloadFromBundle(IMvxBundle state)
    {
        base.ReloadFromBundle(state);
        if (state.Data.TryGetValue(nameof(Depth), out string ds))
            _ = int.TryParse(ds, out depth);
    }
}

public class OuterPageViewModel : InnerPageViewModel
{
    public IMvxCommand GoToOuterPageCommand { get; init; }

    public OuterPageViewModel()
    {
        GoToOuterPageCommand = new MvxAsyncCommand(async () => await NavigationService.Navigate<OuterPageViewModel, int>(Depth+1));
    }
}

public class TablessRootViewModel : BaseViewModel
{
    public IMvxCommand GoToTabsCommand { get; init; }
    public IMvxCommand GoToOuterPageCommand { get; init; }

    public TablessRootViewModel()
    {
        GoToTabsCommand = new MvxAsyncCommand(async () => await NavigationService.Navigate<MainViewModel>());
        GoToOuterPageCommand = new MvxAsyncCommand(async () => await NavigationService.Navigate<OuterPageViewModel, int>(1));
    }

    public override Task Initialize()
    {
        //save so on the next load it shows tabless page
        AppSettings.IsTabsRoot = false;
        return base.Initialize();
    }
}