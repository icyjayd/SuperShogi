using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpot : MonoBehaviour
{
    [SerializeField]
    Color originalColor;
    bool colorSet = false;
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

    public void SetColor(Color color)
    {
        if (!colorSet)
        {
            originalColor = color;
            colorSet = true;
        }
        rend.material.SetColor("_Color", color);
    }

    public void ResetColor()
    {
        rend.material.SetColor("_Color", originalColor);

    }
    // Start is called before the first frame update
    private void OnMouseUp()
    {
        SendMessageUpwards("OnSpaceClick", position, SendMessageOptions.RequireReceiver);
       // print(System.String.Format("Selected space {0} at board coordinate {1}", this.name, this.position));
    }

}
