﻿using System;

namespace Duality
{
	/// <summary>
	/// Describes a rectangular area.
	/// </summary>
	public struct RectD : IEquatable<RectD>
	{
		/// <summary>
		/// An empty RectD.
		/// </summary>
		public static readonly RectD Empty = new RectD(0, 0, 0, 0);

		/// <summary>
		/// The Rects x-Coordinate.
		/// </summary>
		public	double	X;
		/// <summary>
		/// The Rects y-Coordinate.
		/// </summary>
		public	double	Y;
		/// <summary>
		/// The Rects width.
		/// </summary>
		public	double	W;
		/// <summary>
		/// The Rects height.
		/// </summary>
		public	double	H;

		/// <summary>
		/// [GET / SET] The Rects position
		/// </summary>
		public Vector2D Pos
		{
			get { return new Vector2D(this.X, this.Y); }
			set { this.X = value.X; this.Y = value.Y; }
		}
		/// <summary>
		/// [GET / SET] The Rects size.
		/// </summary>
		public Vector2D Size
		{
			get { return new Vector2D(this.W, this.H); }
			set { this.W = value.X; this.H = value.Y; }
		}

		/// <summary>
		/// [GET] The minimum x-Coordinate occupied by the RectD. Accounts for negative sizes.
		/// </summary>
		public double LeftX
		{
			get { return MathD.Min(this.X, this.X + this.W); }
		}
		/// <summary>
		/// [GET] The minimum y-Coordinate occupied by the RectD. Accounts for negative sizes.
		/// </summary>
		public double TopY
		{
			get { return MathD.Min(this.Y, this.Y + this.H); }
		}
		/// <summary>
		/// [GET] The maximum y-Coordinate occupied by the RectD. Accounts for negative sizes.
		/// </summary>
		public double BottomY
		{
			get { return MathD.Max(this.Y, this.Y + this.H); }
		}
		/// <summary>
		/// [GET] The maximum x-Coordinate occupied by the RectD. Accounts for negative sizes.
		/// </summary>
		public double RightX
		{
			get { return MathD.Max(this.X, this.X + this.W); }
		}
		/// <summary>
		/// [GET] The center x-Coordinate occupied by the RectD.
		/// </summary>
		public double CenterX
		{
			get { return this.X + this.W * 0.5f; }
		}
		/// <summary>
		/// [GET] The center y-Coordinate occupied by the RectD.
		/// </summary>
		public double CenterY
		{
			get { return this.Y + this.H * 0.5f; }
		}

		/// <summary>
		/// [GET] The Rects top left coordinates
		/// </summary>
		public Vector2D TopLeft
		{
			get { return new Vector2D(this.LeftX, this.TopY); }
		}
		/// <summary>
		/// [GET] The Rects top right coordinates
		/// </summary>
		public Vector2D TopRight
		{
			get { return new Vector2D(this.RightX, this.TopY); }
		}
		/// <summary>
		/// [GET] The Rects bottom left coordinates
		/// </summary>
		public Vector2D BottomLeft
		{
			get { return new Vector2D(this.LeftX, this.BottomY); }
		}
		/// <summary>
		/// [GET] The Rects bottom right coordinates
		/// </summary>
		public Vector2D BottomRight
		{
			get { return new Vector2D(this.RightX, this.BottomY); }
		}
		/// <summary>
		/// [GET] The Rects center coordinates
		/// </summary>
		public Vector2D Center
		{
			get { return new Vector2D(this.CenterX, this.CenterY); }
		}

		/// <summary>
		/// [GET] If this RectD was to fit inside a bounding circle originating from [0,0],
		/// this would be its radius.
		/// </summary>
		public double BoundingRadius
		{
			get
			{
				return MathD.Distance(
					Math.Max(Math.Abs(this.X), Math.Abs(this.X + this.W)), 
					Math.Max(Math.Abs(this.Y), Math.Abs(this.Y + this.H)));
			}
		}
		
		/// <summary>
		/// Creates a RectD of the given size.
		/// </summary>
		/// <param name="size"></param>
		public RectD(Vector2D size)
		{
			this.X = 0;
			this.Y = 0;
			this.W = size.X;
			this.H = size.Y;
		}
		/// <summary>
		/// Creates a RectD of the given size.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="h"></param>
		public RectD(double w, double h)
		{
			this.X = 0;
			this.Y = 0;
			this.W = w;
			this.H = h;
		}
		/// <summary>
		/// Creates a RectD of the given size and position.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		public RectD(double x, double y, double w, double h)
		{
			this.X = x;
			this.Y = y;
			this.W = w;
			this.H = h;
		}

		/// <summary>
		/// Returns a new version of this RectD that has been moved by the specified offset.
		/// </summary>
		/// <param name="x">Movement in x-Direction.</param>
		/// <param name="y">Movement in y-Direction.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD WithOffset(double x, double y)
		{
			RectD newRect = this;
			newRect.X += x;
			newRect.Y += y;
			return newRect;
		}
		/// <summary>
		/// Returns a new version of this RectD that has been moved by the specified offset.
		/// </summary>
		/// <param name="offset">Movement vector.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD WithOffset(Vector2D offset)
		{
			RectD newRect = this;
			newRect.X += offset.X;
			newRect.Y += offset.Y;
			return newRect;
		}

		/// <summary>
		/// Returns a new version of this RectD that has been scaled by the specified factor.
		/// Scaling only affects a Rects size, not its position.
		/// </summary>
		/// <param name="x">x-Scale factor.</param>
		/// <param name="y">y-Scale factor.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD Scaled(double x, double y)
		{
			RectD newRect = this;
			newRect.W *= x;
			newRect.H *= y;
			return newRect;
		}
		/// <summary>
		/// Returns a new version of this RectD that has been scaled by the specified factor.
		/// Scaling only affects a Rects size, not its position.
		/// </summary>
		/// <param name="factor">Scale factor.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD Scaled(Vector2D factor)
		{
			RectD newRect = this;
			newRect.W *= factor.X;
			newRect.H *= factor.Y;
			return newRect;
		}
		/// <summary>
		/// Returns a new version of this RectD that has been transformed by the specified scale factor.
		/// Transforming both affects a Rects size and position.
		/// </summary>
		/// <param name="x">x-Scale factor.</param>
		/// <param name="y">y-Scale factor.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD Transformed(double x, double y)
		{
			RectD newRect = this;
			newRect.X *= x;
			newRect.Y *= y;
			newRect.W *= x;
			newRect.H *= y;
			return newRect;
		}
		/// <summary>
		/// Returns a new version of this RectD that has been transformed by the specified scale factor.
		/// Transforming both affects a Rects size and position.
		/// </summary>
		/// <param name="scale">Scale factor.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD Transformed(Vector2D scale)
		{
			RectD newRect = this;
			newRect.X *= scale.X;
			newRect.Y *= scale.Y;
			newRect.W *= scale.X;
			newRect.H *= scale.Y;
			return newRect;
		}

		/// <summary>
		/// Returns a new version of this RectD that has been expanded to contain
		/// the specified rectangular area.
		/// </summary>
		/// <param name="x">x-Coordinate of the RectD to contain.</param>
		/// <param name="y">y-Coordinate of the RectD to contain.</param>
		/// <param name="w">Width of the RectD to contain.</param>
		/// <param name="h">Height of the RectD to contain.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD ExpandedToContain(double x, double y, double w, double h)
		{
			return this.ExpandedToContain(x, y).ExpandedToContain(x + w, y + h);
		}
		/// <summary>
		/// Returns a new version of this RectD that has been expanded to contain
		/// the specified RectD.
		/// </summary>
		/// <param name="other">The RectD to contain.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD ExpandedToContain(RectD other)
		{
			return this.ExpandedToContain(other.X, other.Y).ExpandedToContain(other.X + other.W, other.Y + other.H);
		}
		/// <summary>
		/// Returns a new version of this RectD that has been expanded to contain
		/// the specified point.
		/// </summary>
		/// <param name="x">x-Coordinate of the point to contain.</param>
		/// <param name="y">y-Coordinate of the point to contain.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD ExpandedToContain(double x, double y)
		{
			RectD newRect = this;
			if (x < newRect.X)
			{
				newRect.W += newRect.X - x;
				newRect.X = x;
			}
			if (y < newRect.Y)
			{
				newRect.H += newRect.Y - y;
				newRect.Y = y;
			}
			if (x > newRect.X + newRect.W) newRect.W = x - newRect.X;
			if (y > newRect.Y + newRect.H) newRect.H = y - newRect.Y;
			return newRect;
		}
		/// <summary>
		/// Returns a new version of this RectD that has been expanded to contain
		/// the specified point.
		/// </summary>
		/// <param name="p">The point to contain.</param>
		/// <returns>A new RectD with the specified adjustments.</returns>
		public RectD ExpandedToContain(Vector2D p)
		{
			return this.ExpandedToContain(p.X, p.Y);
		}

		/// <summary>
		/// Returns a normalized version of the rect, i.e. one with a positive width and height.
		/// </summary>
		/// <returns></returns>
		public RectD Normalized()
		{
			RectD normalized = this;
			if (normalized.W < 0)
			{
				normalized.X += normalized.W;
				normalized.W = -normalized.W;
			}
			if (normalized.H < 0)
			{
				normalized.Y += normalized.H;
				normalized.H = -normalized.H;
			}
			return normalized;
		}

		/// <summary>
		/// Returns whether this RectD contains a given point.
		/// </summary>
		/// <param name="x">x-Coordinate of the point to test.</param>
		/// <param name="y">y-Coordinate of the point to test.</param>
		/// <returns>True, if the RectD contains the point, false if not.</returns>
		public bool Contains(double x, double y)
		{
			return x >= this.LeftX && x <= this.RightX && y >= this.TopY && y <= this.BottomY;
		}
		/// <summary>
		/// Returns whether this RectD contains a given point.
		/// </summary>
		/// <param name="pos">The point to test.</param>
		/// <returns>True, if the RectD contains the point, false if not.</returns>
		public bool Contains(Vector2D pos)
		{
			return pos.X >= this.LeftX && pos.X <= this.RightX && pos.Y >= this.TopY && pos.Y <= this.BottomY;
		}
		/// <summary>
		/// Returns whether this RectD contains a given rectangular area.
		/// </summary>
		/// <param name="x">x-Coordinate of the RectD to test.</param>
		/// <param name="y">y-Coordinate of the RectD to test.</param>
		/// <param name="w">Width of the RectD to test.</param>
		/// <param name="h">Height of the RectD to test.</param>
		/// <returns>True, if the RectD contains the other RectD, false if not.</returns>
		public bool Contains(double x, double y, double w, double h)
		{
			return this.Contains(x, y) && this.Contains(x + w, y + h);
		}
		/// <summary>
		/// Returns whether this RectD contains a given rectangular area.
		/// </summary>
		/// <param name="rect">The RectD to test.</param>
		/// <returns>True, if the RectD contains the other RectD, false if not.</returns>
		public bool Contains(RectD rect)
		{
			return this.Contains(rect.X, rect.Y) && this.Contains(rect.X + rect.W, rect.Y + rect.H);
		}
		
		/// <summary>
		/// Returns whether this RectD intersects a given rectangular area.
		/// </summary>
		/// <param name="x">x-Coordinate of the RectD to test.</param>
		/// <param name="y">y-Coordinate of the RectD to test.</param>
		/// <param name="w">Width of the RectD to test.</param>
		/// <param name="h">Height of the RectD to test.</param>
		/// <returns>True, if the RectD intersects the other RectD, false if not.</returns>
		public bool Intersects(double x, double y, double w, double h)
		{
			return this.Intersects(new RectD(x, y, w, h));
		}
		/// <summary>
		/// Returns whether this RectD intersects a given rectangular area.
		/// </summary>
		/// <param name="rect">The RectD to test.</param>
		/// <returns>True, if the RectD intersects the other RectD, false if not.</returns>
		public bool Intersects(RectD rect)
		{
			rect = rect.Normalized();
			RectD norm = this.Normalized();
			if (norm.X > (rect.X + rect.W) || (norm.X + norm.W) < rect.X) return false;
			if (norm.Y > (rect.Y + rect.H) || (norm.Y + norm.H) < rect.Y) return false;
			return true;
		}
		/// <summary>
		/// Returns a RectD that equals this Rects intersection with another RectD.
		/// </summary>
		/// <param name="x">x-Coordinate of the RectD to intersect with.</param>
		/// <param name="y">y-Coordinate of the RectD to intersect with.</param>
		/// <param name="w">Width of the RectD to intersect with.</param>
		/// <param name="h">Height of the RectD to intersect with.</param>
		/// <returns>A new RectD that describes both Rects intersection area. <see cref="Empty"/>, if there is no intersection.</returns>
		public RectD Intersection(double x, double y, double w, double h)
		{
			return this.Intersection(new RectD(x, y, w, h));
		}
		/// <summary>
		/// Returns a RectD that equals this Rects intersection with another RectD.
		/// </summary>
		/// <param name="rect">The other RectD to intersect with.</param>
		/// <returns>A new RectD that describes both Rects intersection area. <see cref="Empty"/>, if there is no intersection.</returns>
		public RectD Intersection(RectD rect)
		{
			rect = rect.Normalized();
			RectD norm = this.Normalized();

			double tempWidth = Math.Min(rect.W, norm.W - (rect.X - norm.X));
			double tempHeight = Math.Min(rect.H, norm.H - (rect.Y - norm.Y));
			if ((norm.X - rect.X) > 0.0f) tempWidth -= (norm.X - rect.X);
			if ((norm.Y - rect.Y) > 0.0f) tempHeight -= (norm.Y - rect.Y);

			RectD result = new RectD(
				Math.Max(norm.X, rect.X),
				Math.Max(norm.Y, rect.Y),
				Math.Min(norm.W, tempWidth),
				Math.Min(norm.H, tempHeight));

			return (result.W == 0 || result.H == 0) ? RectD.Empty : result;
		}

		/// <summary>
		/// Tests if two Rects are equal.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(RectD other)
		{
			return 
				this.X == other.X &&
				this.Y == other.Y &&
				this.W == other.W &&
				this.H == other.H;
		}
		public override bool Equals(object obj)
		{
			if (!(obj is RectD))
				return false;
			else
				return this.Equals((RectD)obj);
		}
		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.W.GetHashCode() ^ this.H.GetHashCode();
		}
		public override string ToString()
		{
			return string.Format("RectD ({0}, {1}, {2}, {3})", this.X, this.Y, this.W, this.H);
		}

		/// <summary>
		/// Creates a RectD using x and y Coordinates that are assumed to be aligned as specified.
		/// </summary>
		/// <param name="align">The alignment of the Rects x and y Coordinates.</param>
		/// <param name="x">The Rects x-Coordinate.</param>
		/// <param name="y">The Rects y-Coordinate.</param>
		/// <param name="w">The Rects width.</param>
		/// <param name="h">The Rects height.</param>
		/// <returns></returns>
		public static RectD Align(Alignment align, double x, double y, double w, double h)
		{
			switch (align)
			{
				default:
				case Alignment.TopLeft:		return new RectD(x, y, w, h);
				case Alignment.TopRight:	return new RectD(x - w, y, w, h);
				case Alignment.BottomLeft:	return new RectD(x, y - h, w, h);
				case Alignment.BottomRight:	return new RectD(x - w, y - h, w, h);
				case Alignment.Center:		return new RectD(x - w * 0.5f, y - h * 0.5f, w, h);
				case Alignment.Bottom:		return new RectD(x - w * 0.5f, y - h, w, h);
				case Alignment.Left:		return new RectD(x, y - h * 0.5f, w, h);
				case Alignment.Right:		return new RectD(x - w, y - h * 0.5f, w, h);
				case Alignment.Top:			return new RectD(x - w * 0.5f, y, w, h);
			}
		}

		/// <summary>
		/// Returns whether two Rects are equal.
		/// </summary>
		/// <param name="left">The first RectD.</param>
		/// <param name="right">The second RectD.</param>
		/// <returns></returns>
		public static bool operator ==(RectD left, RectD right)
		{
			return left.Equals(right);
		}
		/// <summary>
		/// Returns whether two Rects are unequal.
		/// </summary>
		/// <param name="left">The first RectD.</param>
		/// <param name="right">The second RectD.</param>
		/// <returns></returns>
		public static bool operator !=(RectD left, RectD right)
		{
			return !left.Equals(right);
		}
	}
}
