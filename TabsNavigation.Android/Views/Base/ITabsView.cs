namespace TabsNavigation.Android.Views.Base;

public interface ITabsView
{
    void CloseFragments(bool animated, int tab = -1);

    AndroidX.Fragment.App.Fragment CurrentTabFragment { get; }

    int CurrentTab { get; }

    void MoveToTab(int tab);
}
