using MvvmCross.ViewModels;

namespace TabsNavigation.Core.Models.Navigation.Hints;

public class MoveToTabPresentationHint : MvxPresentationHint
{
    public bool PopToRoot { get; init; } = true;

    public int Tab => (int)NavigationTab;

    private NavigationTab NavigationTab { get; init; }

    public MoveToTabPresentationHint(NavigationTab tab)
    {
        NavigationTab = tab;
    }
}
