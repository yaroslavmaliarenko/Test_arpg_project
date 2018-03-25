using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {

    public Transform playerT;
    public Image HpBarImage;


    [SerializeField] float currentHP = 230;
    [SerializeField]float maxHP = 700;
    bool isDead;

    //===============
    public bool battleMode;//Признак боевого режима
    public bool moveBlock;//Запрет на передвижение
    //===============
    public Animator myAnimator;
    [SerializeField] public AudioClip damageClip;
    [SerializeField] public AudioClip stepClip;

    [SerializeField] Transform rHandT;//Кость или пустой объект прикрепленный к кости руки,для прикрепления обекта
    [SerializeField] Transform backT;//Спина
    public WeaponItem currentWeapon;//Текущее оружие
    bool weapoIsActive;

    // Use this for initialization
    void Start () {
        Cursor.visible = false;
        battleMode = false;
        moveBlock = false;
        weapoIsActive = false;
        UpdateHpBar();		
	}
	
	// Update is called once per frame
	void Update () {

        UpdateHpBar();        

    }

    public bool IsDead()
    {
        return isDead;
    }
    public void HealthRecovery(GameItem _potion,float _countHpRec, float _timeRecovery)
    {
        
        StartCoroutine(UsePotionAction(_potion, _countHpRec, _timeRecovery));
    }

    IEnumerator UsePotionAction(GameItem _potion, float _countHpRec, float _timeRecovery)
    {
       // moveBlock = true;
        if (currentWeapon != null)
        {
            if (weapoIsActive) currentWeapon.gameObject.SetActive(false);
        }
        //myAnimator.SetFloat("speed",0);
        myAnimator.SetTrigger("UsePotion");

        yield return new WaitForSeconds(0.3f);

        _potion.gameObject.SetActive(true);
        _potion.transform.SetParent(rHandT);
        _potion.gameObject.transform.localPosition = _potion.weaponRHandPos;
        _potion.gameObject.transform.localRotation = Quaternion.Euler(_potion.weaponRHandRot);

        StartCoroutine(CoroutineHealthRecovery(_countHpRec, _timeRecovery));
        

        yield return new WaitForSeconds(0.5f);
        _potion.gameObject.SetActive(false);
        Managers.inventory.RemoveItemFromInventory(_potion);

        yield return new WaitForSeconds(0.25f);
       // moveBlock = false;
        if (currentWeapon != null)  currentWeapon.gameObject.SetActive(true);
        

    }

    IEnumerator CoroutineHealthRecovery( float _countHpRec, float _timeRecovery)
    {
        //================
        float timeRecoveryLeft = _timeRecovery;
        //Восстановление HP
        while (timeRecoveryLeft > 0)
        {
            yield return null;
            currentHP += _countHpRec / _timeRecovery * Time.deltaTime;
            timeRecoveryLeft -= Time.deltaTime;            
            if (currentHP > maxHP) currentHP = maxHP;

            UpdateHpBar();
        }
        //=========================
        

    }

    
    public void UpdateHpBar()
    {
        HpBarImage.transform.localScale = new Vector3(currentHP / maxHP, 1, 1);

    }

    public void UseWeapon(WeaponItem weapon)
    {
        if(currentWeapon == weapon)
        {
            if (weapoIsActive)
            {
                //Снять оружие за спину
                weapoIsActive = false;
                currentWeapon.transform.SetParent(backT);
                currentWeapon.gameObject.transform.localPosition = currentWeapon.weaponBackPos;
                currentWeapon.gameObject.transform.localRotation = Quaternion.Euler(currentWeapon.weaponBackRot);

                WeaponDamage wd = currentWeapon.GetComponent<WeaponDamage>();
                if (wd != null)
                {
                    wd.thisPlayerWeapon = true;
                    
                }

                //Анимация + блокировка бега
                battleMode = false;
                myAnimator.SetTrigger("OffBattleMode");
                //myAnimator.SetFloat("speed", 0);                
                //StartCoroutine(SetMoveBlock(0.88f));
            }
            else
            {
                //Взять оружие в руку
                weapoIsActive = true;
                currentWeapon.transform.SetParent(rHandT);
                currentWeapon.gameObject.transform.localPosition = currentWeapon.weaponRHandPos;
                currentWeapon.gameObject.transform.localRotation = Quaternion.Euler(currentWeapon.weaponRHandRot);

                WeaponDamage wd = currentWeapon.GetComponent<WeaponDamage>();
                if (wd != null)
                {
                    wd.thisPlayerWeapon = true;

                }

                //Анимация + блокировка бега

                //myAnimator.SetFloat("speed", 0);
                myAnimator.SetTrigger("OnBattleMode");
                battleMode = true;                
                //StartCoroutine(SetMoveBlock(0.88f));
            }
            
        }
        else
        {
            //Одеть первый раз за спину
            if (currentWeapon != null) currentWeapon.gameObject.SetActive(false);   
            currentWeapon = weapon;
            currentWeapon.gameObject.SetActive(true);
            weapoIsActive = false;
            currentWeapon.transform.SetParent(backT);
            currentWeapon.gameObject.transform.localPosition = currentWeapon.weaponBackPos;
            currentWeapon.gameObject.transform.localRotation= Quaternion.Euler(currentWeapon.weaponBackRot);
        }
    }

    IEnumerator SetMoveBlock(float seconds)
    {
        moveBlock = true;
        yield return new WaitForSeconds(seconds);
        moveBlock = false;
    }

    public void GetDamage(int damage)
    {
        
        currentHP -= damage;

        if (currentHP < 0)
        {
            currentHP = 0;
            isDead = true;
            myAnimator.SetFloat("speed", 0.0f);
            myAnimator.SetBool("IsDead", true);
            moveBlock = true;

        }
    }

}
