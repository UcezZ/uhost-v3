# uHost v3
Новая версия проекта uHost https://ucezz.sytes.net/Projects/mirea/uHost/v2

### Запуск в Docker
0. _Нужно включить Compose v2 в настройках Docker_
1. Создать /Docker/.env по аналогии с /Docker/.env.template
2. Создать /Docker/appsettings.json по аналогии с /Uhost.Core/appsettings.template.json, прописать подключения, специфичные для работы из контейнера
3. Создать /Uhost.Core/appsettings.json по аналогии с /Uhost.Core/appsettings.template.json
4. Создать /Uhost.Console/default-data.json по аналогии с /Uhost.Console/default-data.template.json
5. Выполнить в папке проекта `docker-compose -f ./Docker/app.yml up` и дождаться пока всё запустится
6. Проект инициализирован и запущен

### Запуск в Visual Studio
0. _Нужен любой VisualStudio с поддержкой .NET 5.0 и сам .NET 5.0_
1. Создать /Uhost.Core/appsettings.json по аналогии с /Uhost.Core/appsettings.template.json
2. Создать /Uhost.Console/default-data.json по аналогии с /Uhost.Console/default-data.template.json
3. Создать /Docker/.env по аналогии с /Docker/.env.template
4. Поднять БД - выполнить в папке проекта `docker-compose -f ./Docker/support.yml up` и дождаться пока всё прогрузится
5. Выполнить миграции `dotnet ef database update --project Uhost.Core --context PostgreSqlDbContext`
6. Загрузить данные по умолчанию `dotnet run --project Uhost.Console --file ./Uhost.Console/default-data.json`
7. Решение готово к отладке