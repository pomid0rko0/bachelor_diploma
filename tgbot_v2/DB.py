import json
import requests as r
<<<<<<< HEAD
from config import ADMLOGIN, ADMPASS
import datetime
=======
import os
>>>>>>> d908c6da16060c9007cf887761575a8166c27019

users = {}

DB_API = os.environ["DB_API_HOST"]

def gethandler(url, temptoken):
    headers = {'Accept': 'text/plain', 'Authorization': 'Bearer ' + temptoken}
    request = json.loads((r.get(url, headers=headers)).text)
    print('-----REQUEST COMPLETED-----')
    i = 0
    DB = []
    DB2 = []
    while i != len(request):
        DB.append(request[i]["value"])
        DB2.append(request[i]["id"])
        i += 1

    return DB, DB2

def gentempkey():
<<<<<<< HEAD
    url = 'http://217.71.129.139:4500/AuthManagement/Login'
    data = {'email': ADMLOGIN, 'password': ADMPASS}
=======
    url = f'{DB_API}/AuthManagement/Login'
    data = {'email': 'legalov.2017@stud.nstu.ru', 'password': '~biSXA7SSU]pwI0x&1f'}
>>>>>>> d908c6da16060c9007cf887761575a8166c27019
    headers = {'Content-type': 'application/json', 'Accept': '*/*'}
    temptoken = json.loads(r.post(url, data=json.dumps(data), headers=headers).text)["token"]
    added_seconds = datetime.timedelta(0, int(json.loads(r.post(url, data=json.dumps(data), headers=headers).text)["expires"]))
    expiretime = datetime.datetime.now() + added_seconds
    return (temptoken, expiretime)