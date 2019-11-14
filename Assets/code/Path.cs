using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Path : MonoBehaviour
{
    Heap<Node> toVisit;
	Dictionary<Tile, bool> visited; 
	Dictionary<Tile, Node> storage; 

	[SerializeField]
	PathObj startObj; 
	[SerializeField] 
	PathObj targetObj; 

	Node currentNode; 
	Tile targetTile; 

	void Start(){
		Pathfind(); 
		startObj.SetPathfinder(Pathfind); 
		targetObj.SetPathfinder(Pathfind); 
	}

	void Pathfind(){
		//Setup
		Setup(); 
		while(currentNode.tile != targetTile){
			Loop(); 
		}
		TraverseBack(); 
		//Pathfind Loop

		//Traverse Path
	}

	void Setup(){
		targetTile = targetObj.Tile; 
		currentNode = new Node(startObj.Tile, targetTile); 
		currentNode.traveled = 0;
		toVisit = new Heap<Node>() ;
		storage = new Dictionary<Tile, Node>(); 
		visited = new Dictionary<Tile, bool>(); 
		Grid.ResetGridColor(); 
	}

	void Loop(){
		var neighbors = Grid.GetNeighbors(currentNode.tile); 
		neighbors.ForEach((neighbor)=>{
			if(neighbor.tile.IsBlocked){
				return; 
			}
			if(visited.ContainsKey(neighbor.tile)){
				return; 
			}
			Node node; 
			bool alradyCreated = false; 
			//Node has been created before
			if(storage.ContainsKey(neighbor.tile)){
				node = storage[neighbor.tile];
				alradyCreated = true; 
			}else{
			//Node is new
				node = new Node(neighbor.tile, targetTile); 
				storage[neighbor.tile] = node; 
			}
			float distance = currentNode.traveled + neighbor.distance;
			if(node.traveled > distance){
				if(alradyCreated){
					toVisit.Remove(node); 
				}
				node.traveled = distance;
				node.previous = currentNode; 
				toVisit.Insert(node); 
			}
		}); 
		currentNode = toVisit.Remove(); 
	}

	void TraverseBack(){
		while(currentNode != null){
			currentNode.tile.ChangeColor(Color.black); 
			currentNode = currentNode.previous; 
		}
	}
}

public class Node : IComparable<Node>
{
	public Tile tile;
    public float priority { get { return traveled + crowDistance; } }
    public float traveled = Mathf.Infinity;
    public float crowDistance;
	public Node previous; 

	public Node(Tile _tile, Tile _target){
		tile = _tile; 
		float xDiff = Math.Abs(_tile.X - _target.X); 
		float yDiff = Math.Abs(_tile.Y + _target.Y); 
		crowDistance = xDiff + yDiff; 
	}
    public int CompareTo(Node _info)
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