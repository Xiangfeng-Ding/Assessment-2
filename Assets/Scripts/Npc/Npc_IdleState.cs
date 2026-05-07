using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc_IdleState : NpcState
{
    private Npc npc;
    public Npc_IdleState(Actor _npc, NpcStateMachine __stateMachince, string _animBoolName) : base(_npc, __stateMachince, _animBoolName)
    {
        npc = _npc as Npc;

    }
    public override void Enter()
    {
        base.Enter();
        stateTimer = npc.idleTime;
        npc.meshAgent.isStopped = true;

    }
    public override void Update()
    {
        base.Update();
        if (stateTimer < 0 )//&& !UIManager.Instance.dialogue.activeSelf)//对话时，是不会走的
            npc.stateMachine.ChangeState(npc.moveState);
    }
    public override void Exit()
    {
        base.Exit();
        npc.shouldInIdleState = false;
    }


}
