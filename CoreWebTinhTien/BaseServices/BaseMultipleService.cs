using NHibernate.Criterion;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using CoreWebTinhTien.NHibernateSession;

namespace CoreWebTinhTien.BaseServices
{
    /// <summary>
    /// class ghi đè các phương thức của BaseService và ép kiểu từ Interface sang kiểu dữ liệu thực tế của service đó là T, interface là I
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="IdT"></typeparam>
    public class BaseMultipleService<T, I, IdT> : BaseService<T, IdT> where T : I
    {
        public BaseMultipleService(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
        {
        }

        public System.Linq.IQueryable<I> Query
        {
            get
            {
                return (IQueryable<I>)base.Query;
            }
        }

        public virtual I CreateNew(I entity)
        {
            return base.CreateNew((T)entity);
        }

        public virtual I Delete(I entity)
        {
            return base.Delete((T)entity);
        }

        public virtual I Delete(IdT key)
        {
            return base.Delete(key);
        }

        public virtual I Update(I entity)
        {
            return base.Update((T)entity);
        }

        public virtual I Save(I entity)
        {
            return base.Save((T)entity);
        }

        public virtual R ExecuteScalar<R>(string Query, bool isHQL, params SQLParam[] _params)
        {
            return base.ExecuteScalar<R>(Query, isHQL, _params);
        }

        public virtual I Getbykey(IdT key)
        {
            return base.Getbykey(key);
        }

        public virtual List<I> GetByCriteria(params ICriterion[] criterion)
        {
            return base.GetByCriteria(criterion).ConvertAll<I>(t=> (I)t);
        }

        public virtual List<I> GetByCriteria(ICriteria _crit)
        {
            return base.GetByCriteria(_crit).ConvertAll<I>(t=> (I)t);   
        }

        public virtual ICriteria CreateCriteria()
        {
            return base.CreateCriteria();
        }

        public virtual List<I> GetAll()
        {
            return base.GetAll().ConvertAll<I>(t => (I)t);
        }

        public IList<I> GetAll(int pageIndex, int pageSize, out int total)
        {
            return base.GetAll(pageIndex, pageSize,out total).OfType<I>().ToList();
        }

        public List<I> GetbySQLQuery(string Query, params SQLParam[] _params)
        {
            return base.GetbySQLQuery(Query, _params).ConvertAll<I>(t => (I)t);
        }

        public IList<I> GetbySQLQuery(string Query, int pageIndex, int pageSize, out int total, params SQLParam[] _params)
        {
            return base.GetbySQLQuery(Query, pageIndex, pageSize, out total, _params).OfType<I>().ToList();
        }

        public int ExecuteCountQuery(string Query, bool isHQL, params SQLParam[] _params)
        {
            return base.ExecuteCountQuery(Query, isHQL, _params);
        }

        public List<I> GetbyHQuery(string query, int pageIndex, int pageSize, out int total, params SQLParam[] _params)
        {
            return base.GetbyHQuery(query, pageIndex, pageSize, out total, _params).ConvertAll<I>(t=>(I)t);
        }

        public virtual I Get(System.Linq.Expressions.Expression<System.Func<I, bool>> predicate)
        {
            var result = Query.FirstOrDefault(predicate);
            return result;
        }

        public List<I> GetbyHQuery(string Query, params SQLParam[] _params)
        {
            return base.GetbyHQuery(Query, _params).ConvertAll<I>(t=> (I)t);
        }
    }
}
