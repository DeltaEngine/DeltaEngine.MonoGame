using System.Reflection;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Xna game to just call AutofacStarter.UpdateAndRenderEntities. Xna components are not supported
	/// and we do not use any Xna Game feature, even Run is not called directly, but via trickery.
	/// </summary>
	internal sealed class XnaGame : Game
	{
		public XnaGame(XnaResolver resolver)
		{
			this.resolver = resolver;
			IsFixedTimeStep = false;
		}

		private readonly XnaResolver resolver;

		/// <summary>
		/// For XnaResolver and any app initialization code following this call we need a initialized
		/// graphics device. We can't call Run as it would block the App constructor, but we still need
		/// to call Game.Run later via RunXnaGame. StartGameLoop does this like Run without blocking.
		/// </summary>
		public void StartXnaGameToInitializeGraphics()
		{
			var deviceManagerProperty = GetType().GetProperty("graphicsDeviceManager",
				BindingFlags.NonPublic | BindingFlags.Instance);
			deviceManagerProperty.GetGetMethod(true).Invoke(this, null);
}

		/// <summary>
		/// This is the continuation of the StartXnaGameToInitializeGraphics method above. Here we
		/// continue what Game.Run would have normally done. This blocks until the window is closed.
		/// </summary>
		public void RunXnaGame()
		{
			Run();
}

		protected override void Update(GameTime gameTime) {}

		protected override void Draw(GameTime gameTime)
		{
			resolver.RunTick();
		}
	}
}