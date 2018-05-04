using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality;
using Duality.Cloning;
using Duality.Components.Physics;
using Duality.Editor;
using Duality.Editor.Plugins.CamView.Properties;


namespace Duality.Editor.Plugins.CamView.UndoRedoActions
{
	public class EditRigidBodyPolyShapeAction : UndoRedoAction
	{
		private Vector2D[]            originalVertices = null;
		private Vector2D[]            newVertices      = null;
		private VertexBasedShapeInfo targetShape      = null;
		private bool                 sameVertices     = false;

		public override string Name
		{
			get
			{
				if (this.originalVertices.Length == this.newVertices.Length)
					return CamViewRes.UndoRedo_MoveRigidBodyShapeVertex;
				else if (this.originalVertices.Length > this.newVertices.Length)
					return CamViewRes.UndoRedo_DeleteRigidBodyShapeVertex;
				else 
					return CamViewRes.UndoRedo_CreateRigidBodyShapeVertex;
			}
		}
		public override bool IsVoid
		{
			get { return this.sameVertices; }
		}

		public EditRigidBodyPolyShapeAction(VertexBasedShapeInfo shape, Vector2D[] originalVertices, Vector2D[] newVertices)
		{
			this.targetShape = shape;
			this.originalVertices = (Vector2D[])originalVertices.Clone();
			this.newVertices = (Vector2D[])newVertices.Clone();
			this.sameVertices = this.AreVerticesEqual(this.originalVertices, this.newVertices);
		}

		public override void Do()
		{
			Vector2D[] temp = (Vector2D[])this.newVertices.Clone();
			this.targetShape.Vertices = temp;
            
			DualityEditorApp.NotifyObjPropChanged(
				this, 
				new ObjectSelection(this.targetShape.Parent), 
				ReflectionInfo.Property_RigidBody_Shapes);
		}
		public override void Undo()
		{
			Vector2D[] temp = (Vector2D[])this.originalVertices.Clone();
			this.targetShape.Vertices = temp;

			DualityEditorApp.NotifyObjPropChanged(
				this, 
				new ObjectSelection(this.targetShape.Parent), 
				ReflectionInfo.Property_RigidBody_Shapes);
		}

		private bool AreVerticesEqual(Vector2D[] a, Vector2D[] b)
		{
			if (a == b) return true;
			if (a == null || b == null) return false;
			if (a.Length != b.Length) return false;
			
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i]) return false;
			}

			return true;
		}
	}
}
