#tool "nuget:?package=NUnit.ConsoleRunner"

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// VARIABLES
//////////////////////////////////////////////////////////////////////

string semVersion = EnvironmentVariable("BUILD_VERSION") ?? "1.0.0-DEV";
string version = string.Join(".", semVersion.Split(new string []{".","-"}, StringSplitOptions.RemoveEmptyEntries).Take(3));

const string BUILD_CONFIG = "Release";
const string SOLUTION_PATH = "./Faker.sln";
const string FRAMEWORK = "netcoreapp2.0";

///////////////////////////////////////////////////////////////////////////////
// PREPARE
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() => {
        CleanDirectories("./src/**/bin");
        CleanDirectories("./tests/**/bin");
    });

Task("NugetRestore")
    .IsDependentOn("Clean")
    .Does(() => 
    {
        NuGetRestore(SOLUTION_PATH);
    });

///////////////////////////////////////////////////////////////////////////////
// BUILD & PATCH
///////////////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("NugetRestore")
    .Does(() =>
    {
        var buildSettings = new DotNetCoreBuildSettings
        {
            Framework = FRAMEWORK,
            Configuration = BUILD_CONFIG,
            ArgumentCustomization = args => args.Append("/p:SemVer=" + semVersion)
        };

        DotNetCoreBuild(SOLUTION_PATH, buildSettings);
    });

///////////////////////////////////////////////////////////////////////////////
// TESTS
///////////////////////////////////////////////////////////////////////////////

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
		var testAssemblies = GetFiles(string.Format("./tests/**/bin/{0}/{1}/*Tests.dll", BUILD_CONFIG, FRAMEWORK));

        DotNetCoreTest(testAssemblies);
    });

///////////////////////////////////////////////////////////////////////////////
// TAG
///////////////////////////////////////////////////////////////////////////////


Task("Default")
    .IsDependentOn("Test");

RunTarget(target);