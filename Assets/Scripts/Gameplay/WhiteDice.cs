// Белый куб — стандартный, расходный материал (по ГДД)
// Пока ведёт себя как базовый Dice
// В будущем: совмещённый белый куб даёт числа 7-12
public class WhiteDice : Dice
{
    public WhiteDice()
    {
        type = DiceType.White;
        faces = 6;
    }
}