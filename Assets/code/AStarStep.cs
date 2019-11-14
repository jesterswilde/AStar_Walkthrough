using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum StepsEnum
{
    PICK_NODE,
    CREATE_NODES,
    UPDATE_NEIGHBORS,
}

public class AStarStep : MonoBehaviour
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
    StepsEnum step = StepsEnum.PICK_NODE;

    [SerializeField]
    Color creatingNodeColor;
    [SerializeField]
    Color tooVisitColor;
    [SerializeField]
    Color visitedColor;
    [SerializeField]
    Color currentNodeColor;
    bool shouldDraw = false;

    void Start()
    {
        Pathfind();
        startingObj.SetPathfinder(Pathfind);
        targetObj.SetPathfinder(Pathfind);
    }

    void Update()
    {
        if (isPathfinding && Input.GetMouseButtonDown(0))
        {
            if (step == StepsEnum.PICK_NODE)
            {
                step = StepsEnum.CREATE_NODES;
                PickNode();
                return;
            }
            if (step == StepsEnum.CREATE_NODES)
            {
                step = StepsEnum.UPDATE_NEIGHBORS;
                CreateNeighbors();
                return;
            }
            if (step == StepsEnum.UPDATE_NEIGHBORS)
            {
                step = StepsEnum.PICK_NODE;
                UpdateNeighbors();
                return;
            }
        }
    }

    void PickNode()
    {
        Debug.Log("Picking");
        if (currentNode != null)
        {
            currentNode.tile.ChangeColor(visitedColor);
        }
        currentNode = distHeap.Remove();
        visited[currentNode.tile] = true;
        currentNode.tile.ChangeColor(currentNodeColor);
        if (currentNode.tile == targetTile)
        {
            isPathfinding = false;
            FoundTarget();
            return;
        };
    }

    void CreateNeighbors()
    {
        Debug.Log("creating");
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
            StepNode node;
            if (storage.ContainsKey(neighbor.tile))
            {
                node = storage[neighbor.tile];
            }
            else
            {
                node = new StepNode(neighbor.tile, targetTile);
                storage[node.tile] = node;
                node.tile.SetPriority(node.priority.ToString("n1"));
                node.tile.SetCrowDist(node.crowDist.ToString("n1"));
                node.tile.SetTraveled(node.traveled.ToString("n1"));
            }
            node.tile.ChangeColor(tooVisitColor);
        });
    }

    void UpdateNeighbors()
    {
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
                node = new StepNode(neighbor.tile, targetTile);
                storage[node.tile] = node;
            }
            node.tile.ChangeColor(creatingNodeColor);
            if (node.traveled > currentNode.traveled + neighbor.distance)
            {
                if (alreadyVisited)
                {
                    distHeap.Remove(node);
                }
                var distance = currentNode.traveled + neighbor.distance;
                node.traveled = currentNode.traveled + neighbor.distance;
                node.Prev = currentNode;
                node.tile.SetTraveled(node.traveled.ToString("n1"));
                node.tile.SetPriority(node.priority.ToString("n2"));
                distHeap.Insert(node);
            }
        });
    }

    public void Pathfind()
    {
        if (startingObj.Tile.IsBlocked || targetObj.Tile.IsBlocked)
        {
            return;
        }
        isPathfinding = true;
        distHeap = new Heap<StepNode>();
        storage = new Dictionary<Tile, StepNode>();
        visited = new Dictionary<Tile, bool>();
        targetTile = targetObj.Tile;
        var node = new StepNode(startingObj.Tile, targetTile);
        node.traveled = 0;
        storage[node.tile] = node;
        distHeap.Insert(node);
        Grid.ResetGridColor();
        node.tile.SetPriority(node.priority.ToString("n2"));
        node.tile.SetCrowDist(node.crowDist.ToString("n1"));
        node.tile.SetTraveled(node.traveled.ToString("n1"));
    }



    void FoundTarget()
    {
        var path = new List<Tile>();
        var node = currentNode;
        while (node != null)
        {
            node.tile.ChangeColor(Color.black);
            node = node.Prev;
        }
    }
}

public class StepNode : IComparable<StepNode>
{
    static event Action DrawLines;
    static event Action DestroyLines;
    public float traveled = Mathf.Infinity;
    public float crowDist;
    public float priority { get { return traveled + crowDist; } }
    public Tile tile;
    LineRenderer line;
    StepNode prev;
    public StepNode Prev
    {
        get { return prev; }
        set
        {
            DrawLines();
            prev = value;
        }
    }

    public static void ToggleLines()
    {
        DrawLines();
    }
    public static void ClearAllLines()
    {
        if (DestroyLines != null)
        {
            DestroyLines();
        }
    }

    void DrawLine()
    {
        if (Grid.ShouldDrawLines)
        {
            if (Prev == null)
            {
                return;
            }
            if (line == null)
            {
                var go = new GameObject();
                line = go.AddComponent<LineRenderer>();
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;
                line.material = Grid.LineMat;
            }
            Vector3[] positions = new Vector3[]{
                tile.transform.position + Vector3.up * 0.2f,
                Prev.tile.transform.position + Vector3.up * 0.2f
            };
            line.SetPositions(positions);

        }
        else
        {
            if (line == null)
            {
                return;
            }
            GameObject.Destroy(line.gameObject);
            line = null;
        }
    }

    void DestroyLine()
    {
        if (line != null)
        {
            GameObject.Destroy(line.gameObject);
        }
        DestroyLines -= DestroyLine;
        DrawLines -= DrawLine;
    }

    public StepNode(Tile _tile, Tile _target, DistMethod method = DistMethod.GRID)
    {
        DrawLines += DrawLine;
        DestroyLines += DestroyLine;
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
        DrawLine();

    }
    public int CompareTo(StepNode _info)
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