using System.Runtime.CompilerServices;
using Microsoft.Maui.Storage;

namespace TabsNavigation.Core;

public static class AppSettings
{
    public static bool IsTabsRoot
    {
        get => Get(true);
        set => Set(value);
    }

    private static bool Get(bool defaultValue, [CallerMemberName] string key = "") => Preferences.Get(key, defaultValue);
    private static void Set(bool value, [CallerMemberName] string key = "") => Preferences.Set(key, value);

    private static string Get(string defaultValue, [CallerMemberName] string key = "") => Preferences.Get(key, defaultValue);
    private static void Set(string value, [CallerMemberName] string key = "") => Preferences.Set(key, value);

    private static DateTime Get(DateTime defaultValue, [CallerMemberName] string key = "") => Preferences.Get(key, defaultValue);
    private static void Set(DateTime value, [CallerMemberName] string key = "") => Preferences.Set(key, value);
}
