using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dijkstras : MonoBehaviour
{
    [SerializeField]
    Gradient distGrad;
    [SerializeField]
    float maxDist = 5;
    [SerializeField]
    float distMultiplier = 2f;
    Tile startingTile;
    Tile targetTile;
    [SerializeField]
    PathObj startingObj;
    [SerializeField]
    PathObj targetObj;
    Heap<DNode> heap;
    Dictionary<Tile, DNode> dict;
    List<Tile> path;
    bool isPathfinding = false;
    DNode currentNode;
    [SerializeField]
    int tilesPerFrame = 3;
    int cycles; 

    void Start()
    {
        startingObj.SetPathfinder(Pathfind);
        targetObj.SetPathfinder(Pathfind);
        Pathfind();
    }

    public void Pathfind()
    {
        cycles = 0; 
        maxDist = Vector3.Distance(startingObj.transform.position, targetObj.transform.position) * distMultiplier;
        Grid.ResetGridColor();
        heap = new Heap<DNode>();
        dict = new Dictionary<Tile, DNode>();
        startingTile = startingObj.Tile;
        targetTile = targetObj.Tile;
        isPathfinding = true;
        var currentTile = startingTile;
        var startingNode = new DNode(currentTile);
        startingNode.distance = 0;
        dict[startingNode.tile] = startingNode;
        heap.Insert(startingNode);
    }

    void FoundPath(DNode node)
    {
        Debug.Log("Cycles: " + cycles); 
        path = new List<Tile>();
        while (node != null)
        {
            path.Add(node.tile);
            node.tile.ChangeColor(Color.black);
            node = node.prev;
        }
        path.Reverse();
    }

    void Update()
    {
        if (isPathfinding)
        {
            PathfindLoop();
        }
    }

    void PathfindLoop()
    {
        int count = 0;
        while (count < tilesPerFrame)
        {
            count++;
            cycles++; 
            currentNode = heap.Remove();
            if (currentNode.tile == targetTile)
            {
                isPathfinding = false;
                FoundPath(currentNode);
                return;
            }
            var neighbors = Grid.GetNeighbors(currentNode.tile);
            neighbors.ForEach((neighbor) =>
            {
                DNode neighborNode;
                bool alreadyContained = false;
                if (neighbor.tile.IsBlocked)
                {
                    return;
                }
                if (dict.ContainsKey(neighbor.tile))
                {
                    neighborNode = dict[neighbor.tile];
                    alreadyContained = true;
                }
                else
                {
                    neighborNode = new DNode(neighbor.tile);
                }
                if (neighborNode.distance > currentNode.distance + neighbor.distance)
                {
                    if (alreadyContained)
                    {
                        heap.Remove(neighborNode);
                    }
                    var distance = currentNode.distance + neighbor.distance;
                    neighborNode.distance = distance;
                    neighborNode.prev = currentNode;
                    dict[neighborNode.tile] = neighborNode;
                    neighborNode.tile.ChangeColor(distGrad.Evaluate(distance / maxDist));
                    heap.Insert(neighborNode);
                }
            });
        }
    }
}

public class DNode : IComparable<DNode>
{
    public Tile tile;
    public float distance = Mathf.Infinity;
    public 	DNode prev;
	// public DNode Prev {get{return prev;} set{
	// 	prev = value; 
	// 	if(lr == null){
	// 		var go = new GameObject(); 
	// 		lr = go.AddComponent<LineRenderer>(); 
	// 		lr.SetPositions(new Array<Vector3>()[tile.transform.position, prev.tile.transform.position]); 
	// 	}
	// }}
    public bool visited = false;
	LineRenderer lr; 

    public DNode(Tile _tile)
    {
        tile = _tile;
    }

    public int CompareTo(DNode _item)
    {
        if (distance > _item.distance)
        {
            return 1;
        }
        if (distance < _item.distance)
        {
            return -1;
        }
        return 0;
    }
}