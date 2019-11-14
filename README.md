# Interactive Pathfinding

## Scenes

    * AStar_Base - Will pathfind (over time) from startObj to endObj
        WASD moves camera
        Arrows move startObj (recalculates path)
    * Dijkstras_Base - Will pathfind immediately, using Dijkstras
        WASD moves camera
        Arrows move startObj (recalculates path)
    * Maze - Same as A* base, but with a more complex scene
        WASD moves camera
        Arrow moves startObj (recalculates path)
    * StepScene - This scene will walk through the algorithm
        Currently evaluting node is purple
        Uncalculated neighbors are blue
        Neighbors in heap are yellow
        Completed Nodes in green

        WASD moves camera
        Click steps thorugh algorithm

## Files

    * CameraController - Lets you move camera
    * Grid - The generated board
    * Heap - Just a normal heap impelmentation
    * Path - Live coded A*
    * Dikjkstras - Dijkstars Implementation (immediate solving)
    * AStar - AStar implementation that colors squares over time (Can choose distance hueristic)
    * AStarStep - AStar implementation that will step through the algorithm visually

## Dijkstras Pseudo Code

    - Create a ‘visited’ list
    - Create a ‘toVisit’ list
    - Set all nodes distance to infinity
    - Set starting node’s distance to 0
    - If current node is not target node   //Start of loop
        -For every neighbor
            -if Neighbor is not blocked && neighbor hasn’t been visited before && current node’s distance + distance to neighbor is less than neighbor’s distance
                -Set neighbors distance to current node’s distance + distance to neighbor
                -Set neighbors ‘previous’ pointer to current node
                -Add / Update neighbor in ‘toVisit’ list
        - Take next node with smallest distance, and repeat //End of loop
    - Once target found, build path by traversing the ‘previous’ property
    - Follow path from target to start node

## A\*, an Optimization

    -Create a ‘visited’ list
    -Create a ‘toVisit’ list
    -Set all nodes distance to infinity
    -Calculate node’s “crowDistance’
    -Set starting node’s distance to 0
    -If current node is not target node   //Start of loop
        -For every neighbor
            -if Neighbor is not blocked && neighbor hasn’t been visited before && current node’s distance + distance to neighbor is less than neighbor’s distance
                -Set neighbors distance to current node’s distance + distance to neighbor
                -Set neighbors ‘previous’ pointer to current node
                -Add / Update neighbor in ‘toVisit’ list
        -Take next node with smallest priorty, and repeat //End of loop
    -Once target found, build path by traversing the ‘previous’ property
    -Follow path from target to start node

## The Difference

    The only difference between Dijkstras and A* is the 'crowDistance' heuristic. Dijsktras finds the distance between a node, and all other nodes in the graph.
    If your nodes are laid out in something like cartesisan space (real worlds positions) you can guide the algorithm by saying "evaluate the node that appears to the closest to the our target" as opposed to Dijkstras "evaluate the node closest to starting position"
