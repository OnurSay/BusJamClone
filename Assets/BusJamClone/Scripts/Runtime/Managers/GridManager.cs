using System;
using System.Collections.Generic;
using BusJamClone.Scripts.Runtime.Models;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager instance;
        public int width, height;
        [SerializeField] public GridBase[,] gridBaseArray;

        private void Awake()
        {
            MakeSingleton();
        }

        private void MakeSingleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
                Destroy(this);
        }

        public void Init(int w, int h, GridBase[,] gridBases)
        {
            width = w;
            height = h;

            gridBaseArray = gridBases;
        }

        public GridBase[,] GetGrid()
        {
            return gridBaseArray;
        }
        
    }

    [System.Serializable]
    public class IntList
    {
        public List<int> list = new List<int>();
    }
}