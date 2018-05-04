using System;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;

using Duality.Editor;
using Duality.Resources;

namespace Duality.Components.Physics
{
	/// <summary>
	/// The line joint is also called "wheel joint", because it behaves like the spring of a car tire:
	/// A body is only allowed to travel on a specific world axis relative to the other one but can rotate
	/// freely or accelerated by a motor.
	/// </summary>
	public sealed class LineJointInfo : JointInfo
	{
		private	Vector2D		localAnchorA	= Vector2D.Zero;
		private	Vector2D		localAnchorB	= Vector2D.Zero;
		private	Vector2D		moveAxis		= Vector2D.UnitY;
		private	bool		motorEnabled	= false;
		private double		maxMotorTorque	= 0.0f;
		private double		motorSpeed		= 0.0f;
		private	double		dampingRatio	= 0.5f;
		private	double		frequency		= 5.0f;


		/// <summary>
		/// [GET / SET] The car RigidBodies local anchor point.
		/// </summary>
		[EditorHintIncrement(1)]
		public Vector2D CarAnchor
		{
			get { return this.localAnchorA; }
			set { this.localAnchorA = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The wheel RigidBodies local anchor point.
		/// </summary>
		[EditorHintIncrement(1)]
		public Vector2D WheelAnchor
		{
			get { return this.localAnchorB; }
			set { this.localAnchorB = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The axis on which the body may move.
		/// </summary>
		public Vector2D MovementAxis
		{
			get { return this.moveAxis; }
			set { this.moveAxis = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The damping ratio. Zero means "no damping", one means "critical damping".
		/// </summary>
		[EditorHintRange(0.0f, 1.0f)]
		public double DampingRatio
		{
			get { return this.dampingRatio; }
			set { this.dampingRatio = value; this.UpdateJoint(); }
		}
		/// <summary>
		/// [GET / SET] The mass spring damper frequency in hertz.
		/// </summary>
		[EditorHintRange(0.01f, 1000.0f)]
		public double Frequency
		{
			get { return this.frequency; }
			set { this.frequency = value; this.UpdateJoint(); }
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
			get { return this.joint == null ? 0.0f : (PhysicsUnit.AngularVelocityToDuality * (this.joint as LineJoint).JointSpeed); }
		}
		/// <summary>
		/// [GET] The current joint translation.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public double JointTranslation
		{
			get { return this.joint == null ? 0.0f : (PhysicsUnit.LengthToDuality * (this.joint as LineJoint).JointTranslation); }
		}
		/// <summary>
		/// [GET] The current joint motor torque.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public double MotorTorque
		{
			get { return this.joint == null ? 0.0f : (PhysicsUnit.TorqueToDuality * (this.joint as LineJoint).GetMotorTorque(1.0f)); }
		}


		protected override Joint CreateJoint(World world, Body bodyA, Body bodyB)
		{
			return bodyA != null && bodyB != null ? JointFactory.CreateLineJoint(world, bodyA, bodyB, Vector2D.Zero, Vector2D.UnitY) : null;
		}
		internal override void UpdateJoint()
		{
			base.UpdateJoint();
			if (this.joint == null) return;

			LineJoint j = this.joint as LineJoint;
			j.LocalAnchorB = GetFarseerPoint(this.OtherBody, this.localAnchorB);
			j.LocalAnchorA = GetFarseerPoint(this.ParentBody, this.localAnchorA);
			// Farseer gotcha: Setter is in world coordinates even though getter returns local coordinates.
			// Movement axis is relative to OtherBody, as that reflects Farseer behavior.
			j.LocalXAxis = this.OtherBody.GameObj.Transform.GetWorldVector(this.moveAxis).Normalized;
			j.MotorEnabled = this.motorEnabled;
			j.MotorSpeed = PhysicsUnit.AngularVelocityToPhysical * this.motorSpeed;
			j.MaxMotorTorque = PhysicsUnit.TorqueToPhysical * this.maxMotorTorque;
			j.DampingRatio = this.dampingRatio;
			j.Frequency = this.frequency;
		}
	}
}
