using Android.Runtime;
using Android.Views;

namespace TabsNavigation.Android;

[Preserve(AllMembers = true)]
public class LinkerPleaseInclude
{
    public void Include(TextView tv)
    {
        tv.TextChanged += (sender, args) => tv.Text = "" + tv.Text;
    }

    public void Include(View view)
    {
        view.Click += (sender, ev) => view.ContentDescription += "";
    }
}
