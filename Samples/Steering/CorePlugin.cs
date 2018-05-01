﻿using Duality;
using Duality.Resources;

namespace Steering
{
	public class SteeringCorePlugin : CorePlugin
	{
		protected override void OnBeforeUpdate()
		{
			if (DualityApp.ExecContext == DualityApp.ExecutionContext.Game)
			{
				var agents = Scene.Current.FindComponents<Agent>();
				foreach (var agent in agents)
					agent.Update();
			}
		}
	}
}