using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using log4net;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("FaceDominator")]
[assembly: AssemblyDescription("Facebook Marketing Software")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("FaceDominator LLC")]
[assembly: AssemblyProduct("FaceDominator")]
[assembly: AssemblyCopyright("Copyright © FaceDominator LLC 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("eb4003cf-0d3c-4b59-a08e-31007b40f131")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.0.9")]
[assembly: AssemblyFileVersion("2.0.0.9")]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.config", Watch = true)]
