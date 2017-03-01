using System.Diagnostics.CodeAnalysis;

namespace RPH.Utilities.Physics
{
    // RPH
    using Rage;

    public class Particle : IPhysicsObject
    {
        public float Mass { get; set; }

        private Vector3 position, pinPosition;
        public Vector3 Position { get { return IsPinned ? pinPosition : position; } set { position = value; } }
        public Vector3 Force { get; set; }
        public Vector3 Velocity { get; set; }

        public bool IsPinned { get; set; }

        public bool IsAffectedByGravity { get; set; } = false;
        public Vector3 Gravitation { get; set; } = new Vector3(0f, 0f, -9.81f);

        public bool IsAffectedByAirFriction { get; set; } = false;
        public float AirFrictionConstant { get; set; } = 0.025f;

        public Particle(float mass)
        {
            Mass = mass;
        }

        public void ApplyForce(Vector3 force)
        {
            Force += force;
        }

        public void Pin(Vector3 position)
        {
            pinPosition = position;
            IsPinned = true;
        }

        public void Unpin()
        {
            pinPosition = Vector3.Zero;
            IsPinned = false;
        }

        public virtual void Init()
        {
            Force = Vector3.Zero;
            if (IsPinned)
            {
                Velocity = Vector3.Zero;
            }
        }

        public virtual void Solve()
        {
        }

        public virtual void Simulate(float deltaTime)
        {
            if (IsPinned)
                return;

            if (IsAffectedByGravity)
            {
                ApplyForce(Gravitation * Mass); // Force = gravitation * mass
            }

            if (IsAffectedByAirFriction)
            {
                ApplyForce(-Velocity * AirFrictionConstant);
            }

            Velocity += (Force / Mass) * deltaTime;

            Position += Velocity * deltaTime;
        }

        public void DebugDraw()
        {
            Util.DrawMarker(28, Position, Vector3.Zero, Rotator.Zero, new Vector3(MathHelper.Max(Mass / 10.0f, 0.125f)), IsPinned ? System.Drawing.Color.FromArgb(140, 20, 20, 20) : System.Drawing.Color.FromArgb(110, 255, 0, 0));
            //Util.DrawLine(Position, Position + Velocity.ToNormalized() * Velocity.Length(), System.Drawing.Color.Red);
        }
    }
}
