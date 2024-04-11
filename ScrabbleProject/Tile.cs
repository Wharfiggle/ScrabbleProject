using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScrabbleProject;

public class Tile : GameObject
{
    protected int letterIndex; // == -1 if tile is empty
    protected Color contourColor;
    protected char letter; // == ' ' if tile is empty
    protected int pointValue; // == 0 if tile is empty
    public Point boardSpot = new Point(-1, -1);
    public LinkedList<Point> horWord = new LinkedList<Point>();
    public LinkedList<Point> vertWord = new LinkedList<Point>();
    private LinkedList<Point> backupHorWord = new LinkedList<Point>();
    private LinkedList<Point> backupVertWord = new LinkedList<Point>();

    public Tile(char letter) : base(centerOrigin: true, size: new Vector2(35, 35), color: new Color(206, 163, 119))
    {
        SetLetter(letter);
    }

    public void BackupWords()
    {
        backupHorWord = new LinkedList<Point>(horWord);
        backupVertWord = new LinkedList<Point>(vertWord);
    }
    public void RestoreWords()
    {
        horWord = new LinkedList<Point>(backupHorWord);
        vertWord = new LinkedList<Point>(backupVertWord);
    }

    //update the point value when letterIndex is updated or when game reference is set
    private void UpdatePointValue()
    {
        if(game != null && letterIndex != -1)
            pointValue = game.scrabble.pointsForEachLetter[letterIndex];
    }

    public void SetLetter(char letter)
    {
        this.letter = char.ToUpper(letter);
        if(letter == ' ')
            letterIndex = -1;
        else if(letter == '?')
            letterIndex = 26;
        else
            letterIndex = letter - 'A';

        UpdatePointValue();
    }
    public char GetLetter() { return letter; }
    public int GetPointValue() { return pointValue; }

    public override void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        base.Draw(gameTime, _spriteBatch);

        if(letterIndex != -1)
        {
            game.DrawRect(pos: drawPos, size: GetSize(), centered: false, thickness: -2, color: color);
            game.DrawRect(pos: drawPos, size: GetSize(), centered: false, thickness: 2, filled: false, color: contourColor);
            game.DrawStringCentered(font: game.fonts[3], str: letter.ToString(), pos: GetPos() - GetSize() / 20, color: contourColor);
            if(pointValue > 0)
                game.DrawStringCentered(font: game.fonts[5], str: pointValue.ToString(), pos: GetPos() + new Vector2(GetSize().X / 3, GetSize().Y / 4), color: contourColor);
        }
    }

    public override void SetGameReference(Game1 game)
    {
        base.SetGameReference(game);

        UpdatePointValue();
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