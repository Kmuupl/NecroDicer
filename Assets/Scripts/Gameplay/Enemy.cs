using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP = 10;
    public int currentHP;
    public int attackDamage = 3;
    public int speed = 5;
    public Armor armor = new();
    public HPBar hpBar;
    public bool IsDead() => currentHP <= 0;
    public EnemyData data;



    void Start()
    {
        if (data != null)
        {
            maxHP = data.maxHP;
            attackDamage = data.attackDamage;
            speed = data.speed;
        }

        currentHP = maxHP;
        hpBar?.UpdateBar(currentHP, maxHP);
        SetupTestArmor();
    }

    public event Action<int, int> OnHPChanged; // current, max

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
        OnHPChanged?.Invoke(currentHP, maxHP);
        if (currentHP <= 0) Die();
        hpBar?.UpdateBar(currentHP, maxHP);
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

    // В Enemy.cs — замени Attack():
    public int Attack()
    {
        Debug.Log($"Враг атакует игрока, нанося {attackDamage} урона.");
        return attackDamage;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            // В Enemy.cs — замени внутри Update():
            if (hit != null && hit.gameObject == gameObject)
            {
                BattleManager.Instance?.StartBattle(this);
            }
        }
    }
}
