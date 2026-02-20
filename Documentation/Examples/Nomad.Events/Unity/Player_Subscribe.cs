using UnityEngine;
using Nomad.Core.Events;

// Declare the payload struct
public readonly struct DamagePlayerEventArgs
{
    /// <summary>
    /// The amount of damage dealt to the player.
    /// </summary>
    public readonly int Amount;

    /// <summary>
    /// Creates a new DamagePlayerEventArgs object.
    /// </summary>
    /// <param name="amount">The amount of damage dealt to the player.</param>
    public DamagePlayerEventArgs(int amount)
    {
        Amount = amount;
    }
}

public class Player : MonoBehaviour
{
    private int _health = 100;

    // Declare the event.
    private IGameEvent<DamagePlayerEventArgs> _takeDamage;

    /// <summary>
    /// This will be called when the GameObject is initializing.
    /// </summary>
    private void Awake()
    {
        // Retrieve the event from the global registry, its name is "TakeDamage", and it's within the player's
        // namespace, so "Player".
        _takeDamage = GameEventRegistry.GetEvent<DamagePlayerEventArgs>("TakeDamage", "Player");

        // Hook the subscription callbacks. "Subscribe" to the event, the "TakeDamage" method will be
        // called into every time the event is published.
        // Ensure that the payload is put as the only parameter for the "TakeDamage" method with the "in" modifier as seen below.
        _takeDamage.Subscribe(this, OnTakeDamage);
    }

    // This will be called whenever the TakeDamage event is published.
    private void OnTakeDamage(in DamagePlayerEventArgs args)
    {
        _health -= args.Amount;
        Debug.Log("Took Damage!");
    }
}
