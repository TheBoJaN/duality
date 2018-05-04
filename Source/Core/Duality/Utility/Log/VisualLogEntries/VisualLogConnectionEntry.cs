using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality.Drawing;

namespace Duality
{
	/// <summary>
	/// Represents a connection between two points in space in the context of <see cref="VisualLog">visual logging</see>.
	/// </summary>
	public sealed class VisualLogConnectionEntry : VisualLogEntry
	{
		private	Vector3D posA;
		private	Vector3D posB;
		
		/// <summary>
		/// [GET / SET] The first points spatial location.
		/// </summary>
		public Vector3D PosA
		{
			get { return this.posA; }
			set { this.posA = value; }
		}
		/// <summary>
		/// [GET / SET] The second points spatial location.
		/// </summary>
		public Vector3D PosB
		{
			get { return this.posB; }
			set { this.posB = value; }
		}

		public override void Draw(Canvas target, Vector3D basePos, double baseRotation, double baseScale)
		{
			double originRadius = 5.0f;
			double vectorThickness = 4.0f;
			double borderRadius = DefaultOutlineWidth;

			// Scale anti-proportional to perspective scale in order to keep a constant size 
			// in screen space even when actually drawing in world space.
			{
				double scale = target.DrawDevice.GetScaleAtZ((float)(this.posA.Z + basePos.Z));
				originRadius /= scale;
				borderRadius /= scale;
				vectorThickness /= scale;
			}


			// Determine base and target positions
			Vector3D originPos = this.posA;
			Vector3D targetPos = this.posB;
			MathD.TransformCoord(ref originPos.X, ref originPos.Y, baseRotation, baseScale);
			MathD.TransformCoord(ref targetPos.X, ref targetPos.Y, baseRotation, baseScale);
			originPos += basePos;
			targetPos += basePos;

			// Create connection polygon
			double vectorLen = (targetPos.Xy - originPos.Xy).Length;
			Vector2D dirForward = (targetPos.Xy - originPos.Xy).Normalized;
			Vector2D dirLeft = dirForward.PerpendicularLeft;
			Vector2D dirRight = -dirLeft;
			Vector2D[] connection = new Vector2D[4];
			connection[0] = dirLeft * vectorThickness * 0.5f;
			connection[1] = dirLeft * vectorThickness * 0.5f + dirForward * vectorLen;
			connection[2] = dirRight * vectorThickness * 0.5f + dirForward * vectorLen;
			connection[3] = dirRight * vectorThickness * 0.5f;

			// Draw vector and outline
			ColorRgba areaColor = target.State.ColorTint * this.Color;
			ColorRgba outlineColor = areaColor * ColorRgba.Black;
			target.State.ColorTint = areaColor;
			target.FillPolygon(
				connection, 
				originPos.X, 
				originPos.Y, 
				originPos.Z);
			target.State.DepthOffset -= 0.01f;
			target.State.ColorTint = outlineColor;
			target.FillPolygonOutline(
				connection, 
				borderRadius,
				originPos.X, 
				originPos.Y, 
				originPos.Z);

			// Draw connection points and outline
			target.State.DepthOffset -= 0.01f;
			target.State.ColorTint = areaColor;
			target.FillCircle(
				originPos.X, 
				originPos.Y, 
				originPos.Z, 
				originRadius - borderRadius * 0.5f);
			target.FillCircle(
				targetPos.X, 
				targetPos.Y, 
				targetPos.Z, 
				originRadius - borderRadius * 0.5f);
			target.State.DepthOffset -= 0.01f;
			target.State.ColorTint = outlineColor;
			target.FillCircleSegment(
				originPos.X, 
				originPos.Y, 
				originPos.Z, 
				originRadius, 
				0.0f, 
				MathD.RadAngle360, 
				borderRadius);
			target.FillCircleSegment(
				targetPos.X, 
				targetPos.Y, 
				targetPos.Z, 
				originRadius, 
				0.0f, 
				MathD.RadAngle360, 
				borderRadius);
		}
	}
}
