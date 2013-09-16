using System;
using DeltaEngine;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering.Sprites;

namespace $safeprojectname$
{
	public class GhostWave : Entity, Updateable, IDisposable
	{
		public GhostWave(Point start, Point target, int waveSize, Color color)
		{
			this.start = start;
			this.target = target;
			sprites = new Sprite[waveSize];
			var ghostMaterial = new Material(Shader.Position2DColorUv, "Ghost") {
				DefaultColor = color
			};
			for (int num = 0; num < waveSize; num++)
				sprites [num] = CreateSpriteWithOrientation(ghostMaterial);

			UpdatePriority = Priority.Low;
		}

		private readonly Point start;
		private readonly Point target;
		private readonly Sprite[] sprites;

		private Sprite CreateSpriteWithOrientation(Material ghostMaterial)
		{
			var newSprite = new Sprite(ghostMaterial, start) {
				RenderLayer = 1
			};
			if (GameLogic.GhostSize != 1.0f)
				newSprite.Size *= GameLogic.GhostSize;

			if (start.X > target.X)
				newSprite.Coordinates = new Sprite.SpriteCoordinates(Rectangle.One, FlipMode.Horizontal);

			return newSprite;
		}

		public object Attacker
		{
			get;
			set;
		}

		public void Update()
		{
			runTime += Time.Delta;
			for (int i = 0; i < sprites.Length; i++)
				UpdateDrawAreaAndRotation(i);

			if (ReachedTarget)
				Dispose();
		}

		private float runTime;

		private void UpdateDrawAreaAndRotation(int num)
		{
			sprites [num].DrawArea = CurrentDrawArea(num);
			sprites [num].Rotation = CurrentRotation(num);
		}

		private Rectangle CurrentDrawArea(int num)
		{
			var direction = Point.Normalize(target - start);
			var pos = start + direction * runTime * Speed;
			var vertical = direction.RotateAround(Point.Zero, 90);
			pos += vertical * MathExtensions.Sin(runTime * 300) * 0.0035f;
			pos += vertical * MathExtensions.Sin(runTime * 44 + num * 27) * 0.0135f;
			pos += vertical * SpreadDistance * CalcDistanceFromCenter(num, 1.0f, 90, 90);
			return Rectangle.FromCenter(pos, GameLogic.GhostSize * sprites 
				[0].Material.MaterialRenderSize);
		}

		private const float SpreadDistance = 0.06f;
		private const float Speed = 0.08f;

		private float CalcDistanceFromCenter(int num, float initialDistanceValue, float startSin, 
			float targetSin)
		{
			var goalTime = start.DistanceTo(target) / Speed;
			float distanceFromCenter = initialDistanceValue;
			float increaseTime = Math.Min(3, goalTime / 2);
			if (runTime < increaseTime)
				distanceFromCenter = MathExtensions.Sin(startSin * runTime / increaseTime);
			else if (goalTime - runTime < increaseTime)
				distanceFromCenter = MathExtensions.Sin(targetSin * (goalTime - runTime) / increaseTime);

			var normalizedNum = (num - (sprites.Length - 1) / 2.0f) / (sprites.Length / 2.0f);
			return distanceFromCenter * normalizedNum;
		}

		private float CurrentRotation(int num)
		{
			var distanceFromCenter = CalcDistanceFromCenter(num, 0.0f, 180, -180);
			return start.RotationTo(target) + distanceFromCenter * 30 + (start.RotationTo(target).Abs() 
				< 90 ? 0 : 180);
		}

		protected bool ReachedTarget
		{
			get
			{
				return runTime * Speed > start.DistanceTo(target);
			}
		}

		public void Dispose()
		{
			if (TargetReached != null)
				TargetReached(Attacker, sprites.Length);

			TargetReached = null;
			foreach (Sprite sprite in sprites)
				sprite.IsActive = false;

			IsActive = false;
		}

		public Action<object, int> TargetReached;
	}
}