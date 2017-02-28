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

        Vector3 gravitation;
        float airFrictionConstant;

        public int VertexCount { get { return vertices.Length; } }

        //public Vector3 Position { get { return vertices[0].Position; } set { vertices[0].Position = value; } }
        //public Vector3 Velocity { get { return vertices[0].Velocity; } set { vertices[0].Velocity = value; } }

        public Rope(Vector3 position, int verticesCount, float verticesMass, float ropeTotalLength, float springConstant, float springFrictionConstant, Vector3 gravitation, float airFrictionConstant)
        {
            float springLength = ropeTotalLength / Math.Max(verticesCount - 1, 1);

            this.gravitation = gravitation;
            this.airFrictionConstant = airFrictionConstant;

            vertices = new RopeVertex[verticesCount];
            for (int i = 0; i < verticesCount; i++)
            {
                vertices[i] = new RopeVertex(verticesMass) { Position = new Vector3(position.X, position.Y, position.Z + (i * -springLength)) };
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
            }
        }

        public void Simulate(float deltaTime)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Simulate(deltaTime);
            }

            //Position += Velocity * deltaTime;

            //vertices[0].Pin(Position);
        }

        public void PinVertex(int index, Vector3 position)
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
