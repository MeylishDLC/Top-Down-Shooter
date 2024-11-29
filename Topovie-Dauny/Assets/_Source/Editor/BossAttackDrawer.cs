using Enemies.Boss.BossAttacks;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(BossAttackAttribute))]
    public class BossAttackDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, label);

            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;

            property.objectReferenceValue = EditorGUI.ObjectField(
                position,
                property.objectReferenceValue,
                typeof(MonoBehaviour),
                true
            );

            if (property.objectReferenceValue != null &&
                !(property.objectReferenceValue is IBossAttack))
            {
                property.objectReferenceValue = null;
                Debug.LogError("Assigned object must implement IBossAttack.");
            }
        }
    }
}