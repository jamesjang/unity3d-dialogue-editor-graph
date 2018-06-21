using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DA;

namespace DA
{

    [System.Serializable]
    public class Connections
    {
        public ConnectionPoints inPoint;

        public ConnectionPoints outPoint;

        [XmlIgnore]
        public Action<Connections> OnClickRemoveConnection;

        public Connections() { }
        public Connections(ConnectionPoints inPoint, ConnectionPoints outPoint, Action<Connections> OnClickRemoveConnection)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;

            inPoint.node.inPointNode = outPoint.node;
            outPoint.node.outPointNode = inPoint.node;

            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }

        public void Draw()
        {
            
            Handles.DrawBezier(
                inPoint.rect.center,
                outPoint.rect.center,
                inPoint.rect.center + Vector2.left * 50f,
                outPoint.rect.center - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap))
            {
                if (OnClickRemoveConnection != null)
                {


                    //breaks node reference 
                    this.inPoint.node.inPointNode = null;
                    this.outPoint.node.outPointNode = null;

                    OnClickRemoveConnection(this);
                }
            }
        }
    }
}
