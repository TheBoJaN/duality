using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality;
using Duality.Cloning;
using Duality.Resources;

using Duality.Editor;

using Duality.Editor.Plugins.CamView.Properties;
using Duality.Editor.Plugins.CamView.CamViewStates;

namespace Duality.Editor.Plugins.CamView.UndoRedoActions
{
	public class RotateCamViewObjAction : CamViewObjAction
	{
		private Vector3D[]	backupPos	= null;
		private double[]		backupAngle	= null;
		private	double		turnBy		= 0.0f;

		public override string Name
		{
			get { return this.targetObj.Length == 1 ? 
				string.Format(CamViewRes.UndoRedo_RotateCamViewObj, this.targetObj[0].DisplayObjectName) :
				string.Format(CamViewRes.UndoRedo_RotateCamViewObjMulti, this.targetObj.Length); }
		}
		public override bool IsVoid
		{
			get { return base.IsVoid || this.turnBy == 0.0f; }
		}

		public RotateCamViewObjAction(IEnumerable<ObjectEditorSelObj> obj, PostPerformAction postPerform, double turnBy) : base(obj, postPerform)
		{
			this.turnBy = turnBy;
		}

		public override bool CanAppend(UndoRedoAction action)
		{
			return action is RotateCamViewObjAction && base.CanAppend(action);
		}
		public override void Append(UndoRedoAction action, bool performAction)
		{
			base.Append(action, performAction);
			RotateCamViewObjAction moveAction = action as RotateCamViewObjAction;
			if (performAction)
			{
				moveAction.backupPos = this.backupPos;
				moveAction.backupAngle = this.backupAngle;
				moveAction.Do();
			}
			this.turnBy += moveAction.turnBy;
		}
		public override void Do()
		{
			if (this.backupPos == null)
			{
				this.backupPos = new Vector3D[this.targetObj.Length];
				this.backupAngle = new double[this.targetObj.Length];
				for (int i = 0; i < this.targetObj.Length; i++)
				{
					this.backupPos[i] = this.targetObj[i].Pos;
					this.backupAngle[i] = this.targetObj[i].Angle;
				}
			}
			
			Vector3D center = Vector3D.Zero;
			foreach (ObjectEditorSelObj s in this.targetObj)
			{
				center += s.Pos;
			}
			if (this.targetObj.Length > 0) center /= this.targetObj.Length;

			foreach (ObjectEditorSelObj s in this.targetObj)
			{
				Vector3D posRelCenter = s.Pos - center;
				Vector3D posRelCenterTarget = posRelCenter;
				MathD.TransformCoord(ref posRelCenterTarget.X, ref posRelCenterTarget.Y, this.turnBy);
				s.Pos = center + posRelCenterTarget;
				s.Angle += this.turnBy;
			}

			if (this.postPerform != null)
				this.postPerform(this.targetObj);
		}
		public override void Undo()
		{
			if (this.backupPos == null) throw new InvalidOperationException("Can't undo what hasn't been done yet");

			for (int i = 0; i < this.backupPos.Length; i++)
			{
				this.targetObj[i].Pos = this.backupPos[i];
				this.targetObj[i].Angle = this.backupAngle[i];
			}

			if (this.postPerform != null)
				this.postPerform(this.targetObj);
		}
	}
}
