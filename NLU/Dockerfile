FROM rasa/rasa:2.6.3 as base

ENV RASA_TELEMETRY_ENABLED=false
ENV SPELLING_MODEL_FILE ru_nstu_chatbot.bin

WORKDIR /app/NLU
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

ENV SPELLING_ARCHIVE_FILE dialogues.zip
ENV SPELLING_ALPHABET_FILE alhabet_ru.txt
ENV SPELLING_DATASET_FILE dialogues.txt
ENV SPELLING_MODEL_FILE ru_nstu_chatbot.bin

RUN apt-get update 
RUN apt-get install -y locales p7zip-full p7zip-rar
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
COPY JamSpell/${SPELLING_ARCHIVE_FILE} ${SPELLING_ARCHIVE_FILE}
RUN 7za x ${SPELLING_ARCHIVE_FILE}
RUN ./jamspell train ${SPELLING_ALPHABET_FILE} ${SPELLING_DATASET_FILE} ${SPELLING_MODEL_FILE}

FROM rasa_build AS rasa_run

COPY --from=jamspell_train /JamSpell/${SPELLING_MODEL_FILE} models/${SPELLING_MODEL_FILE}
CMD [ "run", "--enable-api" ]
