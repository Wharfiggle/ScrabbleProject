using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class RackTile : Tile
{
    public bool pickedUp = false;
    private int player = -1;
    private Vector2 putbackPos;
    private bool onBoard = false;
    
    public RackTile(char letter, int player) : base(letter)
    {
        this.player = player;
    }

    public void PutBack()
    {
        pickedUp = false;
        onBoard = false;
        SetPos(putbackPos);
        boardSpot = new Point(-1, -1);
    }

    public void AddToIncomingWord(Point boardSpot)
    {
        pickedUp = false;
        onBoard = true;
        this.boardSpot = boardSpot; //assign board spot coordinate to this rack tile
        game.scrabble.AddToIncomingWord(this); //pass rack tile to ScrabbleGame to determine if the word is valid
    }
    public void RemoveFromIncomingWord()
    {
        game.scrabble.RemoveFromIncomingWord(this); //remove this tile from word consideration
        boardSpot = new Point(-1, -1);
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
                    if(game.scrabble.board[i, j].GetLetter() != ' ')
                        continue;
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
                SetPos(game.scrabble.board[bSpot.X, bSpot.Y].GetPos()); //snap to board spot
                if(game.GetMousePressed()[0]) //if clicked while snapping to a board spot, place tile there
                    AddToIncomingWord(bSpot);
            }
            else if(IsClicked()[0]) //if clicked while not snapping to a board spot, return tile to rack
                PutBack();
        }
        else if(game.scrabble.playerTurn == player) //clicked while not picked up
        {
            if(IsClicked()[0]) //left clicked
            {
                if(onBoard)
                    RemoveFromIncomingWord();
                else
                    putbackPos = GetPos();
                pickedUp = true;
                onBoard = false;
            }
            else if(IsClicked()[1]) //right clicked
            {
                if(!onBoard)
                {
                    onBoard = true;
                    game.scrabble.AddToIncomingSwap(this);
                    putbackPos = GetPos();
                }
                else
                {
                    RemoveFromIncomingWord();
                    PutBack();
                }
            }
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

        if(!onBoard) //dont draw while on board because board will draw that tile separately anyway
            base.Draw(gameTime, _spriteBatch);
    }
}