using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

// https://discussions.unity.com/t/making-a-proper-drawer-similar-to-vector3-how/616416/4

[CustomPropertyDrawer(typeof(Hex))]
public class HexPropertyDrawer : PropertyDrawer
{
    private const float SubLabelSpacing = 4;
    private const float BottomSpacing = 2;

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        pos.height -= BottomSpacing;
        label = EditorGUI.BeginProperty(pos, label, prop);
        var contentRect = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
        var labels = new[] { new GUIContent("Q"), new GUIContent("R"), new GUIContent("S") };
        var properties = new[] { prop.FindPropertyRelative("q"), prop.FindPropertyRelative("r"), null};
        DrawMultiplePropertyFields(contentRect, labels, properties);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + BottomSpacing;
    }


    private static void DrawMultiplePropertyFields(Rect pos, GUIContent[] subLabels, SerializedProperty[] props)
    {
        // backup gui settings
        var indent = EditorGUI.indentLevel;
        var labelWidth = EditorGUIUtility.labelWidth;

        // draw properties
        var propsCount = props.Length;
        var width = (pos.width - (propsCount - 1) * SubLabelSpacing) / propsCount;
        var contentPos = new Rect(pos.x, pos.y, width, pos.height);
        EditorGUI.indentLevel = 0;
        for (var i = 0; i < propsCount; i++)
        {
            EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(subLabels[i]).x;
            if(i == 2)
            {
                using var _ = new EditorGUI.DisabledGroupScope(true);
                EditorGUI.IntField(contentPos, subLabels[i], -props[0].intValue - props[1].intValue);
            } else
            {
                EditorGUI.PropertyField(contentPos, props[i], subLabels[i]);
            }
            contentPos.x += width + SubLabelSpacing;
        }

        // restore gui settings
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.indentLevel = indent;
    }
}
