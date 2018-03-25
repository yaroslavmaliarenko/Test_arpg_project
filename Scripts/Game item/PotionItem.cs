using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : GameItem
{

    //============= Описание характеристик объекта ===========================
    //POTION 
    public int hp_Recovery;
    public float hp_time_recovery;
    public int mp_Recovery;
    public float mp_time_recovery;

    private void Awake()
    {
        _itemType = GameItemType.POTION;
    }

    public override void UseItem()
    {
        Managers.player.HealthRecovery(this, hp_Recovery, hp_time_recovery);//Восстановить здоровье
    }

    
}
