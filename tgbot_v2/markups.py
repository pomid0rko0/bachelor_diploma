from telebot import types

def create_markup(texts, callbacks, leng, type, previousID):
    if type == 1:
        markup = types.InlineKeyboardMarkup(row_width=1)
        i = 0
        while i != leng:
            markup_btn = types.InlineKeyboardButton(text = texts[i], callback_data='t' + str(callbacks[i]))
            markup.add(markup_btn)
            i+=1
        markup_btn = types.InlineKeyboardButton(text='📖ОБРАТИТЬСЯ В ПОДДЕРЖКУ', callback_data='gotosupport')
        markup.add(markup_btn)
        print('-----MARKUP LISTED-----')
    if type == 2:
        markup = types.InlineKeyboardMarkup(row_width=1)
        i = 0
        while i != leng:
            markup_btn = types.InlineKeyboardButton(text=texts[i], callback_data='s' + str(callbacks[i]))
            markup.add(markup_btn)
            i += 1
        markup_btn = types.InlineKeyboardButton(text ='↩️ВЕРНУТЬСЯ НАЗАД', callback_data ='a' + str(previousID))
        markup.add(markup_btn)
        markup_btn = types.InlineKeyboardButton(text='📖ОБРАТИТЬСЯ В ПОДДЕРЖКУ', callback_data='gotosupport')
        markup.add(markup_btn)
        print('-----MARKUP LISTED-----')
    if type == 3:
        markup = types.InlineKeyboardMarkup(row_width=1)
        i = 0
        while i != leng:
            markup_btn = types.InlineKeyboardButton(text=texts[i], callback_data='q' + str(callbacks[i]))
            markup.add(markup_btn)
            i += 1
        markup_btn = types.InlineKeyboardButton(text='↩️ВЕРНУТЬСЯ НАЗАД', callback_data='b' + str(previousID))
        markup.add(markup_btn)
        markup_btn = types.InlineKeyboardButton(text='📖 ОБРАТИТЬСЯ В ПОДДЕРЖКУ', callback_data='gotosupport')
        markup.add(markup_btn)

    return markup

def create_additional_markup(type, userID):
    if type == 1:
        markup = types.InlineKeyboardMarkup(row_width=1)
        print('-----MARKUP LISTED-----')
        markup_btn = types.InlineKeyboardButton(text='ЗАКРЫТЬ ТИКЕТ', callback_data='c' + str(userID))
        markup.add(markup_btn)

    if type == 2:
        markup = types.InlineKeyboardMarkup(row_width=1)
        print('-----MARKUP LISTED-----')
        markup_btn = types.InlineKeyboardButton(text='ОТВЕТ ПОЛУЧЕН', callback_data='c' + str(userID))
        markup.add(markup_btn)

    if type == 3:
        markup = types.InlineKeyboardMarkup(row_width=1)
        print('-----MARKUP LISTED-----')
        markup_btn = types.InlineKeyboardButton(text='🆕 НАЧАТЬ ЗАНОВО', callback_data='/start')
        markup.add(markup_btn)
    return markup