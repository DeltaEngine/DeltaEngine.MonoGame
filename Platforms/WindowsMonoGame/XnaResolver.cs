﻿using DeltaEngine.Graphics.MonoGame;
using DeltaEngine.Input.MonoGame;
using DeltaEngine.Multimedia.MonoGame;
using DeltaEngine.Platforms.Windows;
using Microsoft.Xna.Framework.Media;

namespace DeltaEngine.Platforms
{
	internal class XnaResolver : AppRunner
	{
		public XnaResolver()
		{
			RegisterCommonEngineSingletons();
			game = new XnaGame(this);
			window = new XnaWindow(game);
			window.ViewportPixelSize = settings.Resolution;
			RegisterInstance(window);
			RegisterSingleton<WindowsSystemInformation>();
			var device = new XnaDevice(game, window, settings);
			RegisterInstance(device);
			game.StartXnaGameToInitializeGraphics();
			RegisterInstance(game);
			RegisterInstance(game.Content);
			RegisterSingleton<XnaSoundDevice>();
			RegisterSingleton<XnaScreenshotCapturer>();
			RegisterSingleton<XnaMouse>();
			RegisterSingleton<XnaKeyboard>();
			RegisterSingleton<XnaTouch>();
			RegisterSingleton<XnaGamePad>();
			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();
		}
		
		private readonly XnaGame game;
		private readonly XnaWindow window;

		protected override void RegisterMediaTypes()
		{
			base.RegisterMediaTypes();
			Register<XnaImage>();
			Register<XnaShader>();
			Register<XnaGeometry>();
			Register<XnaSound>();
			Register<XnaMusic>();
		}

		/// <summary>
		/// Instead of starting the game normally and blocking we will delay the initialization in
		/// XnaGame until the game class has been constructed and the graphics soundDevice is available.
		/// </summary>
		public override void Run()
		{
			game.RunXnaGame();
			game.Dispose();
		}
	}
}