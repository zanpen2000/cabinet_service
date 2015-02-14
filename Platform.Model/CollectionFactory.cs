using System.Collections.Concurrent;

namespace Platform.Model
{
    public class CollectionFactory
    {
        private static readonly object SyncLock = new object();
        private static CollectionFactory _instance;

        public static CollectionFactory Default
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CollectionFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        private CollectionFactory() { }

        public void CreateCollection<T>(string collectionName)
        {
            _concurrentDictionary.TryAdd(collectionName, new ItemCollection<T>(collectionName));
        }

        private readonly ConcurrentDictionary<string, object> _concurrentDictionary = new ConcurrentDictionary<string, object>();

    }
}
