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

using System.Numerics;
using Nomad.Core.Scene.GameObjects;
using Nomad.EngineTemplates.Attributes;
using Nomad.EngineTemplates.BaseClasses;

namespace Nomad.EngineTemplates.Scene.GameObjects
{
    /// <summary>
    /// Declares the engine template for simulated 2D rigid bodies.
    /// </summary>
    [TemplateClass(Contract = typeof(IRigidBody2D), GodotBase = "Godot.RigidBody2D", UnityBase = "UnityEngine.Rigidbody2D")]
    [TemplateNamespace(Name = "Scene.GameObjects")]
    [TemplateProperty(
        Name = "LinearVelocity",
        Type = typeof(Vector2),
        GodotGetterExpression = "new global::System.Numerics.Vector2(base.LinearVelocity.X, base.LinearVelocity.Y)",
        GodotSetterExpression = "base.LinearVelocity = new global::Godot.Vector2(value.X, value.Y)",
        UnityGetterExpression = "new global::System.Numerics.Vector2(base.velocity.x, base.velocity.y)",
        UnitySetterExpression = "base.velocity = new global::UnityEngine.Vector2(value.X, value.Y)")]
    [TemplateProperty(
        Name = "AngularVelocity",
        Type = typeof(float),
        GodotGetterExpression = "base.AngularVelocity",
        GodotSetterExpression = "base.AngularVelocity = value",
        UnityGetterExpression = "base.angularVelocity",
        UnitySetterExpression = "base.angularVelocity = value")]
    [TemplateProperty(
        Name = "Mass",
        Type = typeof(float),
        GodotGetterExpression = "base.Mass",
        GodotSetterExpression = "base.Mass = value",
        UnityGetterExpression = "base.mass",
        UnitySetterExpression = "base.mass = value")]
    [TemplateProperty(
        Name = "GravityScale",
        Type = typeof(float),
        GodotGetterExpression = "base.GravityScale",
        GodotSetterExpression = "base.GravityScale = value",
        UnityGetterExpression = "base.gravityScale",
        UnitySetterExpression = "base.gravityScale = value")]
    [TemplateProperty(
        Name = "LinearDamping",
        Type = typeof(float),
        GodotGetterExpression = "base.LinearDamp",
        GodotSetterExpression = "base.LinearDamp = value",
        UnityGetterExpression = "base.drag",
        UnitySetterExpression = "base.drag = value")]
    [TemplateProperty(
        Name = "AngularDamping",
        Type = typeof(float),
        GodotGetterExpression = "base.AngularDamp",
        GodotSetterExpression = "base.AngularDamp = value",
        UnityGetterExpression = "base.angularDrag",
        UnitySetterExpression = "base.angularDrag = value")]
    [TemplateProperty(
        Name = "Sleeping",
        Type = typeof(bool),
        GodotGetterExpression = "base.Sleeping",
        GodotSetterExpression = "base.Sleeping = value",
        UnityGetterExpression = "base.IsSleeping()",
        UnitySetterExpression = "{ if ( value ) { base.Sleep(); } else { base.WakeUp(); } }")]
    [TemplateObject2D]
    internal class EngineRigidBody2D
    {
    }
}
