using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScrabbleProject;

public class Tile : GameObject
{
    protected int letterIndex;
    protected Color contourColor;
    public char letter;
    public int pointValue;
    
    public Tile(char letter) : base(centerOrigin: true, size: new Vector2(35, 35), color: new Color(206, 163, 119))
    {
        this.letter = char.ToUpper(letter);
        if(letter == '?')
            letterIndex = 26;
        else
            letterIndex = letter - 'A';
    }

    public override void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        base.Draw(gameTime, _spriteBatch);

        game.DrawRect(pos: drawPos, size: GetSize(), centered: false, thickness: -2, color: color);
        game.DrawRect(pos: drawPos, size: GetSize(), centered: false, thickness: 2, filled: false, color: contourColor);
        game.DrawStringCentered(font: game.fonts[3], str: letter.ToString(), pos: GetPos() - GetSize() / 20, color: contourColor);
        if(pointValue > 0)
            game.DrawStringCentered(font: game.fonts[5], str: pointValue.ToString(), pos: GetPos() + new Vector2(GetSize().X / 3, GetSize().Y / 4), color: contourColor);
    }

    public override void SetGameReference(Game1 game)
    {
        base.SetGameReference(game);

        pointValue = game.scrabble.pointsForEachLetter[letterIndex];
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