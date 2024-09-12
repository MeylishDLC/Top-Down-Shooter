using System.Collections.Generic;
using _Support.Editor_Toolbox.Editor.ContextMenu.Operations.SerializeReference;
using Toolbox.Editor.ContextMenu;
using Toolbox.Editor.ContextMenu.Operations;
using UnityEditor;

namespace _Support.Editor_Toolbox.Editor.ContextMenu.Management
{
    [InitializeOnLoad]
    internal static class ToolboxContextMenuManager
    {
        private static readonly List<IContextMenuOperation> registeredOperations;

        static ToolboxContextMenuManager()
        {
            registeredOperations = new List<IContextMenuOperation>()
            {
                new CopySerializeReferenceOperation(),
                new PasteSerializeReferenceOperation(),
                new DuplicateSerializeReferenceArrayElementOperation()
            };

            EditorApplication.contextualPropertyMenu -= OnContextMenuOpening;
            EditorApplication.contextualPropertyMenu += OnContextMenuOpening;
        }

        public static void AppendOpertation(IContextMenuOperation operation)
        {
            registeredOperations.Add(operation);
        }

        public static bool RemoveOperation(IContextMenuOperation operation)
        {
            return registeredOperations.Remove(operation);
        }

        private static void OnContextMenuOpening(GenericMenu menu, SerializedProperty property)
        {
            foreach (var operation in registeredOperations)
            {
                if (!operation.IsVisible(property))
                {
                    continue;
                }

                var label = operation.Label;
                if (!operation.IsEnabled(property))
                {
                    menu.AddDisabledItem(label);
                    continue;
                }

                menu.AddItem(label, false, () =>
                {
                    operation.Perform(property);
                });
            }
        }
    }
}