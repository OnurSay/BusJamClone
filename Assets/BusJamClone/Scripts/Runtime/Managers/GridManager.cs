using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.Mechanic;
using BusJamClone.Scripts.Runtime.Models;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.Managers
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager instance;
        public int width, height;
        [SerializeField] private GridBase[,] gridBaseArray;
        public AStarPathfinding pathfinder;
        [SerializeField] private LevelContainer currentLevel;

        private void Awake()
        {
            MakeSingleton();
        }

        private void InitializePathfinder()
        {
            pathfinder = new AStarPathfinding(gridBaseArray);
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

        public void Init(int w, int h, GridBase[,] gridBases, LevelContainer level)
        {
            width = w;
            height = h;
            
            gridBaseArray = gridBases;
            currentLevel = level;
            InitializePathfinder();
        }

        public void RecalculatePaths()
        {
            currentLevel.HandleGridBasesPathfinding(gridBaseArray);
        }
        
    }
}