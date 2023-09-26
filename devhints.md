Installing EF tools
```
dotnet tool install --global dotnet-ef --version 5.0.10
```

Add migration
```
dotnet ef migrations add <name> --project Uhost.Core --context PostgreSqlDbContext -o Data/Migrations
```

Apply migrations
```
dotnet ef database update --project Uhost.Core --context PostgreSqlDbContext
```

Building dockerfile
```
docker build -t <tag> -f <dockerfile> .
```
