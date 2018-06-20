using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DA;

namespace DA
{
    public enum ConnectionPointType { In, Out }

    public class ConnectionPoints
    {

        public string id;
        [XmlIgnore]
        public Rect rect;

        [XmlIgnore]
        public ConnectionPointType type;

        [XmlIgnore]
        public DialogNode node;


        [XmlIgnore]
        public GUIStyle style;

        [XmlIgnore]
        public Action<ConnectionPoints> OnClickConnectionPoint;

        public ConnectionPoints() { }

        public ConnectionPoints(DialogNode node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoints> OnClickConnectionPoint, string id = null)
        {
            this.node = node;
            this.type = type;
            this.style = style;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, 10f, 20f);

            this.id = id ?? Guid.NewGuid().ToString();
        }


        public void Draw()
        {
            rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;

            switch (type)
            {
                case ConnectionPointType.In:
                    rect.x = node.rect.x - rect.width + 8f;
                    break;

                case ConnectionPointType.Out:
                    rect.x = node.rect.x + node.rect.width - 8f;
                    break;
            }

            if (GUI.Button(rect, "", style))
            {
                if (OnClickConnectionPoint != null)
                {
                    OnClickConnectionPoint(this);
                }
            }
        }
    }

}