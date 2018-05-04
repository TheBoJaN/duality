using System;
using System.Diagnostics;
using Duality;

namespace FarseerPhysics.Dynamics.Joints
{
	/// <summary>
	/// Maintains a fixed angle between two bodies
	/// </summary>
	public class AngleJoint : Joint
	{
		public double BiasFactor;
		public double MaxImpulse;
		public double Softness;
		private double _bias;
		private double _jointError;
		private double _massFactor;
		private double _targetAngle;

		internal AngleJoint()
		{
			this.JointType = JointType.Angle;
		}

		public AngleJoint(Body bodyA, Body bodyB)
			: base(bodyA, bodyB)
		{
			this.JointType = JointType.Angle;
			this.TargetAngle = 0;
			this.BiasFactor = .2f;
			this.Softness = 0f;
			this.MaxImpulse = double.MaxValue;
		}

		public double TargetAngle
		{
			get { return this._targetAngle; }
			set
			{
				if (value != this._targetAngle)
				{
					this._targetAngle = value;
					WakeBodies();
				}
			}
		}

		public override Vector2D WorldAnchorA
		{
			get { return this.BodyA.Position; }
		}

		public override Vector2D WorldAnchorB
		{
			get { return this.BodyB.Position; }
			set { Debug.Assert(false, "You can't set the world anchor on this joint type."); }
		}

		public override Vector2D GetReactionForce(double inv_dt)
		{
			//TODO
			//return _inv_dt * _impulse;
			return Vector2D.Zero;
		}

		public override double GetReactionTorque(double inv_dt)
		{
			return 0;
		}

		internal override void InitVelocityConstraints(ref TimeStep step)
		{
			this._jointError = (this.BodyB.Sweep.A - this.BodyA.Sweep.A - this.TargetAngle);

			this._bias = -this.BiasFactor * step.inv_dt * this._jointError;

			this._massFactor = (1 - this.Softness) / (this.BodyA.InvI + this.BodyB.InvI);
		}

		internal override void SolveVelocityConstraints(ref TimeStep step)
		{
			double p = (this._bias - this.BodyB.AngularVelocity + this.BodyA.AngularVelocity) * this._massFactor;
			this.BodyA.AngularVelocity -= this.BodyA.InvI * Math.Sign(p) * Math.Min(Math.Abs(p), this.MaxImpulse);
			this.BodyB.AngularVelocity += this.BodyB.InvI * Math.Sign(p) * Math.Min(Math.Abs(p), this.MaxImpulse);
		}

		internal override bool SolvePositionConstraints()
		{
			//no position solving for this joint
			return true;
		}
	}
}