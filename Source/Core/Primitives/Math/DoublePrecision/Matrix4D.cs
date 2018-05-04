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

namespace Duality
{
	/// <summary>
	/// Represents a 4x4 matrix containing 3D rotation, scale, transform, and projection.
	/// </summary>
	/// <seealso cref="Matrix4d"/>
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix4D : IEquatable<Matrix4D>
	{
		/// <summary>
		/// The identity matrix.
		/// </summary>
		public static readonly Matrix4D Identity = new Matrix4D(Vector4D.UnitX, Vector4D.UnitY, Vector4D.UnitZ, Vector4D.UnitW);
		/// <summary>
		/// The zero matrix.
		/// </summary>
		public static readonly Matrix4D Zero = new Matrix4D(Vector4D.Zero, Vector4D.Zero, Vector4D.Zero, Vector4D.Zero);

		/// <summary>
		/// Top row of the matrix.
		/// </summary>
		public Vector4D Row0;
		/// <summary>
		/// 2nd row of the matrix.
		/// </summary>
		public Vector4D Row1;
		/// <summary>
		/// 3rd row of the matrix.
		/// </summary>
		public Vector4D Row2;
		/// <summary>
		/// Bottom row of the matrix.
		/// </summary>
		public Vector4D Row3;
		

		/// <summary>
		/// Gets the first column of this matrix.
		/// </summary>
		public Vector4D Column0
		{
			get { return new Vector4D(this.Row0.X, this.Row1.X, this.Row2.X, this.Row3.X); }
			set { this.Row0.X = value.X; this.Row1.X = value.Y; this.Row2.X = value.Z; this.Row3.X = value.W; }
		}
		/// <summary>
		/// Gets the second column of this matrix.
		/// </summary>
		public Vector4D Column1
		{
			get { return new Vector4D(this.Row0.Y, this.Row1.Y, this.Row2.Y, this.Row3.Y); }
			set { this.Row0.Y = value.X; this.Row1.Y = value.Y; this.Row2.Y = value.Z; this.Row3.Y = value.W; }
		}
		/// <summary>
		/// Gets the third column of this matrix.
		/// </summary>
		public Vector4D Column2
		{
			get { return new Vector4D(this.Row0.Z, this.Row1.Z, this.Row2.Z, this.Row3.Z); }
			set { this.Row0.Z = value.X; this.Row1.Z = value.Y; this.Row2.Z = value.Z; this.Row3.Z = value.W; }
		}
		/// <summary>
		/// Gets the fourth column of this matrix.
		/// </summary>
		public Vector4D Column3
		{
			get { return new Vector4D(this.Row0.W, this.Row1.W, this.Row2.W, this.Row3.W); }
			set { this.Row0.W = value.X; this.Row1.W = value.Y; this.Row2.W = value.Z; this.Row3.W = value.W; }
		}
		/// <summary>
		/// Gets or sets the value at row 1, column 1 of this instance.
		/// </summary>
		public double M11 { get { return this.Row0.X; } set { this.Row0.X = value; } }
		/// <summary>
		/// Gets or sets the value at row 1, column 2 of this instance.
		/// </summary>
		public double M12 { get { return this.Row0.Y; } set { this.Row0.Y = value; } }
		/// <summary>
		/// Gets or sets the value at row 1, column 3 of this instance.
		/// </summary>
		public double M13 { get { return this.Row0.Z; } set { this.Row0.Z = value; } }
		/// <summary>
		/// Gets or sets the value at row 1, column 4 of this instance.
		/// </summary>
		public double M14 { get { return this.Row0.W; } set { this.Row0.W = value; } }
		/// <summary>
		/// Gets or sets the value at row 2, column 1 of this instance.
		/// </summary>
		public double M21 { get { return this.Row1.X; } set { this.Row1.X = value; } }
		/// <summary>
		/// Gets or sets the value at row 2, column 2 of this instance.
		/// </summary>
		public double M22 { get { return this.Row1.Y; } set { this.Row1.Y = value; } }
		/// <summary>
		/// Gets or sets the value at row 2, column 3 of this instance.
		/// </summary>
		public double M23 { get { return this.Row1.Z; } set { this.Row1.Z = value; } }
		/// <summary>
		/// Gets or sets the value at row 2, column 4 of this instance.
		/// </summary>
		public double M24 { get { return this.Row1.W; } set { this.Row1.W = value; } }
		/// <summary>
		/// Gets or sets the value at row 3, column 1 of this instance.
		/// </summary>
		public double M31 { get { return this.Row2.X; } set { this.Row2.X = value; } }
		/// <summary>
		/// Gets or sets the value at row 3, column 2 of this instance.
		/// </summary>
		public double M32 { get { return this.Row2.Y; } set { this.Row2.Y = value; } }
		/// <summary>
		/// Gets or sets the value at row 3, column 3 of this instance.
		/// </summary>
		public double M33 { get { return this.Row2.Z; } set { this.Row2.Z = value; } }
		/// <summary>
		/// Gets or sets the value at row 3, column 4 of this instance.
		/// </summary>
		public double M34 { get { return this.Row2.W; } set { this.Row2.W = value; } }
		/// <summary>
		/// Gets or sets the value at row 4, column 1 of this instance.
		/// </summary>
		public double M41 { get { return this.Row3.X; } set { this.Row3.X = value; } }
		/// <summary>
		/// Gets or sets the value at row 4, column 2 of this instance.
		/// </summary>
		public double M42 { get { return this.Row3.Y; } set { this.Row3.Y = value; } }
		/// <summary>
		/// Gets or sets the value at row 4, column 3 of this instance.
		/// </summary>
		public double M43 { get { return this.Row3.Z; } set { this.Row3.Z = value; } }
		/// <summary>
		/// Gets or sets the value at row 4, column 4 of this instance.
		/// </summary>
		public double M44 { get { return this.Row3.W; } set { this.Row3.W = value; } }
		/// <summary>
		/// Gets or sets the value at a specified row and column.
		/// </summary>
		public double this[int rowIndex, int columnIndex]
		{
			get
			{
				if (rowIndex == 0) return this.Row0[columnIndex];
				else if (rowIndex == 1) return this.Row1[columnIndex];
				else if (rowIndex == 2) return this.Row2[columnIndex];
				else if (rowIndex == 3) return this.Row3[columnIndex];
				throw new IndexOutOfRangeException("You tried to access this matrix at: (" + rowIndex + ", " + columnIndex + ")");
			}
			set
			{
				if (rowIndex == 0) this.Row0[columnIndex] = value;
				else if (rowIndex == 1) this.Row1[columnIndex] = value;
				else if (rowIndex == 2) this.Row2[columnIndex] = value;
				else if (rowIndex == 3) this.Row3[columnIndex] = value;
				else throw new IndexOutOfRangeException("You tried to set this matrix at: (" + rowIndex + ", " + columnIndex + ")");
			}
		}
		/// <summary>
		/// Gets the determinant of this matrix.
		/// </summary>
		public double Determinant
		{
			get
			{
				double m11 = this.Row0.X, m12 = this.Row0.Y, m13 = this.Row0.Z, m14 = this.Row0.W,
						m21 = this.Row1.X, m22 = this.Row1.Y, m23 = this.Row1.Z, m24 = this.Row1.W,
						m31 = this.Row2.X, m32 = this.Row2.Y, m33 = this.Row2.Z, m34 = this.Row2.W,
						m41 = this.Row3.X, m42 = this.Row3.Y, m43 = this.Row3.Z, m44 = this.Row3.W;
				return
					m11 * m22 * m33 * m44 - m11 * m22 * m34 * m43 + m11 * m23 * m34 * m42 - m11 * m23 * m32 * m44
					+ m11 * m24 * m32 * m43 - m11 * m24 * m33 * m42 - m12 * m23 * m34 * m41 + m12 * m23 * m31 * m44
					- m12 * m24 * m31 * m43 + m12 * m24 * m33 * m41 - m12 * m21 * m33 * m44 + m12 * m21 * m34 * m43
					+ m13 * m24 * m31 * m42 - m13 * m24 * m32 * m41 + m13 * m21 * m32 * m44 - m13 * m21 * m34 * m42
					+ m13 * m22 * m34 * m41 - m13 * m22 * m31 * m44 - m14 * m21 * m32 * m43 + m14 * m21 * m33 * m42
					- m14 * m22 * m33 * m41 + m14 * m22 * m31 * m43 - m14 * m23 * m31 * m42 + m14 * m23 * m32 * m41;
			}
		}
		/// <summary>
		/// Gets or sets the values along the main diagonal of the matrix.
		/// </summary>
		public Vector4D Diagonal
		{
			get
			{
				return new Vector4D(this.Row0.X, this.Row1.Y, this.Row2.Z, this.Row3.W);
			}
			set
			{
				this.Row0.X = value.X;
				this.Row1.Y = value.Y;
				this.Row2.Z = value.Z;
				this.Row3.W = value.W;
			}
		}
		/// <summary>
		/// Gets the trace of the matrix, the sum of the values along the diagonal.
		/// </summary>
		public double Trace { get { return this.Row0.X + this.Row1.Y + this.Row2.Z + this.Row3.W; } }

		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="row0">Top row of the matrix.</param>
		/// <param name="row1">Second row of the matrix.</param>
		/// <param name="row2">Third row of the matrix.</param>
		/// <param name="row3">Bottom row of the matrix.</param>
		public Matrix4D(Vector4D row0, Vector4D row1, Vector4D row2, Vector4D row3)
		{
			this.Row0 = row0;
			this.Row1 = row1;
			this.Row2 = row2;
			this.Row3 = row3;
		}
		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="m00">First item of the first row of the matrix.</param>
		/// <param name="m01">Second item of the first row of the matrix.</param>
		/// <param name="m02">Third item of the first row of the matrix.</param>
		/// <param name="m03">Fourth item of the first row of the matrix.</param>
		/// <param name="m10">First item of the second row of the matrix.</param>
		/// <param name="m11">Second item of the second row of the matrix.</param>
		/// <param name="m12">Third item of the second row of the matrix.</param>
		/// <param name="m13">Fourth item of the second row of the matrix.</param>
		/// <param name="m20">First item of the third row of the matrix.</param>
		/// <param name="m21">Second item of the third row of the matrix.</param>
		/// <param name="m22">Third item of the third row of the matrix.</param>
		/// <param name="m23">First item of the third row of the matrix.</param>
		/// <param name="m30">Fourth item of the fourth row of the matrix.</param>
		/// <param name="m31">Second item of the fourth row of the matrix.</param>
		/// <param name="m32">Third item of the fourth row of the matrix.</param>
		/// <param name="m33">Fourth item of the fourth row of the matrix.</param>
		public Matrix4D(
			double m00, double m01, double m02, double m03,
			double m10, double m11, double m12, double m13,
			double m20, double m21, double m22, double m23,
			double m30, double m31, double m32, double m33)
		{
			this.Row0 = new Vector4D(m00, m01, m02, m03);
			this.Row1 = new Vector4D(m10, m11, m12, m13);
			this.Row2 = new Vector4D(m20, m21, m22, m23);
			this.Row3 = new Vector4D(m30, m31, m32, m33);
		}


		/// <summary>
		/// Converts this instance into its inverse.
		/// </summary>
		public void Invert()
		{
			this = Matrix4D.Invert(this);
		}
		/// <summary>
		/// Converts this instance into its transpose.
		/// </summary>
		public void Transpose()
		{
			this = Matrix4D.Transpose(this);
		}
		/// <summary>
		/// Divides each element in the Matrix by the <see cref="Determinant"/>.
		/// </summary>
		public void Normalize()
		{
			double determinant = this.Determinant;
			this.Row0 /= determinant;
			this.Row1 /= determinant;
			this.Row2 /= determinant;
			this.Row3 /= determinant;
		}

		/// <summary>
		/// Returns a normalised copy of this instance.
		/// </summary>
		public Matrix4D Normalized()
		{
			Matrix4D m = this;
			m.Normalize();
			return m;
		}
		/// <summary>
		/// Returns an inverted copy of this instance.
		/// </summary>
		public Matrix4D Inverted()
		{
			Matrix4D m = this;
			if (m.Determinant != 0)
				m.Invert();
			return m;
		}

		/// <summary>
		/// Returns a copy of this Matrix4D without translation.
		/// </summary>
		public Matrix4D ClearTranslation()
		{
			Matrix4D m = this;
			m.Row3.Xyz = Vector3D.Zero;
			return m;
		}
		/// <summary>
		/// Returns a copy of this Matrix4D without scale.
		/// </summary>
		public Matrix4D ClearScale()
		{
			Matrix4D m = this;
			m.Row0.Xyz = m.Row0.Xyz.Normalized;
			m.Row1.Xyz = m.Row1.Xyz.Normalized;
			m.Row2.Xyz = m.Row2.Xyz.Normalized;
			return m;
		}
		/// <summary>
		/// Returns a copy of this Matrix4D without rotation.
		/// </summary>
		public Matrix4D ClearRotation()
		{
			Matrix4D m = this;
			m.Row0.Xyz = new Vector3D(m.Row0.Xyz.Length, 0, 0);
			m.Row1.Xyz = new Vector3D(0, m.Row1.Xyz.Length, 0);
			m.Row2.Xyz = new Vector3D(0, 0, m.Row2.Xyz.Length);
			return m;
		}
		/// <summary>
		/// Returns a copy of this Matrix4D without projection.
		/// </summary>
		public Matrix4D ClearProjection()
		{
			Matrix4D m = this;
			m.Column3 = Vector4D.Zero;
			return m;
		}

		/// <summary>
		/// Returns the translation component of this instance.
		/// </summary>
		public Vector3D ExtractTranslation() { return this.Row3.Xyz; }
		/// <summary>
		/// Returns the scale component of this instance.
		/// </summary>
		public Vector3D ExtractScale() { return new Vector3D(this.Row0.Xyz.Length, this.Row1.Xyz.Length, this.Row2.Xyz.Length); }
		/// <summary>
		/// Returns the rotation component of this instance. Quite slow.
		/// </summary>
		/// <param name="row_normalise">Whether the method should row-normalise (i.e. remove scale from) the Matrix. Pass false if you know it's already normalised.</param>
		public QuaternionD ExtractRotation(bool row_normalise = true)
		{
			var row0 = this.Row0.Xyz;
			var row1 = this.Row1.Xyz;
			var row2 = this.Row2.Xyz;

			if (row_normalise)
			{
				row0 = row0.Normalized;
				row1 = row1.Normalized;
				row2 = row2.Normalized;
			}

			// code below adapted from Blender

			QuaternionD q = new QuaternionD();
			double trace = 0.25 * (row0[0] + row1[1] + row2[2] + 1.0);

			if (trace > 0)
			{
				double sq = Math.Sqrt(trace);

				q.W = (double)sq;
				sq = 1.0 / (4.0 * sq);
				q.X = (double)((row1[2] - row2[1]) * sq);
				q.Y = (double)((row2[0] - row0[2]) * sq);
				q.Z = (double)((row0[1] - row1[0]) * sq);
			}
			else if (row0[0] > row1[1] && row0[0] > row2[2])
			{
				double sq = 2.0 * Math.Sqrt(1.0 + row0[0] - row1[1] - row2[2]);

				q.X = (double)(0.25 * sq);
				sq = 1.0 / sq;
				q.W = (double)((row2[1] - row1[2]) * sq);
				q.Y = (double)((row1[0] + row0[1]) * sq);
				q.Z = (double)((row2[0] + row0[2]) * sq);
			}
			else if (row1[1] > row2[2])
			{
				double sq = 2.0 * Math.Sqrt(1.0 + row1[1] - row0[0] - row2[2]);

				q.Y = (double)(0.25 * sq);
				sq = 1.0 / sq;
				q.W = (double)((row2[0] - row0[2]) * sq);
				q.X = (double)((row1[0] + row0[1]) * sq);
				q.Z = (double)((row2[1] + row1[2]) * sq);
			}
			else
			{
				double sq = 2.0 * Math.Sqrt(1.0 + row2[2] - row0[0] - row1[1]);

				q.Z = (double)(0.25 * sq);
				sq = 1.0 / sq;
				q.W = (double)((row1[0] - row0[1]) * sq);
				q.X = (double)((row2[0] + row0[2]) * sq);
				q.Y = (double)((row2[1] + row1[2]) * sq);
			}

			q.Normalize();
			return q;
		}
		/// <summary>
		/// Returns the projection component of this instance.
		/// </summary>
		public Vector4D ExtractProjection()
		{
			return this.Column3;
		}
        
		/// <summary>
		/// Build a rotation matrix from the specified axis/angle rotation.
		/// </summary>
		/// <param name="axis">The axis to rotate about.</param>
		/// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
		/// <param name="result">A matrix instance.</param>
		public static void CreateFromAxisAngle(Vector3D axis, double angle, out Matrix4D result)
		{
			// normalize and create a local copy of the vector.
			axis.Normalize();
			double axisX = axis.X, axisY = axis.Y, axisZ = axis.Z;

			// calculate angles
			double cos = (double)System.Math.Cos(-angle);
			double sin = (double)System.Math.Sin(-angle);
			double t = 1.0f - cos;

			// do the conversion math once
			double tXX = t * axisX * axisX,
				tXY = t * axisX * axisY,
				tXZ = t * axisX * axisZ,
				tYY = t * axisY * axisY,
				tYZ = t * axisY * axisZ,
				tZZ = t * axisZ * axisZ;

			double sinX = sin * axisX,
				sinY = sin * axisY,
				sinZ = sin * axisZ;

			result.Row0.X = tXX + cos;
			result.Row0.Y = tXY - sinZ;
			result.Row0.Z = tXZ + sinY;
			result.Row0.W = 0;
			result.Row1.X = tXY + sinZ;
			result.Row1.Y = tYY + cos;
			result.Row1.Z = tYZ - sinX;
			result.Row1.W = 0;
			result.Row2.X = tXZ - sinY;
			result.Row2.Y = tYZ + sinX;
			result.Row2.Z = tZZ + cos;
			result.Row2.W = 0;
			result.Row3 = Vector4D.UnitW;
		}
		/// <summary>
		/// Build a rotation matrix from the specified axis/angle rotation.
		/// </summary>
		/// <param name="axis">The axis to rotate about.</param>
		/// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
		/// <returns>A matrix instance.</returns>
		public static Matrix4D CreateFromAxisAngle(Vector3D axis, double angle)
		{
			Matrix4D result;
			CreateFromAxisAngle(axis, angle, out result);
			return result;
		}

		/// <summary>
		/// Builds a rotation matrix from a quaternion.
		/// </summary>
		/// <param name="q">The quaternion to rotate by.</param>
		/// <param name="result">A matrix instance.</param>
		public static void CreateFromQuaternion(ref QuaternionD q, out Matrix4D result)
		{
			Vector3D axis;
			double angle;
			q.ToAxisAngle(out axis, out angle);
			CreateFromAxisAngle(axis, angle, out result);
		}
		/// <summary>
		/// Builds a rotation matrix from a quaternion.
		/// </summary>
		/// <param name="q">The quaternion to rotate by.</param>
		/// <returns>A matrix instance.</returns>
		public static Matrix4D CreateFromQuaternion(QuaternionD q)
		{
			Matrix4D result;
			CreateFromQuaternion(ref q, out result);
			return result;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the x-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <param name="result">The resulting Matrix4D instance.</param>
		public static void CreateRotationX(double angle, out Matrix4D result)
		{
			double cos = (double)System.Math.Cos(angle);
			double sin = (double)System.Math.Sin(angle);

			result = Identity;
			result.Row1.Y = cos;
			result.Row1.Z = sin;
			result.Row2.Y = -sin;
			result.Row2.Z = cos;
		}
		/// <summary>
		/// Builds a rotation matrix for a rotation around the x-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting Matrix4D instance.</returns>
		public static Matrix4D CreateRotationX(double angle)
		{
			Matrix4D result;
			CreateRotationX(angle, out result);
			return result;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the y-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <param name="result">The resulting Matrix4D instance.</param>
		public static void CreateRotationY(double angle, out Matrix4D result)
		{
			double cos = (double)System.Math.Cos(angle);
			double sin = (double)System.Math.Sin(angle);

			result = Identity;
			result.Row0.X = cos;
			result.Row0.Z = -sin;
			result.Row2.X = sin;
			result.Row2.Z = cos;
		}
		/// <summary>
		/// Builds a rotation matrix for a rotation around the y-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting Matrix4D instance.</returns>
		public static Matrix4D CreateRotationY(double angle)
		{
			Matrix4D result;
			CreateRotationY(angle, out result);
			return result;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the z-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <param name="result">The resulting Matrix4D instance.</param>
		public static void CreateRotationZ(double angle, out Matrix4D result)
		{
			double cos = (double)System.Math.Cos(angle);
			double sin = (double)System.Math.Sin(angle);

			result = Identity;
			result.Row0.X = cos;
			result.Row0.Y = sin;
			result.Row1.X = -sin;
			result.Row1.Y = cos;
		}
		/// <summary>
		/// Builds a rotation matrix for a rotation around the z-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <returns>The resulting Matrix4D instance.</returns>
		public static Matrix4D CreateRotationZ(double angle)
		{
			Matrix4D result;
			CreateRotationZ(angle, out result);
			return result;
		}

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="x">X translation.</param>
		/// <param name="y">Y translation.</param>
		/// <param name="z">Z translation.</param>
		/// <param name="result">The resulting Matrix4D instance.</param>
		public static void CreateTranslation(double x, double y, double z, out Matrix4D result)
		{
			result = Identity;
			result.Row3.X = x;
			result.Row3.Y = y;
			result.Row3.Z = z;
		}
		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="vector">The translation vector.</param>
		/// <param name="result">The resulting Matrix4D instance.</param>
		public static void CreateTranslation(ref Vector3D vector, out Matrix4D result)
		{
			result = Identity;
			result.Row3.X = vector.X;
			result.Row3.Y = vector.Y;
			result.Row3.Z = vector.Z;
		}

		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="x">X translation.</param>
		/// <param name="y">Y translation.</param>
		/// <param name="z">Z translation.</param>
		/// <returns>The resulting Matrix4D instance.</returns>
		public static Matrix4D CreateTranslation(double x, double y, double z)
		{
			Matrix4D result;
			CreateTranslation(x, y, z, out result);
			return result;
		}
		/// <summary>
		/// Creates a translation matrix.
		/// </summary>
		/// <param name="vector">The translation vector.</param>
		/// <returns>The resulting Matrix4D instance.</returns>
		public static Matrix4D CreateTranslation(Vector3D vector)
		{
			Matrix4D result;
			CreateTranslation(vector.X, vector.Y, vector.Z, out result);
			return result;
		}

		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Single scale factor for the x, y, and z axes.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix4D CreateScale(double scale)
		{
			Matrix4D result;
			CreateScale(scale, out result);
			return result;
		}
		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Scale factors for the x, y, and z axes.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix4D CreateScale(Vector3D scale)
		{
			Matrix4D result;
			CreateScale(ref scale, out result);
			return result;
		}

		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="x">Scale factor for the x axis.</param>
		/// <param name="y">Scale factor for the y axis.</param>
		/// <param name="z">Scale factor for the z axis.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix4D CreateScale(double x, double y, double z)
		{
			Matrix4D result;
			CreateScale(x, y, z, out result);
			return result;
		}
		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Single scale factor for the x, y, and z axes.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(double scale, out Matrix4D result)
		{
			result = Identity;
			result.Row0.X = scale;
			result.Row1.Y = scale;
			result.Row2.Z = scale;
		}
		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Scale factors for the x, y, and z axes.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(ref Vector3D scale, out Matrix4D result)
		{
			result = Identity;
			result.Row0.X = scale.X;
			result.Row1.Y = scale.Y;
			result.Row2.Z = scale.Z;
		}
		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="x">Scale factor for the x axis.</param>
		/// <param name="y">Scale factor for the y axis.</param>
		/// <param name="z">Scale factor for the z axis.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(double x, double y, double z, out Matrix4D result)
		{
			result = Identity;
			result.Row0.X = x;
			result.Row1.Y = y;
			result.Row2.Z = z;
		}

		/// <summary>
		/// Creates an orthographic projection matrix.
		/// </summary>
		/// <param name="width">The width of the projection volume.</param>
		/// <param name="height">The height of the projection volume.</param>
		/// <param name="zNear">The near edge of the projection volume.</param>
		/// <param name="zFar">The far edge of the projection volume.</param>
		/// <param name="result">The resulting Matrix4D instance.</param>
		public static void CreateOrthographic(double width, double height, double zNear, double zFar, out Matrix4D result)
		{
			CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
		}
		/// <summary>
		/// Creates an orthographic projection matrix.
		/// </summary>
		/// <param name="width">The width of the projection volume.</param>
		/// <param name="height">The height of the projection volume.</param>
		/// <param name="zNear">The near edge of the projection volume.</param>
		/// <param name="zFar">The far edge of the projection volume.</param>
		/// <rereturns>The resulting Matrix4D instance.</rereturns>
		public static Matrix4D CreateOrthographic(double width, double height, double zNear, double zFar)
		{
			Matrix4D result;
			CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
			return result;
		}

		/// <summary>
		/// Creates an orthographic projection matrix.
		/// </summary>
		/// <param name="left">The left edge of the projection volume.</param>
		/// <param name="right">The right edge of the projection volume.</param>
		/// <param name="bottom">The bottom edge of the projection volume.</param>
		/// <param name="top">The top edge of the projection volume.</param>
		/// <param name="zNear">The near edge of the projection volume.</param>
		/// <param name="zFar">The far edge of the projection volume.</param>
		/// <param name="result">The resulting Matrix4D instance.</param>
		public static void CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNear, double zFar, out Matrix4D result)
		{
			result = Identity;

			double invRL = 1.0f / (right - left);
			double invTB = 1.0f / (top - bottom);
			double invFN = 1.0f / (zFar - zNear);

			result.Row0.X = 2 * invRL;
			result.Row1.Y = 2 * invTB;
			result.Row2.Z = -2 * invFN;

			result.Row3.X = -(right + left) * invRL;
			result.Row3.Y = -(top + bottom) * invTB;
			result.Row3.Z = -(zFar + zNear) * invFN;
		}
		/// <summary>
		/// Creates an orthographic projection matrix.
		/// </summary>
		/// <param name="left">The left edge of the projection volume.</param>
		/// <param name="right">The right edge of the projection volume.</param>
		/// <param name="bottom">The bottom edge of the projection volume.</param>
		/// <param name="top">The top edge of the projection volume.</param>
		/// <param name="zNear">The near edge of the projection volume.</param>
		/// <param name="zFar">The far edge of the projection volume.</param>
		/// <returns>The resulting Matrix4D instance.</returns>
		public static Matrix4D CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNear, double zFar)
		{
			Matrix4D result;
			CreateOrthographicOffCenter(left, right, bottom, top, zNear, zFar, out result);
			return result;
		}
        
		/// <summary>
		/// Creates a perspective projection matrix.
		/// </summary>
		/// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
		/// <param name="aspect">Aspect ratio of the view (width / height)</param>
		/// <param name="zNear">Distance to the near clip plane</param>
		/// <param name="zFar">Distance to the far clip plane</param>
		/// <param name="result">A projection matrix that transforms camera space to raster space</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown under the following conditions:
		/// <list type="bullet">
		/// <item>fovy is zero, less than zero or larger than Math.PI</item>
		/// <item>aspect is negative or zero</item>
		/// <item>zNear is negative or zero</item>
		/// <item>zFar is negative or zero</item>
		/// <item>zNear is larger than zFar</item>
		/// </list>
		/// </exception>
		public static void CreatePerspectiveFieldOfView(double fovy, double aspect, double zNear, double zFar, out Matrix4D result)
		{
			if (fovy <= 0 || fovy > Math.PI)
				throw new ArgumentOutOfRangeException("fovy");
			if (aspect <= 0)
				throw new ArgumentOutOfRangeException("aspect");
			if (zNear <= 0)
				throw new ArgumentOutOfRangeException("zNear");
			if (zFar <= 0)
				throw new ArgumentOutOfRangeException("zFar");
            
			double yMax = zNear * (double)System.Math.Tan(0.5f * fovy);
			double yMin = -yMax;
			double xMin = yMin * aspect;
			double xMax = yMax * aspect;

			CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar, out result);
		}
		/// <summary>
		/// Creates a perspective projection matrix.
		/// </summary>
		/// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
		/// <param name="aspect">Aspect ratio of the view (width / height)</param>
		/// <param name="zNear">Distance to the near clip plane</param>
		/// <param name="zFar">Distance to the far clip plane</param>
		/// <returns>A projection matrix that transforms camera space to raster space</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown under the following conditions:
		/// <list type="bullet">
		/// <item>fovy is zero, less than zero or larger than Math.PI</item>
		/// <item>aspect is negative or zero</item>
		/// <item>zNear is negative or zero</item>
		/// <item>zFar is negative or zero</item>
		/// <item>zNear is larger than zFar</item>
		/// </list>
		/// </exception>
		public static Matrix4D CreatePerspectiveFieldOfView(double fovy, double aspect, double zNear, double zFar)
		{
			Matrix4D result;
			CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar, out result);
			return result;
		}
        
		/// <summary>
		/// Creates an perspective projection matrix.
		/// </summary>
		/// <param name="left">Left edge of the view frustum</param>
		/// <param name="right">Right edge of the view frustum</param>
		/// <param name="bottom">Bottom edge of the view frustum</param>
		/// <param name="top">Top edge of the view frustum</param>
		/// <param name="zNear">Distance to the near clip plane</param>
		/// <param name="zFar">Distance to the far clip plane</param>
		/// <param name="result">A projection matrix that transforms camera space to raster space</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown under the following conditions:
		/// <list type="bullet">
		/// <item>zNear is negative or zero</item>
		/// <item>zFar is negative or zero</item>
		/// <item>zNear is larger than zFar</item>
		/// </list>
		/// </exception>
		public static void CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double zNear, double zFar, out Matrix4D result)
		{
			if (zNear <= 0)
				throw new ArgumentOutOfRangeException("zNear");
			if (zFar <= 0)
				throw new ArgumentOutOfRangeException("zFar");
			if (zNear >= zFar)
				throw new ArgumentOutOfRangeException("zNear");

			double x = (2.0f * zNear) / (right - left);
			double y = (2.0f * zNear) / (top - bottom);
			double a = (right + left) / (right - left);
			double b = (top + bottom) / (top - bottom);
			double c = -(zFar + zNear) / (zFar - zNear);
			double d = -(2.0f * zFar * zNear) / (zFar - zNear);

			result.Row0.X = x;
			result.Row0.Y = 0;
			result.Row0.Z = 0;
			result.Row0.W = 0;
			result.Row1.X = 0;
			result.Row1.Y = y;
			result.Row1.Z = 0;
			result.Row1.W = 0;
			result.Row2.X = a;
			result.Row2.Y = b;
			result.Row2.Z = c;
			result.Row2.W = -1;
			result.Row3.X = 0;
			result.Row3.Y = 0;
			result.Row3.Z = d;
			result.Row3.W = 0;
		}
		/// <summary>
		/// Creates an perspective projection matrix.
		/// </summary>
		/// <param name="left">Left edge of the view frustum</param>
		/// <param name="right">Right edge of the view frustum</param>
		/// <param name="bottom">Bottom edge of the view frustum</param>
		/// <param name="top">Top edge of the view frustum</param>
		/// <param name="zNear">Distance to the near clip plane</param>
		/// <param name="zFar">Distance to the far clip plane</param>
		/// <returns>A projection matrix that transforms camera space to raster space</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown under the following conditions:
		/// <list type="bullet">
		/// <item>zNear is negative or zero</item>
		/// <item>zFar is negative or zero</item>
		/// <item>zNear is larger than zFar</item>
		/// </list>
		/// </exception>
		public static Matrix4D CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double zNear, double zFar)
		{
			Matrix4D result;
			CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar, out result);
			return result;
		}

		/// <summary>
		/// Build a world space to camera space matrix
		/// </summary>
		/// <param name="eye">Eye (camera) position in world space</param>
		/// <param name="target">Target position in world space</param>
		/// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
		/// <returns>A Matrix4D that transforms world space to camera space</returns>
		public static Matrix4D LookAt(Vector3D eye, Vector3D target, Vector3D up)
		{
			Vector3D z = (eye - target).Normalized;
			Vector3D x = (Vector3D.Cross(up, z)).Normalized;
			Vector3D y = (Vector3D.Cross(z, x)).Normalized;

			Matrix4D result;

			result.Row0.X = x.X;
			result.Row0.Y = y.X;
			result.Row0.Z = z.X;
			result.Row0.W = 0;
			result.Row1.X = x.Y;
			result.Row1.Y = y.Y;
			result.Row1.Z = z.Y;
			result.Row1.W = 0;
			result.Row2.X = x.Z;
			result.Row2.Y = y.Z;
			result.Row2.Z = z.Z;
			result.Row2.W = 0;
			result.Row3.X = -((x.X * eye.X) + (x.Y * eye.Y) + (x.Z * eye.Z));
			result.Row3.Y = -((y.X * eye.X) + (y.Y * eye.Y) + (y.Z * eye.Z));
			result.Row3.Z = -((z.X * eye.X) + (z.Y * eye.Y) + (z.Z * eye.Z));
			result.Row3.W = 1;

			return result;
		}
		/// <summary>
		/// Build a world space to camera space matrix
		/// </summary>
		/// <param name="eyeX">Eye (camera) position in world space</param>
		/// <param name="eyeY">Eye (camera) position in world space</param>
		/// <param name="eyeZ">Eye (camera) position in world space</param>
		/// <param name="targetX">Target position in world space</param>
		/// <param name="targetY">Target position in world space</param>
		/// <param name="targetZ">Target position in world space</param>
		/// <param name="upX">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
		/// <param name="upY">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
		/// <param name="upZ">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
		/// <returns>A Matrix4D that transforms world space to camera space</returns>
		public static Matrix4D LookAt(double eyeX, double eyeY, double eyeZ, double targetX, double targetY, double targetZ, double upX, double upY, double upZ)
		{
			return LookAt(new Vector3D(eyeX, eyeY, eyeZ), new Vector3D(targetX, targetY, targetZ), new Vector3D(upX, upY, upZ));
		}

		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The left operand of the addition.</param>
		/// <param name="right">The right operand of the addition.</param>
		/// <returns>A new instance that is the result of the addition.</returns>
		public static Matrix4D Add(Matrix4D left, Matrix4D right)
		{
			Matrix4D result;
			Add(ref left, ref right, out result);
			return result;
		}
		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The left operand of the addition.</param>
		/// <param name="right">The right operand of the addition.</param>
		/// <param name="result">A new instance that is the result of the addition.</param>
		public static void Add(ref Matrix4D left, ref Matrix4D right, out Matrix4D result)
		{
			result.Row0 = left.Row0 + right.Row0;
			result.Row1 = left.Row1 + right.Row1;
			result.Row2 = left.Row2 + right.Row2;
			result.Row3 = left.Row3 + right.Row3;
		}

		/// <summary>
		/// Subtracts one instance from another.
		/// </summary>
		/// <param name="left">The left operand of the subraction.</param>
		/// <param name="right">The right operand of the subraction.</param>
		/// <returns>A new instance that is the result of the subraction.</returns>
		public static Matrix4D Subtract(Matrix4D left, Matrix4D right)
		{
			Matrix4D result;
			Subtract(ref left, ref right, out result);
			return result;
		}
		/// <summary>
		/// Subtracts one instance from another.
		/// </summary>
		/// <param name="left">The left operand of the subraction.</param>
		/// <param name="right">The right operand of the subraction.</param>
		/// <param name="result">A new instance that is the result of the subraction.</param>
		public static void Subtract(ref Matrix4D left, ref Matrix4D right, out Matrix4D result)
		{
			result.Row0 = left.Row0 - right.Row0;
			result.Row1 = left.Row1 - right.Row1;
			result.Row2 = left.Row2 - right.Row2;
			result.Row3 = left.Row3 - right.Row3;
		}

		/// <summary>
		/// Multiplies two instances.
		/// </summary>
		/// <param name="left">The left operand of the multiplication.</param>
		/// <param name="right">The right operand of the multiplication.</param>
		/// <returns>A new instance that is the result of the multiplication.</returns>
		public static Matrix4D Mult(Matrix4D left, Matrix4D right)
		{
			Matrix4D result;
			Mult(ref left, ref right, out result);
			return result;
		}
		/// <summary>
		/// Multiplies two instances.
		/// </summary>
		/// <param name="left">The left operand of the multiplication.</param>
		/// <param name="right">The right operand of the multiplication.</param>
		/// <param name="result">A new instance that is the result of the multiplication.</param>
		public static void Mult(ref Matrix4D left, ref Matrix4D right, out Matrix4D result)
		{
			double lM11 = left.Row0.X, lM12 = left.Row0.Y, lM13 = left.Row0.Z, lM14 = left.Row0.W,
				lM21 = left.Row1.X, lM22 = left.Row1.Y, lM23 = left.Row1.Z, lM24 = left.Row1.W,
				lM31 = left.Row2.X, lM32 = left.Row2.Y, lM33 = left.Row2.Z, lM34 = left.Row2.W,
				lM41 = left.Row3.X, lM42 = left.Row3.Y, lM43 = left.Row3.Z, lM44 = left.Row3.W,
				rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z, rM14 = right.Row0.W,
				rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z, rM24 = right.Row1.W,
				rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z, rM34 = right.Row2.W,
				rM41 = right.Row3.X, rM42 = right.Row3.Y, rM43 = right.Row3.Z, rM44 = right.Row3.W;

			result.Row0.X = (((lM11 * rM11) + (lM12 * rM21)) + (lM13 * rM31)) + (lM14 * rM41);
			result.Row0.Y = (((lM11 * rM12) + (lM12 * rM22)) + (lM13 * rM32)) + (lM14 * rM42);
			result.Row0.Z = (((lM11 * rM13) + (lM12 * rM23)) + (lM13 * rM33)) + (lM14 * rM43);
			result.Row0.W = (((lM11 * rM14) + (lM12 * rM24)) + (lM13 * rM34)) + (lM14 * rM44);
			result.Row1.X = (((lM21 * rM11) + (lM22 * rM21)) + (lM23 * rM31)) + (lM24 * rM41);
			result.Row1.Y = (((lM21 * rM12) + (lM22 * rM22)) + (lM23 * rM32)) + (lM24 * rM42);
			result.Row1.Z = (((lM21 * rM13) + (lM22 * rM23)) + (lM23 * rM33)) + (lM24 * rM43);
			result.Row1.W = (((lM21 * rM14) + (lM22 * rM24)) + (lM23 * rM34)) + (lM24 * rM44);
			result.Row2.X = (((lM31 * rM11) + (lM32 * rM21)) + (lM33 * rM31)) + (lM34 * rM41);
			result.Row2.Y = (((lM31 * rM12) + (lM32 * rM22)) + (lM33 * rM32)) + (lM34 * rM42);
			result.Row2.Z = (((lM31 * rM13) + (lM32 * rM23)) + (lM33 * rM33)) + (lM34 * rM43);
			result.Row2.W = (((lM31 * rM14) + (lM32 * rM24)) + (lM33 * rM34)) + (lM34 * rM44);
			result.Row3.X = (((lM41 * rM11) + (lM42 * rM21)) + (lM43 * rM31)) + (lM44 * rM41);
			result.Row3.Y = (((lM41 * rM12) + (lM42 * rM22)) + (lM43 * rM32)) + (lM44 * rM42);
			result.Row3.Z = (((lM41 * rM13) + (lM42 * rM23)) + (lM43 * rM33)) + (lM44 * rM43);
			result.Row3.W = (((lM41 * rM14) + (lM42 * rM24)) + (lM43 * rM34)) + (lM44 * rM44);
		}
		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="left">The left operand of the multiplication.</param>
		/// <param name="right">The right operand of the multiplication.</param>
		/// <returns>A new instance that is the result of the multiplication</returns>
		public static Matrix4D Mult(Matrix4D left, double right)
		{
			Matrix4D result;
			Mult(ref left, right, out result);
			return result;
		}
		/// <summary>
		/// Multiplies an instance by a scalar.
		/// </summary>
		/// <param name="left">The left operand of the multiplication.</param>
		/// <param name="right">The right operand of the multiplication.</param>
		/// <param name="result">A new instance that is the result of the multiplication</param>
		public static void Mult(ref Matrix4D left, double right, out Matrix4D result)
		{
			result.Row0 = left.Row0 * right;
			result.Row1 = left.Row1 * right;
			result.Row2 = left.Row2 * right;
			result.Row3 = left.Row3 * right;
		}

		/// <summary>
		/// Calculate the inverse of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to invert</param>
		/// <param name="result">The inverse of the given matrix if it has one, or the input if it is singular</param>
		/// <exception cref="InvalidOperationException">Thrown if the Matrix4D is singular.</exception>
		public static void Invert(ref Matrix4D mat, out Matrix4D result)
		{
			int[] colIdx = { 0, 0, 0, 0 };
			int[] rowIdx = { 0, 0, 0, 0 };
			int[] pivotIdx = { -1, -1, -1, -1 };

			// convert the matrix to an array for easy looping
			double[,] inverse = {{mat.Row0.X, mat.Row0.Y, mat.Row0.Z, mat.Row0.W}, 
								{mat.Row1.X, mat.Row1.Y, mat.Row1.Z, mat.Row1.W}, 
								{mat.Row2.X, mat.Row2.Y, mat.Row2.Z, mat.Row2.W}, 
								{mat.Row3.X, mat.Row3.Y, mat.Row3.Z, mat.Row3.W} };
			int icol = 0;
			int irow = 0;
			for (int i = 0; i < 4; i++)
			{
				// Find the largest pivot value
				double maxPivot = 0.0f;
				for (int j = 0; j < 4; j++)
				{
					if (pivotIdx[j] != 0)
					{
						for (int k = 0; k < 4; ++k)
						{
							if (pivotIdx[k] == -1)
							{
								double absVal = System.Math.Abs(inverse[j, k]);
								if (absVal > maxPivot)
								{
									maxPivot = absVal;
									irow = j;
									icol = k;
								}
							}
							else if (pivotIdx[k] > 0)
							{
								result = mat;
								return;
							}
						}
					}
				}

				++(pivotIdx[icol]);

				// Swap rows over so pivot is on diagonal
				if (irow != icol)
				{
					for (int k = 0; k < 4; ++k)
					{
						double f = inverse[irow, k];
						inverse[irow, k] = inverse[icol, k];
						inverse[icol, k] = f;
					}
				}

				rowIdx[i] = irow;
				colIdx[i] = icol;

				double pivot = inverse[icol, icol];
				// check for singular matrix
				if (pivot == 0.0f)
				{
					throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
				}

				// Scale row so it has a unit diagonal
				double oneOverPivot = 1.0f / pivot;
				inverse[icol, icol] = 1.0f;
				for (int k = 0; k < 4; ++k)
					inverse[icol, k] *= oneOverPivot;

				// Do elimination of non-diagonal elements
				for (int j = 0; j < 4; ++j)
				{
					// check this isn't on the diagonal
					if (icol != j)
					{
						double f = inverse[j, icol];
						inverse[j, icol] = 0.0f;
						for (int k = 0; k < 4; ++k)
							inverse[j, k] -= inverse[icol, k] * f;
					}
				}
			}

			for (int j = 3; j >= 0; --j)
			{
				int ir = rowIdx[j];
				int ic = colIdx[j];
				for (int k = 0; k < 4; ++k)
				{
					double f = inverse[k, ir];
					inverse[k, ir] = inverse[k, ic];
					inverse[k, ic] = f;
				}
			}

			result.Row0.X = inverse[0, 0];
			result.Row0.Y = inverse[0, 1];
			result.Row0.Z = inverse[0, 2];
			result.Row0.W = inverse[0, 3];
			result.Row1.X = inverse[1, 0];
			result.Row1.Y = inverse[1, 1];
			result.Row1.Z = inverse[1, 2];
			result.Row1.W = inverse[1, 3];
			result.Row2.X = inverse[2, 0];
			result.Row2.Y = inverse[2, 1];
			result.Row2.Z = inverse[2, 2];
			result.Row2.W = inverse[2, 3];
			result.Row3.X = inverse[3, 0];
			result.Row3.Y = inverse[3, 1];
			result.Row3.Z = inverse[3, 2];
			result.Row3.W = inverse[3, 3];
		}
		/// <summary>
		/// Calculate the inverse of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to invert</param>
		/// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
		/// <exception cref="InvalidOperationException">Thrown if the Matrix4D is singular.</exception>
		public static Matrix4D Invert(Matrix4D mat)
		{
			Matrix4D result;
			Invert(ref mat, out result);
			return result;
		}

		/// <summary>
		/// Calculate the transpose of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to transpose</param>
		/// <returns>The transpose of the given matrix</returns>
		public static Matrix4D Transpose(Matrix4D mat)
		{
			return new Matrix4D(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
		}
		/// <summary>
		/// Calculate the transpose of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to transpose</param>
		/// <param name="result">The result of the calculation</param>
		public static void Transpose(ref Matrix4D mat, out Matrix4D result)
		{
			result.Row0 = mat.Column0;
			result.Row1 = mat.Column1;
			result.Row2 = mat.Column2;
			result.Row3 = mat.Column3;
		}

		/// <summary>
		/// Matrix multiplication
		/// </summary>
		/// <param name="left">left-hand operand</param>
		/// <param name="right">right-hand operand</param>
		/// <returns>A new Matrix4D which holds the result of the multiplication</returns>
		public static Matrix4D operator *(Matrix4D left, Matrix4D right)
		{
			return Matrix4D.Mult(left, right);
		}
		/// <summary>
		/// Matrix-scalar multiplication
		/// </summary>
		/// <param name="left">left-hand operand</param>
		/// <param name="right">right-hand operand</param>
		/// <returns>A new Matrix4D which holds the result of the multiplication</returns>
		public static Matrix4D operator *(Matrix4D left, double right)
		{
			return Matrix4D.Mult(left, right);
		}
		/// <summary>
		/// Matrix addition
		/// </summary>
		/// <param name="left">left-hand operand</param>
		/// <param name="right">right-hand operand</param>
		/// <returns>A new Matrix4D which holds the result of the addition</returns>
		public static Matrix4D operator +(Matrix4D left, Matrix4D right)
		{
			return Matrix4D.Add(left, right);
		}
		/// <summary>
		/// Matrix subtraction
		/// </summary>
		/// <param name="left">left-hand operand</param>
		/// <param name="right">right-hand operand</param>
		/// <returns>A new Matrix4D which holds the result of the subtraction</returns>
		public static Matrix4D operator -(Matrix4D left, Matrix4D right)
		{
			return Matrix4D.Subtract(left, right);
		}

		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator ==(Matrix4D left, Matrix4D right)
		{
			return left.Equals(right);
		}
		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equal right; false otherwise.</returns>
		public static bool operator !=(Matrix4D left, Matrix4D right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Returns a System.String that represents the current Matrix4D.
		/// </summary>
		/// <returns>The string representation of the matrix.</returns>
		public override string ToString()
		{
			return string.Format("{0}\n{1}\n{2}\n{3}", this.Row0, this.Row1, this.Row2, this.Row3);
		}
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode()
		{
			return this.Row0.GetHashCode() ^ this.Row1.GetHashCode() ^ this.Row2.GetHashCode() ^ this.Row3.GetHashCode();
		}
		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare tresult.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Matrix4D))
				return false;

			return this.Equals((Matrix4D)obj);
		}
		/// <summary>
		/// Indicates whether the current matrix is equal to another matrix.
		/// </summary>
		/// <param name="other">An matrix to compare with this matrix.</param>
		/// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
		public bool Equals(Matrix4D other)
		{
			return
				this.Row0 == other.Row0 &&
				this.Row1 == other.Row1 &&
				this.Row2 == other.Row2 &&
				this.Row3 == other.Row3;
		}


        /// <summary>
        /// Convert from QuaternionD
        /// </summary>
        /// <param name="v"></param
        public static implicit operator Matrix4D(Matrix4 m)
        {
            return new Matrix4D(m.Row0, m.Row1, m.Row2, m.Row3);
        }

        /// <summary>
        /// Convert to QuaternionD
        /// </summary>
        public static implicit operator Matrix4(Matrix4D m)
        {
            return new Matrix4(m.Row0, m.Row1, m.Row2, m.Row3);
        }
    }
}
