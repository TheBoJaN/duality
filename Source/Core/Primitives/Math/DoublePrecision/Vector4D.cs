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

namespace Duality
{
	/// <summary>
	/// Represents a 4D vector using four single-precision floating-point numbers.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4D : IEquatable<Vector4D>
	{
		/// <summary>
		/// Defines a unit-length Vector4D that points towards the X-axis.
		/// </summary>
		public static Vector4D UnitX = new Vector4D(1, 0, 0, 0);
		/// <summary>
		/// Defines a unit-length Vector4D that points towards the Y-axis.
		/// </summary>
		public static Vector4D UnitY = new Vector4D(0, 1, 0, 0);
		/// <summary>
		/// Defines a unit-length Vector4D that points towards the Z-axis.
		/// </summary>
		public static Vector4D UnitZ = new Vector4D(0, 0, 1, 0);
		/// <summary>
		/// Defines a unit-length Vector4D that points towards the W-axis.
		/// </summary>
		public static Vector4D UnitW = new Vector4D(0, 0, 0, 1);
		/// <summary>
		/// Defines a zero-length Vector4D.
		/// </summary>
		public static Vector4D Zero = new Vector4D(0, 0, 0, 0);
		/// <summary>
		/// Defines an instance with all components set to 1.
		/// </summary>
		public static readonly Vector4D One = new Vector4D(1, 1, 1, 1);

		/// <summary>
		/// The X component of the Vector4D.
		/// </summary>
		public double X;
		/// <summary>
		/// The Y component of the Vector4D.
		/// </summary>
		public double Y;
		/// <summary>
		/// The Z component of the Vector4D.
		/// </summary>
		public double Z;
		/// <summary>
		/// The W component of the Vector4D.
		/// </summary>
		public double W;

		/// <summary>
		/// Gets or sets an OpenTK.Vector2D with the X and Y components of this instance.
		/// </summary>
		public Vector2D Xy { get { return new Vector2D(this.X, this.Y); } set { this.X = value.X; this.Y = value.Y; } }
		/// <summary>
		/// Gets or sets an OpenTK.Vector3D with the X, Y and Z components of this instance.
		/// </summary>
		public Vector3D Xyz { get { return new Vector3D(this.X, this.Y, this.Z); } set { this.X = value.X; this.Y = value.Y; this.Z = value.Z; } }

		
		/// <summary>
		/// Gets the length (magnitude) of the vector.
		/// </summary>
		/// <seealso cref="LengthSquared"/>
		public double Length
		{
			get
			{
				return Math.Sqrt(
					this.X * this.X +
					this.Y * this.Y +
					this.Z * this.Z +
					this.W * this.W);
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
				return 
					this.X * this.X + 
					this.Y * this.Y + 
					this.Z * this.Z + 
					this.W * this.W;
			}
		}
		/// <summary>
		/// Returns a normalized version of this vector.
		/// </summary>
		public Vector4D Normalized
		{
			get
			{
				double length = this.Length;
				if (length < 1e-15f) return Vector4D.Zero;

				double scale = 1.0f / length;
				return new Vector4D(
					this.X * scale, 
					this.Y * scale, 
					this.Z * scale, 
					this.W * scale);
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
					case 2: return this.Z;
					case 3: return this.W;
					default: throw new IndexOutOfRangeException("Vector4D access at index: " + index);
				}
			}
			set
			{
				switch (index)
				{
					case 0: this.X = value; return;
					case 1: this.Y = value; return;
					case 2: this.Z = value; return;
					case 3: this.W = value; return;
					default: throw new IndexOutOfRangeException("Vector4D access at index: " + index);
				}
			}
		}

		/// <summary>
		/// Scales the Vector4D to unit length.
		/// </summary>
		public void Normalize()
		{
			double length = this.Length;
			if (length < 1e-15f)
			{
				this = Vector4D.Zero;
		}
			else
			{
				double scale = 1.0f / length;
				this.X *= scale;
				this.Y *= scale;
				this.Z *= scale;
				this.W *= scale;
			}
		}

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="value">The value that will initialize this instance.</param>
		public Vector4D(double value)
		{
			this.X = value;
			this.Y = value;
			this.Z = value;
			this.W = value;
		}
		/// <summary>
		/// Constructs a new Vector4D.
		/// </summary>
		/// <param name="x">The x component of the Vector4D.</param>
		/// <param name="y">The y component of the Vector4D.</param>
		/// <param name="z">The z component of the Vector4D.</param>
		/// <param name="w">The w component of the Vector4D.</param>
		public Vector4D(double x, double y, double z, double w)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}
		/// <summary>
		/// Constructs a new Vector4D from the given Vector2D.
		/// </summary>
		/// <param name="v">The Vector2D to copy components from.</param>
		public Vector4D(Vector2D v)
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = 0.0f;
			this.W = 0.0f;
		}
		/// <summary>
		/// Constructs a new Vector4D from the given Vector2D.
		/// </summary>
		/// <param name="v">The Vector2D to copy components from.</param>
		/// <param name="z"></param>
		public Vector4D(Vector2D v, double z)
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = z;
			this.W = 0.0f;
		}
		/// <summary>
		/// Constructs a new Vector4D from the given Vector2D.
		/// </summary>
		/// <param name="v">The Vector2D to copy components from.</param>
		/// <param name="z"></param>
		/// <param name="w"></param>
		public Vector4D(Vector2D v, double z, double w)
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = z;
			this.W = w;
		}
		/// <summary>
		/// Constructs a new Vector4D from the given Vector3D.
		/// The w component is initialized to 0.
		/// </summary>
		/// <param name="v">The Vector3D to copy components from.</param>
		/// <remarks><seealso cref="Vector4D(Vector3D, double)"/></remarks>
		public Vector4D(Vector3D v)
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
			this.W = 0.0f;
		}
		/// <summary>
		/// Constructs a new Vector4D from the specified Vector3D and w component.
		/// </summary>
		/// <param name="v">The Vector3D to copy components from.</param>
		/// <param name="w">The w component of the new Vector4D.</param>
		public Vector4D(Vector3D v, double w)
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
			this.W = w;
		}

		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="a">Left operand.</param>
		/// <param name="b">Right operand.</param>
		/// <param name="result">Result of operation.</param>
		public static void Add(ref Vector4D a, ref Vector4D b, out Vector4D result)
		{
			result = new Vector4D(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}
		/// <summary>
		/// Subtract one Vector from another
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">Result of subtraction</param>
		public static void Subtract(ref Vector4D a, ref Vector4D b, out Vector4D result)
		{
			result = new Vector4D(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
		}
		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector4D vector, double scale, out Vector4D result)
		{
			result = new Vector4D(vector.X * scale, vector.Y * scale, vector.Z * scale, vector.W * scale);
		}
		/// <summary>
		/// Multiplies a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Multiply(ref Vector4D vector, ref Vector4D scale, out Vector4D result)
		{
			result = new Vector4D(vector.X * scale.X, vector.Y * scale.Y, vector.Z * scale.Z, vector.W * scale.W);
		}
		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector4D vector, double scale, out Vector4D result)
		{
			Multiply(ref vector, 1 / scale, out result);
		}
		/// <summary>
		/// Divide a vector by the components of a vector (scale).
		/// </summary>
		/// <param name="vector">Left operand.</param>
		/// <param name="scale">Right operand.</param>
		/// <param name="result">Result of the operation.</param>
		public static void Divide(ref Vector4D vector, ref Vector4D scale, out Vector4D result)
		{
			result = new Vector4D(vector.X / scale.X, vector.Y / scale.Y, vector.Z / scale.Z, vector.W / scale.W);
		}
		
		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise minimum</returns>
		public static Vector4D Min(Vector4D a, Vector4D b)
		{
			a.X = a.X < b.X ? a.X : b.X;
			a.Y = a.Y < b.Y ? a.Y : b.Y;
			a.Z = a.Z < b.Z ? a.Z : b.Z;
			a.W = a.W < b.W ? a.W : b.W;
			return a;
		}
		/// <summary>
		/// Calculate the component-wise minimum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise minimum</param>
		public static void Min(ref Vector4D a, ref Vector4D b, out Vector4D result)
		{
			result.X = a.X < b.X ? a.X : b.X;
			result.Y = a.Y < b.Y ? a.Y : b.Y;
			result.Z = a.Z < b.Z ? a.Z : b.Z;
			result.W = a.W < b.W ? a.W : b.W;
		}

		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <returns>The component-wise maximum</returns>
		public static Vector4D Max(Vector4D a, Vector4D b)
		{
			a.X = a.X > b.X ? a.X : b.X;
			a.Y = a.Y > b.Y ? a.Y : b.Y;
			a.Z = a.Z > b.Z ? a.Z : b.Z;
			a.W = a.W > b.W ? a.W : b.W;
			return a;
		}
		/// <summary>
		/// Calculate the component-wise maximum of two vectors
		/// </summary>
		/// <param name="a">First operand</param>
		/// <param name="b">Second operand</param>
		/// <param name="result">The component-wise maximum</param>
		public static void Max(ref Vector4D a, ref Vector4D b, out Vector4D result)
		{
			result.X = a.X > b.X ? a.X : b.X;
			result.Y = a.Y > b.Y ? a.Y : b.Y;
			result.Z = a.Z > b.Z ? a.Z : b.Z;
			result.W = a.W > b.W ? a.W : b.W;
		}

		/// <summary>
		/// Calculate the dot product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The dot product of the two inputs</returns>
		public static double Dot(Vector4D left, Vector4D right)
		{
			return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
		}
		/// <summary>
		/// Calculate the dot product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <param name="result">The dot product of the two inputs</param>
		public static void Dot(ref Vector4D left, ref Vector4D right, out double result)
		{
			result = left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
		}

		/// <summary>
		/// Returns a new Vector that is the linear blend of the 2 given Vectors
		/// </summary>
		/// <param name="a">First input vector</param>
		/// <param name="b">Second input vector</param>
		/// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
		/// <returns>a when blend=0, b when blend=1, and a linear combination otherwise</returns>
		public static Vector4D Lerp(Vector4D a, Vector4D b, double blend)
		{
			a.X = blend * (b.X - a.X) + a.X;
			a.Y = blend * (b.Y - a.Y) + a.Y;
			a.Z = blend * (b.Z - a.Z) + a.Z;
			a.W = blend * (b.W - a.W) + a.W;
			return a;
		}
		/// <summary>
		/// Returns a new Vector that is the linear blend of the 2 given Vectors
		/// </summary>
		/// <param name="a">First input vector</param>
		/// <param name="b">Second input vector</param>
		/// <param name="blend">The blend factor. a when blend=0, b when blend=1.</param>
		/// <param name="result">a when blend=0, b when blend=1, and a linear combination otherwise</param>
		public static void Lerp(ref Vector4D a, ref Vector4D b, double blend, out Vector4D result)
		{
			result.X = blend * (b.X - a.X) + a.X;
			result.Y = blend * (b.Y - a.Y) + a.Y;
			result.Z = blend * (b.Z - a.Z) + a.Z;
			result.W = blend * (b.W - a.W) + a.W;
		}
		
		/// <summary>
		/// Transform a Vector by the given Matrix</summary>
		/// <param name="vec">The vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <returns>The transformed vector</returns>
		public static Vector4D Transform(Vector4D vec, Matrix4 mat)
		{
			Vector4D result;
			Transform(ref vec, ref mat, out result);
			return result;
		}
		/// <summary>
		/// Transform a Vector by the given Matrix</summary>
		/// <param name="vec">The vector to transform</param>
		/// <param name="mat">The desired transformation</param>
		/// <param name="result">The transformed vector</param>
		public static void Transform(ref Vector4D vec, ref Matrix4 mat, out Vector4D result)
		{
			result.X = vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X + vec.W * mat.Row3.X;
			result.Y = vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y + vec.W * mat.Row3.Y;
			result.Z = vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z + vec.W * mat.Row3.Z;
			result.W = vec.X * mat.Row0.W + vec.Y * mat.Row1.W + vec.Z * mat.Row2.W + vec.W * mat.Row3.W;
		}
		/// <summary>
		/// Transforms a vector by a quaternion rotation.
		/// </summary>
		/// <param name="vec">The vector to transform.</param>
		/// <param name="quat">The quaternion to rotate the vector by.</param>
		/// <returns>The result of the operation.</returns>
		public static Vector4D Transform(Vector4D vec, QuaternionD quat)
		{
			Vector4D result;
			Transform(ref vec, ref quat, out result);
			return result;
		}
		/// <summary>
		/// Transforms a vector by a quaternion rotation.
		/// </summary>
		/// <param name="vec">The vector to transform.</param>
		/// <param name="quat">The quaternion to rotate the vector by.</param>
		/// <param name="result">The result of the operation.</param>
		public static void Transform(ref Vector4D vec, ref QuaternionD quat, out Vector4D result)
		{
			QuaternionD v = new QuaternionD(vec.X, vec.Y, vec.Z, vec.W), i, t;
			QuaternionD.Invert(ref quat, out i);
			QuaternionD.Multiply(ref quat, ref v, out t);
			QuaternionD.Multiply(ref t, ref i, out v);

			result = new Vector4D(v.X, v.Y, v.Z, v.W);
		}

		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4D operator +(Vector4D left, Vector4D right)
		{
			return new Vector4D(
				left.X + right.X, 
				left.Y + right.Y, 
				left.Z + right.Z, 
				left.W + right.W);
		}
		/// <summary>
		/// Subtracts two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4D operator -(Vector4D left, Vector4D right)
		{
			return new Vector4D(
				left.X - right.X, 
				left.Y - right.Y, 
				left.Z - right.Z, 
				left.W - right.W);
		}
		/// <summary>
		/// Negates an instance.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4D operator -(Vector4D vec)
		{
			return new Vector4D(
				-vec.X, 
				-vec.Y, 
				-vec.Z, 
				-vec.W);
		}
		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4D operator *(Vector4D vec, double scale)
		{
			return new Vector4D(
				vec.X * scale, 
				vec.Y * scale, 
				vec.Z * scale,
				vec.W * scale);
		}
		/// <summary>
		/// Scales an instance by a vector.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4D operator *(Vector4D vec, Vector4D scale)
		{
			return new Vector4D(
				vec.X * scale.X, 
				vec.Y * scale.Y, 
				vec.Z * scale.Z, 
				vec.W * scale.W);
		}
		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="scale">The scalar.</param>
		/// <param name="vec">The instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4D operator *(double scale, Vector4D vec)
		{
			return vec * scale;
		}
		/// <summary>
		/// Divides an instance by a scalar.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4D operator /(Vector4D vec, double scale)
		{
			return vec * (1.0f / scale);
		}
		/// <summary>
		/// Divides an instance by a vector.
		/// </summary>
		/// <param name="vec">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector4D operator /(Vector4D vec, Vector4D scale)
		{
			return new Vector4D(
				vec.X / scale.X, 
				vec.Y / scale.Y, 
				vec.Z / scale.Z, 
				vec.W / scale.W);
		}
		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator ==(Vector4D left, Vector4D right)
		{
			return left.Equals(right);
		}
		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equa lright; false otherwise.</returns>
		public static bool operator !=(Vector4D left, Vector4D right)
		{
			return !left.Equals(right);
		}


		/// <summary>
		/// Returns a System.String that represents the current Vector4D.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("({0}, {1}, {2}, {3})", this.X, this.Y, this.Z, this.W);
		}
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode() ^ this.W.GetHashCode();
		}
		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Vector4D))
				return false;

			return this.Equals((Vector4D)obj);
		}

		/// <summary>
		/// Indicates whether the current vector is equal to another vector.</summary>
		/// <param name="other">A vector to compare with this vector.</param>
		/// <returns>true if the current vector is equal to the vector parameter; otherwise, false.</returns>
		public bool Equals(Vector4D other)
		{
			return
				this.X == other.X &&
				this.Y == other.Y &&
				this.Z == other.Z &&
				this.W == other.W;
		}

		/// <summary>
		/// Convert from Vector4
		/// </summary>
		/// <param name="v"></param>
		public static implicit operator Vector4D(Vector4 v)
		{
			return new Vector4D(v.X, v.Y, v.Z, v.W);
		}

		/// <summary>
		/// Convert to Vector4
		/// </summary>
		/// <param name="v"></param>
		public static implicit operator Vector4(Vector4D v)
		{
			return new Vector4((float)v.X, (float)v.Y, (float)v.Z, (float)v.W);
		}
	}
}
