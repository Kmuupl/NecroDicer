// Gameplay/EnemyData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "NecroDicer/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public Sprite battleSprite;
    public int maxHP;
    public int attackDamage;
    public int speed;
    // броня добавим отдельно
}