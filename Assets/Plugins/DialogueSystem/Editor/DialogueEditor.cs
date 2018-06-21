using System;
using System.Xml;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using DA;

public class DialogueEditor : EditorWindow
{
    private List<DialogNode> nodes;
    
    private GUIStyle nodeStyle;
    private List<Connections> connections;

    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private ConnectionPoints selectedInPoint;
    private ConnectionPoints selectedOutPoint;
    private GUIStyle selectedNodeStyle;
    private Vector2 offset;
    private Vector2 drag;

    private float menuBarHeight = 20f;
    private Rect menuBar;
    public DialogueObject dialogueObject;

    public string saveFileName;

    [MenuItem("Window/Dialogue Editor")]
    private static void OPenWindow()
    {
        DialogueEditor window = GetWindow<DialogueEditor>();
        window.titleContent = new GUIContent("Dialogue Editor");
        GUI.FocusControl(null);
    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawMenuBar();
        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed)
        {
            Repaint();
  
        }

    }

    private void Save()
    {
        XMLOp.Serialize(nodes, "Assets/Resources/nodes.xml");
        XMLOp.Serialize(connections, "Assets/Resources/connections.xml");


        dialogueObject = CreateDialogueObject.Create(saveFileName);

        if (dialogueObject)
        {
            dialogueObject.dialogueid = 5;

            dialogueObject.ConversationSet = (GenerateDictionaryFromNode());

            dialogueObject.noders = nodes;

        }
    }

    private void Load()
    {
        var nodesDeserialized = XMLOp.Deserialize<List<DialogNode>>("Assets/Resources/nodes.xml");
        var connectionsDeserialized = XMLOp.Deserialize<List<Connections>>("Assets/Resources/connections.xml");

        nodes = new List<DialogNode>();
        connections = new List<Connections>();
        foreach (var nodeDeserialized in nodesDeserialized)
        {
            nodes.Add(new DialogNode(
                nodeDeserialized.rect.position,
                nodeDeserialized.rect.width,
                nodeDeserialized.rect.height,
                nodeStyle,
                selectedNodeStyle,
                inPointStyle,
                outPointStyle,
                OnClickInPoint,
                OnClickOutPoint,
                nodeDeserialized.inPoint.id,
                nodeDeserialized.outPoint.id,
                nodeDeserialized.isRoot,
                nodeDeserialized.OnRemoveNode,
                nodeDeserialized.nodeID
                )
            );
        }

        foreach (var connectionDeserialized in connectionsDeserialized)
        {
            var inPoint = nodes.First(n => n.inPoint.id == connectionDeserialized.inPoint.id).inPoint;
            var outPoint = nodes.First(n => n.outPoint.id == connectionDeserialized.outPoint.id).outPoint;
            connections.Add(new Connections(inPoint, outPoint, OnClickRemoveConnection));
        }
    }

    private void LoadTest(string fileName)
    {
        string[] results = AssetDatabase.FindAssets(fileName);

        if (results.Length != 0)
        {
            DialogueObject d = (DialogueObject)AssetDatabase.LoadAssetAtPath("Assets/Dialogues/" + fileName + ".asset", typeof(DialogueObject));
            if (d)
            {
                nodes = new List<DialogNode>();
              

                for (int i = 0; i < d.noders.Count; i++)
                {
                    string inID = d.noders[i].isRoot? "0" : d.noders[i].inPoint.id;
    

                    nodes.Add(new DialogNode(
                         d.noders[i].rect.position,
                         d.noders[i].rect.width,
                         d.noders[i].rect.height,
                         nodeStyle,
                         selectedNodeStyle,
                         inPointStyle,
                         outPointStyle,
                         OnClickInPoint,
                         OnClickOutPoint,
                         inID,
                         d.noders[i].outPoint.id,
                         d.noders[i].isRoot,
                         d.noders[i].OnRemoveNode,
                         d.noders[i].nodeID
                       ));
                }

                foreach(ConversationStruct a in d.ConversationSet)
                {
                    Debug.Log(a.Key);
                }

                GUI.changed = true;
            }
        }
    }

    public List<ConversationStruct> GenerateDictionaryFromNode()
    {
        List<ConversationStruct> tempDict = new List<ConversationStruct>();

        foreach (DialogNode n in nodes)
        {
            var element = new ConversationStruct(1, n.conversationText);
            tempDict.Add(element);

            Debug.Log(element);
        }


        return tempDict;
    }

    private void DrawMenuBar()
    {
        menuBar = new Rect(0, 0, position.width, menuBarHeight);

        GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(35)))
        {
            Save();
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Load"), EditorStyles.toolbarButton, GUILayout.Width(35)))
        {
            //  Load();
            Debug.Log("loading file" + saveFileName);
            LoadTest(saveFileName);
        }

        GUILayout.Space(5);
        GUILayout.Label("Save File Name: ", GUILayout.Width(100) );
        saveFileName = GUILayout.TextField( saveFileName, GUILayout.Width(200));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }


        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] != null)
                    nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                    GUI.FocusControl(null);
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }
    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }
 

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (nodes == null)
        {
            nodes = new List<DialogNode>();
        }

        bool isRoot = nodes.Count < 1;
        nodes.Add(new DialogNode(mousePosition, 200, 100, nodeStyle, selectedNodeStyle, 
                                inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, 
                                isRoot, OnClickRemoveNode, (nodes.Count+1).ToString()));
    }

    private void OnClickInPoint(ConnectionPoints inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {

            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void OnClickOutPoint(ConnectionPoints outPoint)
    {
        selectedOutPoint = outPoint;

        Debug.Log("On click outpoint");

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveConnection(Connections connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connections>();
        }
        if (selectedOutPoint.node != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].outPoint == selectedOutPoint)
                {
                    connections[i].inPoint.node.inPointNode = null;
                    OnClickRemoveConnection(connections[i]);
                }
            }
        }

        if (selectedInPoint.node != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == selectedInPoint)
                {
                    connections[i].outPoint.node.outPointNode = null;
                    OnClickRemoveConnection(connections[i]);
                }
            }
        }

        connections.Add(new Connections(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void OnClickRemoveNode(DialogNode node)
    {
        if (connections != null)
        {
            Debug.Log("Clicking on click remove node");

            List<Connections> connectionsToRemove = new List<Connections>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);

                    connections[i].inPoint.node.inPointNode = null;
                    connections[i].outPoint.node.outPointNode = null;
                }


            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);

        nodes = RecalculateNodePos();
    }


    //we root, 2, 3, 4, 5 , 6, 7
    private List<DialogNode> RecalculateNodePos()
    {
        List<DialogNode> noders = new List< DialogNode > ();
        for (int i = 0; i < nodes.Count; i++)
        {
            DialogNode d = nodes[i];
            if (i <= 0)
                d.nodeID = "root";
            else
                d.nodeID = (i+1).ToString();

            noders.Add(d);
        }

        return noders;
    }

    private void DebugConnection(DialogNode n)
    {
      //  Debug.Log("The next conversation item is: " + n.outPoint.id);
    }



}

public class CreateDialogueObject
{
    public static DialogueObject Create(string filename)
    {
        DialogueObject asset = ScriptableObject.CreateInstance<DialogueObject>();

        AssetDatabase.CreateAsset(asset, "Assets/Dialogues/"+filename+".asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}

