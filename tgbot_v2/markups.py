from telebot import types

def create_markup(texts, callbacks, leng, type, previousID):
    if type == 1:
        markup = types.InlineKeyboardMarkup(row_width=1)
        for text, callback in zip(texts, callbacks):
            markup_btn = types.InlineKeyboardButton(text=text, callback_data='t' + str(callback))
            markup.add(markup_btn)

        markup_btn = types.InlineKeyboardButton(text='📖ОБРАТИТЬСЯ В ПОДДЕРЖКУ', callback_data='gotosupport')
        markup.add(markup_btn)
        print('-----MARKUP LISTED-----')
    if type == 2:
        markup = types.InlineKeyboardMarkup(row_width=1)
        for text, callback in zip(texts, callbacks):
            markup_btn = types.InlineKeyboardButton(text=text, callback_data='s' + str(callback))
            markup.add(markup_btn)

        markup_btn = types.InlineKeyboardButton(text ='↩️ВЕРНУТЬСЯ НАЗАД', callback_data ='a' + str(previousID))
        markup.add(markup_btn)
        markup_btn = types.InlineKeyboardButton(text='📖ОБРАТИТЬСЯ В ПОДДЕРЖКУ', callback_data='gotosupport')
        markup.add(markup_btn)
        print('-----MARKUP LISTED-----')
    if type == 3:
        markup = types.InlineKeyboardMarkup(row_width=1)
        for text, callback in zip(texts, callbacks):
            markup_btn = types.InlineKeyboardButton(text=text, callback_data='q' + str(callback))
            markup.add(markup_btn)

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