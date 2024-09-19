using System.Reflection;
using Android.Content;
using Android.Views;
using AndroidX.Core.View;
using Google.Android.Material.Internal;
using Microsoft.Extensions.Logging;
using MvvmCross.Binding;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Binding.Combiners;
using MvvmCross.Converters;
using MvvmCross.IoC;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.Binders;
using MvvmCross.Platforms.Android.Binding.Binders.ViewTypeResolvers;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using Serilog;
using Serilog.Extensions.Logging;
using TabsNavigation.Android.Presenter;
using TabsNavigation.Core;
using TabsNavigation.Core.Navigation;

namespace TabsNavigation.Android;

public class Setup : MvxAndroidSetup<App>
{
    public override IEnumerable<Assembly> GetViewModelAssemblies()
    {
        var assemblies = base.GetViewModelAssemblies();
        var extraAssemblies = new[] { typeof(Setup).Assembly };
        assemblies = assemblies != null ? assemblies.Concat(extraAssemblies) : extraAssemblies;
        return assemblies;
    }

    protected override IMvxAndroidViewPresenter CreateViewPresenter()
    {
        return new TabsNavigationViewPresenter(AndroidViewAssemblies);
    }

    protected override MvxBindingBuilder CreateBindingBuilder()
    {
        return new InsetsAndroidBindingBuilder(FillValueConverters, FillValueCombiners, FillTargetFactories, FillBindingNames, FillViewTypes, FillAxmlViewTypeResolver, FillNamespaceListViewTypeResolver);
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

    protected override ILoggerProvider CreateLogProvider()
    {
        return new SerilogLoggerProvider();
    }

    protected override ILoggerFactory CreateLogFactory()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            // add more sinks here
            .WriteTo.AndroidLog()
            .CreateLogger();

        return new SerilogLoggerFactory();
    }

    private class InsetsAndroidBindingBuilder : MvxAndroidBindingBuilder
    {
        public InsetsAndroidBindingBuilder(Action<IMvxValueConverterRegistry> fillValueConverters, Action<IMvxValueCombinerRegistry> fillValueCombiners, Action<IMvxTargetBindingFactoryRegistry> fillTargetFactories, Action<IMvxBindingNameRegistry> fillBindingNames, Action<IMvxTypeCache<View>> fillViewTypes, Action<IMvxAxmlNameViewTypeResolver> fillAxmlViewTypeResolver, Action<IMvxNamespaceListViewTypeResolver> fillNamespaceListViewTypeResolver)
            : base(fillValueConverters, fillValueCombiners, fillTargetFactories, fillBindingNames, fillViewTypes, fillAxmlViewTypeResolver, fillNamespaceListViewTypeResolver)
        {
        }

        protected override IMvxLayoutInflaterHolderFactoryFactory CreateLayoutInflaterFactoryFactory()
        {
            return new InsetsLayoutInflaterFactoryFactory();
        }

        private class InsetsLayoutInflaterFactoryFactory : IMvxLayoutInflaterHolderFactoryFactory
        {
            public IMvxLayoutInflaterHolderFactory Create(object source)
            {
                return new InsetsBindingLayoutInflaterFactory(source);
            }

            private class InsetsBindingLayoutInflaterFactory : MvxBindingLayoutInflaterFactory
            {
                public InsetsBindingLayoutInflaterFactory(object source) : base(source)
                {
                }

                public override View BindCreatedView(View view, Context context, global::Android.Util.IAttributeSet attrs)
                {
                    using var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.View);
                    int numStyles = typedArray.IndexCount;
                    WindowInsetsFlags flags = WindowInsetsFlags.None;
                    for (var i = 0; i < numStyles; ++i)
                    {
                        var attributeId = typedArray.GetIndex(i);

                        bool attributeValue = typedArray.GetBoolean(attributeId, false);
                        if (attributeValue)
                        {
                            flags |= attributeId switch
                            {
                                Resource.Styleable.View_paddingLeftFitsWindowInsets => WindowInsetsFlags.PaddingLeft,
                                Resource.Styleable.View_paddingRightFitsWindowInsets => WindowInsetsFlags.PaddingRight,
                                Resource.Styleable.View_paddingTopFitsWindowInsets => WindowInsetsFlags.PaddingTop,
                                Resource.Styleable.View_paddingBottomFitsWindowInsets => WindowInsetsFlags.PaddingBottom,

                                Resource.Styleable.View_marginLeftFitsWindowInsets => WindowInsetsFlags.MarginLeft,
                                Resource.Styleable.View_marginRightFitsWindowInsets => WindowInsetsFlags.MarginRight,
                                Resource.Styleable.View_marginTopFitsWindowInsets => WindowInsetsFlags.MarginTop,
                                Resource.Styleable.View_marginBottomFitsWindowInsets => WindowInsetsFlags.MarginBottom,

                                _ => WindowInsetsFlags.None
                            };
                        }
                    }
                    if (flags != WindowInsetsFlags.None)
                        ViewUtils.DoOnApplyWindowInsets(view, new OnApplyWindowInsetsListener(flags));
                    typedArray.Recycle();
                    return base.BindCreatedView(view, context, attrs);
                }
            }
        }

        private class OnApplyWindowInsetsListener : Java.Lang.Object, ViewUtils.IOnApplyWindowInsetsListener
        {
            private readonly WindowInsetsFlags flags;

            private ViewGroup.MarginLayoutParams initialLayoutParameters;

            public OnApplyWindowInsetsListener(WindowInsetsFlags flags)
            {
                this.flags = flags;
            }

            public WindowInsetsCompat OnApplyWindowInsets(View view, WindowInsetsCompat insetsCompat, ViewUtils.RelativePadding initialPadding)
            {
                bool paddingTop = flags.HasFlag(WindowInsetsFlags.PaddingTop);
                bool paddingBottom = flags.HasFlag(WindowInsetsFlags.PaddingBottom);
                bool paddingLeft = flags.HasFlag(WindowInsetsFlags.PaddingLeft);
                bool paddingRight = flags.HasFlag(WindowInsetsFlags.PaddingRight);

                bool isRtl = ViewCompat.GetLayoutDirection(view) == ViewCompat.LayoutDirectionRtl;
                var insets = insetsCompat.GetInsets(WindowInsetsCompat.Type.SystemBars());
                int insetTop = insets.Top;
                int insetBottom = insets.Bottom;
                int insetLeft = insets.Left;
                int insetRight = insets.Right;

                initialPadding.Top += paddingTop ? insetTop : 0;
                initialPadding.Bottom += paddingBottom ? insetBottom : 0;
                int systemWindowInsetLeft = paddingLeft ? insetLeft : 0;
                int systemWindowInsetRight = paddingRight ? insetRight : 0;
                initialPadding.Start += isRtl ? systemWindowInsetRight : systemWindowInsetLeft;
                initialPadding.End += isRtl ? systemWindowInsetLeft : systemWindowInsetRight;
                initialPadding.ApplyToView(view);

                if (view.LayoutParameters is ViewGroup.MarginLayoutParams marginLayoutParameters && initialLayoutParameters == null)
                {
                    initialLayoutParameters = new ViewGroup.MarginLayoutParams(marginLayoutParameters);
                }

                if (initialLayoutParameters != null)
                {
                    bool marginTop = flags.HasFlag(WindowInsetsFlags.MarginTop);
                    bool marginBottom = flags.HasFlag(WindowInsetsFlags.MarginBottom);
                    bool marginLeft = flags.HasFlag(WindowInsetsFlags.MarginLeft);
                    bool marginRight = flags.HasFlag(WindowInsetsFlags.MarginRight);

                    if (marginTop || marginBottom || marginLeft || marginRight)
                    {
                        var newLayoutParameters = view.LayoutParameters as ViewGroup.MarginLayoutParams;
                        newLayoutParameters.TopMargin = initialLayoutParameters.TopMargin + (marginTop ? insetTop : 0);
                        newLayoutParameters.BottomMargin = initialLayoutParameters.BottomMargin + (marginBottom ? insetBottom : 0);
                        newLayoutParameters.LeftMargin = initialLayoutParameters.LeftMargin + (marginLeft ? insetLeft : 0);
                        newLayoutParameters.RightMargin = initialLayoutParameters.RightMargin + (marginRight ? insetRight : 0);
                        view.LayoutParameters = newLayoutParameters;
                    }
                }

                return insetsCompat;
            }
        }

        [Flags]
        private enum WindowInsetsFlags
        {
            None = 0,

            PaddingLeft = 1 << 0,
            PaddingRight = 1 << 1,
            PaddingTop = 1 << 2,
            PaddingBottom = 1 << 3,

            MarginLeft = 1 << 4,
            MarginRight = 1 << 5,
            MarginTop = 1 << 6,
            MarginBottom = 1 << 7
        }
    }
}
