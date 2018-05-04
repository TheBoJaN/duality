using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality.Drawing;
using Duality.Resources;

namespace Duality
{
	/// <summary>
	/// Represents a textual description of a certain point in space in the context of <see cref="VisualLog">visual logging</see>.
	/// </summary>
	public sealed class VisualLogTextEntry : VisualLogEntry
	{
		private	Vector3D		pos			= Vector3D.Zero;
		private	Alignment	blockAlign	= Alignment.TopLeft;
		private	string[]	lines		= new string[0];
		
		/// <summary>
		/// [GET / SET] The point in space that is described by this text.
		/// </summary>
		public Vector3D Pos
		{
			get { return this.pos; }
			set { this.pos = value; }
		}
		/// <summary>
		/// [GET / SET] The texts (multiline block) spatial alignment.
		/// </summary>
		public Alignment BlockAlignment
		{
			get { return this.blockAlign; }
			set { this.blockAlign = value; }
		}
		/// <summary>
		/// [GET / SET] The text that will be displayed. Newline characters will
		/// be parsed correctly and be correctly displayed as line breaks.
		/// </summary>
		public string Text
		{
			get { return string.Join(Environment.NewLine, this.lines); }
			set
			{
				this.lines = value.Split('\n');
				for (int i = 0; i < this.lines.Length; i++)
				{
					this.lines[i] = this.lines[i].Trim('\n', '\r');
				}
			}
		}
		/// <summary>
		/// [GET] The displayed text, broken up into distinct lines. Do not modify - use <see cref="Text"/> instead.
		/// </summary>
		public string[] TextLines
		{
			get { return this.lines; }
		}

		public override void Draw(Canvas target, Vector3D basePos, double baseRotation, double baseScale)
		{
			double borderRadius = DefaultOutlineWidth;
			double textScale = 1.0f;
			bool worldSpace = (target.DrawDevice.VisibilityMask & VisibilityFlag.ScreenOverlay) == VisibilityFlag.None;

			// Scale anti-proportional to perspective scale in order to keep a constant size 
			// in screen space even when actually drawing in world space.
			{
				double scale = target.DrawDevice.GetScaleAtZ((float)(this.pos.Z + basePos.Z));
				borderRadius /= scale;
				textScale /= scale;
			}

			// Determine base position
			Vector3D originPos = this.pos;
			MathD.TransformCoord(ref originPos.X, ref originPos.Y, baseRotation, baseScale);
			originPos += basePos;

			// Draw text and background
			target.State.ColorTint = target.State.ColorTint.WithAlpha(target.State.ColorTint.A * 2.0f / 255.0f);
			target.State.ColorTint *= this.Color;
			if (worldSpace) target.State.TransformAngle = target.DrawDevice.ViewerAngle;
			target.State.TransformScale = new Vector2D(textScale, textScale);
			target.DrawText(
				this.lines,
				originPos.X, 
				originPos.Y, 
				originPos.Z,
				this.blockAlign,
				true);
		}
	}
}
