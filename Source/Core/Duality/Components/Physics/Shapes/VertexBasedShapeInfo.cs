using System;
using System.Collections.Generic;
using System.Linq;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;

using Duality.Editor;

namespace Duality.Components.Physics
{
	public abstract class VertexBasedShapeInfo : ShapeInfo
	{
		protected static readonly Vector2D[] EmptyVertices = new Vector2D[0];

		protected Vector2D[] vertices;


		/// <summary>
		/// [GET / SET] The vertices that describe this shape. 
		/// While assinging the array will cause an automatic update, simply 
		/// modifying it will require you to call <see cref="ShapeInfo.UpdateShape"/> manually.
		/// </summary>
		[EditorHintFlags(MemberFlags.ForceWriteback)]
		[EditorHintIncrement(1)]
		[EditorHintDecimalPlaces(1)]
		public Vector2D[] Vertices
		{
			get { return this.vertices ?? EmptyVertices; }
			set
			{
				this.vertices = value ?? new Vector2D[] { Vector2D.Zero, Vector2D.UnitX, Vector2D.UnitY };
				this.UpdateInternalShape(true);
			}
		}
		/// <summary>
		/// [GET] A flagged enum describing traits of the geometry that is formed by this shapes <see cref="Vertices"/>.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public abstract VertexShapeTrait ShapeTraits { get; }
		[EditorHintFlags(MemberFlags.Invisible)]
		public override RectD AABB
		{
			get 
			{
				if (this.vertices == null || this.vertices.Length == 0)
					return Rect.Empty;

				double minX = double.MaxValue;
				double minY = double.MaxValue;
				double maxX = double.MinValue;
				double maxY = double.MinValue;
				for (int i = 0; i < this.vertices.Length; i++)
				{
					minX = MathD.Min(minX, this.vertices[i].X);
					minY = MathD.Min(minY, this.vertices[i].Y);
					maxX = MathD.Max(maxX, this.vertices[i].X);
					maxY = MathD.Max(maxY, this.vertices[i].Y);
				}
				return new RectD(minX, minY, maxX - minX, maxY - minY);
			}
		}


		protected VertexBasedShapeInfo() { }
		protected VertexBasedShapeInfo(Vector2D[] vertices)
		{
			this.vertices = vertices;
		}
	}
}
