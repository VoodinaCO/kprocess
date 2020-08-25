using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

[assembly: KProcess.Presentation.Windows.DesignTime.DesignTimeBootstrapper]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page, 
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page, 
                                              // app, or any theme specific resource dictionaries)
)]

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("KProcess.Ksmed.Presentation.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b9c62ac49ab4e1b49fafae846dbaec7d23358dd37bcc80ef31ae303d0fc981578d1af9d03cf36198943c5adf1a49633507ea325ff60745ecd90d0587146c3cd6682749f4e578c904916bc08fb500fb0d697a924ec32301e3220425248a0e62865c43338febde6e104f897e265b93580c6b74a810d24a4a3f6b1f382c8ab626b7")]

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DirectShowInternalClockChecker, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b9c62ac49ab4e1b49fafae846dbaec7d23358dd37bcc80ef31ae303d0fc981578d1af9d03cf36198943c5adf1a49633507ea325ff60745ecd90d0587146c3cd6682749f4e578c904916bc08fb500fb0d697a924ec32301e3220425248a0e62865c43338febde6e104f897e265b93580c6b74a810d24a4a3f6b1f382c8ab626b7")]