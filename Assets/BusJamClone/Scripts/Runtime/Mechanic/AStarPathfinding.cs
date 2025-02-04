using System.Collections.Generic;
using System.Linq;
using BusJamClone.Scripts.Runtime.Models;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Mechanic
{
    public class AStarPathfinding
    {
        private readonly GridBase[,] gridBases;
        private readonly int gridWidth;
        private readonly int gridHeight;

        public AStarPathfinding(GridBase[,] grid)
        {
            gridBases = grid;
            gridWidth = grid.GetLength(0);
            gridHeight = grid.GetLength(1);
        }

        public List<GridBase> FindPath(Vector2Int startPos)
        {
            var openSet = new List<GridBase>();
            var closedSet = new HashSet<GridBase>();

            var startNode = gridBases[startPos.x, startPos.y];
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = GetLowestFCostNode(openSet);
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                currentNode.visited = true;

                if (currentNode.y == 0)
                    return RetracePath(startNode, currentNode);

                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (neighbor.visited || neighbor.isClosed || closedSet.Contains(neighbor) || neighbor.stickman)
                        continue;

                    var newGCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (!(newGCost < neighbor.gCost) && openSet.Contains(neighbor)) continue;
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetHeuristic(neighbor);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }

            return null;
        }

        private GridBase GetLowestFCostNode(List<GridBase> openSet)
        {
            var bestNode = openSet[0];
            foreach (var node in openSet.Where(node => node.fCost < bestNode.fCost))
                bestNode = node;
            return bestNode;
        }

        private List<GridBase> GetNeighbors(GridBase node)
        {
            var neighbors = new List<GridBase>();
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var dir in directions)
            {
                var newX = node.x + dir.x;
                var newY = node.y + dir.y;

                if (newX >= 0 && newX < gridWidth && newY >= 0 && newY < gridHeight)
                    neighbors.Add(gridBases[newX, newY]);
            }

            return neighbors;
        }

        private float GetDistance(GridBase a, GridBase b)
        {
            return Vector2Int.Distance(new Vector2Int(a.x, a.y), new Vector2Int(b.x, b.y));
        }

        private float GetHeuristic(GridBase node)
        {
            return node.y; // Lower y-values are better
        }

        private List<GridBase> RetracePath(GridBase startNode, GridBase endNode)
        {
            var path = new List<GridBase>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }
    
        public void ResetVisitedStates()
        {
            foreach (var gridBase in gridBases)
            {
                gridBase.ResetVisited();
            }
        }
    }
}
