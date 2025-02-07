using System.Collections.Generic;
using BusJamClone.Scripts.Runtime.LevelCreation;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using BusJamClone.Scripts.Data;

namespace BusJamClone.Scripts.Editor
{
    [CustomEditor(typeof(LevelCreator))]
    public class LevelCreatorEditor : UnityEditor.Editor
    {
        private LevelCreator levelCreator;

        private void OnEnable()
        {
            levelCreator = (LevelCreator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            levelCreator.GenerateLevel();
            DrawGridProperties();

            if (!IsLevelDataAvailable())
            {
                EditorGUILayout.HelpBox("Please regenerate the Grid!", MessageType.Error);
                return;
            }

            DrawGrid();
            DrawSaveLoadButtons(DisplayColorStatus());

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(levelCreator, "Change Level Goals");
                EditorUtility.SetDirty(levelCreator);
                Repaint();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool IsLevelDataAvailable()
        {
            var isLevelDataExist = levelCreator.GetLevelData() != null;
            if (!isLevelDataExist)
                return false;

            var isLevelGridExist = levelCreator.GetLevelData().GetGrid() != null;
            return isLevelGridExist;
        }

        private void DrawGridProperties()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grid Width");
            levelCreator.gridWidth = EditorGUILayout.IntField(levelCreator.gridWidth);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grid Height");
            levelCreator.gridHeight = EditorGUILayout.IntField(levelCreator.gridHeight);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level Index");
            levelCreator.levelIndex = EditorGUILayout.IntField(levelCreator.levelIndex);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Grid"))
            {
                levelCreator.SpawnGrid();
            }

            if (GUILayout.Button("Reset"))
            {
                levelCreator.ResetLevel();
            }

            if (GUILayout.Button("Load"))
            {
                levelCreator.LoadLevel();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGrid()
        {
            var style = new GUIStyle(GUI.skin.button) { fontSize = 64 };
            var grid = levelCreator.GetLevelData().GetGrid();
            if (ReferenceEquals(grid, null) || grid.Length.Equals(0))
            {
                return;
            }

            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);

            for (var y = 0; y < levelCreator.gridHeight; y++)
            {
                EditorGUILayout.BeginHorizontal();

                for (var x = levelCreator.gridWidth - 1; x >= 0; x--)
                {
                    EditorGUILayout.BeginVertical();

                    var cell = levelCreator.GetLevelData().GetGridCell(x, y);

                    var subColors = new List<Color>
                        { levelCreator.GetGameColors().activeColors[(int)cell.stackData.stickmanColorType] };
                    var text = "";

                    if (cell.stackData.isSecret)
                    {
                        if (text == "")
                        {
                            text += "S";
                        }
                        else
                        {
                            text += "," + "S";
                        }
                    }

                    if (cell.stackData.isReserved)
                    {
                        if (text == "")
                        {
                            text += "R";
                        }
                        else
                        {
                            text += "," + "R";
                        }
                    }

                    var buttonRect = GUILayoutUtility.GetRect(new GUIContent(text), style, GUILayout.Width(75),
                        GUILayout.Height(75));

                    var textStyle = new GUIStyle
                    {
                        fontSize = 16,
                        fontStyle = FontStyle.Bold,
                        normal = { textColor = Color.black },
                    };


                    Rect[] subRects =
                    {
                        new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height)
                    };

                    for (var t = 0; t < subRects.Length; t++)
                    {
                        EditorGUI.DrawRect(subRects[t],
                            subColors[t] == Color.black
                                ? Color.black + new Color(0.1f * t, 0.1f * t, 0.1f * t)
                                : subColors[t]);
                    }

                    Handles.Label(
                        new Vector3(buttonRect.x, buttonRect.y, 0),
                        text, textStyle);


                    for (var o = subRects.Length - 1; o >= 0; o--)
                    {
                        if ((Event.current.type != EventType.MouseDrag && Event.current.type != EventType.MouseDown) ||
                            !subRects[o].Contains(Event.current.mousePosition)) continue;

                        Event.current.Use();

                        switch (Event.current.button)
                        {
                            case 0:
                                levelCreator.GridButtonAction(x, y);
                                break;
                            case 1:
                                levelCreator.GridRemoveButtonAction(x, y);
                                break;
                        }
                    }


                    EditorGUILayout.EndVertical();
                    GUI.backgroundColor = Color.white;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5f);
            }
        }

        private void DrawSaveLoadButtons(bool canInteractable)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Don't forget the save grid!", MessageType.Warning);
            EditorGUILayout.LabelField("Save/Load", EditorStyles.boldLabel);

            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = canInteractable;
            if (GUILayout.Button("Save"))
            {
                levelCreator.SaveLevel();
            }


            EditorGUILayout.EndHorizontal();
        }

        private bool DisplayColorStatus()
        {
            var grid = levelCreator.GetLevelData().GetGrid();
            var errorCount = 0;
            foreach (LevelData.GridColorType colorType in Enum.GetValues(typeof(LevelData.GridColorType)))
            {
                if (colorType is LevelData.GridColorType.None or LevelData.GridColorType.Close)
                    continue;

                var colorCount = grid.Cast<GridCell>().Count(cell => cell.stackData.stickmanColorType == colorType);
                var reservedColorCount = grid.Cast<GridCell>().Count(cell =>
                    cell.stackData.stickmanColorType == colorType && cell.stackData.isReserved);
                if (colorCount == 0)
                    continue;

                var colorName = colorType.ToString();
                var busCount = DisplayBusStatus(colorType);
                var reservedBusCount = DisplayReservedBusCount(colorType);
                if (colorCount % 3 == 0)
                {
                    if (busCount == colorCount / 3)
                    {
                        if (reservedColorCount == reservedBusCount)
                        {
                            EditorGUILayout.HelpBox(
                                $"{colorName} Color: OK ({colorCount} {colorName} Color with {busCount} Bus Count that has {reservedColorCount})",
                                MessageType.Info);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox(
                                $"{colorName} Color: NOT OK ({colorCount} {colorName} Color with {busCount} Bus Count that has {reservedColorCount})",
                                MessageType.Error);
                            errorCount++;
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(
                            $"{colorName} Color: NOT OK ({colorCount} {colorName} Color with {busCount} Bus Count that has {reservedColorCount})",
                            MessageType.Error);
                        errorCount++;
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        $"{colorName} Color: NOT OK ({colorCount} {colorName} Color with {busCount} Bus Count that has {reservedColorCount})",
                        MessageType.Error);
                    errorCount++;
                }
            }

            return errorCount == 0;
        }

        private int DisplayReservedBusCount(LevelData.GridColorType colorType)
        {
            if (levelCreator.levelGoals == null || levelCreator.levelGoals.Count == 0)
                return 0;

            var sameColorBuses = levelCreator.levelGoals.Where(goal => colorType == goal.colorType);
            return sameColorBuses.Sum(colorBus => colorBus.reservedCount);
        }

        private int DisplayBusStatus(LevelData.GridColorType gridColorType)
        {
            if (levelCreator.levelGoals == null || levelCreator.levelGoals.Count == 0)
                return 0;

            return levelCreator.levelGoals.Count(goal => gridColorType == goal.colorType);
        }
    }
}