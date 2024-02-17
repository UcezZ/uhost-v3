# uHost v3
Новая версия проекта uHost https://ucezz.sytes.net/Projects/mirea/uHost/v2

### Запуск в Docker
1. _Нужно включить Compose v2 в настройках Docker_
1. Создать /Docker/sentry/.env по аналогии с /Docker/sentry/.env.template
1. Запустить Sentry - выполнить в папке проекта `docker-compose -f ./Docker/sentry/sentry.yml up` и дождаться пока всё прогрузится
1. Создать /Docker/.env по аналогии с /Docker/.env.template
1. Создать /Uhost.Core/appsettings.docker.json по аналогии с /Uhost.Core/appsettings.template.json, прописать подключения, специфичные для работы из контейнера
1. Создать /Uhost.Console/default-data.json по аналогии с /Uhost.Console/default-data.template.json
1. Выполнить в папке проекта `docker-compose -f ./Docker/deploy.yml up` и дождаться пока всё запустится
1. Проект инициализирован и запущен

### Запуск в Visual Studio
1. _Нужен любой VisualStudio с поддержкой .NET SDK 5.0 и сам .NET SDK 5.0_
1. Создать /Docker/sentry/.env по аналогии с /Docker/sentry/.env.template
1. Запустить Sentry - выполнить в папке проекта `docker-compose -f ./Docker/sentry/sentry.yml up` и дождаться пока всё прогрузится
1. Создать /Uhost.Core/appsettings.json по аналогии с /Uhost.Core/appsettings.template.json с учётом окружения системы
1. Создать /Uhost.Console/default-data.json по аналогии с /Uhost.Console/default-data.template.json с учётом окружения Docker'а
1. Создать /Docker/.env по аналогии с /Docker/.env.template
1. Поднять БД - выполнить в папке проекта `docker-compose -f ./Docker/support.yml up` и дождаться пока всё прогрузится
1. Выполнить миграции `dotnet ef database update --project Uhost.Core --context PostgreSqlDbContext`
1. Выполнить миграции `dotnet ef database update --project Uhost.Core --context PostgreSqlLogDbContext`
1. Загрузить данные по умолчанию `dotnet run --project Uhost.Console --file ./Uhost.Console/default-data.json`
1. Решение готово к отладке