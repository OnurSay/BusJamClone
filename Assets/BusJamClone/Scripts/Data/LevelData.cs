using System;
using System.Collections.Generic;
using BusJamClone.Scripts.Runtime.Models;
using static BusJamClone.Scripts.Data.LevelData;

namespace BusJamClone.Scripts.Data
{
    [Serializable]
    public struct GridCell
    {
        public int X;
        public int Y;
        public StickmanData stackData;

        // Add your serializable data here
    }

    [Serializable]
    public class LevelData
    {
        public enum GridColorType
        {
            None = 0,
            Red = 1,
            Green = 2,
            Blue = 3,
            Yellow = 4,
            Purple = 5,
            Orange = 6,
            Pink = 7,
            Close = 8
        }

        [Serializable]
        public struct StickmanData
        {
            public GridColorType stickmanColorType;
        }

        public int width => gridCells.GetLength(0);
        public int height => gridCells.GetLength(1);
        public GridCell[,] gridCells { get; set; }

        public LevelData(int width, int height)
        {
            gridCells = new GridCell[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    gridCells[x, y] = new GridCell
                    {
                        X = x,
                        Y = y
                    };
                }
            }
        }

        public GridCell[,] GetGrid() => gridCells;
        public GridCell GetGridCell(int x, int y) => gridCells[x, y];

        public void SetCellColor(int x, int y, GridColorType stickmanColor)
        {
            gridCells[x, y].stackData.stickmanColorType = stickmanColor;
        }

        public void RemoveCellColor(int x, int y)
        {
            gridCells[x, y].stackData.stickmanColorType = 0;
        }
    }
}