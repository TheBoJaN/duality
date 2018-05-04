using System;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;

using Duality.Editor;
using Duality.Resources;

namespace Duality.Components.Physics
{
	/// <summary>
	/// Pins two locally anchored RigidBodies together without constraining rotation.
	/// </summary>
	public sealed class RevoluteJointInfo : JointInfo
	{
		private	Vector2D		localAnchorA	= Vector2D.Zero;
		private	Vector2D		localAnchorB	= Vector2D.Zero;
		private	double		lowerLimit		= 0.0f;
		private	double		upperLimit		= 0.0f;
		private	bool		limitEnabled	= false;
		private	double		refAngle		= 0.0f;
		private	bool		motorEnabled	= false;
		private double		maxMotorTorque	= 0.0f;
		private double		motorSpeed		= 0.0f;


		/// <summary>
		/// [GET / SET] The first RigidBodies local anchor point.
		/// </summary>
		[EditorHintIncrement(1)]
		public Vector2D LocalAnchorA
		{
			get { return this.localAnchorA; }
			set { this.localAnchorA = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The second RigidBodies local anchor point.
		/// </summary>
		[EditorHintIncrement(1)]
		public Vector2D LocalAnchorB
		{
			get { return this.localAnchorB; }
			set { this.localAnchorB = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] Is the joint limited in its angle?
		/// </summary>
		public bool LimitEnabled
		{
			get { return this.limitEnabled; }
			set { this.limitEnabled = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The lower joint limit in radians.
		/// </summary>
		[EditorHintIncrement(MathD.RadAngle1)]
		public double LowerLimit
		{
			get { return this.lowerLimit; }
			set { this.lowerLimit = MathD.Min(value, this.upperLimit); this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The upper joint limit in radians.
		/// </summary>
		[EditorHintIncrement(MathD.RadAngle1)]
		public double UpperLimit
		{
			get { return this.upperLimit; }
			set { this.upperLimit = MathD.Max(value, this.lowerLimit); this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The joint's reference angle.
		/// </summary>
		[EditorHintIncrement(MathD.RadAngle1)]
		public double ReferenceAngle
		{
			get { return this.refAngle; }
			set { this.refAngle = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] Is the joint motor enabled?
		/// </summary>
		public bool MotorEnabled
		{
			get { return this.motorEnabled; }
			set { this.motorEnabled = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The maximum motor torque.
		/// </summary>
		[EditorHintIncrement(10.0f)]
		[EditorHintDecimalPlaces(0)]
		[EditorHintRange(0, 100000, 0, 5000)]
		public double MaxMotorTorque
		{
			get { return this.maxMotorTorque; }
			set { this.maxMotorTorque = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The desired motor speed in radians per frame.
		/// </summary>
		[EditorHintIncrement(MathD.RadAngle1)]
		public double MotorSpeed
		{
			get { return this.motorSpeed; }
			set { this.motorSpeed = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET] The current joint angle speed in radians per frame.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public double JointSpeed
		{
			get { return this.joint == null ? 0.0f : (PhysicsUnit.AngularVelocityToDuality * (this.joint as RevoluteJoint).JointSpeed); }
		}
		/// <summary>
		/// [GET] The current joint angle in radians.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public double JointAngle
		{
			get { return this.joint == null ? 0.0f : (PhysicsUnit.AngleToDuality * (this.joint as RevoluteJoint).JointAngle); }
		}
		/// <summary>
		/// [GET] The current joint motor torque.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public double MotorTorque
		{
			get { return this.joint == null ? 0.0f : (PhysicsUnit.TorqueToDuality * (this.joint as RevoluteJoint).MotorTorque); }
		}


		protected override Joint CreateJoint(World world, Body bodyA, Body bodyB)
		{
			return bodyA != null && bodyB != null ? JointFactory.CreateRevoluteJoint(world, bodyA, bodyB, Vector2D.Zero) : null;
		}
		internal override void UpdateJoint()
		{
			base.UpdateJoint();
			if (this.joint == null) return;

			RevoluteJoint j = this.joint as RevoluteJoint;
			j.LocalAnchorB = GetFarseerPoint(this.OtherBody, this.localAnchorB);
			j.LocalAnchorA = GetFarseerPoint(this.ParentBody, this.localAnchorA);
			j.MotorEnabled = this.motorEnabled;
			j.MotorSpeed = PhysicsUnit.AngularVelocityToPhysical * -this.motorSpeed;
			j.MaxMotorTorque = PhysicsUnit.TorqueToPhysical * this.maxMotorTorque;
			j.LimitEnabled = this.limitEnabled;
			j.LowerLimit = -this.upperLimit;
			j.UpperLimit = -this.lowerLimit;
			j.ReferenceAngle = -this.refAngle;
		}
	}
}
