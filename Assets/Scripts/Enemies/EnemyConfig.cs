using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Mario/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    public float moveSpeed = 2f;
    public int scoreValue = 100;
    public bool turnsAtLedge = false;
    public float stompBounce = 12f;
}
