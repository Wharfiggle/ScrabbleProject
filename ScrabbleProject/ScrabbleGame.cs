using System.Formats.Tar;

internal class ScrabbleGame
{
    //2d array of characters representing each letter that's been played
    //If nothing has been played at a spot then the element is null.
    char[,] board = new char[15, 15];

    //Info for board spot bonuses
    enum BonusType {Letter, Word, None}
    struct Bonus
    {
        public BonusType type;
        public int multiplier;
        public Bonus(BonusType type, int multiplier)
        {
            this.type = type;
            this.multiplier = multiplier;
        }
    }
    static Bonus DL = new Bonus(BonusType.Letter, 2);
    static Bonus TL = new Bonus(BonusType.Letter, 3);
    static Bonus DW = new Bonus(BonusType.Word, 2);
    static Bonus TW = new Bonus(BonusType.Word, 3);
    static Bonus nb = new Bonus(BonusType.None, 1); //no bonus
    
    //2d array of bonuses at each spot on the board.
    //If there's no bonus at a given spot, the element is nb (no bonus).
    Bonus[,] bonuses = 
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
        {nb,nb,DW,nb,nb,nb,DL,nb,DL,nb,nb,DL,DW,nb,nb},
        {nb,DW,nb,nb,nb,TL,nb,nb,nb,TL,nb,DL,nb,DW,nb},
        {TW,nb,nb,DL,nb,nb,nb,TW,nb,nb,nb,DL,nb,nb,TW}
    };

    //Info for tiles
                                   //A  B  C  D  E  F  G  H  I  J  K  L  M  N  O  P  Q  R  S  T  U  V  W  X  Y  Z  ?
    int[] numTilesForEachLetter =   {9, 2, 2, 4,12, 2, 3, 2, 9, 1, 1, 4, 2, 6, 8, 2, 1, 6, 4, 6, 4, 2, 2, 1, 2, 1, 2};
    int[] pointsForEachLetter =     {1, 3, 3, 2, 1, 4, 2, 4, 1, 8, 5, 1, 3, 1, 1, 3,10, 1, 1, 1, 1, 4, 4, 8, 4,10, 0};
    char[] tileBag;

    ScrabbleGame()
    {
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

    static void Main(string[] args)
    {
        ScrabbleGame scrabble = new ScrabbleGame();
        for(int i = 0; i < scrabble.bonuses.GetLength(0); i++)
        {
            for(int j = 0; j < scrabble.bonuses.GetLength(1); j++)
            {
                Console.Write(scrabble.bonuses[i, j].multiplier + " ");
            }
            Console.WriteLine();
        }

        Console.WriteLine("\n");

        for(int i = 0; i < scrabble.tileBag.Length; i++)
        {
            Console.Write(scrabble.tileBag[i] + " ");
        }
    }
}