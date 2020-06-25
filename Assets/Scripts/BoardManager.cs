using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //public GameObject kingObj, rookObj, bishopObj, goldObj, silverObj, knightObj, lanceObj, pawnObj;
    public Piece king, rook, bishop, gold, silver, knight, lance, pawn;
    //Dictionary<string, Occupant> string2piece;
    //Dictionary<Occupant, string> piece2string;
   
    public Occupant[,] currentArrangement;
    Grid grid;
    Occupant[,] startingArrangement;
    Occupant[] backLine, midLine, frontLine, emptyLine;//Player 1 on bottom, player 2 on top
    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<Grid>();
        //string2piece.Add("King", Occupant.King);
        //piece2string.Add(Occupant.King, "King");
        //string2piece.Add("Gold General", Occupant.Gold);
        //piece2string.Add(Occupant.Gold, "Gold General");
        //string2piece.Add("Silver General", Occupant.Silver);

        frontLine = new Occupant[] { Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn };
        midLine = new Occupant[] { Occupant.None, Occupant.Bishop, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.Rook, Occupant.None };
        backLine = new Occupant[] { Occupant.Lance, Occupant.Knight, Occupant.Silver, Occupant.Gold, Occupant.King, Occupant.Gold, Occupant.Silver, Occupant.Knight, Occupant.Lance };
        emptyLine = new Occupant[] { Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None };


        startingArrangement = new Occupant[9, 9];
        

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                switch (i)
                {
                    case 0:
                        startingArrangement[i, j] = backLine[j];
                        break;
                    case 1:
                        startingArrangement[i, j] = midLine[j];
                        break;
                    case 2:
                        startingArrangement[i, j] = frontLine[j];
                        break;
                    case 6:
                        startingArrangement[i, j] = frontLine[j];
                        break;
                    case 7:
                        startingArrangement[i, j] = midLine[j];
                        break;
                    case 8:
                        startingArrangement[i, j] = backLine[j];
                        break;
                    default:
                        startingArrangement[i, j] = emptyLine[j];
                        break;



                }
            }
            if (i == 3)
            {
                System.Array.Reverse(frontLine);
                System.Array.Reverse(midLine);
                System.Array.Reverse(backLine);
            }

        }

        currentArrangement = startingArrangement;
     
            
    }

            // Update is called once per frame
            void Update()
    {
        
    }
}
