using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpot : MonoBehaviour
{
    Vector2Int position;
    public Vector2Int Position
    {
        get { return position; }
        set { position = value; }
    }

    public Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }
    // Start is called before the first frame update
    private void OnMouseUp()
    {
        SendMessageUpwards("OnSpaceClick", position, SendMessageOptions.RequireReceiver);
       // print(System.String.Format("Selected space {0} at board coordinate {1}", this.name, this.position));
    }

}
