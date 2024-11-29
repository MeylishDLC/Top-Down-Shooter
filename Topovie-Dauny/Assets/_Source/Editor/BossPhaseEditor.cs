using Enemies.Boss.Phases;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(BossPhase))]
    public class BossPhaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var serializedObj = new SerializedObject(target);
            var phaseTypeProperty = serializedObj.FindProperty("phaseType");
            var aProperty = serializedObj.FindProperty("a");
            var bProperty = serializedObj.FindProperty("b");

            EditorGUILayout.PropertyField(phaseTypeProperty);

            var phaseType = (BossPhase.PhaseType)phaseTypeProperty.enumValueIndex;
            if (phaseType == BossPhase.PhaseType.SingleAttackPhase)
            {
                EditorGUILayout.PropertyField(aProperty);
            }
            else if (phaseType == BossPhase.PhaseType.AttackCombinationPhase)
            {
                EditorGUILayout.PropertyField(bProperty);
            }

            DrawPropertiesExcluding(serializedObj, "phaseType", "a", "b");

            serializedObj.ApplyModifiedProperties();
        }
    }

}