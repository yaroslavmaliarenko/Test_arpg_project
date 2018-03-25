using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Creature : NPC_Manager {

    
    // Use this for initialization
    void Start () {

        base.Start();
        
        if (currentWeapon != null)
        {
            currentWeapon.isEquipped = true;            
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

    public override void GetDamage(int damage, Transform damageObject)
    {
        AI_BlackSpider ai = gameObject.GetComponent<AI_BlackSpider>();
        if (ai != null) ai.AddNewTarget(damageObject);

        AI_Tarantula ai_2 = gameObject.GetComponent<AI_Tarantula>();
        if (ai_2 != null) ai_2.AddNewTarget(damageObject);
        //battleMode = true;
        currentHP -= damage;


        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;
            thisAnimator.SetFloat("speed", 0.0f);
            thisAnimator.SetBool("IsDead", true);
            thisAgent.isStopped = true;
            
            if (ai != null) ai.StopAllCoroutines();
            if (ai_2 != null) ai_2.StopAllCoroutines();

        }
    }
}
