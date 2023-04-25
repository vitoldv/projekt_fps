namespace _Core
{
    public interface IPoolableObject
    {
        void Enable();
        void Disable();
        bool IsActive();
    }
}