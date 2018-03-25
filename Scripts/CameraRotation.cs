using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {

    [SerializeField] Transform target;
    public float rotSpeed = 10;
    public float scrollSpeed = 1;
    private float _rotY;
    private float _rotX;
    private Vector3 offset;

    // Use this for initialization
    void Start () {

        //transform.LookAt(target);
        transform.LookAt(target.position + Vector3.up * 2);
        _rotY = transform.eulerAngles.y;
        _rotX = transform.eulerAngles.x;
        offset = target.position - transform.position;        
        //Debug.Log("Player pos " + target.position);
        //Debug.Log("Camera pos " + transform.position);
        Debug.Log("offset " + offset);

    }
	
	// Update is called once per frame
	void Update () {

        
    }

    private void LateUpdate()
    {
        //if (Managers.inventory.inventoryPanelWindow.activeSelf == true) return;

        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        float mouseWhell = Input.GetAxis("Mouse ScrollWheel");

        //Scroll
        if (mouseWhell != 0)
        {
            Debug.Log("mouseWhell " + mouseWhell);
            if (mouseWhell > 0)
            {
                //Debug.Log("forward " + transform.forward);                
                //transform.position -= transform.TransformDirection(Vector3.forward*0.1f);

                offset += offset.normalized * Time.deltaTime * scrollSpeed;                
                //offset += transform.TransformDirection(Vector3.forward) * Time.deltaTime * scrollSpeed;
            }
            else if (mouseWhell < 0)
            {
                //transform.position += transform.TransformDirection(Vector3.forward * 0.1f);
                offset -= offset.normalized * Time.deltaTime * scrollSpeed;
            }

            

        }

        _rotY += x * rotSpeed * Time.deltaTime;
        _rotX -= y * rotSpeed * Time.deltaTime;
        //Debug.Log("_rotY " + _rotY);
        //Debug.Log("_rotX " + _rotX);

        //if (_rotX > 40)//Ограничить облет камеры вокруг персонажа по оси У
        //{
        //    _rotX = 40;

        //}       
        
        //if (x!=0 || y != 0)
        //{
            Quaternion rotation = Quaternion.Euler(_rotX, _rotY, 0);
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Debug.Log("Player pos " + target.position);
                //Debug.Log("Old Camera pos " + transform.position);
                //Debug.Log("Quaternion rotation " + rotation);
                Debug.Log("NEw offset rotation " + (target.position - transform.position));
                //Debug.Log("rotation * offset " + rotation * offset);

            }
            
            transform.position = target.position - (rotation * offset);
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //Debug.Log("New Camera pos " + transform.position);

            }
            

            transform.LookAt(target.position + Vector3.up * 2);
            //transform.LookAt(target);

        //}



    }
}
