using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    public PieceData pieceData;
    public bool forward = true;
    Vector3Int[] moveset;
    public List<Vector2Int> moves;
    Renderer rend;
    bool selected = false;
    public bool Selected
    {
        get { return selected; }
        set { selected = value; }
    }
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
            return false;
        }
        else
        {
            return true;
        }
    }


    public void CalculateLegalMoves(Piece[,]boardPieces = null)
    {
        //
        moves = new List<Vector2Int>();
        if(currentPosition.x != -1)//piece on board
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
          for(int i = 0; i < 9; i++)
            {
               
                for(int j = 0; i < 9; j++)
                {
                    if (boardPieces[i, j] != null)
                    {
                        moves.Add(new Vector2Int(i, j));
                    }
                }

            }
        }
    }
    public void Promote()
    {

        id = pieceData.proID;
        rend.material.mainTexture = pieceData.promotedTexture;
        moveset = pieceData.promotedLegalMoves;
    }
    public void Demote()
    {

        id = pieceData.id;
        rend.material.mainTexture = pieceData.baseTexture;
        moveset = pieceData.legalMoves;
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
            for (int i = 1; i < limit; i++)
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
                for (int i = 1; i < limit; i++)
                {
                    newMove = new Vector2Int(0, i * signY);
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
                for (int i = 1; i < limit; i++)
                {

                    newMove = new Vector2Int(move.x + i * signX, 0);
                    if (MoveInBounds(newMove))
                    {
                        moves.Add(newMove);
                    }



                }
            }

        }

    }


}
