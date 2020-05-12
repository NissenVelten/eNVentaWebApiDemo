namespace NVShop.Data.FS
{
    using System;
    using System.Collections.Concurrent;

    using FrameworkSystems.FrameworkBase.GlobalObj;

    internal class FSGlobalPool : IDisposable
    {
        
        private readonly ConcurrentBag<IGlobalObjects> _globals = new ConcurrentBag<IGlobalObjects>();
        private readonly Func<IGlobalObjects> _factory;
        private readonly int _poolSize;

        public FSGlobalPool(Func<IGlobalObjects> factory, int poolSize)
        {
            _factory = factory;
            _poolSize = poolSize;
        }

        public IGlobalObjects GetGlobal()
        {
            return _globals.TryTake(out var global) ? global : _factory();
        }

        /// <summary>
        ///     Puts the global.
        /// </summary>
        /// <param name="global">The global.</param>
        public void PutGlobal(IGlobalObjects global)
        {
            if (_globals.Count < _poolSize)
            {
                _globals.Add(global);
            }
            else
            {
                ((GlobalObjects) global).Dispose();
            }
        }
        
        public void Dispose()
        {
            while (_globals.TryTake(out var global))
            {
                ((GlobalObjects)global).Dispose();
            }
        }
    }
}