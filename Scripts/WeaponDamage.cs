using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour {
    
    List<NPC_Manager> damagaList;
    List<PlayerManager> damagaListPM;   
    

    public bool thisPlayerWeapon;//Признак того что этот меч надет игроком    
    public NPC_Manager npc;//Ссылка на NPC который держит этот меч

    private void Awake()
    {
        damagaList = new List<NPC_Manager>();
        damagaListPM = new List<PlayerManager>();
    }

    // Use this for initialization
    void Start () {        
               
    }

    
    

    // Update is called once per frame	

    private void OnTriggerEnter(Collider other)
    {
        OnWeaponCollision(other);

    }

    private void OnTriggerStay(Collider other)
    {
        OnWeaponCollision(other);        

    }

    private void OnTriggerExit(Collider other)
    {
        OnWeaponCollision(other);
    }

    public void ClearDamageList()
    {
        damagaList.Clear();
        damagaListPM.Clear();
    }

    private void OnWeaponCollision(Collider other)
    {
        if (thisPlayerWeapon)
        {
            NPC_Manager npcDamage = other.GetComponent<NPC_Manager>();
            if (npcDamage != null)
            {
                Vector3 dist = npcDamage.transform.position - Managers.player.playerT.position;                
                if (!damagaList.Contains(npcDamage) && (Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(Managers.player.playerT.forward, dist.normalized))) < 55)
                {
                    if (npcDamage.IsDead()) return;
                    npcDamage.GetDamage(Managers.player.currentWeapon.physicalDamage, Managers.player.playerT);
                    damagaList.Add(npcDamage);
                    AudioSource aSrc = npcDamage.gameObject.GetComponent<AudioSource>();
                    if (aSrc != null)
                    {
                        aSrc.clip = npcDamage.damageClip;
                        aSrc.Play();
                    }
                    
                }
            }
        }        
        else if(npc!= null)
        {
            if (npc.IsDead()) return;
            //Обработка нанесения урона нашему игроку
            if (other.gameObject.tag == "Player")
            {
                Vector3 dist = other.transform.position - npc.transform.position;

                if (!damagaListPM.Contains(Managers.player) && (Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(npc.transform.forward, dist.normalized))) < 55)
                {
                    //if (npc.friendList.Contains(other.tag)) return;
                    Managers.player.GetDamage(npc.currentWeapon.physicalDamage);
                    damagaListPM.Add(Managers.player);
                    AudioSource aSrc = Managers.player.playerT.gameObject.GetComponent<AudioSource>();
                    if (aSrc != null)
                    {
                        aSrc.clip = Managers.player.damageClip;
                        aSrc.Play();
                    }

                }

            }

            //Обработка нанесения урона NPC ===> NPC
            NPC_Manager npcDamage = other.GetComponent<NPC_Manager>();
            if (npcDamage != null)
            {
                Vector3 dist = npcDamage.gameObject.transform.position - npc.gameObject.transform.position; ;
                if (!damagaList.Contains(npcDamage) && (Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(npc.gameObject.transform.forward, dist.normalized))) < 55)
                {
                    if (npc.friendList.Contains(npcDamage.tag) || npc.IsDead()) return;
                    npcDamage.GetDamage(npc.currentWeapon.physicalDamage, npc.gameObject.transform);
                    damagaList.Add(npcDamage);
                    AudioSource aSrc = npcDamage.gameObject.GetComponent<AudioSource>();
                    if (aSrc != null)
                    {
                        aSrc.clip = npcDamage.damageClip;
                        aSrc.Play();
                    }
                }
            }




        }

    }




}
