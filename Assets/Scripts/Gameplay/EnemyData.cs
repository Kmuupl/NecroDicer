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
    public ArmorData armorData;
    public DiceType[] dicePool;
    public int dicePoolSize = 5;
}