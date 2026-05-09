
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class Actor : MonoBehaviour
{
    public ThirdPersonController player;


    public NavMeshAgent meshAgent;
    public Animator animator;

    [Header("Idle&Talk Info")]
    public float idleTime;
    public float talkRange;


    [Header("Patrol Info")]
    public bool ifPatrol;
    public Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;//用于独立储存巡逻点位置
    private int currentPatrolIndex;//未设置，默认是零


    public bool shouldInIdleState;
    public float turnSpeed;

    protected virtual void Awake()
    {

        meshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

    }

    protected virtual void Start()
    {

        InitializePatrolPont();

    }

    // Update is called once per frame
    protected virtual void Update()
    {


    }

    #region 巡逻相关代码
    private void InitializePatrolPont()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position; //将巡逻点位置存储到数组中//确保巡逻点不受父物体Enemy变化，即开始后巡逻点不会因为enemy移动而移动
            patrolPoints[i].gameObject.SetActive(false);//隐藏
        }
    }
    public Vector3 GetPatrolPoint()
    {
        Vector3 patrolPoint = patrolPointsPosition[currentPatrolIndex];
        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length) //例：3>=3时，实际上只运用到了2，所以正好重置为0
        {
            currentPatrolIndex = 0;
        }

        return patrolPoint;
    }
    public void RotateFaceTarget(Vector3 target)
    {
        if (target == Vector3.zero)
            return;

        //根据目标位置计算需要的旋转角度
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        float yRotationAngles = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, 100  * Time.deltaTime);//y轴上需要旋转的角度
        //不旋转xz是为了保持敌人只在y轴上旋转，避免在xz平面上倾斜（x轴旋转控制俯仰、z轴旋转控制侧倾）

        transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotationAngles, currentEulerAngles.z); //返回新的旋转
    }
    public Vector3 GeneratePathPointToDestination()
    {
        NavMeshAgent agent = meshAgent;
        if (agent.path.corners.Length < 2)
            return agent.destination; //如果没有路径点，返回目标点

        for (int i = 0; i < agent.path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, agent.path.corners[i]) < 1)
                return agent.path.corners[i + 1]; //如果当前点与路径点距离小于1，则返回下一个路径点，用于转向 与上面的FaceTarget方法配合使用，使得敌人可以转向下一个路径点，而不是一直朝向目标点，这样更符合实际情况
        }

        return agent.destination; //如果没有找到合适的路径点，则返回目标点
    }

    public virtual void ChangeToIdleState()
    {

    }
    #endregion  
    public virtual bool IfPlayerInTalkRange()
    { 
        
      if( Vector3.Distance(player.transform.position, transform.position) < talkRange)
        {
            
            return true;

        }
      else 
            return false;
    
    }

}
