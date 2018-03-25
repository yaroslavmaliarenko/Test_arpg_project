using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : GameItem
{
    //ARMOR
    public int physicalDefence;
    public int magicDefence;

    private void Awake()
    {
        _itemType = GameItemType.ARMOR;
    }

    public override void UseItem()
    {
        Debug.Log("Use Armor");
        
    }

}
