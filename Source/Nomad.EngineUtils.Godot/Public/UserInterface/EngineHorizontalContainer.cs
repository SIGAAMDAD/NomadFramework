/*
===========================================================================
The Nomad Framework
Copyright (C) 2025 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

/*
using System;
using System.Collections.Generic;
using Godot;
using Nomad.Core.ECS;
using Nomad.Core.EngineUtils;
using Nomad.Core.EngineUtils.UserInterface;
using Nomad.Core.Events;
using Nomad.Events.Globals;

namespace Nomad.EngineUtils.UserInterface
{
    /// <summary>
    ///
    /// </summary>
    public partial class EngineHorizontalContainer : VBoxContainer, IVerticalContainer
    {
        private static readonly StringName @SeparationThemeConstantName = "separation";

        System.Numerics.Vector2 IUIElement.Position
        {
            get => Position.ToSystem();
            set => Position = value.ToGodot();
        }

        System.Numerics.Vector2 IUIElement.Scale
        {
            get => Scale.ToSystem();
            set => Scale = value.ToGodot();
        }

        float IVerticalContainer.Spacing
        {
            get => GetThemeConstant(SeparationThemeConstantName);
            set => AddThemeConstantOverride(SeparationThemeConstantName, (int)value);
        }

        /// <summary>
        ///
        /// </summary>
        string IGameObject.Name
        {
            get => _impl.Name;
            set => _impl.Name = value;
        }

        IGameObject? IGameObject.Parent
        {
            get => _impl.Parent;
            set => _impl.Parent = value;
        }

        IReadOnlyList<IGameObject> IGameObject.Children => _impl.Children;

        bool IGameObject.Enabled
        {
            get => _impl.Enabled;
            set => _impl.Enabled = value;
        }

        private readonly GodotGameObject _impl;

        /// <summary>
        ///
        /// </summary>
        IGameEvent<EmptyEventArgs> IUIElement.Focused => _focused;
        private IGameEvent<EmptyEventArgs> _focused;

        /// <summary>
        ///
        /// </summary>
        IGameEvent<EmptyEventArgs> IUIElement.Unfocused => _unfocused;
        private IGameEvent<EmptyEventArgs> _unfocused;

        /// <summary>
        ///
        /// </summary>
        public EngineHorizontalContainer()
        {
            _impl = new GodotGameObject(this);
        }

        /// <summary>
        ///
        /// </summary>
        public sealed override void _Ready()
        {
            base._Ready();

            _impl.OnInit();
            OnInit();

            _focused = GameEventRegistry.GetEvent<EmptyEventArgs>($"{GetHashCode()}:{Constants.Events.UI_ELEMENT_FOCUSED}", Constants.Events.NAMESPACE);
            _unfocused = GameEventRegistry.GetEvent<EmptyEventArgs>($"{GetHashCode()}:{Constants.Events.UI_ELEMENT_UNFOCUSED}", Constants.Events.NAMESPACE);

            FocusEntered += () => _focused.Publish(default);
            MouseEntered += () => _focused.Publish(default);
            FocusExited += () => _unfocused.Publish(default);
            MouseExited += () => _unfocused.Publish(default);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delta"></param>
        public sealed override void _Process(double delta)
        {
            base._Process(delta);

            float deltaTime = (float)delta;
            _impl.OnUpdate((float)delta);
            OnUpdate(deltaTime);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delta"></param>
        public sealed override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            float deltaTime = (float)delta;
            _impl.OnPhysicsUpdate(deltaTime);
            OnPhysicsUpdate(deltaTime);
        }

        /// <summary>
        ///
        /// </summary>
        public sealed override void _ExitTree()
        {
            base._ExitTree();

            _impl.OnShutdown();
            OnShutdown();
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnShutdown()
        {
        }

        protected virtual void OnUpdate(float delta)
        {
        }

        protected virtual void OnPhysicsUpdate(float delta)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public T AddComponent<T>(Action<T>? initializer = null)
            where T : IComponent, new()
        {
            return _impl.AddComponent(initializer);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetComponent<T>()
            where T : IComponent
        {
            return _impl.GetComponent<T>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasComponent<T>()
            where T : IComponent
        {
            return _impl.HasComponent<T>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveComponent<T>()
            where T : IComponent
        {
            _impl.RemoveComponent<T>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        public T? FindChild<T>(string childName)
            where T : class, IGameObject
        {
            return _impl.FindChild<T>(childName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(IGameObject child)
        {
            _impl.AddChild(child);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(IGameObject child)
        {
            _impl.RemoveChild(child);
        }
    }
}
*/
