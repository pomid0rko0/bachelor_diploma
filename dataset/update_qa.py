import requests
import pandas as pd
import exrex
import os
from dotenv import load_dotenv

load_dotenv("../.env")

host = "http://localhost:5005"

data = pd.read_csv("./(with_examples)qa.csv", sep=";", names=["topic", "subtopic", "answer", "question", "regex_question"])

print(requests.get(f"{host}/AuthManagement/RegisterFirst/"))

token = requests.post(f"{host}/AuthManagement/Login/", json={
    "email": os.environ["FIRST_EMAIL"],
    "password": os.environ["FIRST_PASSWORD"]
}).json()["token"]

for X in ["Questions", "Answers", "Subtopics", "Topics"]:
    while True:
        es = requests.get(f"{host}/{X}/get/all", params={ "offset": 0, "size": 1000 }, headers={ "Authorization": f'Bearer {token}' }).json()
        if len(es) == 0:
            break
        for e in es:
            requests.delete(f"{host}/{X}/delete/{e['id']}", headers={ "Authorization": f'Bearer {token}' })

prev_topic = ""
prev_subtopic = ""

topic_id = None
subtopic_id = None

for index, row in data.iterrows():
    print(index)
    topic = row["topic"]
    subtopic = row["subtopic"]
    answer = row["answer"]
    question = row["question"]
    regex_question = row["regex_question"]
    if topic != prev_topic:
        topic_id = requests.post(f"{host}/Topics/add", json=topic, headers={ "Authorization": f'Bearer {token}' }).json()["id"]
        prev_topic = topic
    if subtopic != prev_subtopic:
        subtopic_id = requests.post(f"{host}/Subtopics/add",  params={"topicId": topic_id }, json=subtopic, headers={ "Authorization": f'Bearer {token}' }).json()["id"]
        prev_subtopic = subtopic
    questions = set(exrex.generate(regex_question))
    print("generated questions:", len(questions))
    try:
        answer_id = requests.post(f"{host}/Answers/add", json={ "text": answer, "url": "https://ciu.nstu.ru/enrollee_account/answers" }, headers={ "Authorization": f'Bearer {token}' }).json()["id"]
        requests.post(f"{host}/Questions/add",  params={"answerId": answer_id, "subtopicId": subtopic_id, "isUiQuestion": True }, json=question, headers={ "Authorization": f'Bearer {token}' })
        for q in questions:
            requests.post(f"{host}/Questions/add",  params={"answerId": answer_id, "subtopicId": subtopic_id, "isUiQuestion": False }, json=q, headers={ "Authorization": f'Bearer {token}' })
    except:
        pass
