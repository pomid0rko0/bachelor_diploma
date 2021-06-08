FROM python:rc-alpine3.13

RUN pip install --upgrade pytelegrambotapi requests urllib3

WORKDIR /app/tgbot_v2

COPY . .

CMD ["python", "bot.py"]
