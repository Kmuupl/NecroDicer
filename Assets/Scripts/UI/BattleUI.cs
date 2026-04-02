// UI/BattleUI.cs
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button endTurnButton;

    void Start()
    {
        attackButton.onClick.AddListener(BattleManager.Instance.PlayerAttack);
        endTurnButton.onClick.AddListener(BattleManager.Instance.PlayerEndTurn);

        BattleManager.Instance.OnBattleStart    += () => battlePanel.SetActive(true);
        BattleManager.Instance.OnBattleEnd      += _ => battlePanel.SetActive(false);
        BattleManager.Instance.OnPlayerTurnStart += () => SetButtons(true);
        BattleManager.Instance.OnEnemyTurnStart  += () => SetButtons(false);

        battlePanel.SetActive(false);
    }

    private void SetButtons(bool interactable)
    {
        attackButton.interactable = interactable;
        endTurnButton.interactable = interactable;
    }
}