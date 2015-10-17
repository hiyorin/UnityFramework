using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer (typeof (LabelAttribute))]
public class LabelDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SetLabel (label);
        EditorGUI.PropertyField (position, property, label, true);
    }

    protected void SetLabel (GUIContent label)
    {
        LabelAttribute labelAttribute = attribute as LabelAttribute;
        label.text = labelAttribute.label;
    }
}

[CustomPropertyDrawer (typeof (LabelRangeAttribute))]
public class LabelRangeDrawer : LabelDrawer
{
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        SetLabel (label);
        LabelRangeAttribute labelAttribute = attribute as LabelRangeAttribute;
        switch (property.propertyType)
        {
        case SerializedPropertyType.Integer:
            property.intValue = (int)EditorGUI.Slider (position, label, property.intValue, labelAttribute.min, labelAttribute.max);
            return;
        case SerializedPropertyType.Float:
            EditorGUI.Slider (position, property, labelAttribute.min, labelAttribute.max, label);
            return;
        }
        base.OnGUI (position, property, label);
    }
}