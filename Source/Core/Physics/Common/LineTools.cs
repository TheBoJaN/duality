using System;
using System.Collections.Generic;
using FarseerPhysics.Collision;
using Duality;

namespace FarseerPhysics.Common
{
	/// <summary>
	/// Collection of helper methods for misc collisions.
	/// Does double tolerance and line collisions with lines and AABBs.
	/// </summary>
	public static class LineTools
	{
		public static double DistanceBetweenPointAndPoint(ref Vector2D point1, ref Vector2D point2)
		{
			Vector2D v;
			Vector2D.Subtract(ref point1, ref point2, out v);
			return v.Length;
		}

		public static double DistanceBetweenPointAndLineSegment(ref Vector2D point, ref Vector2D lineEndPoint1,
															   ref Vector2D lineEndPoint2)
		{
			Vector2D v = lineEndPoint2 - lineEndPoint1;
			Vector2D w = point - lineEndPoint1;

			double c1 = Vector2D.Dot(w, v);
			if (c1 <= 0) return DistanceBetweenPointAndPoint(ref point, ref lineEndPoint1);

			double c2 = Vector2D.Dot(v, v);
			if (c2 <= c1) return DistanceBetweenPointAndPoint(ref point, ref lineEndPoint2);

			double b = c1 / c2;
			Vector2D pointOnLine = lineEndPoint1 + (v * b);
			return DistanceBetweenPointAndPoint(ref point, ref pointOnLine);
		}

		// From Eric Jordan's convex decomposition library
		/// <summary>
		///Check if the lines a0->a1 and b0->b1 cross.
		///If they do, intersectionPoint will be filled
		///with the point of crossing.
		///
		///Grazing lines should not return true.
		/// 
		/// </summary>
		/// <param name="a0"></param>
		/// <param name="a1"></param>
		/// <param name="b0"></param>
		/// <param name="b1"></param>
		/// <param name="intersectionPoint"></param>
		/// <returns></returns>
		public static bool LineIntersect2(Vector2D a0, Vector2D a1, Vector2D b0, Vector2D b1, out Vector2D intersectionPoint)
		{
			intersectionPoint = Vector2D.Zero;

			if (a0 == b0 || a0 == b1 || a1 == b0 || a1 == b1)
				return false;

			double x1 = a0.X;
			double y1 = a0.Y;
			double x2 = a1.X;
			double y2 = a1.Y;
			double x3 = b0.X;
			double y3 = b0.Y;
			double x4 = b1.X;
			double y4 = b1.Y;

			//AABB early exit
			if (Math.Max(x1, x2) < Math.Min(x3, x4) || Math.Max(x3, x4) < Math.Min(x1, x2))
				return false;

			if (Math.Max(y1, y2) < Math.Min(y3, y4) || Math.Max(y3, y4) < Math.Min(y1, y2))
				return false;

			double ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3));
			double ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3));
			double denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
			if (Math.Abs(denom) < Settings.Epsilon)
			{
				//Lines are too close to parallel to call
				return false;
			}
			ua /= denom;
			ub /= denom;

			if ((0 < ua) && (ua < 1) && (0 < ub) && (ub < 1))
			{
				intersectionPoint.X = (x1 + ua * (x2 - x1));
				intersectionPoint.Y = (y1 + ua * (y2 - y1));
				return true;
			}

			return false;
		}

		//From Mark Bayazit's convex decomposition algorithm
		public static Vector2D LineIntersect(Vector2D p1, Vector2D p2, Vector2D q1, Vector2D q2)
		{
			Vector2D i = Vector2D.Zero;
			double a1 = p2.Y - p1.Y;
			double b1 = p1.X - p2.X;
			double c1 = a1 * p1.X + b1 * p1.Y;
			double a2 = q2.Y - q1.Y;
			double b2 = q1.X - q2.X;
			double c2 = a2 * q1.X + b2 * q1.Y;
			double det = a1 * b2 - a2 * b1;

			if (!MathUtils.FloatEquals(det, 0))
			{
				// lines are not parallel
				i.X = (b2 * c1 - b1 * c2) / det;
				i.Y = (a1 * c2 - a2 * c1) / det;
			}
			return i;
		}

		/// <summary>
		/// This method detects if two line segments (or lines) intersect,
		/// and, if so, the point of intersection. Use the <paramref name="firstIsSegment"/> and
		/// <paramref name="secondIsSegment"/> parameters to set whether the intersection point
		/// must be on the first and second line segments. Setting these
		/// both to true means you are doing a line-segment to line-segment
		/// intersection. Setting one of them to true means you are doing a
		/// line to line-segment intersection test, and so on.
		/// Note: If two line segments are coincident, then 
		/// no intersection is detected (there are actually
		/// infinite intersection points).
		/// Author: Jeremy Bell
		/// </summary>
		/// <param name="point1">The first point of the first line segment.</param>
		/// <param name="point2">The second point of the first line segment.</param>
		/// <param name="point3">The first point of the second line segment.</param>
		/// <param name="point4">The second point of the second line segment.</param>
		/// <param name="point">This is set to the intersection
		/// point if an intersection is detected.</param>
		/// <param name="firstIsSegment">Set this to true to require that the 
		/// intersection point be on the first line segment.</param>
		/// <param name="secondIsSegment">Set this to true to require that the
		/// intersection point be on the second line segment.</param>
		/// <returns>True if an intersection is detected, false otherwise.</returns>
		public static bool LineIntersect(ref Vector2D point1, ref Vector2D point2, ref Vector2D point3, ref Vector2D point4,
										 bool firstIsSegment, bool secondIsSegment,
										 out Vector2D point)
		{
			point = new Vector2D();

			// these are reused later.
			// each lettered sub-calculation is used twice, except
			// for b and d, which are used 3 times
			double a = point4.Y - point3.Y;
			double b = point2.X - point1.X;
			double c = point4.X - point3.X;
			double d = point2.Y - point1.Y;

			// denominator to solution of linear system
			double denom = (a * b) - (c * d);

			// if denominator is 0, then lines are parallel
			if (!(denom >= -Settings.Epsilon && denom <= Settings.Epsilon))
			{
				double e = point1.Y - point3.Y;
				double f = point1.X - point3.X;
				double oneOverDenom = 1.0f / denom;

				// numerator of first equation
				double ua = (c * e) - (a * f);
				ua *= oneOverDenom;

				// check if intersection point of the two lines is on line segment 1
				if (!firstIsSegment || ua >= 0.0f && ua <= 1.0f)
				{
					// numerator of second equation
					double ub = (b * e) - (d * f);
					ub *= oneOverDenom;

					// check if intersection point of the two lines is on line segment 2
					// means the line segments intersect, since we know it is on
					// segment 1 as well.
					if (!secondIsSegment || ub >= 0.0f && ub <= 1.0f)
					{
						// check if they are coincident (no collision in this case)
						if (ua != 0f || ub != 0f)
						{
							//There is an intersection
							point.X = point1.X + ua * b;
							point.Y = point1.Y + ua * d;
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// This method detects if two line segments (or lines) intersect,
		/// and, if so, the point of intersection. Use the <paramref name="firstIsSegment"/> and
		/// <paramref name="secondIsSegment"/> parameters to set whether the intersection point
		/// must be on the first and second line segments. Setting these
		/// both to true means you are doing a line-segment to line-segment
		/// intersection. Setting one of them to true means you are doing a
		/// line to line-segment intersection test, and so on.
		/// Note: If two line segments are coincident, then 
		/// no intersection is detected (there are actually
		/// infinite intersection points).
		/// Author: Jeremy Bell
		/// </summary>
		/// <param name="point1">The first point of the first line segment.</param>
		/// <param name="point2">The second point of the first line segment.</param>
		/// <param name="point3">The first point of the second line segment.</param>
		/// <param name="point4">The second point of the second line segment.</param>
		/// <param name="intersectionPoint">This is set to the intersection
		/// point if an intersection is detected.</param>
		/// <param name="firstIsSegment">Set this to true to require that the 
		/// intersection point be on the first line segment.</param>
		/// <param name="secondIsSegment">Set this to true to require that the
		/// intersection point be on the second line segment.</param>
		/// <returns>True if an intersection is detected, false otherwise.</returns>
		public static bool LineIntersect(Vector2D point1, Vector2D point2, Vector2D point3, Vector2D point4,
										 bool firstIsSegment,
										 bool secondIsSegment, out Vector2D intersectionPoint)
		{
			return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment, secondIsSegment,
								 out intersectionPoint);
		}

		/// <summary>
		/// This method detects if two line segments intersect,
		/// and, if so, the point of intersection. 
		/// Note: If two line segments are coincident, then 
		/// no intersection is detected (there are actually
		/// infinite intersection points).
		/// </summary>
		/// <param name="point1">The first point of the first line segment.</param>
		/// <param name="point2">The second point of the first line segment.</param>
		/// <param name="point3">The first point of the second line segment.</param>
		/// <param name="point4">The second point of the second line segment.</param>
		/// <param name="intersectionPoint">This is set to the intersection
		/// point if an intersection is detected.</param>
		/// <returns>True if an intersection is detected, false otherwise.</returns>
		public static bool LineIntersect(ref Vector2D point1, ref Vector2D point2, ref Vector2D point3, ref Vector2D point4,
										 out Vector2D intersectionPoint)
		{
			return LineIntersect(ref point1, ref point2, ref point3, ref point4, true, true, out intersectionPoint);
		}

		/// <summary>
		/// This method detects if two line segments intersect,
		/// and, if so, the point of intersection. 
		/// Note: If two line segments are coincident, then 
		/// no intersection is detected (there are actually
		/// infinite intersection points).
		/// </summary>
		/// <param name="point1">The first point of the first line segment.</param>
		/// <param name="point2">The second point of the first line segment.</param>
		/// <param name="point3">The first point of the second line segment.</param>
		/// <param name="point4">The second point of the second line segment.</param>
		/// <param name="intersectionPoint">This is set to the intersection
		/// point if an intersection is detected.</param>
		/// <returns>True if an intersection is detected, false otherwise.</returns>
		public static bool LineIntersect(Vector2D point1, Vector2D point2, Vector2D point3, Vector2D point4,
										 out Vector2D intersectionPoint)
		{
			return LineIntersect(ref point1, ref point2, ref point3, ref point4, true, true, out intersectionPoint);
		}

		/// <summary>
		/// Get all intersections between a line segment and a list of vertices
		/// representing a polygon. The vertices reuse adjacent points, so for example
		/// edges one and two are between the first and second vertices and between the
		/// second and third vertices. The last edge is between vertex vertices.Count - 1
		/// and verts0. (ie, vertices from a Geometry or AABB)
		/// </summary>
		/// <param name="point1">The first point of the line segment to test</param>
		/// <param name="point2">The second point of the line segment to test.</param>
		/// <param name="vertices">The vertices, as described above</param>
		/// <param name="intersectionPoints">An list of intersection points. Any intersection points
		/// found will be added to this list.</param>
		public static void LineSegmentVerticesIntersect(ref Vector2D point1, ref Vector2D point2, Vertices vertices,
														ref List<Vector2D> intersectionPoints)
		{
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2D point;
				if (LineIntersect(vertices[i], vertices[vertices.NextIndex(i)],
								  point1, point2, true, true, out point))
				{
					intersectionPoints.Add(point);
				}
			}
		}

		/// <summary>
		/// Get all intersections between a line segment and an AABB. 
		/// </summary>
		/// <param name="point1">The first point of the line segment to test</param>
		/// <param name="point2">The second point of the line segment to test.</param>
		/// <param name="aabb">The AABB that is used for testing intersection.</param>
		/// <param name="intersectionPoints">An list of intersection points. Any intersection points found will be added to this list.</param>
		public static void LineSegmentAABBIntersect(ref Vector2D point1, ref Vector2D point2, AABB aabb,
													ref List<Vector2D> intersectionPoints)
		{
			LineSegmentVerticesIntersect(ref point1, ref point2, aabb.Vertices, ref intersectionPoints);
		}
	}
}