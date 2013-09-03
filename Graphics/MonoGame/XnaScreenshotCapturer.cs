using System.IO;
using DeltaEngine.Core;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace DeltaEngine.Graphics.MonoGame
{
	/// <summary>
	/// Capturing screenshots with Xna is easy in the HiDef profile, for Reach use RenderToTexture.
	/// </summary>
	public class XnaScreenshotCapturer : ScreenshotCapturer
	{
		public XnaScreenshotCapturer(XnaDevice device, Window window)
		{
			this.window = window;
			this.device = device;
		}

		private readonly XnaDevice device;
		private readonly Window window;

		public void MakeScreenshot(string fileName)
		{
		}
	}
}