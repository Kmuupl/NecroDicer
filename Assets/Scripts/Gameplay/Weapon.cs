using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName = "sword";
    public int damageMin = 4;
    public int damageMax = 10;
    public int tier = 1;

    public int RollDamage()
    {
        return Random.Range(damageMin, damageMax + 1);
    }
    public override string ToString()
    {
        return $"{weaponName} (Tier {tier}): {damageMin}-{damageMax} damage";
    }
}