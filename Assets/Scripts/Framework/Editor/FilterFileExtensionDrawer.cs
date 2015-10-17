using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer (typeof (FilterFileExtensionAttribute))]
public class FilterFileExtensionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        FilterFileExtensionAttribute attr = attribute as FilterFileExtensionAttribute;
        if (property.objectReferenceValue != null) {
            string path = AssetDatabase.GetAssetPath (property.objectReferenceValue);
            if (path.EndsWith (attr.fileExtension) == false) {
                Debug.LogErrorFormat ("{0} is not {1}", property.objectReferenceValue.name, attr.fileExtension);
                property.objectReferenceValue = null;
            }
        }
        EditorGUI.PropertyField (position, property, label);
    }
}
