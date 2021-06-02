FROM rasa/rasa:2.6.3

ENV RASA_TELEMETRY_ENABLED=false
ENV NLU_PATH /app/NLU
ENV JAMSPELL_FOLDER JamSpell
ENV MODELS_FOLDER models
ENV SPELLING_ARCHIVE_FILE dialogues.zip
ENV SPELLING_ALPHABET_FILE alhabet_ru.txt
ENV SPELLING_DATASET_FILE dialogues.txt
ENV SPELLING_MODEL_FILE ru_nstu_chatbot.bin

USER root
RUN apt-get install -y \
    cmake \
    g++ \
    git \
    locales \
    p7zip-full \
    p7zip-rar \
    swig3.0
RUN pip install jamspell

RUN rm -rf /var/lib/apt/lists/* 
RUN localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8
ENV LANG en_US.utf8

WORKDIR /
RUN git clone https://github.com/bakwc/JamSpell.git
WORKDIR /${JAMSPELL_FOLDER}/build
RUN cmake ..
RUN make

WORKDIR /${JAMSPELL_FOLDER}/build/main
COPY ${JAMSPELL_FOLDER}/${SPELLING_ARCHIVE_FILE} ${SPELLING_ARCHIVE_FILE}
RUN 7za x ${SPELLING_ARCHIVE_FILE}
RUN ./jamspell train ${SPELLING_ALPHABET_FILE} ${SPELLING_DATASET_FILE} ${SPELLING_MODEL_FILE}


WORKDIR ${NLU_PATH}
RUN if [ ! -d ./${JAMSPELL_FOLDER} ]; then mkdir ./${JAMSPELL_FOLDER}; fi
RUN if [ ! -d ./${MODELS_FOLDER} ]; then mkdir ./${MODELS_FOLDER}; fi
RUN mv /${JAMSPELL_FOLDER}/build/main/${SPELLING_MODEL_FILE} ${JAMSPELL_FOLDER}/${SPELLING_MODEL_FILE}
COPY spelling_correction.py spelling_correction.py
COPY text_preprocessing.py text_preprocessing.py
EXPOSE 5005
CMD [ "run", "--enable-api" ]
