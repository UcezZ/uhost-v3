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
dotnet ef database update --project Uhost.Core --context PostgreSqlLogDbContext
```

Building dockerfile
```
docker build -t <tag> -f <dockerfile> .
```

Multifile URL
```
source urls
/hls/0f/f3/4df239801e90893662a3a5b13368/temp_6a96fd76-0364-4fd1-ad57-429703c47006.mp4/master.m3u8
/hls/1a/9f/3e7f4308fce4b5f09ce8a1859364/2023-04-21 15-49-13.m4v/master.m3u8

merged url
/hls/,0f/f3/4df239801e90893662a3a5b13368/temp_6a96fd76-0364-4fd1-ad57-429703c47006.mp4,dZ/ri/1kR7Qerwt1wdlTRWeUh628WwoO3R/2023-05-01 17-29-44.mp4,.urlset/master.m3u8
```