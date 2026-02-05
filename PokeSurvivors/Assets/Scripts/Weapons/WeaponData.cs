using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "PokemonSurvivor/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite icon;
    public string description;

    // This is the list you currently have in Weapon.cs
    public List<WeaponLevelStats> levels;
}

[System.Serializable]
public class WeaponLevelStats
{
    public float cooldown;
    public float duration;
    public float damage;
    public float range;
    public float speed;
    public int amount;
    public string levelDescription;
}