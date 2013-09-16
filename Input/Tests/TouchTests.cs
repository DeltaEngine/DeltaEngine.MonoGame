﻿using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowRedCircleOnTouchAtTouchPosition()
		{
			new FontText(Font.Default, "Touch screen to show red circle", Rectangle.One);
			var ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			new Command(() => TranslateOnTouch(ellipse)).Add(new TouchPressTrigger(State.Pressed));
		}

		private void TranslateOnTouch(Entity2D ellipse)
		{
			Point position = Resolve<Touch>().GetPosition(0);
			var drawArea = ellipse.DrawArea;
			drawArea.Left = position.X;
			drawArea.Top = position.Y;
			ellipse.DrawArea = drawArea;
		}

		[Test, CloseAfterFirstFrame]
		public void TestPositionAndState()
		{
			bool isTouched = false;
			new Command(() => isTouched = true).Add(new TouchPressTrigger(State.Pressed));
			Assert.IsFalse(isTouched);
			var mockTouch = Resolve<Touch>() as MockTouch;
			if (mockTouch == null)
				return; //ncrunch: no coverage
			Assert.NotNull(mockTouch);
			Assert.AreEqual(State.Released, mockTouch.GetState(0));
			Assert.True(mockTouch.IsAvailable);
		}
	}
}