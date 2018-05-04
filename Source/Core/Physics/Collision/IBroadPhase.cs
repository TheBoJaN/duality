﻿using System;
using FarseerPhysics.Dynamics;
using Duality;

namespace FarseerPhysics.Collision
{
	public interface IBroadPhase
	{
		int ProxyCount { get; }
		void UpdatePairs(BroadphaseDelegate callback);

		bool TestOverlap(int proxyIdA, int proxyIdB);

		int AddProxy(ref FixtureProxy proxy);

		void RemoveProxy(int proxyId);

		void MoveProxy(int proxyId, ref AABB aabb, Vector2D displacement);

		FixtureProxy GetProxy(int proxyId);

		void TouchProxy(int proxyId);

		void GetFatAABB(int proxyId, out AABB aabb);

		void Query(Func<int, bool> callback, ref AABB aabb);

		void RayCast(Func<RayCastInput, int, double> callback, ref RayCastInput input);
	}
}