public class Tile : GameObject
{
    char letter;
    public Tile(char letter) : base()
    {
        this.letter = char.ToUpper(letter);
    }

    public static bool operator==(Tile t, char c)
    {
        return t.letter == char.ToUpper(c);
    }
    public static bool operator!=(Tile t, char c)
    {
        return t.letter != char.ToUpper(c);
    }
    public override string ToString()
    {
        return letter.ToString();
    }
}