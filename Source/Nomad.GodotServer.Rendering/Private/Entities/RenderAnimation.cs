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
using System.Runtime.CompilerServices;
using Godot;
using Nomad.GodotServer.Rendering.Private.ValueObjects;

namespace Nomad.GodotServer.Rendering {
	/*
	===================================================================================

	RenderAnimator

	===================================================================================
	*/
	/// <summary>
	///
	/// </summary>

	internal unsafe sealed class RenderAnimator : RenderEntity {
		private readonly struct AnimationData( Rid* textureRids, Rect2* textureRegions, float* frameDurations, int frameCount, bool loop ) {
			public readonly Rid* TextureRids = textureRids;
			public readonly Rect2* TextureRegions = textureRegions;
			public readonly float* FrameDurations = frameDurations;
			public readonly int FrameCount = frameCount;
			public readonly bool Loop = loop;
		};

		private string _currentAnimation = String.Empty;
		private float *_frameTimer;
		private int *_currentFrame;
		private bool *_playing;
		private bool *_backwards;
		private float *_speedScale;

		private readonly int _animationCount = 0;
		private readonly ImmutableDictionary<string, AnimationData> _animationData;

		/*
		===============
		RenderAnimator
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="entityDto"></param>
		/// <param name="animationDto"></param>
		/// <param name="animatedSprite"></param>
		public RenderAnimator( EntityDataDto entityDto, AnimationDataDto animationDto, AnimatedSprite2D animatedSprite )
			: base( entityDto, animatedSprite )
		{
			_currentFrame = animationDto.CurrentFrame;
			_frameTimer = animationDto.FrameTimer;
			_speedScale = animationDto.SpeedScale;
			_playing = animationDto.Playing;

			var spriteFrames = animatedSprite.SpriteFrames;
			var animationNames = spriteFrames.GetAnimationNames();

			var animations = new Dictionary<string, AnimationData>( animationNames.Length );
			for ( int i = 0; i < animationNames.Length; i++ ) {
				var animationName = animationNames[ i ];
				int frameCount = spriteFrames.GetFrameCount( animationName );

				var rids = new Rid[ frameCount ];
				var regions = new Rect2[ frameCount ];
				var durations = new float[ frameCount ];
				for ( int f = 0; f < frameCount; f++ ) {
					var texture = spriteFrames.GetFrameTexture( animationName, f );
					rids[ i ] = texture.GetRid();
					regions[ i ] = GetTextureRegion( texture );
					durations[ i ] = spriteFrames.GetFrameDuration( animationName, f );
				}

				fixed ( Rid* ridPtr = rids )
				fixed ( Rect2* regionPtr = regions )
				fixed ( float* durationsPtr = durations ) {
					animations[ animationNames[ i ] ] = new AnimationData(
						ridPtr,
						regionPtr,
						durationsPtr,
						frameCount,
						spriteFrames.GetAnimationLoop( animationName )
					);
				}
			}
			_animationData = animations.ToImmutableDictionary();
		}

		/*
		===============
		Play
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="animationName"></param>
		/// <param name="speedScale"></param>
		public void Play( string animationName = "", float speedScale = 1.0f ) {

		}

		private float GetFrameProgress() {
			if ( _currentAnimation.Length == 0 ) {
				return 0.0f;
			}

			var animation = _animationData[ _currentAnimation ];
			return *_frameTimer / animation.FrameDurations[ *_currentFrame ];
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
			if ( _currentAnimation.Length == 0 ) {
				return;
			}

			var animation = _animationData[ _currentAnimation ];

			float frameTimer = *_frameTimer;
			int currentFrame = *_currentFrame;
			frameTimer += delta * *_speedScale;
			while ( frameTimer >= animation.FrameDurations[ currentFrame ] ) {
				frameTimer -= animation.FrameDurations[ currentFrame ];

				if ( *_backwards ) {
					currentFrame--;
					if ( currentFrame < 0 ) {
						if ( animation.Loop ) {

						}
					}
				}
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
	};
};
