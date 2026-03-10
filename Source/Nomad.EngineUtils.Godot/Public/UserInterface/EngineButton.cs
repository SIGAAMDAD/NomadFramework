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

using Godot;
using Nomad.Core.EngineUtils.UserInterface;
using Nomad.Core.Events;
using Nomad.Events.Global;

namespace Nomad.EngineUtils.UserInterface
{
    /// <summary>
    ///
    /// </summary>
    public partial class EngineButton : GodotControl<Button>, IButton
    {
        /// <summary>
        ///
        /// </summary>
        public string Text
        {
            get => control.Text;
            set => control.Text = value;
        }

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<EmptyEventArgs> Pressed => _pressed;
        private readonly IGameEvent<EmptyEventArgs> _pressed;

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<EmptyEventArgs> Focused => _focused;
        private readonly IGameEvent<EmptyEventArgs> _focused;

        /// <summary>
        ///
        /// </summary>
        public IGameEvent<EmptyEventArgs> Unfocused => _unfocused;
        private readonly IGameEvent<EmptyEventArgs> _unfocused;

        /// <summary>
        ///
        /// </summary>
        public EngineButton()
        {
            _pressed = GameEventRegistry.GetEvent<EmptyEventArgs>($"{Name}:{Constants.Events.BUTTON_CLICKED}", Constants.Events.NAMESPACE);
            _focused = GameEventRegistry.GetEvent<EmptyEventArgs>($"{Name}:{Constants.Events.BUTTON_FOCUSED}", Constants.Events.NAMESPACE);
            _unfocused = GameEventRegistry.GetEvent<EmptyEventArgs>($"{Name}:{Constants.Events.BUTTON_UNFOCUSED}", Constants.Events.NAMESPACE);
        }

        /// <summary>
        ///
        /// </summary>
        public sealed override void _ExitTree()
        {
            base._ExitTree();

            _pressed?.Dispose();
            _focused?.Dispose();
        }
    }
}
