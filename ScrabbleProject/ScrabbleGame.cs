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

    private Point[] GetAdjacentTiles(Point boardSpot)
    {
        Point[] result = new Point[4]; //{left, right, up, down}
        for(int i = 0; i < 4; i++)
        {
            result[i] = new Point(-1, -1);
        }
        if(boardSpot.X > 0)
        {
            Point bs = new Point(boardSpot.X - 1, boardSpot.Y);
            if(board[bs.X, bs.Y].GetLetter() != ' ')
                result[0] = bs;
        }
        if(boardSpot.X < board.GetLength(0) - 1)
        {
            Point bs = new Point(boardSpot.X + 1, boardSpot.Y);
            if(board[bs.X, bs.Y].GetLetter() != ' ')
                result[1] = bs;
        }
        if(boardSpot.Y > 0)
        {
            Point bs = new Point(boardSpot.X, boardSpot.Y - 1);
            if(board[bs.X, bs.Y].GetLetter() != ' ')
                result[2] = bs;
        }
        if(boardSpot.Y < board.GetLength(1) - 1)
        {
            Point bs = new Point(boardSpot.X, boardSpot.Y + 1);
            if(board[bs.X, bs.Y].GetLetter() != ' ')
                result[3] = bs;
        }
        return result;
    }

    //iterates through all tiles in incomingWord and updates all horWord's and vertWord's of tiles modified by them.
    //each modified word is then checked to be a valid word. if all are valid then the function returns true,
    //if any isnt then each modified tile is reset and the function returns false.
    private bool IsIncomingWordValid()
    {
        //list of tiles to be considered in validity, starts out with the same tiles as incomingWord
        List<Point> consideredTiles = new List<Point>();
        for(int i = 0; i < incomingWord.Count; i++)
        {
            consideredTiles.Add(incomingWord[i].boardSpot);
        }

        if(consideredTiles.Count() == 0) //return false if no tiles placed
        {
            Console.WriteLine("No tiles placed.");
            return false;
        }
        
        //return false if all tiles placed are not in a straight line
        bool xChanged = false;
        bool yChanged = false;
        for(int i = 1; i < consideredTiles.Count; i++)
        {
            if(consideredTiles[i].X != consideredTiles[0].X)
                xChanged = true;
            if(consideredTiles[i].Y != consideredTiles[0].Y)
                yChanged = true;
        }
        if(xChanged && yChanged)
        {
            Console.WriteLine("Invalid placement. Not in a straight line.");
            return false;
        }

        List<Point> modifiedTiles = new List<Point>();
        //iterate through all placed tiles
        for(int i = 0; i < consideredTiles.Count(); i++)
        {
            Point ct = consideredTiles[i];
            if(!modifiedTiles.Contains(ct))
            {
                board[ct.X, ct.Y].BackupWords();
                modifiedTiles.Add(ct);
            }
            //get tiles adjacent to current placed tile
            Point[] adjTiles = GetAdjacentTiles(ct);
            for(int j = 0; j < adjTiles.Length; j++)
            {
                Point p = adjTiles[j];
                if(p.X != -1 && !modifiedTiles.Contains(p))
                {
                    board[p.X, p.Y].BackupWords();
                    modifiedTiles.Add(p);
                }
            }

            if(board[ct.X, ct.Y].horWord.Count == 0) //no hor
            {Console.WriteLine("***no hor***: " + board[ct.X, ct.Y].GetLetter());
                if(adjTiles[0].X != -1 || adjTiles[1].X != -1) //if this tile is connected horizontally, start building horWord
                {
                    board[ct.X, ct.Y].horWord.AddFirst(ct);

                    Point p = adjTiles[0];
                    if(p.X != -1) //left exists
                    {
                        if(board[p.X, p.Y].horWord.Count == 0){ Console.WriteLine("left no hor: " + board[p.X, p.Y].GetLetter()); //no hor and left no hor
                            board[ct.X, ct.Y].horWord.AddFirst(p);}
                        else //no hor and left hor
                        {Console.WriteLine("left hor: " + board[p.X, p.Y].phorWord());
                            for(LinkedListNode<Point> node = board[p.X, p.Y].horWord.Last; node != null; node = node.Previous)
                            {
                                board[ct.X, ct.Y].horWord.AddFirst(node.Value);
                            }
                        }
                    }
                    p = adjTiles[1];
                    if(p.X != -1) //right exists
                    {
                        if(board[p.X, p.Y].horWord.Count == 0){Console.WriteLine("right no hor: " + board[p.X, p.Y].GetLetter()); //no hor and right no hor
                            board[ct.X, ct.Y].horWord.AddLast(p);}
                        else //no hor and right hor
                        {Console.WriteLine("right hor: " + board[p.X, p.Y].phorWord());
                            for(LinkedListNode<Point> node = board[p.X, p.Y].horWord.First; node != null; node = node.Next)
                            {
                                board[ct.X, ct.Y].horWord.AddLast(node.Value);
                            }
                        }
                    }
                    //now that our tile's word is accurate, update horWord for all letters in it
                    Console.WriteLine("---update horWord---: " + board[ct.X, ct.Y].phorWord());
                    foreach(Point j in board[ct.X, ct.Y].horWord)
                    {
                        if(j != ct)
                        {
                            board[j.X, j.Y].horWord.Clear();
                            board[j.X, j.Y].horWord = new LinkedList<Point>(board[ct.X, ct.Y].horWord);
                        }
                    }
                }
            }
            else //hor
            {Console.WriteLine("***hor***: " + board[ct.X, ct.Y].GetLetter());
                if(adjTiles[0].X != -1 || adjTiles[1].X != -1) //if this tile is connected horizontally, update horWord
                {
                    Point p = adjTiles[0];
                    if(p.X != -1) //left exists
                    {
                        if(board[p.X, p.Y].horWord.Count == 0){Console.WriteLine("left no hor: " + board[p.X, p.Y].GetLetter()); //hor and left no hor
                            board[ct.X, ct.Y].horWord.AddFirst(p);}
                        else
                        {Console.WriteLine("left hor: " + board[p.X, p.Y].phorWord()); //hor and left hor
                            if(!board[p.X, p.Y].horWord.Contains(ct))
                            {
                                for(LinkedListNode<Point> node = board[p.X, p.Y].horWord.Last; node != null; node = node.Previous)
                                {
                                    board[ct.X, ct.Y].horWord.AddFirst(node.Value);
                                }
                            }
                        }
                    }
                    p = adjTiles[1];
                    if(p.X != -1) //right exists
                    {
                        if(board[p.X, p.Y].horWord.Count == 0){Console.WriteLine("right no hor: " + board[p.X, p.Y].GetLetter()); //hor and right no hor
                            board[ct.X, ct.Y].horWord.AddLast(p);}
                        else
                        {Console.WriteLine("right hor: " + board[p.X, p.Y].phorWord()); //hor and right hor
                            if(!board[p.X, p.Y].horWord.Contains(ct))
                            {
                                for(LinkedListNode<Point> node = board[p.X, p.Y].horWord.First; node != null; node = node.Next)
                                {
                                    board[ct.X, ct.Y].horWord.AddLast(node.Value);
                                }
                            }
                        }
                    }
                    //now that our tile's word is accurate, update horWord for all letters in it
                    Console.WriteLine("---update horWord---: " + board[ct.X, ct.Y].phorWord());
                    foreach(Point j in board[ct.X, ct.Y].horWord)
                    {
                        if(j != ct)
                        {
                            board[j.X, j.Y].horWord.Clear();
                            board[j.X, j.Y].horWord = new LinkedList<Point>(board[ct.X, ct.Y].horWord);
                        }
                    }
                }
            }
            if(board[ct.X, ct.Y].vertWord.Count == 0) //no vert
            {Console.WriteLine("***no vert***: " + board[ct.X, ct.Y].GetLetter());
                if(adjTiles[2].X != -1 || adjTiles[3].X != -1) //if this tile is connected vertically, start building vertWord
                {
                    board[ct.X, ct.Y].vertWord.AddFirst(ct);

                    Point p = adjTiles[2];
                    if(p.X != -1) //up exists
                    {
                        if(board[p.X, p.Y].vertWord.Count == 0){Console.WriteLine("up no vert: " + board[p.X, p.Y].GetLetter()); //no vert and up no vert
                            board[ct.X, ct.Y].vertWord.AddFirst(p);}
                        else //no vert and up vert
                        {Console.WriteLine("up vert: " + board[p.X, p.Y].pvertWord());
                            for(LinkedListNode<Point> node = board[p.X, p.Y].vertWord.Last; node != null; node = node.Previous)
                            {
                                board[ct.X, ct.Y].vertWord.AddFirst(node.Value);
                            }
                        }
                    }
                    p = adjTiles[3];
                    if(p.X != -1) //down exists
                    {
                        if(board[p.X, p.Y].vertWord.Count == 0){Console.WriteLine("down no vert: " + board[p.X, p.Y].GetLetter()); //no vert and down no vert
                            board[ct.X, ct.Y].vertWord.AddLast(p);}
                        else //no vert and down vert
                        {Console.WriteLine("down vert: " + board[p.X, p.Y].pvertWord());
                            for(LinkedListNode<Point> node = board[p.X, p.Y].vertWord.First; node != null; node = node.Next)
                            {
                                board[ct.X, ct.Y].vertWord.AddLast(node.Value);
                            }
                        }
                    }
                    //now that our tile's word is accurate, update vertWord for all letters in it
                    Console.WriteLine("---update vertWord---: " + board[ct.X, ct.Y].pvertWord());
                    foreach(Point j in board[ct.X, ct.Y].vertWord)
                    {
                        if(j != ct)
                        {
                            board[j.X, j.Y].vertWord.Clear();
                            board[j.X, j.Y].vertWord = new LinkedList<Point>(board[ct.X, ct.Y].vertWord);
                        }
                    }
                }
            }
            else //vert
            {Console.WriteLine("***vert***: " + board[ct.X, ct.Y].GetLetter());
                if(adjTiles[2].X != -1 || adjTiles[3].X != -1) //if this tile is connected vertically, update vertWord
                {
                    Point p = adjTiles[2];
                    if(p.X != -1) //up exists
                    {
                        if(board[p.X, p.Y].vertWord.Count == 0){Console.WriteLine("up no vert: " + board[p.X, p.Y].GetLetter()); //vert and up no vert
                            board[ct.X, ct.Y].vertWord.AddFirst(p);}
                        else
                        {Console.WriteLine("up vert: " + board[p.X, p.Y].pvertWord());//vert and up vert
                            if(!board[p.X, p.Y].vertWord.Contains(ct))
                            {
                                for(LinkedListNode<Point> node = board[p.X, p.Y].vertWord.Last; node != null; node = node.Previous)
                                {
                                    board[ct.X, ct.Y].vertWord.AddFirst(node.Value);
                                }
                            }
                        }
                    }
                    p = adjTiles[3];
                    if(p.X != -1) //down exists
                    {
                        if(board[p.X, p.Y].vertWord.Count == 0){Console.WriteLine("down no vert: " + board[p.X, p.Y].GetLetter()); //vert and down no vert
                            board[ct.X, ct.Y].vertWord.AddLast(p);}
                        else
                        {Console.WriteLine("down vert: " + board[p.X, p.Y].pvertWord());//vert and down vert
                            if(!board[p.X, p.Y].vertWord.Contains(ct))
                            {
                                for(LinkedListNode<Point> node = board[p.X, p.Y].vertWord.First; node != null; node = node.Next)
                                {
                                    board[ct.X, ct.Y].vertWord.AddLast(node.Value);
                                }
                            }
                        }
                    }
                    //now that our tile's word is accurate, update vertWord for all letters in it
                    Console.WriteLine("---update vertWord---: " + board[ct.X, ct.Y].pvertWord());
                    foreach(Point j in board[ct.X, ct.Y].vertWord)
                    {
                        if(j != ct)
                        {
                            board[j.X, j.Y].vertWord.Clear();
                            board[j.X, j.Y].vertWord = new LinkedList<Point>(board[ct.X, ct.Y].vertWord);
                        }
                    }
                }
            }
        }

        //get list of unique words modified
        List<LinkedList<Point>> wordsBuilt = new List<LinkedList<Point>>();
        for(int i = 0; i < consideredTiles.Count; i++)
        {
            Tile tile = board[consideredTiles[i].X, consideredTiles[i].Y];
            for(int j = 0; j < 2; j++)
            {
                LinkedList<Point> word;
                if(j == 0)
                    word = tile.horWord;
                else
                    word = tile.vertWord;

                if(word.Count > 0)
                {
                    bool found = false;
                    for(int k = 0; k < wordsBuilt.Count && !found; k++)
                    {
                        if(word.Count != wordsBuilt[k].Count)
                            continue;
                        found = true;
                        LinkedListNode<Point> wordNode = word.First;
                        LinkedListNode<Point> listNode = wordsBuilt[k].First;
                        while(wordNode != null && listNode != null && found)
                        {
                            if(wordNode.Value != listNode.Value)
                                found = false;
                            wordNode = wordNode.Next;
                            listNode = listNode.Next;
                        }
                    }
                    if(found == false)
                        wordsBuilt.Add(word);
                }
            }
        }

        //check if at least one of the tiles in the words built was already on the board or is on the center tile
        //reset modified tiles and return false otherwise
        bool result = false;
        for(int i = 0; i < wordsBuilt.Count && result == false; i++)
        {
            foreach(Point p in wordsBuilt[i])
            {
                if(!consideredTiles.Contains(p) || p == new Point(board.GetLength(0) / 2, board.GetLength(1) / 2))
                    result = true;
            }
        }
        if(!result)
        {
            Console.WriteLine("Invalid placement.");
            for(int i = 0; i < modifiedTiles.Count; i++)
            {
                board[modifiedTiles[i].X, modifiedTiles[i].Y].RestoreWords();
            }
            return false;
        }

        //construct list of forwards and backwards strings made from our placed tiles
        List<string> stringsBuilt = new List<string>();
        for(int i = 0; i < wordsBuilt.Count(); i++)
        {
            string forwards = "";
            string backwards = "";
            foreach(Point p in wordsBuilt[i])
            {
                forwards = forwards + board[p.X, p.Y].GetLetter();
                backwards = board[p.X, p.Y].GetLetter() + backwards;
            }
            stringsBuilt.Add(forwards);
            stringsBuilt.Add(backwards);
            Console.WriteLine("\"" + forwards + "\" or \"" + backwards + "\"");
        }    
        
        //compare list of forwards and backwards strings against all possible words, if any unique string isn't found then return false
        result = true;
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
        {
            Console.WriteLine("All words valid!");
            AddPointsFromWords(wordsBuilt);
        }
        else
        {
            for(int i = 0; i < modifiedTiles.Count; i++)
            {
                board[modifiedTiles[i].X, modifiedTiles[i].Y].RestoreWords();
            }
        }

        return result;
    }

    //takes words built from IsIncomingWordValid, calculates points, and gives them to the current player
    private void AddPointsFromWords(List<LinkedList<Point>> wordsBuilt)
    {

    }

    //called during game's Update
    public void Update(GameTime gameTime)
    {
        for(int i = 0; i < 26; i++)
        {
            if(Keyboard.GetState().IsKeyDown((Keys)(65 + i)))
            {
                foreach(Tile t in playerRacks[playerTurn])
                {
                    if(t.boardSpot == new Point(-1, -1))
                    {
                        t.SetLetter((char)('A' + i));
                        break;
                    }
                }
            }
        }
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
                        board[bs.X, bs.Y].SetLetter(' ');
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