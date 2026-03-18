public static class DiceFactory
{
    public static Dice Create(DiceType type, string id)
    {
        Dice dice = type switch
        {
            DiceType.White => new WhiteDice(),
            // сюда будем добавлять новые кубы:
            // DiceType.Purple => new PurpleDice(),
            // DiceType.Echo   => new EchoDice(),
            _ => new WhiteDice() // по умолчанию белый
        };

        dice.id = id;
        return dice;
    }
}