using UnityEngine;

public enum DiceType
{
    White,
    Red,
    Blue,
    Green,
    Orange,
    Gold,
    Purple,
    Echo
}

// Базовый класс. Наследники переопределяют OnRoll()
public class Dice
{
    public string id;
    public DiceType type;
    public int faces = 6; // сколько граней (d6 по умолчанию)

    // Главный метод — бросить куб
    // virtual означает: наследники могут переопределить его
    public virtual int Roll()
    {
        return Random.Range(1, faces + 1);
    }

    // Для удобства — показать инфо о кубе в консоли
    public override string ToString()
    {
        return $"[{type} d{faces} id:{id}]";
    }
}