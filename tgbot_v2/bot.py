import telebot

import markups as m
import DB
import re
import requests as r
import json
import os

TOKEN = os.environ["TG_TOKEN"]
CHATID = os.environ["TG_CHAT_ID"]

switch = 0

bot = telebot.TeleBot(TOKEN)
print('-----BOT STARTED-----')
temptoken = DB.gentempkey()
print('-----AUTH DONE-----')


@bot.message_handler(commands=['start', 'go'])
def start_handler(message):
    user_info = message.from_user.to_dict()
    DB.users.update({message.chat.id:0})
    bot.send_message(CHATID, text = f"""*Connected new user*\n {user_info}.""", parse_mode= 'Markdown')
    url = 'http://217.71.129.139:4500/Topics/get/all?offset=0&size=1000'
    btntxt, btnclbc = DB.gethandler(url, temptoken)
    bot.send_message(message.chat.id, 'Вас приветствует бот-помощник НГТУ!')
    genmessage = 'Выберите тему, которая вас интересует:\n'
    i = 0
    while i < len(btntxt):
        genmessage += str(i + 1) + '. *' + btntxt[i] + '*\n'
        i += 1
    bot.send_message(message.chat.id, genmessage,
                     reply_markup=m.create_markup(btntxt, btnclbc, len(btntxt), 1, -1), parse_mode= 'Markdown')


@bot.message_handler(content_types=["text"])
def forward(message):
    if message.chat.type == 'private':
        if DB.users.get(message.chat.id) == 1:
            bot.send_message(CHATID, 'new message from: *@' + str(message.chat.username) + '* userID: *' + str(
            message.chat.id) + '*', parse_mode= 'Markdown', reply_markup=m.create_additional_markup(1, message.chat.id))
            bot.forward_message(CHATID, message.chat.id, message.id)
        else:
            bot.send_message(message.chat.id, 'я не понимаю, что ты от меня хочешь!')
    if message.chat.type == 'group':
        user_id = message.reply_to_message.forward_from.id
        bot.send_message(user_id, 'new answer from: *SUPPORT*', parse_mode='Markdown', reply_markup=m.create_additional_markup(2, user_id))
        bot.copy_message(user_id, from_chat_id=message.chat.id, message_id=message.id)

@bot.callback_query_handler(func=lambda call: True)
def allcallbacks_handler(call):
    print(call.data)
    if call.data[0] == 't':
        topic_id = re.sub('\D', '', call.data)
        url = 'http://217.71.129.139:4500/Topics/get/' + topic_id + '/subtopics'
        btntxt, btnclbc = DB.gethandler(url, temptoken)
        genmessage = 'Выберите тему, которая вас интересует:\n'
        i = 0
        while i<len(btntxt):
            genmessage +=str(i+1) + '. *'+ btntxt[i] + '*\n'
            i+=1
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_markup(btntxt, btnclbc, len(btntxt), 2, -1), parse_mode= 'Markdown')

    if call.data[0] == 's':
        subtopic_id = re.sub('\D', '', call.data)
        url = 'http://217.71.129.139:4500/Subtopics/get/' + subtopic_id + '/questions'
        btntxt, btnclbc = DB.gethandler(url, temptoken)
        url2 = 'http://217.71.129.139:4500/Subtopics/get/' + subtopic_id + '/topic'
        headers = {'Accept': 'text/plain', 'Authorization': 'Bearer ' + temptoken}
        request = json.loads((r.get(url2, headers = headers)).text)
        previousID = request["id"]
        genmessage = 'Выберите тему, которая вас интересует:\n'
        i = 0
        while i < len(btntxt):
            genmessage += str(i + 1) + '. *' + btntxt[i] + '*\n'
            i += 1
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_markup(btntxt, btnclbc, len(btntxt), 3, previousID), parse_mode= 'Markdown')


    if call.data[0] == 'q':
        question_id = re.sub('\D', '', call.data)
        url = 'http://217.71.129.139:4500/Questions/get/' + question_id + '/answer'
        headers = {'Accept': 'text/plain', 'Authorization': 'Bearer ' + temptoken}
        requestvalue = json.loads((r.get(url, headers=headers)).text)["value"]
        print(requestvalue)
        url2 = 'http://217.71.129.139:4500/Questions/get/' + question_id + '/subtopic'
        previousID = json.loads((r.get(url2, headers=headers)).text)["id"]
        url3 = 'http://217.71.129.139:4500/Questions/get/' + question_id
        questiontext = json.loads((r.get(url3, headers=headers)).text)["value"]
        genmessage = 'Ваш вопрос:\n*' + questiontext + '*\nВаш ответ:\n*' + requestvalue + '*'
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_additional_markup(3, -1), parse_mode= 'Markdown')

    if call.data[0] == 'a':
        url = 'http://217.71.129.139:4500/Topics/get/all?offset=0&size=1000'
        btntxt, btnclbc = DB.gethandler(url, temptoken)
        genmessage = 'Выберите тему, которая вас интересует:\n'
        i = 0
        while i < len(btntxt):
            genmessage += str(i + 1) + '. *' + btntxt[i] + '*\n'
            i += 1
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_markup(btntxt, btnclbc, len(btntxt), 1, -1), parse_mode= 'Markdown')

    if call.data[0] == 'b':
        topic_id = re.sub('\D', '', call.data)
        url = 'http://217.71.129.139:4500/Topics/get/' + topic_id + '/subtopics'
        btntxt, btnclbc = DB.gethandler(url, temptoken)
        genmessage = 'Выберите тему, которая вас интересует:\n'
        i = 0
        while i < len(btntxt):
            genmessage += str(i + 1) + '. *' + btntxt[i] + '*\n'
            i += 1
        bot.edit_message_text(chat_id=call.message.chat.id, message_id=call.message.message_id, text=genmessage,
                     reply_markup=m.create_markup(btntxt, btnclbc, len(btntxt), 2, -1, -1), parse_mode= 'Markdown')

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
        url = 'http://217.71.129.139:4500/Topics/get/all?offset=0&size=1000'
        btntxt, btnclbc = DB.gethandler(url, temptoken)
        genmessage = 'Выберите тему, которая вас интересует:\n'
        i = 0
        while i < len(btntxt):
            genmessage += str(i + 1) + '. *' + btntxt[i] + '*\n'
            i += 1
        bot.send_message(call.message.chat.id, genmessage,
                         reply_markup=m.create_markup(btntxt, btnclbc, len(btntxt), 1, -1), parse_mode='Markdown')
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
            bot.send_message(CHATID, 'Тикет: ' + userID + ' / @' + nick + ' закрыт')


bot.polling()

