using System.Drawing;
using System.Linq;

using Duality;
using Duality.Components;
using Duality.Components.Physics;

namespace Duality.Editor.Plugins.CamView.CamViewStates
{
	public class RigidBodyEditorSelVertexShape : RigidBodyEditorSelShape
	{
		private Vector2D center;
		private double   boundRad;
		private double   angle;
		private Vector2D scale;

		public override Vector3D Pos
		{
			get
			{
				return this.Body.GameObj.Transform.GetWorldPoint(new Vector3D(this.center));
			}
			set
			{
				value.Z = this.Body.GameObj.Transform.Pos.Z;
				this.MoveCenterTo(this.Body.GameObj.Transform.GetLocalPoint(value).Xy);
			}
		}
		public override Vector3D Scale
		{
			get { return new Vector3D(this.scale); }
			set { this.ScaleTo(value.Xy); }
		}
		public override double Angle
		{
			get { return this.angle; }
			set { this.RotateTo(value); }
		}
		public override double BoundRadius
		{
			get { return this.boundRad * this.Body.GameObj.Transform.Scale; }
		}
		public override string DisplayObjectName
		{
			get { return Properties.CamViewRes.RigidBodyCamViewState_SelPolyShapeName; }
		}
		private VertexBasedShapeInfo VertexShape
		{
			get { return this.Shape as VertexBasedShapeInfo; }
		}


		public RigidBodyEditorSelVertexShape(VertexBasedShapeInfo shape) : base(shape)
		{
			this.UpdateShapeStats();
		}

		public override string UpdateActionText(ObjectEditorAction action, bool performing)
		{
			if (action == ObjectEditorAction.Move)
			{
				return
					string.Format("Center X:{0,9:0.00}/n", this.center.X) +
					string.Format("Center Y:{0,9:0.00}", this.center.Y);
			}
			else if (action == ObjectEditorAction.Scale)
			{
				if (MathD.Abs(this.scale.X - this.scale.Y) >= 0.01f)
				{
					return
						string.Format("Scale X:{0,8:0.00}/n", this.scale.X) +
						string.Format("Scale Y:{0,8:0.00}", this.scale.Y);
				}
				else
				{
					return string.Format("Scale:{0,8:0.00}", this.scale.X);
				}
			}
			else if (action == ObjectEditorAction.Rotate)
			{
				return string.Format("Angle:{0,6:0.0}°", MathD.RadToDeg(this.angle));
			}
			return base.UpdateActionText(action, performing);
		}

		public override void UpdateShapeStats()
		{
			Vector2D[] vertices = this.VertexShape.Vertices;

			this.center = Vector2D.Zero;
			for (int i = 0; i < vertices.Length; i++)
				this.center += vertices[i];
			this.center /= vertices.Length;

			this.scale = Vector2D.Zero;
			for (int i = 0; i < vertices.Length; i++)
			{
				this.scale.X = MathD.Max(this.scale.X, MathD.Abs(vertices[i].X - this.center.X));
				this.scale.Y = MathD.Max(this.scale.Y, MathD.Abs(vertices[i].Y - this.center.Y));
			}

			this.boundRad = 0.0f;
			for (int i = 0; i < vertices.Length; i++)
				this.boundRad = MathD.Max(this.boundRad, (vertices[i] - this.center).Length);

			this.angle = MathD.Angle(this.center.X, this.center.Y, vertices[0].X, vertices[0].Y);
		}
		private void MoveCenterTo(Vector2D newPos)
		{
			Vector2D mov = newPos - this.center;

			Vector2D[] movedVertices = this.VertexShape.Vertices.ToArray();
			for (int i = 0; i < movedVertices.Length; i++)
				movedVertices[i] += mov;

			this.VertexShape.Vertices = movedVertices;
			this.UpdateShapeStats();
		}
		private void ScaleTo(Vector2D newScale)
		{
			Vector2D scaleRatio = newScale / this.scale;

			Vector2D[] scaledVertices = this.VertexShape.Vertices.ToArray();
			for (int i = 0; i < scaledVertices.Length; i++)
			{
				scaledVertices[i].X = (scaledVertices[i].X - this.center.X) * scaleRatio.X + this.center.X;
				scaledVertices[i].Y = (scaledVertices[i].Y - this.center.Y) * scaleRatio.Y + this.center.Y;
			}

			this.VertexShape.Vertices = scaledVertices;
			this.UpdateShapeStats();
		}
		private void RotateTo(double newAngle)
		{
			double rot = newAngle - this.angle;

			Vector2D[] rotatedVertices = this.VertexShape.Vertices.ToArray();
			for (int i = 0; i < rotatedVertices.Length; i++)
				MathD.TransformCoord(ref rotatedVertices[i].X, ref rotatedVertices[i].Y, rot, 1.0f, this.center.X, this.center.Y);

			this.VertexShape.Vertices = rotatedVertices;
			this.UpdateShapeStats();
		}
	}
}
