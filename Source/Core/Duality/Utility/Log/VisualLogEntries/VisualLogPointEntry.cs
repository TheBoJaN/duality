using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality.Drawing;

namespace Duality
{
	/// <summary>
	/// Represents a point in space in the context of <see cref="VisualLog">visual logging</see>. Points do not have a physical size 
	/// and thus are displayed invariantly to parent or perspective scale.
	/// </summary>
	public sealed class VisualLogPointEntry : VisualLogEntry
	{
		private	Vector3D pos;

		/// <summary>
		/// [GET / SET] The points spatial location.
		/// </summary>
		public Vector3D Pos
		{
			get { return this.pos; }
			set { this.pos = value; }
		}

		public override void Draw(Canvas target, Vector3D basePos, double baseRotation, double baseScale)
		{
			double circleRadius = 5.0f;
			double borderRadius = DefaultOutlineWidth;

			// Scale anti-proportional to perspective scale in order to keep a constant size 
			// in screen space even when actually drawing in world space.
			{
				double scale = target.DrawDevice.GetScaleAtZ((float)(this.pos.Z + basePos.Z));
				circleRadius /= scale;
				borderRadius /= scale;
			}

			// Determine circle position
			Vector3D circlePos = this.pos;
			MathD.TransformCoord(ref circlePos.X, ref circlePos.Y, baseRotation, baseScale);
			circlePos += basePos;

			// Draw circle
			target.State.ColorTint *= this.Color;
			target.FillCircle(
				circlePos.X, 
				circlePos.Y, 
				circlePos.Z, 
				circleRadius - borderRadius * 0.5f);

			// Draw circle outline
			target.State.DepthOffset -= 0.01f;
			target.State.ColorTint *= ColorRgba.Black;
			target.FillCircleSegment(
				circlePos.X, 
				circlePos.Y, 
				circlePos.Z, 
				circleRadius, 
				0.0f, 
				MathD.RadAngle360, 
				borderRadius);
		}
	}
}
