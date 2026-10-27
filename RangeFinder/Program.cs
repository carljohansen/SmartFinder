using System.Security.Principal;

namespace RangeFinder;

internal static class Program
{
    private static Mutex? _mutex;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        string userName = WindowsIdentity.GetCurrent().User?.Value ?? "DefaultUser";
        string mutexName = $"CJ.RangeFinder_{userName}";

        _mutex = new Mutex(true, mutexName, out var createdNew);

        if (!createdNew)
        {
            MessageBox.Show("Already running for this user.",
            "RangeFinder", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string? argOpenFileName = null;
        if (args.Length == 1)
        {
            argOpenFileName = args[0];
        }

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new SearchForm() { InitialOpenZipFileName = argOpenFileName });

        _mutex.ReleaseMutex();
    }
}