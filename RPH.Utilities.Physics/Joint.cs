namespace RPH.Utilities.Physics
{
    public abstract class Joint : IPhysicsObject
    {
        public Particle A { get; }
        public Particle B { get; }

        protected Joint(Particle a, Particle b)
        {
            A = a;
            B = b;
        }

        public virtual void Init()
        {
        }

        public virtual void Solve()
        {
        }

        public virtual void Simulate(float deltaTime)
        {
        }

        public void DebugDraw()
        {
            Util.DrawLine(A.Position, B.Position, System.Drawing.Color.Blue);
        }
    }
}
