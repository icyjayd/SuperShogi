using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //public GameObject kingObj, rookObj, bishopObj, goldObj, silverObj, knightObj, lanceObj, pawnObj;
    public Piece king, rook, bishop, gold, silver, knight, lance, pawn;
    public Renderer boardChunk;
    //Dictionary<string, Occupant> string2piece;
    //Dictionary<Occupant, string> piece2string;
    Dictionary<Occupant, Piece> pieces;
   
    public Occupant[,] currentArrangement;
    public Color[] boardColors, playerColors;
    Grid grid;
    Occupant[,] startingArrangement;
    Occupant[] backLine, midLine, frontLine, emptyLine;//Player 1 on bottom, player 2 on top
    List<Piece>[] pieceGroups;//0 = board , 1 = player 1 barracks, 2 = player 2 barracks
    int boardY;
    // Start is called before the first frame update
    void Start()
    {
        pieces = new Dictionary<Occupant, Piece>();
        grid = GetComponent<Grid>();
        //string2piece.Add("King", Occupant.King);
        //piece2string.Add(Occupant.King, "King");
        //string2piece.Add("Gold General", Occupant.Gold);
        //piece2string.Add(Occupant.Gold, "Gold General");
        //string2piece.Add("Silver General", Occupant.Silver);
        pieceGroups = new List<Piece>[3];
        for (int i = 0; i < 3; i++){
            pieceGroups[i] = new List<Piece>();
        }
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

        boardY = grid.WorldToCell(transform.position).y;
        pieces.Add(Occupant.King, king);
        pieces.Add(Occupant.Rook, rook);
        pieces.Add(Occupant.Bishop, bishop);
        pieces.Add(Occupant.Gold, gold);
        pieces.Add(Occupant.Silver, silver);
        pieces.Add(Occupant.Knight, knight);
        pieces.Add(Occupant.Lance, lance);
        pieces.Add(Occupant.Pawn, pawn);

        int count = pieceGroups[0].Count;
        Renderer chunk;
        for (int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                chunk = Instantiate(boardChunk, parent: this.transform, position: grid.CellToWorld(new Vector3Int(i, boardY - 1, j)), rotation: Quaternion.identity);

                chunk.material.SetColor("_Color", boardColors[(i + j) % 2]);

                if (startingArrangement[i, j] != Occupant.None)
                {
                    pieceGroups[0].Add(Instantiate(pieces[startingArrangement[i, j]], position: grid.CellToWorld(new Vector3Int(i, boardY, j)), rotation: Quaternion.identity));
                    count = pieceGroups[0].Count;
                    print(count);
                    pieceGroups[0][count-1].transform.parent = this.transform;
                    pieceGroups[0][count-1].CurrentPosition = new Vector2Int(i, j);
                    pieceGroups[0][count -1].Owner = 1;
                    pieceGroups[0][count - 1].transform.RotateAround(pieceGroups[0][count - 1].transform.position, pieceGroups[0][count - 1].transform.up, -90);

                    if (i > 3)
                    {
                        pieceGroups[0][count-1].transform.RotateAround(pieceGroups[0][count-1].transform.position, pieceGroups[0][count-1].transform.up, 180);
                        pieceGroups[0][count-1].Owner = 2;
                    }
                    pieceGroups[0][count - 1].SetColor(playerColors[(pieceGroups[0][count - 1].Owner - 1) % 2]);
                } 
            }
            
        }
    }

            // Update is called once per frame
            void Update()
    {
        
    }
}
