using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActivationCondition {QUEST_IS_ACTIVE, QUEST_IS_COMPLETED,HAVE_AN_ITEM, NPC_IS_DEAD};
public enum DialogAction { ACTIVATE_QUEST,COMPLETE_QUEST,RETURN_ITEM};

public class NPC_Dialogue : MonoBehaviour {
    public int currentNode = 0;
    [SerializeField] NodeDialogue[] nodes;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

       
		
	}

    public void UpdateDialogButton()
    {
        for(int i=0;i < Managers.textDialogMgr.TextDialogArea.transform.childCount; i++)
        {           
                Destroy(Managers.textDialogMgr.TextDialogArea.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < nodes[currentNode].options.Length; i++)
        {
            if (!nodes[currentNode].options[i].isActive || !CheckDialogOptionConditions(i)) continue;//Не показывать вариант если не удовлетворяют условия отображения

            GameObject newObj = Instantiate(Managers.textDialogMgr.TextDialogButtonPrefab);
            TextDialogButton tdButton = newObj.GetComponent<TextDialogButton>();
            tdButton.npcDialogue = this;
            tdButton.optionNum = i;
            newObj.transform.SetParent(Managers.textDialogMgr.TextDialogArea.transform);
            Text txt = newObj.GetComponent<Text>();
            txt.text = nodes[currentNode].options[i].optionMainText;

        }

    }

    //Проверить условия для появления варианта диалога
    bool CheckDialogOptionConditions(int optionNum)
    {
        bool isActive = true;
        for (int i = 0; i < nodes[currentNode].options[optionNum].conditions.Length; i++)
        {
            ActivationCondition currentCondition = nodes[currentNode].options[optionNum].conditions[i].condition;

            bool bValue;
            switch (currentCondition)
            {
                case ActivationCondition.QUEST_IS_ACTIVE:
                    bValue = Managers.questMgr.QuestIsActive(nodes[currentNode].options[optionNum].conditions[i].questID);
                    isActive = isActive & bValue;
                    break;

                case ActivationCondition.HAVE_AN_ITEM:
                    bValue = Managers.inventory.HaveAnItem(nodes[currentNode].options[optionNum].conditions[i].requiredItem);
                    isActive = isActive & bValue;

                    break;

                case ActivationCondition.NPC_IS_DEAD:
                    bValue = false;
                    NPC_Manager npc = nodes[currentNode].options[optionNum].conditions[i].npc;
                    if (npc != null)
                    {
                        bValue = npc.IsDead();
                    }
                    isActive = isActive & bValue;

                    break;

                case ActivationCondition.QUEST_IS_COMPLETED:
                    bValue = Managers.questMgr.QuestIsComleted(nodes[currentNode].options[optionNum].conditions[i].questID);
                    isActive = isActive & bValue;
                    break;

            }
        }

        return isActive;
    }

    public void UseDialogueOption(int optionNum)
    {
        Managers.textDialogMgr.TextDialogWindow.SetActive(false);
        Managers.textDialogMgr.FullTextDialog.SetActive(true);

        StartCoroutine(DisplayTextDialogue(optionNum));

        
    }



    //Отображение текста выбранного варианта диалога
    IEnumerator DisplayTextDialogue(int optionNum)
    {
        for (int i = 0; i < nodes[currentNode].options[optionNum].doText.Length; i++)
        {
            
            Text dialogueText = Managers.textDialogMgr.FullTextDialog.transform.GetChild(0).GetComponent<Text>();
            if (dialogueText != null) dialogueText.text = nodes[currentNode].options[optionNum].doText[i].Text;

            float delayLeft = nodes[currentNode].options[optionNum].doText[i].delayDisplayedText;            
            while (delayLeft > 0)
            {
                if (Input.GetMouseButtonUp(1)) delayLeft = 0;
                yield return null;
                delayLeft = delayLeft - Time.deltaTime;
            }

            
        }
        
        ExecuteDialogOptionAction(optionNum);

    }  


    //Выполнить все дейсвтия после нажатия на варианте диалога
    void ExecuteDialogOptionAction(int optionNum)
    {
        
        for (int i = 0; i < nodes[currentNode].options[optionNum].actions.Length; i++)
        {
            DialogAction currentAction = nodes[currentNode].options[optionNum].actions[i].action;
            if(currentAction == DialogAction.ACTIVATE_QUEST)
            {
                Managers.questMgr.ActivateQuest(nodes[currentNode].options[optionNum].actions[i].questID);                
            }
            else if (currentAction == DialogAction.COMPLETE_QUEST)
            {
                Managers.questMgr.CompleteQuest(nodes[currentNode].options[optionNum].actions[i].questID);
            }
            else if (currentAction == DialogAction.RETURN_ITEM)
            {
                //Передать предмет NPC
                NPC_Manager thisNpc = gameObject.GetComponent<NPC_Manager>();
                if(thisNpc!= null) thisNpc.gameItemList.Add(nodes[currentNode].options[optionNum].actions[i].item);
                Managers.inventory.RemoveItemFromInventory(nodes[currentNode].options[optionNum].actions[i].item,false);
            }

        }

        //===
        if (!nodes[currentNode].options[optionNum].endOption)
        {
            nodes[currentNode].options[optionNum].isActive = false;//Сделать неактивным вариант диалога
            currentNode = nodes[currentNode].options[optionNum].toNode;
            UpdateDialogButton();

            Managers.textDialogMgr.TextDialogWindow.SetActive(true);
            Managers.textDialogMgr.FullTextDialog.SetActive(false);
        }
        else//Завершение диалога
        {
            Managers.textDialogMgr.TextDialogWindow.SetActive(false);
            Managers.textDialogMgr.FullTextDialog.SetActive(false);

            NPC_Manager npc = gameObject.GetComponent<NPC_Manager>();
            if (npc != null) npc.EndNpcDialogue();

        }


    }
}

[System.Serializable]
public class NodeDialogue
{
    public int nodeNumber = 0;
    public DialogueOption[] options;
}

[System.Serializable]
public class DialogueOption
{
    public bool isActive = true;
    public int toNode = 0;
    public bool endOption = false;
    public string optionMainText;
    public DialogueOptionText[] doText;//Массив текста для данного варианта диалога
    public DO_ActivationCondition[] conditions;//Условия активации варианта диалога
    public DO_Action[] actions;
}

[System.Serializable]
public class DialogueOptionText
{
    public float delayDisplayedText;//Задержка отображения выведенного текста
    public string Text;
    public string nameTextOwner;//Имя владельца текста
}

[System.Serializable]
//Условие активации варианта диалога
public class DO_ActivationCondition
{
    public ActivationCondition condition;
    public string questID;
    public GameItem requiredItem;
    public NPC_Manager npc;
    }

[System.Serializable]
//Действие которое происходит после нажатия на варианте диалога
public class DO_Action
{
    public DialogAction action;
    public string questID;
    public GameItem item;

}