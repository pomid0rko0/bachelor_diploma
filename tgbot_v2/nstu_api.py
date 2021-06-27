import os
import requests

class NstuApi:

    def __init__(self):
        self.host = os.environ["NSTU_API_HOST"]

    def request(self, method, endpoint, *args, **kwargs):
        return requests.request(
            method=method, 
            url=self.host + endpoint, 
            *args,
            **kwargs,
        ).json()

    def get(self, endpoint, *args, **kwargs):
        return self.request(method="GET", endpoint=endpoint, *args, **kwargs)

    def check_abit(self, id):
        return self.get("/v1.0/api/abit_bot/check_abit", params={ "id": id })
