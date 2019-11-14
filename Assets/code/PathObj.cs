using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathObj : MonoBehaviour
{

    [SerializeField]
    Tile tile;
    public Tile Tile { get { return tile; } }
    Action recalc;
    Vector3 pos;

    public void SetPathfinder(Action pathfinder)
    {
        recalc = pathfinder;
    }

    void Start()
    {
        FindTile();
        pos = transform.position;
    }

    void Update()
    {
        if (pos != transform.position)
        {

            pos = transform.position;
            if (recalc != null)
            {
                FindTile();
                if (tile != null && !tile.IsBlocked)
                {
                    recalc();
                }
            }
        }
    }

    void FindTile()
    {
        Ray _ray = new Ray(transform.position, Vector3.down);
        RaycastHit _hit;
        if (Physics.Raycast(_ray, out _hit))
        {
            var _tile = _hit.collider.gameObject.GetComponent<Tile>();
            if (_tile != null && !_tile.IsBlocked)
            {
                tile = _tile;
            }
        }
    }
}
