using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 30;
    public int currentHP;

    void Start()
    {
        currentHP = maxHP;
        Debug.Log($"Player HP: {currentHP}/{maxHP}");
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
        Debug.Log($"Player takes {damage} damage. HP: {currentHP}/{maxHP}");

        if (currentHP == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
        Debug.Log($"Player heals {amount} HP. HP: {currentHP}/{maxHP}");
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // Здесь можно добавить анимацию смерти, перезагрузку уровня и т.д.
    }

    public static PlayerHealth Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}