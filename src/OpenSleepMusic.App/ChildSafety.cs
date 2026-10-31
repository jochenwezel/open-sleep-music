using OpenSleepMusic.App.Localization;
using OpenSleepMusic.Core.Privacy;

namespace OpenSleepMusic.App;

internal static class ChildSafety
{
    private const string EnabledKey = "childSafety.enabled";
    private const string PinKey = "childSafety.pin.v1";
    private const string AttemptsKey = "childSafety.attempts";
    private const string RetryKey = "childSafety.retryAfter";
    private const string CollectionsKey = "childSafety.collections";
    private static int _sessionVersion;
    public static ChildModeStartup Startup { get; } = new();

    public static ParentalControl Control { get; } = new(
        Preferences.Default.Get(EnabledKey, false),
        failedAttempts: Preferences.Default.Get(AttemptsKey, 0),
        retryAfter: DateTimeOffset.TryParse(Preferences.Default.Get(RetryKey, ""),
            System.Globalization.CultureInfo.InvariantCulture, out var retry) ? retry : default,
        allowedCollections: Preferences.Default.ContainsKey(CollectionsKey)
            ? Preferences.Default.Get(CollectionsKey, "").Split('\n', StringSplitOptions.RemoveEmptyEntries) : null);

    public static string MenuTitle => !Control.IsEnabled ? AppText.Get("ChildSafetySetup")
        : Control.IsLocked ? AppText.Get("ParentArea") : AppText.Get("ParentAreaUnlocked");

    public static void LeaveApp()
    {
        Interlocked.Increment(ref _sessionVersion);
        Control.Lock();
    }

    public static async Task<bool> RequireParentAsync(Page page)
    {
        if (!Control.IsLocked) return true;
        await page.DisplayAlertAsync(AppText.Get("ChildSafety"), AppText.Get("ParentRequired"), AppText.Get("Close"));
        return false;
    }

    public static async Task OpenAsync(Page page, IReadOnlyList<ChildCollectionOption> collections)
    {
        if (!Control.IsEnabled)
        {
            await page.Navigation.PushModalAsync(new ParentAccessPage(ParentAccessPurpose.Enable, collections));
            return;
        }
        if (Control.IsLocked)
        {
            await page.Navigation.PushModalAsync(new ParentAccessPage(ParentAccessPurpose.Unlock));
            return;
        }
        var action = await page.DisplayActionSheetAsync(AppText.Get("ParentAreaUnlocked"), AppText.Get("Close"), null,
            AppText.Get("EnterChildMode"), AppText.Get("ChooseChildCollections"), AppText.Get("ChangeParentPin"), AppText.Get("DisableChildSafety"));
        // The app can have been backgrounded while the action sheet was open.
        if (Control.IsLocked) return;
        if (action == AppText.Get("EnterChildMode")) Control.Lock();
        else if (action == AppText.Get("ChooseChildCollections"))
            await page.Navigation.PushModalAsync(new ParentAccessPage(ParentAccessPurpose.Collections, collections));
        else if (action == AppText.Get("ChangeParentPin"))
            await page.Navigation.PushModalAsync(new ParentAccessPage(ParentAccessPurpose.ChangePin));
        else if (action == AppText.Get("DisableChildSafety"))
        {
            Preferences.Default.Set(EnabledKey, false);
            Control.Disable();
            SecureStorage.Default.Remove(PinKey);
        }
    }

    public static async Task<string?> SubmitAsync(ParentAccessPurpose purpose, string pin, IReadOnlyList<string> selectedCollections)
    {
        if (purpose != ParentAccessPurpose.Collections && !ParentPin.IsValid(pin)) return AppText.Get("PinFormat");
        try
        {
            if (purpose == ParentAccessPurpose.Enable && Control.IsEnabled) return AppText.Get("ParentRequired");
            if (purpose == ParentAccessPurpose.Collections)
            {
                if (Control.IsLocked) return AppText.Get("ParentRequired");
                SaveCollectionSelection(selectedCollections);
                return null;
            }
            if (purpose == ParentAccessPurpose.Unlock)
            {
                var version = Volatile.Read(ref _sessionVersion);
                var credential = await SecureStorage.Default.GetAsync(PinKey);
                if (version != Volatile.Read(ref _sessionVersion)) return AppText.Get("ParentRequired");
                if (credential is null) return AppText.Get("PinUnavailable");
                var unlocked = Control.TryUnlock(pin, credential);
                Preferences.Default.Set(AttemptsKey, Control.FailedAttempts);
                Preferences.Default.Set(RetryKey, Control.RetryAfter.ToString("O", System.Globalization.CultureInfo.InvariantCulture));
                if (unlocked) return null;
                return Control.RetryIn > TimeSpan.Zero
                    ? AppText.Get("PinWait") : AppText.Get("PinIncorrect");
            }
            if (purpose == ParentAccessPurpose.ChangePin && Control.IsLocked) return AppText.Get("ParentRequired");
            await SecureStorage.Default.SetAsync(PinKey, ParentPin.Create(pin));
            if (purpose == ParentAccessPurpose.Enable)
            {
                SaveCollectionSelection(selectedCollections);
                Preferences.Default.Set(EnabledKey, true);
                Control.Enable();
            }
            return null;
        }
        catch (Exception exception)
        {
            if (purpose == ParentAccessPurpose.Unlock) Control.Lock();
            System.Diagnostics.Debug.WriteLine($"Parent gate storage: {exception.GetType().Name}");
            return AppText.Get("PinStorageError");
        }
    }

    private static void SaveCollectionSelection(IReadOnlyList<string> selectedCollections)
    {
        Preferences.Default.Set(CollectionsKey, string.Join('\n', selectedCollections));
        Control.SetAllowedCollections(selectedCollections);
    }
}

internal sealed record ChildCollectionOption(string Id, string Name, int DownloadedCount);
