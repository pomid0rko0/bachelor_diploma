print('-----STARTING BOT-----', flush=True)

import telebot

import markups as m
import DB
import re
import os
import nstu_api

TOKEN = os.environ["TG_TOKEN"]
CHATID = os.environ["TG_CHAT_ID"]

print('-----CONNECTING TO DATABASE-----', flush=True)

database = DB.Database()

print('-----CONNECTED TO DATABASE-----', flush=True)

nstuapi = nstu_api.NstuApi()

switch = 0

bot = telebot.TeleBot(TOKEN)
print('-----BOT STARTED-----', flush=True)

def make_text_list(arr):
    return "\n".join(f"{i + 1}. *{text['value']}*" for i, text in enumerate(arr))

@bot.message_handler(commands=['start', 'go'])
def start_handler(message):
    user_info = message.from_user.to_dict()
    DB.users.update({message.chat.id:0})
    bot.send_message(CHATID, f"Connected new user:\n*{message.from_user}*", parse_mode= 'Markdown')
    genmessage = f"Простите, что-то пошло не так. Попробуйте ещё раз позже."
    reply_markup = None
    try:
        topics = database.get_topics()
        genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(topics)}\n"
        reply_markup = m.create_markup(topics, 1, -1)
    except Exception as e:
        print(f"Error:\n{e}", flush=True)
        bot.send_message(CHATID, f"Error:\n```{e}```", parse_mode= 'Markdown')
    finally:
        print(genmessage, flush=True)
        print(reply_markup, flush=True)
        bot.send_message(message.chat.id, genmessage, reply_markup=reply_markup, parse_mode= 'Markdown')

@bot.message_handler(commands=['userinfo'])
def userinfo(message):
    userid = re.sub(r"[^A-Z\d]", "", message.text)
    print("userid:", userid, flush=True)
    bot.send_message(CHATID, f"User:\n*{message.from_user}* requested *{userid}*", parse_mode= 'Markdown')
    info = nstuapi.check_abit(userid)[0]
    print("info:", info, flush=True)
    if "NAME" in info:
        userinfomsg = f"Статус: *{info['STATUS']}*\n" \
                    f"Имя: *{info['NAME']}*\n" \
                    f"Заочная форма обучения: *{ 'Нет' if int(info['IS_ZAOCH']) == 0 else 'Да'}*\n" \
                    f"Иностранный студент: *{ 'Нет' if int(info['IS_FOREIGN']) == 0 else 'Да'}*\n" \
                    f"Имеются льготы: *{ 'Нет' if int(info['IS_LGOTA']) == 0 else 'Да'}*\n" \
                    f"Контрактная форма обучения: *{ 'Нет' if int(info['IS_CONTRACT']) == 0 else 'Да'}*\n"
    else:
        userinfomsg = "Извините, такого пользователя не знаю!"                
    bot.send_message(message.chat.id, userinfomsg, parse_mode='Markdown')

@bot.message_handler(content_types=["text"])
def forward(message):
    if message.chat.type == 'private':
        if DB.users.get(message.chat.id) == 1:
            bot.send_message(CHATID, f"new message from: *@{message.chat.username}* userID: *{message.chat.id}*", 
            parse_mode= 'Markdown', reply_markup=m.create_additional_markup(1, message.chat.id))
            bot.forward_message(CHATID, message.chat.id, message.id)
        else:
            genmessage = f"Простите, что-то пошло не так. Попробуйте ещё раз позже."
            try:
                genmessage = database.parse(message.chat.username, message.text)
            except Exception as e:
                print(f"Error:\n{e}", flush=True)
                bot.send_message(CHATID, f"Error:\n```{e}```", parse_mode= 'Markdown')
            finally:
                bot.send_message(message.chat.id, genmessage)
    if message.chat.type == 'group':
        user_id = message.reply_to_message.forward_from.id
        bot.send_message(user_id, 'new answer from: *SUPPORT*', parse_mode='Markdown', reply_markup=m.create_additional_markup(2, user_id))
        bot.copy_message(user_id, from_chat_id=message.chat.id, message_id=message.id)

@bot.callback_query_handler(func=lambda call: True)
def allcallbacks_handler(call):
    if call.data[0] == 't':
        topic_id = re.sub('\D', '', call.data)
        genmessage = f"Простите, что-то пошло не так. Попробуйте ещё раз позже."
        reply_markup = None
        try:
            subtopics = database.topic_get_subotpics(topic_id)
            genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(subtopics)}\n"
            reply_markup = m.create_markup(subtopics, 2, -1)
        except Exception as e:
            print(f"Error:\n{e}", flush=True)
            bot.send_message(CHATID, f"Error:\n```{e}```", parse_mode= 'Markdown')
        finally:
            bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=reply_markup, parse_mode= 'Markdown')

    if call.data[0] == 's':
        subtopic_id = re.sub('\D', '', call.data)
        genmessage = f"Простите, что-то пошло не так. Попробуйте ещё раз позже."
        reply_markup = None
        try:
            questions = database.subtopic_get_questions(subtopic_id)
            topic = database.subtopic_get_topic(subtopic_id)
            previousID = topic["id"]
            genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(questions)}\n"
            reply_markup = m.create_markup(questions, 3, previousID)
        except Exception as e:
            print(f"Error:\n{e}", flush=True)
            bot.send_message(CHATID, f"Error:\n```{e}```", parse_mode= 'Markdown')
        finally:
            bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=reply_markup, parse_mode= 'Markdown')


    if call.data[0] == 'q':
        question_id = re.sub('\D', '', call.data)
        genmessage = f"Простите, что-то пошло не так. Попробуйте ещё раз позже."
        reply_markup = None
        try:
            answer = database.question_get_answer(question_id)
            answervalue = answer["value"]
            answerlink = answer["fullAnswerUrl"]
            subtopic = database.question_get_subtopic(question_id)
            previousID = subtopic["id"]
            question = database.question_get_question(question_id)
            questiontext = question["value"]
            genmessage = f"Ваш вопрос:\n*{questiontext}*\nВаш ответ:\n*{answervalue}*\n[Прочитать больше]({answerlink})"
            reply_markup = m.create_additional_markup(3, -1)
        except Exception as e:
            print(f"Error:\n{e}", flush=True)
            bot.send_message(CHATID, f"Error:\n```{e}```", parse_mode= 'Markdown')
        finally:
            bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=reply_markup, parse_mode= 'Markdown')

    if call.data[0] == 'a':
        genmessage = f"Простите, что-то пошло не так. Попробуйте ещё раз позже."
        reply_markup = None
        try:
            topics = database.get_topics()
            genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(topics)}\n"
            reply_markup = m.create_markup(topics, 1, -1)
        except Exception as e:
            print(f"Error:\n{e}", flush=True)
            bot.send_message(CHATID, f"Error:\n```{e}```", parse_mode= 'Markdown')
        finally:
            bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=reply_markup, parse_mode= 'Markdown')

    if call.data[0] == 'b':
        topic_id = re.sub('\D', '', call.data)
        genmessage = f"Простите, что-то пошло не так. Попробуйте ещё раз позже."
        reply_markup = None
        try:
            subtopics = database.topic_get_subotpics(topic_id)
            genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(subtopics)}\n"
            reply_markup = m.create_markup(subtopics, 2, -1)
        except Exception as e:
            print(f"Error:\n{e}", flush=True)
            bot.send_message(CHATID, f"Error:\n```{e}```", parse_mode= 'Markdown')
        finally:
            bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=reply_markup, parse_mode= 'Markdown')

    if call.data == 'gotosupport':
        genmessage = 'Отправьте в чат *вопрос*, который вас интересует:\n'
        DB.users.update({call.message.chat.id:1})
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=call.message.text,
                              parse_mode='Markdown')
        bot.send_message(call.message.chat.id, genmessage, reply_markup = m.create_additional_markup(3, -1),
                         parse_mode= 'Markdown')

    if call.data == '/start':
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=call.message.text, parse_mode ='Markdown')
        DB.users.update({call.message.chat.id: 0})
        genmessage = f"Простите, что-то пошло не так. Попробуйте ещё раз позже."
        reply_markup = None
        try:
            topics = database.get_topics()
            genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(topics)}\n"
            reply_markup = m.create_markup(topics, 1, -1)
        except Exception as e:
            print(f"Error:\n{e}", flush=True)
            bot.send_message(CHATID, f"Error:\n```{e}```", parse_mode= 'Markdown')
        finally:
            bot.send_message(call.message.chat.id, genmessage, reply_markup=reply_markup, parse_mode='Markdown')
    if call.data[0] == 'c':
        userID = re.sub('\D', '', call.data)
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=call.message.text,
                              parse_mode='Markdown')
        if DB.users.get(int(userID)) == 1:
            DB.users.update({int(userID): 0})
            bot.send_message(userID, '*Спасибо, что обратились в поддержку\n'
                                     'Это сообщение означает то, что ваш тикет был закрыт вами или администратором поддержки\n'
                                     'Спасибо, что воспользовались нашим ботом, если у вас ещё есть вопросы - начните заново*',
                             reply_markup = m.create_additional_markup(3, -1), parse_mode= 'Markdown')
            nick = bot.get_chat_member(userID, userID).user.username
            bot.send_message(CHATID, f"Тикет: {userID} / @{nick} закрыт")

print('-----POLLING-----', flush=True)
bot.polling()
