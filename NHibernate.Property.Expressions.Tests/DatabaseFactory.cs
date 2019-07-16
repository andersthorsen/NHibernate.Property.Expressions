using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Property.Expressions.Tests
{
    public class DatabaseFactory : IDisposable
    {
        private SQLiteConnection _connection;
        private ISessionFactory _sessionFactory;
        private Configuration _configuration;

        public DbConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SQLiteConnection("Data Source=:memory:");

                _connection.Open();

                var exporter = new SchemaExport(CreateConfiguration());

                exporter.Create(ExecuteSql, false);

            }

            return _connection;
        }

        public void ExecuteSql(string sql)
        {
            var cn = GetConnection();

            var cmd = cn.CreateCommand();

            cmd.CommandText = sql;

            cmd.ExecuteNonQuery();
        }

        public ISessionFactory GetSessionFactory()
        {
            if (_sessionFactory != null)
                return _sessionFactory;

            var cfg = CreateConfiguration();

            _sessionFactory = cfg.BuildSessionFactory();

            return _sessionFactory;
        }

        public ISession OpenSession()
        {
            var sessionFactory = GetSessionFactory();

            return sessionFactory.OpenSession(GetConnection());
        }

        public Configuration CreateConfiguration()
        {
            if (_configuration != null)
                return _configuration;
            
            var mapper = new ModelMapper();
            var cfg = new Configuration();

            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            /*
            cfg.Properties.Add(Environment.QueryLinqProvider, typeof(DefaultQueryProvider).FullName);
            */
            cfg.DataBaseIntegration(c =>
            {
                c.ConnectionString = @"Data Source=:memory:";
                c.Driver<SQLite20Driver>();
                c.Dialect<SQLiteDialect>();

                c.LogSqlInConsole = true;
                c.LogFormattedSql = true;
                c.AutoCommentSql = true;
            });

            cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            _configuration = cfg;
            return _configuration;
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();

            _connection = null;
        }
    }
}
