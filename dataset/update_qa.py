from typing import Set
import requests
import pandas as pd
import rstr

# host = "http://217.71.129.139:4500"
host = "http://localhost:5005"

data = pd.read_csv("./(with_examples)qa.csv", sep=";", names=["topic", "subtopic", "answer", "question", "regex_question"])

token = requests.post(f"{host}/AuthManagement/Login/", json={
  "email": "sychev.2017@stud.nstu.ru",
  "password": "(sdHcmPT|4j1!3=K3t[T[?GgQ4d8"
}).json()["token"]

for X in ["Questions", "Answers", "Subtopics", "Topics"]:
    es = requests.get(f"{host}/{X}/get/all", params={ "offset": 0, "size": 1000 }, headers={ "Authorization": f'Bearer {token}' }).json()
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
    response = None
    if topic != prev_topic:
        topic_id = requests.post(f"{host}/Topics/add", json=topic, headers={ "Authorization": f'Bearer {token}' }).json()["id"]
        prev_topic = topic
    if subtopic != prev_subtopic:
        subtopic_id = requests.post(f"{host}/Subtopics/add",  params={"topicId": topic_id }, json=subtopic, headers={ "Authorization": f'Bearer {token}' }).json()["id"]
        prev_subtopic = subtopic
    questions = set()
    for _ in range(100_000):
        questions.add(rstr.xeger(regex_question))
    print("generated questions:", len(questions))
    try:
        answer_id = requests.post(f"{host}/Answers/add", json={ "text": answer, "url": "https://ciu.nstu.ru/enrollee_account/answers" }, headers={ "Authorization": f'Bearer {token}' }).json()["id"]
        requests.post(f"{host}/Questions/add",  params={"answerId": answer_id, "subtopicId": subtopic_id, "isUiQuestion": True }, json=question, headers={ "Authorization": f'Bearer {token}' })
        for q in questions:
            requests.post(f"{host}/Questions/add",  params={"answerId": answer_id, "subtopicId": subtopic_id, "isUiQuestion": False }, json=q, headers={ "Authorization": f'Bearer {token}' })
    except:
        pass
