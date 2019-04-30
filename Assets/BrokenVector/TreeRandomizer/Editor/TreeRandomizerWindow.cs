using UnityEngine;
using UnityEditor;

using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace BrokenVector.TreeRandomizer
{
    public sealed class TreeRandomizerWindow : EditorWindow
    {
        private Tree treeTemplate = null;
        private int treeCount = 1;
        private bool cloneMaterials = false;

        private static TreeRandomizerWindow GetWindowInstance()
        {
            var window = GetWindow<TreeRandomizerWindow>();

            #if UNITY_5_4_OR_NEWER
                window.titleContent = new GUIContent(Constants.ASSET_NAME);
            #else
                window.title = Constants.ASSET_NAME;
            #endif

            window.Show();

            return window;
        }
        
        [MenuItem("CONTEXT/Tree/Open in TreeRandomizer")]
        public static void TreeContextMenu(MenuCommand cmd)
        {
            Tree tree = cmd.context as Tree;
            if (tree == null)
                return;

            ShowWindow(tree);
        }

        //Menu Item
        [MenuItem(Constants.WINDOW_PATH), MenuItem(Constants.WINDOW_PATH_ALT)]
        public static void ShowWindow()
        {
            GetWindowInstance();
        }

        //For use with context Menus
        public static void ShowWindow(Tree treeTemplate)
        {
            var window = GetWindowInstance();

            window.treeTemplate = treeTemplate;
        }

        void OnGUI()
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("You can only generate trees while not in playmode!", MessageType.Info);
                return;
            }

            HandleDragDrop();

            treeTemplate = EditorGUILayout.ObjectField("Tree Template", treeTemplate, typeof(Tree), true) as Tree;
            treeCount = EditorGUILayout.IntSlider("Tree Count", treeCount, 1, Constants.SLIDER_TREE_COUNT_MAX);
            cloneMaterials = EditorGUILayout.ToggleLeft("Clone Materials", cloneMaterials);

            if (GUILayout.Button("Generate Trees"))
            {
                TreeRandomizer.RandomizeTree(treeTemplate, treeCount, cloneMaterials);
            }
        }

        private void HandleDragDrop()
        {
            var currentEvent = Event.current;
            var currentEventType = currentEvent.type;

            if(currentEventType == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                DragAndDrop.AcceptDrag();
            }
            if(currentEventType == EventType.DragPerform)
            {
                foreach(var obj in DragAndDrop.objectReferences)
                {
                    if (obj is Tree)
                    {
                        treeTemplate = obj as Tree;
                    }
                    else if(obj is GameObject)
                    {
                        treeTemplate = (obj as GameObject).GetComponent<Tree>();
                    }
                }
            }
        }
    }
}