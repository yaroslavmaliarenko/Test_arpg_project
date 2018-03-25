using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class AI_Tarantula : MonoBehaviour
{
    NPC_Creature thisNPC;
    Animator thisAnimator;
    NavMeshAgent thisAgent;

    [SerializeField] AI_STATE currentAi_State;//Текущая модель поведения
    public float attackDistance;//Расстояние ближней атаки
    public float walkingSpeed = 1.5f;//Расстояние ближней атаки
    public float runningSpeed = 4.5f;//Расстояние ближней атаки
    
    //
    public Transform[] WayPoint;
    int currentPointNum;//Номер текущей точки к которой происходит движение
    Transform currentPos;

    //Случайное блуждание отностительно точки
    public Transform randomWalkPoint;
    public float minRadiusRandWalk = 30;//Минимальный радиус удаления от точки
    public float maxRadiusRandWalk = 50;//Максимальный радиус удаления от точки
    public float minLengthRandWalk = 25;//Длина пути между переходами от точки к точке
    public float minStayTime = 20;//Мин время ожидание между перемещением
    public float maxStayTime = 40;//Макс время ожидание между перемещением

    public List<Transform> targetList = new List<Transform>();//Список целей для боя
    public Transform currentTarget;

    private void Awake()
    {
        thisAnimator = GetComponent<Animator>();
        thisAgent = GetComponent<NavMeshAgent>();
        thisNPC = GetComponent<NPC_Creature>();


    }

    // Use this for initialization
    void Start()
    {
        if (WayPoint.Length > 0)
        {
            Debug.Log("currentPointNum = 1");
            currentPos = WayPoint[0];
            currentPointNum = 1;
        }

        SetCurrentState(currentAi_State);
        StartCurrentState();

    }

    // Update is called once per frame
    void Update()
    {





    }

    public void AddNewTarget(Transform tr)
    {
        NPC_Manager npc = tr.GetComponent<NPC_Manager>();
        if (npc != null)
        {
            if (!npc.IsDead())
            {
                if (!targetList.Contains(tr)) targetList.Add(tr);
            }

        }

        if (tr.gameObject.CompareTag("Player"))
        {
            if (!Managers.player.IsDead())
            {
                if (!targetList.Contains(tr)) targetList.Add(tr);
            }
        }

    }

    IEnumerator RandomWalk()
    {
        startLoop:
        //========= Случайно выбираем новую точку для перемещения ========
        Vector3 pos;
        while (true)
        {
            Debug.DrawRay(gameObject.transform.position, Vector3.up * 10, Color.blue);
            thisAnimator.SetFloat("speed", 0.0f);
            thisAgent.isStopped = true;

            float randomX = Random.Range(-1f, 1f);
            float randomZ = Random.Range(-1f, 1f);
            //Debug.Log("randomX -" + randomX);
            //Debug.Log("randomZ -" + randomZ);
            Vector3 direction = new Vector3(randomX, 0, randomZ);
            direction.Normalize();
            Debug.Log("direction Normalize - x =" + direction.x + " y= " + direction.y + " z= " + direction.z);
            direction *= Random.Range(minRadiusRandWalk, maxRadiusRandWalk);

            Debug.Log("direction -" + direction);
            Vector3 newPos = new Vector3(randomWalkPoint.position.x, randomWalkPoint.position.y, randomWalkPoint.position.z) + direction;
            //Debug.Log("Start pos - " + randomWalkPoint.position + " new pos - " + newPos);
            if (Vector3.Distance(newPos, gameObject.transform.position) < minLengthRandWalk)
            {
                yield return null;
                continue;
            }
            pos = newPos;


            Debug.Log("Distance  " + Vector3.Distance(newPos, gameObject.transform.position));
            bool pointIsReachable = false;
            RaycastHit hit;
            if (Physics.Raycast(newPos, Vector3.up, out hit))
            {

                NavMeshPath newPath = new NavMeshPath();
                if (thisAgent.CalculatePath(hit.point, newPath))
                {
                    Debug.Log("pointIsReachable UP");
                    pointIsReachable = true;
                    newPos = hit.point;
                }
            }
            else if (Physics.Raycast(newPos, Vector3.down, out hit))
            {

                NavMeshPath newPath = new NavMeshPath();
                if (thisAgent.CalculatePath(hit.point, newPath))
                {
                    Debug.Log("pointIsReachable DOWN");
                    pointIsReachable = true;
                    newPos = hit.point;
                }
            }

            if (pointIsReachable)
            {
                thisAnimator.SetFloat("speed", 0.5f);
                thisAgent.SetDestination(newPos);
                thisAgent.speed = walkingSpeed;
                thisAgent.isStopped = false;
                break;
            }

            yield return null;
        }

        Debug.Log("New position" + pos);
        Debug.Log("My position  " + gameObject.transform.position);
        //
        while (true)
        {
            if (Vector3.Distance(gameObject.transform.position, thisAgent.destination) < 0.5f)
            {
                thisAnimator.SetFloat("speed", 0.0f);
                thisAgent.isStopped = true;
                break;
            }
            yield return null;
        }

        float stayTime = Random.Range(minStayTime, maxStayTime);
        //Ожидание между переходами от точки к точке
        while (true)
        {
            stayTime -= Time.deltaTime;
            if (stayTime <= 0) goto startLoop;
            yield return null;
        }






    }

    IEnumerator WaypointPatrolState()
    {
        thisAgent.SetDestination(currentPos.position);
        //thisAnimator.speed = 0.9f;
        thisAnimator.SetFloat("speed", 0.5f);
        thisAgent.speed = walkingSpeed;
        thisAgent.isStopped = false;

        while (!thisNPC.battleMode)
        {
            if (Vector3.Distance(transform.position, currentPos.position) <= 1.5)
            {
                //thisAgent.isStopped = true;
                //thisAnimator.SetFloat("speed", 0);
                currentPointNum++;

                if (currentPointNum > WayPoint.Length) currentPointNum = 1;

                currentPos = WayPoint[currentPointNum - 1];

                thisAgent.isStopped = false;
                thisAgent.SetDestination(currentPos.position);
                thisAnimator.SetFloat("speed", 0.5f);

            }

            yield return null;
        }

        StartState(AI_STATE.BATTLE_STATE);


    }


    IEnumerator IdleState()
    {
        while (!thisNPC.battleMode)
        {
            thisAnimator.SetFloat("speed", 0.0f);
            thisAgent.isStopped = true;
            yield return null;

        }

        StartState(AI_STATE.BATTLE_STATE);


    }

    IEnumerator CheckBattleModeCondition()
    {
        while (true)
        {
            yield return null;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15f);

            //Проверить всех НПС вокруг
            foreach (Collider hitCollider in hitColliders)
            {
                NPC_Manager npc = hitCollider.gameObject.GetComponent<NPC_Manager>();
                BattleMode bm = hitCollider.gameObject.GetComponent<BattleMode>();
                if (npc == null && bm == null) continue;

                if (!thisNPC.friendList.Contains(hitCollider.gameObject.tag))
                {
                    Vector3 dist = hitCollider.gameObject.transform.position - transform.position;
                    //Проверяем видим ли мы игровой объект
                    float angle = Mathf.Abs(Vector3.Angle(gameObject.transform.forward, dist.normalized));//Угол между направлением взгляда и персонажем

                    if (angle < 80)
                    {
                        RaycastHit hitInfo;
                        if (Physics.Linecast(transform.position + transform.up, hitCollider.gameObject.transform.position + hitCollider.gameObject.transform.up, out hitInfo))
                        {
                            if (hitInfo.transform.gameObject == hitCollider.gameObject)
                            {
                                AddNewTarget(hitCollider.gameObject.transform);
                            }

                        }

                    }
                    else
                    {
                        //Если не видим но подошли слишком близко
                        if (Vector3.Distance(transform.position, hitCollider.transform.position) <= 4) AddNewTarget(hitCollider.gameObject.transform);
                    }
                }

            }


            //Выбираем текущую цель            
            if (targetList.Count > 0)
            {
                if (currentTarget == null)
                {
                    // ============= Первая установка цели (Начало боя) =================
                    currentTarget = targetList[0];
                    thisNPC.battleMode = true;
                    Debug.Log("currentTarget = NULL ");

                    StopAllCoroutines();
                    StartState(AI_STATE.BATTLE_STATE);

                }
                else
                {
                    //============== В течении боя ======================
                    float dist = Vector3.Distance(transform.position, currentTarget.position);
                    NPC_Manager npc = currentTarget.GetComponent<NPC_Manager>();
                    if (npc != null)
                    {
                        if (npc.IsDead() || dist > 25)
                        {
                            if (targetList.Remove(currentTarget))
                            {
                                StopAllCoroutines();
                                currentTarget = null;
                                if (targetList.Count > 0)
                                {
                                    currentTarget = targetList[0];
                                    StartState(AI_STATE.BATTLE_STATE);
                                }
                                else
                                {
                                    thisNPC.battleMode = false;
                                    //StartCurrentState();
                                    StartCoroutine(OffBattleModeState());//Переход к мирному состоянию
                                }
                                yield break;
                            }
                        }
                    }

                    if (currentTarget != null)
                    {
                        if (currentTarget.gameObject.CompareTag("Player"))
                        {
                            if (Managers.player.IsDead() || dist > 25)
                            {
                                Debug.Log("Player is Dead , target count = " + targetList.Count);
                                if (targetList.Remove(currentTarget))
                                {
                                    StopAllCoroutines();
                                    currentTarget = null;
                                    if (targetList.Count > 0)
                                    {
                                        currentTarget = targetList[0];
                                        StartState(AI_STATE.BATTLE_STATE);
                                    }
                                    else
                                    {
                                        thisNPC.battleMode = false;
                                        StartCoroutine(OffBattleModeState());//Переход к мирному состоянию

                                    }
                                    yield break;
                                }
                            }
                        }
                    }

                }

            }


        }
    }

    IEnumerator BattleModeState()
    {

        if (thisNPC.battleMode && !thisNPC.IsDead())
        {
            StartCoroutine(ChaseState());
            yield break;
        }

        if (!thisNPC.IsDead()) StartCurrentState();
        else StopAllCoroutines();

    }

    IEnumerator ChaseState()
    {
        if (currentTarget == null)
        {
            StartCoroutine(BattleModeState());
            yield break;
        }

        Transform buffTarget = currentTarget;
        if (thisAnimator.GetFloat("speed") < 1)
        {
            //thisAnimator.speed = 1.5f;
            thisAnimator.SetFloat("speed", 1.0f);
        }
        thisAgent.isStopped = false;
        thisAgent.SetDestination(buffTarget.position);
        thisAgent.speed = runningSpeed;



        while (Vector3.Distance(gameObject.transform.position, buffTarget.position) > attackDistance)
        {
            if (!thisNPC.battleMode)
            {
                StartCurrentState();
                yield break;
            }
            thisAgent.SetDestination(buffTarget.position);

            yield return null;
        }



        Vector3 dist = buffTarget.position - gameObject.transform.position;
        float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(gameObject.transform.forward, dist.normalized));
        //Debug.Log("Angel " + angle);

        if (angle > 5) StartCoroutine(LookAtTarget());
        else StartCoroutine(AttackState());

    }

    IEnumerator LookAtTarget()
    {
        Transform buffTarget = currentTarget;

        if (thisAnimator.GetFloat("speed") == 0) thisAnimator.SetFloat("speed", 0.5f);
        thisAgent.isStopped = true;

        Vector3 dist = buffTarget.position - gameObject.transform.position;
        float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(gameObject.transform.forward, dist.normalized));

        while (angle > 5)
        {

            if (Vector3.Distance(gameObject.transform.position, buffTarget.position) > attackDistance)
            {
                Debug.Log("Look at coroutine break " + angle);
                StartCoroutine(BattleModeState());
                yield break;
            }
            Quaternion direction = Quaternion.LookRotation(dist.normalized);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, direction, 120 / angle * Time.deltaTime);

            dist = buffTarget.position - gameObject.transform.position; ;
            angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(gameObject.transform.forward, dist.normalized));

            yield return null;
        }

        StartCoroutine(AttackState());

    }


    IEnumerator AttackState()
    {


        AudioSource aSrcWeapon = thisNPC.currentWeapon.gameObject.GetComponent<AudioSource>();
        Collider weaponCollider = thisNPC.currentWeapon.GetComponent<Collider>();
        WeaponDamage wd = thisNPC.currentWeapon.GetComponent<WeaponDamage>();

        if (thisAnimator.GetFloat("speed") > 0) thisAnimator.SetFloat("speed", 0.0f);
        thisAgent.isStopped = true;
        yield return new WaitForSeconds(0.1f);
        thisAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.25f);

        //Начало нанесения урона
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
        if (!aSrcWeapon.isPlaying) aSrcWeapon.Play();//Звук удара

        yield return new WaitForSeconds(0.3f);

        //Конец нанесения урона
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        if (wd != null) wd.ClearDamageList();

        yield return new WaitForSeconds(0.25f);

        StartCoroutine(BattleModeState());

    }

    //Переход из боевого режима в мирное состояние
    IEnumerator OffBattleModeState()
    {
        thisAgent.isStopped = true;
        thisAnimator.SetFloat("speed", 0);
        yield return new WaitForSeconds(3.5f);
        StartCurrentState();

    }


    public void SetCurrentState(AI_STATE _state)
    {
        currentAi_State = _state;


    }

    public void StartCurrentState()
    {
        StopAllCoroutines();

        switch (currentAi_State)
        {
            case AI_STATE.IDLE_STATE:
                StartCoroutine(IdleState());
                StartCoroutine(CheckBattleModeCondition());
                break;
            case AI_STATE.PATROL_STATE:
                StartCoroutine(WaypointPatrolState());
                StartCoroutine(CheckBattleModeCondition());
                break;
            case AI_STATE.RANDOM_WALK_STATE:
                StartCoroutine(RandomWalk());
                StartCoroutine(CheckBattleModeCondition());
                break;
            case AI_STATE.BATTLE_STATE:
                StartCoroutine(BattleModeState());
                StartCoroutine(CheckBattleModeCondition());
                break;
        }

    }

    public void StartState(AI_STATE _state)
    {

        StopAllCoroutines();

        switch (_state)
        {
            case AI_STATE.IDLE_STATE:
                StartCoroutine(IdleState());
                StartCoroutine(CheckBattleModeCondition());
                break;
            case AI_STATE.PATROL_STATE:
                StartCoroutine(WaypointPatrolState());
                StartCoroutine(CheckBattleModeCondition());
                break;
            case AI_STATE.RANDOM_WALK_STATE:
                StartCoroutine(RandomWalk());
                StartCoroutine(CheckBattleModeCondition());
                break;
            case AI_STATE.BATTLE_STATE:
                StartCoroutine(BattleModeState());
                StartCoroutine(CheckBattleModeCondition());
                break;
        }

    }
}

