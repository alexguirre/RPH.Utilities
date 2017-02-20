namespace RPH.Utilities.AI
{
    // System
    using System;

    public abstract class BehaviorTask
    {
        public Guid Id { get; }

        public BehaviorTask()
        {
            Id = Guid.NewGuid();
        }

        protected virtual void OnEnter(ref BehaviorTreeContext context) { }
        protected virtual void OnOpen(ref BehaviorTreeContext context) { }
        protected virtual BehaviorStatus OnBehave(ref BehaviorTreeContext context) => BehaviorStatus.Success;
        protected virtual void OnClose(ref BehaviorTreeContext context) { }
        protected virtual void OnExit(ref BehaviorTreeContext context) { }

        public BehaviorStatus Behave(ref BehaviorTreeContext context)
        {
            EnterInternal(ref context);

            if (!context.Agent.Blackboard.Get<bool>("isOpen", context.Tree.Id, this.Id))
            {
                OpenInternal(ref context);
            }

            BehaviorStatus status = BehaveInternal(ref context);

            if (status != BehaviorStatus.Running)
            {
                CloseInternal(ref context);
            }

            ExitInternal(ref context);

            return status;
        }

        internal void EnterInternal(ref BehaviorTreeContext context)
        {
            context.OnEnterNode(this.Id);
            OnEnter(ref context);
        }

        internal void OpenInternal(ref BehaviorTreeContext context)
        {
            context.OnOpenNode(this.Id);
            context.Agent.Blackboard.Set<bool>("isOpen", true, context.Tree.Id, this.Id);
            OnOpen(ref context);
        }

        internal BehaviorStatus BehaveInternal(ref BehaviorTreeContext context)
        {
            context.OnBehaveNode(this.Id);
            return OnBehave(ref context);
        }

        internal void CloseInternal(ref BehaviorTreeContext context)
        {
            context.OnCloseNode(this.Id);
            context.Agent.Blackboard.Set<bool>("isOpen", false, context.Tree.Id, this.Id);
            OnClose(ref context);
        }

        internal void ExitInternal(ref BehaviorTreeContext context)
        {
            context.OnExitNode(this.Id);
            OnExit(ref context);
        }

        public virtual BehaviorTask GetTaskById(Guid id)
        {
            if (Id == id)
                return this;
            return null;
        }
    }
}
