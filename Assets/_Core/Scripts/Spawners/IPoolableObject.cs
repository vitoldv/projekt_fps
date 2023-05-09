namespace _Core.Spawners
{
    public interface IPoolableObject
    {
        void Enable();
        void Disable();
        bool IsActive();
    }
}