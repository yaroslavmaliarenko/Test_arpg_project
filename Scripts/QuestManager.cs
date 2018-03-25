using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

    public Quest[] quests;
    public List<Quest> questList = new List<Quest>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool QuestIsActive(string _questID)
    {
        bool isActive = false;
        for(int i=0; i <quests.Length ; i++)
        {
            if(quests[i].questID == _questID)
            {
                isActive = quests[i].isActive;

            }
        }

        return isActive;
    }

    public bool QuestIsComleted(string _questID)
    {
        bool isCompleted = false;
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i].questID == _questID)
            {
                isCompleted = quests[i].completed;

            }
        }

        return isCompleted;
    }

    public void ActivateQuest(string _questID)
    {
        
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i].questID == _questID)
            {
                quests[i].isActive = true;

            }
        }        
    }

    public void CompleteQuest(string _questID)
    {

        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i].questID == _questID)
            {
                quests[i].isActive = false;
                quests[i].completed = true;

            }
        }
    }


}

[System.Serializable]
public class Quest
{
    
    public string questID;
    public string questName;//Название квеста
    public string questDescription;//Описание квеста
    public bool isActive;
    public bool completed;

}
