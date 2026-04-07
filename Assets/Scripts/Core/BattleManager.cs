// Core/BattleManager.cs
using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    // ── Стейт ──────────────────────────────────────────
    public enum BattleState { Idle, PlayerTurn, EnemyTurn, BattleEnd }
    public BattleState State { get; private set; } = BattleState.Idle;
    public Enemy CurrentEnemy { get; private set; }

    // ── События (UI подписывается на них) ───────────────
    public event Action OnBattleStart;
    public event Action OnPlayerTurnStart;
    public event Action OnEnemyTurnStart;
    public event Action<int> OnEnemyTookDamage;
    public event Action<int> OnPlayerTookDamage;
    public event Action<bool> OnBattleEnd;

    // ── Ссылки ──────────────────────────────────────────
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private EnemyBattleView enemyBattleView;
    [SerializeField] private ArmorPanel armorPanel;  // ← добавили
    private DiceBag diceBag => PlayerDiceManager.Instance.diceBag;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Запуск боя ──────────────────────────────────────
    public void StartBattle(Enemy enemy)
    {
        if (State != BattleState.Idle) return;
        CurrentEnemy = enemy;
        State = BattleState.PlayerTurn;
        EnemyBattleView.Instantiate(enemy);
        armorPanel.Init(enemy.armor);  // ← добавили
        OnBattleStart?.Invoke();
        StartPlayerTurn();
    }

    // ── Ход игрока ──────────────────────────────────────
    private void StartPlayerTurn()
    {
        State = BattleState.PlayerTurn;

        foreach (var dice in diceBag.bag.ToArray())
            diceBag.AddToPool(dice);

        diceBag.DrawToTray();
        Debug.Log($"StartPlayerTurn: кубов в трее = {diceBag.tray.Count}");
        OnPlayerTurnStart?.Invoke();
    }

    // Вызывается кнопкой Attack в BattleUI
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
    }

    // Вызывается кнопкой End Turn в BattleUI
    public void PlayerEndTurn()
    {
        if (State != BattleState.PlayerTurn) return;
        combatSystem.ClearBattlefield();
        StartEnemyTurn();
    }

    // ── Ход врага ───────────────────────────────────────
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

    // ── Конец боя ───────────────────────────────────────
    private void EndBattle(bool playerWon)
    {
        State = BattleState.BattleEnd;
        enemyBattleView.Cleanup();
        armorPanel.Init(null);  // ← чистим панель после боя
        OnBattleEnd?.Invoke(playerWon);
        CurrentEnemy = null;
        State = BattleState.Idle;
    }
}