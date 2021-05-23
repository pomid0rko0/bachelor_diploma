# bachelor_diploma

## Установка

1. Склонировать этот репозиторий `git clone https://github.com/pomid0rko0/bachelor_diploma.git`
2. (Установить docker engine)[https://docs.docker.com/engine/install/ubuntu/]
3. **Linux-only** (Установить docker-compose)[https://docs.docker.com/compose/install/]

## Запуск

```bash
docker-compose up --build
```

## Доступ

NLU:  
http://localhost:5005 - проверить работоспособность  
http://localhost:5005/model/parse - POST-запрос json-объекта с полем `text`, содержащий строку для классификации. Например:

```bash
curl --header "Content-Type: application/json" \
    --request POST \
    --data "{\"text\":\"пользовательский ввод\"}" \
    http://localhost:5005/model/parse
```

API:  
http://localhost:5004/swagger - swagger, там есть достаточно подробное описание имеющихся методов и там же их можно выполнить вручную.
