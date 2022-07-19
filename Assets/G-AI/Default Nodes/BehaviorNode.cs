using System.Linq;
using UnityEngine;

public abstract class BehaviorNode : ScriptableObject
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

    public void SetBlackboard(Blackboard setBlackboard)
    {
        blackboard = setBlackboard;
        //ASSIGN BLACKBOARD KEY SELECTOR TO BLACKBOARD
        var properties = this.GetType().GetFields().
                Where(info => info.FieldType == typeof(BlackboardKeySelector));

        foreach (var prop in properties)
        {
            var newSelector = (BlackboardKeySelector)this.GetType().GetField(prop.Name).GetValue(this);
            typeof(BlackboardKeySelector).GetField("blackboard").SetValue(newSelector, setBlackboard);
            //prop.SetValue(this, new BlackboardKeySelector(setBlackboard));
        }
    }

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

    public virtual BehaviorNode Clone()
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