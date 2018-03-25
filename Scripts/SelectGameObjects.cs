using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGameObjects : MonoBehaviour {

    Camera myCam;
    InteractiveObject selectedObject;

    // Use this for initialization
    void Start () {

        myCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        

    }
	
	// Update is called once per frame
	void Update () {

        
        if (Managers.player.moveBlock) return;

        if (selectedObject != null)
        {
            IInteractiveObject iObject = selectedObject.gameObject.GetComponent<IInteractiveObject>();
            if (iObject != null) iObject.HideObjectName();

        }
        selectedObject = null;

        if (Managers.player.battleMode) return;//Нельзя в боевом режиме собирать предметы или обращаться к НПС

        float prevDotValue = 0;//Предыдущее значение скалярного произведения
        

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.5f);
        
        //Обработка выделения ближайшего объекта
        foreach (Collider hitCollider in hitColliders)
        {
            InteractiveObject laObject = hitCollider.GetComponent<InteractiveObject>();
            
            if(laObject != null)
            {
                Vector3 dis =  laObject.transform.position - transform.position;
                //Debug.Log("Dot select" + Vector3.Dot(transform.forward, dis.normalized));
                if(Vector3.Dot(transform.forward,dis.normalized) > 0.5)
                {
                    
                    //Из всех предметов которы расположены перед персонажем выбираем тот на которого ближе смотрит камера
                    Vector3 camLookForward = new Vector3(myCam.transform.forward.x, 0, myCam.transform.forward.z);
                    if (Vector3.Dot(camLookForward, dis.normalized) > prevDotValue)
                    {                        
                        selectedObject = laObject;
                        prevDotValue = Vector3.Dot(camLookForward, dis.normalized);

                    }
                        

                }                
            }               
        }


        //Отображение имени объекта
        if (selectedObject != null)
        {
            IInteractiveObject iObject = selectedObject.gameObject.GetComponent<IInteractiveObject>();
            if (iObject != null) iObject.ShowObjectName();
        }
        


        //Обарботка нажатия на выделенном объекте (выделенным объектом может быть игровой перс, игр предмет или механизм)
        //- Добавление в инвентарь
        if (selectedObject != null && Input.GetMouseButtonDown(0) && Managers.inventory.inventoryPanelWindow.activeSelf == false)
        {
            GameItem _item = selectedObject.gameObject.GetComponent<GameItem>();
            if (_item != null)
            {
                StartCoroutine(AddItem(_item));
            }

            NPC_Manager _npc = selectedObject.gameObject.GetComponent<NPC_Manager>();
            if (_npc != null)
            {
                _npc.ExploreNpc();
            }

        }




    }

    IEnumerator AddItem(GameItem item)
    {
        if (item.isEquipped) yield break;

        Managers.player.myAnimator.SetFloat("speed", 0);
        Managers.player.myAnimator.SetTrigger("UseObject");        
        Managers.player.moveBlock = true;

        yield return new WaitForSeconds(0.7f);

        Managers.inventory.AddItem(item);
        item.HideItemName();
        selectedObject = null;

        yield return new WaitForSeconds(0.2f);

        Managers.player.moveBlock = false;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //if (selectedObject != null)
        //{
            
        //    Managers.player.myAnimator.SetLookAtWeight(0.2f);
        //    Managers.player.myAnimator.SetLookAtPosition(selectedObject.gameObject.transform.position);
            

        //}
        
    }






}
