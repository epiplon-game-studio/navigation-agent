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
        SerializedProperty positionSpeed, rotationSpeed;
        SerializedProperty rotationStyle;
        SerializedProperty rotationPrecision;
        SerializedProperty repathDelay;
        SerializedProperty navmeshAreas;
        SerializedProperty advancedOptions;
        
        GUIStyle debugStyle;

        private void OnEnable()
        {
            isEnabled = serializedObject.FindProperty("m_enabled");
            axisX = serializedObject.FindProperty("axisX");
            axisY = serializedObject.FindProperty("axisY");
            axisZ = serializedObject.FindProperty("axisZ");
            positionSpeed = serializedObject.FindProperty("m_positionSpeed");
            rotationSpeed = serializedObject.FindProperty("m_rotationSpeed");
            rotationStyle = serializedObject.FindProperty("rotationStyle");
            rotationPrecision = serializedObject.FindProperty("rotationPrecision");
            repathDelay = serializedObject.FindProperty("repathDelay");
            navmeshAreas = serializedObject.FindProperty("navmeshAreas");
            advancedOptions = serializedObject.FindProperty("advancedOptions");

            debugStyle = new GUIStyle();
            var debugTexture = new Texture2D(1, 1);
            debugTexture.SetPixel(0, 0, Color.red);
            debugTexture.Apply();
            debugStyle.normal.background = debugTexture;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("GENERAL", EditorStyles.boldLabel);
            isEnabled.boolValue = EditorGUILayout.Toggle("Enabled", isEnabled.boolValue);
            EditorGUILayout.Space();

            #region POSITION
            EditorGUILayout.LabelField("POSITION", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(positionSpeed);

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
            EditorGUILayout.Space();
            #endregion

            #region ROTATION
            EditorGUILayout.LabelField("ROTATION", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(rotationSpeed);
            EditorGUILayout.PropertyField(rotationStyle);
            #endregion

            advancedOptions.boolValue = EditorGUILayout.Foldout(advancedOptions.boolValue, "Advanced");
            if (advancedOptions.boolValue)
            {
                EditorGUI.PropertyField(FoldRect(), rotationPrecision);

                var repathContent = new GUIContent("Repath Delay (seconds)", "Value in seconds");
                repathDelay.floatValue = EditorGUI.Slider(FoldRect(), repathContent, repathDelay.floatValue, 0.01f, 9f);
                navmeshAreas.intValue = EditorGUI.MaskField(FoldRect(), "Area Mask",  navmeshAreas.intValue, GameObjectUtility.GetNavMeshAreaNames());

                EditorGUILayout.Space();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #region Utils
        Rect FoldRect()
        {
            Rect r = EditorGUILayout.GetControlRect();
            r.x += 10f;
            r.width -= 10f;
            return r;
        }
        #endregion
    }
}
