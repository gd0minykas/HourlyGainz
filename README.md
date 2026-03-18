# 💪 HourlyGainz — C# Console App (.NET 10)

A lightweight Windows console app that reminds you to complete an activity
(e.g. push-ups, stretching) on a repeating timer using Windows Toast Notifications.

---

## ✅ Requirements

- Windows 10 / 11
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

---

## 🚀 How to Run

1. Open a terminal in the `HourlyGainz` folder.

2. Restore dependencies:
   ```
   dotnet restore HourlyGainz.csproj
   ```

3. Run the app:
   ```
   dotnet run --project HourlyGainz.csproj
   ```

4. Follow the prompts:
   ```
   ⏱  Enter reminder interval (in minutes): 60
   🏃 Enter the activity to be reminded of: Do 20 push-ups
   ```

5. The app counts down and fires a Windows Toast Notification when the timer expires,
   then automatically repeats on the same interval.

6. Stop the app with **Ctrl+C**, **Alt+F4**, or closing the window.

---

## 📦 Build a Standalone Executable

```
dotnet publish HourlyGainz.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

The `.exe` will be in:
```
bin\Release\net10.0-windows10.0.17763.0\win-x64\publish\HourlyGainz.exe
```

---

## 📋 Notes

- Toast Notifications require Windows 10 version 1607 or later.
- The app must be run on Windows (uses Windows-specific notification APIs).
- Windows SmartScreen may warn on first run — click "More info" → "Run anyway".
