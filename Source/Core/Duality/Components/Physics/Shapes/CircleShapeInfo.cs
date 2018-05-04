using System;
using System.Collections.Generic;
using System.Linq;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;

using Duality.Editor;

namespace Duality.Components.Physics
{
	/// <summary>
	/// Describes a <see cref="RigidBody">Colliders</see> circle shape.
	/// </summary>
	public sealed class CircleShapeInfo : ShapeInfo
	{
		[DontSerialize]
		private Fixture fixture;
		private double   radius;
		private Vector2D position;


		/// <summary>
		/// [GET / SET] The circles radius.
		/// </summary>
		[EditorHintIncrement(1)]
		[EditorHintDecimalPlaces(1)]
		public double Radius
		{
			get { return this.radius; }
			set { this.radius = value; this.UpdateInternalShape(true); }
		}
		/// <summary>
		/// [GET / SET] The circles position.
		/// </summary>
		[EditorHintIncrement(1)]
		[EditorHintDecimalPlaces(1)]
		public Vector2D Position
		{
			get { return this.position; }
			set { this.position = value; this.UpdateInternalShape(true); }
		}
		public override RectD AABB
		{
			get { return RectD.Align(Alignment.Center, this.position.X, this.position.Y, this.radius * 2, this.radius * 2); }
		}
		protected override bool IsInternalShapeCreated
		{
			get { return this.fixture != null; }
		}


		public CircleShapeInfo() {}
		public CircleShapeInfo(double radius, Vector2D position, double density)
		{
			this.radius = radius;
			this.position = position;
			this.density = density;
		}

		protected override void DestroyFixtures()
		{
			if (this.fixture == null) return;
			if (this.fixture.Body != null)
				this.fixture.Body.DestroyFixture(this.fixture);
			this.fixture = null;
		}
		protected override void SyncFixtures()
		{
			if (!this.EnsureFixtures()) return;

			this.fixture.IsSensor = this.sensor;
			this.fixture.Restitution = this.restitution;
			this.fixture.Friction = this.friction;

			CircleShape circle = this.fixture.Shape as CircleShape;
			circle.Density = this.density * PhysicsUnit.DensityToPhysical / (10.0f * 10.0f);
		}

		private bool EnsureFixtures()
		{
			if (this.fixture == null)
			{
				Body body = this.Parent.PhysicsBody;
				if (body != null)
				{
					double scale = this.ParentScale;
					CircleShape circle = new CircleShape(
						PhysicsUnit.LengthToPhysical * this.radius * scale, 
						this.density);
					circle.Position = PhysicsUnit.LengthToPhysical * this.position * scale;

					this.fixture = new Fixture(
						body, 
						circle, 
						this);
				}
			}

			return this.fixture != null;
		}
	}
}
