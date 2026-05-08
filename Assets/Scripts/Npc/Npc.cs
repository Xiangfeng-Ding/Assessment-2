public class Npc : Actor
{
    public Npc_IdleState idleState { get; private set; }
    public Npc_MoveState moveState { get; private set; }

    public NpcStateMachine stateMachine;
    protected override void Awake()
    {
        base.Awake();
        idleState = new Npc_IdleState(this, stateMachine, "Idle");
        moveState = new Npc_MoveState(this, stateMachine, "Move");
        stateMachine = new NpcStateMachine();

    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(moveState);

    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

    }

    public override void ChangeToIdleState()
    {
        stateMachine.ChangeState(idleState);
    }

    public override bool IfPlayerInTalkRange()
    {
        bool isInBaseRange = base.IfPlayerInTalkRange();

        // 在基类判断的基础上，添加你自己的代码
        if (isInBaseRange)
        {
            stateMachine.ChangeState(idleState);
        }
     

        //  直接返回基类的结果（或者你可以修改后返回）
        return isInBaseRange;
    }



}
