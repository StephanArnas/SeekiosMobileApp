using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;

// Information about this assembly is defined by the following attributes.
// Change them to the values specific to your project.

[assembly: AssemblyTitle("SeekiosApp.Droid")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Seekios")]
[assembly: AssemblyProduct("Seekios")]
[assembly: AssemblyCopyright("Things Of Tomorrow")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion("1.0.0")]

#if DEBUG
[assembly: Application(Debuggable=true)]
#else
[assembly: Application(Debuggable = false)]
#endif

// The following attributes are used to specify the signing key for the assembly,
// if desired. See the Mono documentation for more information about signing.

//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]

//Cross Connectivity Plugin
//The ACCESS_NETWORK_STATE and ACCESS_WIFI_STATE permissions are required and will be automatically added to your Android Manifest.
//By adding these permissions Google Play will automatically filter out devices without specific hardware.You can get around this by adding the following to your AssemblyInfo.cs file in your Android project:
[assembly: UsesFeature("android.hardware.wifi", Required = false)]
[assembly: UsesPermission("com.android.vending.BILLING")]

