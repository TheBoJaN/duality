using System;
using System.Linq;

using Duality;

namespace Duality.Editor.Controls.PropertyEditors
{
	[PropertyEditorAssignment(typeof(Vector2D))]
	public class Vector2DPropertyEditor : VectorPropertyEditor
	{
		public override object DisplayedValue
		{
			get 
			{ 
				return new Vector2D((double)this.editor[0].Value, (double)this.editor[1].Value);
			}
		}


		public Vector2DPropertyEditor() : base(2, 1)
		{
			this.editor[0].Edited += this.editorX_Edited;
			this.editor[1].Edited += this.editorY_Edited;
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
			}
			else
			{
				var valNotNull = values.NotNull();
				double avgX = valNotNull.Average(o => ((Vector2D)o).X);
				double avgY = valNotNull.Average(o => ((Vector2D)o).Y);

				this.editor[0].Value = MathD.SafeToDecimal(avgX);
				this.editor[1].Value = MathD.SafeToDecimal(avgY);

				this.multiple[0] = (values.Any(o => o == null) || values.Any(o => ((Vector2D)o).X != avgX));
				this.multiple[1] = (values.Any(o => o == null) || values.Any(o => ((Vector2D)o).Y != avgY));
			}
			this.EndUpdate();
		}

		private void editorX_Edited(object sender, EventArgs e)
		{
			this.HandleValueEdited<Vector2D>((oldVal, newVal) => new Vector2D(newVal.X, oldVal.Y));
		}
		private void editorY_Edited(object sender, EventArgs e)
		{
			this.HandleValueEdited<Vector2D>((oldVal, newVal) => new Vector2D(oldVal.X, newVal.Y));
		}
	}
}

