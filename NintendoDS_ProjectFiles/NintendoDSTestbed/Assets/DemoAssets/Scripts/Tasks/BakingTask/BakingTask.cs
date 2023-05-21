using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface State
{
    public void Enter();
    public void Execute();
    public void Exit();
}

public class StateMachine
{
    private State currentState; //Current State

    public void ChangeState(State newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null) currentState.Execute();
    }
}

public class StateCommand : StateTasks, State
{
    BakingTask owner;
    public StateCommand(BakingTask owner) { this.owner = owner; }

    public void Enter()
    {

    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}

public class BakingTask : MonoBehaviour
{
    StateMachine stateMachine = new StateMachine();

    private void Start()
    {
        stateMachine.ChangeState(new StateCommand(this));
    }

    void Update()
    {
        stateMachine.Update();
    }

}

public class StateTasks
{

}


