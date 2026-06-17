using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance { get; private set; }
    [SerializeField] private ArmorData startingArmorData;
    public Armor armor { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        if (startingArmorData != null) armor = new Armor(startingArmorData);
    }

    public void TakeDamage(int incomingDamage)
    {
        int damage = armor != null ? armor.CalcDamage(incomingDamage) : incomingDamage;
        PlayerHealth.Instance.TakeDamage(damage);
        armor?.ClearAll(); // очищаем броню после того, как она сработала
    }

    public void ClearArmor()
    {
        armor?.ClearAll();
    }
}