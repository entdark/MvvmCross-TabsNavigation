using MvvmCross.ViewModels;

namespace TabsNavigation.Core.Models.Navigation.Hints;

public class MoveToTabPresentationHint : MvxPresentationHint
{
    public bool PopToRoot { get; init; } = true;

    public int Tab => (int)NavigationTab;

    private NavigaionTab NavigationTab { get; init; }

    public MoveToTabPresentationHint(NavigaionTab tab)
    {
        NavigationTab = tab;
    }
}
