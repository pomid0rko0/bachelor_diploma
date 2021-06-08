FROM python:rc-alpine3.13

RUN pip install pytelegrambotapi requests

WORKDIR /app/tgbot_v2

COPY ./tgbot_v2 .

CMD ["python", "bot.py"]
