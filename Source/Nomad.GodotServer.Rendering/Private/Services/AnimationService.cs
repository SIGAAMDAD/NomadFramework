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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Godot;
using Nomad.GodotServer.Rendering.Private.ValueObjects;

namespace Nomad.GodotServer.Rendering {
	/*
	===================================================================================

	AnimationService

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal sealed class AnimationService : IDisposable {
		private readonly struct AnimationBatchData() {
			public readonly float[][] FrameDurations = new float[ EntityService.MAX_ENTITIES ][];
			public readonly Rid[][] TextureRids = new Rid[ EntityService.MAX_ENTITIES ][];
			public readonly Rect2[][] TextureRegions = new Rect2[ EntityService.MAX_ENTITIES ][];
			public readonly bool[][] Loops = new bool[ EntityService.MAX_ENTITIES ][];
			public readonly int[][] FrameCounts = new int[ EntityService.MAX_ENTITIES ][];
		};
		private readonly struct HotBatchData() {
			public readonly int[] CurrentFrames = new int[ EntityService.MAX_ENTITIES ];
			public readonly float[] FrameTimers = new float[ EntityService.MAX_ENTITIES ];
			public readonly float[] SpeedScales = new float[ EntityService.MAX_ENTITIES ];
			public readonly bool[] Playing = new bool[ EntityService.MAX_ENTITIES ];
			public readonly bool[] Backwards = new bool[ EntityService.MAX_ENTITIES ];
		};

		private readonly HotBatchData _animationBatchData = new HotBatchData();
		private readonly AnimationBatchData _framesBatchData = new AnimationBatchData();
		private int _animationCount = 0;

		/*
		===============
		Dispose
		===============
		*/
		/// <summary>
		///
		/// </summary>
		public void Dispose() {
		}

		/*
		===============
		AllocAnimationData
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private unsafe AnimationDataDto AllocAnimationDto( int index ) {
			fixed ( int* currentFramePtr = &_animationBatchData.CurrentFrames[ index ] )
			fixed ( float* frameTimerPtr = &_animationBatchData.FrameTimers[ index ] )
			fixed ( float* speedScalePtr = &_animationBatchData.SpeedScales[ index ] )
			fixed ( bool* playingPtr = &_animationBatchData.Playing[ index ] )
			fixed ( bool* backwardsPtr = &_animationBatchData.Backwards[ index ] ) {
				return new AnimationDataDto(
					currentFramePtr,
					frameTimerPtr,
					speedScalePtr,
					playingPtr,
					backwardsPtr
				);
			}
		}

		/*
		===============
		GetTextureRegion
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="texture"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		private Rect2 GetTextureRegion( Texture2D texture )
			=> texture is AtlasTexture atlas ? atlas.Region : texture.GetImage().GetUsedRect();

		/*
		===============
		AllocFrames
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="animatedSprite"></param>
		/// <returns></returns>
		private unsafe ImmutableDictionary<string, AnimationFramesData> AllocFrames( AnimatedSprite2D animatedSprite, int index ) {
			var spriteFrames = animatedSprite.SpriteFrames;
			var animationNames = spriteFrames.GetAnimationNames();

			var animations = new Dictionary<string, AnimationFramesData>( animationNames.Length );
			int[] frameCounts = new int[ animationNames.Length ];
			bool[] loops = new bool[ animationNames.Length ];
			for ( int i = 0; i < animationNames.Length; i++ ) {
				var animationName = animationNames[ i ];

				int frameCount = spriteFrames.GetFrameCount( animationName );
				frameCounts[ i ] = frameCount;

				loops[ i ] = spriteFrames.GetAnimationLoop( animationName );

				var rids = new Rid[ frameCount ];
				var regions = new Rect2[ frameCount ];
				var durations = new float[ frameCount ];
				for ( int f = 0; f < frameCount; f++ ) {
					var texture = spriteFrames.GetFrameTexture( animationName, f );
					rids[ i ] = texture.GetRid();
					regions[ i ] = GetTextureRegion( texture );
					durations[ i ] = spriteFrames.GetFrameDuration( animationName, f );
				}

				fixed ( float* durationsPtr = durations )
				fixed ( Rid* ridsPtr = rids )
				fixed ( Rect2* regionsPtr = regions )
				fixed ( bool* loopPtr = &loops[ i ] )
				fixed ( int* frameCountPtr = &frameCounts[ i ] ) {
					animations[ animationNames[ i ] ] = new AnimationFramesData(
						durationsPtr,
						ridsPtr,
						regionsPtr,
						loopPtr,
						frameCountPtr
					);
				}

				_framesBatchData.Loops[ index ] = loops;
				_framesBatchData.FrameDurations[ index ] = durations;
				_framesBatchData.TextureRids[ index ] = rids;
				_framesBatchData.TextureRegions[ index ] = regions;
				_framesBatchData.FrameCounts[ index ] = frameCounts;
			}
			return animations.ToImmutableDictionary();
		}

		/*
		===============
		CreateAnimator
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="entityDto"></param>
		/// <param name="index"></param>
		/// <param name="animatedSprite"></param>
		/// <returns></returns>
		public RenderAnimator CreateAnimator( EntityDataDto entityDto, int index, AnimatedSprite2D animatedSprite ) {
			return null;
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
		public unsafe void Update( float delta ) {
			int count = _animationCount;

			fixed ( float* frameTimersPtr = _animationBatchData.FrameTimers )
			fixed ( int* currentFramePtr = _animationBatchData.CurrentFrames )
			fixed ( bool* backwardsPtr = _animationBatchData.Backwards )
			fixed ( bool* playingPtr = _animationBatchData.Playing )
			fixed ( float* speedScalesPtr = _animationBatchData.SpeedScales ) {
				for ( int i = 0; i < count; i++ ) {
					UpdateFrameData(
						&currentFramePtr[ i ],
						&frameTimersPtr[ i ],
						&playingPtr[ i ],
						backwardsPtr[ i ],
						speedScalesPtr[ i ],
						i,
						delta
					);
//					DrawAnimation( i );
				}
			}
		}

		/*
		===============
		DrawAnimation
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="rid"></param>
		/// <param name="currentFrame"></param>
		/// <param name="index"></param>
		private void DrawAnimation( Rid rid, int currentFrame, int index ) {
			var textureRid = _framesBatchData.TextureRids[ index ][ currentFrame ];
			var textureRegion = _framesBatchData.TextureRegions[ index ][ currentFrame ];

			if ( textureRid.IsValid ) {
				var frameSize = textureRegion.Size;

				Rect2 destRect = new Rect2(
					-frameSize / 2,
					frameSize
				);

				RenderingServer.CanvasItemAddTextureRectRegion( rid, destRect, textureRid, textureRegion );
			}
		}

		/*
		===============
		UpdateFrameData
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="currentFrame"></param>
		/// <param name="frameTimer"></param>
		/// <param name="playing"></param>
		/// <param name="backwards"></param>
		/// <param name="speedScale"></param>
		/// <param name="index"></param>
		/// <param name="delta"></param>
		private unsafe void UpdateFrameData( int* currentFrame, float *frameTimer, bool* playing, bool backwards, float speedScale, int index, float delta ) {
			fixed ( bool* loopsPtr = _framesBatchData.Loops[ index ] )
			fixed ( int* frameCountPtr = _framesBatchData.FrameCounts[ index ] )
			fixed ( float* frameDurationsPtr = _framesBatchData.FrameDurations[ index ] ) {
				*frameTimer += delta *speedScale;
				while ( *frameTimer >= frameDurationsPtr[ *currentFrame ] ) {
					*frameTimer -= frameDurationsPtr[ *currentFrame ];
					if ( backwards ) {
						( *currentFrame )--;
					} else {
						( *currentFrame )++;
					}
					if ( *currentFrame >= frameCountPtr[ index ] ) {
						if ( loopsPtr[ index ] ) {
							*currentFrame = 0;
						} else {
							*currentFrame = frameCountPtr[ index ] - 1;
							*playing = false;
							break;
						}
					}
				}
			}
		}
	};
};
