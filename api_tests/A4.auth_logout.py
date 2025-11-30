import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL, load_config

def logout():
    refresh_token = load_config("refresh_token")
    if not refresh_token:
        print("No refresh token found. Already logged out or never logged in.")
        return

    url = f"{BASE_URL}/auth/logout"
    body = {
        "refreshToken": refresh_token
    }
    
    send_and_print(
        url=url,
        method="POST",
        body=body,
        headers={"Content-Type": "application/json"},
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

if __name__ == "__main__":
    logout()