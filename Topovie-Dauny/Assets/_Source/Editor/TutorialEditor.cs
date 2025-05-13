using Tutorial;
using Tutorial.Scenarios.ScenariosTypes;

namespace Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    [CustomEditor(typeof(BasicTutorial))]
    public class TutorialEditor: Editor
    {
        private static Type[] _scenarioTypes;

        private void OnEnable()
        {
            _scenarioTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ITutorialScenario).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToArray();
        }

        public override void OnInspectorGUI()
        {
            var tutorial = (BasicTutorial)target;

            if (GUILayout.Button("Add Scenario"))
            {
                var menu = new GenericMenu();
                foreach (var t in _scenarioTypes)
                {
                    menu.AddItem(new GUIContent(t.Name), false, () =>
                    {
                        var instance = (ITutorialScenario)Activator.CreateInstance(t);
                        tutorial.scenarios.Add(instance);
                    });
                }
                menu.ShowAsContext();
            }

            var listProp = serializedObject.FindProperty("scenarios");

            for (int i = 0; i < listProp.arraySize; i++)
            {
                var element = listProp.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element, new GUIContent($"Scenario {i}"), true);
            }

            serializedObject.ApplyModifiedProperties();
            
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogueOnTutorialStart"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogueOnTutorialEnd"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shopTrigger"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("portalTrigger"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeZone"));

            serializedObject.ApplyModifiedProperties();
        }
    }
    
#endif
}