import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL, load_config

def delete_user():
    token = load_config("access_token")
    if not token:
        print("No access token found.")
        return

    # Warning: Ensure this ID exists and is safe to delete.
    # Usually we delete the second user created (ID 2).
    user_id = 1
    url = f"{BASE_URL}/users/{user_id}"
    
    send_and_print(
        url=url,
        method="DELETE",
        headers={
            "Authorization": f"Bearer {token}"
        },
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

if __name__ == "__main__":
    delete_user()