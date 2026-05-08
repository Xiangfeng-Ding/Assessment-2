using UnityEngine;
using UnityEngine.AI;

public class NpcState
{
    protected Actor npcBase { get; private set; }
    protected NpcStateMachine stateMachine { get; private set; }

    protected string animBoolName;
    protected float stateTimer;
    protected bool animationTiggerCalled;

    public NpcState(Actor _npc, NpcStateMachine __stateMachince, string _animBoolName)
    {
        this.npcBase = _npc;
        this.stateMachine = __stateMachince;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()

    {
        animationTiggerCalled = false;
        npcBase.animator.SetBool(animBoolName, true);

    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        npcBase.animator.SetBool(animBoolName, false);
    }



    public bool AnimationTigger() => animationTiggerCalled = true;
}
