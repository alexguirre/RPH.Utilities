namespace RPH.Utilities.AI.Decorators
{
    // RPH
    using Rage;

    /// <summary>
    /// </summary>
    /// <seealso cref="RPH.Utilities.AI.Decorators.BehaviorDecorator" />
    public class Service : BehaviorDecorator
    {
        public delegate void ServiceDelegate(ref BehaviorTreeContext context);

        private readonly int interval = -1; // ms
        protected ServiceDelegate ServiceMethod { get; set; }

        public Service(int interval, ServiceDelegate service, BehaviorTask child) : this(service, child)
        {
            this.interval = interval;
        }

        public Service(ServiceDelegate service, BehaviorTask child) : base(child)
        {
            this.ServiceMethod = service;
        }

        protected override BehaviorStatus OnBehave(ref BehaviorTreeContext context)
        {
            try
            {
                if (interval <= -1)
                {
                    ServiceMethod.Invoke(ref context);
                }
                else
                {
                    uint lastServiceGameTime = context.Agent.Blackboard.Get<uint>("lastServiceGameTime", context.Tree.Id, this.Id, 0);
                    uint gameTime = Game.GameTime;

                    if (gameTime - lastServiceGameTime > interval)
                    {
                        ServiceMethod.Invoke(ref context);
                        context.Agent.Blackboard.Set<uint>("lastServiceGameTime", gameTime, context.Tree.Id, this.Id);
                    }
                }

                return Child.Behave(ref context);

            }
            catch (System.Exception ex)
            {
                Game.LogTrivial($"[{this.GetType().Name}] Exception thrown at OnBehave(): {ex}");

                return BehaviorStatus.Failure;
            }
        }
    }
}
