# uHost v3
Новая версия проекта uHost https://ucezz.sytes.net/Projects/mirea/uHost/v2

### Запуск Sentry
1. _Нужно включить Compose v2 в настройках Docker_
1. Создать файл `/Docker/sentry/.env` по аналогии с `/Docker/sentry/.env.template`
1. В папке проекта выполнить `docker-compose -f ./Docker/sentry/docker-compose.yml up -d` и дождаться запуска контейнера sentry-web

### Запуск продакшн версии в Docker
1. _Нужно включить Compose v2 в настройках Docker_
1. Запустить Sentry - выполнить в папке проекта `docker-compose -f ./Docker/sentry/sentry.yml up` и дождаться пока всё прогрузится
1. Создать файл `/Docker/.env` по аналогии с `/Docker/.env.template`
1. Создать файл `/Uhost.Core/appsettings.json` по аналогии с `/Uhost.Core/appsettings.json.template`, прописать подключения и пути, специфичные для работы из контейнера
1. Создать файл `/Uhost.Core/appsettings.docker.json` по аналогии с `/Uhost.Core/appsettings.json.template`, прописать подключения и пути, специфичные для работы из контейнера
1. Создать файл `/Uhost.Core/appsettings.deploy-win.json` по аналогии с `/Uhost.Core/appsettings.json.template`, прописать подключения и пути, специфичные для работы на машине, где будет обрабатываться очередь конвертации
1. Создать `/Uhost.Console/default-data.json` по аналогии с `/Uhost.Console/default-data.template.json`
1. Выполнить в папке проекта `docker-compose -f ./Docker/deploy.yml up -d` и дождаться пока всё запустится
1. Проект инициализирован и запущен

### Запуск бэка в Visual Studio
1. _Нужен любой Visual Studio с поддержкой .NET SDK 5.0 и сам .NET SDK 5.0_
1. Создать `/Docker/sentry/.env` по аналогии с `/Docker/sentry/.env.template`
1. Запустить Sentry - выполнить в папке проекта `docker-compose -f ./Docker/sentry/sentry.yml up` и дождаться пока всё прогрузится
1. Создать файл `/Uhost.Core/appsettings.json` по аналогии с `/Uhost.Core/appsettings.json.template` с учётом окружения системы
1. Создать файл `/Uhost.Console/default-data.json` по аналогии с `/Uhost.Console/default-data.json.template`
1. Создать файл `/Docker/.env` по аналогии с `/Docker/.env.template`
1. Поднять БД - выполнить в папке проекта `docker-compose -f ./Docker/support.yml up -d` и дождаться пока всё прогрузится
1. Выполнить миграции `dotnet ef database update --project Uhost.Core --context PostgreSqlDbContext`
1. Выполнить миграции `dotnet ef database update --project Uhost.Core --context PostgreSqlLogDbContext`
1. Загрузить данные по умолчанию `dotnet run --project Uhost.Console --file ./Uhost.Console/default-data.json`
1. Решение готово к отладке

### Запуск фронта
1. _Нужен установленный node 16 LTS и выше, а также npm_
1. Перейти в папку `/Frontend`
1. Создать файл `.env` по аналогии с `.env.template`
1. Создать файл `src/config.json` по аналогии с `src/config.json.template`
1. Установить зависимости node - выполнить `npm i`
1. Запустить dev сервер фронта - выполнить `npm run start`
1. Решение готово к отладке