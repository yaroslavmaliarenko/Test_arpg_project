using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Human_Manager : NPC_Manager {

	// Use this for initialization
	void Start () {

        base.Start();
        
        //Одеть оружие за спину
        if (currentWeapon != null)
        {
            currentWeapon.isEquipped = true;
            currentWeapon.transform.SetParent(backT);
            currentWeapon.gameObject.transform.localPosition = currentWeapon.weaponBackPos;
            currentWeapon.gameObject.transform.localRotation = Quaternion.Euler(currentWeapon.weaponBackRot);
            //TakeWeapon();
            WeaponDamage wd = currentWeapon.GetComponent<WeaponDamage>();
            if (wd != null)
            {
                wd.thisPlayerWeapon = false;
                wd.npc = this;
            }
            Collider weaponCollider = currentWeapon.GetComponent<Collider>();
            if (weaponCollider != null)
            {
                weaponCollider.enabled = false;

            }
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}



    public void TakeWeapon()
    {
        if (currentWeapon != null)
        {
            thisAnimator.SetTrigger("OnBattleMode");
            currentWeapon.transform.SetParent(rHandT);
            currentWeapon.gameObject.transform.localPosition = currentWeapon.weaponRHandPos;
            currentWeapon.gameObject.transform.localRotation = Quaternion.Euler(currentWeapon.weaponRHandRot);

        }


    }

    public void HideWeapon()
    {
        if (currentWeapon != null)
        {
            thisAnimator.SetTrigger("OffBattleMode");
            currentWeapon.transform.SetParent(backT);
            currentWeapon.gameObject.transform.localPosition = currentWeapon.weaponBackPos;
            currentWeapon.gameObject.transform.localRotation = Quaternion.Euler(currentWeapon.weaponBackRot);
        }


    }

    public override void GetDamage(int damage, Transform damageObject)
    {
        AI_NPC ai = gameObject.GetComponent<AI_NPC>();
        if (ai != null) ai.AddNewTarget(damageObject);
        //battleMode = true;
        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;
            thisAnimator.SetFloat("speed", 0.0f);
            thisAnimator.SetBool("IsDead", true);
            thisAgent.isStopped = true;

            AI_NPC thisNPC = gameObject.GetComponent<AI_NPC>();

            if (thisNPC != null) thisNPC.StopAllCoroutines();
            //Дать возможность забрать оружие
            if (currentWeapon != null)
            {
                currentWeapon.isEquipped = false;
                Collider weaponCollider = currentWeapon.GetComponent<Collider>();
                if (weaponCollider != null)
                {                    
                    weaponCollider.enabled = true;
                }
            }

        }

        
    }
}
