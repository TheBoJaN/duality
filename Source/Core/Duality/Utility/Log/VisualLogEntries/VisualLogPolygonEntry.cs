using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality.Drawing;

namespace Duality
{
	/// <summary>
	/// Represents a polygon in the context of <see cref="VisualLog">visual logging</see>.
	/// </summary>
	public sealed class VisualLogPolygonEntry : VisualLogEntry
	{
		private	Vector3D		pos				= Vector3D.Zero;
		private	Vector2D[]	vertices		= null;
		private	bool		invariantScale	= false;
		
		/// <summary>
		/// [GET / SET] The polygons origin in space.
		/// </summary>
		public Vector3D Pos
		{
			get { return this.pos; }
			set { this.pos = value; }
		}
		/// <summary>
		/// [GET / SET] The vertices that form the displayed polygon.
		/// </summary>
		public Vector2D[] Vertices
		{
			get { return this.vertices; }
			set { this.vertices = value; }
		}
		/// <summary>
		/// [GET / SET] Whether or not the polygon area will be displayed at a constant size regardless of perspective scale.
		/// </summary>
		public bool InvariantScale
		{
			get { return this.invariantScale; }
			set { this.invariantScale = value; }
		}

		public override void Draw(Canvas target, Vector3D basePos, double baseRotation, double baseScale)
		{
			double borderRadius = DefaultOutlineWidth;
			double polyScale = 1.0f;

			// Scale anti-proportional to perspective scale in order to keep a constant size 
			// in screen space even when actually drawing in world space.
			{
				double scale = target.DrawDevice.GetScaleAtZ((float)(this.pos.Z + basePos.Z));
				borderRadius /= scale;
				if (this.invariantScale) polyScale /= scale;
			}

			// Determine base position
			Vector3D circlePos = this.pos;
			MathD.TransformCoord(ref circlePos.X, ref circlePos.Y, baseRotation, baseScale);
			circlePos += basePos;

			// Draw polygon and outline
			target.State.ColorTint *= this.Color;
			target.State.TransformAngle = (float)baseRotation;
			target.State.TransformScale = new Vector2D(baseScale, baseScale) * polyScale;
			target.FillPolygon(
				this.vertices,
				circlePos.X, 
				circlePos.Y, 
				circlePos.Z);
			target.State.DepthOffset -= 0.01f;
			target.State.ColorTint *= ColorRgba.Black;
			target.FillPolygonOutline(
				this.vertices,
				borderRadius / polyScale,
				circlePos.X, 
				circlePos.Y, 
				circlePos.Z);
		}
	}
}
