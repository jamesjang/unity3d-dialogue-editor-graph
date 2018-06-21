using System.Collections;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace DA
{
    [System.Serializable]
    public struct ConversationStruct
    {
        public int Key;
        public string Value;

        public ConversationStruct(int key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    [System.Serializable]
    public struct ConnectionStruct
    {
        public ConnectionPoints inPoint;

        public ConnectionPoints outPoint;

        public Action<Connections> OnClickRemoveConnection;


        public ConnectionStruct(ConnectionPoints inPoint, ConnectionPoints outPoint, Action<Connections> OnClickRemoveConnection)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }
       // connections.Add(new Connections(inp, outp, OnClickRemoveConnection));
    }



    public class DialogueObject : ScriptableObject
    {
        [XmlIgnore]
        public List<ConversationStruct> ConversationSet = new List<ConversationStruct>();

        public int dialogueid;
        [XmlIgnore]
        public List<DialogNode> noders = new List<DialogNode>();

        public List<ConnectionStruct> connectionList = new List<ConnectionStruct>();

      //  public List<Connections> connectionlist = new List<Connections>();

        public DialogueObject() { }
        public int GetID()
        {
            return dialogueid;
        }

        public List<ConversationStruct> GetConversationSet()
        {
            return ConversationSet;
        }

        public ConversationStruct GetIndex(string nodeID)
        {
            for (int i = 0; i < GetConversationSet().Count; i++)
            {
                if (ConversationSet[i].Key == int.Parse(nodeID))
                {
                    return ConversationSet[i];
                }
            }

            return new ConversationStruct();
        }

    }
}
