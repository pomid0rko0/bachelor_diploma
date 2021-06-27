import telebot

import markups as m
import DB
import re
import os

TOKEN = os.environ["TG_TOKEN"]
CHATID = os.environ["TG_CHAT_ID"]

database = DB.Database()
nstuapi = nstu_api.NstuApi

switch = 0

bot = telebot.TeleBot(TOKEN)
print('-----BOT STARTED-----')

def make_text_list(arr):
    return "\n".join(f"{i + 1}. *{text['value']}*" for i, text in enumerate(arr))

@bot.message_handler(commands=['start', 'go'])
def start_handler(message):
    user_info = message.from_user.to_dict()
    DB.users.update({message.chat.id:0})
    bot.send_message(CHATID, text = f"Connected new user:\n*{message.from_user}*", parse_mode= 'Markdown')
    topics = database.get_topics()
    genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(topics)}\n"
    bot.send_message(message.chat.id, genmessage,
                     reply_markup=m.create_markup(topics, 1, -1), parse_mode= 'Markdown')

@bot.message_handler(commands=['userinfo'])
def userinfo(message):
    userid = message.replace('/userinfo', '').strip()
    info = nstuapi.check_abit(userid)
    userinfomsg = f"Статус: *{info['STATUS'].encode('ascii').decode('unicode_escape')}*\n " \
                f"Имя: *{info['NAME'].encode('ascii').decode('unicode_escape')}*\n" \
                f"Заочная форма обучения: *{ 'Нет' if int(info['IS_ZAOCH']) == 0 else 'Да'}*\n" \
                f"Иностранный студент: *{ 'Нет' if int(info['IS_FOREIGN']) == 0 else 'Да'}*\n" \
                f"Имеются льготы: *{ 'Нет' if int(info['IS_LGOTA']) == 0 else 'Да'}*\n" \
                f"Контрактная форма обучения: *{ 'Нет' if int(info['IS_CONTRACT']) == 0 else 'Да'}*\n"    
    bot.send_message(CHATID, userinfomsg, parse_mode='Markdown')

@bot.message_handler(content_types=["text"])
def forward(message):
    if message.chat.type == 'private':
        if DB.users.get(message.chat.id) == 1:
            bot.send_message(CHATID, f"new message from: *@{message.chat.username}* userID: *{message.chat.id}*", 
            parse_mode= 'Markdown', reply_markup=m.create_additional_markup(1, message.chat.id))
            bot.forward_message(CHATID, message.chat.id, message.id)
        else:
            bot.send_message(message.chat.id, database.parse(message.chat.username, message.text))
    if message.chat.type == 'group':
        user_id = message.reply_to_message.forward_from.id
        bot.send_message(user_id, 'new answer from: *SUPPORT*', parse_mode='Markdown', reply_markup=m.create_additional_markup(2, user_id))
        bot.copy_message(user_id, from_chat_id=message.chat.id, message_id=message.id)

@bot.callback_query_handler(func=lambda call: True)
def allcallbacks_handler(call):
    if call.data[0] == 't':
        topic_id = re.sub('\D', '', call.data)
        subtopics = database.topic_get_subotpics(topic_id)
        genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(subtopics)}\n"
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_markup(subtopics, 2, -1), parse_mode= 'Markdown')

    if call.data[0] == 's':
        subtopic_id = re.sub('\D', '', call.data)
        questions = database.subtopic_get_questions(subtopic_id)
        topic = database.subtopic_get_topic(subtopic_id)
        previousID = topic["id"]
        genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(questions)}\n"
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_markup(questions, 3, previousID), parse_mode= 'Markdown')


    if call.data[0] == 'q':
        question_id = re.sub('\D', '', call.data)
        answer = database.question_get_answer(question_id)
        answervalue = answer["value"]
        answerlink = answer["fullAnswerUrl"]
        subtopic = database.question_get_subtopic(question_id)
        previousID = subtopic["id"]
        question = database.question_get_question(question_id)
        questiontext = question["value"]
        genmessage = f"Ваш вопрос:\n*{questiontext}*\nВаш ответ:\n*{answervalue}*\n[Прочитать больше]({answerlink})"
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_additional_markup(3, -1), parse_mode= 'Markdown')

    if call.data[0] == 'a':
        topics = database.get_topics()
        genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(topics)}\n"
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_markup(topics, 1, -1), parse_mode= 'Markdown')

    if call.data[0] == 'b':
        topic_id = re.sub('\D', '', call.data)
        subtopics = database.topic_get_subotpics(topic_id)
        genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(subtopics)}\n"
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_markup(subtopics, 2, -1), parse_mode= 'Markdown')

    if call.data == 'gotosupport':
        genmessage = 'Отправьте в чат *вопрос*, который вас интересует:\n'
        DB.users.update({call.message.chat.id:1})
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=call.message.text,
                              parse_mode='Markdown')
        bot.send_message(call.message.chat.id, text = genmessage, reply_markup = m.create_additional_markup(3, -1),
                         parse_mode= 'Markdown')

    if call.data == '/start':
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=call.message.text, parse_mode ='Markdown')
        DB.users.update({call.message.chat.id: 0})
        topics = database.get_topics()
        genmessage = f"Выберите тему, которая вас интересует:\n{make_text_list(topics)}\n"
        bot.send_message(call.message.chat.id, genmessage,
                         reply_markup=m.create_markup(topics, 1, -1), parse_mode='Markdown')
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

while True:
    try:
        bot.polling()
    except:
        pass
