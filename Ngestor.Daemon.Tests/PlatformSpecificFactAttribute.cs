using System.Runtime.InteropServices;

public class MacOsSpecificFactAttribute : FactAttribute
{
    public MacOsSpecificFactAttribute() : base()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Skip = "Test is specific to MacOS";
        }
    }
}

public class WindowsSpecificFactAttribute : FactAttribute
{
    public WindowsSpecificFactAttribute() : base()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Skip = "Test is specific to Windows";
        }
    }
}