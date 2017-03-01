namespace RPH.Utilities.Physics.Ropes
{
    // System
    using System;

    // RPH
    using Rage;

    public class Rope : IPhysicsObject
    {
        readonly RopeVertex[] vertices;
        readonly SpringJoint[] springs;

        readonly Vector3 gravitation;
        readonly float airFrictionConstant;

        readonly float groundFrictionConstant, groundAbsorptionConstant, groundRepulsionConstant;

        public int VertexCount { get { return vertices.Length; } }

        public bool DetectGroundCollisions { get; set; }

        public Rope(Vector3 position, int verticesCount, float verticesMass, float ropeTotalLength, float springConstant, float springFrictionConstant, Vector3 gravitation, float airFrictionConstant, float groundFrictionConstant, float groundAbsorptionConstant, float groundRepulsionConstant)
        {
            float springLength = ropeTotalLength / Math.Max(verticesCount - 1, 1);

            this.gravitation = gravitation;
            this.airFrictionConstant = airFrictionConstant;
            this.groundFrictionConstant = groundFrictionConstant;
            this.groundAbsorptionConstant = groundAbsorptionConstant;
            this.groundRepulsionConstant = groundRepulsionConstant;

            vertices = new RopeVertex[verticesCount];
            for (int i = 0; i < verticesCount; i++)
            {
                vertices[i] = new RopeVertex(verticesMass) { Position = new Vector3(position.X, position.Y + (i * -springLength), position.Z) };
            }

            springs = new SpringJoint[verticesCount - 1];
            for (int i = 0; i < verticesCount - 1; i++)
            {
                springs[i] = new SpringJoint(vertices[i], vertices[i + 1], springLength, springConstant, springFrictionConstant);
            }

            PinVertex(0, position);
        }

        public void Init()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Init();
            }
        }

        public void Solve()
        {
            for (int i = 0; i < springs.Length; i++)
            {
                springs[i].Solve();
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].ApplyForce(gravitation * vertices[i].Mass);

                vertices[i].ApplyForce(-vertices[i].Velocity * airFrictionConstant);

                if (DetectGroundCollisions)
                {
                    HitResult hitResult = World.TraceCapsule(vertices[i].Position, vertices[i].Position, 0.125f, TraceFlags.IntersectWorld);

                    if (hitResult.Hit)
                    {
                        float groundHeight = hitResult.HitPosition.Z;

                        if (vertices[i].Position.Z < groundHeight)
                        {
                            Vector3 v = vertices[i].Velocity;
                            v.Z = 0.0f;

                            vertices[i].ApplyForce(-v * groundFrictionConstant);

                            v = vertices[i].Velocity;
                            v.X = 0.0f;
                            v.Y = 0.0f;

                            if (v.Z < 0.0f)
                            {
                                vertices[i].ApplyForce(-v * groundAbsorptionConstant);
                            }

                            Vector3 force = new Vector3(0.0f, 0.0f, groundRepulsionConstant) * (groundHeight - vertices[i].Position.Z);

                            vertices[i].ApplyForce(force);
                        }
                    }
                }
            }
        }

        public void Simulate(float deltaTime)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Simulate(deltaTime);
            }
        }

        public void PinVertex(int index, Vector3 position) // TODO: PinVertex with Entities
        {
            vertices[index].Pin(position);
        }

        public void UnpinVertex(int index)
        {
            vertices[index].Unpin();
        }

        public Vector3 GetVertexPosition(int index)
        {
            return vertices[index].Position;
        }

        public Vector3 GetVertexVelocity(int index)
        {
            return vertices[index].Velocity;
        }

        public float GetCurrentLength()
        {
            float result = 0.0f;
            for (int i = 0; i < vertices.Length - 1; i++)
            {
                result += Vector3.Distance(vertices[i].Position, vertices[i + 1].Position);
            }
            return result;
        }

        public void DebugDraw()
        {
            for (int i = 0; i < springs.Length; i++)
            {
                springs[i].DebugDraw();
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].DebugDraw();
            }
        }


        private class RopeVertex : Particle
        {
            public RopeVertex(float mass) : base(mass)
            {
            }
        }
    }
}
