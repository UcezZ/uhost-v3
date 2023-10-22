using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        /// <param name="args">Параметры запроса</param>
        /// <returns>true если удачно</returns>
        public static bool TryExecuteSqlRaw<T>(this DatabaseFacade facade, string sql, out T value, params NpgsqlParameter[] args)
        {
            value = facade.FromSqlRaw<T>(sql, args).FirstOrDefault();

            return value != null;
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
        /// <param name="args">Параметры запроса</param>
        /// <returns>true если удачно</returns>
        public static bool TryExecuteSqlRaw<T1, T2>(this DatabaseFacade facade, string sql, out T1 value1, out T2 value2, params NpgsqlParameter[] args)
        {
            var result = facade.FromSqlRaw<T1, T2>(sql, args).FirstOrDefault();

            try
            {
                value1 = result.Item1;
                value2 = result.Item2;

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

        /// <summary>
        /// Добавление множества элементов в коллекцию
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        public static void AddRange<TItem, TCollection>(this TCollection collection, IEnumerable<TItem> items) where TCollection : IList, IDataParameterCollection
        {
            items.ForEach(item => collection.Add(item));
        }

        /// <summary>
        /// Получить колонку типа <typeparamref name="T"/> по запросу
        /// </summary>
        /// <typeparam name="T">Тип колонки</typeparam>
        /// <param name="facade">Фасад БД</param>
        /// <param name="sql">Запрос</param>
        /// <param name="args">Параметры запроса</param>
        /// <returns></returns>
        public static IEnumerable<T> FromSqlRaw<T>(this DatabaseFacade facade, string sql, params NpgsqlParameter[] args)
        {
            var conn = facade.GetDbConnection();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;

                if (args != null && args.Any())
                {
                    cmd.Parameters.AddRange(args);
                }

                cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        yield return (T)reader.GetValue(0);
                    }
                }
            }
        }

        /// <summary>
        /// Получить колонки типа <typeparamref name="T1"/>, <typeparamref name="T2"/> по запросу
        /// </summary>
        /// <typeparam name="T1">Тип колонки 1</typeparam>
        /// <typeparam name="T2">Тип колонки 2</typeparam>
        /// <param name="facade">Фасад БД</param>
        /// <param name="sql">Запрос</param>
        /// <param name="args">Параметры запроса</param>
        /// <returns></returns>
        public static IEnumerable<Tuple<T1, T2>> FromSqlRaw<T1, T2>(this DatabaseFacade facade, string sql, params NpgsqlParameter[] args)
        {
            var conn = facade.GetDbConnection();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                if (args != null && args.Any())
                {
                    cmd.Parameters.AddRange(args);
                }

                cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        yield return new Tuple<T1, T2>((T1)reader.GetValue(0), (T2)reader.GetValue(1));
                    }
                }
            }
        }

        /// <summary>
        /// Получить колонки типа <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/> по запросу
        /// </summary>
        /// <typeparam name="T1">Тип колонки 1</typeparam>
        /// <typeparam name="T2">Тип колонки 2</typeparam>
        /// <typeparam name="T3">Тип колонки 3</typeparam>
        /// <param name="facade">Фасад БД</param>
        /// <param name="sql">Запрос</param>
        /// <param name="args">Параметры запроса</param>
        /// <returns></returns>
        public static IEnumerable<Tuple<T1, T2, T3>> FromSqlRaw<T1, T2, T3>(this DatabaseFacade facade, string sql, params NpgsqlParameter[] args)
        {
            var conn = facade.GetDbConnection();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                if (args != null && args.Any())
                {
                    cmd.Parameters.AddRange(args);
                }

                cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        yield return new Tuple<T1, T2, T3>((T1)reader.GetValue(0), (T2)reader.GetValue(1), (T3)reader.GetValue(2));
                    }
                }
            }
        }
    }
}
