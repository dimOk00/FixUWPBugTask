using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GhostCore.Caching
{
    public class SelfCleaningObjectCache
    {
        #region Singleton

        private static volatile SelfCleaningObjectCache _instance;
        private static object _syncRoot = new object();


        public static SelfCleaningObjectCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new SelfCleaningObjectCache();
                    }
                }

                return _instance;
            }
        }
        private SelfCleaningObjectCache()
        {
            Initialize();
        }

        #endregion

        private Dictionary<string, object> _objectPool;
        private Dictionary<object, int> _accessCount;
        private SelfCleaningCacheConfiguration _configuration;
        private bool _isStarted;


        public SelfCleaningCacheConfiguration Configuration
        {
            get { return _configuration; }
            set
            {
                if (_isStarted)
                    throw new Exception("Cannot set Configuration after the cache is started.");

                _configuration = value;
            }
        }


        private void Initialize()
        {
            _objectPool = new Dictionary<string, object>();
            _accessCount = new Dictionary<object, int>();

            Configuration = new SelfCleaningCacheConfiguration()
            {
                CleanupPeriod = 60,
                CleanupThreshold = 2
            }; // DEFAULTS
        }

        public void Start()
        {
            _isStarted = true;
        }
    }

    public class SelfCleaningCacheConfiguration
    {
        /// <summary>
        /// Period used by the cache to clean up objects under the specified CleanupThreshold in seconds
        /// </summary>
        public double CleanupPeriod { get; set; }

        /// <summary>
        /// When the cleanup ocurs, if an objects is under this specified number, it will be removed from the pool and later gc'ed
        /// </summary>
        public int CleanupThreshold { get; set; }
    }
}
