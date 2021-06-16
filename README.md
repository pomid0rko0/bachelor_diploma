# bachelor_diploma

## Установка

1. Склонировать этот репозиторий `git clone https://github.com/pomid0rko0/bachelor_diploma.git`
2. (Установить docker engine)[https://docs.docker.com/engine/install/ubuntu/]
3. **Linux-only** (Установить docker-compose)[https://docs.docker.com/compose/install/]

## Запуск

```bash
docker-compose up --build
```

Или дял запуска в "тихом" режиме (без вывода логов)

```
docker-compose up --build --detach
```

## Доступ

API:  
http://localhost:5005/swagger - swagger, там есть достаточно подробное описание имеющихся методов и там же их можно выполнить вручную.

Чат-бот:  
В телеграмме написать `/start` боту @NSTUtest2bot
