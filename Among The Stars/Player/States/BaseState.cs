using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected PlayerController Controller { get; private set; }
    public BaseState(PlayerController controller)
    {
        this.Controller = controller;
    }

    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();
}

public enum StateName
{
    Idle = 0,
    Move,
    Jump,
    SpaceWalk
}

public class StateMachine
{
    public BaseState CurrentState { get; private set; }  // ���� ����
    private Dictionary<StateName, BaseState> states = new Dictionary<StateName, BaseState>();

    public StateMachine(StateName stateName, BaseState state)
    {
        AddState(stateName, state);
        CurrentState = GetState(stateName);
    }

    public void AddState(StateName stateName, BaseState state)  // ���� ���
    {
        if (!states.ContainsKey(stateName))
        {
            states.Add(stateName, state);
        }
    }

    public BaseState GetState(StateName stateName)  // ���� ��������
    {
        if (states.TryGetValue(stateName, out BaseState state))
            return state;
        return null;
    }

    public void DeleteState(StateName removeStateName)  // ���� ����
    {
        if (states.ContainsKey(removeStateName))
        {
            states.Remove(removeStateName);
        }
    }

    public void ChangeState(StateName nextStateName)    // ���� ��ȯ
    {
        Debug.Log("Change " + nextStateName.ToString());
        CurrentState?.OnExitState();   //���� ���¸� �����ϴ� �޼ҵ带 �����ϰ�,
        if (states.TryGetValue(nextStateName, out BaseState newState)) // ���� ��ȯ
        {
            CurrentState = newState;
        }
        CurrentState?.OnEnterState();  // ���� ���� ���� �޼ҵ� ����
    }

    public void UpdateState()
    {
        CurrentState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        CurrentState?.OnFixedUpdateState();
    }
}