import json
import requests as r

users = {}

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
    url = 'http://217.71.129.139:4500/AuthManagement/Login'
    data = {'email': 'legalov.2017@stud.nstu.ru', 'password': '~biSXA7SSU]pwI0x&1f'}
    headers = {'Content-type': 'application/json', 'Accept': '*/*'}
    temptoken = json.loads(r.post(url, data=json.dumps(data), headers=headers).text)["token"]
    return(temptoken)