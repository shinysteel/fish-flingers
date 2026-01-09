using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ShinyOwl.Common.Structures
{
    [CustomEditor(typeof(BoolGrid))]
    public class BoolGridEditor : Editor
    {
        private SerializedProperty _columnsProperty;
        private SerializedProperty _rowsProperty;
        private SerializedProperty _boolsProperty;

        private const int MaxSize = 10; // Columns & Rows
        private const float ToggleSize = 20f;

        private void OnEnable()
        {
            _columnsProperty = serializedObject.FindProperty(BoolGrid.ColumnsName);
            _rowsProperty = serializedObject.FindProperty(BoolGrid.RowsName);
            _boolsProperty = serializedObject.FindProperty(BoolGrid.BoolsName);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDimensions();
            EditorGUILayout.Space();
            DrawBoolGrid();
            EditorGUILayout.Space();
            DrawToggleAll();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDimensions()
        {
            EditorGUILayout.LabelField("Dimensions", EditorStyles.boldLabel);

            int oldColumns = _columnsProperty.intValue;
            int oldRows = _rowsProperty.intValue;

            EditorGUILayout.PropertyField(_columnsProperty);
            _columnsProperty.intValue = Mathf.Clamp(_columnsProperty.intValue, 1, MaxSize);

            EditorGUILayout.PropertyField(_rowsProperty);
            _rowsProperty.intValue = Mathf.Clamp(_rowsProperty.intValue, 1, MaxSize);

            ResizeBools(oldColumns, oldRows);
        }

        private void ResizeBools(int oldColumns, int oldRows)
        {
            int newColumns = _columnsProperty.intValue;
            int newRows = _rowsProperty.intValue;
            int newSize = newColumns * newRows;

            if (_boolsProperty.arraySize == newSize)
            {
                return;
            }

            bool[] oldBools = new bool[oldColumns * oldRows];
            for (int y = 0; y < oldRows; y++)
            {
                for (int x = 0; x < oldColumns; x++)
                {
                    int index = y * oldColumns + x;
                    oldBools[index] = _boolsProperty.GetArrayElementAtIndex(index).boolValue;
                }
            }

            _boolsProperty.arraySize = newSize;

            // Don't entirely understand it, but we need to reset all values in the new array
            for (int i = 0; i < _boolsProperty.arraySize; i++)
            {
                _boolsProperty.GetArrayElementAtIndex(i).boolValue = false;
            }

            for (int y = 0; y < Mathf.Min(oldRows, newRows); y++)
            {
                for (int x = 0; x < Mathf.Min(oldColumns, newColumns); x++)
                {
                    _boolsProperty.GetArrayElementAtIndex(y * newColumns + x).boolValue = oldBools[y * oldColumns + x];
                }
            }
        }

        private void DrawBoolGrid()
        {
            int columns = _columnsProperty.intValue;
            int rows = _rowsProperty.intValue;

            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical();

            // Builds rows first so we can keep using our local y value
            for (int y = 0; y < rows; y++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < columns; x++)
                {
                    int index = y * columns + x;
                    SerializedProperty boolProperty = _boolsProperty.GetArrayElementAtIndex(index);
                    boolProperty.boolValue = EditorGUI.Toggle(GUILayoutUtility.GetRect(ToggleSize, ToggleSize, GUILayout.ExpandWidth(false)), boolProperty.boolValue);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawToggleAll()
        {
            if (GUILayout.Button("Toggle All"))
            {
                bool toggle = true;

                // If at least one bool is true, we will set them all to false 
                for (int i = 0; i < _boolsProperty.arraySize; i++)
                {
                    if (_boolsProperty.GetArrayElementAtIndex(i).boolValue)
                    {
                        toggle = false;
                        break;
                    }
                }

                for (int i = 0; i < _boolsProperty.arraySize; i++)
                {
                    _boolsProperty.GetArrayElementAtIndex(i).boolValue = toggle;
                }
            }
        }
    }

    [CreateAssetMenu(fileName = "BoolGrid", menuName = "Structures/BoolGrid")]
    public class BoolGrid : ScriptableObject
    {
        [SerializeField] private int _columns;
        [SerializeField] private int _rows;
        [SerializeField] private bool[] _bools;

        public static string ColumnsName => nameof(_columns);
        public static string RowsName => nameof(_rows);
        public static string BoolsName => nameof(_bools);

        public int Columns => _columns;
        public int Rows => _rows;

        public bool this[int x, int y]
        {
            get => _bools[y * _columns + x];
        }
    }
}