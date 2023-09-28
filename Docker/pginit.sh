#!/bin/bash

set -e

# Настройка основной БД
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -c "ALTER DATABASE $POSTGRES_DB OWNER TO $POSTGRES_USER"
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
# psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_SENTRY" -c "CREATE SCHEMA IF NOT EXISTS $POSTGRES_DB_SCHEMA;"
# psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_SENTRY" -c "ALTER SCHEMA $POSTGRES_DB_SCHEMA OWNER TO $POSTGRES_USER;"
# psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_SENTRY" -c "ALTER DATABASE $POSTGRES_DB_SENTRY SET SEARCH_PATH TO $POSTGRES_DB_SCHEMA, public;"
# psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB_SENTRY" -c "SET SEARCH_PATH TO $POSTGRES_DB_SCHEMA, public;"

