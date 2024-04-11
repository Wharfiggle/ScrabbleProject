using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScrabbleProject;

public class ScrabbleGame
{
    Game1 game;
    //list of valid english words recognized by scrabble
    public string[] allPossibleWords;

    //player information. For now player 0 is assumed to be the player and player 1 is assumed to be the AI
    public int playerTurn = 0;
    public LinkedList<RackTile>[] playerRacks = new LinkedList<RackTile>[2];
    public Vector2 rackSize = new Vector2(600, 80);
    public int rackTileSize = -1;
    private List<RackTile> incomingWord = new List<RackTile>();

    //2d array of characters representing each letter that's been played
    //If nothing has been played at a spot then the element is null.
    public Tile[,] board = new Tile[15, 15];
    public int squareSize = 35;
    public Vector2 boardPos;
    private bool enterPressed = false;

    //Info for board spot bonuses
    public enum BonusType {Letter, Word, None}
    public struct Bonus
    {
        public BonusType type;
        public int multiplier;
        public Bonus(BonusType type, int multiplier)
        {
            this.type = type;
            this.multiplier = multiplier;
        }
        public override string ToString()
        {
            string result = "";
            if(multiplier == 2)
                result += "D";
            else if(multiplier == 3)
                result += "T";
            if(type == BonusType.Letter)
                result += "L";
            else if(type == BonusType.Word)
                result += "W";
            return result;
        }
    }
    static Bonus DL = new Bonus(BonusType.Letter, 2);
    static Bonus TL = new Bonus(BonusType.Letter, 3);
    static Bonus DW = new Bonus(BonusType.Word, 2);
    static Bonus TW = new Bonus(BonusType.Word, 3);
    static Bonus nb = new Bonus(BonusType.None, 1); //no bonus
    
    //2d array of bonuses at each spot on the board.
    //If there's no bonus at a given spot, the element is nb (no bonus).
    public Bonus[,] bonuses = 
    {
        {TW,nb,nb,DL,nb,nb,nb,TW,nb,nb,nb,DL,nb,nb,TW},
        {nb,DW,nb,nb,nb,TL,nb,nb,nb,TL,nb,nb,nb,DW,nb},
        {nb,nb,DW,nb,nb,nb,DL,nb,DL,nb,nb,nb,DW,nb,nb},
        {DL,nb,nb,DW,nb,nb,nb,DL,nb,nb,nb,DW,nb,nb,DL},
        {nb,nb,nb,nb,DW,nb,nb,nb,nb,nb,DW,nb,nb,nb,nb},
        {nb,TL,nb,nb,nb,TL,nb,nb,nb,TL,nb,nb,nb,TL,nb},
        {nb,nb,DL,nb,nb,nb,DL,nb,DL,nb,nb,nb,DL,nb,nb},
        {TW,nb,nb,DL,nb,nb,nb,DW,nb,nb,nb,DL,nb,nb,TW},
        {nb,nb,DL,nb,nb,nb,DL,nb,DL,nb,nb,nb,DL,nb,nb},
        {nb,TL,nb,nb,nb,TL,nb,nb,nb,TL,nb,nb,nb,TL,nb},
        {nb,nb,nb,nb,DW,nb,nb,nb,nb,nb,DW,nb,nb,nb,nb},
        {DL,nb,nb,DW,nb,nb,nb,DL,nb,nb,nb,DW,nb,nb,DL},
        {nb,nb,DW,nb,nb,nb,DL,nb,DL,nb,nb,nb,DW,nb,nb},
        {nb,DW,nb,nb,nb,TL,nb,nb,nb,TL,nb,nb,nb,DW,nb},
        {TW,nb,nb,DL,nb,nb,nb,TW,nb,nb,nb,DL,nb,nb,TW}
    };

    //Info for tiles
                                          //A  B  C  D  E  F  G  H  I  J  K  L  M  N  O  P  Q  R  S  T  U  V  W  X  Y  Z  ?
    public int[] numTilesForEachLetter =   {9, 2, 2, 4,12, 2, 3, 2, 9, 1, 1, 4, 2, 6, 8, 2, 1, 6, 4, 6, 4, 2, 2, 1, 2, 1, 2};
    public int[] pointsForEachLetter =     {1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3,10, 1, 1, 1, 1, 4, 4, 8, 4,10, 0};
    public List<char> tileBag = new List<char>();

    //constructor, information that needs to be accessed by other objects for the rest of the game should be set here
    public ScrabbleGame(Game1 game)
    {
        this.game = game;
        
        //get total number of tiles to go in tileBag
        int numTiles = 0;
        for(int i = 0; i < numTilesForEachLetter.Length; i++)
        {
            numTiles += numTilesForEachLetter[i];
        }
        //add tiles to tileBag based on numTilesForEachLetter
        for(int i = 0; i < numTilesForEachLetter.Length; i++)
        {
            for(int j = 0; j < numTilesForEachLetter[i]; j++)
            {
                if(i < 26) 
                    tileBag.Add((char)('A' + i)); //convert index to uppercase letter
                else
                    tileBag.Add('?'); //blank tiles
            }
        }

        //set position to draw the board at
        boardPos = new Vector2(game.GetWindowSize().X / 2 - squareSize * board.GetLength(0) / 2, game.GetWindowSize().Y / 2 - squareSize * board.GetLength(1) / 2);

        for(int i = 0; i < board.GetLength(0); i++)
        {
            for(int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j] = new Tile(' ');
                board[i, j].SetPos(new Vector2(boardPos.X + squareSize * j + squareSize / 2, boardPos.Y + squareSize * i + squareSize / 2));
                board[i, j].boardSpot = new Point(i, j);
                game.AddGameObject(board[i, j]);
            }
        }

        //read text file with word data and parse into allPossibleWords array
        string[] tempWords = File.ReadAllLines("../../../Content/Collins Scrabble Words (2019).txt");
        allPossibleWords = new string[tempWords.Length - 2];
        for(int i = 2; i < tempWords.Length; i++) //chop off first two lines since they're not words
        {
            allPossibleWords[i - 2] = tempWords[i];
        }
    }

    //called during game's Initialize
    //anything done at the start of the game that also needs information from ScrabbleGame should be done here
    public void Initialize()
    {
        for(int i = 0; i < playerRacks.Length; i++)
        {
            playerRacks[i] = new LinkedList<RackTile>();
            RefillRack(i);
        }
    }

    public void RefillRack(int ind)
    {
        Random rand = new Random();
        int numNeeded = 7 - playerRacks[ind].Count;
        for(int i = 0; i < numNeeded; i++)
        {
            int rn = rand.Next(0, tileBag.Count);

            RackTile tile = new RackTile(tileBag[rn], ind);
            game.AddGameObject(tile);
            playerRacks[ind].AddLast(tile);
            tileBag.RemoveAt(rn);
        }

        if(rackTileSize == -1)
            rackTileSize = (int)playerRacks[0].First().GetSize().X;

        UpdateRackTilePositions(ind);
    }

    public void AddToIncomingWord(RackTile rt)
    {
        incomingWord.Add(rt);
        board[rt.boardSpot.X, rt.boardSpot.Y].SetLetter(rt.GetLetter());
    }
    public void RemoveFromIncomingWord(RackTile rt)
    {
        incomingWord.Remove(rt);
        board[rt.boardSpot.X, rt.boardSpot.Y].SetLetter(' ');
    }

    private Tile[] GetAdjacentTiles(Point boardSpot)
    {
        Tile[] result = new Tile[4]; //{left, right, up, down}
        if(boardSpot.X > 0)
        {
            Tile left = board[boardSpot.X - 1, boardSpot.Y];
            if(left.GetLetter() != ' ')
                result[0] = left;
        }
        if(boardSpot.X < board.GetLength(0) - 1)
        {
            Tile right = board[boardSpot.X + 1, boardSpot.Y];
            if(right.GetLetter() != ' ')
                result[1] = right;
        }
        if(boardSpot.Y > 0)
        {
            Tile up = board[boardSpot.X, boardSpot.Y - 1];
            if(up.GetLetter() != ' ')
                result[2] = up;
        }
        if(boardSpot.Y < board.GetLength(1) - 1)
        {
            Tile down = board[boardSpot.X, boardSpot.Y + 1];
            if(down.GetLetter() != ' ')
                result[3] = down;
        }
        return result;
    }

    private bool IsIncomingWordValid()
    {
        //list of tiles to be considered in validity, starts out with the same tiles as incomingWord
        List<Tile> consideredTiles = new List<Tile>();
        for(int i = 0; i < incomingWord.Count; i++)
        {
            consideredTiles.Add(incomingWord[i]);
        }

        int placedTiles = consideredTiles.Count();
        if(placedTiles == 0) //return false if no tiles placed
            return false;
        
        //return false if all tiles placed are not in a straight line
        bool xChanged = false;
        bool yChanged = false;
        for(int i = 1; i < consideredTiles.Count; i++)
        {
            if(consideredTiles[i].boardSpot.X != consideredTiles[0].boardSpot.X)
                xChanged = true;
            if(consideredTiles[i].boardSpot.Y != consideredTiles[0].boardSpot.Y)
                yChanged = true;
        }
        if(xChanged && yChanged)
            return false;

        List<LinkedList<Tile>> wordsBuilt = new List<LinkedList<Tile>>();
        List<bool> horizontal = new List<bool>();
        //iterate through all placed tiles
        for(int i = 0; i < consideredTiles.Count(); i++)
        {
            //get tiles adjacent to current placed tile
            Tile[] adjTiles = GetAdjacentTiles(consideredTiles[i].boardSpot);
            for(int j = 0; j < adjTiles.Length; j++)
            {
                if(adjTiles[j] == null)
                    continue;
                    
                //if we've already checked an adjacent tile, remove it from list of adjacent tiles
                bool found = false;
                for(int k = 0; k < wordsBuilt.Count && found == false; k++)
                {
                    foreach(Tile tile in wordsBuilt[k])
                    {
                        if(tile.boardSpot == adjTiles[j].boardSpot)
                            found = true;
                    }
                }
                if(found)
                    adjTiles[j] = null;
                //if the adjacent tile isnt in our incoming word then add it to it for word processing
                else
                {
                    found = false;
                    for(int k = 0; k < consideredTiles.Count() && found == false; k++)
                    {
                        if(consideredTiles[k].boardSpot == adjTiles[j].boardSpot)
                            found = true;
                    }
                    if(!found)
                        consideredTiles.Add(adjTiles[j]);
                }
            }

            //see what word the current placed tile is in if any
            int foundWord = -1;
            for(int j = 0; j < wordsBuilt.Count() && foundWord == -1; j++)
            {
                foreach(Tile tile in wordsBuilt[j])
                {
                    if(tile.boardSpot == consideredTiles[i].boardSpot)
                        foundWord = j;
                }
            }
            if(foundWord != -1 && horizontal[foundWord]) //current tile is part of a horizontal word
            {
                if(adjTiles[0] != null) //add left tile to start of word
                    wordsBuilt[foundWord].AddFirst(adjTiles[0]);
                if(adjTiles[1] != null) //add right tile to end of word
                    wordsBuilt[foundWord].AddLast(adjTiles[1]);
            }
            else if(foundWord != -1 && !horizontal[foundWord]) //current tile is part of a vertical word
            {
                if(adjTiles[2] != null) //add top tile to start of word
                    wordsBuilt[foundWord].AddFirst(adjTiles[2]);
                if(adjTiles[3] != null) //add bottom tile to end of word
                    wordsBuilt[foundWord].AddLast(adjTiles[3]);
            }
            if(foundWord == -1 || !horizontal[foundWord])
            {
                if(adjTiles[0] != null || adjTiles[1] != null)
                {
                    //add new horizontal word
                    wordsBuilt.Add(new LinkedList<Tile>());
                    horizontal.Add(true);
                    wordsBuilt[wordsBuilt.Count() - 1].AddLast(consideredTiles[i]);
                    if(adjTiles[0] != null) //add left tile to start of new word
                        wordsBuilt[wordsBuilt.Count() - 1].AddFirst(adjTiles[0]);
                    if(adjTiles[1] != null) //add right tile to end of new word
                        wordsBuilt[wordsBuilt.Count() - 1].AddLast(adjTiles[1]);
                }
            }
            if(foundWord == -1 || horizontal[foundWord])
            {
                if(adjTiles[2] != null || adjTiles[3] != null)
                {
                    //add new vertical word
                    wordsBuilt.Add(new LinkedList<Tile>());
                    horizontal.Add(false);
                    wordsBuilt[wordsBuilt.Count() - 1].AddLast(consideredTiles[i]);
                    if(adjTiles[2] != null) //add top tile to start of new word
                        wordsBuilt[wordsBuilt.Count() - 1].AddFirst(adjTiles[2]);
                    if(adjTiles[3] != null) //add bottom tile to end of new word
                        wordsBuilt[wordsBuilt.Count() - 1].AddLast(adjTiles[3]);
                }
            }
        }

        //construct list of forwards and backwards strings made from our placed tiles
        List<string> stringsBuilt = new List<string>();
        for(int i = 0; i < wordsBuilt.Count(); i++)
        {
            string forwards = "";
            string backwards = "";
            foreach(Tile tile in wordsBuilt[i])
            {
                forwards += tile.GetLetter();
                backwards = tile.GetLetter() + backwards;
            }
            stringsBuilt.Add(forwards);
            stringsBuilt.Add(backwards);
            Console.WriteLine(forwards);
        }    
        
        //compare list of forwards and backwards strings against all possible words, if any unique string isn't found then return false
        bool result = true;
        for(int i = 0; i < stringsBuilt.Count && result == true; i += 2)
        {
            bool found = false;
            for(int j = 0; j < allPossibleWords.Length && found == false && result == true; j++)
            {
                string possibleWord = allPossibleWords[j];
                if(possibleWord == stringsBuilt[i] || possibleWord == stringsBuilt[i + 1])
                    found = true;

                if(j == allPossibleWords.Length - 1 && found == false) //one of the spelled words was not found in the list of all possible words
                {
                    Console.WriteLine("\"" + stringsBuilt[i] + "\" or \"" + stringsBuilt[i + 1] + "\" is not a word.");
                    result = false;
                }
            }
        }

        if(result)
            Console.WriteLine("All words valid!");
        return result;
    }

    //called during game's Update
    public void Update(GameTime gameTime)
    {
        if(Keyboard.GetState().IsKeyDown(Keys.Enter)) //when enter is pressed, apply placed tiles and move to next turn
        {
            if(!enterPressed)
            {
                if(IsIncomingWordValid())
                {
                    for(int i = 0; i < incomingWord.Count(); i++)
                    {
                        Point bs = incomingWord[i].boardSpot;
                        board[bs.X, bs.Y].SetLetter(incomingWord[i].GetLetter());
                        game.RemoveGameObject(incomingWord[i]);
                        playerRacks[playerTurn].Remove(incomingWord[i]);
                    }
                    incomingWord.Clear();
                    RefillRack(playerTurn);
                    playerTurn = (playerTurn + 1) % playerRacks.Count();
                    Console.WriteLine("Player " + (playerTurn + 1) + "'s turn");
                }
                else
                {
                    for(int i = 0; i < incomingWord.Count(); i++)
                    {
                        Point bs = incomingWord[i].boardSpot;
                        try
                        {
                            board[bs.X, bs.Y].SetLetter(' ');
                        }
                        catch(IndexOutOfRangeException e)
                        {
                            Console.WriteLine("\nINVALID board position: " + bs + "\n");
                        }
                        incomingWord[i].PutBack();
                    }
                    incomingWord.Clear();
                }
            }
            enterPressed = true;
        }
        else
            enterPressed = false;
    }

    //called during game's Draw
    public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        int lineThickness = 3;
        for(int y = 0; y < bonuses.GetLength(0); y++)
        {
            for(int x = 0; x < bonuses.GetLength(1); x++)
            {
                Vector2 tilePos = boardPos + new Vector2(squareSize * x, squareSize * y);
                string bonus = bonuses[y, x].ToString();

                //draw colored rectangle
                Color squareColor;
                switch(bonus)
                {
                    case "DW": squareColor = new Color(233, 167, 151); break;
                    case "DL": squareColor = new Color(173, 187, 187); break;
                    case "TW": squareColor = new Color(160, 51, 54); break;
                    case "TL": squareColor = new Color(101, 111, 124); break;
                    default: squareColor = new Color(244, 230, 209); break;
                }
                game.DrawRect(tilePos, new Vector2(squareSize, squareSize), false, -lineThickness, true, squareColor);
                
                //draw bonus text label
                game.DrawStringCentered(game.fonts[3], bonus, tilePos + new Vector2(squareSize / 2, squareSize / 2), Color.White);
                
                //draw square border lines
                game.DrawLine(tilePos, tilePos + new Vector2(squareSize, 0), lineThickness, new Color(115, 76, 35));
                game.DrawLine(tilePos, tilePos + new Vector2(0, squareSize), lineThickness, new Color(115, 76, 35));
                if(x == bonuses.GetLength(0) - 1) //draw right lines
                    game.DrawLine(tilePos + new Vector2(squareSize, squareSize), tilePos + new Vector2(squareSize, 0), lineThickness, new Color(115, 76, 35));
                if(y == bonuses.GetLength(1) - 1) //draw bottom lines
                    game.DrawLine(tilePos + new Vector2(squareSize, squareSize), tilePos + new Vector2(0, squareSize), lineThickness, new Color(115, 76, 35));
            }
        }

        //draw racks
        game.DrawRect(pos: new Vector2(game.GetWindowSize().X / 2, rackSize.Y / 2), 
            size: rackSize, centered: true, color: Color.Brown);
        game.DrawRect(pos: new Vector2(game.GetWindowSize().X / 2, game.GetWindowSize().Y - rackSize.Y / 2), 
            size: rackSize, centered: true, color: Color.Brown);
        game.DrawRect(pos: new Vector2(rackSize.Y / 2, game.GetWindowSize().Y / 2), 
            size: new Vector2(rackSize.Y, rackSize.X), centered: true, color: Color.Brown);
        game.DrawRect(pos: new Vector2(game.GetWindowSize().X - rackSize.Y / 2, game.GetWindowSize().Y / 2), 
            size: new Vector2(rackSize.Y, rackSize.X), centered: true, color: Color.Brown);
    }

    private void UpdateRackTilePositions(int ind)
    {
        //update each RackTile based on which rack they're in and their position in that rack
        int bufferWidth = (int)((rackSize.X - playerRacks[0].Count() * rackTileSize) / (playerRacks[0].Count() + 1));
        Vector2 tilePos = new Vector2(0, 0);
        if(ind < 2) //bottom and top
        {
            tilePos.X = game.GetWindowSize().X / 2 - rackSize.X / 2;
            if(ind == 0) //bottom
                tilePos.Y = game.GetWindowSize().Y - rackSize.Y / 2.5f;
            else //top
                tilePos.Y = rackSize.Y / 2.5f;
        }
        else //left and right
        {
            tilePos.Y = game.GetWindowSize().Y / 2 - rackSize.X / 2;
            if(ind == 2) //left
                tilePos.X = rackSize.Y / 2.5f;
            else //right
                tilePos.X = game.GetWindowSize().X - rackSize.Y / 2.5f;
        }

        LinkedListNode<RackTile> node = playerRacks[ind].First;
        for(int j = 0; j < playerRacks[ind].Count(); j++)
        {
            if(ind < 2) //bottom and top, expand leftward
                node.Value.SetPos(new Vector2(tilePos.X + bufferWidth + rackTileSize / 2 + (rackTileSize + bufferWidth) * j, tilePos.Y));
            else //left and right, expand downward
                node.Value.SetPos(new Vector2(tilePos.X, tilePos.Y + bufferWidth + rackTileSize / 2 + (rackTileSize + bufferWidth) * j));
            node = node.Next;
        }
    }
}