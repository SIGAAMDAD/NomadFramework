/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using Godot;
using Nomad.GodotServer.Rendering.Interfaces;
using Nomad.GodotServer.Rendering.Private.ValueObjects;

namespace Nomad.GodotServer.Rendering {
	/*
	===================================================================================

	EntityService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class EntityService : IEntitySystemService {
		public const int MAX_ENTITIES = 2048;

		private readonly struct HotBatchData() {
			public readonly Color[] Colors = new Color[ MAX_ENTITIES ];
			public readonly Rid[] Rids = new Rid[ MAX_ENTITIES ];
			public readonly Vector2[] Positions = new Vector2[ MAX_ENTITIES ];
			public readonly Vector2[] Scales = new Vector2[ MAX_ENTITIES ];
			public readonly float[] Rotations = new float[ MAX_ENTITIES ];
			public readonly bool[] Visibilities = new bool[ MAX_ENTITIES ];
		};
		private readonly struct ColdBatchData() {
			public readonly uint[] VisibilityLayers = new uint[ MAX_ENTITIES ];
			public readonly int[] ZIndexes = new int[ MAX_ENTITIES ];
			public readonly int[] LightMasks = new int[ MAX_ENTITIES ];
		};

		private readonly int[] _freeIndices = new int[ MAX_ENTITIES ];
		private int _freeIndexCount = 0;
		private int _entityCount;

		private readonly HotBatchData _hotBatchData = new HotBatchData();
		private readonly ColdBatchData _coldBatchData = new ColdBatchData();
		private readonly AnimationService _animationService = new AnimationService();

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
			for ( int i = 0; i < _entityCount; i++ ) {
				if ( _hotBatchData.Rids[ i ].IsValid ) {
					RenderingServer.FreeRid( _hotBatchData.Rids[ i ] );
				}
			}
		}

		/*
		===============
		AllocEntityIndex
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		private unsafe EntityDataDto? AllocEntityIndex( out int index ) {
			index = -1;
			if ( _freeIndexCount > 0 ) {
				index = _freeIndices[ --_freeIndexCount ];
			} else if ( _freeIndexCount < MAX_ENTITIES ) {
				index = _entityCount++;
			} else {
				return null;
			}

			fixed ( Vector2* positionPtr = &_hotBatchData.Positions[ index ] )
			fixed ( Vector2* scalePtr = &_hotBatchData.Scales[ index ] )
			fixed ( float* rotationPtr = &_hotBatchData.Rotations[ index ] )
			fixed ( bool* visiblePtr = &_hotBatchData.Visibilities[ index ] )
			fixed ( Color* modulatePtr = &_hotBatchData.Colors[ index ] )
			fixed ( uint* visibilityLayerPtr = &_coldBatchData.VisibilityLayers[ index ] )
			fixed ( int* zindexPtr = &_coldBatchData.ZIndexes[ index ] )
			fixed ( int* lightmaskPtr = &_coldBatchData.LightMasks[ index ] )
			fixed ( Rid* ridPtr = &_hotBatchData.Rids[ index ] ) {
				return new EntityDataDto(
					modulatePtr,
					ridPtr,
					zindexPtr,
					lightmaskPtr,
					visibilityLayerPtr,
					positionPtr,
					scalePtr,
					rotationPtr,
					visiblePtr
				);
			}
		}

		/*
		===============
		CreateEntity
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="canvasItem"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public IRenderEntity? CreateEntity( CanvasItem canvasItem ) {
			switch ( canvasItem ) {
				case AnimatedSprite2D animatedSprite: {
						return new RenderAnimator( animatedSprite );
					}
				case Sprite2D sprite: {
						var entityDto = AllocEntityIndex( out _ );
						if ( !entityDto.HasValue ) {
							return null;
						}
						return new RenderSprite( entityDto.Value, sprite );
					}
			}
			throw new InvalidOperationException( "Invalid CanvasItem type!" );
		}

		/*
		===============
		CreateAnimator
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="canvasItem"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public IAnimationEntity? CreateAnimator( CanvasItem canvasItem ) {
			if ( canvasItem is AnimatedSprite2D animatedSprite ) {
				return new RenderAnimator( animatedSprite );
			}
			throw new InvalidCastException();
		}

		/*
		===============
		Update
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="delta"></param>
		public void Update( float delta ) {
			int count = _entityCount;
			unsafe {
				fixed ( Rid* ridPtr = _hotBatchData.Rids )
				fixed ( Vector2* positionPtr = _hotBatchData.Positions )
				fixed ( Vector2* scalePtr = _hotBatchData.Scales )
				fixed ( float* rotationPtr = _hotBatchData.Rotations )
				fixed ( bool* visiblePtr = _hotBatchData.Visibilities ) {
					for ( int i = 0; i < count; i++ ) {
						RenderingServer.CanvasItemClear( ridPtr[ i ] );
						RenderingServer.CanvasItemSetTransform( ridPtr[ i ], new Transform2D( rotationPtr[ i ], scalePtr[ i ], 0.0f, positionPtr[ i ] ) );
					}
				}
				_animationService.Update( delta );
			}
		}
	};
};
