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
    public event Action<int> OnEnemyTookDamage;   // int = урон
    public event Action<int> OnPlayerTookDamage;  // int = урон
    public event Action<bool> OnBattleEnd;         // bool = победа игрока

    // ── Ссылки ──────────────────────────────────────────
    [SerializeField] private CombatSystem combatSystem;
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
        OnBattleStart?.Invoke();
        StartPlayerTurn();
    }

    // ── Ход игрока ──────────────────────────────────────
    private void StartPlayerTurn()
    {
        State = BattleState.PlayerTurn;

        // временно: перекидываем всё из мешка в пул
        foreach (var dice in diceBag.bag.ToArray())
            diceBag.AddToPool(dice);

        diceBag.DrawToTray();          // пул → трей
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
        // После атаки игрок может атаковать снова — стейт не меняем
    }

    // Вызывается кнопкой End Turn в BattleUI
    public void PlayerEndTurn()
    {
        if (State != BattleState.PlayerTurn) return;
        combatSystem.ClearBattlefield();   // кубы без применения → сброс
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
        OnBattleEnd?.Invoke(playerWon);
        CurrentEnemy = null;
        State = BattleState.Idle;
    }
}