using System;
using System.Collections.Generic;
using System.Linq;

using Duality;
using Duality.Drawing;
using Duality.Resources;
using Duality.Components;
using Duality.Editor;
using Duality.Plugins.Tilemaps.Properties;

namespace Duality.Plugins.Tilemaps
{
	/// <summary>
	/// A helper class that provides algorithms for determining the visible tile area for a given <see cref="IDrawDevice"/>.
	/// </summary>
	public static class TilemapCulling
	{
		/// <summary>
		/// Input parameters for a <see cref="Tilemap"/> culling operation, detailing world transform and
		/// tile extent.
		/// </summary>
		public struct TileInput
		{
			/// <summary>
			/// World space position of the <see cref="Tilemap"/>.
			/// </summary>
			public Vector3D TilemapPos;
			/// <summary>
			/// World space scale of the <see cref="Tilemap"/>.
			/// </summary>
			public double TilemapScale;
			/// <summary>
			/// World space rotation of the <see cref="Tilemap"/>.
			/// </summary>
			public double TilemapAngle;
			/// <summary>
			/// The size of a single tile in the <see cref="Tilemap"/>.
			/// </summary>
			public Vector2D TileSize;
			/// <summary>
			/// The total number of tiles in the <see cref="Tilemap"/>.
			/// </summary>
			public Point2 TileCount;
		}
		/// <summary>
		/// The end result of a <see cref="Tilemap"/> culling operation, specifying the rectangular area
		/// of the <see cref="Tilemap"/> that is to be rendered, as well as view space transform data
		/// which can be used for doing so.
		/// </summary>
		public struct TileOutput
		{
			/// <summary>
			/// The top left origin of the visible <see cref="Tilemap"/> rect, in tile coordinates.
			/// </summary>
			public Point2 VisibleTileStart;
			/// <summary>
			/// The number of visible tiles to render, starting from <see cref="VisibleTileStart"/>.
			/// </summary>
			public Point2 VisibleTileCount;
			/// <summary>
			/// The world space rendering origin of the visible tile rect.
			/// </summary>
			public Vector3D RenderOriginWorld;
			/// <summary>
			/// The world space x axis of the rendered <see cref="Tilemap"/>, taking rotation and scale into account.
			/// </summary>
			public Vector2D XAxisWorld;
			/// <summary>
			/// The world space y axis of the rendered <see cref="Tilemap"/>, taking rotation and scale into account.
			/// </summary>
			public Vector2D YAxisWorld;
		}

		/// <summary>
		/// An empty culling result that indicates no rendering is necessary at all.
		/// </summary>
		public static readonly TileOutput EmptyOutput = new TileOutput();

		/// <summary>
		/// Determines the rectangular tile area that is visible to the specified <see cref="IDrawDevice"/>.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="input"></param>
		/// <returns></returns>
		public static TileOutput GetVisibleTileRect(IDrawDevice device, TileInput input)
		{
			TileOutput output;

			// Determine the view space transform of the tilemap
			double cameraScaleAtObj = device.GetScaleAtZ((float)input.TilemapPos.Z);
			Vector3D viewCenterWorldPos = device.GetWorldPos(new Vector3D(device.TargetSize * 0.5f, input.TilemapPos.Z));

			// Early-out, if so small that it might break the math behind rendering a single tile.
			if (cameraScaleAtObj <= 0.000000001f) return EmptyOutput;

			// Determine transformed X and Y axis in world space
			output.XAxisWorld = Vector2D.UnitX;
			output.YAxisWorld = Vector2D.UnitY;
			MathD.TransformCoord(ref output.XAxisWorld.X, ref output.XAxisWorld.Y, input.TilemapAngle, input.TilemapScale);
			MathD.TransformCoord(ref output.YAxisWorld.X, ref output.YAxisWorld.Y, input.TilemapAngle, input.TilemapScale);

			// Determine which tile is in the center of view space.
			Point2 viewCenterTile = Point2.Zero;
			{
				Vector2D localViewCenter = (viewCenterWorldPos - input.TilemapPos).Xy;
				localViewCenter = new Vector2D(
					Vector2D.Dot(localViewCenter, output.XAxisWorld.Normalized),
					Vector2D.Dot(localViewCenter, output.YAxisWorld.Normalized)) / input.TilemapScale;
				viewCenterTile = new Point2(
					(int)MathD.Floor(localViewCenter.X / input.TileSize.X),
					(int)MathD.Floor(localViewCenter.Y / input.TileSize.Y));
			}

			// Determine the edge length of a square that is big enough to enclose the world space rect of the Camera view
			double visualAngle = input.TilemapAngle - device.ViewerAngle;
			Vector2D visualBounds = new Vector2D(
				device.TargetSize.Y * MathD.Abs(MathD.Sin(visualAngle)) + device.TargetSize.X * MathD.Abs(MathD.Cos(visualAngle)),
				device.TargetSize.X * MathD.Abs(MathD.Sin(visualAngle)) + device.TargetSize.Y * MathD.Abs(MathD.Cos(visualAngle)));
			Vector2D localVisualBounds = visualBounds / cameraScaleAtObj;
			Point2 targetVisibleTileCount = new Point2(
				3 + (int)MathD.Ceiling(localVisualBounds.X / (MathD.Min(input.TileSize.X, input.TileSize.Y) * input.TilemapScale)), 
				3 + (int)MathD.Ceiling(localVisualBounds.Y / (MathD.Min(input.TileSize.X, input.TileSize.Y) * input.TilemapScale)));

			// Determine the tile indices (xy) that are visible within that rect
			output.VisibleTileStart = new Point2(
				MathD.Max(viewCenterTile.X - targetVisibleTileCount.X / 2, 0),
				MathD.Max(viewCenterTile.Y - targetVisibleTileCount.Y / 2, 0));
			Point2 tileGridEndPos = new Point2(
				MathD.Min(viewCenterTile.X + targetVisibleTileCount.X / 2, input.TileCount.X),
				MathD.Min(viewCenterTile.Y + targetVisibleTileCount.Y / 2, input.TileCount.Y));
			output.VisibleTileCount = new Point2(
				MathD.Clamp(tileGridEndPos.X - output.VisibleTileStart.X, 0, input.TileCount.X),
				MathD.Clamp(tileGridEndPos.Y - output.VisibleTileStart.Y, 0, input.TileCount.Y));

			// Determine start position for rendering
			output.RenderOriginWorld = input.TilemapPos + new Vector3D(
				output.VisibleTileStart.X * output.XAxisWorld * input.TileSize.X + 
				output.VisibleTileStart.Y * output.YAxisWorld * input.TileSize.Y);

			return output;
		}
	}
}
