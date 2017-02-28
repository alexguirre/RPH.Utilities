namespace RPH.Utilities.Physics
{
    // RPH
    using Rage;

    public class StringJoint : Joint
    {
        public float StringLength { get; }
        public float StringConstant { get; }
        public float FrictionConstant { get; }

        public StringJoint(Particle a, Particle b, float stringLength, float stringConstant = 5000f, float frictionConstant = 0.5f) : base(a, b)
        {
            StringLength = stringLength;
            StringConstant = stringConstant;
            FrictionConstant = frictionConstant;
        }

        public override void Init()
        {
        }

        public override void Solve()
        {
            Vector3 stringVector = A.Position - B.Position;

            float distance = stringVector.Length();

            Vector3 force = Vector3.Zero;

            if (distance > StringLength)
            {
                if (distance != 0.0f)
                {
                    force += -(stringVector / distance) * (distance - StringLength) * StringConstant;
                }

                force += -(A.Velocity - B.Velocity) * FrictionConstant;

                A.ApplyForce(force);
                B.ApplyForce(-force);
            }
        }

        public override void Simulate(float deltaTime)
        {
        }
    }
}
