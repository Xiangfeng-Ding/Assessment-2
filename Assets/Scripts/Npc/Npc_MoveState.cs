using UnityEngine;

public class Npc_MoveState : NpcState
{
    private Npc npc;

    private Vector3 destination;
    public Npc_MoveState(Actor _npc, NpcStateMachine __stateMachince, string _animBoolName) : base(_npc, __stateMachince, _animBoolName)
    {
        npc = _npc as Npc;

    }

    public override void Enter()
    {
        base.Enter();

        npc.meshAgent.isStopped = false;
        if (npc.ifPatrol)
        {
            destination = npc.GetPatrolPoint();
            npc.meshAgent.SetDestination(destination); 
        }

    }

    public override void Update()
    {
        base.Update();
        npc.RotateFaceTarget(npc.GeneratePathPointToDestination());//时刻朝向 根据目的地生成的一连串路径点 的下一个路径点，这样移动时会更真实
        if (npc.meshAgent.remainingDistance <= npc.meshAgent.stoppingDistance + 0.05)
            npc.stateMachine.ChangeState(npc.idleState);


    }

    public override void Exit()
    {
        base.Exit();
     
    }


}
