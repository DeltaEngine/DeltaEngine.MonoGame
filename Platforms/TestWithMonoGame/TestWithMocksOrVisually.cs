﻿using System;
using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Automatically tests with MockResolver when NCrunch is used, otherwise XnaResolver is used.
	/// </summary>
	[TestFixture]
	public class TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeResolver()
		{
			if (StackTraceExtensions.StartedFromNCrunch)
			{
				resolver = new MockResolver();
				return;
			}
			//ncrunch: no coverage start
			if (!StackTraceExtensions.StartedFromProgramMain)
				StackTraceExtensions.SetUnitTestName(TestContext.CurrentContext.Test.FullName);
			resolver = new XnaResolver();
			if (StackTraceExtensions.IsCloseAfterFirstFrameAttributeUsed() ||
				StackTraceExtensions.IsStartedFromNunitConsole())
				Resolve<Window>().CloseAfterFrame();
			//ncrunch: no coverage end
		}

		protected AppRunner resolver;

		[TearDown]
		public void RunTestAndDisposeResolverWhenDone()
		{
			if (StackTraceExtensions.StartedFromProgramMain ||
				TestContext.CurrentContext.Result.Status == TestStatus.Passed)
				resolver.Run();
			else
				resolver.Dispose();
		}

		protected T Resolve<T>() where T : class
		{
			return resolver.Resolve<T>();
		}

		protected void RunAfterFirstFrame(Action executeOnce)
		{
			resolver.CodeAfterFirstFrame = executeOnce;
		}

		protected void AdvanceTimeAndUpdateEntities(
			float timeToAddInSeconds = 1.0f / Settings.DefaultUpdatesPerSecond)
		{
			var drawing = resolver.Resolve<Drawing>();
			if (CheckIfWeNeedToRunTickToAvoidInitializationDelay())
				RunTickOnce(drawing);
			var startTimeMs = GlobalTime.Current.Milliseconds;
			do
				RunTickOnce(drawing);
			while (GlobalTime.Current.Milliseconds - startTimeMs +
				MathExtensions.Epsilon < timeToAddInSeconds * 1000);
		}

		private bool CheckIfWeNeedToRunTickToAvoidInitializationDelay()
		{
			return !(resolver is MockResolver) && GlobalTime.Current.Milliseconds == 0;
		}

		private static void RunTickOnce(Drawing drawing)
		{
			GlobalTime.Current.Update();
			EntitiesRunner.Current.UpdateAndDrawAllEntities(drawing.DrawEverythingInCurrentLayer);
		}
	}
}