// Test-only storage surface for exercising the actual AppStateStore without a
// platform host. Production compiles that file against MAUI's real IPreferences.
namespace Microsoft.Maui.Storage;

public interface IPreferences
{
    bool ContainsKey(string key);
    void Remove(string key);
    T Get<T>(string key, T defaultValue);
    void Set<T>(string key, T value);
}

public static class Preferences
{
    public static IPreferences Default => throw new InvalidOperationException("Tests must supply their own storage.");
}
