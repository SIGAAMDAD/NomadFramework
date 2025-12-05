/*
===========================================================================
The Nomad AGPL Source Code
Copyright (C) 2025 Noah Van Til

The Nomad Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Nomad Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with The Nomad Source Code.  If not, see <http://www.gnu.org/licenses/>.

If you have questions concerning this license or the applicable additional
terms, you may contact me via email at nyvantil@gmail.com.
===========================================================================
*/

using Godot;
using NomadCore.Abstractions.Services;
using NomadCore.Interfaces.EntitySystem;
using NomadCore.Systems.EntitySystem.Common;
using NomadCore.Systems.EntitySystem.Common.Events;
using NomadCore.Systems.EntitySystem.Common.Models.Components;
using System;
using System.Collections.Generic;

namespace NomadCore.Systems.EntitySystem.Infrastructure.Rendering {
	/*
	===================================================================================
	
	AnimationEntity
	
	===================================================================================
	*/
	/// <summary>
	/// 
	/// </summary>
	
	internal sealed class AnimationEntity : ServerRenderEntity {
		private readonly struct FrameData( Texture2D texture, float duration ) {
			public readonly Texture2D Texture = texture;
			public readonly float Duration = duration;
		};
		private readonly struct AnimationData( bool loop, float speed, int frameCount, FrameData[] frames ) {
			public readonly FrameData[] Frames = frames;
			public readonly int FrameCount = frameCount;

			public readonly float Speed = speed;

			public readonly bool Loop = loop;
		};

		public bool Playing {
			get => _playing;
			set => _playing = value;
		}
		private bool _playing;

		public string Animation {
			get => _animation;
			set {
				if ( string.Equals( _animation, value ) ) {
					return;
				}
				_animation = value;
				Play( _animation );
			}
		}
		private string _animation;

		public float SpeedScale {
			get => _speedScale;
			set => _speedScale = value;
		}
		private float _speedScale = 0.0f;

		private readonly Dictionary<string, AnimationData> _animations;

		private int _currentFrame = 0;
		private float _customSpeedScale = 1.0f;
		private float _frameProgress = 0.0f;

		private Vector2 _offset = Vector2.Zero;

		private float _frameSpeedScale = 0.0f;

		public readonly AnimationChanged AnimationChanged = new AnimationChanged();
		public readonly AnimationFinished AnimationFinished = new AnimationFinished();
		public readonly AnimationLooped AnimationLooped = new AnimationLooped();
		public readonly FrameChanged FrameChanged = new FrameChanged();

		/*
		===============
		AnimationEntity
		===============
		*/
		public AnimationEntity( IEntityComponentSystemService ecs, IEntity owner, AnimatedSprite2D animatedSprite )
			: base( ecs, owner, animatedSprite )
		{
			_animation = animatedSprite.Animation;

			ref var animationComponent = ref ecs.GetOrAddComponent<AnimationStateComponent>( owner );
			animationComponent.CurrentAnimation = animatedSprite.Animation;

			string[] animationNames = animatedSprite.SpriteFrames.GetAnimationNames();
			var animationCount = animationNames.Length;
			_animations = new Dictionary<string, AnimationData>( animationCount );

			SpriteFrames spriteFrames = animatedSprite.SpriteFrames;
			for ( int i = 0; i < animationCount; i++ ) {
				StringName animationName = animationNames[ i ];
				
				var frameCount = spriteFrames.GetFrameCount( animationName );
				var frames = new FrameData[ frameCount ];
				for ( int f = 0; f < frameCount; f++ ) {
					frames[ f ] = new FrameData(
						spriteFrames.GetFrameTexture( animationName, f ),
						spriteFrames.GetFrameDuration( animationName, f )
					);
				}

				_animations[ animationName ] = new AnimationData(
					spriteFrames.GetAnimationLoop( animationName ),
					(float)spriteFrames.GetAnimationSpeed( animationName ),
					frameCount,
					frames
				);
			}
			_currentFrame = animatedSprite.Frame;

			_offset = animatedSprite.Offset;

			if ( _animations.ContainsKey( animatedSprite.Autoplay ) ) {
				Play( animatedSprite.Autoplay );
			}
			animatedSprite.CallDeferred( AnimatedSprite2D.MethodName.QueueFree );
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
		/// <param name="fromEnd"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="KeyNotFoundException"></exception>
		public void Play( string animationName, float speedScale = 1.0f, bool fromEnd = false ) {
			if ( string.IsNullOrEmpty( animationName ) ) {
				throw new ArgumentException( "Animation name cannot be null or empty" );
			}
			if ( !_animations.TryGetValue( animationName, out var animation ) ) {
				throw new KeyNotFoundException( $"There is no animation with name '{animationName}'" );
			}

			if ( animation.FrameCount == 0 ) {
				return;
			}

			_playing = true;
			int endFrame = Math.Max( 0, animation.FrameCount - 1 );
			if ( animationName != _animation ) {
				_animation = animationName;
				
				if ( fromEnd ) {
					SetFrameAndProgress( endFrame, 1.0f );
				} else {
					SetFrameAndProgress( 0, 0.0f );
				}
				AnimationChanged.Publish( new AnimationChangedEventData() );
			} else {
				bool isBackward = Math.Sign( speedScale * _customSpeedScale ) == -1;
				
				if ( fromEnd && isBackward && _currentFrame == 0 && _frameProgress <= 0.0f ) {
					SetFrameAndProgress( endFrame, 1.0f );
				} else if ( !fromEnd && !isBackward && _currentFrame == endFrame && _frameProgress >= 1.0f ) {
					SetFrameAndProgress( 0, 0.0f );
				}
			}
		}

		/*
		===============
		Pause
		===============
		*/
		public void Pause() {
			StopInternal( false );
		}

		/*
		===============
		Stop
		===============
		*/
		public void Stop() {
			StopInternal( true );
		}

		public override void Update( float deltaTime ) {
			if ( !_playing || !_animations.TryGetValue( _animation, out var animation ) ) {
				return;
			}

			float remaining = deltaTime;
			int i = 0;
			while ( remaining > 0.0f ) {
				float speed = animation.Speed * _speedScale * _customSpeedScale * _frameSpeedScale;
				float absSpeed = MathF.Abs( speed );

				if ( speed == 0.0f ) {
					return;
				}

				int frameCount = animation.FrameCount;
				int lastFrame = frameCount - 1;
				if ( Math.Sign( speed ) == -1 ) {
					if ( _frameProgress >= 1.0f ) {
						if ( _currentFrame >= lastFrame ) {
							if ( animation.Loop ) {
								_currentFrame = 0;
								AnimationLooped.Publish( new AnimationLoopedEventData() );
							} else {
								_currentFrame = 0;
								Pause();
								AnimationFinished.Publish( new AnimationFinishedEventArgs() );
								return;
							}
						} else {
							_currentFrame++;
						}
						CalcFrameSpeedScale();
						_frameProgress = 0.0f;
						FrameChanged.Publish( new FrameChangedEventData() );
					}
					float toProcess = Math.Min( ( 1.0f - _frameProgress ) / absSpeed, remaining );
					_frameProgress += toProcess * absSpeed;
					remaining -= toProcess;
				} else {
					// backwards

					if ( _frameProgress <= 0.0f ) {
						if ( _currentFrame <= 0 ) {
							if ( animation.Loop ) {
								_currentFrame = lastFrame;
								Pause();
								AnimationFinished.Publish( new AnimationFinishedEventArgs() );
								return;
							}
						} else {
							_currentFrame--;
						}
						CalcFrameSpeedScale();
						_frameProgress = 1.0f;
						FrameChanged.Publish( new FrameChangedEventData() );
					}
					float toProcess = Math.Min( _frameProgress / absSpeed, remaining );
					_frameProgress = toProcess * absSpeed;
					remaining -= toProcess;
				}
				i++;
				if ( i > frameCount ) {
					return; // prevents freezing if toProcess is each time much less than remaining 
				}
			}
		}

		/*
		===============
		Draw
		===============
		*/
		public void Draw( float deltaTime ) {
			if ( !_animations.TryGetValue( _animation, out var animation ) ) {
				return;
			}
			
			Texture2D texture = animation.Frames[ _currentFrame ].Texture;
			Vector2 offset = ( _offset + new Vector2( 0.5f, 0.5f ) ).Floor();
			Rect2 dstRect = new Rect2( offset, texture.GetSize() );

			ref var animationState = ref _ecs.GetComponent<AnimationStateComponent>( _owner );
			if ( animationState.HFlip ) {
				dstRect.Size = new Vector2( -dstRect.Size.X, dstRect.Size.Y );
			}
			if ( animationState.VFlip ) {
				dstRect.Size = new Vector2( dstRect.Size.X, -dstRect.Size.Y );
			}
			RenderingServer.CanvasItemAddTextureRectRegion( _canvasItemRid, dstRect, texture.GetRid(), new Rect2( new Vector2(), texture.GetSize() ) );
		}

		/*
		===============
		SetFrameAndProgress
		===============
		*/
		/// <summary>
		/// 
		/// </summary>
		/// <param name="frame"></param>
		/// <param name="progress"></param>
		private void SetFrameAndProgress( int frame, float progress ) {
			bool hasAnimation = _animations.ContainsKey( _animation );
			int endFrame = hasAnimation ? Math.Max( 0, _animations[ _animation ].FrameCount - 1 ) : 0;
			bool isChanged = _currentFrame != frame;

			if ( frame < 0 ) {
				_currentFrame = 0;
			} else if ( hasAnimation && frame > endFrame ) {
				_currentFrame = endFrame;
			} else {
				_currentFrame = frame;
			}

			CalcFrameSpeedScale();
			_frameProgress = progress;

			if ( !isChanged ) {
				return; // no changes, don't redraw
			}
			FrameChanged.Publish( new FrameChangedEventData() );
		}

		private void StopInternal( bool reset ) {
			_playing = false;
			if ( reset ) {
				_customSpeedScale = 1.0f;
				SetFrameAndProgress( 0, 0.0f );
			}
		}
		
		/*
		===============
		GetFrameDuration
		===============
		*/
		private float GetFrameDuration() {
			if ( _animations.TryGetValue( _animation, out var animation ) ) {
				return animation.Frames[ _currentFrame ].Duration;
			}
			return 1.0f;
		}

		/*
		===============
		CalcFrameSpeedScale
		===============
		*/
		private void CalcFrameSpeedScale() {
			_frameSpeedScale = 1.0f / GetFrameDuration();
		}
	};
};