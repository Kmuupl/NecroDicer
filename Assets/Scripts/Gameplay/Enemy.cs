using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP = 10;
    public int currentHP;

    public int attackDamage = 3;

    public int speed = 5;

    public Armor armor = new();

    void Start()
    {
        currentHP = maxHP;
        SetupTestArmor(); //TEST
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"Враг получил {damage} урона. Текущее HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Враг побежден!");
        Destroy(gameObject);
    }

    //тестовый метод для настройки брони врага
    void SetupTestArmor()
    {
        armor.slots.Add(new ArmorSlot
        {
            condition = new ArmorCondition
            {
                requiredValue = 1,
                mustBeGreater = false
            }
        });

        armor.slots.Add(new ArmorSlot
        {
            condition = new ArmorCondition
            {
                requiredValue = 4,
                mustBeGreater = true
            }
        });

        armor.damagePercents = new float[] { 0f, 0.6f, 1f }; // 0% урона, если 0 слотов заполнено, 60% урона при 1 заполненном слоте, 100% урона при 2 заполненных слотах

        //Debug.Log("Тестовая броня врага: =1, >4.");
    }

    public void Attack()
    {
        if (PlayerHealth.Instance == null)
        {
            Debug.LogError("PlayerHealth instance not found!");
            return;
        }

        Debug.Log($"Враг атакует игрока, нанося {attackDamage} урона.");
        PlayerHealth.Instance.TakeDamage(attackDamage);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && hit.gameObject == gameObject)
            {
                CombatSystem combatSystem = FindObjectOfType<CombatSystem>();
                if (combatSystem != null)
                    combatSystem.StartBattle(this);
            }
        }
    }
}
