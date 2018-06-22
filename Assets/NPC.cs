using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DialogueComponent))]
public class NPC : MonoBehaviour
{
    private DialogueComponent _dc;

    public Text npcText;

    public Button nextSetBtn;

    void Awake()
    {
        _dc = GetComponent<DialogueComponent>();

        nextSetBtn.onClick.AddListener(_dc.GoToNextDialogue);
    }

    void Start()
    {
        if (npcText)
        {
            npcText.text = _dc.currentText();
        }
    }

    void Update()
    {
        if (npcText)
        {
            npcText.text = _dc.currentText();
        }
    }

}
