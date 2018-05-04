using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using Duality;
using Duality.Drawing;
using Duality.Resources;
using Duality.Components;
using Duality.Components.Physics;

using Duality.Editor;
using Duality.Editor.Forms;
using Duality.Editor.Plugins.CamView.CamViewStates;

namespace Duality.Editor.Plugins.CamView.CamViewLayers
{
	public class RigidBodyShapeCamViewLayer : CamViewLayer
	{
		private double shapeOutlineWidth = 2.0f;
		private double depthOffset = -0.5f;

		public override string LayerName
		{
			get { return Properties.CamViewRes.CamViewLayer_RigidBodyShape_Name; }
		}
		public override string LayerDesc
		{
			get { return Properties.CamViewRes.CamViewLayer_RigidBodyShape_Desc; }
		}
		public ColorRgba MassCenterColor
		{
			get
			{
				double fgLum = this.FgColor.GetLuminance();
				if (fgLum > 0.5f)
					return new ColorRgba(255, 128, 255);
				else
					return new ColorRgba(192, 0, 192);
			}
		}
		public ColorRgba ObjectCenterColor
		{
			get
			{
				double fgLum = this.FgColor.GetLuminance();
				if (fgLum > 0.5f)
					return new ColorRgba(255, 255, 128);
				else
					return new ColorRgba(192, 192, 0);
			}
		}
		public ColorRgba ShapeColor
		{
			get
			{
				double fgLum = this.FgColor.GetLuminance();
				if (fgLum > 0.5f)
					return ColorRgba.Lerp(ColorRgba.Blue, ColorRgba.VeryLightGrey, 0.5f);
				else
					return ColorRgba.Lerp(ColorRgba.Blue, ColorRgba.VeryDarkGrey, 0.5f);
			}
		}
		public ColorRgba ShapeSensorColor
		{
			get
			{
				double fgLum = this.FgColor.GetLuminance();
				if (fgLum > 0.5f)
					return ColorRgba.Lerp(new ColorRgba(255, 128, 0), ColorRgba.VeryLightGrey, 0.5f);
				else
					return ColorRgba.Lerp(new ColorRgba(255, 128, 0), ColorRgba.VeryDarkGrey, 0.5f);
			}
		}
		public ColorRgba ShapeErrorColor
		{
			get
			{
				double fgLum = this.FgColor.GetLuminance();
				if (fgLum > 0.5f)
					return ColorRgba.Lerp(ColorRgba.Red, ColorRgba.VeryLightGrey, 0.5f);
				else
					return ColorRgba.Lerp(ColorRgba.Red, ColorRgba.VeryDarkGrey, 0.5f);
			}
		}

		protected internal override void OnCollectWorldOverlayDrawcalls(Canvas canvas)
		{
			base.OnCollectWorldOverlayDrawcalls(canvas);
			List<RigidBody> visibleColliders = this.QueryVisibleColliders().ToList();

			RigidBody selectedBody = this.QuerySelectedCollider();

			canvas.State.SetMaterial(DrawTechnique.Alpha);
			canvas.State.TextFont = Font.GenericMonospace10;
			canvas.State.DepthOffset = (float)this.depthOffset;
			Font textFont = canvas.State.TextFont.Res;

			// Retrieve selected shapes
			ObjectEditorCamViewState editorState = this.View.ActiveState as ObjectEditorCamViewState;
			object[] editorSelectedObjects = editorState != null ? editorState.SelectedObjects.Select(item => item.ActualObject).ToArray() : new object[0];
			
			bool isAnyBodySelected = (selectedBody != null);
			bool isAnyShapeSelected = isAnyBodySelected && editorSelectedObjects.OfType<ShapeInfo>().Any();

			// Draw Shape layer
			foreach (RigidBody body in visibleColliders)
			{
				if (!body.Shapes.Any()) continue;

				Vector3D objPos = body.GameObj.Transform.Pos;
				double objAngle = body.GameObj.Transform.Angle;
				double objScale = body.GameObj.Transform.Scale;

				bool isBodySelected = (body == selectedBody);

				double bodyAlpha = isBodySelected ? 1.0f : (isAnyBodySelected ? 0.5f : 1.0f);
				double maxDensity = body.Shapes.Max(s => s.Density);
				double minDensity = body.Shapes.Min(s => s.Density);
				double avgDensity = (maxDensity + minDensity) * 0.5f;

				int shapeIndex = 0;
				foreach (ShapeInfo shape in body.Shapes)
				{
					bool isShapeSelected = isBodySelected && editorSelectedObjects.Contains(shape);

					double shapeAlpha = bodyAlpha * (isShapeSelected ? 1.0f : (isAnyShapeSelected && isBodySelected ? 0.75f : 1.0f));
					double densityRelative = MathD.Abs(maxDensity - minDensity) < 0.01f ? 1.0f : shape.Density / avgDensity;
					ColorRgba shapeColor = shape.IsSensor ? this.ShapeSensorColor : this.ShapeColor;
					ColorRgba fontColor = this.FgColor;

					if (!body.IsAwake) shapeColor = shapeColor.ToHsva().WithSaturation(0.0f).ToRgba();
					if (!shape.IsValid) shapeColor = this.ShapeErrorColor;

					// Draw the shape itself
					ColorRgba fillColor = shapeColor.WithAlpha((0.25f + densityRelative * 0.25f) * shapeAlpha);
					ColorRgba outlineColor = ColorRgba.Lerp(shapeColor, fontColor, isShapeSelected ? 0.75f : 0.25f).WithAlpha(shapeAlpha);
					this.DrawShape(canvas, body.GameObj.Transform, shape, fillColor, outlineColor);

					// Calculate the center coordinate 
					Vector2D shapeCenter = Vector2D.Zero;
					if (shape is CircleShapeInfo)
					{
						CircleShapeInfo circleShape = shape as CircleShapeInfo;
						shapeCenter = circleShape.Position * objScale;
					}
					else if (shape is VertexBasedShapeInfo)
					{
						VertexBasedShapeInfo vertexShape = shape as VertexBasedShapeInfo;
						Vector2D[] shapeVertices = vertexShape.Vertices;

						for (int i = 0; i < shapeVertices.Length; i++)
							shapeCenter += shapeVertices[i];

						shapeCenter /= shapeVertices.Length;
					}
					MathD.TransformCoord(ref shapeCenter.X, ref shapeCenter.Y, objAngle, objScale);

					// Draw shape index
					if (body == selectedBody)
					{
						string indexText = shapeIndex.ToString();
						Vector2D textSize = textFont.MeasureText(indexText);
						canvas.State.ColorTint = fontColor.WithAlpha((shapeAlpha + 1.0f) * 0.5f);
						canvas.State.TransformScale = Vector2D.One / canvas.DrawDevice.GetScaleAtZ(0.0f);
						canvas.DrawText(indexText, 
							objPos.X + shapeCenter.X, 
							objPos.Y + shapeCenter.Y,
							0.0f);
						canvas.State.TransformScale = Vector2D.One;
					}

					shapeIndex++;
				}
				
				// Draw center of mass
				if (body.BodyType == BodyType.Dynamic)
				{
					Vector2D localMassCenter = body.LocalMassCenter;
					MathD.TransformCoord(ref localMassCenter.X, ref localMassCenter.Y, objAngle, objScale);

					double size = this.GetScreenConstantScale(canvas, 6.0f);

					canvas.State.ColorTint = this.MassCenterColor.WithAlpha(bodyAlpha);
					canvas.DrawLine(
						objPos.X + localMassCenter.X - size, 
						objPos.Y + localMassCenter.Y, 
						0.0f,
						objPos.X + localMassCenter.X + size, 
						objPos.Y + localMassCenter.Y, 
						0.0f);
					canvas.DrawLine(
						objPos.X + localMassCenter.X, 
						objPos.Y + localMassCenter.Y - size, 
						0.0f,
						objPos.X + localMassCenter.X, 
						objPos.Y + localMassCenter.Y + size, 
						0.0f);
				}
				
				// Draw transform center
				{
					double size = this.GetScreenConstantScale(canvas, 3.0f);
					canvas.State.ColorTint = this.ObjectCenterColor.WithAlpha(bodyAlpha);
					canvas.FillCircle(objPos.X, objPos.Y, 0.0f, size);
				}
			}
		}
		
		private void DrawShape(Canvas canvas, Transform transform, ShapeInfo shape, ColorRgba fillColor, ColorRgba outlineColor)
		{
			if      (shape is CircleShapeInfo)      this.DrawShape(canvas, transform, shape as CircleShapeInfo     , fillColor, outlineColor);
			else if (shape is PolyShapeInfo)        this.DrawShape(canvas, transform, shape as PolyShapeInfo       , fillColor, outlineColor);
			else if (shape is VertexBasedShapeInfo) this.DrawShape(canvas, transform, shape as VertexBasedShapeInfo, fillColor, outlineColor);
		}
		private void DrawShape(Canvas canvas, Transform transform, VertexBasedShapeInfo shape, ColorRgba fillColor, ColorRgba outlineColor)
		{
			bool isSolid = (shape.ShapeTraits & VertexShapeTrait.IsSolid) != VertexShapeTrait.None;
			bool isLoop = (shape.ShapeTraits & VertexShapeTrait.IsLoop) != VertexShapeTrait.None;

			if (isSolid)
			{
				this.FillPolygon(canvas, transform, shape.Vertices, fillColor);
			}

			this.DrawPolygonOutline(canvas, transform, shape.Vertices, outlineColor, isLoop);
		}
		private void DrawShape(Canvas canvas, Transform transform, PolyShapeInfo shape, ColorRgba fillColor, ColorRgba outlineColor)
		{
			if (shape.ConvexPolygons != null)
			{
				// Fill each convex polygon individually
				foreach (Vector2D[] polygon in shape.ConvexPolygons)
				{
					this.FillPolygon(canvas, transform, polygon, fillColor);
				}

				// Draw all convex polygon edges that are not outlines
				canvas.State.DepthOffset = (float)this.depthOffset - 0.05f;
				this.DrawPolygonInternals(canvas, transform, shape.Vertices, shape.ConvexPolygons, outlineColor);
				canvas.State.DepthOffset = (float)this.depthOffset;
			}


			// Draw the polygon outline
			canvas.State.DepthOffset = (float)this.depthOffset - 0.1f;
			this.DrawPolygonOutline(canvas, transform, shape.Vertices, outlineColor, true);
			canvas.State.DepthOffset = (float)this.depthOffset;
		}
		private void DrawShape(Canvas canvas, Transform transform, CircleShapeInfo shape, ColorRgba fillColor, ColorRgba outlineColor)
		{
			Vector3D objPos = transform.Pos;
			double objAngle = transform.Angle;
			double objScale = transform.Scale;

			Vector2D circlePos = shape.Position * objScale;
						MathD.TransformCoord(ref circlePos.X, ref circlePos.Y, objAngle);

			if (fillColor.A > 0)
						{
				canvas.State.ColorTint = fillColor;
							canvas.FillCircle(
								objPos.X + circlePos.X,
								objPos.Y + circlePos.Y,
					0.0f, 
					shape.Radius * objScale);
						}

			double outlineWidth = this.GetScreenConstantScale(canvas, this.shapeOutlineWidth);
			canvas.State.ColorTint = outlineColor;
			canvas.State.DepthOffset = (float)this.depthOffset - 0.1f;
			canvas.FillCircleSegment(
							objPos.X + circlePos.X,
							objPos.Y + circlePos.Y,
				0.0f, 
				shape.Radius * objScale,
				0.0f,
				MathD.RadAngle360,
				outlineWidth);
			canvas.State.DepthOffset = (float)this.depthOffset;
		}

		private void FillPolygon(Canvas canvas, Transform transform, Vector2D[] polygon, ColorRgba fillColor)
		{
			Vector3D objPos = transform.Pos;
			double objAngle = transform.Angle;
			double objScale = transform.Scale;

			canvas.State.ColorTint = fillColor;
			canvas.State.TransformAngle = (float)objAngle;
			canvas.State.TransformScale = new Vector2D(objScale, objScale);

			canvas.FillPolygon(polygon, objPos.X, objPos.Y, 0.0f);

			canvas.State.TransformAngle = 0.0f;
			canvas.State.TransformScale = Vector2D.One;
		}
		private void DrawPolygonOutline(Canvas canvas, Transform transform, Vector2D[] polygon, ColorRgba outlineColor, bool closedLoop)
		{
			Vector3D objPos = transform.Pos;
			double objAngle = transform.Angle;
			double objScale = transform.Scale;

			canvas.State.TransformAngle = (float)objAngle;
			canvas.State.TransformScale = new Vector2D(objScale, objScale);
			canvas.State.ColorTint = outlineColor;

			double outlineWidth = this.GetScreenConstantScale(canvas, this.shapeOutlineWidth);
			outlineWidth /= objScale;
			if (closedLoop)
				canvas.FillPolygonOutline(polygon, outlineWidth, objPos.X, objPos.Y, 0.0f);
			else
				canvas.FillThickLineStrip(polygon, outlineWidth, objPos.X, objPos.Y, 0.0f);

			canvas.State.TransformAngle = 0.0f;
			canvas.State.TransformScale = Vector2D.One;
		}
		private void DrawPolygonInternals(Canvas canvas, Transform transform, Vector2D[] hullVertices, IReadOnlyList<Vector2D[]> convexPolygons, ColorRgba outlineColor)
		{
			if (convexPolygons.Count <= 1) return;

			Vector3D objPos = transform.Pos;
			double objAngle = transform.Angle;
			double objScale = transform.Scale;

			Vector2D xDot;
			Vector2D yDot;
			MathD.GetTransformDotVec(objAngle, objScale, out xDot, out yDot);

			double dashPatternLength = this.GetScreenConstantScale(canvas, this.shapeOutlineWidth * 0.5f);

			// Generate a lookup of drawn vertex indices, so we can
			// avoid drawing the same edge twice. Every item is a combination
			// of two indices.
			HashSet<uint> drawnEdges = new HashSet<uint>();
			for (int i = 0; i < hullVertices.Length; i++)
			{
				int currentHullIndex = i;
				int nextHullIndex = (i + 1) % hullVertices.Length;
				uint edgeId = (currentHullIndex > nextHullIndex) ?
					((uint)currentHullIndex << 16) | (uint)nextHullIndex :
					((uint)nextHullIndex << 16) | (uint)currentHullIndex;
				drawnEdges.Add(edgeId);
			}

			canvas.State.ColorTint = outlineColor;

			foreach (Vector2D[] polygon in convexPolygons)
			{
				if (polygon.Length < 2) continue;

				int currentHullIndex;
				int nextHullIndex = VertexListIndex(hullVertices, polygon[0]);
				for (int i = 0; i < polygon.Length; i++)
				{
					int nextIndex = (i + 1) % polygon.Length;
					currentHullIndex = nextHullIndex;
					nextHullIndex = VertexListIndex(hullVertices, polygon[nextIndex]);

					// Filter out edges that have already been drawn
					if (currentHullIndex >= 0 && nextHullIndex >= 0)
					{
						uint edgeId = (currentHullIndex > nextHullIndex) ?
							((uint)currentHullIndex << 16) | (uint)nextHullIndex :
							((uint)nextHullIndex << 16) | (uint)currentHullIndex;
						if (!drawnEdges.Add(edgeId))
							continue;
					}

					Vector2D lineStart = new Vector2D(
						polygon[i].X, 
						polygon[i].Y);
					Vector2D lineEnd = new Vector2D(
						polygon[nextIndex].X, 
						polygon[nextIndex].Y);
					MathD.TransformDotVec(ref lineStart, ref xDot, ref yDot);
					MathD.TransformDotVec(ref lineEnd, ref xDot, ref yDot);

					canvas.DrawDashLine(
						objPos.X + lineStart.X, 
						objPos.Y + lineStart.Y, 
						0.0f, 
						objPos.X + lineEnd.X, 
						objPos.Y + lineEnd.Y,
						0.0f, 
						DashPattern.Dash, 
						1.0f / dashPatternLength);
				}
			}
		}

		private double GetScreenConstantScale(Canvas canvas, double baseScale)
		{
			return baseScale / MathD.Max(0.0001f, canvas.DrawDevice.GetScaleAtZ(0.0f));
		}

		private IEnumerable<RigidBody> QueryVisibleColliders()
		{
			var allColliders = Scene.Current.FindComponents<RigidBody>();
			return allColliders.Where(r => 
				r.Active && 
				!DesignTimeObjectData.Get(r.GameObj).IsHidden && 
				this.IsSphereInView(r.GameObj.Transform.Pos, r.BoundRadius));
		}
		private RigidBody QuerySelectedCollider()
		{
			return 
				DualityEditorApp.Selection.Components.OfType<RigidBody>().FirstOrDefault() ?? 
				DualityEditorApp.Selection.GameObjects.GetComponents<RigidBody>().FirstOrDefault();
		}

		private static int VertexListIndex(Vector2D[] vertices, Vector2D checkVertex)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				if (Math.Abs(vertices[i].X - checkVertex.X) < 0.001f &&
					Math.Abs(vertices[i].Y - checkVertex.Y) < 0.001f)
					return i;
	}

			return -1;
		}
	}
}
