import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL

def forgot_password():
    url = f"{BASE_URL}/auth/forgot-password"
    body = {
        "email": "admin@example.com"
    }
    
    send_and_print(
        url=url,
        method="POST",
        body=body,
        headers={"Content-Type": "application/json"},
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

if __name__ == "__main__":
    forgot_password()