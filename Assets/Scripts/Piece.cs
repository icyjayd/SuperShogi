using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public PieceData pieceData;
    public bool forward = true;
    Vector3[] moves;
    Renderer rend;
    int Owner = 0;
    Vector2 currentPosition;
    bool promoted = false;


    // Start is called before the first frame update
    void Awake()
    {
        moves = pieceData.legalMoves;
        rend = GetComponent<Renderer>();
        rend.material.mainTexture =  pieceData.baseTexture;

    }



}
