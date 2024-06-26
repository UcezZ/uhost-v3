#!/bin/bash
# KEEP THIS FILE LF-LINEBREAKED!!!

set -e

# Настройка основной БД
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "ALTER DATABASE $POSTGRES_DB OWNER TO $POSTGRES_USER;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "CREATE SCHEMA IF NOT EXISTS $POSTGRES_DB_SCHEMA;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "ALTER SCHEMA $POSTGRES_DB_SCHEMA OWNER TO $POSTGRES_USER;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "ALTER DATABASE $POSTGRES_DB SET SEARCH_PATH TO $POSTGRES_DB_SCHEMA, public;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "SET SEARCH_PATH TO $POSTGRES_DB_SCHEMA, public;"

# Добавление расширений
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "CREATE EXTENSION IF NOT EXISTS pg_trgm;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "CREATE EXTENSION IF NOT EXISTS btree_gin;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "SELECT * FROM pg_extension;"

# tgrm
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "ALTER SYSTEM SET pg_trgm.similarity_threshold = 0.1;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "ALTER SYSTEM SET pg_trgm.word_similarity_threshold = 0.1;"

# TZ
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "ALTER SYSTEM SET timezone = '+03:00';"

# Создание БД логов
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "CREATE DATABASE $POSTGRES_DB_LOGS;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "ALTER DATABASE $POSTGRES_DB_LOGS OWNER TO $POSTGRES_USER;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "CREATE SCHEMA IF NOT EXISTS $POSTGRES_DB_SCHEMA;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "ALTER SCHEMA $POSTGRES_DB_SCHEMA OWNER TO $POSTGRES_USER;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "ALTER DATABASE $POSTGRES_DB_LOGS SET SEARCH_PATH TO $POSTGRES_DB_SCHEMA, public;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "SET SEARCH_PATH TO $POSTGRES_DB_SCHEMA, public;"

# TZ
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "ALTER SYSTEM SET timezone = '+03:00';"
