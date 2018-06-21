using System.Xml.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DA
{
    public class Node
    {
        public Rect rect;
        [XmlIgnore]
        public string title;

        [XmlIgnore]
        public bool isDragged;
        [XmlIgnore]
        public bool isSelected;

        [XmlIgnore]
        public GUIStyle style;

        //node that connects to in
        public ConnectionPoints inPoint;

        //node that connects to out
        public ConnectionPoints outPoint;

        [XmlIgnore]
        public GUIStyle defaultNodeStyle;
        [XmlIgnore]
        public GUIStyle selectedNodeStyle;


        public Node()
        {

        }

        public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle)
        {
            Debug.Log("constructor base");

            rect = new Rect(position.x, position.y, width, height);
            style = nodeStyle;
            defaultNodeStyle = nodeStyle;
            selectedNodeStyle = selectedStyle;

        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        public virtual void Draw()
        {
            GUI.Box(rect, title, style);


        }
        public virtual bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            GUI.changed = true;
                            isSelected = true;
                            style = selectedNodeStyle;

              
                        }
                        else
                        {
                            isSelected = false;
                            style = defaultNodeStyle;
                            GUI.changed = true;
                        }
                    }

                    break;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }


    }

}