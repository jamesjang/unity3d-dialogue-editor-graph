using System;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DA;
using UnityEditor;

namespace DA
{
    [System.Serializable]
    public class DialogNode : Node
    {
        //our conversation text
        public string conversationText = "Input conversation";

        public bool isRoot;

        public string nodeID;

        [XmlIgnore]
        public Action<DialogNode> OnRemoveNode;

        [SerializeField]
        public DialogNode inPointNode;
        [SerializeField]
        public DialogNode outPointNode;

        public DialogNode()
        {

        }

        public DialogNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoints> OnClickInPoint,
                            Action<ConnectionPoints> OnClickOutPoint, string inPointID, string outPointID, 
                            bool isRoot, Action<DialogNode> OnClickRemoveNode, string id, string cs)
        {
            rect = new Rect(position.x, position.y, width, height);
            style = nodeStyle;

            nodeID = id;


            this.isRoot = isRoot;

            conversationText = cs;

            OnRemoveNode = OnClickRemoveNode;
            inPoint = new ConnectionPoints(this, ConnectionPointType.In, inPointStyle, OnClickInPoint, inPointID);
            outPoint = new ConnectionPoints(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint, outPointID);

            defaultNodeStyle = nodeStyle;
            selectedNodeStyle = selectedStyle;
        }

        public DialogNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, 
                            GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoints> OnClickInPoint,
                            Action<ConnectionPoints> OnClickOutPoint, bool isRoot, Action<DialogNode> OnClickRemoveNode, string id)
                            : base(position, width, height, nodeStyle, selectedStyle, inPointStyle, outPointStyle)
        {
            inPoint = new ConnectionPoints(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);


            nodeID = id;

            this.isRoot = isRoot;
            OnRemoveNode = OnClickRemoveNode;
            outPoint = new ConnectionPoints(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        }

        public override void Draw()
        {
            if (this != null)
            {
                base.Draw();

                if (!isRoot)
                    inPoint.Draw();

                outPoint.Draw();

                Rect textRect = new Rect(rect.position.x + 25, rect.position.y + rect.size.y / 2 - 35, rect.size.x - 50, 25);

                conversationText = GUI.TextField(textRect, conversationText, 500);

                GUI.Label(rect, "Node Id is: " + nodeID);
            }
        }


        public override bool ProcessEvents(Event e)
        {
            base.ProcessEvents(e);
            switch (e.type)
            {
                case EventType.MouseDown:

                    if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();

                    }

                    if (e.button == 0)
                    {
                        if (rect.Contains(e.mousePosition))
                        {

                            if (inPointNode != null)
                            {
                                Debug.Log("Connecting from nodeID: " + inPointNode.nodeID);
                            }

                            if (outPointNode != null)
                            {
                                Debug.Log("Connecting to nodeID: " + outPointNode.nodeID);
                            }
                        }
                    }

                break;

            }

            return false;

        }
        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }

        private void OnClickRemoveNode()
        {
            if (OnRemoveNode != null)
            {
                OnRemoveNode(this);
            }
        }

    }

}



