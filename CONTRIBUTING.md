# Contributing

Contributions are welcome. For media catalog changes, include the direct media URL, source page, creator or performer, recording license, and license URL. Confirm the license applies to the **recording**, not only to the underlying composition.

Before opening a pull request, run:

```powershell
dotnet test tests/OpenSleepMusic.Core.Tests/OpenSleepMusic.Core.Tests.csproj --configuration Release
```

Never commit downloaded audio, local logs, signing keys, certificates, or store credentials.
