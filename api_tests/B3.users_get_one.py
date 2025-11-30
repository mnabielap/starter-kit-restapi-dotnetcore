import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL, load_config

def get_user():
    token = load_config("access_token")
    if not token:
        print("No access token found.")
        return

    # Assuming ID 1 exists (usually the first registered admin)
    user_id = 1
    url = f"{BASE_URL}/users/{user_id}"
    
    send_and_print(
        url=url,
        method="GET",
        headers={
            "Authorization": f"Bearer {token}"
        },
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

if __name__ == "__main__":
    get_user()