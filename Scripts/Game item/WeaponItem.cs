using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : GameItem
{

    //WEAPON
    public int physicalDamage;
    public int magicDamage;

    private void Awake()
    {
        
        _itemType = GameItemType.WEAPON;
    }

    public override void UseItem()
    {
        Managers.player.UseWeapon(this);
    }

}
