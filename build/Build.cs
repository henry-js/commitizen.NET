using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MinVer;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    On = [GitHubActionsTrigger.Push],
    InvokedTargets = [nameof(Compile)])]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)] readonly Solution Solution;
    [GitRepository] readonly GitRepository Repository;
    [MinVer] readonly MinVer MinVer;
    AbsolutePath ProjectDirectory => SourceDirectory / "Cli";
    AbsolutePath ArtifactsDirectory => RootDirectory / ".artifacts";
    AbsolutePath PublishDirectory => RootDirectory / "publish";
    AbsolutePath PackDirectory => RootDirectory / "packages";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestDirectory => RootDirectory / "tests" / "commitizen.NET.Tests";
    IEnumerable<string> Projects => Solution.AllProjects.Select(x => x.Name);

    Target Print => _ => _
    .Executes(() =>
    {
        Log.Information("Minver Version = {Value}", MinVer.Version);
        Log.Information("Commit = {Value}", Repository.Commit);
        Log.Information("Branch = {Value}", Repository.Branch);
        Log.Information("Tags = {Value}", Repository.Tags);

        Log.Information("main branch = {Value}", Repository.IsOnMainBranch());
        Log.Information("main/master branch = {Value}", Repository.IsOnMainOrMasterBranch());
        Log.Information("release/* branch = {Value}", Repository.IsOnReleaseBranch());
        Log.Information("hotfix/* branch = {Value}", Repository.IsOnHotfixBranch());

        Log.Information("Https URL = {Value}", Repository.HttpsUrl);
        Log.Information("SSH URL = {Value}", Repository.SshUrl);
    });

    Target Clean => _ => _
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
    .After(Clean)
        .Executes(() =>
        {
            DotNetRestore(c =>
            c.SetForce(true)
                .SetProjectFile(Solution.Directory));
        });

    Target Compile => _ => _
        .DependsOn(Clean, Restore)
        .Executes(() =>
            {
                Log.Information("Building version {Value}", MinVer.Version);
                DotNetBuild(_ => _
                    // .EnableSelfContained()
                    // .EnablePublishSingleFile()
                    .SetProjectFile(ProjectDirectory)
                    .SetConfiguration("Release")
                    // .SetRuntime("win-x64")
                    .EnableNoRestore());
            });
    IReadOnlyCollection<Output> Outputs;
    Target Test => _ => _
        .TriggeredBy(Compile)
        .Before(Publish, Pack)
        .Executes(() =>
        {
            var ResultsDirectory = RootDirectory / "TestResults";
            ResultsDirectory.CreateOrCleanDirectory();
            Outputs = DotNetTest(_ => _
                .SetProjectFile(TestDirectory)
                // .EnableNoBuild()
                // .EnableNoRestore()
                .SetDataCollector("XPlat Code Coverage")
                .SetResultsDirectory(ResultsDirectory)
                .SetRunSetting(
                    "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByAttribute",
                     "Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute")
                );

            Log.Information($"Outputs: {Outputs.Count}");

            var coverageReport = (RootDirectory / "TestResults").GetFiles("coverage.cobertura.xml", 2).FirstOrDefault();

            if (coverageReport is not null)
            {
                ReportGenerator(_ => _
                    .AddReports(coverageReport)
                    .SetTargetDirectory(ResultsDirectory / "coveragereport")
                    );
            }
        });

    Target Pack => _ => _
        .Requires(() => RepoIsMainOrDevelop)
        .WhenSkipped(DependencyBehavior.Skip)
        // .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(ProjectDirectory)
                .EnableNoBuild()
                .EnableNoRestore()
            );
        });
    Target Publish => _ => _
        .Requires(() => RepoIsMainOrDevelop)
        .WhenSkipped(DependencyBehavior.Skip)
        .DependsOn(Compile)
        .Executes(() =>
        {
            PublishDirectory.CreateOrCleanDirectory();

            DotNetPublish(_ => _
                .SetProject(ProjectDirectory)
                // .EnableSelfContained()
                // .EnablePublishSingleFile()
                // .SetRuntime("win-x64")
                .EnableNoBuild()
                .EnableNoRestore()
            );
        });
    bool RepoIsMainOrDevelop => Repository.IsOnDevelopBranch() || Repository.IsOnMainOrMasterBranch();
}
