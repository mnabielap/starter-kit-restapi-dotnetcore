import sys
import os
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from utils import send_and_print, BASE_URL, save_config

def register_admin():
    url = f"{BASE_URL}/auth/register"
    body = {
        "name": "Super Admin",
        "email": "admin@example.com",
        "password": "Password123!" # Meets complexity requirements
    }
    
    # Note: The .NET implementation sets Role='User' by default in AuthService.RegisterAsync.
    # To get an admin, you might need to manually update the DB or modify the code 
    # if the registration endpoint doesn't accept a 'role' parameter (the DTO doesn't have it).
    # Based on the provided C# code, RegisterRequest only has Name, Email, Password.
    
    response = send_and_print(
        url=url,
        method="POST",
        body=body,
        headers={"Content-Type": "application/json"},
        output_file=f"{os.path.splitext(os.path.basename(__file__))[0]}.json"
    )

    if response.status_code == 201:
        data = response.json()
        tokens = data.get("tokens", {})
        
        # Save tokens for subsequent requests
        if tokens:
            save_config("access_token", tokens["access"]["token"])
            save_config("refresh_token", tokens["refresh"]["token"])
            print("\n[SUCCESS] Tokens saved to secrets.json")
    else:
        print("\n[ERROR] Registration failed.")

if __name__ == "__main__":
    register_admin()