//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var verbosity = Argument<Verbosity>("verbosity", Verbosity.Minimal);

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var baseName = "LocaltunnelClient";
var buildDir = Directory("./build") + Directory(configuration);
var dotNetSdkProjectFile = $"./{baseName}.sln";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

MSBuildSettings SetDefaultMSBuildSettings(MSBuildSettings msBuildSettings) {
	DirectoryPath vsLocation = VSWhereLatest( new VSWhereLatestSettings {
		Requires = "Microsoft.Component.MSBuild"
	});
	
	if (vsLocation == null) {
		msBuildSettings.ToolVersion = MSBuildToolVersion.Default;
	} else {
		// Reference: http://cakebuild.net/blog/2017/03/vswhere-and-visual-studio-2017-support
		FilePath msBuildPathX64 = vsLocation.CombineWithFilePath("./MSBuild/15.0/Bin/amd64/MSBuild.exe");
		msBuildSettings.ToolPath = msBuildPathX64;
	}
	
	return msBuildSettings;
}

Task("Clean")
    .Does(() => {
    CleanDirectory(buildDir);
	CleanDirectories("./src/**/bin");
	CleanDirectories("./src/**/obj");
});

Task("Rebuild")
	.IsDependentOn("Clean")
	.IsDependentOn("Build");

Task("Restore-NuGet-Packages")
    .Does(() => {
	DotNetCoreRestore(dotNetSdkProjectFile);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
        DotNetCoreBuild(dotNetSdkProjectFile, new DotNetCoreBuildSettings {Configuration = configuration});
});

Task("Publish")
	.Description("Internal task - do not use")
    .IsDependentOn("Rebuild");

Task("NuGet-Pack")
	.IsDependentOn("Rebuild")
	.Description("Packs up a NuGet package")
	.Does(() => {
		DotNetCorePack(dotNetSdkProjectFile, new DotNetCorePackSettings {Configuration = configuration});
	});
	

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("None");

Task("Default")
    .IsDependentOn("Rebuild");
	
Task("Pack")
    .IsDependentOn("NuGet-Pack");


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
