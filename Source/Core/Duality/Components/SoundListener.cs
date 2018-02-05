﻿using System;

using Duality.Properties;
using Duality.Editor;

namespace Duality.Components
{
	/// <summary>
	/// Makes this <see cref="GameObject"/> the 3d sound listener.
	/// </summary>
	[RequiredComponent(typeof(Transform))]
	[RequiredComponent(typeof(VelocityTracker))]
	[EditorHintCategory(CoreResNames.CategorySound)]
	[EditorHintImage(CoreResNames.ImageSoundListener)]
	public sealed class SoundListener : Component, ICmpInitializable
	{
		public Vector3 Position
		{
			get { return this.GameObj.Transform.Pos; }
		}
		public Vector3 Velocity
		{
			get { return this.GameObj.GetComponent<VelocityTracker>().Vel; }
		}
		public float Angle
		{
			get { return this.GameObj.Transform.Angle; }
		}

		public void MakeCurrent()
		{
			if (!this.Active) return;
			DualityApp.Sound.Listener = this;
		}

		void ICmpInitializable.OnInit(InitContext context)
		{
			if (DualityApp.ExecContext != DualityApp.ExecutionContext.Editor && context == InitContext.Activate)
				this.MakeCurrent();
		}
		void ICmpInitializable.OnShutdown(ShutdownContext context)
		{
		}
	}
}
