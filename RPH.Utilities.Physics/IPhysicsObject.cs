namespace RPH.Utilities.Physics
{
    public interface IPhysicsObject
    {
        void Init();
        void Solve();
        void Simulate(float deltaTime);
        void DebugDraw();
    }
}
