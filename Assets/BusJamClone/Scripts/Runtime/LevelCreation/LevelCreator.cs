#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.Managers;
using BusJamClone.Scripts.Runtime.Models;
using UnityEditor;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.LevelCreation
{
    public class LevelCreator : MonoBehaviour
    {
        [HideInInspector] public int gridWidth;
        [HideInInspector] public int gridHeight;
        [HideInInspector] public int levelIndex;
        [SerializeField] private GameObject gridBasePrefab;
        [SerializeField] private GameObject stickmanPrefab;
        [SerializeField] private GameObject busPrefab;
        [SerializeField] private GameplayManager gameplayManager;
        public GameColors gameColors;
        [SerializeField] private float spaceModifier;

        [Header("Level Settings")] [SerializeField]
        private List<LevelGoal> levelGoals;

        [SerializeField] private int levelTime;

        [SerializeField] private LevelData.GridColorType colorTypes;


        private LevelData _levelData;

        public void GenerateLevel()
        {
            if (_levelData != null)
                return;
            //Debug.Log(_levelData.GetGridCount() + " 1");
            _levelData = LevelSaveSystem.LoadLevel(levelIndex);
            if (_levelData == null)
            {
                _levelData = new LevelData(gridWidth, gridHeight);
                SaveLevel();
            }
        }

        public void GridButtonAction(int x, int y)
        {
            _levelData.SetCellColor(x, y, colorTypes);
        }

        public void GridRemoveButtonAction(int x, int y)
        {
            _levelData.RemoveCellColor(x, y);
        }

        public void SaveLevel() => LevelSaveSystem.SaveLevel(_levelData, levelIndex);

        public void LoadLevel() => _levelData = LevelSaveSystem.LoadLevel(levelIndex);

        public void ResetLevel()
        {
            _levelData = new LevelData(gridWidth, gridHeight);
            //SaveLevel();
        }

        public LevelData GetLevelData() => _levelData;

        public void SpawnGrid()
        {
            SaveLevel();

            var gridBases = new GridBase[gridWidth,gridHeight];
            var oldParentObject = GameObject.FindGameObjectWithTag("LevelParent");
            if (oldParentObject)
            {
                DestroyImmediate(oldParentObject);
            }

            var newParentObject = new GameObject("Level_" + levelIndex);
            newParentObject.transform.tag = "LevelParent";


            var gridParentObject = new GameObject("GridParent");
            gridParentObject.transform.SetParent(newParentObject.transform);


            for (var y = 0; y < _levelData.height; y++)
            {
                for (var x = 0; x < _levelData.width; x++)
                {
                    var cell = _levelData.GetGridCell(x, y);

                    var pos = transform.position + GridSpaceToWorldSpace(x, y);

                    var gridBaseObj = PrefabUtility.InstantiatePrefab(gridBasePrefab.gameObject) as GameObject;
                    gridBaseObj.transform.SetParent(gridParentObject.transform);
                    gridBaseObj.transform.position = pos;

                    var gridBaseScript = gridBaseObj.GetComponent<GridBase>();
                    gridBases[x, y] = gridBaseScript;
                    if (cell.stackData.stickmanColorType is LevelData.GridColorType.None
                        or LevelData.GridColorType.Close)
                    {
                        gridBaseScript.Init(null, x, y);
                        continue;
                    }

                    var stickman = PrefabUtility.InstantiatePrefab(stickmanPrefab.gameObject) as GameObject;
                    stickman.transform.SetParent(gridBaseObj.transform);
                    stickman.transform.localPosition = Vector3.zero;
                    var stickmanScript = stickman.GetComponent<Stickman>();
                    stickmanScript.Init(cell.stackData.stickmanColorType, cell);

                    gridBaseScript = gridBaseObj.GetComponent<GridBase>();
                    gridBaseScript.Init(stickmanScript, cell.X, cell.Y);
                }
            }

            SpawnLevelGoals(newParentObject.transform);


            // var scoreManager = FindObjectOfType<ScoreManager>();
            // EditorUtility.SetDirty(scoreManager);
            var gridManager = FindObjectOfType<GridManager>();
            gridManager.Init(gridWidth, gridHeight, gridBases);
            EditorUtility.SetDirty(gridManager);
        }

        private void SpawnLevelGoals(Transform levelParent)
        {
            var busParent = new GameObject("BusParent");
            busParent.transform.SetParent(levelParent);
            busParent.transform.position = new Vector3(0, 0.60848f, -9.21f);
            var x = 5.75f;
            for (var i = 0; i < levelGoals.Count; i++)
            {
                var bus = PrefabUtility.InstantiatePrefab(busPrefab) as GameObject;
                bus.transform.SetParent(busParent.transform);
                bus.transform.localPosition = new Vector3(x, 0, 0);
                x += 5.75f;
                var busScript = bus.GetComponent<BusScript>();
                busScript.Init(levelGoals[i].colorType);
                gameplayManager.AddBus(busScript);
            }
        }

        private Vector3 CalculateUpperCenter(int width, int height)
        {
            return new Vector3((width * spaceModifier - spaceModifier) / 2, 0, spaceModifier * height - spaceModifier);
        }

        private Vector3 GridSpaceToWorldSpace(int x, int y)
        {
            return new Vector3(x * spaceModifier * 0.9f - (gridWidth * spaceModifier * 0.9f / 2) + 0.575f,
                0, y * spaceModifier * 0.9f - (gridWidth * spaceModifier * 0.9f / 2) + 0.7f);
        }
    }

    [System.Serializable]
    public class LevelGoal
    {
        public LevelData.GridColorType colorType;
    }
#endif
}