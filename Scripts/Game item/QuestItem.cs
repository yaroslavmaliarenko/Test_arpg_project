using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItem : GameItem {

    private void Awake()
    {
        _itemType = GameItemType.QUEST_ITEM;
    }

    public override void UseItem()
    {
        Debug.Log("Use quest item");
    }

}
