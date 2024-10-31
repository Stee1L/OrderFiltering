# Обзор

<p style="font-size: 16px;">
Приложение OrderFiltering представляет собой Web API, разработанное на платформе .NET Core,
предназначенное для управления и фильтрации заказов на основе различных критериев. 
Приложение позволяет создавать, изменять, удалять и получать заказы и районы, 
а также фильтровать заказы по району и времени доставки. 
Результаты фильтрации могут быть сохранены либо в базу данных, либо в файл, 
в зависимости от конфигурации.
</p>

## Основные функции


+ <p style="font-size: 16px;">Управление заказами: Создание, обновление, удаление и получение заказов.</p>

- <p style="font-size: 16px;">Управление районами: Создание, обновление, удаление и получение районов.</p>

+ <p style="font-size: 16px;">Фильтрация заказов: Фильтрация заказов по району и времени доставки с сохранением результатов в базу данных или файл.</p>

- <p style="font-size: 16px;">Поддержка баз данных: Поддерживает как PostgreSQL, так и SQL Server.</p>

+ <p style="font-size: 16px;">Документация API: Предоставляет документацию API через Swagger UI. </p>

## Используемые технологии


+ <p style="font-size: 16px;">.NET Core 6.0</p>

- <p style="font-size: 16px;">Entity Framework Core</p>

+ <p style="font-size: 16px;">PostgreSQL Server</p>

- <p style="font-size: 16px;">Swagger</p>

+ <p style="font-size: 16px;">CORS</p>

- <p style="font-size: 16px;">Логирование</p>

## Настройка и конфигурация

### Предварительные требования

+ <p style="font-size: 16px;">Установленный .NET Core SDK 6.0</p>

- <p style="font-size: 16px;">PostgreSQL или SQL Server</p>

+ <p style="font-size: 16px;">JetBrains Rider / Visual Studio или любой другой редактор кода (например, Visual Studio Code)</p>

## Шаги по настройке


<p style="font-size: 16px;">Клонирование репозитория:</p>

```bash
git clone <https://github.com/Stee1L/OrderFiltering/tree/master>
cd OrderFiltering
```

<p style="font-size: 16px;">Настройка строки подключения к базе данных:</p>
Откройте файл appsettings.json и настройте строку подключения к базе данных в разделе ConnectionStrings. Пример для PostgreSQL:
    
```json
"ConnectionStrings": {
    "PostgreSQL": "PORT = 5432; HOST = localhost; TIMEOUT = 15; POOLING = True; MINPOOLSIZE = 1; MAXPOOLSIZE = 100; COMMANDTIMEOUT = 20; DATABASE = 'OrderFiltering_v1'; PASSWORD = '1'; USER ID = 'postgres'"
}
```

<p style="font-size: 16px;">Выбор базы данных:</p>
В файле appsettings.json укажите тип базы данных, которую вы хотите использовать:

```json
"CurrentDatabaseConnectionString": "PostgreSQL"
```
<p style="font-size: 16px;">Миграция базы данных:</p>
Выполните миграцию базы данных, чтобы создать необходимые таблицы:

```bash
dotnet ef database update
```
<p style="font-size: 16px;">Запуск приложения:</p>
Запустите приложение с помощью команды:

```bash
dotnet run
```
<p style="font-size: 16px;">Доступ к API:</p>
После запуска приложения, вы можете получить доступ к API через Swagger по адресу:

>http://localhost:8000/swagger

<p style="font-size: 16px;">Конфигурация приложения</p>
Файл appsettings.json содержит основные настройки приложения:

- Logging: Уровень логирования и путь к файлу логов.

- ResultFilePath: Путь к файлу, в который будут сохраняться результаты фильтрации заказов.

- AllowedHosts: Разрешенные хосты для доступа к API.

- ConnectionStrings: Строки подключения к базам данных.

- URLS: URL-адреса, на которых будет доступно приложение.

- CurrentDatabaseConnectionString: Тип используемой базы данных.

## Использование API
### Управление районами

<p style="font-size: 16px;">Получение всех районов: </p>

```http request
GET /api/district/districts
```
<p style="font-size: 16px;">Создание нового района: </p>

```http request
POST /api/district/create
```

<p style="font-size: 16px;">Изменение района: </p>

```http request
PUT /api/district/change/{id}
```

<p style="font-size: 16px;">Удаление района:</p>

```http request
DELETE /api/district/delete/{id}
```
### Управление заказами

<p style="font-size: 16px;">Получение всех заказов: </p>

```http request
GET /api/order/orders
```

<p style="font-size: 16px;">Создание нового заказа: </p>

```http request
POST /api/order/create
```

<p style="font-size: 16px;">Изменение заказа: </p>

```http request
PUT /api/order/change/{id}
```

<p style="font-size: 16px;">Удаление заказа: </p>

```http request
DELETE /api/order/delete/{id}
```

<p style="font-size: 16px;">Фильтрация заказов: </p>

```http request
POST /api/order/filter
```

В методе фильтрации заказов следует указать, каким образом будут сохраняться результаты фильтрации. 
Для этого нужно отправить запрос в виде:
{
    "districtId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "deliveryTime": "2024-10-31T12:41:53.702Z",
    "filterResultOutputType": "Database"
},
где "filterResultOutputType" - строковое свойство, принимающее одно из значений: 
- "Database" - результаты будут сохранены в БД
- "File" - результаты будут сохранены в файл, указанный в файле конфигурации "appsettings.json"

## Логирование

<p style="font-size: 16px;">Приложение использует логирование для отслеживания ошибок и информационных сообщений. Логи сохраняются в файл, 
указанный в настройках (LoggingFilePath).</p>