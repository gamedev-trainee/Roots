namespace Roots
{
    public interface ITimeLoopListener
    {
        void update(float deltaTime);
        void lateUpdate(float deltaTime);
    }
}
