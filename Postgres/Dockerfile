FROM postgres:13.3

RUN localedef -i ru_RU -c -f UTF-8 -A /usr/share/locale/locale.alias ru_RU.UTF-8
ENV LANG ru_RU.utf8
ENV LANG ru_RU.utf8
ENV LANGUAGE ru_RU.utf8
ENV LC_ALL ru_RU.utf8
ENV LC_CTYPE ru_RU.utf8
ENV POSTGRES_INITDB_ARGS -E UTF8

ENV POSTGRES_USER NSTUChatbot
ENV POSTGRES_PASSWORD NSTUChatbot
ENV POSTGRES_DB NSTUChatbotDatabase
