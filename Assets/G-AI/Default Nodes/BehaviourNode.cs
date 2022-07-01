using UnityEngine;

public abstract class BehaviourNode : ScriptableObject
{
    public enum State
    {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool isStarted;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public Blackboard blackboard;
    
    public string nodeName;

    public virtual string NodeName => "";

    public State Update()
    {
        if (!isStarted)
        {
            OnStart();
            isStarted = true;
        }

        state = OnUpdate();

        if (state == State.Failure || state == State.Success)
        {
            OnStop();
            isStarted = false;
        }

        return state;
    }

    public virtual void UpdateState(State newState)
    {
        state = newState;

        if (state == State.Failure || state == State.Success)
        {
            OnStop();
            isStarted = false;
        }
    }

    public virtual BehaviourNode Clone()
    {
        return Instantiate(this);
    }

    public virtual State OnUpdate()
    {
        return State.Success;
    }
    
    public virtual void OnStart() { }
    public virtual void OnStop() { }
}