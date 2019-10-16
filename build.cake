//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDirAndroidXBiometric = Directory("./AndroidX.Biometric/bin") + Directory(configuration);
var buildDirXamarinAndroidGoldfinger = Directory("./XamarinAndroid.Goldfinger/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDirAndroidXBiometric);
	CleanDirectory(buildDirXamarinAndroidGoldfinger);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./XamarinAndroid.Goldfinger.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./XamarinAndroid.Goldfinger.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./XamarinAndroid.Goldfinger.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>
{
	MSBuild("./XamarinAndroid.Goldfinger/XamarinAndroid.Goldfinger.csproj", settings =>
        {
			settings.SetConfiguration(configuration);
			settings.WithTarget("pack");
		});

});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
