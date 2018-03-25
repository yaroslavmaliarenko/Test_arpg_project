using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollectionInterface : MonoBehaviour {

    public NPC_Manager npc;
    public GameObject itemCollectionlWindow;
    public GameObject itemCollectionPanel;
    public GameObject itemCollectionCellPrefab;//Префаб ячейки
    public GameObject ToolTipWindow;
    public GameObject ToolTipItemNameElement;//Текстовый элемет интерфейса(имя игрового объекта)
    public GameObject ToolTipItemDescElement;//Текстовый элемет интерфейса(описание игрового объекта)

    // Use this for initialization
    void Start () {

        itemCollectionlWindow.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseItemInterface();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpAllItem();
            //Cursor.visible = false;
            //Managers.player.moveBlock = false;
        }


    }

    public void CreatEmptyCells()
    {
        //Создаем пустые ячейки в которых затем будем отображать предметы NPC
        for(int i = 0;i<25; i++)
        {
            GameObject newCell = Instantiate(itemCollectionCellPrefab);
            newCell.transform.SetParent(itemCollectionPanel.transform);

            ItemCollectionButton itemButton = newCell.transform.GetChild(0).GetComponent<ItemCollectionButton>();
            if (itemButton != null)
            {
                itemButton.index = i;
            }


        }
    }


    public void PickUpAnItem(int index)
    {
        if(npc.gameItemList.Count - 1 >= index)
        {
            GameItem item = npc.gameItemList[index];
            if (item != null)
            {
                Managers.inventory.AddItem(item);
                npc.gameItemList.Remove(item);
                //Обновить 
                UpdateItemCollectionInterface();

            }

        }

    }

    public void CloseItemInterface()
    {
        Cursor.visible = false;
        Managers.player.moveBlock = false;
        itemCollectionlWindow.SetActive(false);
    }

    public void PickUpAllItem()
    {
        for (int i = 0; i < npc.gameItemList.Count; i++)
        {
            GameItem item = npc.gameItemList[i];
            if (item != null)
            {
                Managers.inventory.AddItem(item);               

            }
            
        }

        npc.gameItemList.Clear();
        UpdateItemCollectionInterface();



    }

    public void UpdateItemCollectionInterface()
    {
        for(int i=0;i< itemCollectionPanel.transform.childCount; i++)
        {
            ItemCollectionButton itemButton = itemCollectionPanel.transform.GetChild(i).GetChild(0).GetComponent<ItemCollectionButton>();


            if (itemButton != null)
            {
                itemButton.index = i;


                if (i > npc.gameItemList.Count - 1)
                {
                    if (itemButton != null) itemButton._gameItemRef = null;
                    itemCollectionPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = itemButton.emptyButtonSprite;
                    itemCollectionPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = itemButton.emptyButtonColor;
                    itemButton.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "";
                    continue;
                }
                else
                {
                    //Установить  картинку объекта
                    itemCollectionPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = npc.gameItemList[i]._itemSprite;
                    itemButton._gameItemRef = npc.gameItemList[i];//Установка связи между ячейкой и предметом
                    itemButton.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "";
                   
                    if (itemButton._gameItemRef._count > 0) itemButton.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = itemButton._gameItemRef._count.ToString();
                    
                }

                //Делаем максимальную прозрачность ячейки
                Color buffColor = itemCollectionPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().color;
                Color newColor = new Color(buffColor.r, buffColor.g, buffColor.b, 255);
                itemCollectionPanel.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = newColor;

            }

        }

    }



}
