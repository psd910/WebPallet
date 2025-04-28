using NHibernate;
using NHibernate.Cache;
using NHibernate.Cfg;

namespace CoreWebTinhTien.NHibernateSession
{
    public sealed class NHibernateSessionManager
    {
        private class Nested
        {
            internal static readonly NHibernateSessionManager NHibernateSessionManager;

            static Nested()
            {
                NHibernateSessionManager.Nested.NHibernateSessionManager = new NHibernateSessionManager();
            }
        }

        private const string TRANSACTION_KEY = "CONTEXT_TRANSACTIONS";

        private const string SESSION_KEY = "CONTEXT_SESSIONS";

        private static NHibernateSessionManager _Instance = null;

        private System.Collections.Hashtable sessionFactories = new System.Collections.Hashtable();

        public static NHibernateSessionManager Instance
        {
            get
            {
                return NHibernateSessionManager.Nested.NHibernateSessionManager;
            }
        }

        private System.Collections.Hashtable ContextTransactions
        {
            get
            {
                System.Collections.Hashtable result;

                if (CallContext.GetData("CONTEXT_TRANSACTIONS") == null)
                {
                    CallContext.SetData("CONTEXT_TRANSACTIONS", new System.Collections.Hashtable());
                }
                result = (System.Collections.Hashtable)CallContext.GetData("CONTEXT_TRANSACTIONS");

                return result;
            }
        }

        private System.Collections.Hashtable ContextSessions
        {
            get
            {
                System.Collections.Hashtable result;

                if (CallContext.GetData("CONTEXT_SESSIONS") == null)
                {
                    CallContext.SetData("CONTEXT_SESSIONS", new System.Collections.Hashtable());
                }
                result = (System.Collections.Hashtable)CallContext.GetData("CONTEXT_SESSIONS");

                return result;
            }
        }

        private System.Collections.Hashtable ContextSessionsStateLess
        {
            get
            {
                System.Collections.Hashtable result;

                if (CallContext.GetData("hCONTEXT_SESSIONS") == null)
                {
                    CallContext.SetData("hCONTEXT_SESSIONS", new System.Collections.Hashtable());
                }
                result = (System.Collections.Hashtable)CallContext.GetData("hCONTEXT_SESSIONS");

                return result;
            }
        }

        public bool isStateFull
        {
            get
            {
                bool result;

                result = CallContext.GetData("state_CONTEXT_SESSIONS") != null && (bool)CallContext.GetData("state_CONTEXT_SESSIONS");

                return result;
            }
            set
            {

                CallContext.SetData("state_CONTEXT_SESSIONS", value);

            }
        }

        private NHibernateSessionManager()
        {
        }

        public ISessionFactory GetSessionFactoryFor(string sessionFactoryConfigPath)
        {
            Check.Require(!string.IsNullOrEmpty(sessionFactoryConfigPath), "sessionFactoryConfigPath may not be null nor empty");
            ISessionFactory sessionFactory = (ISessionFactory)this.sessionFactories[sessionFactoryConfigPath];
            if (sessionFactory == null)
            {
                Check.Require(System.IO.File.Exists(sessionFactoryConfigPath), "The config file at '" + sessionFactoryConfigPath + "' could not be found");
                Configuration configuration = new Configuration();
                configuration.Configure(sessionFactoryConfigPath);
                string property = configuration.GetProperty("connection.connection_string");
                configuration.SetProperty("connection.connection_string", property);

                sessionFactory = configuration.BuildSessionFactory();
                if (sessionFactory == null)
                {
                    throw new System.InvalidOperationException("cfg.BuildSessionFactory() returned null.");
                }
                this.sessionFactories.Add(sessionFactoryConfigPath, sessionFactory);
            }
            return sessionFactory;
        }

        public void RegisterInterceptorOn(string sessionFactoryConfigPath, IInterceptor interceptor)
        {
            ISession session = (ISession)this.ContextSessions[sessionFactoryConfigPath];
            if (session != null && session.IsOpen)
            {
                throw new CacheException("You cannot register an interceptor once a session has already been opened");
            }
            this.GetSessionFrom(sessionFactoryConfigPath, interceptor);
        }

        public ISession GetSessionFrom(string sessionFactoryConfigPath)
        {
            return this.GetSessionFrom(sessionFactoryConfigPath, null);
        }

        public IStatelessSession GetSessionStateLessFrom(string sessionFactoryConfigPath)
        {
            return this.GetSessionStateLessFrom(sessionFactoryConfigPath, null);
        }

        private ISession GetSessionFrom(string sessionFactoryConfigPath, IInterceptor interceptor)
        {
            ISession session = (ISession)this.ContextSessions[sessionFactoryConfigPath];
            if (session == null)
            {
                if (interceptor != null)
                {
                    session = this.GetSessionFactoryFor(sessionFactoryConfigPath).OpenSession(interceptor);
                }
                else
                {
                    session = this.GetSessionFactoryFor(sessionFactoryConfigPath).OpenSession();
                }
                this.ContextSessions[sessionFactoryConfigPath] = session;
            }
            Check.Ensure(session != null, "session was null");
            return session;
        }

        private IStatelessSession GetSessionStateLessFrom(string sessionFactoryConfigPath, IInterceptor interceptor)
        {
            IStatelessSession statelessSession = (IStatelessSession)this.ContextSessionsStateLess[sessionFactoryConfigPath];
            if (statelessSession == null)
            {
                statelessSession = this.GetSessionFactoryFor(sessionFactoryConfigPath).OpenStatelessSession();
                this.ContextSessionsStateLess[sessionFactoryConfigPath] = statelessSession;
            }
            Check.Ensure(statelessSession != null, "session was null");
            return statelessSession;
        }

        public void CloseSessionOn(string sessionFactoryConfigPath)
        {
            ISession session = (ISession)this.ContextSessions[sessionFactoryConfigPath];
            if (session != null && session.IsOpen)
            {
                try
                {
                    session.Flush();
                }
                catch
                {
                }
                session.Close();
            }
            this.ContextSessions.Remove(sessionFactoryConfigPath);
        }

        public void CloseSession(string sessionFactoryConfigPath)
        {
            ISession session = (ISession)this.ContextSessions[sessionFactoryConfigPath];
            if (session != null && session.IsOpen)
            {
                session.Close();
            }
            this.ContextSessions.Remove(sessionFactoryConfigPath);
        }

        public ITransaction BeginTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
            if (transaction == null)
            {
                transaction = this.GetSessionFrom(sessionFactoryConfigPath).BeginTransaction();
                this.ContextTransactions.Add(sessionFactoryConfigPath, transaction);
            }
            return transaction;
        }

        public ITransaction BeginTransactionStateLessOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
            if (transaction == null)
            {
                transaction = this.GetSessionStateLessFrom(sessionFactoryConfigPath).BeginTransaction();
                this.ContextTransactions.Add(sessionFactoryConfigPath, transaction);
            }
            return transaction;
        }

        public void CommitTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
            try
            {
                if (this.HasOpenTransactionOn(sessionFactoryConfigPath))
                {
                    transaction.Commit();
                    this.ContextTransactions.Remove(sessionFactoryConfigPath);
                }
            }
            catch (HibernateException)
            {
                this.RollbackTransactionOn(sessionFactoryConfigPath);
                throw;
            }
        }

        public void CommitTransactionStateLessOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
            try
            {
                if (this.HasOpenTransactionOn(sessionFactoryConfigPath))
                {
                    transaction.Commit();
                    this.ContextTransactions.Remove(sessionFactoryConfigPath);
                }
            }
            catch (HibernateException)
            {
                this.RollbackTransactionOn(sessionFactoryConfigPath);
                throw;
            }
        }

        public bool HasOpenTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        public void RollbackTransactionStateLessOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
            try
            {
                if (this.HasOpenTransactionOn(sessionFactoryConfigPath))
                {
                    transaction.Rollback();
                }
                this.ContextTransactions.Remove(sessionFactoryConfigPath);
            }
            finally
            {
                this.CloseSession(sessionFactoryConfigPath);
            }
        }

        public void RollbackTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction = (ITransaction)this.ContextTransactions[sessionFactoryConfigPath];
            try
            {
                if (this.HasOpenTransactionOn(sessionFactoryConfigPath))
                {
                    transaction.Rollback();
                }
                this.ContextTransactions.Remove(sessionFactoryConfigPath);
            }
            finally
            {
                this.CloseSession(sessionFactoryConfigPath);
            }
        }


    }
}
