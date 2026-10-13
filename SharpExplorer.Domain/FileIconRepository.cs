using System.Runtime.InteropServices;

namespace SharpExplorer.Domain;

public static class FileIconRepository
{
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

    [StructLayout(LayoutKind.Sequential)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;

        public IntPtr iIcon;

        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    private const uint SHGFI_ICON = 0x100;
    private const uint SHGFI_LARGEICON = 0x0; // Large icon
    private const uint SHGFI_SMALLICON = 0x1; // 'Small icon

    public static System.Drawing.Icon? GetSmallIcon(string file)
    {

        FileIconRepository.SHFILEINFO shinfo = new FileIconRepository.SHFILEINFO();

        IntPtr hImgLarge = FileIconRepository.SHGetFileInfo(file, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), FileIconRepository.SHGFI_ICON | FileIconRepository.SHGFI_SMALLICON);

        if (hImgLarge == IntPtr.Zero)
        {
            return null;
        }

        return System.Drawing.Icon.FromHandle(shinfo.hIcon);
    }
}