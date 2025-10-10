

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShutdownController.Services;

public static class ShutdownOptionInvoker
{

	[DllImport("PowrProf.dll", SetLastError = true)]
	private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);


	public static void Shutdown()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			Process.Start("shutdown", "/s /t 0");
			return;
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			Process.Start("shutdown", "-h now");
			return;
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			Process.Start("shutdown", "-h now");
			return;
		}

		throw new NotImplementedException();
	}

	public static void Restart()
	{
		
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			Process.Start("shutdown", "/r /t 0");
			return;
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			Process.Start("shutdown", "-r now");
			return;
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			Process.Start("shutdown", "-r now");
			return;
		}

		throw new NotImplementedException();
	}

	public static void Sleep()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			SetSuspendState(false, true, true);
			return;
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			Process.Start("pmset", "sleepnow");
			return;
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			Process.Start("systemctl", "suspend");
			return;
		}

		throw new NotImplementedException();
	}
}
