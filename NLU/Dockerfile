FROM rasa/rasa:2.6.2

WORKDIR /app/NLU

USER root

RUN if [ ! -d ./models ]; then mkdir ./models; fi

RUN apt-get update && \
    apt-get install -y \
    glibc-source \
    language-pack-ru

ENV LANG ru_RU.utf8
ENV LANG ru_RU.utf8
ENV LANGUAGE ru_RU.utf8
ENV LC_ALL ru_RU.utf8
ENV LC_CTYPE ru_RU.utf8

RUN localedef -i ru_RU -c -f UTF-8 -A /usr/share/locale/locale.alias ru_RU.UTF-8

RUN apt-get update && \
    apt-get install -y \
    python3-dev \
    python3-pip && \
    pip3 install -U pip && \
    pip3 install rasa

COPY ./config.yml ./config.yml
COPY ./domain.yml ./domain.yml
COPY ./data ./data

EXPOSE 5005
