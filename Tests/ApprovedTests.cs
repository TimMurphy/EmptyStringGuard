﻿#if(DEBUG)
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;

[Ignore("ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.Version40) returns null on my machine")]
[TestFixture]
[UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
public class ApprovedTests
{
    [Test]
    public void ClassWithBadAttributes()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "ClassWithBadAttributes"));
    }

    [Test]
    public void ClassWithPrivateMethod()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "ClassWithPrivateMethod"));
    }

    [Test]
    public void InterfaceBadAttributes()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "InterfaceBadAttributes"));
    }

    [Test]
    public void SimpleClass()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "SimpleClass"));
    }

    [Test]
    public void SkipIXamlMetadataProvider()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "XamlMetadataProvider"));
    }

    [Test]
    public void SpecialClass()
    {
        Approvals.Verify(Decompile(AssemblyWeaver.AfterAssemblyPath, "SpecialClass"));
    }

    private static string Decompile(string assemblyPath, string identifier = "")
    {
        var exePath = GetPathToILDasm();

        if (!string.IsNullOrEmpty(identifier))
            identifier = "/item:" + identifier;

        using (var process = Process.Start(new ProcessStartInfo(exePath, $"\"{assemblyPath}\" /nobar /text {identifier}")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }))
        {

            var projectFolder = Path.GetFullPath(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory) + @"\..\..").Replace(@"\", @"\\");
            projectFolder = $@"{projectFolder[0]}{projectFolder.Substring(1)}\\";

            process.WaitForExit(10000);

            var decompile = process.StandardOutput.ReadToEnd()
                .Replace(projectFolder, "");

            return decompile.Substring(decompile.IndexOf("// warning :"));
        }
    }

    private static string GetPathToILDasm()
    {
        var path = Path.Combine(ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.Version40), @"bin\NETFX 4.0 Tools\ildasm.exe");
        if (!File.Exists(path))
            path = path.Replace("v7.0", "v8.0");
        if (!File.Exists(path))
            Assert.Ignore("ILDasm could not be found");
        return path;
    }
}

#endif