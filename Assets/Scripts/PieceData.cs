using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PieceData : ScriptableObject
{
    public string pieceName;//the name of the piece
    public Vector3[] legalMoves;
    public Vector3[] promotedLegalMoves;
    public Texture baseTexture;
    public Texture promotedTexture;
    public Occupant id;


   //for the moves, the xy coordinates are the actual placements of the move; the z coordinate simply denotes whether the move is finite (0) or infinite (1)
  
}
