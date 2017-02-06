namespace RPH.Utilities.AI
{
    public abstract class Component
    {
        public ComplexObject Parent { get; internal set; }

        public virtual void OnStart()
        {
        }

        public virtual void OnContinuousUpdate()
        {
        }

        public virtual void OnTimedUpdate()
        {
        }
    }
}
