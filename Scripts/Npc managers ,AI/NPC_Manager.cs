using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AI;
using System;

public abstract class NPC_Manager : MonoBehaviour, IInteractiveObject
{

    public string npcName;
    public float textPosition;
    public float healthBarPosition;

    protected Animator thisAnimator;
    protected NavMeshAgent thisAgent;

    //Отображение ХП при боевом режиме
    public GameObject hpBarPrefab;
    public GameObject npcName_UI_Prefab;
    public GameObject parentCanvas;
    protected GameObject HpBarImage;
    protected GameObject npcNameUI;
    protected Camera myCam;
    public bool bDrwaHpBar;

    protected float currentHP;
    [SerializeField] protected float maxHP = 100;
    protected bool isDead;

    public bool battleMode;//Признак боевого режима

    [SerializeField] protected Transform rHandT;
    [SerializeField] protected Transform backT;
    public WeaponItem currentWeapon;//Текущее оружие

    [SerializeField] public AudioClip damageClip;
    [SerializeField] public AudioClip stepClip;

    public List<string> friendList = new List<string>();
    public List<GameItem> gameItemList = new List<GameItem>();

    // Use this for initialization
   protected void Start() {

        currentHP = maxHP;
        isDead = false;
        battleMode = false;

        thisAnimator = GetComponent<Animator>();
        thisAgent = GetComponent<NavMeshAgent>();

        //Создание элемента интерфейса, который отвечает за отображение очков жизни НПС
        HpBarImage = Instantiate(hpBarPrefab);
        HpBarImage.transform.SetParent(parentCanvas.transform);
        HpBarImage.transform.GetChild(0).GetComponent<Text>().text = npcName;

        bDrwaHpBar = false;
        myCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();  
        
        //Спрятать все игровые предметы инветаря данного НПС которые были ему присвоены
        for(int i=0;i<gameItemList.Count ;i++)
        {
            if (gameItemList[i] != null) gameItemList[i].gameObject.SetActive(false);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        if (bDrwaHpBar)
        {
            HpBarImage.SetActive(true);
            UpdateHpBar();
        }
        else HpBarImage.SetActive(false);

        if(npcNameUI != null)
        {
                       
            Vector3 screenPos = myCam.WorldToScreenPoint(gameObject.transform.position + (Vector3.up * textPosition));            
            npcNameUI.GetComponent<RectTransform>().SetPositionAndRotation(screenPos, new Quaternion(0, 0, 0, 0));
        }
        
    }

    public void ShowObjectName()
    {
        //Создание элемента интерфейса, который отвечает за отображение имени НПС
        if (npcNameUI == null)
        {
            npcNameUI = Instantiate(npcName_UI_Prefab);
            npcNameUI.transform.SetParent(parentCanvas.transform);
            npcNameUI.GetComponent<Text>().text = npcName;
        }

        if (npcNameUI != null)
        {
            if(!npcNameUI.activeSelf) npcNameUI.SetActive(true);
        }

        

    }

    public void HideObjectName()
    {
        if (npcNameUI != null)
        {
            npcNameUI.SetActive(false); 
            
        }
    }

    void UpdateHpBar()
    {
        
        Vector3 screenPos = myCam.WorldToScreenPoint(gameObject.transform.position + (Vector3.up * healthBarPosition));        
        HpBarImage.GetComponent<RectTransform>().SetPositionAndRotation(screenPos, new Quaternion(0, 0, 0, 0));
        HpBarImage.transform.GetChild(1).GetChild(0).localScale = new Vector3(currentHP / maxHP, 1, 1);

    }

    public bool IsDead()
    {
        return isDead;
    }

    public abstract void GetDamage(int damage, Transform damageObject);   

    

    public void ExploreNpc()
    {
        if (!battleMode && currentHP > 0)
        {
            NPC_Dialogue npcDialogue = gameObject.GetComponent<NPC_Dialogue>();
            if(npcDialogue != null)
            {
                //Проверить есть ли хоть 1 активный вариант диалога с этим NPC

                AI_NPC ai = gameObject.GetComponent<AI_NPC>();
                //Блокировка движения обеих персонажей
                if (ai != null) ai.StartState(AI_STATE.IDLE_STATE);
                Managers.player.moveBlock = true;
                Managers.player.myAnimator.SetFloat("speed", 0);
                gameObject.transform.LookAt(Managers.player.playerT);//Повернуть лицом к персонажу
                Managers.player.playerT.LookAt(gameObject.transform);

                npcDialogue.UpdateDialogButton();
                Managers.textDialogMgr.TextDialogWindow.SetActive(true);
                Managers.textDialogMgr.TextDialogArea.SetActive(true);
                Cursor.visible = true;
                Managers.inventory.QuickBar.SetActive(false);
                QuickBar Qb = Managers.inventory.QuickBar.GetComponent<QuickBar>();
                Qb.useBlock = true;

            }
        }
        else if (isDead)
        {
            if(gameItemList.Count > 0)
            {
                Managers.itemCollectionInterface.npc = this;
                Managers.itemCollectionInterface.UpdateItemCollectionInterface();
                Managers.itemCollectionInterface.itemCollectionlWindow.SetActive(true);
                Cursor.visible = true;
                Managers.player.moveBlock = true;
            }
            

        }
    }

    public void EndNpcDialogue()
    {
        AI_NPC ai = gameObject.GetComponent<AI_NPC>();
        ai.StartCurrentState();
        Managers.player.moveBlock = false;
        Cursor.visible = false;
        Managers.inventory.QuickBar.SetActive(true);
        QuickBar Qb = Managers.inventory.QuickBar.GetComponent<QuickBar>();
        Qb.useBlock = false;

    }

    
}
