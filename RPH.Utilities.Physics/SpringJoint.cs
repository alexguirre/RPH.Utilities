namespace RPH.Utilities.Physics
{
    // RPH
    using Rage;

    public class SpringJoint : Joint
    {
        public float SpringLength { get; }
        public float SpringConstant { get; }
        public float FrictionConstant { get; }

        public SpringJoint(Particle a, Particle b, float springLength, float springConstant = 5000f, float frictionConstant = 0.5f) : base(a, b)
        { 
            SpringLength = springLength;
            SpringConstant = springConstant;
            FrictionConstant = frictionConstant;
        }

        public override void Init()
        {
        }

        public override void Solve()
        {
            Vector3 springVector = A.Position - B.Position;

            float distance = springVector.Length();

            Vector3 force = Vector3.Zero;

            if (distance != 0.0f)
            {
                force += -(springVector / distance) * (distance - SpringLength) * SpringConstant;
            }

            force += -(A.Velocity - B.Velocity) * FrictionConstant;

            A.ApplyForce(force);
            B.ApplyForce(-force);
        }

        public override void Simulate(float deltaTime)
        {
        }
    }
}
