using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PieceInfo : ScriptableObject
{
    public string name;//the name of the piece
    public Vector3[] legalMoves;
    public Vector3[] promotedLegalMoves;
   //for the moves, the xy coordinates are the actual placements of the move; the z coordinate simply denotes whether the move is finite (0) or infinite (1)
  
}
