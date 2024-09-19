namespace TabsNavigation.iOS;

[Preserve(AllMembers = true)]
public class LinkerPleaseInclude
{
    public void Include(UILabel label)
    {
        label.AttributedText = new NSAttributedString(label.Text);
    }
}
