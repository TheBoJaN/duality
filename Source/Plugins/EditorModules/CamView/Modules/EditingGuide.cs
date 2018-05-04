using System;
using System.Collections.Generic;
using System.Linq;

using Duality;

namespace Duality.Editor.Plugins.CamView
{
	public class EditingGuide
	{
		private	Vector3D		gridSize		= Vector3D.Zero;
		private	Vector3D		snapPosOrigin	= Vector3D.Zero;
		private	Vector3D		snapScaleOrigin	= Vector3D.One;


		public Vector3D GridSize
		{
			get { return this.gridSize; }
			set { this.gridSize = value; }
		}
		public Vector3D SnapPosOrigin
		{
			get { return this.snapPosOrigin; }
			set { this.snapPosOrigin = value; }
		}
		public Vector3D SnapScaleOrigin
		{
			get { return this.snapScaleOrigin; }
			set { this.snapScaleOrigin = value; }
		}
		

		/// <summary>
		/// Snaps the specified world position to this editing guide.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Vector3D SnapPosition(Vector3D pos)
		{
			Vector3D localPos = (pos - this.snapPosOrigin) / this.snapScaleOrigin;
			Vector3D snapLocalPos = localPos;

			if (this.gridSize.X > 0.001f) snapLocalPos.X = this.gridSize.X * MathD.RoundToInt(snapLocalPos.X / this.gridSize.X);
			if (this.gridSize.Y > 0.001f) snapLocalPos.Y = this.gridSize.Y * MathD.RoundToInt(snapLocalPos.Y / this.gridSize.Y);
			if (this.gridSize.Z > 0.001f) snapLocalPos.Z = this.gridSize.Z * MathD.RoundToInt(snapLocalPos.Z / this.gridSize.Z);

			Vector3D snapPos = this.snapPosOrigin + this.snapScaleOrigin * snapLocalPos;
			return snapPos;
		}
		/// <summary>
		/// Snaps the specified world position to this editing guide.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Vector2D SnapPosition(Vector2D pos)
		{
			return this.SnapPosition(new Vector3D(pos)).Xy;
		}

		/// <summary>
		/// Snaps the specified size value according to match this editing guide.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public Vector3D SnapSize(Vector3D size)
		{
			size /= this.snapScaleOrigin;
			return this.snapScaleOrigin * new Vector3D(
				this.gridSize.X > 0.001f ? this.gridSize.X * Math.Max(1, (int)(size.X / this.gridSize.X)) : size.X,
				this.gridSize.Y > 0.001f ? this.gridSize.Y * Math.Max(1, (int)(size.Y / this.gridSize.Y)) : size.Y,
				this.gridSize.Z > 0.001f ? this.gridSize.Z * Math.Max(1, (int)(size.Z / this.gridSize.Z)) : size.Z);
		}
		/// <summary>
		/// Snaps the specified size value according to match this editing guide.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public Vector2D SnapSize(Vector2D size)
		{
			return this.SnapSize(new Vector3D(size)).Xy;
		}
		/// <summary>
		/// Snaps the specified size value according to match this editing guide.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public double SnapSize(double size)
		{
			double snapOrigin = 1.0f;
			double snapStep = 0.001f;
			if (this.gridSize.X > snapStep)
			{
				snapStep = this.gridSize.X;
				snapOrigin = this.snapScaleOrigin.X;
			}
			if (this.gridSize.Y > snapStep)
			{
				snapStep = this.gridSize.Y;
				snapOrigin = this.snapScaleOrigin.Y;
			}
			if (this.gridSize.Z > snapStep)
			{
				snapStep = this.gridSize.Z;
				snapOrigin = this.snapScaleOrigin.Z;
			}
			return snapStep > 0.001f ? snapOrigin * snapStep * Math.Max(1, (int)((size / snapOrigin) / snapStep)) : size;
		}
	}
}
