using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public enum GameItemType {POTION,ARMOR,WEAPON,QUEST_ITEM};

public abstract class GameItem : MonoBehaviour,IInteractiveObject {
       

    public Sprite _itemSprite;
    public string _itemFullName;
    public string _itemID;
    public bool _oneOffItem;//Признак что этот предмет одноразового использования
    public int  _count;
    protected GameItemType _itemType;
    public string itemDescription;//Описание характеристик игрового предмета  
    public bool isEquipped;


    public Vector3 weaponRHandPos;
    public Vector3 weaponRHandRot;
    public Vector3 weaponBackPos;
    public Vector3 weaponBackRot;

    Camera myCam;
    //public Transform gameItemNameT;
    GameObject GameItemNameText;//Текстовый объект для отображения имени 


    private void Awake()
    {
        
        isEquipped = false;
    }

    // Use this for initialization
    void Start () {

        myCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
    

    public abstract void UseItem();
    public GameItemType GetItemType()
    {
        return _itemType;
    }

    public void HideItemName()
    {
        GameItemNameText.SetActive(false);
    }

    private void OnGUI()
    {        

        if (GameItemNameText != null)
        {           
            Vector3 screenPos = myCam.WorldToScreenPoint(transform.position);
            GameItemNameText.GetComponent<RectTransform>().SetPositionAndRotation(screenPos, new Quaternion(0, 0, 0, 0));
        }
        
    }

    public void ShowObjectName()
    {
        if (isEquipped) return;//Устраняем баг, когда во время боя можно забрать оружие противника ))

        if(GameItemNameText == null)
        {
            GameItemNameText = Instantiate(Managers.inventory.GameItemNamePrefab);
            GameItemNameText.transform.SetParent(Managers.inventory.MainCanvas.transform);
            GameItemNameText.GetComponent<Text>().text = _itemFullName;
            
        }
        if (GameItemNameText != null)
        {
            if(!GameItemNameText.activeSelf) GameItemNameText.SetActive(true);
        }

    }

    public void HideObjectName()
    {
        if (GameItemNameText != null)
        {
            GameItemNameText.SetActive(false);            
        }
    }
}
