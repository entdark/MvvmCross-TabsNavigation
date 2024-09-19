namespace TabsNavigation.Android.Callbacks;

public class OnBackPressedCallback : AndroidX.Activity.OnBackPressedCallback
{
    private readonly Action onBackPressed;

    public OnBackPressedCallback(Action onBackPressed, bool enabled = true) : base(enabled)
    {
        this.onBackPressed = onBackPressed;
    }

    public override void HandleOnBackPressed()
    {
        onBackPressed?.Invoke();
    }
}
