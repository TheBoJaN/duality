using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality.Drawing;

namespace Duality
{
	/// <summary>
	/// Represents a vector based on a certain space origin in the context of <see cref="VisualLog">visual logging</see>.
	/// </summary>
	public sealed class VisualLogVectorEntry : VisualLogEntry
	{
		private	Vector3D		origin			= Vector3D.Zero;
		private	Vector2D		vec				= -Vector2D.UnitY * 50.0f;
		private	bool		invariantScale	= false;

		/// <summary>
		/// [GET / SET] The vectors origin in space.
		/// </summary>
		public Vector3D Origin
		{
			get { return this.origin; }
			set { this.origin = value; }
		}
		/// <summary>
		/// [GET / SET] The vector to display.
		/// </summary>
		public Vector2D Vector
		{
			get { return this.vec; }
			set { this.vec = value; }
		}
		/// <summary>
		/// [GET / SET] Whether or not the vector will be displayed at a constant size regardless of perspective scale.
		/// </summary>
		public bool InvariantScale
		{
			get { return this.invariantScale; }
			set { this.invariantScale = value; }
		}

		public override void Draw(Canvas target, Vector3D basePos, double baseRotation, double baseScale)
		{
			double originRadius = 5.0f;
			double vectorThickness = 4.0f;
			double borderRadius = DefaultOutlineWidth;
			double vectorLengthFactor = 1.0f;

			// Scale anti-proportional to perspective scale in order to keep a constant size 
			// in screen space even when actually drawing in world space.
			{
				double scale = target.DrawDevice.GetScaleAtZ((float)(this.origin.Z + basePos.Z));
				originRadius /= scale;
				borderRadius /= scale;
				vectorThickness /= scale;
				if (this.invariantScale) vectorLengthFactor /= scale;
			}


			// Determine base and target positions
			Vector3D originPos = this.origin;
			Vector3D targetPos = this.origin + new Vector3D(this.vec * vectorLengthFactor);
			MathD.TransformCoord(ref originPos.X, ref originPos.Y, baseRotation, baseScale);
			MathD.TransformCoord(ref targetPos.X, ref targetPos.Y, baseRotation, baseScale);
			originPos += basePos;
			targetPos += basePos;

			// Downscale vector arrow, if too small for display otherwise
			double vectorLen = (targetPos.Xy - originPos.Xy).Length;
			double vectorLenScale = MathD.Clamp(vectorLen / (vectorThickness * 7.0f), 0.0f, 1.0f);
			vectorThickness *= vectorLenScale;

			// Create arrow polygon
			Vector2D dirForward = (targetPos.Xy - originPos.Xy).Normalized;
			Vector2D dirLeft = dirForward.PerpendicularLeft;
			Vector2D dirRight = -dirLeft;
			Vector2D[] arrow = new Vector2D[7];
			arrow[0] = dirLeft * vectorThickness * 0.5f;
			arrow[1] = dirLeft * vectorThickness * 0.5f + dirForward * (vectorLen - vectorThickness * 2);
			arrow[2] = dirLeft * vectorThickness * 1.25f + dirForward * (vectorLen - vectorThickness * 2);
			arrow[3] = dirForward * vectorLen;
			arrow[4] = dirRight * vectorThickness * 1.25f + dirForward * (vectorLen - vectorThickness * 2);
			arrow[5] = dirRight * vectorThickness * 0.5f + dirForward * (vectorLen - vectorThickness * 2);
			arrow[6] = dirRight * vectorThickness * 0.5f;
			Vector2D[] arrowHead = new Vector2D[3];
			arrowHead[0] = arrow[2];
			arrowHead[1] = arrow[3];
			arrowHead[2] = arrow[4];
			Vector2D[] arrowLine = new Vector2D[4];
			arrowLine[0] = arrow[0];
			arrowLine[1] = arrow[1];
			arrowLine[2] = arrow[5];
			arrowLine[3] = arrow[6];

			// Draw vector and outline
			ColorRgba areaColor = target.State.ColorTint * this.Color;
			ColorRgba outlineColor = areaColor * ColorRgba.Black;
			target.State.ColorTint = areaColor;
			target.FillPolygon(
				arrowLine, 
				originPos.X, 
				originPos.Y, 
				originPos.Z);
			target.FillPolygon(
				arrowHead, 
				originPos.X, 
				originPos.Y, 
				originPos.Z);
			target.State.DepthOffset -= 0.01f;
			target.State.ColorTint = outlineColor;
			target.FillPolygonOutline(
				arrow, 
				borderRadius,
				originPos.X, 
				originPos.Y, 
				originPos.Z);

			// Draw origin and outline
			target.State.DepthOffset -= 0.01f;
			target.State.ColorTint = areaColor;
			target.FillCircle(
				originPos.X, 
				originPos.Y, 
				originPos.Z, 
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
		}
	}
}
