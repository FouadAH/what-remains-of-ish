//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor.IMGUI.Controls;
//using UnityEditor;

//class SimpleTreeViewWindow : EditorWindow
//{
//    [SerializeField] TreeViewState m_TreeViewState;

//    SimpleTreeView m_SceneDataView;
//    SimpleTreeView m_EntityDataView;
//    SimpleTreeView m_ItemDataView;

//    SaveManager m_SaveManager;

//    void OnEnable()
//    {
//        if (m_TreeViewState == null)
//            m_TreeViewState = new TreeViewState();

//        m_SaveManager = FindObjectOfType<SaveManager>();

//        if (m_SaveManager.currentSaveFile != null)
//        {
//            m_SceneDataView = new SimpleTreeView(m_TreeViewState, "Scene Data", m_SaveManager.currentSaveFile.gameData);
//        }
//    }

//    void OnGUI()
//    {
//        m_SceneDataView.OnGUI(new Rect(0, 0, position.width, position.height));
//        //m_EntityDataView.OnGUI(new Rect(0, m_SceneDataView.totalHeight, position.width, position.height));
//        //m_ItemDataView.OnGUI(new Rect(0, m_EntityDataView.totalHeight, position.width, position.height));
//    }

//    // Add menu named "My Window" to the Window menu
//    [MenuItem("Save System/Save Window")]
//    static void ShowWindow()
//    {
//        // Get existing open window or if none, make a new one:
//        var window = GetWindow<SimpleTreeViewWindow>();
//        window.titleContent = new GUIContent("Save Window");
//        window.Show();
//    }
//}

//class SimpleTreeView : TreeView
//{
//    GameData gameData;
//    string label;

//    public SimpleTreeView(TreeViewState treeViewState, string label, GameData gameData): base(treeViewState)
//    {
//        this.gameData = gameData;
//        this.label = label;
//        Reload();
//    }

//    protected override TreeViewItem BuildRoot()
//    {
//        var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

//        int i = 1;

//        var dataEntries = gameData.scene_data.data_entries;
//        var labelNode = new TreeViewItem { id = i, displayName = label };
//        root.AddChild(labelNode);
//        i++;

//        foreach (string key in dataEntries.Keys)
//        {
//            var keyNode = new TreeViewItem { id = i, displayName = key };
//            labelNode.AddChild(keyNode);
//            i++;

//            var valueNode = new TreeViewItem { id = i, displayName = dataEntries[key] };
//            keyNode.AddChild(valueNode);
//            i++;
//        }

//        dataEntries = gameData.enemy_data.data_entries;
//        var labelENode = new TreeViewItem { id = i, displayName = label };
//        root.AddChild(labelENode);
//        i++;

//        foreach (string key in dataEntries.Keys)
//        {
//            var keyNode = new TreeViewItem { id = i, displayName = key };
//            labelNode.AddChild(keyNode);
//            i++;

//            var valueNode = new TreeViewItem { id = i, displayName = dataEntries[key] };
//            keyNode.AddChild(valueNode);
//            i++;
//        }

//        dataEntries = gameData.item_data.data_entries;
//        var labelINode = new TreeViewItem { id = i, displayName = label };
//        root.AddChild(labelINode);
//        i++;

//        foreach (string key in dataEntries.Keys)
//        {
//            var keyNode = new TreeViewItem { id = i, displayName = key };
//            labelNode.AddChild(keyNode);
//            i++;

//            var valueNode = new TreeViewItem { id = i, displayName = dataEntries[key] };
//            keyNode.AddChild(valueNode);
//            i++;
//        }
//        SetupDepthsFromParentsAndChildren(root);

//        return root;
//    }
//}