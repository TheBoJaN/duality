using System;
using System.Linq;

using Duality;

namespace Duality.Editor.Controls.PropertyEditors
{
	[PropertyEditorAssignment(typeof(Vector3D))]
	public class Vector3DPropertyEditor : VectorPropertyEditor
	{
		public override object DisplayedValue
		{
			get 
			{ 
				return new Vector3D((double)this.editor[0].Value, (double)this.editor[1].Value, (double)this.editor[2].Value);
			}
		}


		public Vector3DPropertyEditor() : base(3, 1)
		{
			this.editor[0].Edited += this.editorX_Edited;
			this.editor[1].Edited += this.editorY_Edited;
			this.editor[2].Edited += this.editorZ_Edited;
		}


		protected override void OnGetValue()
		{
			base.OnGetValue();
			object[] values = this.GetValue().ToArray();

			this.BeginUpdate();
			if (!values.Any())
			{
				this.editor[0].Value = 0;
				this.editor[1].Value = 0;
				this.editor[2].Value = 0;
			}
			else
			{
				var valNotNull = values.NotNull();
				double avgX = valNotNull.Average(o => ((Vector3D)o).X);
				double avgY = valNotNull.Average(o => ((Vector3D)o).Y);
				double avgZ = valNotNull.Average(o => ((Vector3D)o).Z);

				this.editor[0].Value = MathD.SafeToDecimal(avgX);
				this.editor[1].Value = MathD.SafeToDecimal(avgY);
				this.editor[2].Value = MathD.SafeToDecimal(avgZ);

				this.multiple[0] = (values.Any(o => o == null) || values.Any(o => ((Vector3D)o).X != avgX));
				this.multiple[1] = (values.Any(o => o == null) || values.Any(o => ((Vector3D)o).Y != avgY));
				this.multiple[2] = (values.Any(o => o == null) || values.Any(o => ((Vector3D)o).Z != avgZ));
			}
			this.EndUpdate();
		}

		private void editorX_Edited(object sender, EventArgs e)
		{
			this.HandleValueEdited<Vector3D>((oldVal, newVal) => new Vector3D(newVal.X, oldVal.Y, oldVal.Z));
		}
		private void editorY_Edited(object sender, EventArgs e)
		{
			this.HandleValueEdited<Vector3D>((oldVal, newVal) => new Vector3D(oldVal.X, newVal.Y, oldVal.Z));
		}
		private void editorZ_Edited(object sender, EventArgs e)
		{
			this.HandleValueEdited<Vector3D>((oldVal, newVal) => new Vector3D(oldVal.X, oldVal.Y, newVal.Z));
		}
	}
}

