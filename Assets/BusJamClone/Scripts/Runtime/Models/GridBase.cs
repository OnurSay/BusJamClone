using System.Collections.Generic;
using BusJamClone.Scripts.Runtime.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace BusJamClone.Scripts.Runtime.Models
{
    public class GridBase : MonoBehaviour
    {
        public Stickman stickman;
        public Renderer gridRenderer;
        public GameObject wallObject;
        public int x, y;
        public bool isSecret;
        public bool isClosed;
        public bool visited;
        public float gCost, hCost;
        public float fCost => gCost + hCost;
        public GridBase parent;
        public bool hasPath;
        public List<GridBase> closestPath;

        private void Start()
        {
            if (!stickman) return;
            if (!isSecret) return;
            stickman.SetSecret();
        }

        public void HandlePath()
        { 
            if (!stickman) return;
            closestPath = GridManager.instance.pathfinder.FindPath(new Vector2Int(x, y));
            GridManager.instance.pathfinder.ResetVisitedStates();
            if (closestPath == null)
            {
                hasPath = false;
                stickman.DisableInteraction();
            }
            else
            {
                hasPath = true;
                stickman.EnableInteraction();
            }
        }

        public void Init(Stickman stkMan, bool flag, int x, int y)
        {
            this.x = x;
            this.y = y;

            if (!stkMan)
            {
                if (!flag) return;
                isClosed = true;
                EnableWall();

                return;
            }

            stickman = stkMan;
        }

        private void EnableWall()
        {
            gridRenderer.enabled = false;
            wallObject.SetActive(true);
        }
        
        public void ResetVisited()  
        {
            visited = false;
        }

        public void DissociateStickman()
        {
            stickman = null;
            hasPath = false;
        }
    }
}