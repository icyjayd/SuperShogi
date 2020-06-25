using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public PieceData pieceData;
    public bool forward = true;
    Vector3Int[] moves;
    Renderer rend;
    int owner = 0;
    public int Owner
    {
        get { return owner; }
        set { owner = value; }
    }
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

    public void SetColor(Color color)
    {
        rend.material.SetColor("_Color", color);
    }

    // Start is called before the first frame update
    void Awake()
    {
        moves = pieceData.legalMoves;
        rend = GetComponent<Renderer>();
        rend.material.mainTexture =  pieceData.baseTexture;

    }




}
