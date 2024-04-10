using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScrabbleProject;

public class RackTile : Tile
{
    public bool pickedUp = false;
    private int player = -1;
    private Vector2 putbackPos;
    public Point boardSpot = new Point(-1, -1);
    
    public RackTile(char letter, int player) : base(letter)
    {
        this.player = player;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        if(pickedUp)
        {
            SetPos(game.GetMousePos());

            //find closest tile to picked up rack tile and see if it's close enough to snap to
            float minDistance = 99999999999999;
            Point bSpot = new Point(-1, -1);
            for(int i = 0; i < game.scrabble.board.GetLength(0); i++)
            {
                for(int j = 0; j < game.scrabble.board.GetLength(1); j++)
                {
                    float dist = (game.scrabble.board[i, j].GetPos() - GetPos()).Length();
                    if(dist < minDistance)
                    {
                        minDistance = dist;
                        bSpot = new Point(i, j);
                    }
                }
            }

            if(minDistance < GetSize().X) //if there is a valid board spot to snap to
            {
                boardSpot = bSpot; //assign board spot coordinate to this rack tile
                SetPos(game.scrabble.board[boardSpot.X, boardSpot.Y].GetPos()); //snap to board spot
                if(IsClicked()) //if clicked while snapping to a board spot, place tile there
                {
                    pickedUp = false;
                    game.scrabble.incomingWord.Add(this); //pass rack tile to ScrabbleGame to determine if the word is valid
                }
            }
            else if(IsClicked()) //if clicked while not snapping to a board spot, return tile to rack
            {
                pickedUp = false;
                SetPos(putbackPos);
                boardSpot = new Point(-1, -1);
            }
        }
        else if(IsClicked() && game.scrabble.playerTurn == player) //clicked while not picked up
        {
            pickedUp = true;
            if(boardSpot == new Point(-1, -1)) //if in rack when picked up
                putbackPos = GetPos();
            else //if on board when picked up
                game.scrabble.incomingWord.Remove(this); //remove this tile from word consideration
        }
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