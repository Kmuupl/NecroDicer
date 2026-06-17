using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattlefieldPanel : MonoBehaviour
{
    public static BattlefieldPanel Instance { get; private set; }
    [SerializeField] private GameObject diceButtonPrefab;
    [SerializeField] private float throwDuration = 0.4f;
    [SerializeField] private int maxDice = 10;
    [SerializeField] private float diceSize = 50f;
    [SerializeField] private float padding = 10f;

    private DiceBag diceBag => PlayerDiceManager.Instance.diceBag;
    private Dictionary<Dice, GameObject> spawnedButtons = new();

    // слоты — заранее вычисленные позиции на поле
    private List<Vector2> slots = new();
    private Dictionary<int, Dice> slotOccupied = new(); // индекс слота → куб

    private System.Action<bool> onBattleEnd;
    /*     void Awake()
        {
            GenerateSlots();

        } */

    void Start()
    {
        GenerateSlots();
        BattleManager.Instance.OnBattlefieldClear += ClearField;
        BattleManager.Instance.OnPlayerTurnStart += ClearField;
        onBattleEnd = (_) => ClearField();
        BattleManager.Instance.OnBattleEnd += onBattleEnd;
    }

    // private IEnumerator Init()
    // {
    //     yield return null; // ждём один кадр — Canvas успевает посчитать размеры
    //     GenerateSlots();
    //     BattleManager.Instance.OnPlayerTurnStart += ClearField;
    //     BattleManager.Instance.OnBattleEnd += _ => ClearField();
    // }

    void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPlayerTurnStart -= ClearField;
            BattleManager.Instance.OnBattleEnd -= onBattleEnd;
            BattleManager.Instance.OnBattlefieldClear -= ClearField;
        }
        // BattleManager.Instance.OnBattlefieldClear -= ClearField;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void GenerateSlots()
    {
        slots.Clear();
        RectTransform rt = GetComponent<RectTransform>();
        float w = rt.rect.width;
        float h = rt.rect.height;
        Debug.Log($"GenerateSlots: размер панели {w}x{h}");

        float step = diceSize + padding;
        int cols = Mathf.FloorToInt((w - padding) / step);
        int rows = Mathf.FloorToInt((h - padding) / step);

        float totalW = cols * step - padding;
        float totalH = rows * step - padding;
        float startX = -totalW / 2f + diceSize / 2f;
        float startY = -totalH / 2f + diceSize / 2f;

        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                if (slots.Count >= maxDice) break;
                slots.Add(new Vector2(
                    startX + col * step,
                    startY + row * step
                ));
            }

        // перемешиваем чтобы кубы не ложились строго по сетке
        for (int i = slots.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (slots[i], slots[j]) = (slots[j], slots[i]);
        }
        Debug.Log($"Сгенерировано слотов: {slots.Count}");
    }

    private int GetFreeSlot()
    {
        for (int i = 0; i < slots.Count; i++)
            if (!slotOccupied.ContainsKey(i)) return i;
        return -1;
    }

    public void AddDice(Dice dice, Vector3 startWorldPos)
    {
        int slotIndex = GetFreeSlot();
        if (slotIndex == -1) { Debug.LogWarning("Поле боя заполнено!"); return; }

        var btn = Instantiate(diceButtonPrefab, transform);
        btn.GetComponent<Button>().interactable = false;

        int rolledValue = diceBag.battleFieldRolls[dice];
        var label = btn.GetComponentInChildren<Text>();
        if (label != null) label.text = rolledValue.ToString();

        float randomRot = Random.Range(-15f, 15f);
        slotOccupied[slotIndex] = dice;
        spawnedButtons[dice] = btn;

        var d = dice;
        int si = slotIndex;
        StartCoroutine(ThrowAnim(btn, startWorldPos, slots[slotIndex], randomRot, () =>
        {
            btn.GetComponent<Button>().interactable = true;
            btn.GetComponent<Button>().onClick.AddListener(() => SelectDice(d, btn, si));
        }));
    }

    private IEnumerator ThrowAnim(GameObject obj, Vector3 startWorldPos, Vector2 targetLocal, float finalRot, System.Action onDone)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        RectTransform fieldRT = GetComponent<RectTransform>();

        // переводим мировую позицию трея в локальную позицию внутри BattlefieldPanel
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, startWorldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(fieldRT, screenPoint, Camera.main, out Vector2 startLocal);

        rt.anchoredPosition = startLocal;
        rt.localScale = Vector3.one;

        float t = 0f;
        float spinAmount = Random.Range(360f, 3600f) * (Random.value > 0.5f ? 1f : -1f);

        while (t < 1f)
        {
            t += Time.deltaTime / throwDuration;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            float arc = Mathf.Sin(t * Mathf.PI) * 60f;
            Vector2 pos = Vector2.Lerp(startLocal, targetLocal, ease);
            pos.y += arc;
            rt.anchoredPosition = pos;

            rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, spinAmount + finalRot, ease));

            yield return null;
        }

        rt.anchoredPosition = targetLocal;
        rt.localRotation = Quaternion.Euler(0f, 0f, finalRot);
        onDone?.Invoke();
    }

    private void SelectDice(Dice dice, GameObject btn, int slotIndex)
    {
        int rolledValue = diceBag.battleFieldRolls[dice];
        BattleManager.Instance.SelectDice(dice, rolledValue);
    }

    public void RemoveDice(Dice dice)
    {
        if (spawnedButtons.TryGetValue(dice, out var btn))
        {
            Destroy(btn);
            spawnedButtons.Remove(dice);
        }

        foreach (var kv in slotOccupied)
        {
            if (kv.Value == dice)
            {
                slotOccupied.Remove(kv.Key);
                break;
            }
        }
    }

    private void ClearField()
    {
        foreach (var btn in spawnedButtons.Values)
            if (btn != null) Destroy(btn);
        spawnedButtons.Clear();
        slotOccupied.Clear();
    }
}