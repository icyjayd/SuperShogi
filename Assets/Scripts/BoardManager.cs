using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public List<Piece> activePieces;
    Text boardText;
    List<Occupant> basePieces = new List<Occupant> { Occupant.Rook, Occupant.Bishop, Occupant.Silver, Occupant.Knight, Occupant.Pawn };

    static List<Occupant> forcedPromotions = new List<Occupant> { Occupant.Pawn, Occupant.Lance, Occupant.Knight };
    int boardLength = 9;
    //public GameObject kingObj, rookObj, bishopObj, goldObj, silverObj, knightObj, lanceObj, pawnObj;
    public Piece king, rook, bishop, gold, silver, knight, lance, pawn;
    public BoardSpot boardChunk;
    public GameObject promotionPanel;
    BoardSpot[,] boardSpots;
    //Dictionary<string, Occupant> string2piece;
    //Dictionary<Occupant, string> piece2string;
    Dictionary<Occupant, Piece> pieces;
    Piece[,] boardArrangement;
    [SerializeField]
    Piece selectedPiece;
    bool piecePromotable = false;
    bool promote = false;
    public Occupant[,] currentArrangement;
    public int[,] currentOwners;
    public Color[] boardColors, playerColors, selectedBoardColors, selectedPlayerColors;
    Grid grid;
    Occupant[,] startingArrangement;
    Occupant[] backLine, midLine, frontLine, emptyLine;//Player 1 on bottom, player 2 on top
    List<Piece>[] pieceGroups;//0 = board , 1 = player 1 barracks, 2 = player 2 barracks, 3 = hypotheticals 
    List<Piece> allPieces;
   // List<Piece> boardPieces;
    int boardY;
    [SerializeField]
    Grid[] barracks;
    [SerializeField]
    public int turn = 1;
    public int playerTurn;

    //General turn structure:
    //on the first turn, the player whose turn it is (player 1) Selects a piece
    //OnPieceSelect, piece sends message to board
    //if the piece belongs to the player, mark Piece.Selected as true
    //if selected, highlight all spaces containing a piece's possible moves;
    //if a selected space is within the selected piece's moves, make the move (the legal moves will have been calculated and edited if in checkmate) and end the turn
    //on the end of the turn, ask user for promotion if applicable (coroutine CheckPromotion)
    //once user verifies promotion check for kings in check and checkmate (both of which will recalculate the move)
    // Start is called before the first frame update
    void Start()
    {
        foreach (Text t in GetComponentsInChildren<Text>())
        {
            if (t.name == "BoardText")
            {
                boardText = t;
            }
        }


        boardArrangement = new Piece[9, 9];
        boardLength = 9;
        pieces = new Dictionary<Occupant, Piece>();
        grid = GetComponent<Grid>();
        allPieces = new List<Piece>();
        //string2piece.Add("King", Occupant.King);
        //piece2string.Add(Occupant.King, "King");
        //string2piece.Add("Gold General", Occupant.Gold);
        //piece2string.Add(Occupant.Gold, "Gold General");
        //string2piece.Add("Silver General", Occupant.Silver);
        pieceGroups = new List<Piece>[3];
        for (int i = 0; i < 3; i++) {
            pieceGroups[i] = new List<Piece>();
        }
        frontLine = new Occupant[] { Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn, Occupant.Pawn };
        midLine = new Occupant[] { Occupant.None, Occupant.Bishop, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.Rook, Occupant.None };
        backLine = new Occupant[] { Occupant.Lance, Occupant.Knight, Occupant.Silver, Occupant.Gold, Occupant.King, Occupant.Gold, Occupant.Silver, Occupant.Knight, Occupant.Lance };
        emptyLine = new Occupant[] { Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None, Occupant.None };


        startingArrangement = new Occupant[9, 9];
        boardSpots = new BoardSpot[9, 9];


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
        pieces.Add(Occupant.None, null);
        int count = pieceGroups[0].Count;
        BoardSpot chunk;
        Vector2Int currentPosition;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                currentPosition = new Vector2Int(i, j);
                chunk = Instantiate(boardChunk, parent: this.transform, position: grid.CellToWorld(new Vector3Int(i, boardY - 1, j)), rotation: Quaternion.identity);

                chunk.SetColor(boardColors[(i + j) % 2]);
                chunk.Position = currentPosition;
                boardSpots[i, j] = chunk;

                if (startingArrangement[i, j] != Occupant.None)
                {
                    pieceGroups[0].Add(Instantiate(pieces[startingArrangement[i, j]], position: grid.CellToWorld(new Vector3Int(i, boardY, j)), rotation: Quaternion.identity));
                    count = pieceGroups[0].Count;


                    //print(count);
                    pieceGroups[0][count - 1].transform.parent = this.transform;
                    pieceGroups[0][count - 1].Move(currentPosition);
                    pieceGroups[0][count - 1].Owner = 1;
                    pieceGroups[1].Add(pieceGroups[0][count - 1]);
                    pieceGroups[0][count - 1].transform.RotateAround(pieceGroups[0][count - 1].transform.position, pieceGroups[0][count - 1].transform.up, -90);

                    if (i > 3)
                    {
                        pieceGroups[0][count - 1].transform.RotateAround(pieceGroups[0][count - 1].transform.position, pieceGroups[0][count - 1].transform.up, 180);
                        pieceGroups[0][count - 1].Owner = 2;
                        pieceGroups[2].Add(pieceGroups[0][count - 1]);
                    }
                    pieceGroups[0][count - 1].SetColor(playerColors[(pieceGroups[0][count - 1].Owner - 1) % 2]);
                }
            }

        }

        boardArrangement = ArrangedBoard();
      
        foreach (Piece p in pieceGroups[0])
        {
            allPieces.Add(p);
            p.CalculateLegalMoves();
        }

        ///testing
       // boardPieces = pieceGroups[0];
    }

    Piece[,] ArrangedBoard()
    {
        Piece[,] newBoard = new Piece[9, 9];
        foreach (Piece p in pieceGroups[0])
        {
            //print(System.String.Format("current piece: {0}, position: {1})", p.ID, p.CurrentPosition));
            if (p.CurrentPosition.x != -1)//oop check
            {
                newBoard[p.CurrentPosition.x, p.CurrentPosition.y] = p;
            }
        }
        return newBoard;
    }


    void UpdateText()
    {
        string newBoard = "";
        for (int i = 0; i < boardLength; i++)
        {
            if (i > 0)
            {
                newBoard = newBoard + "\n\n";
            }
            for (int j = 0; j < boardLength; j++)
            {
                if (boardArrangement[i, j])
                {
                    newBoard = newBoard + System.String.Format("[{0}] ", boardArrangement[i, j].ID.ToString() + boardArrangement[i, j].Owner.ToString() + System.String.Concat(Enumerable.Repeat("-", (6 - (boardArrangement[i, j].ID.ToString().Length)))));
                }
                else
                {
                    newBoard = newBoard + "[-------] ";
                }
            }
        }
        boardText.text = newBoard;
    }
    int CurrentPlayerTurn()
    {

        //print(turn % 2);
        return (int)Mathf.Pow(2, (turn + 1) % 2);

    }
    Piece GetCurrentKing()
    {
        foreach (Piece p in pieceGroups[0])
        {
            if (p.pieceData.id == Occupant.King && p.Owner == CurrentPlayerTurn())
            {
                return p;
            }
        }

        Debug.LogError("King not found!");
        return null;

    }
    bool KingInCheck()
    {
        Piece currentKing = GetCurrentKing();
        //print(currentKing.Owner);
        //find the king
        //for each piece on the board, check if there are ANY moves that can target the king - if so, return true;

        if (currentKing != null)//null check
        {


            foreach (Piece p in pieceGroups[0])//scan through each piece
            {

                if (p.Owner != currentKing.Owner)//for all pieces that could possibly capture the king
                {
                    if (p.pieceData.name == "Bishop")
                    {
                        print(p.name + p.moves.Count());

                    }
                    //print(p.ToString() + p.Owner.ToString());
                    //print(currentKing.Owner);

                    EditMoves(p);
                    //edit their moves

                    foreach (Vector2Int move in p.moves)
                    {
                        if (p.pieceData.name == "Bishop")
                        {
                            print(p.pieceData.name + " " + move);
                        }
                        if (PieceInDanger(p, king, move))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    p.CalculateLegalMoves();
                }

            }

        }
        return false;
    }

    //bool PieceProtected(Piece attacker, Piece target, Vector2Int move, Piece defender=null)
    //{
    //    return true;
    //    //after moving the defender or target if defender is null, check if piece is still in danger
    //}
    //Piece[,] Move(Piece p, Vector2Int move)
    //{
    //    Piece[,] newBoard = boardArrangement.Clone()
    //    //this function assumes move is a valid move 
    //    if(LineOfSight(p, move))
    //    {
    //        if(space)
    //    }

    //}
    Vector2Int OneMoveCloser(Vector2Int move)
    {
        int signX = 0, signY =0;
        if(move.x != 0)
        {
            signX = move.x / Mathf.Abs(move.x);
        }
        if (move.y != 0)
        {
            signY = move.y / Mathf.Abs(move.y);
        }
        return move - new Vector2Int(move.x - 1 * signX, move.y - 1 * signY);

    }


    public void OnPieceClick(Piece p)//this is the receiver of the Piece's SendMessageUpward
    {
        if (p.Owner == CurrentPlayerTurn())
        {
            if (promotionPanel == null || promotionPanel.activeInHierarchy == false)
            {
                
                    SelectPiece(p);
                
            }
        }
        else
        {
            if (selectedPiece != null)
            {

                if (selectedPiece.CurrentPosition.x != -1)
                {
                    foreach (Vector2Int move in selectedPiece.moves)
                    {

                        if (p.CurrentPosition == selectedPiece.CurrentPosition + move)
                        {

                            bool LOS = true;
                            if (Mathf.Abs(move.x)>1 || Mathf.Abs(move.y)> 1)
                            {
                                print(move);
                                LOS = LineOfSight(selectedPiece,  move);//OneMoveCloser(move));
                            }
                            //print(LOS);
                            if (LOS)
                            {
                                foreach (Vector2Int oldMove in selectedPiece.moves)
                                {
                                    boardSpots[selectedPiece.CurrentPosition.x + oldMove.x, selectedPiece.CurrentPosition.y + oldMove.y].ResetColor();
                                }

                                //print("capturing");

                                //print(move);
                                Capture(selectedPiece, p, false);
                                selectedPiece.SetColor(playerColors[(selectedPiece.Owner - 1) % 2]);
                                StartCoroutine("EndTurn");
                                break;
                            }
                        }
                    }
                }
       
            }

        }
    }

    bool SpaceContainsEnemy(Vector2Int space)
    {
        if (boardArrangement[space.x, space.y] != null)
        {
          if(boardArrangement[space.x, space.y].Owner != CurrentPlayerTurn())
            {
                return true;
            }
        }
        return false;
    }
    void EditMoves(Piece p)
    {
    
        p.CalculateLegalMoves();
        //if (p.pieceData.name == "Bishop")
        //{
        //    print(p.name + p.moves.Count());

        //}
        List<Vector2Int> newMoves = new List<Vector2Int>();
        if (p.Owner == CurrentPlayerTurn())
        {
            foreach (Vector2Int move in p.moves)
            {
                print(move);
                bool LOS = true;

                if (Mathf.Abs(move.x) > 1 || Mathf.Abs(move.y) > 1)
                {

                    LOS = LineOfSight(p, OneMoveCloser(move));
                    if (LOS)
                    {
                        LOS = (SpaceContainsEnemy(move + p.CurrentPosition) || SpaceEmpty(move + p.CurrentPosition));
                    }
                }
                else
                {
                    LOS = (SpaceContainsEnemy(move + p.CurrentPosition) || SpaceEmpty(move + p.CurrentPosition));
                }

                if (LOS)
                {
                    newMoves.Add(move);

                }

            }


        }
        p.moves = newMoves;
        if(p.pieceData.name == "Bishop")
        {
            print(p.ToString() + p.Owner.ToString() + " " + p.moves.Count());

        }
    }
    void SelectPiece(Piece p)
    {

        if (selectedPiece)
        {
            foreach (Vector2Int move in selectedPiece.moves)
            {
                boardSpots[selectedPiece.CurrentPosition.x + move.x, selectedPiece.CurrentPosition.y + move.y].ResetColor();
            }
            selectedPiece.SetColor(playerColors[(selectedPiece.Owner - 1) % 2]);

        }
        selectedPiece = p;
        selectedPiece.SetColor(selectedPlayerColors[0]);
        foreach (Vector2Int move in p.moves) {
            if (LineOfSight(p, move))
            {
                boardSpots[p.CurrentPosition.x + move.x, p.CurrentPosition.y + move.y].SetColor(selectedBoardColors[0]);
            }
        }//do some code to highlight all the legal move spaces
    }
    void ResetBoardColors()
    {
        foreach(BoardSpot b in boardSpots)
        {
            b.ResetColor();
        }
    }
    void OnSpaceClick(Vector2Int position)//this is the receiver of the BoardSpace's SendMessageUpward
    {
        if (selectedPiece != null) 
        {
            //print(selectedPiece);
            selectedPiece.SetColor(playerColors[(selectedPiece.Owner - 1) % 2]);


            foreach (Vector2Int move in selectedPiece.moves)
            {

                
              // print(selectedPiece);

                Vector2Int newPos = selectedPiece.CurrentPosition + move;

                //if (newPos.x < 9 && newPos.x >= 0 && newPos.y < 9 && newPos.y >= 0)
                //{
                //    if (selectedPiece.id == Occupant.Rook)
                //    {
                //        print(System.String.Format("Rook position: {0}", selectedPiece.CurrentPosition));

                //        print(System.String.Format("Rook move: {0}", move));
                //        print(System.String.Format("new position: {0}", newPos));

                //    }
                //    boardSpots[newPos.x, newPos.y].ResetColor();
                //}
                ResetBoardColors();
                if (newPos == position && SpaceEmpty(position))
                {

                    Move(selectedPiece, move, sim:false);
                    break;
                   
                }
            }

        }



        selectedPiece = null;
       
    }

    bool Checkmate()
    {

        List<Piece> playerPieces = pieceGroups[CurrentPlayerTurn()];
        
        Piece[,] originalBoard = ArrangedBoard();
//        System.Array.Copy(boardArrangement, originalBoard, boardLength * boardLength);
        foreach (Piece p in playerPieces)
        {
            if (p.Owner == CurrentPlayerTurn())
            {
                p.CalculateLegalMoves();
                List<Vector2Int> validMoves = new List<Vector2Int>();
                foreach (Vector2Int move in p.moves)
                {
                    System.Array.Copy(boardArrangement, originalBoard, boardLength * boardLength);

                    Vector2Int lastPos = p.CurrentPosition;



                    Move(p, move, sim: true);
                    //boardArrangement = ArrangedBoard();
                    //UpdateBoardArrangement();
                    //check if king is still in check after simulating each move and if so add it to that piece's valid moveset
                    if (!KingInCheck())
                    {
                        validMoves.Add(move);
                    }
                    //System.Array.Copy(originalBoard, boardArrangement, boardLength * boardLength);
                    if (p.CurrentPosition != lastPos)
                    {
                        p.Move(p.LastPosition);
                    }
                }
                p.moves = validMoves;
                if(validMoves.Count > 0)
                {
                    return false;
                }

            }
        }
        
        return true;
    }

    void UpdatePlayerPieces()
    {
        List<Piece>[] playerPieces = new List<Piece>[2];
        for (int i = 0; i < 2; i++)
        {
            playerPieces[i] = new List<Piece>();
        }
        foreach (Piece p in pieceGroups[0])
        {
            playerPieces[p.Owner - 1].Add(p);
        }

        pieceGroups[1] = playerPieces[0];
        pieceGroups[2] = playerPieces[1];

    }

    bool SpaceEmpty(Vector2Int space, int group = 0)//used to determine if piece p can enter a given space
    {

        //if(boardArrangement[space.x, space.y] == null)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        //if (selectedPiece)
        //{
        //    //print(System.String.Format("Checking emptiness of space {0}", space));
        //}
        //print(pieceGroups[group].Count);
        for (int j = 0; j < pieceGroups[group].Count; j++)
        {

            if (pieceGroups[group][j].CurrentPosition == space)
            {
                //print(System.String.Format("Piece: {0} Current Position: {1} space: {2}", pieceGroups[group][j], pieceGroups[group][j].CurrentPosition, space));

               
                //print("space " + space.ToString());
                //print(System.String.Format("Space {0} is not empty", space));
                return false;
            }
        }
        return true;

    }
    void UpdateBoardArrangement()

    {
        Occupant[,] ids = new Occupant[9, 9];
        int[,] owners = new int[9, 9];
        Piece[,] newBoard = new Piece[boardLength, boardLength];
        //update new board with current positions
        foreach (Piece p in pieceGroups[0])
        {
            if (p.CurrentPosition.x != -1)
            {
                //print(System.String.Format("{0}{1}: {2}", p.name, p.Owner, p.CurrentPosition));
                newBoard[p.CurrentPosition.x, p.CurrentPosition.y] = p;
            }
        }
        boardArrangement = newBoard;

        ////remove all pieces that have been taken;
        //for (int i = 0; i < boardLength; i++)
        //{
        //    for (int j = 0; j < boardLength; j++)
        //    {
        //        if (boardArrangement[i, j] != null)
        //        {
        //            Piece p = boardArrangement[i, j];
        //            p = pieceGroups[0][pieceGroups[0].IndexOf(p)];
        //            if (p.CurrentPosition.x == -1)
        //            {
        //                boardArrangement[i, j] = null;
        //            }
        //        }
        //    }
        //}


    }
    (bool, bool) Promotable(Piece p, Vector2Int newPos)
    {
        switch (p.Owner)
        {
            case 1:
                if (newPos.x >= 6)
                {
                    return (true, true);
                }
                break;
            case 2:
                if (newPos.x <= 2)
                {
                    // print(System.String.Format("promoting {0} owned by player {1}", p.name, p.Owner));
                    return (true, true);
                }
                break;
            default:
                return (false, false);

        }
        return (false, false);

    }
    

    
    void Move(Piece p, Vector2Int move, bool sim = true) //moves the internal board arrangement without redrawing the board, with promotions included
    {
        if(MoveLegal(p, move)){
            //if (p.pieceData.name == "Bishop")
            //{
            //    print(System.String.Format("Name {0} current pos {1} move {2}", p.name + p.Owner.ToString(), p.CurrentPosition, move));
            //}
            if (p.CurrentPosition.x != -1)
            {

                Vector2Int newPos = p.CurrentPosition + move;
                (piecePromotable, p.Promotable) = Promotable(p, newPos);
                if (SpaceEmpty(newPos))
                {
                    //print(System.String.Format("Before: {0}{1}'s current position: {2}", p.name, p.Owner, p.CurrentPosition));
                    p.Move(newPos);
                    //print(System.String.Format("After: {0}{1}'s current position: {2}", p.name, p.Owner, p.CurrentPosition));
                }
                //else
                //{
                //    //print(System.String.Format("Piece {0} at {1} is attempting to attempting to capture", p.name, newPos));
                //    Capture(p, boardArrangement[newPos.x, newPos.y], sim);

                //}

                //    if (promotion)
                //    {
                //        p.CurrentPosition = p.CurrentPosition + move;
                //        if (promotion || (forcedPromotions.Contains(p.ID) && p.CurrentPosition.y % 8 == 0))
                //        {

                //        }
                //    }
            }
            else
            {
                p.Move(move);
            }
            if (!sim)
                
            {
                
                StartCoroutine("EndTurn");
            }

        }
    }
    
    bool MoveLegal(Piece p, Vector2Int move)//determines whether a piece can make a given move
    {
        
        if (p.CurrentPosition.x != -1) //inbounds pieces
        {
            if (LineOfSight(p, move))
            {
                //if(p.id == Occupant.Bishop)
                //{
                //    print(move);
                //}

                if (SpaceEmpty(p.CurrentPosition + move) || SpaceContainsEnemy(p.CurrentPosition + move))//PieceInDanger(p, boardArrangement[(p.CurrentPosition + move).x, (p.CurrentPosition + move).y], move))//in the case of OOP pieces, move = desired position
                {
                    if (SpaceContainsEnemy(p.CurrentPosition + move))
                    {
                        Vector2Int space = p.CurrentPosition + move;
                        //print(System.String.Format("Space: {2} SpaceEmpty: {0} SpaceContainsenemy: {1}", SpaceEmpty(space).ToString(), SpaceContainsEnemy(space).ToString(), space));

                        //print(System.String.Format("Move {0} by {1} is legal", move, p.name));
                    }
                    return true;
                }
            }
            else
            {

            }
        }
        else
        {
                if (SpaceEmpty(move))
                {
                    return true;

                }
                
            
        }
        return false;
    }
    bool LineOfSight(Piece p, Vector2Int move, bool capture = false)
    {
        int limit;
        int sign;
        Vector2Int space;
        //print(p);
        if (p.CurrentPosition.x != -1)
        {//in play pieces 
            switch (p.pieceData.pieceName)
            {
                case "Rook":
                    if (move.x == 0 && Mathf.Abs(move.y) > 1)
                    {
                        sign = move.y / Mathf.Abs(move.y);
                        limit = Mathf.Abs(move.y);//distance check

                        for (int i = 1; i <= limit; i++)
                        {
                            space = p.CurrentPosition + new Vector2Int(0, i * sign);
                            //if (Mathf.Abs(move.x) == 6)
                            //{
                            //    print(space);
                            //}

                            if (!SpaceEmpty(space))
                            {
                                if (i == Mathf.Abs(move.y))
                                {
                                    if (!SpaceContainsEnemy(space))
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else if (move.y == 0 && Mathf.Abs(move.x) > 1)
                    {
                        sign = move.x / Mathf.Abs(move.x);
                        limit = Mathf.Abs(move.x);//distance check

                        for (int i = 1; i <= limit; i++)
                        {

                            space = p.CurrentPosition + new Vector2Int(i * sign, 0);

                            //if (p.CurrentPosition.x != -1)

                            //{
                            //    print(boardArrangement[p.CurrentPosition.x, p.CurrentPosition.y]);
                            //}
                            if (!SpaceEmpty(space))
                            {
                                if (i == Mathf.Abs(move.x))
                                {
                                    if (!SpaceContainsEnemy(space))
                                    {
                                        return false;
                                    }
                                    //else
                                    //{
                                    //    print("enemy in space " + space.ToString());
                                    //}
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }

                    }
                    else
                    {
                        space = p.CurrentPosition + move;
                        if (!SpaceEmpty(space))
                        {
                            //print("space not empty for rook");
                            return false;
                        }

                    }
                    break;
                case "Bishop":
                    //if (Mathf.Abs(move.x) > 1)
                    //{
                    // print(move);

                    int signX = 0, signY = 0;
                    if (move.x != 0)
                    {
                        signX = move.x / Mathf.Abs(move.x);
                    }
                    if (move.y != 0)
                    {
                        signY = move.y / Mathf.Abs(move.y);
                    }
                    for (int i = 1; i <= Mathf.Abs(move.x); i++)
                    {
                        space = p.CurrentPosition + new Vector2Int(i * signX, i * signY);



                        if (i < Mathf.Abs(move.x))//if not at the destination space, any space with a piece in it blocks line of sight
                        {
                            if (!SpaceEmpty(space))
                            {
                                return false;
                            }

                        }
                        else //if at destination space 
                        {
                            print("at destination");
                            if (!SpaceEmpty(space))
                            {//if the space is not empty 
                                print(SpaceContainsEnemy(space));
                                if (!SpaceContainsEnemy(space))
                                {
                                    //but not occupied by an enemy, line of sight is blocked
                                    return false;
                                }

                            }
                            //print(System.String.Format("Space: {2} SpaceEmpty: {0} SpaceContainsenemy: {1}", SpaceEmpty(space).ToString(), SpaceContainsEnemy(space).ToString(), space));

                        }
                    }

                    //}
                    //else
                    //{
                    //    space = p.CurrentPosition + move;

                    //    if(!SpaceEmpty(space))
                    //    {//if the space is not empty 

                    //        if (!SpaceContainsEnemy(space))
                    //        {
                    //            //but not occupied by an enemy, line of sight is blocked
                    //            return false;
                    //        }
                    //    }
                    //}
                    break;
                case "Lance":

                    sign = move.x / Mathf.Abs(move.x);
                    limit = Mathf.Abs(move.x);//distance check

                    for (int i = 1; i <= limit; i++)
                    {

                        space = p.CurrentPosition + new Vector2Int(i * sign, 0);

                        //if (p.CurrentPosition.x != -1)

                        //{
                        //    print(boardArrangement[p.CurrentPosition.x, p.CurrentPosition.y]);
                        //}
                        if (!SpaceEmpty(space))//if the space is occupied
                        {
                            if (i == Mathf.Abs(move.x))//if this is the destination space
                            {
                                if (!SpaceContainsEnemy(space))//if the occupant is not an enemy, line of sight is bloced
                                {
                                    return false;
                                }
                                //else
                                //{
                                //    print("enemy in space " + space.ToString());
                                //}
                            }
                            else//otherwise, line of sight is blocked either way
                            {
                                return false;
                            }
                        }
                    }
                    break;



                //sign = move.x / Mathf.Abs(move.x);

                //for (int i = 1; i <= Mathf.Abs(move.x); i++)
                //{
                //    space = p.CurrentPosition + new Vector2Int(i * sign, 0);
                //    //if (move.x == 2)
                //    //{

                //    //    print(System.String.Format("space: PSpaceEmpty(space));
                //    //}

                //    //print("space " + space.ToString());
                //    //print("testing??");
                //    if (SpaceEmpty(space))
                //    {
                //        return false;
                //    }
                //}
                //break;

                default:
                    space = p.CurrentPosition + move;
                    if (!SpaceEmpty(space))
                    {//if space is not empty
                        if (!SpaceContainsEnemy(space))
                        {//and the space does not contain an enemy
                            return false;
                        }
                    }
                    break;

            }
        }
        else
        {//oop pieces

            if (!SpaceEmpty(move))
            {
                return false;
            }
        }
        return true;
    }
    bool PieceInDanger(Piece attacker, Piece target, Vector2Int move)
    {
        if (attacker.pieceData.name == "Bishop")
        {
            print(System.String.Format("attacker: {0}{1} {5}, target: {2}{3}{6}, move: {4}", attacker.pieceData.name, attacker.Owner, target.pieceData.name, target.Owner, move, attacker.transform.position, target.transform.position));
        }
        

        if (target != null)
        {
            if (LineOfSight(attacker, move) && attacker.Owner != target.Owner)
            {

                if (attacker.CurrentPosition + move == target.CurrentPosition)
                {
                    //print(attacker.CurrentPosition + move);

                    return true;
                }
                else
                {
                    //print(System.String.Format("Space {0} is empty", move + attacker.CurrentPosition));
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else return false;
    }
    void Capture(Piece attacker, Piece target, bool sim = true)
    {

        attacker.Move(target.CurrentPosition);
        if (!sim)
        {
            target.SetColor(playerColors[attacker.Owner - 1]);
            target.Owner = attacker.Owner;
            target.transform.Rotate(new Vector3(0, 180, 0));
           
            pieceGroups[0].Remove(target);
            pieceGroups[attacker.Owner].Add(target);
            (piecePromotable, attacker.Promotable) = Promotable(attacker, target.CurrentPosition);
            
        }

        target.Move(new Vector2Int(-1, -1));
        target.Demote();
    }

    public void Promote(bool promotion)
    {
        promote = promotion;
        piecePromotable = false;
    }
    IEnumerator EndTurn()
    {
        print("ending turn");
        UpdateBoardArrangement();
        RedrawBoard();

        List<Piece> currentPieces = pieceGroups[CurrentPlayerTurn()];
        Piece promotablePiece = null;
        foreach(Piece p in currentPieces)
        {
            if (p.Promotable) {
                promotablePiece = p;

            }
        }
        if (promotablePiece)
        {
          //should only trigger when a promoted piece is available
            if (basePieces.Contains(selectedPiece.ID))
            {

                if (forcedPromotions.Contains(selectedPiece.ID) && selectedPiece.AutoPromote())
                {
                    promote = true;
                    selectedPiece.SetColor(playerColors[CurrentPlayerTurn() - 1]);
                    selectedPiece = null;
                    

                }
                else
                {
                    promotionPanel.SetActive(true);

                    while (piecePromotable)
                    {
                        yield return null;

                    }
                }
            }
            if (promote)
            {
                promotablePiece.Promote();
                promote = false;
                //do something to redraw the model when needed
            }
            promotionPanel.SetActive(false);

        }
        RedrawBoard();

        selectedPiece = null;

        turn += 1;
        UpdatePlayerPieces();
        //foreach(Piece p in pieceGroups[0])
        //{
        //    EditMoves(p);
        //}
        //UpdateBoardPositions()
        if (KingInCheck())
        {
            if (Checkmate())
            {
                print("reverting");
                turn -= 1;
                EndGame();
            }

        }
    }

    void RedrawBoard()
    {


        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (boardArrangement[i, j] != null)
                {
                    //todo: turn this into coroutine
                    boardArrangement[i, j].transform.position = grid.CellToWorld(new Vector3Int(boardArrangement[i, j].CurrentPosition.x, boardY, boardArrangement[i, j].CurrentPosition.y));
                }
            }
        }
        for (int i = 1; i < 3; i++)
        {
            int j = 0, k = 0;
            if (i == 1)
            {
                
            }
            foreach(Piece p in pieceGroups[i])
            {
                if (p.CurrentPosition.x == -1)
                {
                    p.transform.position = barracks[i-1].CellToWorld(new Vector3Int(j, boardY, k));
                    if (k > 8)
                    {
                        j += 1;
                        k = 0;
                    }
                    else
                    {
                        k += 1;
                    }
                }

            }
        }

    }


    void EndGame()
    {
        int player = CurrentPlayerTurn();
        print(System.String.Format("Player {0} wins!", player));
    }
    // Update is called once per frame

    void Update()
    {

        playerTurn = CurrentPlayerTurn();
        //UpdateText();
        activePieces = allPieces;
        //print(PlayerTurn());
    }
}
