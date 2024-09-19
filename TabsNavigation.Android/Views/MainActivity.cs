using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Activity;
using Google.Android.Material.Internal;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;
using TabsNavigation.Android.Views.Base;

namespace TabsNavigation.Android.Views;

[Activity(
   Label = "@string/app_name",
   MainLauncher = true,
   Theme = "@style/AppTheme",
   LaunchMode = LaunchMode.SingleTop,
   ConfigurationChanges = ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden,
   WindowSoftInputMode = SoftInput.StateHidden | SoftInput.AdjustPan
)]
[MvxActivityPresentation]
public class MainActivity : BaseActivity<MainActivityViewModel>
{
    public MainActivity() : base(Resource.Layout.activity_main)
    {
    }

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        EdgeToEdge.Enable(this);
/*        EdgeToEdgeUtils.ApplyEdgeToEdge(this.Window, true);*/
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            this.Window.NavigationBarContrastEnforced = false;
    }
}

public class MainActivityViewModel : MvxViewModel
{
}
