using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScrabbleProject;

public class ScrabbleGame
{
    Game1 game;
    //player information. For now player 0 is assumed to be the player and player 1 is assumed to be the AI
    public int playerTurn = 0;
    public Tile[,] playerRacks = new Tile[2, 7];
    //2d array of characters representing each letter that's been played
    //If nothing has been played at a spot then the element is null.
    public Tile[,] board = new Tile[15, 15];

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
    public char[] tileBag;

    public ScrabbleGame(Game1 game)
    {
        this.game = game;

        int numTiles = 0;
        for(int i = 0; i < numTilesForEachLetter.Length; i++)
        {
            numTiles += numTilesForEachLetter[i];
        }

        tileBag = new char[numTiles];
        for(int i = 0; i < numTilesForEachLetter.Length; i++)
        {
            for(int j = 0; j < numTilesForEachLetter[i]; j++)
            {
                if(i < 26) 
                    tileBag[tileBag.Length - numTiles] = (char)('A' + i); //convert index to uppercase letter
                else
                    tileBag[tileBag.Length - numTiles] = '?'; //blank tiles
                
                numTiles--;
            }
        }
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        Vector2 boardPos = new Vector2(game.windowSize.X / 2 - 40 * board.GetLength(0) / 2, game.windowSize.Y / 2 - 40 * board.GetLength(1) / 2);
        for(int y = 0; y < bonuses.GetLength(0); y++)
        {
            for(int x = 0; x < bonuses.GetLength(1); x++)
            {
                _spriteBatch.DrawString(game.font, bonuses[y, x].ToString(), boardPos + new Vector2(40 * x, 40 * y), Color.Black);
            }
        }

    }
}