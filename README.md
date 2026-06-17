# BetterCRM

BetterCRM - CRM-система для организаций с тикет-менеджментом, учетом рабочего времени, сменами, расчетом зарплаты, чатом и уведомлениями.

Проект построен как backend на ASP.NET Core и frontend на Vue 3. Backend разделен на слои Core, Business, DataAccess и Api; данные хранятся в PostgreSQL, фоновые события обрабатываются через RabbitMQ, файлы вложений загружаются в MinIO.

## Возможности

- Регистрация организации и первого руководителя.
- JWT-аутентификация и роли пользователей: `OrganizationHead`, `DepartmentHead`, `Employee`.
- Мультитенантность на уровне организации: данные фильтруются по `OrganizationId`.
- Управление пользователями внутри организации.
- Тикеты с приоритетами, статусами, SLA, назначением исполнителя и переводом между отделами.
- Комментарии к тикетам и файловые вложения.
- Участники тикета с ролями `Worker`, `Reviewer`, `Observer`.
- Учет рабочего времени: старт/стоп рабочей сессии, статистика за день, неделю и месяц.
- Смены, перерывы, фиксация опозданий и раннего ухода.
- Расчет зарплаты на основе смен, рабочих сессий, ставки и штрафных часов.
- Приватный чат и чат отдела, включая отправку изображений.
- Уведомления и realtime-события через SignalR.
- OpenAPI/Scalar-документация в Development-режиме.

## Стек

Backend:

- .NET 10 / ASP.NET Core
- Entity Framework Core 10
- PostgreSQL
- MassTransit + RabbitMQ
- SignalR
- MinIO
- JWT Bearer Auth
- Scalar для API-документации

Frontend:

- Vue 3
- Vite
- TypeScript
- Pinia
- Vue Router

Инфраструктура:

- Docker
- Docker Compose

## Структура проекта

```text
BetterCRM/
├── BetterCRM.Api/          # ASP.NET Core API, контроллеры, SignalR hubs, middleware
├── BetterCRM.Business/     # бизнес-сервисы, политики, consumers, JWT helper
├── BetterCRM.Core/         # доменные модели, интерфейсы, enums, роли
├── BetterCRM.DataAccess/   # EF Core DbContext, entities, repositories, migrations
├── frontend/               # Vue 3 frontend
├── Dockerfile              # сборка backend API
├── docker-compose.yml      # PostgreSQL, RabbitMQ, MinIO, API
└── BetterCRM.slnx          # solution-файл .NET
```

## Быстрый запуск через Docker Compose

Требования:

- Docker
- Docker Compose

Запуск инфраструктуры и API:

```bash
docker compose up --build
```

После запуска будут доступны:

- API: `http://localhost:5188`
- Scalar API Reference: `http://localhost:5188/scalar`
- OpenAPI JSON: `http://localhost:5188/openapi/v1.json`
- PostgreSQL: `localhost:5433`
- RabbitMQ Management UI: `http://localhost:15672`
- MinIO Console: `http://localhost:9001`

Учетные данные сервисов из `docker-compose.yml`:

```text
PostgreSQL:
  database: bettercrm
  user: postgres
  password: test123

RabbitMQ:
  user: guest
  password: guest

MinIO:
  user: minioadmin
  password: minioadmin
```

В Development-режиме API автоматически применяет pending EF Core migrations при старте.

## Локальный запуск backend

Требования:

- .NET SDK 10
- PostgreSQL
- RabbitMQ
- MinIO

В `BetterCRM.Api/appsettings.json` задаются основные настройки:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=bettercrm;Username=postgres;Password=test123"
  },
  "Jwt": {
    "Key": "your-super-secret-key-minimum-32-characters!!",
    "Issuer": "BetterCRM",
    "Audience": "BetterCRM-Frontend"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Minio": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": "false"
  }
}
```

Запуск API:

```bash
dotnet restore BetterCRM.Api/BetterCRM.Api.csproj
dotnet run --project BetterCRM.Api
```

По умолчанию профиль запуска использует:

- HTTP: `http://localhost:5188`
- HTTPS: `https://localhost:7128`

Полная сборка backend:

```bash
dotnet build BetterCRM.slnx
```

## Локальный запуск frontend

Требования:

- Node.js `^20.19.0` или `>=22.12.0`
- npm

Установка зависимостей и запуск dev-сервера:

```bash
cd frontend
npm install
npm run dev
```

Frontend по умолчанию обращается к API по адресу `http://localhost:5188`.
Адрес можно переопределить через переменную окружения:

```bash
VITE_API_URL=http://localhost:5188 npm run dev
```

Сборка frontend:

```bash
npm run build
```

Проверка типов и lint:

```bash
npm run type-check
npm run lint
```

## Основные API-модули

Все защищенные endpoints требуют заголовок:

```http
Authorization: Bearer <jwt-token>
```

Публичные endpoints:

- `POST /api/auth/register` - регистрация организации и первого руководителя.
- `POST /api/auth/login` - вход по email и паролю.
- `GET /api/auth/me` - текущий пользователь.

Пользователи:

- `POST /api/users` - создание пользователя руководителем организации или отдела.

Тикеты:

- `POST /api/tickets` - создать тикет.
- `GET /api/tickets` - получить доступные пользователю тикеты.
- `GET /api/tickets/{id}` - получить тикет.
- `POST /api/tickets/{id}/approve` - одобрить тикет.
- `POST /api/tickets/{id}/reject` - отклонить тикет.
- `POST /api/tickets/{id}/resolve` - отметить тикет решенным.
- `POST /api/tickets/{id}/close` - закрыть тикет.
- `POST /api/tickets/{id}/transfer` - передать тикет в другой отдел или исполнителю.
- `GET /api/tickets/{id}/participants` - участники тикета.
- `POST /api/tickets/{id}/participants` - добавить участника.
- `DELETE /api/tickets/{id}/participants/{userId}` - удалить участника.

Комментарии и вложения тикетов:

- `GET /api/tickets/{ticketId}/comments` - список комментариев.
- `POST /api/tickets/{ticketId}/comments` - добавить комментарий и вложения.
- `PUT /api/tickets/{ticketId}/comments/{commentId}` - обновить комментарий.
- `DELETE /api/tickets/{ticketId}/comments/{commentId}` - удалить комментарий.
- `POST /api/tickets/{ticketId}/comments/{commentId}/attachments` - добавить вложение.
- `DELETE /api/tickets/{ticketId}/comments/{commentId}/attachments/{attachmentId}` - удалить вложение.

Учет рабочего времени:

- `POST /api/timetracking/start` - начать рабочую сессию.
- `POST /api/timetracking/stop` - завершить рабочую сессию.
- `GET /api/timetracking/active` - активная сессия.
- `GET /api/timetracking/sessions` - история сессий.
- `GET /api/timetracking/stats` - часы за день, неделю и месяц.
- `GET /api/timetracking/earnings/week` - оценка заработка за текущую неделю.
- `GET /api/timetracking/hours-by-day` - часы по дням за период.

Смены:

- `POST /api/shifts` - создать смену.
- `GET /api/shifts/today` - смена текущего пользователя на сегодня.
- `GET /api/shifts/user/{userId}` - смены пользователя.
- `GET /api/shifts/department/{departmentId}` - смены отдела.
- `GET /api/shifts/organization/{orgId}` - смены организации.
- `PATCH /api/shifts/{shiftId}` - обновить смену.
- `POST /api/shifts/{shiftId}/breaks` - добавить перерыв.
- `DELETE /api/shifts/breaks/{breakId}` - удалить перерыв.

Расчет зарплаты:

- `GET /api/payroll/preview` - предварительный расчет для текущего пользователя.
- `GET /api/payroll/preview/user/{userId}` - предварительный расчет для пользователя.
- `POST /api/payroll/calculate/user/{userId}` - рассчитать зарплату пользователя.
- `POST /api/payroll/calculate/department/{departmentId}` - рассчитать зарплату отдела.
- `GET /api/payroll/user/{userId}` - получить сохраненный расчет.

Чат:

- `GET /api/chat/private/{userId}` - история приватного чата.
- `GET /api/chat/department/{departmentId}` - история чата отдела.
- `POST /api/chat/private/{messageId}/read` - отметить приватное сообщение прочитанным.
- `POST /api/chat/private/{userId}/image` - отправить изображение в приватный чат.
- `POST /api/chat/department/{departmentId}/image` - отправить изображение в чат отдела.

Уведомления:

- `GET /api/notifications` - список уведомлений.
- `GET /api/notifications/unread-count` - количество непрочитанных.
- `POST /api/notifications/{id}/read` - отметить уведомление прочитанным.
- `POST /api/notifications/read-all` - отметить все уведомления прочитанными.

SignalR hubs:

- `/hubs/chat`
- `/hubs/notifications`

## Доменные статусы

Тикеты:

- `Draft`
- `Open`
- `InProgress`
- `Resolved`
- `Closed`

Приоритеты тикетов:

- `Low`
- `Medium`
- `High`

Смены:

- `Scheduled`
- `Completed`
- `Cancelled`

Типы перерывов:

- `Lunch`
- `Rest`
- `Custom`

Payroll:

- `Calculated`
- `Approved`
- `Paid`

## Миграции EF Core

В Development-режиме миграции применяются автоматически при старте API.

Ручное применение миграций:

```bash
dotnet ef database update \
  --project BetterCRM.DataAccess \
  --startup-project BetterCRM.Api
```

Создание новой миграции:

```bash
dotnet ef migrations add <MigrationName> \
  --project BetterCRM.DataAccess \
  --startup-project BetterCRM.Api
```

## Переменные окружения

Основные переменные, которые можно задавать через environment variables:

```text
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__Default=Host=localhost;Port=5432;Database=bettercrm;Username=postgres;Password=test123
Jwt__Key=your-super-secret-key-minimum-32-characters!!
Jwt__Issuer=BetterCRM
Jwt__Audience=BetterCRM-Frontend
RabbitMQ__Host=localhost
RabbitMQ__Username=guest
RabbitMQ__Password=guest
Minio__Endpoint=localhost:9000
Minio__AccessKey=minioadmin
Minio__SecretKey=minioadmin
Minio__UseSSL=false
Cors__AllowedOrigins__0=http://localhost:5173
```

Для production нужно заменить тестовые пароли и JWT-ключи, настроить HTTPS, CORS и постоянное хранилище файлов.

## Полезные команды

Backend:

```bash
dotnet restore BetterCRM.Api/BetterCRM.Api.csproj
dotnet build BetterCRM.slnx
dotnet run --project BetterCRM.Api
```

Frontend:

```bash
cd frontend
npm install
npm run dev
npm run build
npm run lint
```

Docker:

```bash
docker compose up --build
docker compose down
docker compose down -v
```

`docker compose down -v` удалит volumes PostgreSQL и MinIO вместе с сохраненными данными.
