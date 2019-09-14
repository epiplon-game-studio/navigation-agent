using UnityEditor;
using UnityEngine;

namespace vnc.AI.Editor
{
    [CustomEditor(typeof(NavigationAgent))]
    public class NavigationAgentEditor : UnityEditor.Editor
    {
        const float TOGGLE_WIDTH = 35f;

        SerializedProperty isEnabled;
        SerializedProperty axisX, axisY, axisZ;
        SerializedProperty positionSpeed;

        GUIStyle debugStyle;

        private void OnEnable()
        {
            isEnabled = serializedObject.FindProperty("m_enabled");
            axisX = serializedObject.FindProperty("axisX");
            axisY = serializedObject.FindProperty("axisY");
            axisZ = serializedObject.FindProperty("axisZ");
            positionSpeed = serializedObject.FindProperty("m_positionSpeed");


            debugStyle = new GUIStyle();
            var debugTexture = new Texture2D(1, 1);
            debugTexture.SetPixel(0, 0, Color.red);
            debugTexture.Apply();
            debugStyle.normal.background = debugTexture;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("GENERAL", EditorStyles.boldLabel);
            isEnabled.boolValue = EditorGUILayout.Toggle("Enabled", isEnabled.boolValue);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("POSITION", EditorStyles.boldLabel);
            positionSpeed.floatValue = EditorGUILayout.FloatField("Speed", positionSpeed.floatValue);
            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            Rect horizontalRect = EditorGUILayout.GetControlRect();
            Rect prefixRect = new Rect(horizontalRect.x, horizontalRect.y, EditorGUIUtility.labelWidth - horizontalRect.x, EditorGUIUtility.singleLineHeight);
            EditorGUI.PrefixLabel(prefixRect, new GUIContent("Axis Aligned"));

            Rect toggleRect = new Rect(prefixRect.x + EditorGUIUtility.labelWidth, horizontalRect.y, TOGGLE_WIDTH, EditorGUIUtility.singleLineHeight);
            axisX.boolValue = EditorGUI.ToggleLeft(toggleRect, "X", axisX.boolValue);
            toggleRect.x += TOGGLE_WIDTH;
            axisY.boolValue = EditorGUI.ToggleLeft(toggleRect, "Y", axisY.boolValue);
            toggleRect.x += TOGGLE_WIDTH;
            axisZ.boolValue = EditorGUI.ToggleLeft(toggleRect, "Z", axisZ.boolValue);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Separator();

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
