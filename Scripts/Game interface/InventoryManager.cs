using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    List<GameItem> listGameItem;
    Dictionary<string, int> oneOffItemsCount;//Содержит количество одноразовых предметов инвентаря которые можно сгруппировать

    public GameObject inventoryPanelWindow;
    public GameObject inventoryPanel;
    public GameObject ToolTipWindow;
    public GameObject ToolTipItemNameElement;//Текстовый элемет интерфейса(имя игрового объекта)
    public GameObject ToolTipItemDescElement;//Текстовый элемет интерфейса(описание игрового объекта)
    public GameObject QuickBar;//Быстрая панель
    public GameObject GameItemNamePrefab;//
    public GameObject MainCanvas;//

    Animator invAnimator; 



    // Use this for initialization
    void Start () {
        invAnimator = inventoryPanelWindow.GetComponent<Animator>();
        inventoryPanelWindow.SetActive(false);
        ToolTipWindow.SetActive(false);
        listGameItem = new List<GameItem>();
        oneOffItemsCount = new Dictionary<string, int>();

        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        {   
            InventButton _invButton = inventoryPanel.transform.GetChild(i).GetChild(0).GetComponent<InventButton>();
            if (_invButton != null)
            {
                _invButton.index = i;
                //GameItem _item = null;
                //listGameItem.Add(_item);
            }
        }

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyUp(KeyCode.I))//Открытие инвентаря
        {
            if (inventoryPanelWindow.activeSelf)
            {
                //Закрытие               
                invAnimator.SetBool("IsOpen", false);
                ToolTipWindow.SetActive(false);
                Cursor.visible = false;
                StartCoroutine(CloseInventory());                
            }
            else
            {
                //Открытие  
                UpdateInventory();
                Cursor.visible = true;
                inventoryPanelWindow.SetActive(true);
                invAnimator.SetBool("IsOpen", true);
            }
            
        }        

    }

   public void UpdateInventory()
    {
        
        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        {
           
            InventButton _invButton = inventoryPanel.transform.GetChild(i).GetChild(0).GetComponent<InventButton>();      
            

            if (_invButton != null)
            {
                //if(listGameItem[i] == null)
                //{
                //    //Сброс значений всех пустых ячеек
                //    if (_invButton != null) _invButton._gameItemRef = null;
                //    inventoryPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = _invButton.emptyButtonSprite;
                //    inventoryPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = _invButton.emptyButtonColor;
                //    _invButton.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "";
                //    continue;
                //}

                if (i > listGameItem.Count - 1)
                {
                    if (_invButton != null) _invButton._gameItemRef = null;
                    inventoryPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = _invButton.emptyButtonSprite;
                    inventoryPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = _invButton.emptyButtonColor;
                    _invButton.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "";
                    continue;
                }
                else
                {
                    //Установить  картинку объекта
                    inventoryPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = listGameItem[i]._itemSprite;
                    _invButton._gameItemRef = listGameItem[i];//Установка связи между ячейкой и предметом
                    _invButton.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "";
                    if (_invButton._gameItemRef._oneOffItem)//Показать количество для одноразовых предметов
                    {
                        if (oneOffItemsCount.ContainsKey(_invButton._gameItemRef._itemID))
                        {
                            _invButton._gameItemRef._count = oneOffItemsCount[_invButton._gameItemRef._itemID];//Увеличим количество
                            if (_invButton._gameItemRef._count > 0) _invButton.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _invButton._gameItemRef._count.ToString();

                        }
                    }
                }              

            }

            //Делаем максимальную прозрачность ячейки
            Color buffColor = inventoryPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().color;
            Color newColor = new Color(buffColor.r, buffColor.g, buffColor.b, 255);
            inventoryPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = newColor;
        }      


    }
    
    IEnumerator CloseInventory()
    {
        yield return new WaitForSeconds(1.0f);
        inventoryPanelWindow.SetActive(false);
    }
    
    public void AddItem(GameItem _object)
    {
        if (_object._oneOffItem)
        {
            if (oneOffItemsCount.ContainsKey(_object._itemID))
            {
                int count = _object._count;

                if (oneOffItemsCount[_object._itemID] <= 0)
                {
                    listGameItem.Add(_object);
                    _object.gameObject.SetActive(false);
                }
                else
                {
                    DestroyObject(_object.gameObject);
                }

                oneOffItemsCount[_object._itemID] += count;//Увеличим количество
                
            }
            else
            {
                //Первое добавление одноразового предмета
                oneOffItemsCount.Add(_object._itemID, _object._count);
                listGameItem.Add(_object);
                _object.gameObject.SetActive(false);

            }

        }
        else
        {
            
            if(_object.GetItemType() == GameItemType.WEAPON)
            {
                Collider weaponCollider =_object.gameObject.GetComponent<Collider>();
                if (weaponCollider != null) weaponCollider.enabled = false;                
            }

            listGameItem.Add(_object);
            _object.gameObject.SetActive(false);
        }

        UpdateInventory();
        
    }

    public int GetItemCount()
    {
        return listGameItem.Count;
    }

    public bool ReplaceGameItem(GameItem _item1,GameItem _item2)
    {
        int firstIndex = listGameItem.IndexOf(_item1);
        int secondIndex = listGameItem.IndexOf(_item2);

        if(firstIndex !=-1 && secondIndex != -1)
        {
            GameItem buffItem = _item1;
            listGameItem[firstIndex] = _item2;
            listGameItem[secondIndex] = buffItem;            
            return true;
        }

        return false;
    }

    public void PressCellButton(GameObject _object)
    {
        //Обработка нажатия на ячейке в инвентаре
        //Здесь необходимо обрабатывать нажатие в зависимости от типа объекта(зелье,оружие и тд)
        InventButton _invButton = _object.transform.GetChild(0).GetComponent<InventButton>();
        if (_invButton._gameItemRef == null) return;

        UseItemFromInventory(_invButton._gameItemRef);      
         
    }

    public void UseItemFromInventory(GameItem _item)
    {
        if (_item.GetItemType() == GameItemType.POTION)
        {

            _item.UseItem();//Использовать предмет из инвентаря
            //oneOffItemsCount[_item._itemID]--;
            //_item._count = oneOffItemsCount[_item._itemID];

            //if (_item._count == 0)
            //{
            //    if (listGameItem.Remove(_item))
            //    {
            //        DestroyObject(_item.gameObject);
            //        _item = null;

            //    }

            //    else Debug.Log("Cannot remove object");

            //}        


        }
        else if (_item.GetItemType() == GameItemType.WEAPON)
        {
            _item.UseItem();

        }

        UpdateInventory();

    }

    public void RemoveItemFromInventory(GameItem _item,bool destroyObject = true)
    {
        if (_item._oneOffItem)
        {
            oneOffItemsCount[_item._itemID]--;
            _item._count = oneOffItemsCount[_item._itemID];

            if (_item._count == 0)
            {
                if (listGameItem.Remove(_item))
                {
                    if(destroyObject) DestroyObject(_item.gameObject);
                    _item = null;

                }

                else Debug.Log("Cannot remove object");

            }
        }
        else
        {
            if (listGameItem.Remove(_item))
            {
                if (destroyObject) DestroyObject(_item.gameObject);
                _item = null;

            }

            else Debug.Log("Cannot remove object");

        }

        UpdateInventory();
        

    }


    public bool HaveAnItem(GameItem _item)
    {
        if (listGameItem.Contains(_item)) return true;
        else return false;
        
    }
}
