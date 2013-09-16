﻿using System;
using System.IO;
using System.Reflection;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Additional methods for assembly related actions.
	/// </summary>
	public static class AssemblyExtensions
	{
		//ncrunch: no coverage start

		public static string GetMyDocumentsAppFolder()
		{
			var appPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DeltaEngine",
				GetEntryAssemblyForProjectName());
			if (!Directory.Exists(appPath))
				Directory.CreateDirectory(appPath);
			return appPath;
		}

		public static string GetTestNameOrProjectName()
		{
			return testOrProjectName ?? (testOrProjectName = StackTraceExtensions.GetEntryName());
		}

		private static string testOrProjectName;

		public static string GetEntryAssemblyForProjectName()
		{
			return projectName ?? (projectName = StackTraceExtensions.GetExecutingAssemblyName());
		}

		private static string projectName;

		public static bool IsAllowed(this AssemblyName assembly)
		{
			var name = assembly.Name;
			return !(IsMicrosoftAssembly(name) || IsIdeHelperTool(name) || IsThirdPartyLibrary(name));
		}

		public static bool IsAllowed(this Assembly assembly)
		{
			var name = assembly.GetName().Name;
			return !(IsMicrosoftAssembly(name) || IsIdeHelperTool(name) || IsThirdPartyLibrary(name));
		}

		private static bool IsMicrosoftAssembly(string name)
		{
			return name.StartsWith("System", "mscorlib", "WindowsBase", "PresentationFramework",
				"PresentationCore", "WindowsFormsIntegration", "Microsoft.");
		}

		private static bool IsIdeHelperTool(string name)
		{
			return name.StartsWith("JetBrains.", "NUnit.", "NCrunch.", "ReSharper.", "vshost32");
		}

		private static bool IsThirdPartyLibrary(string name)
		{
			return name.StartsWith("OpenAL32", "wrap_oal", "libEGL", "libgles", "libGLESv2", "libvlc",
				"libvlccore", "csogg", "csvorbis", "Autofac", "Moq", "DynamicProxyGen",
				"Anonymously Hosted", "AvalonDock", "Pencil.Gaming", "Glfw", "Newtonsoft.Json", "OpenTK",
				"NVorbis", "Farseer", "MvvmLight", "SharpDX", "SlimDX", "ToyMp3", "EntityFramework",
				"NHibernate", "Approval", "System.IO.Abstractions");
		}

		public static bool IsPlatformAssembly(string assemblyName)
		{
			if (assemblyName.EndsWith(".Tests") || assemblyName.EndsWith(".Remote") ||
				assemblyName == "DeltaEngine.Input")
				return false;
			return !new AssemblyName(assemblyName).IsAllowed() || assemblyName == "DeltaEngine" ||
				assemblyName == "DeltaEngine.Content.Disk" || assemblyName == "DeltaEngine.Content.Online" ||
				assemblyName.StartsWith("DeltaEngine.Graphics.") ||
				assemblyName.StartsWith("DeltaEngine.Multimedia.") ||
				assemblyName.StartsWith("DeltaEngine.Input.") ||
				assemblyName.StartsWith("DeltaEngine.Platforms") ||
				assemblyName.StartsWith("DeltaEngine.Windows") ||
				assemblyName.StartsWith("DeltaEngine.TestWith");
		}
	}
}