using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour {

    static public InventoryManager inventory;
    static public ItemCollectionInterface itemCollectionInterface;
    static public PlayerManager player;
    static public UI_DilogManager textDialogMgr;
    static public QuestManager questMgr;

    // Use this for initialization
    private void Awake()
    {
        inventory = GetComponent<InventoryManager>();
        itemCollectionInterface = GetComponent<ItemCollectionInterface>();
        player = GetComponent<PlayerManager>();
        textDialogMgr = GetComponent<UI_DilogManager>();
        questMgr = GetComponent<QuestManager>();
        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
