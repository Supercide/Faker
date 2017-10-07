#tool "nuget:?package=NUnit.ConsoleRunner"
///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////
var task            = Argument<string>("task", "Default");
var configuration   = Argument<string>("configuration", "Release");
var nugetApiKey     = Argument<string>("nugetApiKey", null);
var localFeed       = Argument<string>("localfeed", null);
var isLocal         = Argument<bool>("isLocal", true);
var version         = Argument<string>("buildVersion", "0.0.1");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////
var artifacts       = "./artifacts/";
var testResults     = string.Concat(artifacts, "test-results/");
var packages        = string.Concat(artifacts, "packages/");
var solution        = GetFiles("./**/*.sln").First().FullPath;

///////////////////////////////////////////////////////////////////////////////
// Clean Environment
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() => {
        CleanDirectories("./**/bin");
        CleanDirectories("./**/obj");
        CleanDirectories("./**/artifacts");
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => 
    {
        NuGetRestore(solution);
    });

///////////////////////////////////////////////////////////////////////////////
// Build Projects
///////////////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var buildSettings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            ArgumentCustomization = args => args.Append("/p:SemVer=" + version)
        };

        DotNetCoreBuild(solution, buildSettings);
    });

///////////////////////////////////////////////////////////////////////////////
// Run Tests
///////////////////////////////////////////////////////////////////////////////

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var settings = new DotNetCoreTestSettings
        {
           NoBuild = true,
           Configuration = configuration,
           ArgumentCustomization = args => {
                args.Append("--logger:trx");
                args.Append("--results-directory:"+MakeAbsolute(File(testResults)));
                return args;
           }
        };

        var testProjects = GetFiles("./tests/**/*.csproj");

        foreach(var project in testProjects)
        {
            DotNetCoreTest(project.FullPath, settings);
        }
    });

///////////////////////////////////////////////////////////////////////////////
// Create Nuget Packages
///////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DotNetCoreMSBuildSettings buildSettings = new DotNetCoreMSBuildSettings();
        buildSettings.SetVersionPrefix(version);

        var settings = new DotNetCorePackSettings
        {
            NoBuild = true,
            MSBuildSettings = buildSettings,
            Configuration = configuration,
            OutputDirectory = packages
        };

        var projects = GetFiles("./src/**/*.csproj");

        foreach(var project in projects)
        {
            DotNetCorePack(project.FullPath, settings);
        }
    });

Task("Publish-Local")
    .IsDependentOn("pack")
    .WithCriteria(() => isLocal)
    .WithCriteria(() => !string.IsNullOrWhiteSpace(localFeed))
    .Does(() => {
        var packageFiles = GetFiles(packages + "*.nupkg");
        
        var processSettings = new ProcessSettings{ Arguments = string.Format("init {0} {1}", packages, localFeed) };
        
        var exitCodeWithArgument = StartProcess("nuget", processSettings);
        
        Information("Exit code: {0}", exitCodeWithArgument);
    });

Task("Publish")
    .IsDependentOn("pack")
    .WithCriteria(() => !isLocal)
    .WithCriteria(() => !string.IsNullOrWhiteSpace(nugetApiKey))
    .Does(() => {
        var packageFiles = GetFiles(packages + "*.nupkg");
        foreach(var package in packageFiles)
        {
            var nugetSettings = new NuGetPushSettings 
            {
                Source = "https://www.nuget.org/api/v2/package",
                ApiKey = nugetApiKey 
            };

            NuGetPush(package, nugetSettings);
        }
    });

Task("Default")
    .IsDependentOn("Publish-local");

Task("Local")
    .IsDependentOn("Publish-local");

Task("Server")
    .IsDependentOn("Publish");

RunTarget(task);