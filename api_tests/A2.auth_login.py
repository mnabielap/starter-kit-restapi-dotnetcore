import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL, save_config

def login():
    url = f"{BASE_URL}/auth/login"
    body = {
        "email": "admin@example.com",
        "password": "Password123!"
    }
    
    response = send_and_print(
        url=url,
        method="POST",
        body=body,
        headers={"Content-Type": "application/json"},
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

    if response.status_code == 200:
        data = response.json()
        tokens = data.get("tokens", {})
        
        if tokens:
            save_config("access_token", tokens["access"]["token"])
            save_config("refresh_token", tokens["refresh"]["token"])
            print("\n[SUCCESS] Login successful. Tokens updated in secrets.json")

if __name__ == "__main__":
    login()