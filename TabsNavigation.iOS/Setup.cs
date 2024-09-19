using Microsoft.Extensions.Logging;
using MvvmCross.IoC;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using Serilog;
using Serilog.Extensions.Logging;
using TabsNavigation.Core;
using TabsNavigation.Core.Navigation;
using TabsNavigation.iOS.Presenter;

namespace TabsNavigation.iOS;

public class Setup : MvxIosSetup<App>
{
    protected override IMvxIosViewPresenter CreateViewPresenter()
    {
        return new TabsNavigationViewPresenter(ApplicationDelegate, Window);
    }

    protected override IMvxNavigationService CreateNavigationService(IMvxIoCProvider iocProvider)
    {
        iocProvider.LazyConstructAndRegisterSingleton<IMvxNavigationService, IMvxViewModelLoader, IMvxViewDispatcher, IMvxIoCProvider>(
            (loader, dispatcher, iocProvider) => new NavigationService(loader, dispatcher, iocProvider));
        var navigationService = iocProvider.Resolve<IMvxNavigationService>();
        iocProvider.RegisterSingleton(navigationService as INavigationService);
        return navigationService;
    }

    protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
    {
        iocProvider.RegisterSingleton(() => MvxLogHost.Default);

        base.InitializeFirstChance(iocProvider);
    }

    protected override ILoggerFactory CreateLogFactory()
    {
        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    // add more sinks here
                    .WriteTo.NSLog()
                    .CreateLogger();

        return new SerilogLoggerFactory();
    }

    protected override ILoggerProvider CreateLogProvider()
    {
        return new SerilogLoggerProvider();
    }
}
