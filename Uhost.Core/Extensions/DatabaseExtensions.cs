using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Data;

namespace Uhost.Core.Extensions
{
    public static class DatabaseExtensions
    {
        private const string _dbSizeSql = "SELECT pg_database_size(current_database())::INT";

        /// <summary>
        /// Выполняет SQL запрос, возвращает все строки в <see cref="DataTable"/>
        /// </summary>
        /// <param name="connection">Соединение</param>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public static DataTable ExecuteSqlDataTable(this IDbConnection connection, string sql)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var table = new DataTable();

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    table.Load(reader);
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }

                return table;
            }
        }

        /// <summary>
        /// Выполняет SQL запрос, возвращает первую строку в массиве объектов
        /// </summary>
        /// <param name="connection">Соединение</param>
        /// <param name="sql">SQL</param>
        /// <returns></returns>
        public static object[] ExecuteSqlFirstRow(this IDbConnection connection, string sql)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var row = Array.Empty<object>();

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader.Read())
                    {
                        row = new object[reader.FieldCount];
                        reader.GetValues(row);
                    }
                }

                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }

                return row;
            }
        }

        /// <summary>
        /// Выполняет SQL запрос, возвращает объект типа <typeparamref name="T"/> из первой строки
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="facade">Фасад БД</param>
        /// <param name="sql">SQL</param>
        /// <param name="value">Полученное значение</param>
        /// <returns>true если удачно</returns>
        public static bool TryExecuteSqlRaw<T>(this DatabaseFacade facade, string sql, out T value)
        {
            var conn = facade.GetDbConnection();

            var row = conn.ExecuteSqlFirstRow(sql);

            try
            {
                value = (T)row[0];

                return true;
            }
            catch
            {

                value = default;

                return false;
            }
        }

        /// <summary>
        /// Выполняет SQL запрос, возвращает объекты типа <typeparamref name="T1"/>, <typeparamref name="T2"/> из первой строки
        /// </summary>
        /// <typeparam name="T1">Тип объекта 1</typeparam>
        /// <typeparam name="T2">Тип объекта 1</typeparam>
        /// <param name="facade">Фасад БД</param>
        /// <param name="sql">SQL</param>
        /// <param name="value1">Полученное значение из первого столбца</param>
        /// <param name="value2">Полученное значение из второго столбца</param>
        /// <returns>true если удачно</returns>
        public static bool TryExecuteSqlRaw<T1, T2>(this DatabaseFacade facade, string sql, out T1 value1, out T2 value2)
        {
            var conn = facade.GetDbConnection();

            var row = conn.ExecuteSqlFirstRow(sql);

            try
            {
                value1 = (T1)row[0];
                value2 = (T2)row[1];

                return true;
            }
            catch
            {

                value1 = default;
                value2 = default;

                return false;
            }
        }

        /// <summary>
        /// Получает размер БД в байтах
        /// </summary>
        /// <param name="facade">Фасад БД</param>
        /// <returns></returns>
        public static int GetSize(this DatabaseFacade facade)
        {
            if (facade.TryExecuteSqlRaw<int>(_dbSizeSql, out var size))
            {
                return size;
            }
            else
            {
                return -1;
            }
        }
    }
}
