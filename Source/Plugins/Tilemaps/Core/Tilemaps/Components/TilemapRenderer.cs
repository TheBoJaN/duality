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
	/// Renders a <see cref="Tilemap"/> that either belongs to the same <see cref="GameObject"/>
	/// or is referenced by <see cref="ExternalTilemap"/>.
	/// </summary>
	[EditorHintCategory(TilemapsResNames.CategoryTilemaps)]
	[EditorHintImage(TilemapsResNames.ImageTilemapRenderer)]
	public class TilemapRenderer : Renderer, ICmpTilemapRenderer
	{
		private Alignment           origin          = Alignment.Center;
		private Tilemap             externalTilemap = null;
		private ColorRgba           colorTint       = ColorRgba.White;
		private double               offset          = 0.0f;
		private int                 tileDepthOffset = 0;
		private double               tileDepthScale  = 0.01f;
		private TileDepthOffsetMode tileDepthMode   = TileDepthOffsetMode.Flat;

		[DontSerialize] private Tilemap localTilemap = null;
		[DontSerialize] private RawList<VertexC1P3T2> vertices = null;

		
		/// <summary>
		/// [GET] The base depth offset for generating tile vertices in this <see cref="Tilemap"/>, calculated based
		/// on <see cref="DepthOffset"/> and <see cref="TileDepthOffset"/>.
		/// For an absolute offset that is not based on per-tile depth, see <see cref="DepthOffset"/>.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public double BaseDepthOffset
		{
			get
			{
				if (this.tileDepthMode == TileDepthOffsetMode.Flat)
					return this.offset;

				Tilemap tilemap = this.ActiveTilemap;
				Tileset tileset = tilemap != null ? tilemap.Tileset.Res : null;
				Vector2D tileSize = tileset != null ? tileset.TileSize : Tileset.DefaultTileSize;
				double depthPerTile = -tileSize.Y * this.GameObj.Transform.Scale * this.tileDepthScale;

				return this.offset + this.tileDepthOffset * depthPerTile;
			}
		}
		/// <summary>
		/// [GET / SET] An absolute depth offset that is added to the entire <see cref="Tilemap"/> as whole.
		/// For a relative offset based on per-tile depth, see <see cref="BaseDepthOffset"/>.
		/// </summary>
		public double DepthOffset
		{
			get { return this.offset; }
			set { this.offset = value; }
		}
		/// <summary>
		/// [GET / SET] An offset measured in tiles that is assumed in the depth offset generation.
		/// With an offset of one, each tile will be rendered with the base offset of "one tile higher up".
		/// 
		/// This can be used as a quick way to layer different tilemaps on each other where each layer
		/// represents a certain world space height. The same effect can be achieved by carefully adjusting 
		/// the <see cref="DepthOffset"/>.
		/// </summary>
		public int TileDepthOffset
		{
			get { return this.tileDepthOffset; }
			set { this.tileDepthOffset = value; }
		}
		/// <summary>
		/// [GET / SET] The depth offset scale that is used to determine how much depth each 
		/// tile / pixel / unit adds when using non-flat depth offset generation.
		/// </summary>
		public double TileDepthScale
		{
			get { return this.tileDepthScale; }
			set { this.tileDepthScale = value; }
		}
		/// <summary>
		/// [GET / SET] Specifies the way in which depth offsets are generated per-tile.
		/// </summary>
		public TileDepthOffsetMode TileDepthMode
		{
			get { return this.tileDepthMode; }
			set { this.tileDepthMode = value; }
		}
		/// <summary>
		/// [GET / SET] A color by which the rendered <see cref="Tilemap"/> is tinted.
		/// </summary>
		public ColorRgba ColorTint
		{
			get { return this.colorTint; }
			set { this.colorTint = value; }
		}
		/// <summary>
		/// [GET / SET] The origin of the rendered <see cref="Tilemap"/> as a whole, relative to the position of its <see cref="GameObject"/>.
		/// </summary>
		public Alignment Origin
		{
			get { return this.origin; }
			set { this.origin = value; }
		}
		/// <summary>
		/// [GET / SET] The <see cref="Tilemap"/> that should be rendered. 
		/// If this is null, the local <see cref="Tilemap"/> on the same <see cref="GameObject"/> is used.
		/// </summary>
		public Tilemap ExternalTilemap
		{
			get { return this.externalTilemap; }
			set { this.externalTilemap = value; }
		}
		/// <summary>
		/// [GET] A reference to the <see cref="Tilemap"/> that is currently rendered by this <see cref="TilemapRenderer"/>.
		/// </summary>
		[EditorHintFlags(MemberFlags.Invisible)]
		public Tilemap ActiveTilemap
		{
			get 
			{
				if (this.externalTilemap != null)
				{
					return this.externalTilemap;
				}
				else
				{
					if (this.localTilemap == null || this.localTilemap.GameObj != this.GameObj)
						this.localTilemap = this.GameObj.GetComponent<Tilemap>();
					return this.localTilemap;
				}
			}
		}
		/// <summary>
		/// [GET] The rectangular region that is occupied by the rendered <see cref="Tilemap"/>, in local / object space.
		/// </summary>
		public RectD LocalTilemapRect
		{
			get
			{
				Tilemap tilemap = this.ActiveTilemap;
				Tileset tileset = tilemap != null ? tilemap.Tileset.Res : null;
				Point2 tileCount = tilemap != null ? tilemap.Size : Point2.Zero;
				Vector2D tileSize = tileset != null ? tileset.TileSize : Tileset.DefaultTileSize;
				return RectD.Align(this.origin, 0, 0, tileCount.X * tileSize.X, tileCount.Y * tileSize.Y);
			}
		}
		/// <summary>
		/// [GET] Returns the size of a single tile in local / object space.
		/// </summary>
		public Vector2D LocalTileSize
		{
			get
			{
				Tilemap tilemap = this.ActiveTilemap;
				Tileset tileset = tilemap != null ? tilemap.Tileset.Res : null;
				return tileset != null ? tileset.TileSize : Tileset.DefaultTileSize;
			}
		}
		public override double BoundRadius
		{
			get
			{
				Transform transform = this.GameObj.Transform;
				RectD tilemapRect = this.LocalTilemapRect;
				return tilemapRect.BoundingRadius * transform.Scale;
			}
		}


		/// <summary>
		/// Given the specified coordinate in local / object space, this method returns the
		/// tile index that is located there.
		/// </summary>
		/// <param name="localPos"></param>
		/// <param name="pickMode">Specifies the desired behavior when attempting to get a tile outside the rendered area.</param>
		/// <returns></returns>
		public Point2 GetTileAtLocalPos(Vector2D localPos, TilePickMode pickMode)
		{
			// Early-out, if the specified local position is not within the tilemap rect
			RectD localRect = this.LocalTilemapRect;
			if (pickMode == TilePickMode.Reject && !localRect.Contains(localPos))
				return new Point2(-1, -1);

			Tilemap tilemap = this.ActiveTilemap;
			Tileset tileset = tilemap != null ? tilemap.Tileset.Res : null;
			Point2 tileCount = tilemap != null ? tilemap.Size : Point2.Zero;
			Vector2D tileSize = tileset != null ? tileset.TileSize : Tileset.DefaultTileSize;

			// Determine the tile index at the specified local position
			Point2 tileIndex = new Point2(
				(int)MathD.Floor((localPos.X - localRect.X) / tileSize.X),
				(int)MathD.Floor((localPos.Y - localRect.Y) / tileSize.Y));

			// Clamp or reject the tile index when required
			if (pickMode != TilePickMode.Free)
			{
				if (tileCount.X <= 0 || tileCount.Y <= 0)
					return new Point2(-1, -1);

				tileIndex = new Point2(
					MathD.Clamp(tileIndex.X, 0, tileCount.X - 1),
					MathD.Clamp(tileIndex.Y, 0, tileCount.Y - 1));
			}

			return tileIndex;
		}
		/// <summary>
		/// Gets the local position of the specified tile at the upper left corner.
		/// The function does not check if the point is a valid tile position.
		/// </summary>
		/// <param name="tilePos">The index of the tile of which to calculate the local position.</param>
		/// <returns></returns>
		public Vector2D GetLocalPosAtTile(Point2 tilePos)
		{
			RectD localRect = this.LocalTilemapRect;
			Tilemap tilemap = this.ActiveTilemap;
			Point2 tileCount = tilemap != null ? tilemap.Size : new Point2(1, 1);

			return new Vector2D(
				MathD.Lerp(localRect.LeftX, localRect.RightX, (double)tilePos.X / tileCount.X),
				MathD.Lerp(localRect.TopY, localRect.BottomY, (double)tilePos.Y / tileCount.Y));
		}
		/// <summary>
		/// Determines the generated depth offset for the tile at the specified tile coordinates.
		/// This also inclues the renderers overall offset as specified in <see cref="DepthOffset"/>,
		/// but ignores all actual per-tile and tileset depth offsets. The specified tile position
		/// is considered virtual and does not have to be within the valid tile position range.
		/// </summary>
		/// <param name="tilePos">The index of the tile of which to calculate the depth offset.</param>
		/// <returns></returns>
		public double GetTileDepthOffsetAt(Point2 tilePos)
		{
			if (this.tileDepthMode == TileDepthOffsetMode.Flat)
				return this.offset;

			Tilemap tilemap = this.ActiveTilemap;
			Tileset tileset = tilemap != null ? tilemap.Tileset.Res : null;
			Point2 tileCount = tilemap != null ? tilemap.Size : new Point2(1, 1);
			Vector2D tileSize = tileset != null ? tileset.TileSize : Tileset.DefaultTileSize;

			double depthPerTile = -tileSize.Y * this.GameObj.Transform.Scale * this.tileDepthScale;
			double originDepthOffset = RectD.Align(this.origin, 0, 0, 0, tileCount.Y * depthPerTile).Y;

			if (this.tileDepthMode == TileDepthOffsetMode.World)
				originDepthOffset += (this.GameObj.Transform.Pos.Y / (double)tileSize.Y) * depthPerTile;

			return this.offset + this.tileDepthOffset * depthPerTile + originDepthOffset + tilePos.Y * depthPerTile;
		}

		public override void Draw(IDrawDevice device)
		{
			// Determine basic working data
			Tilemap tilemap = this.ActiveTilemap;
			Tileset tileset = tilemap != null ? tilemap.Tileset.Res : null;
			Point2 tileCount = tilemap != null ? tilemap.Size : new Point2(1, 1);
			Vector2D tileSize = tileset != null ? tileset.TileSize : Tileset.DefaultTileSize;

			// Early-out, if insufficient
			if (tilemap == null) return;
			if (tileset == null) return;

			// Determine the total size and origin of the rendered Tilemap
			Vector2D renderTotalSize = tileCount * tileSize;
			Vector2D renderOrigin = Vector2D.Zero;
			this.origin.ApplyTo(ref renderOrigin, ref renderTotalSize);
			MathD.TransformCoord(ref renderOrigin.X, ref renderOrigin.Y, this.GameObj.Transform.Angle, this.GameObj.Transform.Scale);

			// Determine Tile visibility
			TilemapCulling.TileInput cullingIn = new TilemapCulling.TileInput
			{
				// Remember: All these transform values are in world space
				TilemapPos = this.GameObj.Transform.Pos + new Vector3D(renderOrigin),
				TilemapScale = this.GameObj.Transform.Scale,
				TilemapAngle = this.GameObj.Transform.Angle,
				TileCount = tileCount,
				TileSize = tileSize
			};
			TilemapCulling.TileOutput cullingOut = TilemapCulling.GetVisibleTileRect(device, cullingIn);
			int renderedTileCount = cullingOut.VisibleTileCount.X * cullingOut.VisibleTileCount.Y;

			// Determine rendering parameters
			Material material = (tileset != null ? tileset.RenderMaterial : null) ?? Material.Checkerboard.Res;
			ColorRgba mainColor = this.colorTint;

			// Reserve the required space for vertex data in our locally cached buffer
			if (this.vertices == null) this.vertices = new RawList<VertexC1P3T2>();
			this.vertices.Count = renderedTileCount * 4;
			VertexC1P3T2[] vertexData = this.vertices.Data;
			
			// Determine and adjust data for Z offset generation
			double depthPerTile = -cullingIn.TileSize.Y * cullingIn.TilemapScale * this.tileDepthScale;

			if (this.tileDepthMode == TileDepthOffsetMode.Flat)
				depthPerTile = 0.0f;

			double originDepthOffset = RectD.Align(this.origin, 0, 0, 0, tileCount.Y * depthPerTile).Y;
			if (this.tileDepthMode == TileDepthOffsetMode.World)
				originDepthOffset += (this.GameObj.Transform.Pos.Y / (double)tileSize.Y) * depthPerTile;

			double renderBaseOffset = this.offset + this.tileDepthOffset * depthPerTile + originDepthOffset;

			// Prepare vertex generation data
			Vector2D tileXStep = cullingOut.XAxisWorld * cullingIn.TileSize.X;
			Vector2D tileYStep = cullingOut.YAxisWorld * cullingIn.TileSize.Y;
			Vector3D renderPos = cullingOut.RenderOriginWorld;
			double renderOffset = renderBaseOffset;
			Point2 tileGridPos = cullingOut.VisibleTileStart;

			// Prepare vertex data array for batch-submitting
			IReadOnlyGrid<Tile> tiles = tilemap.Tiles;
			TileInfo[] tileData = tileset.TileData.Data;
			int submittedTileCount = 0;
			int vertexBaseIndex = 0;
			for (int tileIndex = 0; tileIndex < renderedTileCount; tileIndex++)
			{
				Tile tile = tiles[tileGridPos.X, tileGridPos.Y];
				if (tile.Index < tileData.Length)
				{
					RectD uvRect = tileData[tile.Index].TexCoord0;
					bool visualEmpty = tileData[tile.Index].IsVisuallyEmpty;
					int tileBaseOffset = tileData[tile.Index].DepthOffset;
					double localDepthOffset = (tile.DepthOffset + tileBaseOffset) * depthPerTile;

					if (!visualEmpty)
					{
						vertexData[vertexBaseIndex + 0].Pos.X = (float)renderPos.X;
						vertexData[vertexBaseIndex + 0].Pos.Y = (float)renderPos.Y;
						vertexData[vertexBaseIndex + 0].Pos.Z = (float)renderPos.Z;
						vertexData[vertexBaseIndex + 0].DepthOffset = (float)(renderOffset + localDepthOffset);
						vertexData[vertexBaseIndex + 0].TexCoord.X = (float)uvRect.X;
						vertexData[vertexBaseIndex + 0].TexCoord.Y = (float)uvRect.Y;
						vertexData[vertexBaseIndex + 0].Color = mainColor;

						vertexData[vertexBaseIndex + 1].Pos.X = (float)(renderPos.X + tileYStep.X);
						vertexData[vertexBaseIndex + 1].Pos.Y = (float)(renderPos.Y + tileYStep.Y);
						vertexData[vertexBaseIndex + 1].Pos.Z = (float)renderPos.Z;
						vertexData[vertexBaseIndex + 1].DepthOffset = (float)(renderOffset + localDepthOffset + depthPerTile);
						vertexData[vertexBaseIndex + 1].TexCoord.X = (float)uvRect.X;
						vertexData[vertexBaseIndex + 1].TexCoord.Y = (float)(uvRect.Y + uvRect.H);
						vertexData[vertexBaseIndex + 1].Color = mainColor;

						vertexData[vertexBaseIndex + 2].Pos.X = (float)(renderPos.X + tileXStep.X + tileYStep.X);
						vertexData[vertexBaseIndex + 2].Pos.Y = (float)(renderPos.Y + tileXStep.Y + tileYStep.Y);
						vertexData[vertexBaseIndex + 2].Pos.Z = (float)renderPos.Z;
						vertexData[vertexBaseIndex + 2].DepthOffset = (float)(renderOffset + localDepthOffset + depthPerTile);
						vertexData[vertexBaseIndex + 2].TexCoord.X = (float)(uvRect.X + uvRect.W);
						vertexData[vertexBaseIndex + 2].TexCoord.Y = (float)(uvRect.Y + uvRect.H);
						vertexData[vertexBaseIndex + 2].Color = mainColor;
				
						vertexData[vertexBaseIndex + 3].Pos.X = (float)(renderPos.X + tileXStep.X);
						vertexData[vertexBaseIndex + 3].Pos.Y = (float)(renderPos.Y + tileXStep.Y);
						vertexData[vertexBaseIndex + 3].Pos.Z = (float)renderPos.Z;
						vertexData[vertexBaseIndex + 3].DepthOffset = (float)(renderOffset + localDepthOffset);
						vertexData[vertexBaseIndex + 3].TexCoord.X = (float)(uvRect.X + uvRect.W);
						vertexData[vertexBaseIndex + 3].TexCoord.Y = (float)uvRect.Y;
						vertexData[vertexBaseIndex + 3].Color = mainColor;

						bool vertical = tileData[tile.Index].IsVertical;
						if (vertical)
						{
							vertexData[vertexBaseIndex + 0].DepthOffset += (float)depthPerTile;
							vertexData[vertexBaseIndex + 3].DepthOffset += (float)depthPerTile;
						}

						submittedTileCount++;
						vertexBaseIndex += 4;
					}
				}

				tileGridPos.X++;
				renderPos.X += tileXStep.X;
				renderPos.Y += tileXStep.Y;
				if ((tileGridPos.X - cullingOut.VisibleTileStart.X) >= cullingOut.VisibleTileCount.X)
				{
					tileGridPos.X = cullingOut.VisibleTileStart.X;
					tileGridPos.Y++;
					renderPos = cullingOut.RenderOriginWorld;
					renderPos.X += tileYStep.X * (tileGridPos.Y - cullingOut.VisibleTileStart.Y);
					renderPos.Y += tileYStep.Y * (tileGridPos.Y - cullingOut.VisibleTileStart.Y);
					renderOffset = renderBaseOffset + tileGridPos.Y * depthPerTile;
				}
			}

			// Submit all the vertices as one draw batch
			device.AddVertices(
				material,
				VertexMode.Quads, 
				vertexData, 
				submittedTileCount * 4);

			Profile.AddToStat(@"Duality\Stats\Render\Tilemaps\NumTiles", renderedTileCount);
			Profile.AddToStat(@"Duality\Stats\Render\Tilemaps\NumVertices", submittedTileCount * 4);
		}
	}
}
