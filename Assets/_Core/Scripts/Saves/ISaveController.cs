namespace _Core.Saves
{
    public interface ISaveController
    {
        public void Save<T>(string key, T value);
        public T Load<T>(string key);
        public void Delete<T>(string key);
    }
}