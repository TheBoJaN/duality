using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality.Drawing;

namespace Duality
{
	/// <summary>
	/// Represents a circular area in space in the context of <see cref="VisualLog">visual logging</see>.
	/// </summary>
	public sealed class VisualLogCircleEntry : VisualLogEntry
	{
		private	Vector3D pos				= Vector3D.Zero;
		private	bool	invariantScale	= false;
		private	double	radius			= 100.0f;
		private	double	minAngle		= 0.0f;
		private	double	maxAngle		= MathD.RadAngle360;
		
		/// <summary>
		/// [GET / SET] The circular areas center location.
		/// </summary>
		public Vector3D Pos
		{
			get { return this.pos; }
			set { this.pos = value; }
		}
		/// <summary>
		/// [GET / SET] The circles radius.
		/// </summary>
		public double Radius
		{
			get { return this.radius; }
			set { this.radius = value; }
		}
		/// <summary>
		/// [GET / SET] The minimum angle of the displayed circle area segment.
		/// </summary>
		public double MinAngle
		{
			get { return this.minAngle; }
			set { this.minAngle = value; }
		}
		/// <summary>
		/// [GET / SET] The maximum angle of the displayed circle area segment.
		/// </summary>
		public double MaxAngle
		{
			get { return this.maxAngle; }
			set { this.maxAngle = value; }
		}
		/// <summary>
		/// [GET / SET] Whether or not the circle area will be displayed at a constant size regardless of perspective scale.
		/// </summary>
		public bool InvariantScale
		{
			get { return this.invariantScale; }
			set { this.invariantScale = value; }
		}

		public override void Draw(Canvas target, Vector3D basePos, double baseRotation, double baseScale)
		{
			double borderRadius = DefaultOutlineWidth;
			double circleRadius = this.radius * baseScale;

			// Scale anti-proportional to perspective scale in order to keep a constant size 
			// in screen space even when actually drawing in world space.
			{
				double scale = target.DrawDevice.GetScaleAtZ((float)(this.pos.Z + basePos.Z));
				borderRadius /= scale;
				if (this.invariantScale) circleRadius /= scale;
			}

			// Determine circle position
			Vector3D circlePos = this.pos;
			MathD.TransformCoord(ref circlePos.X, ref circlePos.Y, baseRotation, baseScale);
			circlePos += basePos;

			// Draw circle
			target.State.ColorTint *= this.Color;
			target.FillCircleSegment(
				circlePos.X, 
				circlePos.Y, 
				circlePos.Z, 
				circleRadius - borderRadius * 0.5f,
				this.minAngle + baseRotation,
				this.maxAngle + baseRotation);

			// Draw circle outline
			target.State.DepthOffset -= 0.01f;
			target.State.ColorTint *= ColorRgba.Black;
			target.FillCircleSegment(
				circlePos.X, 
				circlePos.Y, 
				circlePos.Z, 
				circleRadius, 
				this.minAngle + baseRotation, 
				this.maxAngle + baseRotation, 
				borderRadius);
			if (MathD.CircularDist(this.minAngle, this.maxAngle) > MathD.RadAngle1 * 0.001f)
			{
				Vector2D minAngleVec = Vector2D.FromAngleLength(this.minAngle + baseRotation, circleRadius);
				Vector2D maxAngleVec = Vector2D.FromAngleLength(this.maxAngle + baseRotation, circleRadius);
				target.FillThickLine(
					circlePos.X, 
					circlePos.Y, 
					circlePos.Z,
					circlePos.X + minAngleVec.X, 
					circlePos.Y + minAngleVec.Y, 
					circlePos.Z,
					borderRadius);
				target.FillThickLine(
					circlePos.X, 
					circlePos.Y, 
					circlePos.Z,
					circlePos.X + maxAngleVec.X, 
					circlePos.Y + maxAngleVec.Y, 
					circlePos.Z,
					borderRadius);
			}
		}
	}
}
