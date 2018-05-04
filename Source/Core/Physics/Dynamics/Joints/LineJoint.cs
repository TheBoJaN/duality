/*
* Farseer Physics Engine based on Box2D.XNA port:
* Copyright (c) 2010 Ian Qvist
* 
* Box2D.XNA port of Box2D:
* Copyright (c) 2009 Brandon Furtwangler, Nathan Furtwangler
*
* Original source Box2D:
* Copyright (c) 2006-2009 Erin Catto http://www.gphysics.com 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using System.Diagnostics;
using FarseerPhysics.Common;
using Duality;

namespace FarseerPhysics.Dynamics.Joints
{
	public class LineJoint : Joint
	{
		private Vector2D _ax, _ay;
		private double _bias;
		private bool _enableMotor;
		private double _gamma;
		private double _impulse;
		private Vector2D _localXAxis;
		private Vector2D _localYAxisA;
		private double _mass;
		private double _maxMotorTorque;
		private double _motorImpulse;
		private double _motorMass;
		private double _motorSpeed;

		private double _sAx;
		private double _sAy;
		private double _sBx;
		private double _sBy;

		private double _springImpulse;
		private double _springMass;

		// Linear constraint (point-to-line)
		// d = pB - pA = xB + rB - xA - rA
		// C = dot(ay, d)
		// Cdot = dot(d, cross(wA, ay)) + dot(ay, vB + cross(wB, rB) - vA - cross(wA, rA))
		//      = -dot(ay, vA) - dot(cross(d + rA, ay), wA) + dot(ay, vB) + dot(cross(rB, ay), vB)
		// J = [-ay, -cross(d + rA, ay), ay, cross(rB, ay)]

		// Spring linear constraint
		// C = dot(ax, d)
		// Cdot = = -dot(ax, vA) - dot(cross(d + rA, ax), wA) + dot(ax, vB) + dot(cross(rB, ax), vB)
		// J = [-ax -cross(d+rA, ax) ax cross(rB, ax)]

		// Motor rotational constraint
		// Cdot = wB - wA
		// J = [0 0 -1 0 0 1]

		internal LineJoint()
		{
			this.JointType = JointType.Line;
		}

		public LineJoint(Body bA, Body bB, Vector2D anchor, Vector2D axis)
			: base(bA, bB)
		{
			this.JointType = JointType.Line;

			this.LocalAnchorA = bA.GetLocalPoint(anchor);
			this.LocalAnchorB = bB.GetLocalPoint(anchor);
			this.LocalXAxis = bA.GetLocalVector(axis);
		}

		public Vector2D LocalAnchorA { get; set; }

		public Vector2D LocalAnchorB { get; set; }

		public override Vector2D WorldAnchorA
		{
			get { return this.BodyA.GetWorldPoint(this.LocalAnchorA); }
		}

		public override Vector2D WorldAnchorB
		{
			get { return this.BodyB.GetWorldPoint(this.LocalAnchorB); }
			set { Debug.Assert(false, "You can't set the world anchor on this joint type."); }
		}

		public double JointTranslation
		{
			get
			{
				Body bA = this.BodyA;
				Body bB = this.BodyB;

				Vector2D pA = bA.GetWorldPoint(this.LocalAnchorA);
				Vector2D pB = bB.GetWorldPoint(this.LocalAnchorB);
				Vector2D d = pB - pA;
				Vector2D axis = bA.GetWorldVector(this.LocalXAxis);

				double translation = Vector2D.Dot(d, axis);
				return translation;
			}
		}

		public double JointSpeed
		{
			get
			{
				double wA = this.BodyA.AngularVelocityInternal;
				double wB = this.BodyB.AngularVelocityInternal;
				return wB - wA;
			}
		}

		public bool MotorEnabled
		{
			get { return this._enableMotor; }
			set
			{
				this.BodyA.Awake = true;
				this.BodyB.Awake = true;
				this._enableMotor = value;
			}
		}

		public double MotorSpeed
		{
			set
			{
				this.BodyA.Awake = true;
				this.BodyB.Awake = true;
				this._motorSpeed = value;
			}
			get { return this._motorSpeed; }
		}

		public double MaxMotorTorque
		{
			set
			{
				this.BodyA.Awake = true;
				this.BodyB.Awake = true;
				this._maxMotorTorque = value;
			}
			get { return this._maxMotorTorque; }
		}

		public double Frequency { get; set; }

		public double DampingRatio { get; set; }

		public Vector2D LocalXAxis
		{
			get { return this._localXAxis; }
			set
			{
				this._localXAxis = value;
				this._localYAxisA = MathUtils.Cross(1.0f, this._localXAxis);
			}
		}

		public override Vector2D GetReactionForce(double invDt)
		{
			return invDt * (this._impulse * this._ay + this._springImpulse * this._ax);
		}

		public override double GetReactionTorque(double invDt)
		{
			return invDt * this._motorImpulse;
		}

		internal override void InitVelocityConstraints(ref TimeStep step)
		{
			Body bA = this.BodyA;
			Body bB = this.BodyB;

			this.LocalCenterA = bA.LocalCenter;
			this.LocalCenterB = bB.LocalCenter;

			Transform xfA;
			bA.GetTransform(out xfA);
			Transform xfB;
			bB.GetTransform(out xfB);

			// Compute the effective masses.
			Vector2D rA = MathUtils.Multiply(ref xfA.R, this.LocalAnchorA - this.LocalCenterA);
			Vector2D rB = MathUtils.Multiply(ref xfB.R, this.LocalAnchorB - this.LocalCenterB);
			Vector2D d = bB.Sweep.C + rB - bA.Sweep.C - rA;

			this.InvMassA = bA.InvMass;
			this.InvIA = bA.InvI;
			this.InvMassB = bB.InvMass;
			this.InvIB = bB.InvI;

			// Point to line constraint
			{
				this._ay = MathUtils.Multiply(ref xfA.R, this._localYAxisA);
				this._sAy = MathUtils.Cross(d + rA, this._ay);
				this._sBy = MathUtils.Cross(rB, this._ay);

				this._mass = this.InvMassA + this.InvMassB + this.InvIA * this._sAy * this._sAy + this.InvIB * this._sBy * this._sBy;

				if (this._mass > 0.0f)
				{
					this._mass = 1.0f / this._mass;
				}
			}

			// Spring constraint
			this._springMass = 0.0f;
			if (this.Frequency > 0.0f)
			{
				this._ax = MathUtils.Multiply(ref xfA.R, this.LocalXAxis);
				this._sAx = MathUtils.Cross(d + rA, this._ax);
				this._sBx = MathUtils.Cross(rB, this._ax);

				double invMass = this.InvMassA + this.InvMassB + this.InvIA * this._sAx * this._sAx + this.InvIB * this._sBx * this._sBx;

				if (invMass > 0.0f)
				{
					this._springMass = 1.0f / invMass;

					double C = Vector2D.Dot(d, this._ax);

					// Frequency
					double omega = 2.0f * Settings.Pi * this.Frequency;

					// Damping coefficient
					double da = 2.0f * this._springMass * this.DampingRatio * omega;

					// Spring stiffness
					double k = this._springMass * omega * omega;

					// magic formulas
					this._gamma = step.dt * (da + step.dt * k);
					if (this._gamma > 0.0f)
					{
						this._gamma = 1.0f / this._gamma;
					}

					this._bias = C * step.dt * k * this._gamma;

					this._springMass = invMass + this._gamma;
					if (this._springMass > 0.0f)
					{
						this._springMass = 1.0f / this._springMass;
					}
				}
			}
			else
			{
				this._springImpulse = 0.0f;
				this._springMass = 0.0f;
			}

			// Rotational motor
			if (this._enableMotor)
			{
				this._motorMass = this.InvIA + this.InvIB;
				if (this._motorMass > 0.0f)
				{
					this._motorMass = 1.0f / this._motorMass;
				}
			}
			else
			{
				this._motorMass = 0.0f;
				this._motorImpulse = 0.0f;
			}

#pragma warning disable 0162 // Unreachable code detected
			if (Settings.EnableWarmstarting)
			{
				// Account for variable time step.
				this._impulse *= step.dtRatio;
				this._springImpulse *= step.dtRatio;
				this._motorImpulse *= step.dtRatio;

				Vector2D P = this._impulse * this._ay + this._springImpulse * this._ax;
				double LA = this._impulse * this._sAy + this._springImpulse * this._sAx + this._motorImpulse;
				double LB = this._impulse * this._sBy + this._springImpulse * this._sBx + this._motorImpulse;

				bA.LinearVelocityInternal -= this.InvMassA * P;
				bA.AngularVelocityInternal -= this.InvIA * LA;

				bB.LinearVelocityInternal += this.InvMassB * P;
				bB.AngularVelocityInternal += this.InvIB * LB;
			}
			else
			{
				this._impulse = 0.0f;
				this._springImpulse = 0.0f;
				this._motorImpulse = 0.0f;
			}
#pragma warning restore 0162 // Unreachable code detected
		}

		internal override void SolveVelocityConstraints(ref TimeStep step)
		{
			Body bA = this.BodyA;
			Body bB = this.BodyB;

			Vector2D vA = bA.LinearVelocity;
			double wA = bA.AngularVelocityInternal;
			Vector2D vB = bB.LinearVelocityInternal;
			double wB = bB.AngularVelocityInternal;

			// Solve spring constraint
			{
				double Cdot = Vector2D.Dot(this._ax, vB - vA) + this._sBx * wB - this._sAx * wA;
				double impulse = -this._springMass * (Cdot + this._bias + this._gamma * this._springImpulse);
				this._springImpulse += impulse;

				Vector2D P = impulse * this._ax;
				double LA = impulse * this._sAx;
				double LB = impulse * this._sBx;

				vA -= this.InvMassA * P;
				wA -= this.InvIA * LA;

				vB += this.InvMassB * P;
				wB += this.InvIB * LB;
			}

			// Solve rotational motor constraint
			{
				double Cdot = wB - wA - this._motorSpeed;
				double impulse = -this._motorMass * Cdot;

				double oldImpulse = this._motorImpulse;
				double maxImpulse = step.dt * this._maxMotorTorque;
				this._motorImpulse = MathUtils.Clamp(this._motorImpulse + impulse, -maxImpulse, maxImpulse);
				impulse = this._motorImpulse - oldImpulse;

				wA -= this.InvIA * impulse;
				wB += this.InvIB * impulse;
			}

			// Solve point to line constraint
			{
				double Cdot = Vector2D.Dot(this._ay, vB - vA) + this._sBy * wB - this._sAy * wA;
				double impulse = this._mass * (-Cdot);
				this._impulse += impulse;

				Vector2D P = impulse * this._ay;
				double LA = impulse * this._sAy;
				double LB = impulse * this._sBy;

				vA -= this.InvMassA * P;
				wA -= this.InvIA * LA;

				vB += this.InvMassB * P;
				wB += this.InvIB * LB;
			}

			bA.LinearVelocityInternal = vA;
			bA.AngularVelocityInternal = wA;
			bB.LinearVelocityInternal = vB;
			bB.AngularVelocityInternal = wB;
		}

		internal override bool SolvePositionConstraints()
		{
			Body bA = this.BodyA;
			Body bB = this.BodyB;

			Vector2D xA = bA.Sweep.C;
			double angleA = bA.Sweep.A;

			Vector2D xB = bB.Sweep.C;
			double angleB = bB.Sweep.A;

			Mat22 RA = new Mat22(angleA);
			Mat22 RB = new Mat22(angleB);

			Vector2D rA = MathUtils.Multiply(ref RA, this.LocalAnchorA - this.LocalCenterA);
			Vector2D rB = MathUtils.Multiply(ref RB, this.LocalAnchorB - this.LocalCenterB);
			Vector2D d = xB + rB - xA - rA;

			Vector2D ay = MathUtils.Multiply(ref RA, this._localYAxisA);

			double sAy = MathUtils.Cross(d + rA, ay);
			double sBy = MathUtils.Cross(rB, ay);

			double C = Vector2D.Dot(d, ay);

			double k = this.InvMassA + this.InvMassB + this.InvIA * this._sAy * this._sAy + this.InvIB * this._sBy * this._sBy;

			double impulse;
			if (k != 0.0f)
			{
				impulse = -C / k;
			}
			else
			{
				impulse = 0.0f;
			}

			Vector2D P = impulse * ay;
			double LA = impulse * sAy;
			double LB = impulse * sBy;

			xA -= this.InvMassA * P;
			angleA -= this.InvIA * LA;
			xB += this.InvMassB * P;
			angleB += this.InvIB * LB;

			// TODO_ERIN remove need for this.
			bA.Sweep.C = xA;
			bA.Sweep.A = angleA;
			bB.Sweep.C = xB;
			bB.Sweep.A = angleB;
			bA.SynchronizeTransform();
			bB.SynchronizeTransform();

			return Math.Abs(C) <= Settings.LinearSlop;
		}

		public double GetMotorTorque(double invDt)
		{
			return invDt * this._motorImpulse;
		}
	}
}