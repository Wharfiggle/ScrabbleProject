using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScrabbleProject;

public class RackTile : Tile
{
    public bool pickedUp = false;
    private int player = -1;
    
    public RackTile(char letter, int player) : base(letter)
    {
        this.player = player;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if(IsClicked() && game.scrabble.playerTurn == player)
            pickedUp = !pickedUp;
        
        if(pickedUp)
            SetPos(game.GetMousePos());
    }

    public override void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        if(pickedUp)
        {
            color.A = 100;
            contourColor.A = 100;
            Color highlight = Color.Cyan;
            highlight.A = 1;
            game.DrawRect(pos: drawPos, size: GetSize(), thickness: 5, filled:false, color: highlight);
        }
        else
        {
            color.A = 255;
            contourColor.A = 255;
        }

        base.Draw(gameTime, _spriteBatch);
    }
}