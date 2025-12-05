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
using NomadCore.Systems.EntitySystem.Common;
using NomadCore.Systems.EntitySystem.Common.Models;
using NomadCore.Systems.EntitySystem.Services;
using System;
using System.Collections.Generic;

namespace NomadCore.Systems.EntitySystem.Infrastructure.Rendering {
	internal sealed class AnimationSystem( EntityComponentSystem ecs ) : RenderingServerSubSystem( ecs ) {
		private struct AnimationData {
			public string CurrentAnimation;
			public string[] AnimationNames;
			public Dictionary<string, Texture2D[]> AnimationFrames;
			public Dictionary<string, float> AnimationSpeeds;
			public bool Playing;
			public bool Loop;
		};

		private readonly Dictionary<Entity, AnimationData> _animations = new Dictionary<Entity, AnimationData>();

		public void Convert( Entity entity, AnimatedSprite2D animatedSprite ) {
			var spriteFrames = animatedSprite.SpriteFrames;
			var currentAnimation = animatedSprite.Animation;
			var currentFrame = animatedSprite.Frame;
			var playing = animatedSprite.IsPlaying();
			var speedScale = animatedSprite.SpeedScale;

			Rid canvasItem = RenderingServer.CanvasItemCreate();
			RenderingServer.CanvasItemSetParent( canvasItem, animatedSprite.GetCanvas() );

			RenderingServer.CanvasItemSetTransform( canvasItem, new Transform2D(
				rotation: animatedSprite.GlobalRotation,
				scale: animatedSprite.GlobalScale,
				skew: 0.0f,
				origin: animatedSprite.GlobalPosition
			) );

			var animationData = ExtractAnimationData( spriteFrames );
			animationData.CurrentAnimation = currentAnimation;
			animationData.Playing = playing;

			var currentTexture = GetCurrentFrameTexture( animationData, currentAnimation, currentFrame );
			var textureRid = currentTexture?.GetRid() ?? default;

			_canvasItems[ entity ] = canvasItem;
			_animations[ entity ] = animationData;
			_renderData[ entity ] = new ConvertedRenderData {
				CanvasItem = canvasItem,
				Texture = textureRid,
				Size = currentTexture?.GetSize() ?? Vector2.Zero,
				IsAnimated = true,
				CurrentFrame = currentFrame
			};

			animatedSprite.CallDeferred( AnimatedSprite2D.MethodName.QueueFree );
		}

		public override void Update( float deltaTime ) {

		}

		private Texture2D? GetCurrentFrameTexture( AnimationData animationData, string animation, int frame ) {
			if ( animationData.AnimationFrames.TryGetValue( animation, out var frames ) && frame >= 0 && frame < frames.Length ) {
				return frames[ frame ];
			}
			return null;
		}

		private AnimationData ExtractAnimationData( SpriteFrames frames ) {
			var data = new AnimationData() {
				AnimationFrames = new Dictionary<string, Texture2D[]>(),
				AnimationSpeeds = new Dictionary<string, float>(),
				AnimationNames = frames.GetAnimationNames() ?? Array.Empty<string>()
			};

			for ( int i = 0; i < data.AnimationNames.Length; i++ ) {
				string animationName = data.AnimationNames[ i ];

				int frameCount = frames.GetFrameCount( animationName );
				Texture2D[] images = new Texture2D[ frameCount ];

				for ( int j = 0; j < frameCount; i++ ) {
					images[ j ] = frames.GetFrameTexture( animationName, j );
				}

				data.AnimationFrames[ animationName ] = images;
				data.AnimationSpeeds[ animationName ] = (float)frames.GetAnimationSpeed( animationName );
			}
			return data;
		}

		private void UpdateAnimation( Entity entity, ref ConvertedRenderData renderData, AnimationData animationData, float deltaTime ) {
			if ( !animationData.Playing ) {
				return;
			}

			renderData.FrameTimer += deltaTime;
			if ( renderData.FrameTimer >= renderData.FrameDuration ) {
				renderData.FrameDuration -= renderData.FrameDuration;
				renderData.CurrentFrame++;

				Texture2D[] frames = animationData.AnimationFrames[ animationData.CurrentAnimation ];

				if ( renderData.CurrentFrame >= frames.Length ) {
					if ( animationData.Loop ) {
						renderData.CurrentFrame = 0;
					} else {
						renderData.CurrentFrame = frames.Length - 1;
						animationData.Playing = false;
						_animations[ entity ] = animationData;
					}

					Texture2D newTexture = frames[ renderData.CurrentFrame ];
					renderData.Texture = newTexture.GetRid();
					renderData.Size = newTexture.GetSize();
				}
				if ( _ecs.HasComponent<ServerRenderingComponent>( entity ) ) {
					ref var renderComponent = ref _ecs.GetOrAddComponent<ServerRenderingComponent>( entity );
					RenderingServer.CanvasItemSetModulate( renderData.CanvasItem, renderComponent.Modulate );
					RenderingServer.CanvasItemSetTransform( renderData.CanvasItem,  );
				}
			}
		}
	};
};