namespace TabsNavigation.Android.Views.Base;

public interface IBaseFragment
{
    static bool DisableAnimations { get; set; }

    bool RegisterBackPressedCallback { get; set; }

    bool OnBackPressed();
}
