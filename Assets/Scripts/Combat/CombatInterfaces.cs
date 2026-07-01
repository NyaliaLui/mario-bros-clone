using UnityEngine;

/// <summary>Implemented by enemies that can be stomped from above.</summary>
public interface IStompable { void OnStomped(PlayerController player); }

/// <summary>Implemented by anything that can take damage (player, enemies).</summary>
public interface IDamageReceiver { void TakeDamage(int amount, Vector2 source); }

/// <summary>Implemented by blocks the player can hit from below.</summary>
public interface IBumpable { void OnBumpFromBelow(PlayerController player); }
