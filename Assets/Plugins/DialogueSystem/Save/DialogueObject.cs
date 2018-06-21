using System.Collections;
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


    [CreateAssetMenu(fileName = "Data", menuName = "Dialogue/Conversation", order = 1)]
    public class DialogueObject : ScriptableObject
    {

        [XmlIgnore]
        public List<ConversationStruct> ConversationSet = new List<ConversationStruct>();

        public int dialogueid;

        public List<DialogNode> noders = new List<DialogNode>();

        public DialogueObject() { }
        public int GetID()
        {
            return dialogueid;
        }

        public List<ConversationStruct> GetConversationSet()
        {
            return ConversationSet;
        }

    }
}
