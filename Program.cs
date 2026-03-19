using System.Runtime.InteropServices;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

var cts = new CancellationTokenSource();

ConsoleCtrlHandler ctrlHandler = _ =>
{
    Cleanup();
    return false;
};

// Register handler for: Alt+F4, window X button, logoff, shutdown for Cleanup to run
NativeMethods.SetConsoleCtrlHandler(ctrlHandler, true);
RegisterAppForToasts();

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Handle Ctrl+C — signal cancellation
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║         HourlyGainz 💪               ║");
Console.WriteLine("╚══════════════════════════════════════╝");
Console.ResetColor();
Console.WriteLine();

int totalMinutes = 0;

// Setup loop for valid reminder interval input
while (true)
{
    Console.Write("⏱ Enter reminder interval (in minutes): ");
    if (int.TryParse(Console.ReadLine(), out totalMinutes) && totalMinutes > 0)
    {
        // Debugging
        if (totalMinutes == 6969)
        {
            SendToastNotification("test", 1);
            Cleanup();
            return;
        }

        break;
    }

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("  Please enter a valid positive number.");
    Console.ResetColor();
}

string activity = "";

while (true)
{
    Console.Write("🏃 Enter the activity to be reminded of: ");
    activity = Console.ReadLine()?.Trim() ?? "";

    if (!string.IsNullOrWhiteSpace(activity))
        break;

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("  Activity cannot be empty.");
    Console.ResetColor();
}

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"✅ Reminder set! Every {totalMinutes} min → \"{activity}\"\n");
Console.WriteLine("Press Ctrl+C, Alt+F4, or close the window to stop.\n");
Console.ResetColor();
Console.WriteLine();

int reminderCount = 0;
var interval = TimeSpan.FromMinutes(totalMinutes);

try
{
    while (!cts.Token.IsCancellationRequested)
    {
        var targetTime = DateTime.Now.Add(interval);

        while (!cts.Token.IsCancellationRequested && DateTime.Now < targetTime)
        {
            var remaining = targetTime - DateTime.Now;
            Console.Write($"\r⏳ Next reminder in: {remaining:hh\\:mm\\:ss}");
            Thread.Sleep(1000);
        }

        if (cts.Token.IsCancellationRequested) break;

        reminderCount++;

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"\n🔔 [{DateTime.Now:HH:mm:ss}] Reminder #{reminderCount}: Time to \"{activity}\"!");
        Console.ResetColor();

        SendToastNotification(activity, reminderCount);

        Console.WriteLine();
    }
}
finally
{
    Cleanup(reminderCount);
}

void Cleanup(int _reminderCount = 0)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n👋 HourlyGainz stopped. {(_reminderCount > 0 ? "Great work today!" : "")}");
    Console.ResetColor();

    if (!cts.IsCancellationRequested) cts.Cancel();
    cts.Dispose();
}

void RegisterAppForToasts()
{
    string exeDir = AppContext.BaseDirectory;
    string iconPath = Path.Combine(exeDir, "icon.ico");

    string regPath = @"SOFTWARE\Classes\AppUserModelId\HourlyGainz";
    using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(regPath);
    key.SetValue("DisplayName", "HourlyGainz");
}

void SendToastNotification(string act, int count)
{
    try
    {
        string xml = $"""
            <toast>
              <visual>
                <binding template="ToastGeneric">
                  <text>💪 HourlyGainz — Reminder #{count}</text>
                  <text>Time to do: {System.Security.SecurityElement.Escape(act)}</text>
                  <text>Get up and crush it!</text>
                </binding>
              </visual>
            </toast>
            """;

        var doc = new XmlDocument();
        doc.LoadXml(xml);

        var toast = new ToastNotification(doc);
        ToastNotificationManager.CreateToastNotifier("HourlyGainz").Show(toast);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚠ Could not send toast notification: {ex.Message}");
        Console.ResetColor();
    }
}

delegate bool ConsoleCtrlHandler(int sig);

static class NativeMethods
{
    [DllImport("Kernel32")]
    public static extern bool SetConsoleCtrlHandler(ConsoleCtrlHandler handler, bool add);
}