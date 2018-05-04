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
	public class ScaleCamViewObjAction : CamViewObjAction
	{
		private Vector3D[]	backupPos	= null;
		private Vector3D[]	backupScale	= null;
		private	double		scaleBy		= 0.0f;

		public override string Name
		{
			get { return this.targetObj.Length == 1 ? 
				string.Format(CamViewRes.UndoRedo_ScaleCamViewObj, this.targetObj[0].DisplayObjectName) :
				string.Format(CamViewRes.UndoRedo_ScaleCamViewObjMulti, this.targetObj.Length); }
		}
		public override bool IsVoid
		{
			get { return base.IsVoid || this.scaleBy == 1.0f; }
		}

		public ScaleCamViewObjAction(IEnumerable<ObjectEditorSelObj> obj, PostPerformAction postPerform, double scaleBy) : base(obj, postPerform)
		{
			this.scaleBy = scaleBy;
		}

		public override bool CanAppend(UndoRedoAction action)
		{
			return action is ScaleCamViewObjAction && base.CanAppend(action);
		}
		public override void Append(UndoRedoAction action, bool performAction)
		{
			base.Append(action, performAction);
			ScaleCamViewObjAction moveAction = action as ScaleCamViewObjAction;
			if (performAction)
			{
				moveAction.backupPos = this.backupPos;
				moveAction.backupScale = this.backupScale;
				moveAction.Do();
			}
			this.scaleBy *= moveAction.scaleBy;
		}
		public override void Do()
		{
			if (this.backupPos == null)
			{
				this.backupPos = new Vector3D[this.targetObj.Length];
				this.backupScale = new Vector3D[this.targetObj.Length];
				for (int i = 0; i < this.targetObj.Length; i++)
				{
					this.backupPos[i] = this.targetObj[i].Pos;
					this.backupScale[i] = this.targetObj[i].Scale;
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
				Vector3D scaleVec = new Vector3D(this.scaleBy, this.scaleBy, this.scaleBy);
				Vector3D posRelCenter = s.Pos - center;
				Vector3D posRelCenterTarget;
				Vector3D.Multiply(ref posRelCenter, ref scaleVec, out posRelCenterTarget);

				s.Pos = center + posRelCenterTarget;
				s.Scale *= scaleVec;
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
				this.targetObj[i].Scale = this.backupScale[i];
			}

			if (this.postPerform != null)
				this.postPerform(this.targetObj);
		}
	}
}
