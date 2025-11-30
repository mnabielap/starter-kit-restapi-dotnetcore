import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL, load_config

def update_user():
    token = load_config("access_token")
    if not token:
        print("No access token found.")
        return

    user_id = 3
    url = f"{BASE_URL}/users/{user_id}"
    body = {
        "name": "Updated User Name"
    }
    
    send_and_print(
        url=url,
        method="PATCH",
        body=body,
        headers={
            "Content-Type": "application/json",
            "Authorization": f"Bearer {token}"
        },
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

if __name__ == "__main__":
    update_user()