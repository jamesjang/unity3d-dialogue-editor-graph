using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DA;

public class DialogueComponent : MonoBehaviour
{

    public DialogueObject a;

    public List<ConversationStruct> ConvoSetRef = new List<ConversationStruct>();

    public int dialogueNum = 1;
    void Awake()
    {
        Init();
    }


    public void Init()
    {
        ConvoSetRef = a.GetConversationSet();
    }

    public string[] GetConversationSet(int index)
    {
        List<string> convoSet = new List<string>();

        foreach (ConversationStruct a in ConvoSetRef)
        {
            if (a.Key == index)
            {
                convoSet.Add(a.Value);
            }
        }
        return convoSet.ToArray();
    }

    public string GetNextConversationSet()
    {
        if (dialogueNum + 1 < ConvoSetRef.Count)
        {
            dialogueNum++;

            foreach (ConversationStruct a in ConvoSetRef)
            {
                if (a.Key == dialogueNum)
                {
                    return a.Value;
                }
            }
        }

        return null;
    }

    public string currentText()
    {
        return ConvoSetRef[dialogueNum].Value;
    }

    public void GoToNextDialogue()
    {
        if (dialogueNum +1 < ConvoSetRef.Count)
            dialogueNum++;
    }

}


