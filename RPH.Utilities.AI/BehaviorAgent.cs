namespace RPH.Utilities.AI
{
    public class BehaviorAgent
    {
        public object Target { get; }
        public Blackboard Blackboard { get; }

        public BehaviorAgent(object target, Blackboard blackboard)
        {
            Target = target;
            Blackboard = blackboard;
        }

        public BehaviorAgent(object target) : this(target, new Blackboard())
        {
        }
    }
}
