﻿using System;

using Duality.Editor;
using Duality.Properties;
using Duality.Cloning;

namespace Duality.Components
{
	/// <summary>
	/// Keeps track of this objects linear and angular velocity by accumulating all
	/// movement (but not teleportation) of its <see cref="Transform"/> component.
	/// </summary>
	[ManuallyCloned]
	[EditorHintCategory(CoreResNames.CategoryNone)]
	[EditorHintImage(CoreResNames.ImageVelocityTracker)]
	[RequiredComponent(typeof(Transform))]
	public sealed class VelocityTracker : Component, ICmpUpdatable, ICmpSerializeListener
	{
		[DontSerialize] private Vector3D velocity      = Vector3D.Zero;
		[DontSerialize] private double   angleVelocity = 0.0f;
		[DontSerialize] private Vector3D posDiff       = Vector3D.Zero;
		[DontSerialize] private double   angleDiff     = 0.0f;
		[DontSerialize] private Vector3D lastPosition  = Vector3D.Zero;
		[DontSerialize] private double   lastAngle     = 0.0f;


		/// <summary>
		/// [GET] The objects measured velocity in world space. The value is internally smoothed
		/// over several frames to filter out fluctuations due to framerate variations.
		/// </summary>
		public Vector3D Vel
		{
			get { return this.velocity; }
		}
		/// <summary>
		/// [GET] The objects measured angle / rotation velocity in world space, in radians.
		/// The value is internally smoothed over several frames to filter out fluctuations due
		/// to framerate variations.
		/// </summary>
		public double AngleVel
		{
			get { return this.angleVelocity; }
		}
		/// <summary>
		/// [GET] The objects measured continuous position change in world space between the last two frames.
		/// Note that this value can fluctuate depending on framerate variations during simulation.
		/// </summary>
		public Vector3D LastMovement
		{
			get { return this.posDiff; }
		}
		/// <summary>
		/// [GET] The objects measuredcontinuous angle / rotation change in world space between the last two frames.
		/// Note that this value can fluctuate depending on framerate variations during simulation.
		/// </summary>
		public double LastAngleMovement
		{
			get { return this.angleDiff; }
		}


		/// <summary>
		/// Resets the objects velocity value for next frame to zero, assuming the
		/// specified world space position as a basis for further movement.
		/// </summary>
		/// <param name="worldPos"></param>
		public void ResetVelocity(Vector3D worldPos)
		{
			this.lastPosition = worldPos;
		}
		/// <summary>
		/// Resets the objects angle velocity value for next frame to zero, assuming the
		/// specified world space angle as a basis for further rotation.
		/// </summary>
		/// <param name="worldAngle"></param>
		public void ResetAngleVelocity(double worldAngle)
		{
			this.lastAngle = worldAngle;
		}
		
		void ICmpUpdatable.OnUpdate()
		{
			// Calculate velocity values from last frames movement
			if (MathD.Abs(Time.TimeMult) > double.Epsilon)
			{
				Transform transform = this.GameObj.Transform;
				Vector3D pos = transform.Pos;
				double angle = transform.Angle;

				this.posDiff = pos - this.lastPosition;
				this.angleDiff = MathD.TurnDir(this.lastAngle, angle) * MathD.CircularDist(this.lastAngle, angle);

				Vector3D lastVelocity = this.posDiff / Time.TimeMult;
				double lastAngleVelocity = this.angleDiff / Time.TimeMult;

				this.velocity += (lastVelocity - this.velocity) * 0.25f * Time.TimeMult;
				this.angleVelocity += (lastAngleVelocity - this.angleVelocity) * 0.25f * Time.TimeMult;
				this.lastPosition = pos;
				this.lastAngle = angle;
			}
		}
		void ICmpSerializeListener.OnLoaded()
		{
			Transform transform = this.GameObj.Transform;
			this.lastPosition = transform.Pos;
			this.lastAngle = transform.Angle;
		}
		void ICmpSerializeListener.OnSaved() { }
		void ICmpSerializeListener.OnSaving() { }

		protected override void OnCopyDataTo(object targetObj, ICloneOperation operation)
		{
			base.OnCopyDataTo(targetObj, operation);
			VelocityTracker target = targetObj as VelocityTracker;
			target.lastPosition   = this.lastPosition;
			target.lastAngle      = this.lastAngle;
			target.posDiff        = this.posDiff;
			target.angleDiff      = this.angleDiff;
			target.velocity       = this.velocity;
			target.angleVelocity  = this.angleVelocity;
		}
	}
}
