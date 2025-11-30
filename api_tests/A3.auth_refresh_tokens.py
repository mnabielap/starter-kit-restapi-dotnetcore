import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL, load_config, save_config

def refresh_tokens():
    refresh_token = load_config("refresh_token")
    if not refresh_token:
        print("No refresh token found in secrets.json. Please login first.")
        return

    url = f"{BASE_URL}/auth/refresh-tokens"
    body = {
        "refreshToken": refresh_token
    }
    
    response = send_and_print(
        url=url,
        method="POST",
        body=body,
        headers={"Content-Type": "application/json"},
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

    if response.status_code == 200:
        tokens = response.json() # Direct TokenDTO response
        if tokens:
            save_config("access_token", tokens["access"]["token"])
            save_config("refresh_token", tokens["refresh"]["token"])
            print("\n[SUCCESS] Tokens refreshed and updated.")

if __name__ == "__main__":
    refresh_tokens()