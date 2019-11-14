using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    static Grid t;
    [SerializeField]
    Object tilePrefab;
    [SerializeField]
    LayerMask obstructionMask;
    public static LayerMask ObstructionMask { get { return t.obstructionMask; } }
    [SerializeField]
    int gridX = 10;
    [SerializeField]
    int gridY = 10;
    [SerializeField]
    float tileSize = 1;
    public static float TileSize { get { return t.tileSize; } }
    Tile[,] tiles;
    [SerializeField]
    Color baseColor;
    public static Color BaseColor { get { return t.baseColor; } }
    [SerializeField]
    Color obstructedColor;
    public static Color ObstructedColor { get { return t.obstructedColor; } }
    [SerializeField]
    Material lineMat;
    public static Material LineMat { get { return t.lineMat; } }
    bool drawLines = false;
    public static bool ShouldDrawLines { get { return t.drawLines; } }


    void Awake()
    {
        t = this;
    }

    public static void ResetGridColor()
    {
        for (int x = 0; x < t.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < t.tiles.GetLength(1); y++)
            {
                var tile = t.tiles[x, y];
                if (!tile.IsBlocked)
                {
                    tile.ChangeColor(BaseColor);
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        tiles = new Tile[gridX, gridY];
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                var go = Instantiate(tilePrefab) as GameObject;
                var tile = go.GetComponent<Tile>();
                tile.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
                tiles[x, y] = tile;
                tile.Setup(x, y);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            drawLines = !drawLines;
            Debug.Log(drawLines); 
            StepNode.ToggleLines();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(((gridX) * tileSize) / 2 - tileSize / 2, 0, ((gridY) * tileSize) / 2 - tileSize / 2), new Vector3(gridX * tileSize, 0.1f, gridY * tileSize));
    }

    public static List<Neighbor> GetNeighbors(Tile _tile)
    {
        List<Neighbor> neighbors = new List<Neighbor>();
        for (int x = Mathf.Max(0, _tile.X - 1); x < Mathf.Min(t.gridX - 1, _tile.X + 2); x++)
        {
            for (int y = Mathf.Max(0, _tile.Y - 1); y < Mathf.Min(t.gridY - 1, _tile.Y + 2); y++)
            {
                if (x != _tile.X || y != _tile.Y)
                {
                    float distance = (x == _tile.X || y == _tile.Y) ? 1 : 1.414f;
                    neighbors.Add(new Neighbor(t.tiles[x, y], distance));
                }
            }
        }
        return neighbors;
    }
}

public struct Neighbor
{
    public float distance;
    public Tile tile;
    public Neighbor(Tile _tile, float _distance)
    {
        distance = _distance;
        tile = _tile;
    }
}