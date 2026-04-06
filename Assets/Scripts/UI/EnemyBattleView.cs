using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private HPBar hpBar;
    private Enemy enemy;

    public void Init(Enemy e)
    {
        enemy = e;
        if (enemy.data !=null && icon != null)
        {
            icon.sprite = enemy.data.battleSprite;
        }
        hpBar?.UpdateBar(e.currentHP, e.maxHP);
        e.OnHPChanged += OnHPChanged;
    }
    private void OnHPChanged(int current, int max)
    {
        hpBar?.UpdateBar(current, max);
    }
    public void Cleanup()
    {
        if (enemy != null)
        {
            enemy.OnHPChanged -= OnHPChanged;
        }
    }
}