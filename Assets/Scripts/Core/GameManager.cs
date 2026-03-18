using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NecroDice.Gameplay;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Список врагов на сцене")]
    public List<EnemyMovement> enemies = new List<EnemyMovement>();

    [HideInInspector]
    public bool isPlayerTurn = true; // флаг, чей сейчас ход

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Вызываем, когда игрок завершил ход
    public void OnPlayerMoveFinished()
    {
        isPlayerTurn = false;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        foreach (var enemy in enemies)
        {
            yield return enemy.StepMove();
        }

        // После ходов всех врагов ход игрока снова доступен
        isPlayerTurn = true;
    }
}
