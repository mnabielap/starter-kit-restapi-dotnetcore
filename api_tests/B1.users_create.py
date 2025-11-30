import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL, load_config

def create_user():
    token = load_config("access_token")
    if not token:
        print("No access token found.")
        return

    url = f"{BASE_URL}/users"
    body = {
        "name": "Regular User",
        "email": "user@example.com",
        "password": "UserPass123!",
        "role": "user"
    }
    
    send_and_print(
        url=url,
        method="POST",
        body=body,
        headers={
            "Content-Type": "application/json",
            "Authorization": f"Bearer {token}"
        },
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

if __name__ == "__main__":
    create_user()