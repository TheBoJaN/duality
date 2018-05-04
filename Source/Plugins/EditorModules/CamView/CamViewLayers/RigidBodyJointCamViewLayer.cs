using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using Duality;
using Duality.Drawing;
using Duality.Resources;
using Duality.Components.Physics;
using Duality.Editor;
using Duality.Editor.Forms;

namespace Duality.Editor.Plugins.CamView.CamViewLayers
{
	public class RigidBodyJointCamViewLayer : CamViewLayer
	{
		private double anchorScale = 3.0f;
		private double lineCapScale = 7.0f;
		private double minAngleConstraintRadius = 20.0f;
		private double defaultLineWidth = 2.0f;
		private double depthOffset = -1.0f;

		public override string LayerName
		{
			get { return Properties.CamViewRes.CamViewLayer_RigidBodyJoint_Name; }
		}
		public override string LayerDesc
		{
			get { return Properties.CamViewRes.CamViewLayer_RigidBodyJoint_Desc; }
		}
		public ColorRgba MotorColor
		{
			get
			{
				double fgLum = this.FgColor.GetLuminance();
				if (fgLum > 0.5f)
					return new ColorRgba(144, 192, 240);
				else
					return new ColorRgba(16, 32, 96);
			}
		}
		public ColorRgba JointColor
		{
			get
			{
				double fgLum = this.FgColor.GetLuminance();
				if (fgLum > 0.5f)
					return new ColorRgba(176, 240, 112);
				else
					return new ColorRgba(16, 96, 0);
			}
		}
		public ColorRgba JointErrorColor
		{
			get
			{
				double fgLum = this.FgColor.GetLuminance();
				if (fgLum > 0.5f)
					return new ColorRgba(240, 144, 112);
				else
					return new ColorRgba(96, 8, 0);
			}
		}

		protected internal override void OnCollectWorldOverlayDrawcalls(Canvas canvas)
		{
			base.OnCollectWorldOverlayDrawcalls(canvas);
			canvas.State.DepthOffset = (float)this.depthOffset;
			canvas.State.SetMaterial(DrawTechnique.Alpha);

			RigidBody selectedBody = this.QuerySelectedCollider();
			bool isAnyBodySelected = (selectedBody != null);

			List<RigidBody> visibleColliders = this.QueryVisibleColliders().ToList();
			List<JointInfo> visibleJoints = new List<JointInfo>();
			foreach (RigidBody body in visibleColliders)
			{
				if (body.Joints == null) continue;
				visibleJoints.AddRange(body.Joints.Where(j => !visibleJoints.Contains(j)));
			}
			foreach (JointInfo joint in visibleJoints)
			{
				bool isBodySelected = selectedBody != null && (joint.ParentBody == selectedBody || joint.OtherBody == selectedBody);
				double jointAlpha = isBodySelected || !isAnyBodySelected ? 1.0f : 0.5f;
				if (!joint.Enabled) jointAlpha *= 0.25f;
				canvas.State.ColorTint = canvas.State.ColorTint.WithAlpha(jointAlpha);

				if (joint.ParentBody == null) continue;
				if (joint.OtherBody == null) continue;
				this.DrawJoint(canvas, joint);
			}
		}

		private void DrawJoint(Canvas canvas, JointInfo joint)
		{
			if (joint.ParentBody == null) return;
			if (joint.OtherBody == null) return;

			if (joint is AngleJointInfo)				this.DrawJoint(canvas, joint as AngleJointInfo);
			else if (joint is DistanceJointInfo)		this.DrawJoint(canvas, joint as DistanceJointInfo);
			else if (joint is FrictionJointInfo)		this.DrawJoint(canvas, joint as FrictionJointInfo);
			else if (joint is RevoluteJointInfo)		this.DrawJoint(canvas, joint as RevoluteJointInfo);
			else if (joint is PrismaticJointInfo)		this.DrawJoint(canvas, joint as PrismaticJointInfo);
			else if (joint is WeldJointInfo)			this.DrawJoint(canvas, joint as WeldJointInfo);
			else if (joint is RopeJointInfo)			this.DrawJoint(canvas, joint as RopeJointInfo);
			else if (joint is LineJointInfo)			this.DrawJoint(canvas, joint as LineJointInfo);
			else if (joint is PulleyJointInfo)			this.DrawJoint(canvas, joint as PulleyJointInfo);
		}
		private void DrawJoint(Canvas canvas, AngleJointInfo joint)
		{
			this.DrawLocalAngleConstraint(canvas, 
				joint.ParentBody, 
				Vector2.Zero, 
				joint.OtherBody.GameObj.Transform.Angle - joint.TargetAngle, 
				(double)joint.ParentBody.GameObj.Transform.Angle, 
				joint.ParentBody.BoundRadius);
			this.DrawLocalAngleConstraint(canvas, 
				joint.OtherBody, 
				Vector2.Zero, 
				joint.ParentBody.GameObj.Transform.Angle + joint.TargetAngle, 
				joint.OtherBody.GameObj.Transform.Angle, 
				joint.OtherBody.BoundRadius);
			this.DrawLocalLooseConstraint(canvas, joint.ParentBody, joint.OtherBody, Vector2.Zero, Vector2.Zero);
		}
		private void DrawJoint(Canvas canvas, DistanceJointInfo joint)
		{
			this.DrawLocalDistConstraint(canvas, joint.ParentBody, joint.OtherBody, joint.LocalAnchorA, joint.LocalAnchorB, joint.TargetDistance, joint.TargetDistance);
			this.DrawLocalAnchor(canvas, joint.OtherBody, joint.LocalAnchorB);
			this.DrawLocalAnchor(canvas, joint.ParentBody, joint.LocalAnchorA);
		}
		private void DrawJoint(Canvas canvas, FrictionJointInfo joint)
		{
			this.DrawLocalFrictionMarker(canvas, joint.ParentBody, joint.LocalAnchorA);
			this.DrawLocalFrictionMarker(canvas, joint.OtherBody, joint.LocalAnchorB);
			this.DrawLocalLooseConstraint(canvas, joint.ParentBody, joint.OtherBody, joint.LocalAnchorA, joint.LocalAnchorB);
		}
		private void DrawJoint(Canvas canvas, RevoluteJointInfo joint)
		{
			double screenScale = this.GetScreenScale(canvas);
			double anchorDist = this.GetAnchorDist(joint.ParentBody, joint.OtherBody, joint.LocalAnchorA, joint.LocalAnchorB);
			double screenAnchorDist = screenScale * anchorDist;
			double angularCircleRadA = joint.ParentBody.BoundRadius * 0.25f;
			double angularCircleRadB = joint.OtherBody.BoundRadius * 0.25f;
			bool displaySecondAngle = screenAnchorDist >= 2.0f * MathD.Max(
				screenScale * angularCircleRadA + angularCircleRadB, 
				2.0f * this.minAngleConstraintRadius);

			this.DrawLocalPosConstraint(canvas, joint.ParentBody, joint.OtherBody, joint.LocalAnchorA, joint.LocalAnchorB);
			
			if (joint.LimitEnabled)
			{
				this.DrawLocalAngleConstraint(canvas, 
					joint.ParentBody, 
					joint.LocalAnchorA, 
					joint.OtherBody.GameObj.Transform.Angle - joint.ReferenceAngle, 
					joint.ParentBody.GameObj.Transform.Angle, 
					angularCircleRadA);
				if (displaySecondAngle)
				{
					this.DrawLocalAngleConstraint(canvas, 
						joint.OtherBody, 
						joint.LocalAnchorB, 
						joint.ParentBody.GameObj.Transform.Angle + joint.ReferenceAngle,
						joint.OtherBody.GameObj.Transform.Angle, 
						angularCircleRadB);
				}
			}

			if (joint.MotorEnabled)
			{
				this.DrawLocalAngleMotor(canvas, joint.ParentBody, Vector2.Zero, joint.MotorSpeed, joint.MaxMotorTorque, joint.ParentBody.BoundRadius * 1.15f);
			}

			this.DrawLocalAnchor(canvas, joint.ParentBody, joint.LocalAnchorA);
			this.DrawLocalAnchor(canvas, joint.OtherBody, joint.LocalAnchorB);
		}
		private void DrawJoint(Canvas canvas, PrismaticJointInfo joint)
		{
			double screenScale = this.GetScreenScale(canvas);
			double screenDist = screenScale * (joint.ParentBody.GameObj.Transform.Pos - joint.OtherBody.GameObj.Transform.Pos).Length;
			double angularCircleRadA = joint.ParentBody.BoundRadius * 0.25f;
			double angularCircleRadB = joint.OtherBody.BoundRadius * 0.25f;
			bool displaySecondAngle = screenDist >= 2.0f * MathD.Max(
				screenScale * angularCircleRadA + angularCircleRadB, 
				2.0f * this.minAngleConstraintRadius);

			if (joint.LimitEnabled)
			    this.DrawLocalAxisConstraint(canvas, joint.ParentBody, joint.OtherBody, joint.MovementAxis, joint.LocalAnchorA, joint.LocalAnchorB, joint.LowerLimit, joint.UpperLimit);
			else
			    this.DrawLocalAxisConstraint(canvas, joint.ParentBody, joint.OtherBody, joint.MovementAxis, joint.LocalAnchorA, joint.LocalAnchorB);

			this.DrawLocalAngleConstraint(canvas, 
				joint.ParentBody, 
				joint.LocalAnchorA, 
				joint.OtherBody.GameObj.Transform.Angle - joint.ReferenceAngle, 
				joint.ParentBody.GameObj.Transform.Angle, 
				angularCircleRadA);
			if (displaySecondAngle)
			{
				this.DrawLocalAngleConstraint(canvas, 
					joint.OtherBody, 
					joint.LocalAnchorB, 
					joint.ParentBody.GameObj.Transform.Angle + joint.ReferenceAngle,
					joint.OtherBody.GameObj.Transform.Angle, 
					angularCircleRadB);
			}

			if (joint.MotorEnabled)
			    this.DrawLocalAxisMotor(canvas, joint.ParentBody, joint.OtherBody, joint.MovementAxis, joint.LocalAnchorA, joint.LocalAnchorB, joint.MotorSpeed, joint.MaxMotorForce, joint.ParentBody.BoundRadius * 1.15f);

			this.DrawLocalAnchor(canvas, joint.ParentBody, joint.LocalAnchorA);
			this.DrawLocalAnchor(canvas, joint.OtherBody, joint.LocalAnchorB);
		}
		private void DrawJoint(Canvas canvas, WeldJointInfo joint)
		{
			double screenScale = this.GetScreenScale(canvas);
			double anchorDist = this.GetAnchorDist(joint.ParentBody, joint.OtherBody, joint.LocalAnchorA, joint.LocalAnchorB);
			double screenAnchorDist = screenScale * anchorDist;
			double angularCircleRadA = joint.ParentBody.BoundRadius * 0.25f;
			double angularCircleRadB = joint.OtherBody.BoundRadius * 0.25f;
			bool displaySecondAngle = screenAnchorDist >= 2.0f * MathD.Max(
				screenScale * angularCircleRadA + angularCircleRadB, 
				2.0f * this.minAngleConstraintRadius);

			this.DrawLocalPosConstraint(canvas, joint.ParentBody, joint.OtherBody, joint.LocalAnchorA, joint.LocalAnchorB);

			this.DrawLocalAnchor(canvas, joint.ParentBody, joint.LocalAnchorA);
			this.DrawLocalAnchor(canvas, joint.OtherBody, joint.LocalAnchorB);

			this.DrawLocalAngleConstraint(canvas, 
				joint.ParentBody, 
				joint.LocalAnchorA, 
				joint.OtherBody.GameObj.Transform.Angle - joint.RefAngle, 
				joint.ParentBody.GameObj.Transform.Angle, 
				angularCircleRadA);
			if (displaySecondAngle)
			{
				this.DrawLocalAngleConstraint(canvas, 
					joint.OtherBody, 
					joint.LocalAnchorB, 
					joint.ParentBody.GameObj.Transform.Angle + joint.RefAngle,
					joint.OtherBody.GameObj.Transform.Angle, 
					angularCircleRadB);
			}
		}
		private void DrawJoint(Canvas canvas, RopeJointInfo joint)
		{
			this.DrawLocalDistConstraint(canvas, joint.ParentBody, joint.OtherBody, joint.LocalAnchorA, joint.LocalAnchorB, 0.0f, joint.MaxLength);
			this.DrawLocalAnchor(canvas, joint.OtherBody, joint.LocalAnchorB);
			this.DrawLocalAnchor(canvas, joint.ParentBody, joint.LocalAnchorA);
		}
		private void DrawJoint(Canvas canvas, LineJointInfo joint)
		{
			Vector2 anchorAToWorld = joint.ParentBody.GameObj.Transform.GetWorldPoint(joint.CarAnchor);

			this.DrawWorldPosConstraint(canvas, joint.ParentBody, joint.CarAnchor, anchorAToWorld);
			this.DrawLocalAxisConstraint(canvas, joint.OtherBody, joint.ParentBody, joint.MovementAxis, joint.WheelAnchor, joint.CarAnchor, -joint.OtherBody.BoundRadius * 0.25f, joint.OtherBody.BoundRadius * 0.25f);
			this.DrawLocalAnchor(canvas, joint.ParentBody, joint.CarAnchor);
			this.DrawLocalAnchor(canvas, joint.OtherBody, joint.WheelAnchor);

			if (joint.MotorEnabled)
			{
				this.DrawLocalAngleMotor(canvas, joint.OtherBody, Vector2.Zero, joint.MotorSpeed, joint.MaxMotorTorque, joint.OtherBody.BoundRadius * 1.15f);
			}
		}
		private void DrawJoint(Canvas canvas, PulleyJointInfo joint)
		{
			double maxLenA = MathD.Min(joint.MaxLengthA, joint.TotalLength - (joint.Ratio * joint.LengthB));
			double maxLenB = MathD.Min(joint.MaxLengthB, joint.Ratio * (joint.TotalLength - joint.LengthA));

			this.DrawWorldDistConstraint(canvas, joint.ParentBody, joint.LocalAnchorA, joint.WorldAnchorA, 0.0f, maxLenA);
			this.DrawWorldDistConstraint(canvas, joint.OtherBody, joint.LocalAnchorB, joint.WorldAnchorB, 0.0f, maxLenB);
			this.DrawWorldLooseConstraint(canvas, joint.ParentBody, joint.WorldAnchorA, joint.WorldAnchorB);
			this.DrawLocalAnchor(canvas, joint.OtherBody, joint.LocalAnchorB);
			this.DrawLocalAnchor(canvas, joint.ParentBody, joint.LocalAnchorA);
		}
		
		private void DrawLocalAnchor(Canvas canvas, RigidBody body, Vector2 anchor)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3 colliderPos = body.GameObj.Transform.Pos;

			double markerCircleRad = this.GetScreenConstantScale(canvas, this.anchorScale);
			Vector2 anchorToWorld = body.GameObj.Transform.GetWorldVector(anchor);

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillCircle(
				colliderPos.X + anchorToWorld.X,
				colliderPos.Y + anchorToWorld.Y,
				0.0f,
				markerCircleRad);
			canvas.State.ColorTint = baseColor;
		}
		private void DrawLocalFrictionMarker(Canvas canvas, RigidBody body, Vector2 anchor)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3 colliderPos = body.GameObj.Transform.Pos;

			double markerCircleRad = this.GetScreenConstantScale(canvas, this.anchorScale);
			Vector2 anchorToWorld = body.GameObj.Transform.GetWorldVector(anchor);

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillCircle(
				colliderPos.X + anchorToWorld.X,
				colliderPos.Y + anchorToWorld.Y,
				0.0f,
				markerCircleRad * 0.5f);
			canvas.DrawCircle(
				colliderPos.X + anchorToWorld.X,
				colliderPos.Y + anchorToWorld.Y,
				0.0f,
				markerCircleRad);
			canvas.DrawCircle(
				colliderPos.X + anchorToWorld.X,
				colliderPos.Y + anchorToWorld.Y,
				0.0f,
				markerCircleRad * 1.5f);
			canvas.State.ColorTint = baseColor;
		}
		private void DrawLocalAngleConstraint(Canvas canvas, RigidBody body, Vector2D anchor, double targetAngle, double currentAngle, double radius)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3 bodyPos = body.GameObj.Transform.Pos;

			radius = this.GetScreenMinScale(canvas, radius, this.minAngleConstraintRadius);

			double lineWidth = this.GetScreenConstantScale(canvas, this.defaultLineWidth);
			Vector2D anchorToWorld = body.GameObj.Transform.GetWorldVector(anchor);
			Vector2D angleVec = Vector2D.FromAngleLength(targetAngle, radius);
			Vector2D errorVec = Vector2D.FromAngleLength(currentAngle, radius);
			bool hasError = MathD.CircularDist(targetAngle, currentAngle) >= MathD.RadAngle1;

			if (hasError)
			{
				double circleBegin = currentAngle;
				double circleEnd = targetAngle;
				if (MathD.TurnDir(circleBegin, circleEnd) < 0)
				{
					MathD.Swap(ref circleBegin, ref circleEnd);
					circleEnd = circleBegin + MathD.CircularDist(circleBegin, circleEnd);
				}

				canvas.State.ColorTint = baseColor * this.JointErrorColor;
				canvas.FillThickLine(
					bodyPos.X + anchorToWorld.X,
					bodyPos.Y + anchorToWorld.Y,
					0.0f, 
					bodyPos.X + anchorToWorld.X + errorVec.X,
					bodyPos.Y + anchorToWorld.Y + errorVec.Y,
					0.0f,
					lineWidth);
				canvas.FillCircleSegment(
					bodyPos.X + anchorToWorld.X,
					bodyPos.Y + anchorToWorld.Y,
					0.0f,
					radius,
					circleBegin,
					circleEnd,
					lineWidth);
			}
			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillThickLine(
				bodyPos.X + anchorToWorld.X,
				bodyPos.Y + anchorToWorld.Y,
				0.0f, 
				bodyPos.X + anchorToWorld.X + angleVec.X,
				bodyPos.Y + anchorToWorld.Y + angleVec.Y,
				0.0f,
				lineWidth);
			canvas.State.ColorTint = baseColor;
		}
		private void DrawLocalAngleConstraint(Canvas canvas, RigidBody body, Vector2 anchor, double minAngle, double maxAngle, double currentAngle, double radius)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D bodyPos = body.GameObj.Transform.Pos;

			radius = this.GetScreenMinScale(canvas, radius, this.minAngleConstraintRadius);

			double lineWidth = this.GetScreenConstantScale(canvas, this.defaultLineWidth);
			Vector2D anchorToWorld = body.GameObj.Transform.GetWorldVector(anchor);
			Vector2D angleVecMin = Vector2D.FromAngleLength(minAngle, radius);
			Vector2D angleVecMax = Vector2D.FromAngleLength(maxAngle, radius);
			Vector2D errorVec = Vector2D.FromAngleLength(currentAngle, radius);
			double angleDistMin = MathD.Abs(currentAngle - minAngle);
			double angleDistMax = MathD.Abs(currentAngle - maxAngle);
			double angleRange = maxAngle - minAngle;
			bool hasError = angleDistMin > angleDistMax ? angleDistMin >= angleRange : angleDistMax >= angleRange;

			if (hasError)
			{
				double circleBegin = currentAngle;
				double circleEnd = angleDistMin < angleDistMax ? minAngle : maxAngle;
				if (MathD.TurnDir(circleBegin, circleEnd) < 0)
				{
					MathD.Swap(ref circleBegin, ref circleEnd);
					circleEnd = circleBegin + MathD.CircularDist(circleBegin, circleEnd);
				}

				canvas.State.ColorTint = baseColor * this.JointErrorColor;
				canvas.FillThickLine(
					bodyPos.X + anchorToWorld.X,
					bodyPos.Y + anchorToWorld.Y,
					0.0f, 
					bodyPos.X + anchorToWorld.X + errorVec.X,
					bodyPos.Y + anchorToWorld.Y + errorVec.Y,
					0.0f,
					lineWidth);
				canvas.FillCircleSegment(
					bodyPos.X + anchorToWorld.X,
					bodyPos.Y + anchorToWorld.Y,
					0.0f,
					radius,
					circleBegin,
					circleEnd,
					lineWidth);
			}

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillCircleSegment(
				bodyPos.X + anchorToWorld.X,
				bodyPos.Y + anchorToWorld.Y,
				0.0f,
				radius,
				minAngle,
				maxAngle,
				lineWidth);
			canvas.FillThickLine(
				bodyPos.X + anchorToWorld.X,
				bodyPos.Y + anchorToWorld.Y,
				0.0f, 
				bodyPos.X + anchorToWorld.X + angleVecMin.X,
				bodyPos.Y + anchorToWorld.Y + angleVecMin.Y,
				0.0f,
				lineWidth);
			canvas.FillThickLine(
				bodyPos.X + anchorToWorld.X,
				bodyPos.Y + anchorToWorld.Y,
				0.0f, 
				bodyPos.X + anchorToWorld.X + angleVecMax.X,
				bodyPos.Y + anchorToWorld.Y + angleVecMax.Y,
				0.0f,
				lineWidth);
			canvas.State.ColorTint = baseColor;
		}
		private void DrawLocalAngleMotor(Canvas canvas, RigidBody body, Vector2D anchor, double speed, double maxTorque, double radius)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D bodyPos = body.GameObj.Transform.Pos;

			radius = this.GetScreenMinScale(canvas, radius, this.minAngleConstraintRadius);

			double lineWidth = this.GetScreenConstantScale(canvas, this.defaultLineWidth);
			double worldLineCapScale = this.GetScreenConstantScale(canvas, this.lineCapScale);
			double baseAngle = body.GameObj.Transform.Angle;
			double speedAngle = baseAngle + speed * Time.FramesPerSecond * 0.2f; // Radians per fifth of a second
			double maxTorqueAngle = baseAngle + MathD.Sign(speed) * maxTorque * PhysicsUnit.TorqueToPhysical * 0.2f;
			Vector2D anchorToWorld = body.GameObj.Transform.GetWorldVector(anchor);
			Vector2D arrowBase = anchorToWorld + Vector2D.FromAngleLength(speedAngle, radius);
			Vector2D arrowA = Vector2D.FromAngleLength(speedAngle - MathD.RadAngle45, MathD.Sign(speed) * worldLineCapScale);
			Vector2D arrowB = Vector2D.FromAngleLength(speedAngle - MathD.RadAngle45 + MathD.RadAngle270, MathD.Sign(speed) * worldLineCapScale);

			canvas.State.ColorTint = baseColor * this.MotorColor.WithAlpha(0.5f);
			canvas.FillCircleSegment(
				bodyPos.X + anchorToWorld.X,
				bodyPos.Y + anchorToWorld.Y,
				0.0f,
				radius - lineWidth * 2.0f,
				MathD.Sign(speed) >= 0 ? baseAngle : maxTorqueAngle,
				MathD.Sign(speed) >= 0 ? maxTorqueAngle : baseAngle,
				lineWidth);
			canvas.FillCircleSegment(
				bodyPos.X + anchorToWorld.X,
				bodyPos.Y + anchorToWorld.Y,
				0.0f,
				radius + lineWidth * 2.0f,
				MathD.Sign(speed) >= 0 ? baseAngle : maxTorqueAngle,
				MathD.Sign(speed) >= 0 ? maxTorqueAngle : baseAngle,
				lineWidth);
			canvas.State.ColorTint = baseColor * this.MotorColor;
			canvas.FillCircleSegment(
				bodyPos.X + anchorToWorld.X,
				bodyPos.Y + anchorToWorld.Y,
				0.0f,
				radius,
				MathD.Sign(speed) >= 0 ? baseAngle : speedAngle,
				MathD.Sign(speed) >= 0 ? speedAngle : baseAngle,
				lineWidth);
			canvas.FillThickLine(
				bodyPos.X + arrowBase.X,
				bodyPos.Y + arrowBase.Y,
				0.0f,
				bodyPos.X + arrowBase.X + arrowA.X,
				bodyPos.Y + arrowBase.Y + arrowA.Y,
				0.0f,
				lineWidth);
			canvas.FillThickLine(
				bodyPos.X + arrowBase.X,
				bodyPos.Y + arrowBase.Y,
				0.0f,
				bodyPos.X + arrowBase.X + arrowB.X,
				bodyPos.Y + arrowBase.Y + arrowB.Y,
				0.0f,
				lineWidth);
			canvas.State.ColorTint = baseColor;
		}
		private void DrawLocalPosConstraint(Canvas canvas, RigidBody bodyA, RigidBody bodyB, Vector2 anchorA, Vector2D anchorB)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D colliderPosA = bodyA.GameObj.Transform.Pos;
			Vector3D colliderPosB = bodyB.GameObj.Transform.Pos;

			double lineWidth = this.GetScreenConstantScale(canvas, this.defaultLineWidth);
			Vector2D anchorAToWorld = bodyA.GameObj.Transform.GetWorldVector(anchorA);
			Vector2D anchorBToWorld = bodyB.GameObj.Transform.GetWorldVector(anchorB);
			Vector2D errorVec = (colliderPosB.Xy + anchorBToWorld) - (colliderPosA.Xy + anchorAToWorld);
			
			bool hasError = errorVec.Length >= 1.0f;
			if (hasError)
			{
				canvas.State.ColorTint = baseColor * this.JointErrorColor;
				canvas.FillThickLine(
					colliderPosA.X + anchorAToWorld.X,
					colliderPosA.Y + anchorAToWorld.Y,
					0.0f,
					colliderPosB.X + anchorBToWorld.X,
					colliderPosB.Y + anchorBToWorld.Y,
					0.0f,
					lineWidth);
			}

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillThickLine(
				colliderPosA.X,
				colliderPosA.Y,
				0.0f,
				colliderPosA.X + anchorAToWorld.X,
				colliderPosA.Y + anchorAToWorld.Y,
				0.0f,
				lineWidth);
			canvas.FillThickLine(
				colliderPosB.X,
				colliderPosB.Y,
				0.0f,
				colliderPosB.X + anchorBToWorld.X,
				colliderPosB.Y + anchorBToWorld.Y,
				0.0f,
				lineWidth);
			canvas.State.ColorTint = baseColor;
		}
		private void DrawLocalDistConstraint(Canvas canvas, RigidBody bodyA, RigidBody bodyB, Vector2D localAnchorA, Vector2D localAnchorB, double minDist, double maxDist)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D bodyPosA = bodyA.GameObj.Transform.Pos;
			Vector3D bodyPosB = bodyB.GameObj.Transform.Pos;
			
			double lineWidth = this.GetScreenConstantScale(canvas, this.defaultLineWidth);
			double worldLineCapScale = this.GetScreenConstantScale(canvas, this.lineCapScale);
			Vector2D anchorA = bodyA.GameObj.Transform.GetWorldVector(localAnchorA);
			Vector2D anchorB = bodyB.GameObj.Transform.GetWorldVector(localAnchorB);
			Vector2D errorVec = (bodyPosB.Xy + anchorB) - (bodyPosA.Xy + anchorA);
			double dist = errorVec.Length;
			Vector2D distVec = errorVec.Normalized * MathD.Clamp(dist, minDist, maxDist);
			Vector2D lineNormal = errorVec.PerpendicularRight.Normalized;
			bool hasError = (errorVec - distVec).Length >= 1.0f;

			if (hasError)
			{
				canvas.State.ColorTint = baseColor * this.JointErrorColor;
				canvas.FillThickLine(
					bodyPosA.X + anchorA.X + distVec.X,
					bodyPosA.Y + anchorA.Y + distVec.Y,
					0.0f, 
					bodyPosA.X + anchorA.X + errorVec.X,
					bodyPosA.Y + anchorA.Y + errorVec.Y,
					0.0f,
					lineWidth);
			}

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillThickLine(
				bodyPosA.X + anchorA.X,
				bodyPosA.Y + anchorA.Y,
				0.0f, 
				bodyPosA.X + anchorA.X + distVec.X,
				bodyPosA.Y + anchorA.Y + distVec.Y,
				0.0f,
				lineWidth);
			if (hasError)
			{
				canvas.FillThickLine(
					bodyPosA.X + anchorA.X + distVec.X - lineNormal.X * worldLineCapScale,
					bodyPosA.Y + anchorA.Y + distVec.Y - lineNormal.Y * worldLineCapScale,
					0.0f, 
					bodyPosA.X + anchorA.X + distVec.X + lineNormal.X * worldLineCapScale,
					bodyPosA.Y + anchorA.Y + distVec.Y + lineNormal.Y * worldLineCapScale,
					0.0f,
					lineWidth);
			}
			canvas.State.ColorTint = baseColor;
		}
		private void DrawLocalAxisConstraint(Canvas canvas, RigidBody bodyA, RigidBody bodyB, Vector2D localAxis, Vector2D localAnchorA, Vector2D localAnchorB, double min = 1, double max = -1)
		{
			Vector3D bodyPosA = bodyA.GameObj.Transform.Pos;
			Vector3D bodyPosB = bodyB.GameObj.Transform.Pos;
			Vector2D worldAxis = bodyB.GameObj.Transform.GetWorldVector(localAxis).Normalized;
			Vector2D worldAnchorB = bodyB.GameObj.Transform.GetWorldPoint(localAnchorB);

			this.DrawWorldAxisConstraint(canvas, bodyA, worldAxis, localAnchorA, worldAnchorB, min, max);
		}
		private void DrawLocalAxisMotor(Canvas canvas, RigidBody bodyA, RigidBody bodyB, Vector2D localAxis, Vector2D localAnchorA, Vector2D localAnchorB, double speed, double maxForce, double offset)
		{
			Vector3D bodyPosA = bodyA.GameObj.Transform.Pos;
			Vector3D bodyPosB = bodyB.GameObj.Transform.Pos;
			Vector2D worldAxis = bodyB.GameObj.Transform.GetWorldVector(localAxis).Normalized;
			Vector2D worldAnchorB = bodyB.GameObj.Transform.GetWorldPoint(localAnchorB);

			this.DrawWorldAxisMotor(canvas, bodyA, worldAxis, localAnchorA, worldAnchorB, speed, maxForce, offset);
		}
		private void DrawLocalLooseConstraint(Canvas canvas, RigidBody bodyA, RigidBody bodyB, Vector2D anchorA, Vector2D anchorB)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D bodyPosA = bodyA.GameObj.Transform.Pos;
			Vector3D bodyPosB = bodyB.GameObj.Transform.Pos;

			Vector2D anchorAToWorld = bodyA.GameObj.Transform.GetWorldVector(anchorA);
			Vector2D anchorBToWorld = bodyB.GameObj.Transform.GetWorldVector(anchorB);

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.DrawDashLine(
				bodyPosA.X + anchorAToWorld.X,
				bodyPosA.Y + anchorAToWorld.Y,
				0.0f,
				bodyPosB.X + anchorBToWorld.X,
				bodyPosB.Y + anchorBToWorld.Y,
				0.0f);
			canvas.State.ColorTint = baseColor;
		}
		
		private void DrawWorldAnchor(Canvas canvas, RigidBody body, Vector2D anchor)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D colliderPos = body.GameObj.Transform.Pos;
			double markerCircleRad = this.GetScreenConstantScale(canvas, this.anchorScale);

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillCircle(
				anchor.X,
				anchor.Y,
				0.0f,
				markerCircleRad);
			canvas.State.ColorTint = baseColor;
		}
		private void DrawWorldPosConstraint(Canvas canvas, RigidBody body, Vector2D localAnchor, Vector2D worldAnchor)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D bodyPos = body.GameObj.Transform.Pos;

			double lineWidth = this.GetScreenConstantScale(canvas, this.defaultLineWidth);
			Vector2D anchorAToWorld = body.GameObj.Transform.GetWorldVector(localAnchor);
			Vector2D errorVec = worldAnchor - (bodyPos.Xy + anchorAToWorld);
			
			bool hasError = errorVec.Length >= 1.0f;
			if (hasError)
			{
				canvas.State.ColorTint = baseColor * this.JointErrorColor;
				canvas.FillThickLine(
					bodyPos.X + anchorAToWorld.X,
					bodyPos.Y + anchorAToWorld.Y,
					0.0f,
					worldAnchor.X,
					worldAnchor.Y,
					0.0f,
					lineWidth);
			}

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillThickLine(
				bodyPos.X,
				bodyPos.Y,
				0.0f,
				bodyPos.X + anchorAToWorld.X,
				bodyPos.Y + anchorAToWorld.Y,
				0.0f,
				lineWidth);
			canvas.State.ColorTint = baseColor;
		}
		private void DrawWorldDistConstraint(Canvas canvas, RigidBody body, Vector2D localAnchor, Vector2D worldAnchor, double minDist, double maxDist)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D colliderPosA = body.GameObj.Transform.Pos;

			double lineWidth = this.GetScreenConstantScale(canvas, this.defaultLineWidth);
			double worldLineCapScale = this.GetScreenConstantScale(canvas, this.lineCapScale);
			Vector2D anchorA = body.GameObj.Transform.GetWorldVector(localAnchor);
			Vector2D errorVec = worldAnchor - (colliderPosA.Xy + anchorA);
			Vector2D lineNormal = errorVec.PerpendicularRight.Normalized;
			double dist = errorVec.Length;
			Vector2D distVec = errorVec.Normalized * MathD.Clamp(dist, minDist, maxDist);
			bool hasError = (errorVec - distVec).Length >= 1.0f;

			if (hasError)
			{
				canvas.State.ColorTint = baseColor * this.JointErrorColor;
				canvas.FillThickLine(
					colliderPosA.X + anchorA.X + distVec.X,
					colliderPosA.Y + anchorA.Y + distVec.Y,
					0.0f, 
					colliderPosA.X + anchorA.X + errorVec.X,
					colliderPosA.Y + anchorA.Y + errorVec.Y,
					0.0f,
					lineWidth);
			}

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillThickLine(
				colliderPosA.X + anchorA.X,
				colliderPosA.Y + anchorA.Y,
				0.0f, 
				colliderPosA.X + anchorA.X + distVec.X,
				colliderPosA.Y + anchorA.Y + distVec.Y,
				0.0f,
				lineWidth);
			if (hasError)
			{
				canvas.FillThickLine(
					colliderPosA.X + anchorA.X + distVec.X - lineNormal.X * worldLineCapScale,
					colliderPosA.Y + anchorA.Y + distVec.Y - lineNormal.Y * worldLineCapScale,
					0.0f, 
					colliderPosA.X + anchorA.X + distVec.X + lineNormal.X * worldLineCapScale,
					colliderPosA.Y + anchorA.Y + distVec.Y + lineNormal.Y * worldLineCapScale,
					0.0f,
					lineWidth);
			}
			canvas.State.ColorTint = baseColor;
		}
		private void DrawWorldAxisConstraint(Canvas canvas, RigidBody body, Vector2D worldAxis, Vector2D localAnchor, Vector2D worldAnchor, double min = 1, double max = -1)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D bodyPos = body.GameObj.Transform.Pos;

			worldAxis = worldAxis.Normalized;
			bool infinite = false;
			if (min > max)
			{
				min = -10000000.0f;
				max = 10000000.0f;
				infinite = true;
			}

			double lineWidth = this.GetScreenConstantScale(canvas, this.defaultLineWidth);
			double worldLineCapScale = this.GetScreenConstantScale(canvas, this.lineCapScale);
			Vector2D anchorToWorld = body.GameObj.Transform.GetWorldVector(localAnchor);
			double axisVal = Vector2D.Dot(bodyPos.Xy + anchorToWorld - worldAnchor, worldAxis);
			Vector2D basePos = MathD.PointLineNearestPoint(
				bodyPos.X + anchorToWorld.X, 
				bodyPos.Y + anchorToWorld.Y, 
				worldAnchor.X + worldAxis.X * min, 
				worldAnchor.Y + worldAxis.Y * min, 
				worldAnchor.X + worldAxis.X * max, 
				worldAnchor.Y + worldAxis.Y * max,
				infinite);
			double errorVal = (bodyPos.Xy + anchorToWorld - basePos).Length;
			Vector2D errorVec = basePos - bodyPos.Xy;
			bool hasError = errorVal >= 1.0f;

			if (hasError)
			{
				canvas.State.ColorTint = baseColor * this.JointErrorColor;
				canvas.FillThickLine(
					bodyPos.X + anchorToWorld.X,
					bodyPos.Y + anchorToWorld.Y,
					0.0f,
					basePos.X,
					basePos.Y,
					0.0f,
					lineWidth);
			}

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.FillThickLine(
				worldAnchor.X + worldAxis.X * min,
				worldAnchor.Y + worldAxis.Y * min,
				0.0f,
				worldAnchor.X + worldAxis.X * max,
				worldAnchor.Y + worldAxis.Y * max,
				0.0f,
				lineWidth);
			if (!infinite)
			{
				canvas.FillThickLine(
					worldAnchor.X + worldAxis.X * min + worldAxis.PerpendicularLeft.X * worldLineCapScale,
					worldAnchor.Y + worldAxis.Y * min + worldAxis.PerpendicularLeft.Y * worldLineCapScale,
					0.0f,
					worldAnchor.X + worldAxis.X * min + worldAxis.PerpendicularRight.X * worldLineCapScale,
					worldAnchor.Y + worldAxis.Y * min + worldAxis.PerpendicularRight.Y * worldLineCapScale,
					0.0f,
					lineWidth);
				canvas.FillThickLine(
					worldAnchor.X + worldAxis.X * max + worldAxis.PerpendicularLeft.X * worldLineCapScale,
					worldAnchor.Y + worldAxis.Y * max + worldAxis.PerpendicularLeft.Y * worldLineCapScale,
					0.0f,
					worldAnchor.X + worldAxis.X * max + worldAxis.PerpendicularRight.X * worldLineCapScale,
					worldAnchor.Y + worldAxis.Y * max + worldAxis.PerpendicularRight.Y * worldLineCapScale,
					0.0f,
					lineWidth);
			}
			canvas.State.ColorTint = baseColor;
		}
		private void DrawWorldAxisMotor(Canvas canvas, RigidBody body, Vector2D worldAxis, Vector2D localAnchor, Vector2D worldAnchor, double speed, double maxForce, double offset)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D bodyPos = body.GameObj.Transform.Pos;

			double lineWidth = this.GetScreenConstantScale(canvas, this.defaultLineWidth);
			double worldLineCapScale = this.GetScreenConstantScale(canvas, this.lineCapScale);
			double worldArrowScale = this.GetScreenConstantScale(canvas, speed * 20.0f);
			Vector2D anchorToWorld = body.GameObj.Transform.GetWorldVector(localAnchor);
			double axisAngle = worldAxis.Angle;
			double maxForceTemp = MathD.Sign(speed) * maxForce * PhysicsUnit.ForceToPhysical * 2.0f;
			Vector2D arrowBegin = bodyPos.Xy + worldAxis.PerpendicularRight * offset;
			Vector2D arrowBase = arrowBegin + worldAxis * worldArrowScale;
			Vector2D arrowA = Vector2D.FromAngleLength(axisAngle + MathD.RadAngle45 + MathD.RadAngle180, MathD.Sign(speed) * worldLineCapScale);
			Vector2D arrowB = Vector2D.FromAngleLength(axisAngle - MathD.RadAngle45 + MathD.RadAngle180, MathD.Sign(speed) * worldLineCapScale);
			
			canvas.State.ColorTint = baseColor * this.MotorColor.WithAlpha(0.5f);
			canvas.FillThickLine(
				arrowBegin.X + worldAxis.PerpendicularLeft.X * 2.0f * lineWidth,
				arrowBegin.Y + worldAxis.PerpendicularLeft.Y * 2.0f * lineWidth,
				0.0f,
				arrowBegin.X + worldAxis.PerpendicularLeft.X * 2.0f * lineWidth + worldAxis.X * maxForceTemp,
				arrowBegin.Y + worldAxis.PerpendicularLeft.Y * 2.0f * lineWidth + worldAxis.Y * maxForceTemp,
				0.0f,
				lineWidth);
			canvas.FillThickLine(
				arrowBegin.X + worldAxis.PerpendicularRight.X * 2.0f * lineWidth,
				arrowBegin.Y + worldAxis.PerpendicularRight.Y * 2.0f * lineWidth,
				0.0f,
				arrowBegin.X + worldAxis.PerpendicularRight.X * 2.0f * lineWidth + worldAxis.X * maxForceTemp,
				arrowBegin.Y + worldAxis.PerpendicularRight.Y * 2.0f * lineWidth + worldAxis.Y * maxForceTemp,
				0.0f,
				lineWidth);
			canvas.State.ColorTint = baseColor * this.MotorColor;
			canvas.FillThickLine(
				arrowBegin.X,
				arrowBegin.Y,
				0.0f,
				arrowBase.X,
				arrowBase.Y,
				0.0f,
				lineWidth);
			canvas.FillThickLine(
				arrowBase.X,
				arrowBase.Y,
				0.0f,
				arrowBase.X + arrowA.X,
				arrowBase.Y + arrowA.Y,
				0.0f,
				lineWidth);
			canvas.FillThickLine(
				arrowBase.X,
				arrowBase.Y,
				0.0f,
				arrowBase.X + arrowB.X,
				arrowBase.Y + arrowB.Y,
				0.0f,
				lineWidth);
			canvas.State.ColorTint = baseColor;
		}
		private void DrawWorldLooseConstraint(Canvas canvas, RigidBody bodyA, Vector2D anchorA, Vector2D anchorB)
		{
			ColorRgba baseColor = canvas.State.ColorTint;
			Vector3D bodyPosA = bodyA.GameObj.Transform.Pos;

			canvas.State.ColorTint = baseColor * this.JointColor;
			canvas.DrawDashLine(
				anchorA.X,
				anchorA.Y,
				0.0f,
				anchorB.X,
				anchorB.Y,
				0.0f);
			canvas.State.ColorTint = baseColor;
		}

		private double GetAnchorDist(RigidBody bodyA, RigidBody bodyB, Vector2D localAnchorA, Vector2D localAnchorB)
		{
			Vector3D colliderPosA = bodyA.GameObj.Transform.Pos;
			Vector3D colliderPosB = bodyB.GameObj.Transform.Pos;
			
			Vector2D anchorAToWorld = bodyA.GameObj.Transform.GetWorldVector(localAnchorA);
			Vector2D anchorBToWorld = bodyB.GameObj.Transform.GetWorldVector(localAnchorB);
			Vector2D errorVec = (colliderPosB.Xy + anchorBToWorld) - (colliderPosA.Xy + anchorAToWorld);

			return errorVec.Length;
		}
		
		private double GetScreenScale(Canvas canvas)
		{
			return MathD.Max(0.0001f, canvas.DrawDevice.GetScaleAtZ(0.0f));
		}
		private double GetScreenMinScale(Canvas canvas, double worldScale, double minScreenScale)
		{
			double scaleToScreen = this.GetScreenScale(canvas);
			double screenRadius = MathD.Max(worldScale * scaleToScreen, minScreenScale);
			return screenRadius / scaleToScreen;
		}
		private double GetScreenConstantScale(Canvas canvas, double baseScale)
		{
			return baseScale / MathD.Max(0.0001f, canvas.DrawDevice.GetScaleAtZ(0.0f));
		}

		private IEnumerable<RigidBody> QueryVisibleColliders()
		{
			var allColliders = Scene.Current.FindComponents<RigidBody>();
			return allColliders.Where(r => 
				r.Active && 
				!DesignTimeObjectData.Get(r.GameObj).IsHidden && 
				this.IsSphereInView(r.GameObj.Transform.Pos, r.BoundRadius));
		}
		private RigidBody QuerySelectedCollider()
		{
			return 
				DualityEditorApp.Selection.Components.OfType<RigidBody>().FirstOrDefault() ?? 
				DualityEditorApp.Selection.GameObjects.GetComponents<RigidBody>().FirstOrDefault();
		}
	}
}
