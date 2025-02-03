using System.Collections.Generic;
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

        public float radius;
        public Vector3 direction;
        public float distance;
        public List<GameObject> neighbourGridBases;
        public LayerMask gridAreaLayer;
        public bool isSecret;
        public bool isClosed;

        private void Start()
        {
            if (!stickman) return;
            if (!isSecret) return;
            stickman.SetSecret();
        }
        public void Init(Stickman stkMan, int x, int y)
        {
            this.x = x;
            this.y = y;
            
            if (!stkMan)
            {
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
    }
}