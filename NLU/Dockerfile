FROM rasa/rasa:2.6.2

WORKDIR /app/NLU

COPY ./config.yml ./config.yml
COPY ./domain.yml ./domain.yml
COPY ./data ./data

RUN if [ ! -d ./models ]; then mkdir ./models; fi

USER root

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    python3-dev \
    python3-pip && \
    pip3 install -U pip && \
    pip3 install rasa

EXPOSE 5005
