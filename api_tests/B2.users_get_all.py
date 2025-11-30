import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL, load_config

def get_all_users():
    token = load_config("access_token")
    if not token:
        print("No access token found.")
        return

    # Query params: limit=10, page=1
    url = f"{BASE_URL}/users?limit=10&page=1"
    
    send_and_print(
        url=url,
        method="GET",
        headers={
            "Authorization": f"Bearer {token}"
        },
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

if __name__ == "__main__":
    get_all_users()