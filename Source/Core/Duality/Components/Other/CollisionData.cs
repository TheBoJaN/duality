using System;
using System.Collections.Generic;
using System.Linq;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace Duality
{
	/// <summary>
	/// Provides detailed information about a collision event.
	/// </summary>
	public class CollisionData
	{
		private	Vector2D pos;
		private	Vector2D	normal;
		private	double	normalImpulse;
		private	double	normalMass;
		private	double	tangentImpulse;
		private	double	tangentMass;

		/// <summary>
		/// [GET] The position at which the collision occurred in absolute world coordinates.
		/// </summary>
		public Vector2D Pos
		{
			get { return this.pos; }
		}
		/// <summary>
		/// [GET] The normal vector of the collision impulse, in the global coordinate system.
		/// </summary>
		public Vector2D Normal
		{
			get { return this.normal; }
		}
		/// <summary>
		/// [GET] The impulse that is delivered along the provided normal vector.
		/// </summary>
		public double NormalImpulse
		{
			get { return this.normalImpulse; }
		}
		/// <summary>
		/// [GET] The mass that is interacting along the provided normal vector.
		/// </summary>
		public double NormalMass
		{
			get { return this.normalMass; }
		}
		/// <summary>
		/// [GET] The speed change that will occur when applying <see cref="NormalImpulse"/> to <see cref="NormalMass"/>.
		/// </summary>
		public double NormalSpeed
		{
			get { return this.normalImpulse / this.normalMass; }
		}
		/// <summary>
		/// [GET] The tangent vector of the collision impulse, in the global coordinate system.
		/// </summary>
		public Vector2D Tangent
		{
			get { return this.normal.PerpendicularRight; }
		}
		/// <summary>
		/// [GET] The impulse that is delivered along the provided tangent vector.
		/// </summary>
		public double TangentImpulse
		{
			get { return this.tangentImpulse; }
		}
		/// <summary>
		/// [GET] The mass that is interacting along the provided tangent vector.
		/// </summary>
		public double TangentMass
		{
			get { return this.tangentMass; }
		}
		/// <summary>
		/// [GET] The speed change that will occur when applying <see cref="TangentImpulse"/> to <see cref="TangentMass"/>.
		/// </summary>
		public double TangentSpeed
		{
			get { return this.tangentImpulse / this.tangentMass; }
		}

		public CollisionData(Vector2D pos, Vector2D normal, double normalImpulse, double tangentImpulse, double normalMass, double tangentMass)
		{
			this.pos = pos;
			this.normal = normal;
			this.normalImpulse = normalImpulse;
			this.tangentImpulse = tangentImpulse;
			this.normalMass = normalMass;
			this.tangentMass = tangentMass;
		}
		internal CollisionData(Body localBody, ContactConstraint impulse, int pointIndex)
		{
			if (localBody == impulse.BodyA)
			{
				this.pos = PhysicsUnit.LengthToDuality * (impulse.Points[pointIndex].rA + impulse.BodyA.WorldCenter);
				this.normal = impulse.Normal;
				this.normalImpulse = PhysicsUnit.ImpulseToDuality * impulse.Points[pointIndex].NormalImpulse;
				this.tangentImpulse = PhysicsUnit.ImpulseToDuality * impulse.Points[pointIndex].TangentImpulse;
				this.normalMass = PhysicsUnit.MassToDuality * impulse.Points[pointIndex].NormalMass;
				this.tangentMass = PhysicsUnit.MassToDuality * impulse.Points[pointIndex].TangentMass;
			}
			else if (localBody == impulse.BodyB)
			{
				this.pos = PhysicsUnit.LengthToDuality * (impulse.Points[pointIndex].rB + impulse.BodyB.WorldCenter);
				this.normal = -impulse.Normal;
				this.normalImpulse = PhysicsUnit.ImpulseToDuality * impulse.Points[pointIndex].NormalImpulse;
				this.tangentImpulse = PhysicsUnit.ImpulseToDuality * impulse.Points[pointIndex].TangentImpulse;
				this.normalMass = PhysicsUnit.MassToDuality * impulse.Points[pointIndex].NormalMass;
				this.tangentMass = PhysicsUnit.MassToDuality * impulse.Points[pointIndex].TangentMass;
			}
			else
				throw new ArgumentException("Local body is not part of the collision", "localBody");
		}
	}
}
