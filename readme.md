Build Image
---
* build image\
    `docker build -t dotnet-docker:v1.0.0 .`
* docker run\
`docker run -d --rm -p 5000:80 dotnet-app dotnet-docker:v1.0.0`
* docker stop\
`docker stop dotnet-app`

Develop
---
### 測試 postgres sql
* Create **volume**\
    `docker volume create postgres-data`
* Create **network**\
`docker network create postgres-net`

* run **PostgreSQL** in a container\
  設定 network 與 volume
  這裡使用 `postgres:12.1-apline` (檔案比較小)
    ```yaml
    docker run --rm -d --volume postgres-data:/var/lib/postgresql/data \
      --network postgres-net \ 
      --name db \
      -e POSTGRES_USER=postgres \
      -e POSTGRES_PASSWORD=postgres \
      postgres:12.1-alpine
    ```
  檢查 db 是否有 run 起來 \ `docker exec -it db psql -U postgres`

### Update the application to connect the database
* 加入 DbContext: `SchoolContext.cs`, 並設定 Program.cs 與 appsettings.json
* 加入 entity `Student.cs`
* 修改 Pages/Index.cshtml.cs

**build image and run** \
`docker build -t dotnet-docker:v1.1.0 .`\
`docker run --rm -d --network postgres-net --name dotnet-app -p 5000:80 dotnet-docker`

### Connect Adminer and populate the database
* run adminer\
  `docker run --rm -d --network postgres-net --name db-admin -p 8080:8080 adminer`
> **Adminer**
輕量級的 SQL 管理工具，有支援 `MySql`, `PostgreSQL`, `MS SQL(beta)`, `MongoDB(alpha)` ... 等等

透過 adminer 手動新增一筆資料, 並且檢查 `dotnet-app` 連線到 `db` 確認資料是否有被寫入 (**localhost:5000**)

接著 stop 所有的 containers, 準備使用 `Docker Compose`\

```
$ docker ps

CONTAINER ID   IMAGE                  COMMAND                  CREATED          STATUS          PORTS                    NAMES
8bbed4804070   adminer                "entrypoint.sh php -…"   9 minutes ago    Up 9 minutes    0.0.0.0:8080->8080/tcp   db-admin
30312323ce05   dotnet-docker:v1.1.0   "dotnet myWebApp.dll"    11 minutes ago   Up 11 minutes   0.0.0.0:5000->80/tcp     dotnet-app
0d1b4ea03b55   postgres:12.1-alpine   "docker-entrypoint.s…"   12 minutes ago   Up 12 minutes   5432/tcp                 db

$ docker stop db-admin dotnet-app db
```
### Docker Compose
定義了 `app`, `db` 與 `adminer`, 並設定了 `volume`
> 此時去檢查 `docker-compose.yml`, 會發現其中並沒有設定 network, 因為在 docker-compose, 會自動幫你建一個 network\


```
$ docker compose up

- Network dotnet-docker_default         Created    0.0s
- Container dotnet-docker-adiminer-1    Created    0.0s
- Container dotnet-docker-db-1          Created    0.0s
- Container dotnet-docker-app-1         Created    0.0s
```
也可以使用 Detached mode `docker-compose up --build -d`


