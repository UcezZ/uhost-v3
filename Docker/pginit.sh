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

# Обновим конфигурацию
CONF_FILE=`psql --dbname $POSTGRES_DB --username $POSTGRES_USER -AXqtc "SHOW config_file;"`
echo "pg_trgm.similarity_threshold = 0.3" >> $CONF_FILE
echo "pg_trgm.word_similarity_threshold = 0.3" >> $CONF_FILE

# Создание БД Sentry
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "CREATE DATABASE $POSTGRES_DB_SENTRY;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "ALTER DATABASE $POSTGRES_DB_SENTRY OWNER TO $POSTGRES_USER;"

# Создание БД логов
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "CREATE DATABASE $POSTGRES_DB_LOGS;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "ALTER DATABASE $POSTGRES_DB_LOGS OWNER TO $POSTGRES_USER;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "CREATE SCHEMA IF NOT EXISTS $POSTGRES_DB_SCHEMA;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "ALTER SCHEMA $POSTGRES_DB_SCHEMA OWNER TO $POSTGRES_USER;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "ALTER DATABASE $POSTGRES_DB_LOGS SET SEARCH_PATH TO $POSTGRES_DB_SCHEMA, public;"
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_LOGS" -c "SET SEARCH_PATH TO $POSTGRES_DB_SCHEMA, public;"
