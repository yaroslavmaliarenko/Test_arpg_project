using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {

    [SerializeField]  Transform cameraTransform;    
    CharacterController _charController;
    public float speed = 4;    
    AudioSource aSrc;

    // Use this for initialization
    void Start () {
        _charController = GetComponent<CharacterController>();
        aSrc = gameObject.GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {

        if (Managers.player.moveBlock) return;//Происходит боевая анимация 
        

        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        float animationMoveSpeed = 0;

        Vector3 moveVector = Vector3.zero;
        
        if(x!=0 || z != 0)
        {
            moveVector.x = x;
            moveVector.z = z;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveVector.x = x / 3;
                moveVector.z = z / 3;
                animationMoveSpeed = 0.2f;

            }
            else animationMoveSpeed = 0.4f;

            Quaternion tmp = cameraTransform.rotation;
            cameraTransform.eulerAngles = new Vector3(0, cameraTransform.eulerAngles.y, 0);
            moveVector = cameraTransform.TransformDirection(moveVector);//Задаем вектор движения персонажа относительно поворота камеры

            //transform.rotation = Quaternion.LookRotation(moveVector);//Поворачиваем персонажа в сторону вектора движения
            Quaternion direction = Quaternion.LookRotation(moveVector);
            transform.rotation = Quaternion.Lerp(transform.rotation,direction, 15.0f * Time.deltaTime);
            cameraTransform.rotation = tmp;

            moveVector = moveVector * speed;
            moveVector = Vector3.ClampMagnitude(moveVector, speed);            

            moveVector -= new Vector3(0, 9.8f, 0);
            moveVector *= Time.deltaTime;            

            _charController.Move(moveVector);

        }

        Managers.player.myAnimator.SetFloat("speed", animationMoveSpeed);





    }

    void MakeStep()
    {
       
        if (aSrc != null)
        {
            if (Managers.player.stepClip != null)
            {
                aSrc.clip = Managers.player.stepClip;
                aSrc.Play();
            }
            
        }

    }

}
