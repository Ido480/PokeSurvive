using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "PokemonSurvivor/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHealth;
    public float moveSpeed;
    public float damage;
    public int expValue;
    public float pushTime;
    public GameObject prefab; // Optional: can store the prefab here too
    public int coinValue; // How many coins they drop
    [Range(0, 100)] public float dropChance; // 10, 15, etc.
}