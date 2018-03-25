using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraRotation : MonoBehaviour {

    [SerializeField] Transform target;
    public float rotSpeed = 50;    
    private float _rotY;
    private float _rotX;
    public float rotXmin = -70;
    public float rotXmax= 85;

    public float xSmoothTime = 0.02f;
    private float xVelocity;
    public float ySmoothTime = 0.02f;
    private float yVelocity;

    public float xCollideSmoothTime = 0.1f;
    private float xCollideVelocity;
    public float yCollideSmoothTime = 0.1f;
    private float yCollideVelocity;


    public float scrollSpeed = 5;
    private float distance = 3;//Расстояние от персонажа до камеры
    private float desiredDistance;//Желаемое асстояние от персонажа до камеры
    public float distanceMin = 2;//Min Расстояние от персонажа до камеры
    public float distanceMax = 5;//Min Расстояние от персонажа до камеры
    public float distanceSmoothTime = 0.3f;
    private float distanceVelocity;
    public float collideDistanceSmoothTime = 0.05f;
    private float collideDistanceVelocity;


    Vector3 desiredPosition;
    Vector3 cameraPosition;
    Camera thisCamera;

    // Use this for initialization
    void Start () {
        thisCamera = GetComponent<Camera>();
        transform.LookAt(target.position + Vector3.up * 2);
        _rotY = 0;
        _rotX = 60;
        desiredDistance = distance;
        cameraPosition = transform.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {

        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        float mouseWhell = Input.GetAxis("Mouse ScrollWheel");

        _rotY += x * rotSpeed * Time.deltaTime;
        _rotX -= y * rotSpeed * Time.deltaTime;
        //Ограничим углы вращения
        _rotX = ClampAngel(_rotX, rotXmin, rotXmax);

        //Ограничим расстояние отдаления/приближения камеры
        desiredDistance = Mathf.Clamp(desiredDistance - mouseWhell * scrollSpeed, distanceMin, distanceMax);

        
        //Проверим не заслоняют ли  другие объекты вид с камеры  
        
        float collidePointDistance = GetVisibilityPointDistance();
            
        if (collidePointDistance != -1f)
        {
            
            desiredDistance = collidePointDistance;                           
            CalculateCameraCollidedPosition();
        }
        else CalculateDesiredPosition();
        




        transform.LookAt(target.position + Vector3.up * 2);
    }


    float ClampAngel(float angel, float minAngel, float maxAngel)
    {
        do
        {
            if (angel > 360) angel -= 360;
            if (angel < -360) angel += 360;

        }
        while (angel<-360 || angel > 360);


        return Mathf.Clamp(angel, minAngel, maxAngel);
    }

    void CalculateDesiredPosition()
    {
        
        //Сгладим расстояние
        distance = Mathf.SmoothDamp(distance, desiredDistance, ref distanceVelocity, distanceSmoothTime);

        //Расчет позиции
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(_rotX, _rotY, 0);

        desiredPosition = (target.position + Vector3.up * 2) + rotation * direction;

        //Сглаживание позиции
        float xPos = Mathf.SmoothDamp(cameraPosition.x, desiredPosition.x, ref xVelocity, xSmoothTime);
        float yPos = Mathf.SmoothDamp(cameraPosition.y, desiredPosition.y, ref yVelocity, ySmoothTime);
        float zPos = Mathf.SmoothDamp(cameraPosition.z, desiredPosition.z, ref xVelocity, xSmoothTime);

        cameraPosition = new Vector3(xPos, yPos, zPos);


        //Окончательная установка позиции
        transform.position = cameraPosition;

    }

    void CalculateCameraCollidedPosition()
    {

        //Сгладим расстояние
        distance = Mathf.SmoothDamp(distance, desiredDistance, ref collideDistanceVelocity, collideDistanceSmoothTime);

        //Расчет позиции
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(_rotX, _rotY, 0);

        desiredPosition = (target.position + Vector3.up * 2) + rotation * direction;

        //Сглаживание позиции
        float xPos = Mathf.SmoothDamp(cameraPosition.x, desiredPosition.x, ref xCollideVelocity, xCollideSmoothTime);
        float yPos = Mathf.SmoothDamp(cameraPosition.y, desiredPosition.y, ref yCollideVelocity, yCollideSmoothTime);
        float zPos = Mathf.SmoothDamp(cameraPosition.z, desiredPosition.z, ref xCollideVelocity, xCollideSmoothTime);

        cameraPosition = new Vector3(xPos, yPos, zPos);


        //Окончательная установка позиции
        transform.position = cameraPosition;

    }

    float GetVisibilityPointDistance( )
    {
        float _desiredDistance = -1f;

        
        if (thisCamera == null)
        {
            thisCamera = GetComponent<Camera>();
            if (thisCamera == null)  return _desiredDistance;
        }
        ClipPlane cpPonits = new ClipPlane(thisCamera);

        //Debug.DrawLine(target.position + Vector3.up * 2, cpPonits.leftBottom,Color.blue);
        //Debug.DrawLine(target.position + Vector3.up * 2, cpPonits.rightBottom, Color.blue);
        //Debug.DrawLine(target.position + Vector3.up * 2, cpPonits.leftTop, Color.blue);
        //Debug.DrawLine(target.position + Vector3.up * 2, cpPonits.rightTop, Color.blue);
        //Debug.DrawLine(target.position + Vector3.up * 2, cpPonits.center, Color.blue);
        //Debug.DrawLine(target.position + Vector3.up * 2, cpPonits.centerTop, Color.blue);
        //Debug.DrawLine(target.position + Vector3.up * 2, cpPonits.centerBottom, Color.blue);

        RaycastHit hitInfo;
        Vector3 start = target.position + Vector3.up * 2;
        //Пускаем луч до тех пор, пока тег объекта столкновения не будет отличатся от "Player", либо пока не промажем
        for (int i = 0; i < 10; i++)
        {
            if (Physics.Linecast(start, cpPonits.leftTop, out hitInfo))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    start = hitInfo.point;                    
                }
                else
                {
                    if ((hitInfo.distance < _desiredDistance || _desiredDistance == -1f) && hitInfo.transform.gameObject.isStatic)
                    {
                        _desiredDistance = hitInfo.distance;
                        break;
                    }
                }

            }
            else break;
        }

        // 2 точка       
        start = target.position + Vector3.up * 2;
        //Пускаем луч до тех пор, пока тег объекта столкновения не будет отличатся от "Player", либо пока не промажем
        for (int i = 0; i < 10; i++)
        {
            if (Physics.Linecast(start, cpPonits.rightTop, out hitInfo))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    start = hitInfo.point;
                    
                }
                else
                {
                    if ((hitInfo.distance < _desiredDistance || _desiredDistance == -1f) && hitInfo.transform.gameObject.isStatic)
                    {
                        _desiredDistance = hitInfo.distance;
                        break;
                    }
                }

            }
            else break;
        }

        start = target.position + Vector3.up * 2;
        //Пускаем луч до тех пор, пока тег объекта столкновения не будет отличатся от "Player", либо пока не промажем
        for (int i=0; i<10; i++)
        {
            if (Physics.Linecast(start, cpPonits.leftBottom, out hitInfo))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    start = hitInfo.point;
                    
                }
                else
                {
                    if ((hitInfo.distance < _desiredDistance || _desiredDistance == -1f) && hitInfo.transform.gameObject.isStatic)
                    {
                        _desiredDistance = hitInfo.distance;
                        break;
                    }
                }

            }
            else break;
        }
            
        


        start = target.position + Vector3.up * 2;
        //Пускаем луч до тех пор, пока тег объекта столкновения не будет отличатся от "Player", либо пока не промажем
        for (int i = 0; i < 10; i++)
        {
            if (Physics.Linecast(start, cpPonits.rightBottom, out hitInfo))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    start = hitInfo.point;
                    
                }
                else
                {
                    if ((hitInfo.distance < _desiredDistance || _desiredDistance == -1f) && hitInfo.transform.gameObject.isStatic)
                    {
                        _desiredDistance = hitInfo.distance;
                        break;
                    }
                }

            }
            else break;
        }

        start = target.position + Vector3.up * 2;
        //Пускаем луч до тех пор, пока тег объекта столкновения не будет отличатся от "Player", либо пока не промажем
        for (int i = 0; i < 10; i++)
        {
            if (Physics.Linecast(start, cpPonits.center, out hitInfo))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    start = hitInfo.point;
                    
                }
                else
                {
                    if ((hitInfo.distance < _desiredDistance || _desiredDistance == -1f) && hitInfo.transform.gameObject.isStatic)
                    {
                        _desiredDistance = hitInfo.distance;
                        break;
                    }
                }

            }
            else break;
        }

        start = target.position + Vector3.up * 2;
        //Пускаем луч до тех пор, пока тег объекта столкновения не будет отличатся от "Player", либо пока не промажем
        for (int i = 0; i < 10; i++)
        {
            if (Physics.Linecast(start, cpPonits.centerTop, out hitInfo))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    start = hitInfo.point;
                    
                }
                else
                {
                    if ((hitInfo.distance < _desiredDistance || _desiredDistance == -1f) && hitInfo.transform.gameObject.isStatic)
                    {
                        _desiredDistance = hitInfo.distance;
                        break;
                    }
                }

            }
            else break;
        }

        start = target.position + Vector3.up * 2;
        //Пускаем луч до тех пор, пока тег объекта столкновения не будет отличатся от "Player", либо пока не промажем
        for (int i = 0; i < 10; i++)
        {
            if (Physics.Linecast(start, cpPonits.centerBottom, out hitInfo))
            {
                if (hitInfo.collider.tag == "Player")
                {
                    start = hitInfo.point;
                    
                }
                else
                {
                    if ((hitInfo.distance < _desiredDistance || _desiredDistance == -1f) && hitInfo.transform.gameObject.isStatic)
                    {
                        _desiredDistance = hitInfo.distance;
                        break;
                    }
                }

            }
            else break;
        }


        return _desiredDistance;
    }




}

public struct ClipPlane
{
    public Vector3 leftTop;
    public Vector3 rightTop;
    public Vector3 leftBottom;
    public Vector3 rightBottom;
    public Vector3 center;
    public Vector3 centerTop;
    public Vector3 centerBottom;

    public ClipPlane(Camera myCamera)
    {
        
        Transform tr = myCamera.transform;
        float distance = myCamera.nearClipPlane;

        //Высота ближней плоскости отсечения
        float height = distance * Mathf.Tan((myCamera.fieldOfView / 2) * Mathf.Deg2Rad);

        //ее ширина
        float width = height * myCamera.aspect;

        distance *= 0.8f;

        leftTop = tr.position - tr.right * width + tr.up * height + tr.forward * distance;
        rightTop = tr.position + tr.right * width + tr.up * height + tr.forward * distance;
        leftBottom = tr.position - tr.right * width - tr.up * height + tr.forward * distance;
        rightBottom = tr.position + tr.right * width   - tr.up * height + tr.forward * distance;
        center = tr.position + tr.forward * distance;
        centerTop = tr.position + tr.up * height + tr.forward * distance;
        centerBottom = tr.position - tr.up * height + tr.forward * distance;
    }

}
