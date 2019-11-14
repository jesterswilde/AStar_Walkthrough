using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    MeshRenderer tile;
    bool isBlocked;
    public bool IsBlocked { get { return isBlocked; } }
    int x, y;
    [SerializeField]
    Text priority;
    [SerializeField]
    Text crowDist;
    [SerializeField]
    Text traveled;
    public int X { get { return x; } }
    public int Y { get { return y; } }
    public void Setup(int _x, int _y)
    {
        x = _x;
        y = _y;
        tile = GetComponent<MeshRenderer>();
        tile.material.color = Grid.BaseColor;
        transform.position = new Vector3(x * Grid.TileSize, 0, y * Grid.TileSize);
        CheckIfBlocked();
        if(priority){
            priority.text = "";
        }
        if(crowDist){
            crowDist.text = "";
        }
        if(traveled){
            traveled.text = "";
        }
    }
    public void SetPriority(string _text)
    {
        if(priority == null){
            return; 
        }
        if (_text == Mathf.Infinity.ToString("0.0"))
        {
            priority.text = "∞";
        }
        else
        {
            priority.text = _text;
        }
    }
    public void SetCrowDist(string _text)
    {
        if(crowDist == null){
            return;
        }
        if (_text == Mathf.Infinity.ToString("0.0"))
        {
            crowDist.text = "∞";
        }
        else
        {
            crowDist.text = _text;
        }
    }
    public void SetTraveled(string _text)
    {
        if(traveled == null){
            return;
        }
        if (_text == Mathf.Infinity.ToString("0.0"))
        {
            traveled.text = "∞";
        }
        else
        {
            traveled.text = _text;
        }
    }
    public void ChangeColor(Color _color)
    {
        tile.material.color = _color;
    }
    public void CheckIfBlocked()
    {
        Vector3 checkPos = transform.position + (Vector3.up * (Grid.TileSize / 2));
        isBlocked = Physics.CheckSphere(checkPos, Grid.TileSize / 2, Grid.ObstructionMask);
        var color = isBlocked ? Grid.ObstructedColor : Grid.BaseColor;
        tile.material.color = color;
    }

}
