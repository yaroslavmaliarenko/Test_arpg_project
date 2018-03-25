using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class TextDialogButton : MonoBehaviour,IPointerClickHandler {

    public int optionNum = 0;
    public NPC_Dialogue npcDialogue;



    // Use this for initialization
    void Start () {       

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Text dialog option click");        
        npcDialogue.UseDialogueOption(optionNum);
    }
}
