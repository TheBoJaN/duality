using System;
using System.Linq;

using AdamsLair.WinForms.PropertyEditing.Templates;

using Duality;

namespace Duality.Editor.Controls.PropertyEditors
{
	[PropertyEditorAssignment(typeof(RectD))]
	public class RectDPropertyEditor : VectorPropertyEditor
	{
		public override object DisplayedValue
		{
			get 
			{ 
				return new RectD((double)this.editor[0].Value, (double)this.editor[1].Value, (double)this.editor[2].Value, (double)this.editor[3].Value);
			}
		}


		public RectDPropertyEditor() : base(4, 2)
		{
			this.editor[0].Edited += this.editorX_Edited;
			this.editor[1].Edited += this.editorY_Edited;
			this.editor[2].Edited += this.editorW_Edited;
			this.editor[3].Edited += this.editorH_Edited;
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
				this.editor[3].Value = 0;
			}
			else
			{
				var valNotNull = values.NotNull();
				double avgX = valNotNull.Average(o => ((RectD)o).X);
				double avgY = valNotNull.Average(o => ((RectD)o).Y);
				double avgW = valNotNull.Average(o => ((RectD)o).W);
				double avgH = valNotNull.Average(o => ((RectD)o).H);

				this.editor[0].Value = MathD.SafeToDecimal(avgX);
				this.editor[1].Value = MathD.SafeToDecimal(avgY);
				this.editor[2].Value = MathD.SafeToDecimal(avgW);
				this.editor[3].Value = MathD.SafeToDecimal(avgH);

				this.multiple[0] = (values.Any(o => o == null) || values.Any(o => ((RectD)o).X != avgX));
				this.multiple[1] = (values.Any(o => o == null) || values.Any(o => ((RectD)o).Y != avgY));
				this.multiple[2] = (values.Any(o => o == null) || values.Any(o => ((RectD)o).W != avgW));
				this.multiple[3] = (values.Any(o => o == null) || values.Any(o => ((RectD)o).H != avgH));
			}
			this.EndUpdate();
		}
		protected override void ApplyDefaultSubEditorConfig(NumericEditorTemplate subEditor)
		{
			base.ApplyDefaultSubEditorConfig(subEditor);
			subEditor.DecimalPlaces = 0;
			subEditor.Increment = 1;
		}

		private void editorX_Edited(object sender, EventArgs e)
		{
			this.HandleValueEdited<RectD>((oldVal, newVal) => new RectD(newVal.X, oldVal.Y, oldVal.W, oldVal.H));
		}
		private void editorY_Edited(object sender, EventArgs e)
		{
			this.HandleValueEdited<RectD>((oldVal, newVal) => new RectD(oldVal.X, newVal.Y, oldVal.W, oldVal.H));
		}
		private void editorW_Edited(object sender, EventArgs e)
		{
			this.HandleValueEdited<RectD>((oldVal, newVal) => new RectD(oldVal.X, oldVal.Y, newVal.W, oldVal.H));
		}
		private void editorH_Edited(object sender, EventArgs e)
		{
			this.HandleValueEdited<RectD>((oldVal, newVal) => new RectD(oldVal.X, oldVal.Y, oldVal.W, newVal.H));
		}
	}
}

