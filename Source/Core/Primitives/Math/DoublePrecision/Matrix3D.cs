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
	/// Represents a 3x3 matrix containing 3D rotation and scale.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix3D : IEquatable<Matrix3D>
	{
		/// <summary>
		/// The identity matrix.
		/// </summary>
		public static readonly Matrix3D Identity = new Matrix3D(Vector3D.UnitX, Vector3D.UnitY, Vector3D.UnitZ);
		/// <summary>
		/// The zero matrix.
		/// </summary>
		public static readonly Matrix3D Zero = new Matrix3D(Vector3D.Zero, Vector3D.Zero, Vector3D.Zero);

		/// <summary>
		/// First row of the matrix.
		/// </summary>
		public Vector3D Row0;
		/// <summary>
		/// Second row of the matrix.
		/// </summary>
		public Vector3D Row1;
		/// <summary>
		/// Third row of the matrix.
		/// </summary>
		public Vector3D Row2;


		/// <summary>
		/// Gets the first column of this matrix.
		/// </summary>
		public Vector3D Column0
		{
			get { return new Vector3D(this.Row0.X, this.Row1.X, this.Row2.X); }
		}
		/// <summary>
		/// Gets the second column of this matrix.
		/// </summary>
		public Vector3D Column1
		{
			get { return new Vector3D(this.Row0.Y, this.Row1.Y, this.Row2.Y); }
		}
		/// <summary>
		/// Gets the third column of this matrix.
		/// </summary>
		public Vector3D Column2
		{
			get { return new Vector3D(this.Row0.Z, this.Row1.Z, this.Row2.Z); }
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
		/// Gets or sets the value at a specified row and column.
		/// </summary>
		public double this[int rowIndex, int columnIndex]
		{
			get
			{
				if (rowIndex == 0) return this.Row0[columnIndex];
				else if (rowIndex == 1) return this.Row1[columnIndex];
				else if (rowIndex == 2) return this.Row2[columnIndex];
				throw new IndexOutOfRangeException("You tried to access this matrix at: (" + rowIndex + ", " + columnIndex + ")");
			}
			set
			{
				if (rowIndex == 0) this.Row0[columnIndex] = value;
				else if (rowIndex == 1) this.Row1[columnIndex] = value;
				else if (rowIndex == 2) this.Row2[columnIndex] = value;
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
				double m11 = this.Row0.X, m12 = this.Row0.Y, m13 = this.Row0.Z,
				m21 = this.Row1.X, m22 = this.Row1.Y, m23 = this.Row1.Z,
				m31 = this.Row2.X, m32 = this.Row2.Y, m33 = this.Row2.Z;
                
				return m11 * m22 * m33 + m12 * m23 * m31 + m13 * m21 * m32
						- m13 * m22 * m31 - m11 * m23 * m32 - m12 * m21 * m33;
			}
		}
		/// <summary>
		/// Gets or sets the values along the main diagonal of the matrix.
		/// </summary>
		public Vector3D Diagonal
		{
			get
			{
				return new Vector3D(this.Row0.X, this.Row1.Y, this.Row2.Z);
			}
			set
			{
				this.Row0.X = value.X;
				this.Row1.Y = value.Y;
				this.Row2.Z = value.Z;
			}
		}
		/// <summary>
		/// Gets the trace of the matrix, the sum of the values along the diagonal.
		/// </summary>
		public double Trace { get { return this.Row0.X + this.Row1.Y + this.Row2.Z; } }


		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="row0">Top row of the matrix</param>
		/// <param name="row1">Second row of the matrix</param>
		/// <param name="row2">Bottom row of the matrix</param>
		public Matrix3D(Vector3D row0, Vector3D row1, Vector3D row2)
		{
			this.Row0 = row0;
			this.Row1 = row1;
			this.Row2 = row2;
		}
		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="m00">First item of the first row of the matrix.</param>
		/// <param name="m01">Second item of the first row of the matrix.</param>
		/// <param name="m02">Third item of the first row of the matrix.</param>
		/// <param name="m10">First item of the second row of the matrix.</param>
		/// <param name="m11">Second item of the second row of the matrix.</param>
		/// <param name="m12">Third item of the second row of the matrix.</param>
		/// <param name="m20">First item of the third row of the matrix.</param>
		/// <param name="m21">Second item of the third row of the matrix.</param>
		/// <param name="m22">Third item of the third row of the matrix.</param>
		public Matrix3D(
			double m00, double m01, double m02,
			double m10, double m11, double m12,
			double m20, double m21, double m22)
		{
			this.Row0 = new Vector3D(m00, m01, m02);
			this.Row1 = new Vector3D(m10, m11, m12);
			this.Row2 = new Vector3D(m20, m21, m22);
		}
		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		/// <param name="matrix">A Matrix4D to take the upper-left 3x3 from.</param>
		public Matrix3D(Matrix4D matrix)
		{
			this.Row0 = matrix.Row0.Xyz;
			this.Row1 = matrix.Row1.Xyz;
			this.Row2 = matrix.Row2.Xyz;
		}

		/// <summary>
		/// Converts this instance into its inverse.
		/// </summary>
		public void Invert()
		{
			this = Matrix3D.Invert(this);
		}
		/// <summary>
		/// Converts this instance into its transpose.
		/// </summary>
		public void Transpose()
		{
			this = Matrix3D.Transpose(this);
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
		}

		/// <summary>
		/// Returns a normalised copy of this instance.
		/// </summary>
		public Matrix3D Normalized()
		{
			Matrix3D m = this;
			m.Normalize();
			return m;
		}
		/// <summary>
		/// Returns an inverted copy of this instance.
		/// </summary>
		public Matrix3D Inverted()
		{
			Matrix3D m = this;
			if (m.Determinant != 0)
				m.Invert();
			return m;
		}

		/// <summary>
		/// Returns a copy of this Matrix3D without scale.
		/// </summary>
		public Matrix3D ClearScale()
		{
			Matrix3D m = this;
			m.Row0 = m.Row0.Normalized;
			m.Row1 = m.Row1.Normalized;
			m.Row2 = m.Row2.Normalized;
			return m;
		}
		/// <summary>
		/// Returns a copy of this Matrix3D without rotation.
		/// </summary>
		public Matrix3D ClearRotation()
		{
			Matrix3D m = this;
			m.Row0 = new Vector3D(m.Row0.Length, 0, 0);
			m.Row1 = new Vector3D(0, m.Row1.Length, 0);
			m.Row2 = new Vector3D(0, 0, m.Row2.Length);
			return m;
		}

		/// <summary>
		/// Returns the scale component of this instance.
		/// </summary>
		public Vector3D ExtractScale() { return new Vector3D(this.Row0.Length, this.Row1.Length, this.Row2.Length); }
		/// <summary>
		/// Returns the rotation component of this instance. Quite slow.
		/// </summary>
		/// <param name="row_normalise">Whether the method should row-normalise (i.e. remove scale from) the Matrix. Pass false if you know it's already normalised.</param>
		public QuaternionD ExtractRotation(bool row_normalise = true)
		{
			var row0 = this.Row0;
			var row1 = this.Row1;
			var row2 = this.Row2;

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
		/// Build a rotation matrix from the specified axis/angle rotation.
		/// </summary>
		/// <param name="axis">The axis to rotate about.</param>
		/// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
		/// <param name="result">A matrix instance.</param>
		public static void CreateFromAxisAngle(Vector3D axis, double angle, out Matrix3D result)
		{
			//normalize and create a local copy of the vector.
			axis.Normalize();
			double axisX = axis.X, axisY = axis.Y, axisZ = axis.Z;
            
			//calculate angles
			double cos = (double)System.Math.Cos(-angle);
			double sin = (double)System.Math.Sin(-angle);
			double t = 1.0f - cos;
            
			//do the conversion math once
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
			result.Row1.X = tXY + sinZ;
			result.Row1.Y = tYY + cos;
			result.Row1.Z = tYZ - sinX;
			result.Row2.X = tXZ - sinY;
			result.Row2.Y = tYZ + sinX;
			result.Row2.Z = tZZ + cos;
		}
		/// <summary>
		/// Build a rotation matrix from the specified axis/angle rotation.
		/// </summary>
		/// <param name="axis">The axis to rotate about.</param>
		/// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
		/// <returns>A matrix instance.</returns>
		public static Matrix3D CreateFromAxisAngle(Vector3D axis, double angle)
		{
			Matrix3D result;
			CreateFromAxisAngle(axis, angle, out result);
			return result;
		}

		/// <summary>
		/// Build a rotation matrix from the specified quaternion.
		/// </summary>
		/// <param name="q">QuaternionD to translate.</param>
		/// <param name="result">Matrix result.</param>
		public static void CreateFromQuaternion(ref QuaternionD q, out Matrix3D result)
		{
			Vector3D axis;
			double angle;
			q.ToAxisAngle(out axis, out angle);
			CreateFromAxisAngle(axis, angle, out result);
		}
		/// <summary>
		/// Build a rotation matrix from the specified quaternion.
		/// </summary>
		/// <param name="q">QuaternionD to translate.</param>
		/// <returns>A matrix instance.</returns>
		public static Matrix3D CreateFromQuaternion(QuaternionD q)
		{
			Matrix3D result;
			CreateFromQuaternion(ref q, out result);
			return result;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the x-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <param name="result">The resulting Matrix3D instance.</param>
		public static void CreateRotationX(double angle, out Matrix3D result)
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
		/// <returns>The resulting Matrix3D instance.</returns>
		public static Matrix3D CreateRotationX(double angle)
		{
			Matrix3D result;
			CreateRotationX(angle, out result);
			return result;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the y-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <param name="result">The resulting Matrix3D instance.</param>
		public static void CreateRotationY(double angle, out Matrix3D result)
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
		/// <returns>The resulting Matrix3D instance.</returns>
		public static Matrix3D CreateRotationY(double angle)
		{
			Matrix3D result;
			CreateRotationY(angle, out result);
			return result;
		}

		/// <summary>
		/// Builds a rotation matrix for a rotation around the z-axis.
		/// </summary>
		/// <param name="angle">The counter-clockwise angle in radians.</param>
		/// <param name="result">The resulting Matrix3D instance.</param>
		public static void CreateRotationZ(double angle, out Matrix3D result)
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
		/// <returns>The resulting Matrix3D instance.</returns>
		public static Matrix3D CreateRotationZ(double angle)
		{
			Matrix3D result;
			CreateRotationZ(angle, out result);
			return result;
		}
        
		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Single scale factor for the x, y, and z axes.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix3D CreateScale(double scale)
		{
			Matrix3D result;
			CreateScale(scale, out result);
			return result;
		}
		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Scale factors for the x, y, and z axes.</param>
		/// <returns>A scale matrix.</returns>
		public static Matrix3D CreateScale(Vector3D scale)
		{
			Matrix3D result;
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
		public static Matrix3D CreateScale(double x, double y, double z)
		{
			Matrix3D result;
			CreateScale(x, y, z, out result);
			return result;
		}
		/// <summary>
		/// Creates a scale matrix.
		/// </summary>
		/// <param name="scale">Single scale factor for the x, y, and z axes.</param>
		/// <param name="result">A scale matrix.</param>
		public static void CreateScale(double scale, out Matrix3D result)
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
		public static void CreateScale(ref Vector3D scale, out Matrix3D result)
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
		public static void CreateScale(double x, double y, double z, out Matrix3D result)
		{
			result = Identity;
			result.Row0.X = x;
			result.Row1.Y = y;
			result.Row2.Z = z;
		}

		/// <summary>
		/// Multiplies two instances.
		/// </summary>
		/// <param name="left">The left operand of the multiplication.</param>
		/// <param name="right">The right operand of the multiplication.</param>
		/// <returns>A new instance that is the result of the multiplication</returns>
		public static Matrix3D Mult(Matrix3D left, Matrix3D right)
		{
			Matrix3D result;
			Mult(ref left, ref right, out result);
			return result;
		}
		/// <summary>
		/// Multiplies two instances.
		/// </summary>
		/// <param name="left">The left operand of the multiplication.</param>
		/// <param name="right">The right operand of the multiplication.</param>
		/// <param name="result">A new instance that is the result of the multiplication</param>
		public static void Mult(ref Matrix3D left, ref Matrix3D right, out Matrix3D result)
		{
			double   lM11 = left.Row0.X, lM12 = left.Row0.Y, lM13 = left.Row0.Z,
			lM21 = left.Row1.X, lM22 = left.Row1.Y, lM23 = left.Row1.Z,
			lM31 = left.Row2.X, lM32 = left.Row2.Y, lM33 = left.Row2.Z,
			rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z,
			rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z,
			rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z;
            
			result.Row0.X = ((lM11 * rM11) + (lM12 * rM21)) + (lM13 * rM31);
			result.Row0.Y = ((lM11 * rM12) + (lM12 * rM22)) + (lM13 * rM32);
			result.Row0.Z = ((lM11 * rM13) + (lM12 * rM23)) + (lM13 * rM33);
			result.Row1.X = ((lM21 * rM11) + (lM22 * rM21)) + (lM23 * rM31);
			result.Row1.Y = ((lM21 * rM12) + (lM22 * rM22)) + (lM23 * rM32);
			result.Row1.Z = ((lM21 * rM13) + (lM22 * rM23)) + (lM23 * rM33);
			result.Row2.X = ((lM31 * rM11) + (lM32 * rM21)) + (lM33 * rM31);
			result.Row2.Y = ((lM31 * rM12) + (lM32 * rM22)) + (lM33 * rM32);
			result.Row2.Z = ((lM31 * rM13) + (lM32 * rM23)) + (lM33 * rM33);
		}

		/// <summary>
		/// Calculate the inverse of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to invert</param>
		/// <param name="result">The inverse of the given matrix if it has one, or the input if it is singular</param>
		/// <exception cref="InvalidOperationException">Thrown if the Matrix3D is singular.</exception>
		public static void Invert(ref Matrix3D mat, out Matrix3D result)
		{
			int[] colIdx = { 0, 0, 0 };
			int[] rowIdx = { 0, 0, 0 };
			int[] pivotIdx = { -1, -1, -1 };
            
			double[,] inverse = {{mat.Row0.X, mat.Row0.Y, mat.Row0.Z},
				{mat.Row1.X, mat.Row1.Y, mat.Row1.Z},
				{mat.Row2.X, mat.Row2.Y, mat.Row2.Z}};
            
			int icol = 0;
			int irow = 0;
			for (int i = 0; i < 3; i++)
			{
				double maxPivot = 0.0f;
				for (int j = 0; j < 3; j++)
				{
					if (pivotIdx[j] != 0)
					{
						for (int k = 0; k < 3; ++k)
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
                
				if (irow != icol)
				{
					for (int k = 0; k < 3; ++k)
					{
						double f = inverse[irow, k];
						inverse[irow, k] = inverse[icol, k];
						inverse[icol, k] = f;
					}
				}
                
				rowIdx[i] = irow;
				colIdx[i] = icol;
                
				double pivot = inverse[icol, icol];
                
				if (pivot == 0.0f)
				{
					throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
				}
                
				double oneOverPivot = 1.0f / pivot;
				inverse[icol, icol] = 1.0f;
				for (int k = 0; k < 3; ++k)
					inverse[icol, k] *= oneOverPivot;
                
				for (int j = 0; j < 3; ++j)
				{
					if (icol != j)
					{
						double f = inverse[j, icol];
						inverse[j, icol] = 0.0f;
						for (int k = 0; k < 3; ++k)
							inverse[j, k] -= inverse[icol, k] * f;
					}
				}
			}
            
			for (int j = 2; j >= 0; --j)
			{
				int ir = rowIdx[j];
				int ic = colIdx[j];
				for (int k = 0; k < 3; ++k)
				{
					double f = inverse[k, ir];
					inverse[k, ir] = inverse[k, ic];
					inverse[k, ic] = f;
				}
			}
            
			result.Row0.X = inverse[0, 0];
			result.Row0.Y = inverse[0, 1];
			result.Row0.Z = inverse[0, 2];
			result.Row1.X = inverse[1, 0];
			result.Row1.Y = inverse[1, 1];
			result.Row1.Z = inverse[1, 2];
			result.Row2.X = inverse[2, 0];
			result.Row2.Y = inverse[2, 1];
			result.Row2.Z = inverse[2, 2];
		}
		/// <summary>
		/// Calculate the inverse of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to invert</param>
		/// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
		/// <exception cref="InvalidOperationException">Thrown if the Matrix4D is singular.</exception>
		public static Matrix3D Invert(Matrix3D mat)
		{
			Matrix3D result;
			Invert(ref mat, out result);
			return result;
		}

		/// <summary>
		/// Calculate the transpose of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to transpose</param>
		/// <returns>The transpose of the given matrix</returns>
		public static Matrix3D Transpose(Matrix3D mat)
		{
			return new Matrix3D(mat.Column0, mat.Column1, mat.Column2);
		}
		/// <summary>
		/// Calculate the transpose of the given matrix
		/// </summary>
		/// <param name="mat">The matrix to transpose</param>
		/// <param name="result">The result of the calculation</param>
		public static void Transpose(ref Matrix3D mat, out Matrix3D result)
		{
			result.Row0.X = mat.Row0.X;
			result.Row0.Y = mat.Row1.X;
			result.Row0.Z = mat.Row2.X;
			result.Row1.X = mat.Row0.Y;
			result.Row1.Y = mat.Row1.Y;
			result.Row1.Z = mat.Row2.Y;
			result.Row2.X = mat.Row0.Z;
			result.Row2.Y = mat.Row1.Z;
			result.Row2.Z = mat.Row2.Z;
		}

		/// <summary>
		/// Matrix multiplication
		/// </summary>
		/// <param name="left">left-hand operand</param>
		/// <param name="right">right-hand operand</param>
		/// <returns>A new Matrix3d which holds the result of the multiplication</returns>
		public static Matrix3D operator *(Matrix3D left, Matrix3D right)
		{
			return Matrix3D.Mult(left, right);
		}

		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left equals right; false otherwise.</returns>
		public static bool operator ==(Matrix3D left, Matrix3D right)
		{
			return left.Equals(right);
		}
		/// <summary>
		/// Compares two instances for inequality.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>True, if left does not equal right; false otherwise.</returns>
		public static bool operator !=(Matrix3D left, Matrix3D right)
		{
			return !left.Equals(right);
		}
        
		/// <summary>
		/// Returns a System.String that represents the current Matrix3d.
		/// </summary>
		/// <returns>The string representation of the matrix.</returns>
		public override string ToString()
		{
			return string.Format("{0}\n{1}\n{2}", this.Row0, this.Row1, this.Row2);
		}
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
		public override int GetHashCode()
		{
			return this.Row0.GetHashCode() ^ this.Row1.GetHashCode() ^ this.Row2.GetHashCode();
		}
		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>True if the instances are equal; false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Matrix3D))
				return false;
            
			return this.Equals((Matrix3D)obj);
		}
		/// <summary>
		/// Indicates whether the current matrix is equal to another matrix.
		/// </summary>
		/// <param name="other">A matrix to compare with this matrix.</param>
		/// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
		public bool Equals(Matrix3D other)
		{
			return
				this.Row0 == other.Row0 &&
					this.Row1 == other.Row1 &&
					this.Row2 == other.Row2;
        }

        /// <summary>
        /// Convert from Quaternion
        /// </summary>
        /// <param name="v"></param
        public static implicit operator Matrix3D(Matrix3 m)
        {
            return new Matrix3D(m.Row0, m.Row1, m.Row2);
        }

        /// <summary>
        /// Convert to Quaternion
        /// </summary>
        public static implicit operator Matrix3(Matrix3D m)
        {
            return new Matrix3(m.Row0, m.Row1, m.Row2);
        }
    }
}