FROM rasa/rasa:2.6.3 as base

ENV APP_FOLDER /app/NLU
ENV DATA_FOLDER data
ENV MODELS_FOLDER models
ENV SPELLING_ALPHABET_FILE alhabet_ru.txt
ENV SPELLING_DATASET_FILE dialogues.txt
ENV SPELLING_MODEL_FILE ru_nstu_chatbot.bin

WORKDIR ${APP_FOLDER}
COPY spelling_correction.py spelling_correction.py
COPY text_preprocessing.py text_preprocessing.py
EXPOSE 5005

FROM base as rasa_build

USER root
RUN apt-get install -y \
    cmake \
    g++ \
    locales \
    swig3.0
RUN pip install jamspell

RUN rm -rf /var/lib/apt/lists/* 
RUN localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8
ENV LANG en_US.utf8

FROM ubuntu:21.10 as lang

ENV DATA_FOLDER data
ENV MODELS_FOLDER models
ENV SPELLING_ALPHABET_FILE alhabet_ru.txt
ENV SPELLING_DATASET_FILE dialogues.txt
ENV SPELLING_MODEL_FILE ru_nstu_chatbot.bin

RUN apt-get update 
RUN apt-get install -y locales 
RUN rm -rf /var/lib/apt/lists/* 
RUN localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8
ENV LANG en_US.utf8

FROM rikorose/gcc-cmake:gcc-11 as jamspell_build

RUN git clone https://github.com/bakwc/JamSpell.git

WORKDIR /JamSpell/build
RUN cmake ..
RUN make

FROM lang as jamspell_train

WORKDIR /JamSpell
COPY --from=jamspell_build /JamSpell/build/main/jamspell ./jamspell
COPY data/${SPELLING_ALPHABET_FILE} ${SPELLING_ALPHABET_FILE}
COPY data/${SPELLING_DATASET_FILE} ${SPELLING_DATASET_FILE}
RUN ./jamspell train ${SPELLING_ALPHABET_FILE} ${SPELLING_DATASET_FILE} ${SPELLING_MODEL_FILE}

FROM rasa_build as rasa_train

RUN if [ ! -d ${MODELS_FOLDER} ]; then mkdir ${MODELS_FOLDER}; fi

COPY --from=jamspell_train /JamSpell/${SPELLING_MODEL_FILE} ${MODELS_FOLDER}/${SPELLING_MODEL_FILE}
COPY ${DATA_FOLDER}/nlu.yml ${DATA_FOLDER}/nlu.yml
COPY ${DATA_FOLDER}/rules.yml ${DATA_FOLDER}/rules.yml
COPY config.yml config.yml
COPY domain.yml domain.yml

RUN rasa train nlu
RUN rasa telemetry disable

FROM rasa_build AS rasa_run

COPY --from=rasa_train ${APP_FOLDER}/${MODELS_FOLDER} ./${MODELS_FOLDER}
CMD [ "run", "--enable-api" ]
