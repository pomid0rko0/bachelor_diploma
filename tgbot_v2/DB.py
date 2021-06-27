import time
import requests
from requests.api import request
from requests.auth import AuthBase
import os
from datetime import datetime, timedelta

users = {}

class TokenAuth(AuthBase):
    """Implements a custom authentication scheme."""
 
    def __init__(self, auth_url, email, password):
        self.auth_url = auth_url
        self.auth_data = { "email": email, "password": password }
        self.auth_token = { "token": "", "expire_date": datetime.utcnow() }
 
    def __call__(self, r):
        """Attach an API token to a custom auth header."""
        token = self.auth_token["token"]
        expire_date = self.auth_token["expire_date"]
        while expire_date <= datetime.utcnow():
            try:
                response = requests.post(self.auth_url, json=self.auth_data).json()
                token = response["token"]
                expires = response["expires"]
                expire_date = datetime.utcnow() + timedelta(seconds=expires - 30)
            except Exception as e:
                print(e)
                time.sleep(10)
        self.auth_token = { "token": token, "expire_date": expire_date }
        r.headers["Authorization"] = f"Bearer {self.auth_token['token']}"
        return r
 
 


class Database:

    def __init__(self):
        self.db_host = os.environ["DB_API_HOST"]
        auth_url = self.db_host + "/AuthManagement/Login"
        email = os.environ["DB_API_USER"]
        password = os.environ["DB_API_PASSWORD"]
        self.auth = TokenAuth(auth_url, email, password)
        self.is_ready = False

    def request(self, method, endpoint, *args, **kwargs):
        while not self.is_ready:
            try:
                print(requests.get(self.db_host + "/Nlu/status").json())
                self.is_ready = True
            except Exception as e:
                print(e)
                time.sleep(10)
        
        response = requests.request(
            method=method, 
            url=self.db_host + endpoint, 
            auth=self.auth,
            *args,
            **kwargs,
        )
        print(response)
        print(response.text)
        try:
            return response.json()
        except Exception as e:
            print(e)
            return response.text

    def get(self, endpoint, *args, **kwargs):
        return self.request(method="GET", endpoint=endpoint, *args, **kwargs)

    def post(self, endpoint, *args, **kwargs):
        return self.request(method="POST", endpoint=endpoint, *args, **kwargs)

    def get_topics(self, offset=0, size=100):
        return self.get(endpoint="/Topics/get/all", params={ "offset": offset, "size": size })

    def topic_get_subotpics(self, topic_id):
        return self.get(f"/Topics/get/{topic_id}/subtopics")

    def subtopic_get_topic(self, subtopic_id):
        return self.get(f"/Subtopics/get/{subtopic_id}/topic")

    def subtopic_get_questions(self, subtopic_id):
        return self.get(f"/Subtopics/get/{subtopic_id}/ui_questions")

    def question_get_question(self, question_id):
        return self.get(f"/Questions/get/{question_id}")

    def question_get_answer(self, question_id):
        return self.get(f"/Questions/get/{question_id}/answer")

    def question_get_subtopic(self, question_id):
        return self.get(f"/Questions/get/{question_id}/subtopic")

    def parse(self, sender, message):
        return "\n".join(r["text"] for r in self.post(endpoint=f"/Nlu/parse", json={ "sender": sender, "message": message }))
