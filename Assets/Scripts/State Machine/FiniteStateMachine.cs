using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    public BaseState currentState { get; private set; }
    public BaseState previousState { get; private set; }

    public void Initialize(BaseState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(BaseState newState)
    {
        previousState = currentState;

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
