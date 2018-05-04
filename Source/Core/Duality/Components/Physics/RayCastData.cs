using System;
using System.Collections.Generic;
using System.Linq;

namespace Duality.Components.Physics
{
	/// <summary>
	///  Provides data about a <see cref="RigidBody"/> RayCast.
	/// </summary>
	public struct RayCastData
	{
		private ShapeInfo	shape;
		private Vector2D		pos;
		private Vector2D		normal;
		private double		fraction;

		/// <summary>
		/// [GET] The shape that was hit.
		/// </summary>
		public ShapeInfo Shape
		{
			get { return this.shape; }
		}
		/// <summary>
		/// [GET] The RigidBody that was hit.
		/// </summary>
		public RigidBody Body
		{
			get { return shape != null ? shape.Parent : null; }
		}
		/// <summary>
		/// [GET] The GameObject that was hit.
		/// </summary>
		public GameObject GameObj
		{
			get { return shape != null && shape.Parent != null ? shape.Parent.GameObj : null; }
		}
		/// <summary>
		/// [GET] The world position at which the shape was hit.
		/// </summary>
		public Vector2D Pos
		{
			get { return this.pos; }
		}
		/// <summary>
		/// [GET] The normal of the ray / shape collision.
		/// </summary>
		public Vector2D Normal
		{
			get { return this.normal; }
		}
		/// <summary>
		/// [GET] The fraction (0.0f - 1.0f) of the ray at which the hit occurred.
		/// </summary>
		public double Fraction
		{
			get { return this.fraction; }
		}

		public RayCastData(ShapeInfo shape, Vector2D pos, Vector2D normal, double fraction)
		{
			this.shape = shape;
			this.pos = pos;
			this.normal = normal;
			this.fraction = fraction;
		}
	}
}
