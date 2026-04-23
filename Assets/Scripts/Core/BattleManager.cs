// Core/BattleManager.cs
using System;
using UnityEngine;

// Controls high-level battle flow and state transitions

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public enum BattleState { Idle, PlayerTurn, EnemyTurn, BattleEnd }
    public BattleState State { get; private set; } = BattleState.Idle;
    public Enemy CurrentEnemy { get; private set; }
    public Enemy AttackTarget { get; private set; }
    public Dice SelectedDice { get; private set; }
    public int SelectedDiceValue { get; private set; }

    public event Action OnBattleStart;
    public event Action OnPlayerTurnStart;
    public event Action OnEnemyTurnStart;
    public event Action<int> OnEnemyTookDamage;
    public event Action<int> OnPlayerTookDamage;
    public event Action<bool> OnBattleEnd;
    public event Action OnBattlefieldClear;

    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private EnemyBattleView enemyBattleView;
    [SerializeField] private ArmorPanel armorPanel;  // ← добавили
    private DiceBag diceBag => PlayerDiceManager.Instance.diceBag;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartBattle(Enemy enemy)
    {
        if (State != BattleState.Idle) return;
        CurrentEnemy = enemy;
        State = BattleState.PlayerTurn;
        EnemyBattleView.Instantiate(enemy);
        combatSystem.SetTarget(enemy);
        armorPanel.Init(enemy.armor);
        OnBattleStart?.Invoke();
        StartPlayerTurn();
    }

    public void NotifyBattlefieldClear()
    {
        OnBattlefieldClear?.Invoke();
    }

    private void StartPlayerTurn()
    {
        State = BattleState.PlayerTurn;

        diceBag.DrawToTray();
        Debug.Log($"Player turn started. Dice in tray: {diceBag.tray.Count}");;
        OnPlayerTurnStart?.Invoke();
    }

    public void SelectDice(Dice dice, int rolledValue)
    {
        SelectedDice = dice;
        SelectedDiceValue = rolledValue;
        Debug.Log($"Player selected dice: {dice.id}, value: {rolledValue}");
    }

    public void ClearSelection()
    {
        SelectedDice = null;
        SelectedDiceValue = 0;
    }

    public void SetAttackTarget(Enemy enemy)
    {
        AttackTarget = enemy;
    }

    public void PlayerAttack()
    {
        if (State != BattleState.PlayerTurn) return;
        int dmg = combatSystem.ResolveAttack();
        OnEnemyTookDamage?.Invoke(dmg);

        if (CurrentEnemy.IsDead())
        {
            EndBattle(playerWon: true);
            return;
        }
        AttackTarget = null;
    }

    public void PlayerEndTurn()
    {
        if (State != BattleState.PlayerTurn) return;
        combatSystem.ClearBattlefield();
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        State = BattleState.EnemyTurn;
        OnEnemyTurnStart?.Invoke();

        int dmg = CurrentEnemy.Attack();
        PlayerHealth.Instance.TakeDamage(dmg);
        OnPlayerTookDamage?.Invoke(dmg);

        if (PlayerHealth.Instance.IsDead())
        {
            EndBattle(playerWon: false);
            return;
        }

        StartPlayerTurn();
    }

    private void EndBattle(bool playerWon)
    {
        State = BattleState.BattleEnd;
        enemyBattleView.Cleanup();
        armorPanel.Init(null);
        OnBattleEnd?.Invoke(playerWon);
        CurrentEnemy = null;
        State = BattleState.Idle;
    }
}