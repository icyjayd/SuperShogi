using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Piece : MonoBehaviour
{

    public PieceData pieceData;
    public bool forward = true;
    [SerializeField]
    Vector3Int[] moveset;
    public List<Vector2Int> moves;
    Vector2Int lastMove;
    public Vector2Int lastPosition;
    public Vector2Int LastPosition
    {
        get { return lastPosition; }
        set { lastPosition = value; }
    }

    public Renderer rend;
    bool selected = false;
    public bool Selected
    {
        get { return selected; }
        set { selected = value; }
    }
    [SerializeField]
    int owner = 0;
    public int Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    [SerializeField]
    Vector2Int currentPosition;
    public Vector2Int CurrentPosition {
        get { return currentPosition; }
        set { currentPosition = value; }
    }
    bool promoted = false;
    public bool Promoted
    {
        get { return promoted; }
        set { promoted = value; }
    }
    bool promotable = false;
    public bool Promotable
    {
        get { return promotable; }
        set { promotable = value; }
    }

    public Occupant id;
    public Occupant ID
    {
        get { return id; }
        set { id = value; }
    }

    public void SetColor(Color color)
    {
        rend.material.SetColor("_Color", color);
    }

    // Start is called before the first frame update
    void Awake()
    {
        moves = new List<Vector2Int>();
        rend = GetComponent<Renderer>();
        rend.material.mainTexture =  pieceData.baseTexture;
        id = pieceData.id;
        moveset = pieceData.legalMoves;

    }

    private void OnMouseUp()
    {

        SendMessageUpwards("OnPieceClick", this, SendMessageOptions.RequireReceiver);
        //print(System.String.Format("Selected piece {0} at board coordinate {1}", this.name, currentPosition));


    }
    public bool MoveInBounds(Vector2Int move)
    {
        Vector2Int projection = currentPosition + move;
        if(projection.x <0 || projection.y <0 || projection.x > 8 || projection.y > 8)
        {
            //if(id == Occupant.Rook && owner == 1)
            //{
            //    print(System.String.Format("Rook cannot access space {0}, as it is not in bounds. Current position: {1}, attempted move {2}", projection, currentPosition, move));
            //}
            return false;
        }
        else
        {
            //if (id == Occupant.Rook && owner == 1)
            //{
            //    print(System.String.Format("Rook can access space {0}, as it is in bounds. Current position: {1}, attempted move {2}", projection, currentPosition, move));
            //}

            return true;
        }
    }

    public void Move(Vector2Int space)
    {
        lastPosition = currentPosition;
        currentPosition = space;
    }

    public void CalculateLegalMoves(Piece[,]boardPieces = null)
    {
        //
        moves = new List<Vector2Int>();
        if (currentPosition.x != -1)//piece on board
        {
            foreach (Vector3Int move in moveset)
            {
                Vector3Int newMove = new Vector3Int(move.y, move.x, move.z);
                if (owner == 2)
                {
                    newMove = new Vector3Int(move.y * -1, move.x, move.z);
                }
                if (move.z != 0)
                {
                    CalculateFarMoves(newMove);
                }
                else
                {
                    if (MoveInBounds(new Vector2Int(newMove.x, newMove.y)))
                    {

                        moves.Add(new Vector2Int(newMove.x, newMove.y));
                    }
                }
            }
        }
        else//piece has position -1, -1 and is therefore OOP
        {
            print(name);
            int limit;
            for (int i = 0; i < 9; i++)

            {

                for (int j = 0; i < 9; j++)
                {
                    //if (boardPieces[i, j] != null)
                    //{
                    switch (id)
                    {
                        case Occupant.Pawn:
                            limit = 1;
                            break;
                        case Occupant.Knight:
                            limit = 2;
                            break;
                        default:
                            limit = 0;
                            break;
                    }



                    if (owner == 2)
                    {
                        if (i >= 0 + limit)
                        {

                            moves.Add(new Vector2Int(i, j));

                        }
                    }
                    else
                    {
                        if (i <= (8 - limit))
                        {
                            print(new Vector2Int(i, j));

                            moves.Add(new Vector2Int(i, j));

                        }

                    }

                    //}
                }

            }
        }
    }
    public void Promote()
    {

        id = pieceData.proID;
        rend.material.mainTexture = pieceData.promotedTexture;
        moveset = pieceData.promotedLegalMoves;
        CalculateLegalMoves();
      
    }
    public void Demote()
    {

        id = pieceData.id;
        rend.material.mainTexture = pieceData.baseTexture;
        moveset = pieceData.legalMoves;
    }

    public bool AutoPromote()
    {
        if (id == Occupant.Pawn || id == Occupant.Lance)
        {
            if (currentPosition.x % 8 == 0)
            {
                return true;
            }

        }
        else if (id == Occupant.Knight)
        {
            print(currentPosition);
            if (Owner == 1)
            {
                if (currentPosition.x % 6 >= 1)
                {
                    
                    return true;
                }
            }
            else
            {
                if (currentPosition.x % 6 <= 1)
                {
                    return true;
                }

            }
        }
        return false;
    }

    public void CalculateFarMoves(Vector3Int move)
    {
        int limit;
        Vector2Int newMove;

        if (pieceData.pieceName == "Bishop")
        {
           
            int signX = move.x / Mathf.Abs(move.x);
            int signY = move.y / Mathf.Abs(move.y);
            limit = Mathf.Min(9 - Mathf.Abs(currentPosition.x), 9 - Mathf.Abs(currentPosition.y));
            for (int i = 1; i < 9; i++)
            {
                newMove = new Vector2Int(i * signX, i * signY);
                
                if (MoveInBounds(newMove))
                {
                    moves.Add(newMove);
                }

            }
        }
        else if (pieceData.pieceName == "Rook")
        {
            if (move.x == 0)
            {
                int signY = move.y / Mathf.Abs(move.y);
                limit = Mathf.Min(9, move.y + currentPosition.y);

                for (int i = 1; i < 9; i++)
                {
                    newMove = new Vector2Int(0, i * signY);
                    //print(System.String.Format("i = {0}, newMove = {1}", i, newMove));
                    if (MoveInBounds(newMove))
                    {
                        moves.Add(newMove);
                    }

                }


            }
            else
            {
                int signX = move.x / Mathf.Abs(move.x);
                limit = Mathf.Min(9, move.x + currentPosition.x);
                //print(System.String.Format("move distance for rook: {0}", limit));

                for (int i = 1; i < 9; i++)
                {

                    newMove = new Vector2Int(i * signX, 0);
                    //print(System.String.Format("i = {0}, newMove = {1}", i, newMove));

                    if (MoveInBounds(newMove))
                    {

                        moves.Add(newMove);
                    }



                }
            }

        }
        else
        {
            for(int i = 1; i<9; i++)
            {
                int signX = move.x / Mathf.Abs(move.x);
                newMove = new Vector2Int(i * signX, 0);
                if (MoveInBounds(newMove)){
                    moves.Add(newMove);
                }
            }
        }



    }


}
