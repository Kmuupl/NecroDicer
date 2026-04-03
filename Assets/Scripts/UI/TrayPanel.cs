using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TrayPanel : MonoBehaviour
{
    [SerializeField] private GameObject diceButtonPrefab;
    [SerializeField] private float slideInDuration = 0.15f;
    private DiceBag diceBag => PlayerDiceManager.Instance.diceBag;
    private List<GameObject> spawnedButtons = new();

    void Start()
    {
        BattleManager.Instance.OnPlayerTurnStart += RefreshTray;
        BattleManager.Instance.OnBattleEnd += _ => ClearTray();
        if (BattleManager.Instance.State == BattleManager.BattleState.PlayerTurn)
            RefreshTray();
    }

    void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPlayerTurnStart -= RefreshTray;
            BattleManager.Instance.OnBattleEnd -= _ => ClearTray();
        }
    }

    private void RefreshTray()
    {
        Debug.Log($"RefreshTray вызван, кубов в трее: {diceBag.tray.Count}");
        ClearTray();
        StartCoroutine(SpawnDice());
    }

    private IEnumerator SpawnDice()
    {
        foreach (var dice in diceBag.tray)
        {
            var btn = Instantiate(diceButtonPrefab, transform);

            //            btn.transform.SetAsFirstSibling();

            var label = btn.GetComponentInChildren<Text>();
            var img = btn.GetComponent<Image>();
            if (img != null) img.sprite = GetSpriteForDice(dice);

            var d = dice;
            btn.GetComponent<Button>().onClick.AddListener(() => ThrowDice(d, btn));

            spawnedButtons.Add(btn);

            // КЛЮЧ: ждём 1 кадр, чтобы LayoutGroup поставил позицию
            yield return null;

            StartCoroutine(SlideFromLeft(btn));

            // задержка между кубами
            yield return new WaitForSeconds(slideInDuration);
        }
    }

    private IEnumerator SlideFromLeft(GameObject obj)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();

        Vector3 targetPos = rt.localPosition;

        // СТАРТ СЛЕВА (вне трея)
        float trayWidth = ((RectTransform)transform).rect.width;

        Vector3 startPos = targetPos + new Vector3(-trayWidth, 0f, 0f);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / slideInDuration;

            rt.localPosition = Vector3.Lerp(
                startPos,
                targetPos,
                Mathf.SmoothStep(0f, 1f, t)
            );

            yield return null;
        }

        rt.localPosition = targetPos;
    }

    /*     private IEnumerator ScaleIn(GameObject obj)
        {
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / slideInDuration;

                obj.transform.localScale = Vector3.Lerp(
                    Vector3.zero,
                    Vector3.one,
                    Mathf.SmoothStep(0f, 1f, t)
                );

                yield return null;
            }

            obj.transform.localScale = Vector3.one;
        } */

    /*     private IEnumerator SlideIn(GameObject obj)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            Vector2 targetPos = rt.anchoredPosition;
            rt.anchoredPosition = targetPos + new Vector2(80f, 0f); // стартуем правее

            float t = 0f;
            Vector2 startPos = rt.anchoredPosition;
            while (t < 1f)
            {
                t += Time.deltaTime / slideInDuration;
                rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            rt.anchoredPosition = targetPos;
        } */

    private void ThrowDice(Dice dice, GameObject btn)
    {
        if (!diceBag.tray.Contains(dice)) return;
        diceBag.ThrowToButtleField(dice);
        Destroy(btn);
        spawnedButtons.Remove(btn);
    }

    private void ClearTray()
    {
        foreach (var btn in spawnedButtons)
            if (btn != null) Destroy(btn);
        spawnedButtons.Clear();
    }

    private Sprite GetSpriteForDice(Dice dice)
    {
        // TODO: вернуть спрайт по типу/цвету куба
        return null; // пока без спрайта, Image останется дефолтной
    }
}