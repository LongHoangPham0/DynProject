using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;

namespace BlueMoon.Session.Providers
{
    public class NoLockStateServerStore : NoLockSessionStore
    {
        public NoLockStateServerStore() : base("System.Web.SessionState.OutOfProcSessionStateStore") { }
    }
    public class NoLockInProcStore : NoLockSessionStore
    {
        public NoLockInProcStore() : base("System.Web.SessionState.InProcSessionStateStore") { }
    }
    public class NoLockSqlStore : NoLockSessionStore
    {
        public NoLockSqlStore() : base("System.Web.SessionState.SqlSessionStateStore") { }
    }
    public class NoLockRedisStore : NoLockSessionStore
    {
        public NoLockRedisStore() : base("Microsoft.Web.Redis.RedisSessionStateProvider, Microsoft.Web.RedisSessionStateProvider") { }
    }
    public abstract class NoLockSessionStore : SessionStateStoreProviderBase
    {
        SessionStateStoreProviderBase _storage = null;
        string _storageType = null;
        protected NoLockSessionStore(string type)
        {
            _storageType = type;
        }
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {           
            base.Initialize(name, config);
            Type type = typeof(SessionStateStoreProviderBase).Assembly.GetType(_storageType);
            if(type==null) type = Type.GetType(_storageType);
            
            _storage = (SessionStateStoreProviderBase)Activator.CreateInstance(type);
            _storage.Initialize(name, config);
        }

        public override void Dispose()
        {
            _storage.Dispose();
        }

        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            SessionStateStoreData result = _storage.GetItem(context, id, out locked, out lockAge, out lockId, out actions);
            if (lockId != null && CheckPageHaveNoLock(context))
            {
                _storage.ReleaseItemExclusive(context, id, lockId);
                if (result == null)
                {
                    
                    result = _storage.GetItem(context, id, out locked, out lockAge, out lockId, out actions);
                }
                //locked = false;
            }
            return result;
        }

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            SessionStateStoreData result = _storage.GetItemExclusive(context, id, out locked, out lockAge, out lockId, out actions);
            if (lockId != null && CheckPageHaveNoLock(context))
            {
                if(result != null)
                {
                    _storage.SetAndReleaseItemExclusive(context, id, result, lockId, true);
                }
                //_storage.ReleaseItemExclusive(context, id, lockId);
                if (result == null)
                {
                    
                    //result = _storage.GetItemExclusive(context, id, out locked, out lockAge, out lockId, out actions);
                }
                //locked = false;
            }
            return result;
        }

        #region  Unchanged methods
        
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return _storage.CreateNewStoreData(context, timeout);
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            _storage.CreateUninitializedItem(context, id, timeout);
        }


        public override void EndRequest(HttpContext context)
        {
            _storage.EndRequest(context);
        }
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            _storage.RemoveItem(context, id, lockId, item);
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            _storage.ResetItemTimeout(context, id);
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            _storage.SetAndReleaseItemExclusive(context, id, item, lockId, newItem);
        }
        public override void InitializeRequest(HttpContext context)
        {
            _storage.InitializeRequest(context);
        }
        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            _storage.ReleaseItemExclusive(context, id, lockId);
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return _storage.SetItemExpireCallback(expireCallback);
        }
        #endregion

        private bool CheckPageHaveNoLock(HttpContext context)
        {
            return true;
            string filePath = context.Server.MapPath("~/App_Data/NoLockSessionPages.txt");
            List<string> pages = null;
            if (HttpRuntime.Cache[filePath] == null)
            {

                if (System.IO.File.Exists(filePath))
                {
                    pages = new List<string>();
                    string[] pageList = System.IO.File.ReadAllLines(filePath);
                    for (int i = 0; i < pageList.Length; i++)
                    {
                        pageList[i] = pageList[i].Trim().ToLower();
                        if (!string.IsNullOrEmpty(pageList[i])) pages.Add(pageList[i]);
                    }
                    HttpRuntime.Cache.Add(filePath, pages, new CacheDependency(filePath), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
            }
            else
            {
                pages = HttpRuntime.Cache[filePath] as List<string>;
            }
            if (pages != null) return pages.Contains(context.Request.Url.Segments.Last().ToLower());
            else return false;
        }
    }
}
