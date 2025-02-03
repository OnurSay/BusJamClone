using System.Collections.Generic;
using BusJamClone.Scripts.Runtime.LevelCreation;
using UnityEditor;
using UnityEngine;

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
            DrawSaveLoadButtons();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(levelCreator, "Change Level Index");
                EditorUtility.SetDirty(levelCreator);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool IsLevelDataAvailable()
        {
            var isLevelDataExist = levelCreator.GetLevelData() != null;
            if (!isLevelDataExist)
                return false;

            var isLevelGridExist = levelCreator.GetLevelData().GetGrid() != null;
            if (!isLevelGridExist)
            {
                return false;
            }

            var isGridBoundsCorrect = (levelCreator.gridWidth * levelCreator.gridHeight) ==
                                      levelCreator.GetLevelData().GetGrid().Length;
            return isGridBoundsCorrect;
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
            var style = new GUIStyle(GUI.skin.button) { fontSize = 8 };
            var grid = levelCreator.GetLevelData().GetGrid();
            if (ReferenceEquals(grid, null) || grid.Length.Equals(0))
            {
                return;
            }

            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);

            for (var y = 0; y < levelCreator.gridHeight; y++)
            {
                EditorGUILayout.BeginHorizontal();

                for (var x = levelCreator.gridWidth-1; x >= 0 ; x--)
                {
                    EditorGUILayout.BeginVertical();

                    var cellText = x + "x" + y;

                    var cell = levelCreator.GetLevelData().GetGridCell(x, y);

                    var subColors = new List<Color> { levelCreator.gameColors.ActiveColors[(int)cell.stackData.stickmanColorType] };

                    var buttonRect = GUILayoutUtility.GetRect(new GUIContent(cellText), style, GUILayout.Width(75),
                        GUILayout.Height(75));

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

        private void DrawSaveLoadButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Don't forget the save grid!", MessageType.Warning);
            EditorGUILayout.LabelField("Save/Load", EditorStyles.boldLabel);

            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginHorizontal();


            if (GUILayout.Button("Save"))
            {
                levelCreator.SaveLevel();
            }


            EditorGUILayout.EndHorizontal();
        }
    }
}