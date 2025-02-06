#if UNITY_EDITOR
using System.Collections.Generic;
using BusJamClone.Scripts.Data;
using BusJamClone.Scripts.Runtime.Models;
using BusJamClone.Scripts.Utilities;
using UnityEditor;
using UnityEngine;

namespace BusJamClone.Scripts.Runtime.LevelCreation
{
    public class LevelCreator : MonoBehaviour
    {
        [Header("Cached References")] [SerializeField]
        private GameObject gridBasePrefab;
        [SerializeField] private GameObject stickmanPrefab;
        [SerializeField] private GameObject busPrefab;
        [SerializeField] private GameColors gameColors;
        [SerializeField] private AddressablePrefabSaver prefabSaver;
        [SerializeField] private AddressablePrefabLoader prefabLoader;
        private GameObject currentParentObject;
        private GameObject loadedLevel;

        [Header("Level Settings")] [HideInInspector]
        public int gridWidth;
        [HideInInspector] public int gridHeight;
        [HideInInspector] public int levelIndex;
        [SerializeField] public List<LevelGoal> levelGoals;
        [SerializeField] private int levelTime;
        [SerializeField] private LevelData.GridColorType colorTypes;
        public bool isSecretStickman;
        public bool isReservedStickman;

        [Header("Constant Variables")] [SerializeField]
        private float spaceModifier;
        private LevelData levelData;

        public void GenerateLevel()
        {
            if (levelData != null)
                return;

            levelData = LevelSaveSystem.LoadLevel(levelIndex);
            if (levelData != null) return;
            levelData = new LevelData(gridWidth, gridHeight);
        }

        public void GridButtonAction(int x, int y)
        {
            levelData.SetCellSettings(x, y, colorTypes, isSecretStickman, isReservedStickman);
        }

        public void GridRemoveButtonAction(int x, int y)
        {
            levelData.RemoveCellSettings(x, y);
        }

        public void SaveLevel()
        {
            prefabSaver.SaveAndAssignPrefab(currentParentObject, levelIndex);
            EditorUtility.SetDirty(prefabSaver);
            LevelSaveSystem.SaveLevel(levelData, levelIndex);
        }

        public void LoadLevel()
        {
            levelData = LevelSaveSystem.LoadLevel(levelIndex);
            if(levelData == null) return;
            
            gridWidth = levelData.width;
            gridHeight = levelData.height;
            var prefabName = $"Level_{levelIndex}";
            if (loadedLevel)
            {
                DestroyImmediate(loadedLevel);
            }

            if (currentParentObject)
            {
                DestroyImmediate(currentParentObject);
            }

            loadedLevel = prefabLoader.ManualPrefabLoader(prefabName,
                (level) =>
                {
                    var container = level.GetComponent<LevelContainer>();
                    levelGoals = container.GetLevelGoals();
                    levelTime = container.GetLevelTime();
                    loadedLevel = level;
                });
            
        }

        public void ResetLevel()
        {
            levelData = new LevelData(gridWidth, gridHeight);
            prefabSaver.RemovePrefabFromAddressablesAndDelete(levelIndex);
        }

        public LevelData GetLevelData() => levelData;

        public void SpawnGrid()
        {
            var gridBases = new GridBase[gridWidth, gridHeight];
            var oldParentObject = GameObject.FindGameObjectWithTag("LevelParent");
            if (oldParentObject)
            {
                DestroyImmediate(oldParentObject);
            }

            var newParentObject = new GameObject("Level_" + levelIndex);
            var levelContainer = newParentObject.AddComponent<LevelContainer>();
            newParentObject.transform.tag = "LevelParent";

            var gridParentObject = new GameObject("GridParent");
            gridParentObject.transform.SetParent(newParentObject.transform);

            for (var y = 0; y < levelData.height; y++)
            {
                for (var x = 0; x < levelData.width; x++)
                {
                    var cell = levelData.GetGridCell(x, y);

                    var pos = transform.position + GridSpaceToWorldSpace(x, y);

                    var gridBaseObj = PrefabUtility.InstantiatePrefab(gridBasePrefab.gameObject) as GameObject;
                    if (!gridBaseObj) continue;
                    gridBaseObj.transform.SetParent(gridParentObject.transform);
                    gridBaseObj.transform.position = pos;

                    var gridBaseScript = gridBaseObj.GetComponent<GridBase>();
                    gridBases[x, y] = gridBaseScript;
                    if (cell.stackData.stickmanColorType is LevelData.GridColorType.None
                        or LevelData.GridColorType.Close)
                    {
                        gridBaseScript.Init(null, cell.stackData.stickmanColorType is LevelData.GridColorType.Close,
                            x,
                            y);
                        continue;
                    }

                    var stickman = PrefabUtility.InstantiatePrefab(stickmanPrefab.gameObject) as GameObject;
                    if (!stickman) continue;
                    stickman.transform.SetParent(gridBaseObj.transform);
                    stickman.transform.localPosition = Vector3.zero;
                    var stickmanScript = stickman.GetComponent<Stickman>();
                    stickmanScript.Init(cell.stackData.stickmanColorType, gridBaseScript);

                    gridBaseScript = gridBaseObj.GetComponent<GridBase>();
                    gridBaseScript.Init(stickmanScript, false, cell.x, cell.y);
                }
            }

            var levelBuses = SpawnLevelGoals(newParentObject.transform);

            levelContainer.Init(gridWidth, gridHeight, levelTime, gridBases, levelBuses, levelGoals);
            EditorUtility.SetDirty(levelContainer);
            currentParentObject = newParentObject;
        }

        private List<BusScript> SpawnLevelGoals(Transform levelParent)
        {
            var buses = new List<BusScript>();
            var busParent = new GameObject("BusParent");
            busParent.transform.SetParent(levelParent);
            busParent.transform.position = new Vector3(0, 0.60848f, -7.5f);
            var x = 5.75f;
            foreach (var levelGoal in levelGoals)
            {
                var bus = PrefabUtility.InstantiatePrefab(busPrefab) as GameObject;
                if (!bus) continue;
                bus.transform.SetParent(busParent.transform);
                bus.transform.localPosition = new Vector3(x, 0, 0);
                x += 5.75f;
                var busScript = bus.GetComponent<BusScript>();
                busScript.Init(levelGoal.colorType);
                buses.Add(busScript);
            }

            return buses;
        }

        private Vector3 GridSpaceToWorldSpace(int x, int y)
        {
            return new Vector3(x * spaceModifier * 0.9f - (gridWidth * spaceModifier * 0.9f / 2) + 0.575f,
                0, y * spaceModifier * 0.9f - 2f);
        }

        public GameColors GetGameColors()
        {
            return gameColors;
        }
    }

    [System.Serializable]
    public class LevelGoal
    {
        public LevelData.GridColorType colorType;
        public int reservedCount;
    }
}
#endif