using UnityEngine;

public class PlayerDiceManager : MonoBehaviour
{
    public static PlayerDiceManager Instance;

    //главное хранилище кубов игрока:
    public DiceBag diceBag = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        //стартовые тестовые кубы:
        AddStartingDice();
    }

    //добавляем кубы в ммешок:
    public bool AddDice(Dice dice)
    {
        if (diceBag.bag.Exists(d => d.id == dice.id))
        {
            Debug.LogWarning($"Куб {dice.id} уже есть в мешке :()");
            return false;
        }
        diceBag.AddToBag(dice);
        return true;
    }

    //тестовый метод для старта игры, добавляет 5 белых кубов в мешок игрока:
    private void AddStartingDice()
    {
        for (int i = 1; i <= 5; i++)
        {
            Dice dice = DiceFactory.Create(DiceType.White, $"white_0{i}");
            AddDice(dice);
        }
        Debug.Log("Стартовые кубы добавлены в мешок игрока.");
    }
}