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
		private float _frameTimer = 0.0f;
		private int _currentFrame = 0;
		private bool _playing = false;
		private bool _backwards = false;
		private float _speedScale = 0.0f;

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
		/// <param name="animatedSprite"></param>
		public RenderAnimator( AnimatedSprite2D animatedSprite )
			: base( animatedSprite )
		{
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
		/// <param name="backwards"></param>
		public void Play( string animationName = "", float speedScale = 1.0f, bool backwards = false ) {
			if ( animationName.Length == 0 || animationName == _currentAnimation ) {
				return;
			}
			if ( !_animationData.TryGetValue( animationName, out var animation ) ) {
				return;
			}
			_currentAnimation = animationName;
			_currentFrame = backwards ? animation.FrameCount - 1 : 0;
			_frameTimer = 0.0f;
			_speedScale = speedScale;

			UpdateCurrentFrameData( in animation );
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
		public override void Update( float delta ) {
			if ( _currentAnimation.Length == 0 || !_visible ) {
				return;
			}
			base.Update( delta );

			var animation = _animationData[ _currentAnimation ];

			_frameTimer += delta * _speedScale;
			while ( _frameTimer >= animation.FrameDurations[ _currentFrame ] ) {
				_frameTimer -= animation.FrameDurations[ _currentFrame ];

				if ( _backwards ) {
					_currentFrame--;
				} else {
					_currentFrame++;
				}
				if ( _currentFrame >= animation.FrameCount ) {
					if ( animation.Loop ) {
						_currentFrame = 0;
					} else {
						_currentFrame = animation.FrameCount - 1;
						_playing = false;
						break;
					}
				}

				UpdateCurrentFrameData( in animation );
			}
		}

		/*
		===============
		UpdateCurrentFrameData
		===============
		*/
		/// <summary>
		///
		/// </summary>
		/// <param name="animation"></param>
		private void UpdateCurrentFrameData( in AnimationData animation ) {
			var textureRid = animation.TextureRids[ _currentFrame ];
			var textureRegion = animation.TextureRegions[ _currentFrame ];

			if ( textureRid.IsValid ) {
				var frameSize = textureRegion.Size;

				Rect2 destRect = new Rect2(
					-frameSize / 2,
					frameSize
				);

				RenderingServer.CanvasItemAddTextureRectRegion( _canvasRid, destRect, textureRid, textureRegion );
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
