using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMode : MonoBehaviour {

    AudioSource aSourceWeapon;    
    public bool attack;
    public bool startDamage;    

    NPC_Manager selectedObject;
    Camera myCam;

    // Use this for initialization
    void Start () {
        
        attack = false;
        startDamage = false;
        myCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        aSourceWeapon = Managers.player.currentWeapon.gameObject.GetComponent<AudioSource>();
        aSourceWeapon.loop = false;
        aSourceWeapon.playOnAwake = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (selectedObject != null)
        {
            selectedObject.bDrwaHpBar = false;
            //IInteractiveObject iObject = selectedObject.gameObject.GetComponent<IInteractiveObject>();
            //if (iObject != null) iObject.HideObjectName();

        }

        selectedObject = null;

        

        //---------------------
        

        if (Managers.player.battleMode)
        {
            

            if (Input.GetKeyDown(KeyCode.Mouse0) && attack == false)//Нажатие левой кнопки мыши
            {
                StartCoroutine(Attack());
            }

            //======================================
            float prevDotValue = 0;//Предыдущее значение скалярного произведения


            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 12.0f);

            //Обработка выделения ближайшего объекта
            foreach (Collider hitCollider in hitColliders)
            {
                NPC_Manager enemyPlayer = hitCollider.GetComponent<NPC_Manager>();

                if (enemyPlayer != null && !enemyPlayer.IsDead())
                {
                    Vector3 dis = enemyPlayer.transform.position - transform.position;

                    //if (Vector3.Dot(transform.forward, dis.normalized) > 0.5)
                    //{

                        //Из всех предметов которы расположены перед персонажем выбираем тот на которого ближе смотрит камера
                        Vector3 camLookForward = new Vector3(myCam.transform.forward.x, 0, myCam.transform.forward.z);
                        if (Vector3.Dot(camLookForward, dis.normalized) > prevDotValue)
                        {
                            selectedObject = enemyPlayer;
                            prevDotValue = Vector3.Dot(camLookForward, dis.normalized);

                        }


                    //}
                }
            }


            //Отображение хп бара  объекта
            if (selectedObject != null)
            {
                selectedObject.bDrwaHpBar = true;
                //IInteractiveObject iObject = selectedObject.gameObject.GetComponent<IInteractiveObject>();
                //if (iObject != null) iObject.ShowObjectName();
            }


        }


    }

    IEnumerator Attack()
    {
        aSourceWeapon = Managers.player.currentWeapon.gameObject.GetComponent<AudioSource>();
        Collider weaponCollider = Managers.player.currentWeapon.GetComponent<Collider>();
        WeaponDamage wd = Managers.player.currentWeapon.GetComponent<WeaponDamage>();

        attack = true;
        Managers.player.moveBlock = true;

        //Managers.player.myAnimator.SetFloat("speed", 0);
        Managers.player.myAnimator.SetTrigger("AttackRight");

        yield return new WaitForSeconds(0.42f);

        

        //Начало нанесения урона
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
        
        startDamage = true;

        //yield return new WaitForSeconds(0.1f);
        if (!aSourceWeapon.isPlaying) aSourceWeapon.Play();//Звук удара

        yield return new WaitForSeconds(0.35f);
        
        //Конец нанесения урона
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
        startDamage = false;


        yield return new WaitForSeconds(0.37f);
        //Завершение анимации
        
        if (wd != null) wd.ClearDamageList();
        attack = false;
        Managers.player.moveBlock = false;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(selectedObject!= null)
        {
            //Managers.player.myAnimator.SetLookAtWeight(0.7f);
            //Managers.player.myAnimator.SetLookAtPosition(selectedObject.gameObject.transform.position + ((Vector3.up * 2 )*0.9f) );

        }
        
    }









}
