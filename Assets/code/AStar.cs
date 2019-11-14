using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AStar : MonoBehaviour
{
    Heap<StepNode> distHeap;
    Dictionary<Tile, StepNode> storage;
    Dictionary<Tile, bool> visited;
    [SerializeField]
    PathObj startingObj;
    [SerializeField]
    PathObj targetObj;
    Tile targetTile;
    StepNode currentNode;
    bool isPathfinding;
    [SerializeField]
    Gradient distGrad;
    float maxDist;
    [SerializeField]
    float distMult = 4f;
    [SerializeField]
    int stepsPerFrame = 3;
    int cycles = 0;
    [SerializeField]
    DistMethod distanceMethod;
    DistMethod currentMethod;

    void Start()
    {
        currentMethod = distanceMethod;
        Pathfind();
        startingObj.SetPathfinder(Pathfind);
        targetObj.SetPathfinder(Pathfind);
    }

    void Update()
    {
        if (isPathfinding)
        {
            PathfindLoop();
        }
        if (currentMethod != distanceMethod)
        {
            currentMethod = distanceMethod;
            Pathfind();
        }
    }

    public void Pathfind()
    {
        if (startingObj.Tile.IsBlocked || targetObj.Tile.IsBlocked)
        {
            return;
        }
        cycles = 0;
        maxDist = Vector3.Distance(targetObj.Tile.transform.position, startingObj.Tile.transform.position) * distMult;
        isPathfinding = true;
        distHeap = new Heap<StepNode>();
        storage = new Dictionary<Tile, StepNode>();
        visited = new Dictionary<Tile, bool>();
        targetTile = targetObj.Tile;
        StepNode.ClearAllLines(); 
        var node = new StepNode(startingObj.Tile, targetTile);
        node.traveled = 0;
        storage[node.tile] = node;
        distHeap.Insert(node);
        Grid.ResetGridColor();
    }

    void PathfindLoop()
    {
        int count = 0;
        while (count < stepsPerFrame)
        {
            cycles++;
            count++;
            currentNode = distHeap.Remove();
            visited[currentNode.tile] = true;
            if (currentNode.tile == targetTile)
            {
                isPathfinding = false;
                FoundTarget();
                return;
            }
            var neighbors = Grid.GetNeighbors(currentNode.tile);
            neighbors.ForEach((neighbor) =>
            {
                if (neighbor.tile.IsBlocked)
                {
                    return;
                }
                if (visited.ContainsKey(neighbor.tile))
                {
                    return;
                }
                bool alreadyVisited = false;
                StepNode node;
                if (storage.ContainsKey(neighbor.tile))
                {
                    alreadyVisited = true;
                    node = storage[neighbor.tile];
                }
                else
                {
                    node = new StepNode(neighbor.tile, targetTile, currentMethod);
                    storage[node.tile] = node;
                }
                if (node.traveled > currentNode.traveled + neighbor.distance)
                {
                    if (alreadyVisited)
                    {
                        distHeap.Remove(node);
                    }
                    var distance = currentNode.traveled + neighbor.distance;
                    node.traveled = currentNode.traveled + neighbor.distance;
                    node.Prev = currentNode;
                    distHeap.Insert(node);
                    node.tile.ChangeColor(distGrad.Evaluate(distance / maxDist));
                }
            });
        }
    }

    void FoundTarget()
    {
        Debug.Log("Curent Method: " + currentMethod + " Cycles: " + cycles);
        var path = new List<Tile>();
        var node = currentNode;
        while (node != null)
        {
            node.tile.ChangeColor(Color.black);
            node = node.Prev;
        }
    }
}

public class ANode : IComparable<ANode>
{
    public float traveled = Mathf.Infinity;
    public float crowDist;
    public float priority { get { return traveled + crowDist; } }
    public Tile tile;
    public ANode prev;

    public ANode(Tile _tile, Tile _target, DistMethod method)
    {
        tile = _tile;

        if (method == DistMethod.GRID)
        {
            //Full Grid 450
            float xDiff = Mathf.Abs(_tile.X - _target.X);
            int yDiff = Mathf.Abs(_tile.Y - _target.Y);
            float straight = Mathf.Abs(xDiff - yDiff);
            float diag = Mathf.Max(xDiff, yDiff) - straight;
            crowDist = straight + diag * 1.414f;
        }

        if (method == DistMethod.MANHATTEN)
        {
            // Manhatten - 800
            crowDist = Math.Abs(_tile.X - _target.X) + Math.Abs(_tile.Y - _target.Y);
        }

        if (method == DistMethod.VECTOR_3)
        {
            //Naive - Unrelated - 1200
            crowDist = Vector3.Distance(_tile.transform.position, _target.transform.position);
        }
    }
    public int CompareTo(ANode _info)
    {
        if (priority > _info.priority)
        {
            return 1;
        }
        if (priority < _info.priority)
        {
            return -1;
        }
        return 0;
    }

}

public enum DistMethod
{
    GRID,
    MANHATTEN,
    VECTOR_3,
    NONE
}