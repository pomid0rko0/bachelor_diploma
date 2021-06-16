# bachelor_diploma

## Установка

1. Склонировать этот репозиторий
   ```bash
   git clone https://github.com/pomid0rko0/bachelor_diploma.git
   ```
2. [Установить docker engine](https://docs.docker.com/engine/install/)
3. **Linux-only** [Установить docker-compose](https://docs.docker.com/compose/install/)

## Подготовка окружения

1. Перейти в папку `bachelor_diploma` (или другую, в которую был склонирован репозиторий)
2. Создать `.env` файл со следующими переменными:
   - `JWT_TOKEN` - токен для работы аутентификации
   - `FIRST_EMAIL` - логин администратора
   - `FIRST_PASSWORD` - пароль администратора
   - `TOKEN_LIFETIME` - время жизни токена аутентификации (в секундах)
   - `TG_TOKEN` - токен чат-бота в Telegram
   - `TG_CHAT_ID` - идентификатор чата в Telegram
   - `DB_API_USER` - логин пользователя, от имени которого чат-бот будет обращаться к базе данных
   - `DB_API_PASSWORD` - пароль пользователя, от имени которого чат-бот будет обращаться к базе данных

## Запуск

```bash
docker-compose up --build
```

Или для запуска в "тихом" режиме (без вывода логов)

```bash
docker-compose up --build --detach
```

После запуска нужно зарегистрировать администратора, выполнив GET-запрос:

```bash
curl http://localhost:5005/AuthManagement/RegisterFirst
```

## Доступ

API:  
http://localhost:5005/swagger - swagger, там есть достаточно подробное описание имеющихся методов и там же их можно выполнить вручную.

Чат-бот:  
В телеграмме написать `/start` боту [@NSTUtest2bot](https://t.me/NSTUtest2bot)

## Остановка

```bash
docker-compose down -v
```
