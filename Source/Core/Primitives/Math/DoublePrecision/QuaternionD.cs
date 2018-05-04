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
	*/
#endregion

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Duality
{
	/// <summary>
	/// Represents a QuaternionD.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct QuaternionD : IEquatable<QuaternionD>
	{
		/// <summary>
		/// Defines the identity quaternion.
		/// </summary>
		public static readonly QuaternionD Identity = new QuaternionD(0, 0, 0, 1);


		private Vector3D xyz;
		private double w;


		/// <summary>
		/// Gets or sets an OpenTK.Vector3D with the X, Y and Z components of this instance.
		/// </summary>
		public Vector3D Xyz { get { return this.xyz; } set { this.xyz = value; } }
		/// <summary>
		/// Gets or sets the X component of this instance.
		/// </summary>
		public double X { get { return this.xyz.X; } set { this.xyz.X = value; } }
		/// <summary>
		/// Gets or sets the Y component of this instance.
		/// </summary>
		public double Y { get { return this.xyz.Y; } set { this.xyz.Y = value; } }
		/// <summary>
		/// Gets or sets the Z component of this instance.
		/// </summary>
		public double Z { get { return this.xyz.Z; } set { this.xyz.Z = value; } }
		/// <summary>
		/// Gets or sets the W component of this instance.
		/// </summary>
		public double W { get { return this.w; } set { this.w = value; } }
		/// <summary>
		/// Gets the length (magnitude) of the quaternion.
		/// </summary>
		/// <seealso cref="LengthSquared"/>
		public double Length
		{
			get
			{
				return (double)System.Math.Sqrt(this.W * this.W + this.Xyz.LengthSquared);
			}
		}
		/// <summary>
		/// Gets the square of the quaternion length (magnitude).
		/// </summary>
		public double LengthSquared
		{
			get
			{
				return this.W * this.W + this.Xyz.LengthSquared;
			}
		}
		

		/// <summary>
		/// Construct a new QuaternionD from vector and w components
		/// </summary>
		/// <param name="v">The vector part</param>
		/// <param name="w">The w part</param>
		public QuaternionD(Vector3D v, double w)
		{
			this.xyz = v;
			this.w = w;
		}
		/// <summary>
		/// Construct a new QuaternionD
		/// </summary>
		/// <param name="x">The x component</param>
		/// <param name="y">The y component</param>
		/// <param name="z">The z component</param>
		/// <param name="w">The w component</param>
		public QuaternionD(double x, double y, double z, double w)
			: this(new Vector3D(x, y, z), w)
		{ }

		/// <summary>
		/// Convert the current quaternion to axis angle representation
		/// </summary>
		/// <param name="axis">The resultant axis</param>
		/// <param name="angle">The resultant angle</param>
		public void ToAxisAngle(out Vector3D axis, out double angle)
		{
			Vector4D result = ToAxisAngle();
			axis = result.Xyz;
			angle = result.W;
		}
		/// <summary>
		/// Convert this instance to an axis-angle representation.
		/// </summary>
		/// <returns>A Vector4D that is the axis-angle representation of this quaternion.</returns>
		public Vector4D ToAxisAngle()
		{
			QuaternionD q = this;
			if (Math.Abs(q.W) > 1.0f)
				q.Normalize();

			Vector4D result = new Vector4D();

			result.W = 2.0f * (double)System.Math.Acos(q.W); // angle
			double den = (double)System.Math.Sqrt(1.0 - q.W * q.W);
			if (den > 0.0001f)
			{
				result.Xyz = q.Xyz / den;
			}
			else
			{
				// This occurs when the angle is zero. 
				// Not a problem: just set an arbitrary normalized axis.
				result.Xyz = Vector3D.UnitX;
			}

			return result;
		}
		
		/// <summary>
		/// Reverses the rotation angle of this Quaterniond.
		/// </summary>
		public void Invert()
		{
			this.W = -this.W;
		}
		/// <summary>
		/// Scales the QuaternionD to unit length.
		/// </summary>
		public void Normalize()
		{
			double scale = 1.0f / this.Length;
			this.Xyz *= scale;
			this.W *= scale;
		}
		/// <summary>
		/// Inverts the Vector3D component of this QuaternionD.
		/// </summary>
		public void Conjugate()
		{
			this.Xyz = -this.Xyz;
		}

		/// <summary>
		/// Returns a copy of the QuaternionD scaled to unit length.
		/// </summary>
		public QuaternionD Normalized()
		{
			QuaternionD q = this;
			q.Normalize();
			return q;
		}
		/// <summary>
		/// Returns a copy of this Quaterniond with its rotation angle reversed.
		/// </summary>
		public QuaternionD Inverted()
		{
			var q = this;
			q.Invert();
			return q;
		}

		/// <summary>
		/// Add two quaternions
		/// </summary>
		/// <param name="left">The first operand</param>
		/// <param name="right">The second operand</param>
		/// <returns>The result of the addition</returns>
		public static QuaternionD Add(QuaternionD left, QuaternionD right)
		{
			return new QuaternionD(
				left.Xyz + right.Xyz,
				left.W + right.W);
		}
		/// <summary>
		/// Add two quaternions
		/// </summary>
		/// <param name="left">The first operand</param>
		/// <param name="right">The second operand</param>
		/// <param name="result">The result of the addition</param>
		public static void Add(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
		{
			result = new QuaternionD(
				left.Xyz + right.Xyz,
				left.W + right.W);
		}

		/// <summary>
		/// Subtracts two instances.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>The result of the operation.</returns>
		public static QuaternionD Sub(QuaternionD left, QuaternionD right)
		{
			return  new QuaternionD(
				left.Xyz - right.Xyz,
				left.W - right.W);
		}
		/// <summary>
		/// Subtracts two instances.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <param name="result">The result of the operation.</param>
		public static void Sub(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
		{
			result = new QuaternionD(
				left.Xyz - right.Xyz,
				left.W - right.W);
		}

		/// <summary>
		/// Multiplies two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>A new instance containing the result of the calculation.</returns>
		public static QuaternionD Multiply(QuaternionD left, QuaternionD right)
		{
			QuaternionD result;
			Multiply(ref left, ref right, out result);
			return result;
		}
		/// <summary>
		/// Multiplies two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <param name="result">A new instance containing the result of the calculation.</param>
		public static void Multiply(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
		{
			result = new QuaternionD(
				right.W * left.Xyz + left.W * right.Xyz + Vector3D.Cross(left.Xyz, right.Xyz),
				left.W * right.W - Vector3D.Dot(left.Xyz, right.Xyz));
		}
		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="quaternion">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <param name="result">A new instance containing the result of the calculation.</param>
		public static void Multiply(ref QuaternionD quaternion, double scale, out QuaternionD result)
		{
			result = new QuaternionD(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
		}
		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="quaternion">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>A new instance containing the result of the calculation.</returns>
		public static QuaternionD Multiply(QuaternionD quaternion, double scale)
		{
			return new QuaternionD(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
		}

		/// <summary>
		/// Get the conjugate of the given quaternion
		/// </summary>
		/// <param name="q">The quaternion</param>
		/// <returns>The conjugate of the given quaternion</returns>
		public static QuaternionD Conjugate(QuaternionD q)
		{
			return new QuaternionD(-q.Xyz, q.W);
		}
		/// <summary>
		/// Get the conjugate of the given quaternion
		/// </summary>
		/// <param name="q">The quaternion</param>
		/// <param name="result">The conjugate of the given quaternion</param>
		public static void Conjugate(ref QuaternionD q, out QuaternionD result)
		{
			result = new QuaternionD(-q.Xyz, q.W);
		}

		/// <summary>
		/// Get the inverse of the given quaternion
		/// </summary>
		/// <param name="q">The quaternion to invert</param>
		/// <returns>The inverse of the given quaternion</returns>
		public static QuaternionD Invert(QuaternionD q)
		{
			QuaternionD result;
			Invert(ref q, out result);
			return result;
		}
		/// <summary>
		/// Get the inverse of the given quaternion
		/// </summary>
		/// <param name="q">The quaternion to invert</param>
		/// <param name="result">The inverse of the given quaternion</param>
		public static void Invert(ref QuaternionD q, out QuaternionD result)
		{
			double lengthSq = q.LengthSquared;
			if (lengthSq != 0.0)
			{
				double i = 1.0f / lengthSq;
				result = new QuaternionD(q.Xyz * -i, q.W * i);
			}
			else
			{
				result = q;
			}
		}

		/// <summary>
		/// Scale the given quaternion to unit length
		/// </summary>
		/// <param name="q">The quaternion to normalize</param>
		/// <returns>The normalized quaternion</returns>
		public static QuaternionD Normalize(QuaternionD q)
		{
			QuaternionD result;
			Normalize(ref q, out result);
			return result;
		}
		/// <summary>
		/// Scale the given quaternion to unit length
		/// </summary>
		/// <param name="q">The quaternion to normalize</param>
		/// <param name="result">The normalized quaternion</param>
		public static void Normalize(ref QuaternionD q, out QuaternionD result)
		{
			double scale = 1.0f / q.Length;
			result = new QuaternionD(q.Xyz * scale, q.W * scale);
		}

		/// <summary>
		/// Build a quaternion from the given axis and angle
		/// </summary>
		/// <param name="axis">The axis to rotate about</param>
		/// <param name="angle">The rotation angle in radians</param>
		/// <returns>The equivalent quaternion</returns>
		public static QuaternionD FromAxisAngle(Vector3D axis, double angle)
		{
			if (axis.LengthSquared == 0.0f)
				return Identity;

			QuaternionD result = Identity;

			angle *= 0.5f;
			axis.Normalize();
			result.Xyz = axis * (double)System.Math.Sin(angle);
			result.W = (double)System.Math.Cos(angle);

			return Normalize(result);
		}

		/// <summary>
		/// Builds a quaternion from the given rotation matrix
		/// </summary>
		/// <param name="matrix">A rotation matrix</param>
		/// <returns>The equivalent quaternion</returns>
		public static QuaternionD FromMatrix(Matrix3 matrix)
		{
			QuaternionD result;
			FromMatrix(ref matrix, out result);
			return result;
		}
		/// <summary>
		/// Builds a quaternion from the given rotation matrix
		/// </summary>
		/// <param name="matrix">A rotation matrix</param>
		/// <param name="result">The equivalent quaternion</param>
		public static void FromMatrix(ref Matrix3 matrix, out QuaternionD result)
		{
			double trace = matrix.Trace;

			if (trace > 0)
			{
				double s = (double)Math.Sqrt(trace + 1) * 2;
				double invS = 1f / s;

				result.w = s * 0.25f;
				result.xyz.X = (matrix.Row2.Y - matrix.Row1.Z) * invS;
				result.xyz.Y = (matrix.Row0.Z - matrix.Row2.X) * invS;
				result.xyz.Z = (matrix.Row1.X - matrix.Row0.Y) * invS;
			}
			else
			{
				double m00 = matrix.Row0.X, m11 = matrix.Row1.Y, m22 = matrix.Row2.Z;

				if (m00 > m11 && m00 > m22)
				{
					double s = (double)Math.Sqrt(1 + m00 - m11 - m22) * 2;
					double invS = 1f / s;

					result.w = (matrix.Row2.Y - matrix.Row1.Z) * invS;
					result.xyz.X = s * 0.25f;
					result.xyz.Y = (matrix.Row0.Y + matrix.Row1.X) * invS;
					result.xyz.Z = (matrix.Row0.Z + matrix.Row2.X) * invS;
				}
				else if (m11 > m22)
				{
					double s = (double)Math.Sqrt(1 + m11 - m00 - m22) * 2;
					double invS = 1f / s;

					result.w = (matrix.Row0.Z - matrix.Row2.X) * invS;
					result.xyz.X = (matrix.Row0.Y + matrix.Row1.X) * invS;
					result.xyz.Y = s * 0.25f;
					result.xyz.Z = (matrix.Row1.Z + matrix.Row2.Y) * invS;
				}
				else
				{
					double s = (double)Math.Sqrt(1 + m22 - m00 - m11) * 2;
					double invS = 1f / s;

					result.w = (matrix.Row1.X - matrix.Row0.Y) * invS;
					result.xyz.X = (matrix.Row0.Z + matrix.Row2.X) * invS;
					result.xyz.Y = (matrix.Row1.Z + matrix.Row2.Y) * invS;
					result.xyz.Z = s * 0.25f;
				}
			}
		}

		/// <summary>
		/// Do Spherical linear interpolation between two quaternions 
		/// </summary>
		/// <param name="q1">The first quaternion</param>
		/// <param name="q2">The second quaternion</param>
		/// <param name="blend">The blend factor</param>
		/// <returns>A smooth blend between the given quaternions</returns>
		public static QuaternionD Slerp(QuaternionD q1, QuaternionD q2, double blend)
		{
			// if either input is zero, return the other.
			if (q1.LengthSquared == 0.0f)
			{
				if (q2.LengthSquared == 0.0f)
				{
					return Identity;
				}
				return q2;
			}
			else if (q2.LengthSquared == 0.0f)
			{
				return q1;
			}


			double cosHalfAngle = q1.W * q2.W + Vector3D.Dot(q1.Xyz, q2.Xyz);

			if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
			{
				// angle = 0.0f, so just return one input.
				return q1;
			}
			else if (cosHalfAngle < 0.0f)
			{
				q2.Xyz = -q2.Xyz;
				q2.W = -q2.W;
				cosHalfAngle = -cosHalfAngle;
			}

			double blendA;
			double blendB;
			if (cosHalfAngle < 0.99f)
			{
				// do proper slerp for big angles
				double halfAngle = (double)System.Math.Acos(cosHalfAngle);
				double sinHalfAngle = (double)System.Math.Sin(halfAngle);
				double oneOverSinHalfAngle = 1.0f / sinHalfAngle;
				blendA = (double)System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
				blendB = (double)System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
			}
			else
			{
				// do lerp if angle is really small.
				blendA = 1.0f - blend;
				blendB = blend;
			}

			QuaternionD result = new QuaternionD(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
			if (result.LengthSquared > 0.0f)
				return Normalize(result);
			else
				return Identity;
		}

		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static QuaternionD operator +(QuaternionD left, QuaternionD right)
		{
			left.Xyz += right.Xyz;
			left.W += right.W;
			return left;
		}
		/// <summary>
		/// Subtracts two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static QuaternionD operator -(QuaternionD left, QuaternionD right)
		{
			left.Xyz -= right.Xyz;
			left.W -= right.W;
			return left;
		}
		/// <summary>
		/// Multiplies two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static QuaternionD operator *(QuaternionD left, QuaternionD right)
		{
			Multiply(ref left, ref right, out left);
			return left;
		}
		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="quaternion">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>A new instance containing the result of the calculation.</returns>
		public static QuaternionD operator *(QuaternionD quaternion, double scale)
		{
			Multiply(ref quaternion, scale, out quaternion);
			return quaternion;
		}
		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="quaternion">The instance.</param>
		/// <param name="scale">The scalar.</param>
		/// <returns>A new instance containing the result of the calculation.</returns>
		public static QuaternionD operator *(double scale, QuaternionD quaternion)
		{
			return new QuaternionD(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
		}

		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator ==(QuaternionD left, QuaternionD right)
		{
			return left.Equals(right);
		}
		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equal right; false otherwise.</returns>
		public static bool operator !=(QuaternionD left, QuaternionD right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Returns a System.String that represents the current QuaternionD.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("V: {0}, W: {1}", this.Xyz, this.W);
		}
		/// <summary>
		/// Compares this object instance to another object for equality. 
		/// </summary>
		/// <param name="other">The other object to be used in the comparison.</param>
		/// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
		public override bool Equals(object other)
		{
			if (other is QuaternionD == false) return false;
				return this == (QuaternionD)other;
		}
		/// <summary>
		/// Provides the hash code for this object. 
		/// </summary>
		/// <returns>A hash code formed from the bitwise XOR of this objects members.</returns>
		public override int GetHashCode()
		{
			return this.Xyz.GetHashCode() ^ this.W.GetHashCode();
		}
		/// <summary>
		/// Compares this QuaternionD instance to another QuaternionD for equality. 
		/// </summary>
		/// <param name="other">The other QuaternionD to be used in the comparison.</param>
		/// <returns>True if both instances are equal; false otherwise.</returns>
		public bool Equals(QuaternionD other)
		{
			return this.Xyz == other.Xyz && this.W == other.W;
		}

		/// <summary>
		/// Convert from Quaternion
		/// </summary>
		/// <param name="v"></param>
		public static implicit operator QuaternionD(Quaternion q)
		{
			return new QuaternionD(q.X, q.Y, q.Z, q.W);
		}

		/// <summary>
		/// Convert to Quaternion
		/// </summary>
		/// <param name="v"></param>
		public static implicit operator Quaternion(QuaternionD q)
		{
			return new Quaternion((float)q.X, (float)q.Y, (float)q.Z, (float)q.W);
		}
	}
}
