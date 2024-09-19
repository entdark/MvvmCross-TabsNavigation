using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using TabsNavigation.Core.Navigation;

namespace TabsNavigation.Core.ViewModels.Base;

public class BaseViewModel : MvxViewModel, IBaseViewModel
{
    protected INavigationService NavigationService { get; init; }

    public virtual IMvxCommand CloseCommand { get; set; }

    public BaseViewModel()
    {
        NavigationService = Mvx.IoCProvider.Resolve<INavigationService>();

        CloseCommand = new MvxAsyncCommand(CloseExecute);
    }

    protected virtual async Task CloseExecute()
    {
        await NavigationService.Close(this);
    }
}

public abstract class BaseViewModel<TParameter> : BaseViewModel, IMvxViewModel<TParameter>
{
    public abstract void Prepare(TParameter parameter);
}
