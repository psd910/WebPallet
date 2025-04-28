using NHibernate.Criterion;
using NHibernate;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CoreWebTinhTien.NHibernateSession;
using NHibernate.Linq;

namespace CoreWebTinhTien.BaseServices
{
    public class BaseService<T, IdT> : IBaseService<T, IdT>
    {
        private System.Type persitentType = typeof(T);

        protected readonly string SessionFactoryConfigPath;

        private int _fromIndex = -1;

        private int _MaxResult = 0;

        public bool isStateFull
        {
            get;
            set;
        }

        protected ISession NHibernateSession
        {
            get
            {
                return NHibernateSessionManager.Instance.GetSessionFrom(this.SessionFactoryConfigPath);
            }
        }

        protected IStatelessSession NHibernateSessionStateLess
        {
            get
            {
                return NHibernateSessionManager.Instance.GetSessionStateLessFrom(this.SessionFactoryConfigPath);
            }
        }

        public System.Linq.IQueryable<T> Query
        {
            get
            {
                System.Linq.IQueryable<T> result;
                if (!this.isStateFull)
                {
                    result = this.NHibernateSession.Query<T>();
                }
                else
                {
                    result = this.NHibernateSessionStateLess.Query<T>();
                }
                return result;
            }
        }

        public BaseService(string sessionFactoryConfigPath)
        {
            Check.Require(!string.IsNullOrEmpty(sessionFactoryConfigPath), "sessionFactoryConfigPath may not be null nor empty");
            if (!sessionFactoryConfigPath.Contains(System.AppDomain.CurrentDomain.BaseDirectory))
            {
                this.SessionFactoryConfigPath = System.AppDomain.CurrentDomain.BaseDirectory + sessionFactoryConfigPath;
            }
            else
            {
                this.SessionFactoryConfigPath = sessionFactoryConfigPath;
            }
        }

        public virtual T CreateNew(T entity)
        {
            if (!this.isStateFull)
            {
                this.NHibernateSession.Save(entity);
            }
            else
            {
                this.NHibernateSessionStateLess.Insert(entity);
            }
            return entity;
        }

        public virtual T Delete(T entity)
        {
            if (!this.isStateFull)
            {
                this.NHibernateSession.Delete(entity);
            }
            else
            {
                this.NHibernateSessionStateLess.Delete(entity);
            }
            return entity;
        }

        public virtual T Delete(IdT key)
        {
            T entity = this.Getbykey(key);
            return this.Delete(entity);
        }

        public virtual T Update(T entity)
        {
            if (!this.isStateFull)
            {
                this.NHibernateSession.Update(entity);
            }
            else
            {
                this.NHibernateSessionStateLess.Update(entity);
            }
            return entity;
        }

        public virtual T Save(T entity)
        {
            if (!this.isStateFull)
            {
                this.NHibernateSession.SaveOrUpdate(entity);
            }
            else
            {
                this.NHibernateSessionStateLess.Insert(entity);
            }
            return entity;
        }

        public virtual R ExecuteScalar<R>(string Query, bool isHQL, params SQLParam[] _params)
        {
            IQuery query = isHQL ? this.NHibernateSession.CreateQuery(Query) : this.NHibernateSession.CreateSQLQuery(Query);
            for (int i = 0; i < _params.Length; i++)
            {
                SQLParam sQLParam = _params[i];
                query.SetParameter(sQLParam.ParameName, sQLParam.ParamValue);
            }
            return query.List<R>().Single<R>();
        }

        public virtual T Getbykey(IdT key)
        {
            object obj;
            if (!this.isStateFull)
            {
                obj = this.NHibernateSession.Get(this.persitentType, key);
            }
            else
            {
                obj = this.NHibernateSessionStateLess.Get<T>(key);
            }
            return (obj == null) ? default(T) : ((T)((object)obj));
        }

        public virtual List<T> GetByCriteria(params ICriterion[] criterion)
        {
            ICriteria criteria;
            if (!this.isStateFull)
            {
                criteria = this.NHibernateSession.CreateCriteria(this.persitentType);
            }
            else
            {
                criteria = this.NHibernateSessionStateLess.CreateCriteria(this.persitentType);
            }
            for (int i = 0; i < criterion.Length; i++)
            {
                ICriterion expression = criterion[i];
                criteria.Add(expression);
            }
            return this.GetByCriteria(criteria);
        }

        public virtual List<T> GetByCriteria(ICriteria _crit)
        {
            List<T> result;
            if (this._fromIndex >= 0 && this._MaxResult > 0)
            {
                _crit.SetFirstResult(this._fromIndex);
                _crit.SetMaxResults(this._MaxResult);
                List<T> list = _crit.List<T>() as List<T>;
                this.ResetFetchPage();
                result = list;
            }
            else
            {
                result = (_crit.List<T>() as List<T>);
            }
            return result;
        }

        public virtual ICriteria CreateCriteria()
        {
            return this.NHibernateSession.CreateCriteria(this.persitentType);
        }

        public virtual List<T> GetAll()
        {
            return this.GetByCriteria(new ICriterion[0]);
        }

        public IList<T> GetAll(int pageIndex, int pageSize, out int total)
        {
            IList<T> result = this.Query.Skip(pageIndex * pageSize).Take(pageSize).ToList<T>();
            total = this.Query.Count<T>();
            return result;
        }

        public List<T> GetbySQLQuery(string Query, params SQLParam[] _params)
        {
            IQuery query = this.NHibernateSession.CreateSQLQuery(Query).AddEntity(typeof(T));
            for (int i = 0; i < _params.Length; i++)
            {
                SQLParam sQLParam = _params[i];
                query.SetParameter(sQLParam.ParameName, sQLParam.ParamValue);
            }
            List<T> result;
            if (this._fromIndex >= 0 && this._MaxResult > 0)
            {
                query.SetFirstResult(this._fromIndex);
                query.SetMaxResults(this._MaxResult);
                List<T> list = query.List<T>() as List<T>;
                this.ResetFetchPage();
                result = list;
            }
            else
            {
                result = (query.List<T>() as List<T>);
            }
            return result;
        }

        public IList<T> GetbySQLQuery(string Query, int pageIndex, int pageSize, out int total, params SQLParam[] _params)
        {
            string str = Query;
            int num = Query.ToLower().IndexOf("order by", System.StringComparison.OrdinalIgnoreCase);
            if (num >= 0)
            {
                str = Query.Substring(0, num);
            }
            string query = "select count(*) from (" + str + ") as ccc";
            total = this.ExecuteCountQuery(query, false, _params);
            IQuery query2 = this.NHibernateSession.CreateSQLQuery(Query).AddEntity(typeof(T));
            for (int i = 0; i < _params.Length; i++)
            {
                SQLParam sQLParam = _params[i];
                query2.SetParameter(sQLParam.ParameName, sQLParam.ParamValue);
            }
            query2.SetFirstResult(pageIndex * pageSize);
            query2.SetMaxResults(pageSize);
            return query2.List<T>();
        }

        public int ExecuteCountQuery(string Query, bool isHQL, params SQLParam[] _params)
        {
            IQuery query = isHQL ? this.NHibernateSession.CreateQuery(Query) : this.NHibernateSession.CreateSQLQuery(Query);
            for (int i = 0; i < _params.Length; i++)
            {
                SQLParam sQLParam = _params[i];
                query.SetParameter(sQLParam.ParameName, sQLParam.ParamValue);
            }
            object value = query.UniqueResult();
            return System.Convert.ToInt32(value);
        }

        public List<T> GetbyHQuery(string query, int pageIndex, int pageSize, out int total, params SQLParam[] _params)
        {
            string text = query;
            int num = query.ToLower().IndexOf("order by", System.StringComparison.OrdinalIgnoreCase);
            if (num >= 0)
            {
                text = query.Substring(0, num);
            }
            int num2 = text.ToLower().IndexOf("from", System.StringComparison.OrdinalIgnoreCase);
            if (num2 >= 0)
            {
                text = text.Substring(num2);
            }
            string query2 = "select count(*) " + text;
            total = this.ExecuteCountQuery(query2, true, _params);
            IQuery query3 = this.NHibernateSession.CreateQuery(query);
            for (int i = 0; i < _params.Length; i++)
            {
                SQLParam sQLParam = _params[i];
                query3.SetParameter(sQLParam.ParameName, sQLParam.ParamValue);
            }
            query3.SetFirstResult(pageIndex * pageSize);
            query3.SetMaxResults(pageSize);
            return query3.List<T>() as List<T>;
        }

        public virtual T Get(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate)
        {
            
            List<T> list = this.Query.Where(predicate).ToList<T>();
            return (list.Count > 0) ? list[0] : default(T);

        }

        public List<T> GetbyHQuery(string Query, params SQLParam[] _params)
        {
            IQuery query = this.NHibernateSession.CreateQuery(Query);
            for (int i = 0; i < _params.Length; i++)
            {
                SQLParam sQLParam = _params[i];
                query.SetParameter(sQLParam.ParameName, sQLParam.ParamValue);
            }
            List<T> result;
            if (this._fromIndex >= 0 && this._MaxResult > 0)
            {
                query.SetFirstResult(this._fromIndex);
                query.SetMaxResults(this._MaxResult);
                List<T> list = query.List<T>() as List<T>;
                this.ResetFetchPage();
                result = list;
            }
            else
            {
                result = (query.List<T>() as List<T>);
            }
            return result;
        }

        public virtual object ExcuteNonQuery(string SQLquery)
        {
            IDbCommand dbCommand = this.NHibernateSession.Connection.CreateCommand();
            dbCommand.CommandText = SQLquery;
            dbCommand.CommandType = CommandType.Text;
            return dbCommand.ExecuteNonQuery();
        }

        public object ExcuteNonQuery(string query, bool isHQL, params SQLParam[] _params)
        {
            IQuery query2 = isHQL ? this.NHibernateSession.CreateQuery(query) : this.NHibernateSession.CreateSQLQuery(query);
            for (int i = 0; i < _params.Length; i++)
            {
                SQLParam sQLParam = _params[i];
                query2.SetParameter(sQLParam.ParameName, sQLParam.ParamValue);
            }
            int num = query2.ExecuteUpdate();
            return num;
        }

        public void CommitChanges()
        {
            NHibernateSessionManager.Instance.GetSessionFrom(this.SessionFactoryConfigPath).Flush();
        }

        public void Clear()
        {
            this.NHibernateSession.Clear();
        }

        public void Flush()
        {
            this.NHibernateSession.Flush();
        }

        public void BindSession(object _obj)
        {
            if (_obj is ICollection<T>)
            {
                foreach (T current in ((ICollection<T>)_obj))
                {
                    this.NHibernateSession.Persist(current);
                }
            }
            else
            {
                this.NHibernateSession.Persist(_obj);
            }
        }

        public void UnbindSession(object _obj)
        {
            if (_obj is ICollection<T>)
            {
                foreach (T current in ((ICollection<T>)_obj))
                {
                    this.NHibernateSession.Evict(current);
                }
            }
            else
            {
                this.NHibernateSession.Evict(_obj);
            }
        }

        public void BeginTran()
        {
            if (!this.isStateFull)
            {
                NHibernateSessionManager.Instance.BeginTransactionOn(this.SessionFactoryConfigPath);
            }
            else
            {
                NHibernateSessionManager.Instance.BeginTransactionStateLessOn(this.SessionFactoryConfigPath);
            }
        }

        public void CommitTran()
        {
            if (!this.isStateFull)
            {
                NHibernateSessionManager.Instance.CommitTransactionOn(this.SessionFactoryConfigPath);
            }
            else
            {
                NHibernateSessionManager.Instance.CommitTransactionStateLessOn(this.SessionFactoryConfigPath);
            }
        }

        public void RolbackTran()
        {
            if (!this.isStateFull)
            {
                NHibernateSessionManager.Instance.RollbackTransactionOn(this.SessionFactoryConfigPath);
            }
            else
            {
                NHibernateSessionManager.Instance.RollbackTransactionStateLessOn(this.SessionFactoryConfigPath);
            }
        }

        public void RemoveCollectionFromCache(string roleName)
        {
            this.NHibernateSession.SessionFactory.EvictCollection(roleName);
        }

        public void RemoveCollectionFromCache(string roleName, int id)
        {
            this.NHibernateSession.SessionFactory.EvictCollection(roleName, id);
        }

        public void RemoveQueryFromCache(string cacheRegion)
        {
            this.NHibernateSession.SessionFactory.EvictQueries(cacheRegion);
        }

        public void SetFetchPage(int from, int maxResult)
        {
            this._fromIndex = from;
            this._MaxResult = maxResult;
        }

        public void ResetFetchPage()
        {
            this._fromIndex = -1;
            this._MaxResult = 0;
        }
    }
}
