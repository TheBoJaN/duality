#region --- License ---
/*
Copyright (c) 2006 - 2008 The Open Toolkit library.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Note: This code has been heavily modified for the Duality framework.

	*/
#endregion

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Duality
{
	/// <summary>
	/// Represents a 2D vector using two single-precision floating-point numbers.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2D : IEquatable<Vector2D>
	{
		/// <summary>
		/// Defines a unit-length Vector2 that points along the X-axis.
		/// </summary>
		public static readonly Vector2D UnitX = new Vector2D(1, 0);
		/// <summary>
		/// Defines a unit-length Vector2D that points along the Y-axis.
		/// </summary>
		public static readonly Vector2D UnitY = new Vector2D(0, 1);
		/// <summary>
		/// Defines a zero-length Vector2D.
		/// </summary>
		public static readonly Vector2D Zero = new Vector2D(0, 0);
		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector2D One = new Vector2D(1, 1);

		/// <summary>
		/// The X component of the Vector2D.
		/// </summary>
		public double X;
		/// <summary>
		/// The Y component of the Vector2D.
		/// </summary>
		public double Y;

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="value">The value that will initialize this instance.</param>
		public Vector2D(double value)
		{
			this.X = value;
			this.Y = value;
		}
		/// <summary>
		/// Constructs a new Vector2D.
		/// </summary>
		/// <param name="x">The x coordinate of the net Vector2D.</param>
		/// <param name="y">The y coordinate of the net Vector2D.</param>
		public Vector2D(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}
		/// <summary>
		/// Constructs a new vector from angle and length.
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static Vector2D FromAngleLength(double angle, double length)
		{
			return new Vector2D(Math.Sin(angle) * length, Math.Cos(angle) * -length);
		}

		/// <summary>
		/// Gets the length (magnitude) of the vector.
		/// </summary>
		/// <seealso cref="LengthSquared"/>
		public double Length
		{
			get
			{
				return Math.Sqrt(this.X * this.X + this.Y * this.Y);
			}
		}
		/// <summary>
		/// Gets the square of the vector length (magnitude).
		/// </summary>
		/// <remarks>
		/// This property avoids the costly square root operation required by the Length property. This makes it more suitable
		/// for comparisons.
		/// </remarks>
		/// <see cref="Length"/>
		public double LengthSquared
		{
			get
			{
				return this.X * this.X + this.Y * this.Y;
			}
		}
		/// <summary>
		/// Returns the vectors angle
		/// </summary>
		public double Angle
		{
			get
			{
				return (Math.Atan2(this.Y, this.X) + Math.PI * 2.5) % (Math.PI * 2);
			}
		}

		/// <summary>
		/// Gets the perpendicular vector on the right side of this vector.
		/// </summary>
		public Vector2D PerpendicularRight
		{
			get
			{
				return new Vector2D(-this.Y, this.X);
			}
		}
		/// <summary>
		/// Gets the perpendicular vector on the left side of this vector.
		/// </summary>
		public Vector2D PerpendicularLeft
		{
			get
			{
				return new Vector2D(this.Y, -this.X);
			}
		}
		/// <summary>
		/// Returns a normalized version of this vector.
		/// </summary>
		public Vector2D Normalized
		{
			get
			{
				double length = this.Length;
				if (length < 1e-15f) return Vector2D.Zero;

				double scale = 1.0f / length;
				return new Vector2D(
					this.X * scale,
					this.Y * scale);
			}
		}

		/// <summary>
		/// Gets or sets the value at the index of the Vector.
		/// </summary>
		public double this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return this.X;
					case 1: return this.Y;
					default: throw new IndexOutOfRangeException("Vector2D access at index: " + index);
				}
			}
			set
			{
				switch (index)
				{
					case 0: this.X = value; return;
					case 1: this.Y = value; return;
					default: throw new IndexOutOfRangeException("Vector2D access at index: " + index);
				}
			}
		}


		/// <summary>
		/// Scales the Vector2D to unit length.
		/// </summary>
		public void Normalize()
		{
			double length = this.Length;
			if (length == 0)
			{
				this = Vector2D.Zero;
			}
			else
			{
				double scale = 1.0f / length;
				this.X *= scale;
				this.Y *= scale;
			}
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <param name="result">Result of operation.</param>
		public static void Add(ref Vector2D a, ref Vector2D b, out Vector2D result)
		{
			result = new Vector2D(a.X + b.X, a.Y + b.Y);
		}
		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">Result of subtraction</param>
		public static void Subtract(ref Vector2D a, ref Vector2D b, out Vector2D result)
		{
			result = new Vector2D(a.X - b.X, a.Y - b.Y);
		}
		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector2D vector, double scale, out Vector2D result)
		{
			result = new Vector2D(vector.X * scale, vector.Y * scale);
		}
		/// <summary>
		/// Multiplies a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector2D vector, ref Vector2D scale, out Vector2D result)
		{
			result = new Vector2D(vector.X * scale.X, vector.Y * scale.Y);
		}
		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector2D vector, double scale, out Vector2D result)
		{
			Multiply(ref vector, 1 / scale, out result);
		}
		/// <summary>
		/// Divide a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector2D vector, ref Vector2D scale, out Vector2D result)
		{
			result = new Vector2D(vector.X / scale.X, vector.Y / scale.Y);
		}

		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise minimum</returns>
		public static Vector2D Min(Vector2D a, Vector2D b)
		{
			a.X = a.X < b.X ? a.X : b.X;
			a.Y = a.Y < b.Y ? a.Y : b.Y;
			return a;
		}
		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise minimum</param>
		public static void Min(ref Vector2D a, ref Vector2D b, out Vector2D result)
		{
			result.X = a.X < b.X ? a.X : b.X;
			result.Y = a.Y < b.Y ? a.Y : b.Y;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise maximum</returns>
		public static Vector2D Max(Vector2D a, Vector2D b)
		{
			a.X = a.X > b.X ? a.X : b.X;
			a.Y = a.Y > b.Y ? a.Y : b.Y;
			return a;
		}
		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise maximum</param>
		public static void Max(ref Vector2D a, ref Vector2D b, out Vector2D result)
		{
			result.X = a.X > b.X ? a.X : b.X;
			result.Y = a.Y > b.Y ? a.Y : b.Y;
		}

		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The dot product of the two inputs</returns>
		public static double Dot(Vector2D left, Vector2D right)
		{
			return left.X * right.X + left.Y * right.Y;
		}
		/// <summary>
		/// Calculate the dot (scalar) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <param name="result">The dot product of the two inputs</param>
		public static void Dot(ref Vector2D left, ref Vector2D right, out double result)
		{
			result = left.X * right.X + left.Y * right.Y;
		}

		/// <summary>
		/// Returns a new Vector that is the linear blend of the 2 given Vectors
		/// </summary>
		/// <param name="a">First input vector</param>
		/// <param name="b">Second input vector</param>
		/// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
		/// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
		public static Vector2D Lerp(Vector2D a, Vector2D b, double blend)
		{
			a.X = blend * (b.X - a.X) + a.X;
			a.Y = blend * (b.Y - a.Y) + a.Y;
			return a;
		}
		/// <summary>
		/// Returns a new Vector that is the linear blend of the 2 given Vectors
		/// </summary>
		/// <param name="a">First input vector</param>
		/// <param name="b">Second input vector</param>
		/// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
		/// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
		public static void Lerp(ref Vector2D a, ref Vector2D b, double blend, out Vector2D result)
		{
			result.X = blend * (b.X - a.X) + a.X;
			result.Y = blend * (b.Y - a.Y) + a.Y;
		}

		/// <summary>
		/// Calculates the angle (in radians) between two vectors.
		/// </summary>
		/// <param name="first">The first vector.</param>
		/// <param name="second">The second vector.</param>
		/// <returns>Angle (in radians) between the vectors.</returns>
		/// <remarks>Note that the returned angle is never bigger than the constant Pi.</remarks>
		public static double AngleBetween(Vector2D first, Vector2D second)
		{
			return Math.Acos((Vector2D.Dot(first, second)) / (first.Length * second.Length));
		}
		/// <summary>
		/// Calculates the angle (in radians) between two vectors.
		/// </summary>
		/// <param name="first">The first vector.</param>
		/// <param name="second">The second vector.</param>
		/// <param name="result">Angle (in radians) between the vectors.</param>
		/// <remarks>Note that the returned angle is never bigger than the constant Pi.</remarks>
		public static void AngleBetween(ref Vector2D first, ref Vector2D second, out double result)
		{
			double temp;
			Vector2D.Dot(ref first, ref second, out temp);
			result = Math.Acos(temp / (first.Length * second.Length));
		}

		/// <summary>
		/// Transforms a vector by a quaternion rotation.
		/// </summary>
		/// <param name="vec">The vector to transform.</param>
		/// <param name="quat">The quaternion to rotate the vector by.</param>
		/// <returns>The result of the operation.</returns>
		public static Vector2D Transform(Vector2D vec, QuaternionD quat)
		{
			Vector2D result;
			Transform(ref vec, ref quat, out result);
			return result;
		}
		/// <summary>
		/// Transforms a vector by a quaternion rotation.
		/// </summary>
		/// <param name="vec">The vector to transform.</param>
		/// <param name="quat">The quaternion to rotate the vector by.</param>
		/// <param name="result">The result of the operation.</param>
		public static void Transform(ref Vector2D vec, ref QuaternionD quat, out Vector2D result)
		{
			QuaternionD v = new QuaternionD(vec.X, vec.Y, 0, 0), i, t;
			QuaternionD.Invert(ref quat, out i);
			QuaternionD.Multiply(ref quat, ref v, out t);
			QuaternionD.Multiply(ref t, ref i, out v);

			result = new Vector2D(v.X, v.Y);
		}
		/// <summary>
		/// Transforms the vector
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="mat"></param>
		/// <returns></returns>
		public static Vector2D Transform(Vector2D vec, Matrix4D mat)
		{
			Vector2D result;
			Transform(ref vec, ref mat, out result);
			return result;
		}
		/// <summary>
		/// Transforms the vector
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="mat"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static void Transform(ref Vector2D vec, ref Matrix4D mat, out Vector2D result)
		{
			Vector4D row0 = mat.Row0;
			Vector4D row1 = mat.Row1;
			Vector4D row3 = mat.Row3;
			result.X = vec.X * row0.X + vec.Y * row1.X + row3.X;
			result.Y = vec.X * row0.Y + vec.Y * row1.Y + row3.Y;
		}

		/// <summary>
		/// Adds the specified instances.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>Result of addition.</returns>
		public static Vector2D operator +(Vector2D left, Vector2D right)
		{
			return new Vector2D(
				left.X + right.X, 
				left.Y + right.Y);
		}
		/// <summary>
		/// Subtracts the specified instances.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>Result of subtraction.</returns>
		public static Vector2D operator -(Vector2D left, Vector2D right)
		{
			return new Vector2D(
				left.X - right.X, 
				left.Y - right.Y);
		}
		/// <summary>
		/// Negates the specified instance.
		/// </summary>
		/// <param name="vec">Operand.</param>
		/// <returns>Result of negation.</returns>
		public static Vector2D operator -(Vector2D vec)
		{
			return new Vector2D(
				-vec.X, 
				-vec.Y);
		}
		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="vec">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2D operator *(Vector2D vec, double scale)
		{
			return new Vector2D(
				vec.X * scale, 
				vec.Y * scale);
		}
		/// <summary>
		/// Multiplies the specified instance by a scalar.
		/// </summary>
		/// <param name="scale">Left operand.</param>
		/// <param name="vec">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2D operator *(double scale, Vector2D vec)
		{
			return vec * scale;
		}
		/// <summary>
		/// Scales the specified instance by a vector.
		/// </summary>
		/// <param name="vec">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static Vector2D operator *(Vector2D vec, Vector2D scale)
		{
			return new Vector2D(
				vec.X * scale.X, 
				vec.Y * scale.Y);
		}
		/// <summary>
		/// Divides the specified instance by a scalar.
		/// </summary>
		/// <param name="vec">Left operand</param>
		/// <param name="scale">Right operand</param>
		/// <returns>Result of the division.</returns>
		public static Vector2D operator /(Vector2D vec, double scale)
		{
			return vec * (1.0f / scale);
		}
		/// <summary>
		/// Divides the specified instance by a vector.
		/// </summary>
		/// <param name="vec">Left operand</param>
		/// <param name="scale">Right operand</param>
		/// <returns>Result of the division.</returns>
		public static Vector2D operator /(Vector2D vec, Vector2D scale)
		{
			return new Vector2D(
				vec.X / scale.X, 
				vec.Y / scale.Y);
		}
		/// <summary>
		/// Compares the specified instances for equality.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>True if both instances are equal; false otherwise.</returns>
		public static bool operator ==(Vector2D left, Vector2D right)
		{
			return left.Equals(right);
		}
		/// <summary>
		/// Compares the specified instances for inequality.
		/// </summary>
		/// <param name="left">Left operand.</param>
		/// <param name="right">Right operand.</param>
		/// <returns>True if both instances are not equal; false otherwise.</returns>
		public static bool operator !=(Vector2D left, Vector2D right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Returns a System.String that represents the current Vector2D.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("({0:F}, {1:F})", this.X, this.Y);
		}
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
		}
		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Vector2D))
				return false;

			return this.Equals((Vector2D)obj);
		}

		/// <summary>
		/// Indicates whether the current vector is equal to another vector.
		/// </summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals(Vector2D other)
		{
			return
				this.X == other.X &&
				this.Y == other.Y;
		}

		/// <summary>
		/// Convert from Vector2D
		/// </summary>
		/// <param name="v"></param>
		public static implicit operator Vector2D(Vector2 v)
		{
			return new Vector2D(v.X, v.Y);
		}

		/// <summary>
		/// Convert to Vector2
		/// </summary>
		/// <param name="v"></param>
		public static implicit operator Vector2(Vector2D v)
		{
			return new Vector2((float)v.X, (float)v.Y);
		}
	}
}
