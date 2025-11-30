import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL

def reset_password():
    # PLACEHOLDER: You must manually replace this with a valid token from your DB
    # or implement a way to fetch it if you are testing locally.
    token_from_db = "REPLACE_WITH_TOKEN_FROM_DB" 
    
    url = f"{BASE_URL}/auth/reset-password"
    body = {
        "token": token_from_db,
        "password": "NewPassword123!"
    }
    
    send_and_print(
        url=url,
        method="POST",
        body=body,
        headers={"Content-Type": "application/json"},
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

if __name__ == "__main__":
    reset_password()