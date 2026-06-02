import argparse
import base64
import hashlib
import hmac
import json
import time


def base64url_encode(raw: bytes) -> str:
    return base64.urlsafe_b64encode(raw).rstrip(b"=").decode("ascii")


def create_access_token(
    user_id: int,
    username: str,
    secret: str,
    expires_in_seconds: int,
) -> str:
    payload = {
        "sub": str(user_id),
        "username": username,
        "exp": int(time.time()) + expires_in_seconds,
    }
    payload_json = json.dumps(
        payload,
        separators=(",", ":"),
        sort_keys=True,
    ).encode("utf-8")
    encoded_payload = base64url_encode(payload_json)
    signature = hmac.new(
        secret.encode("utf-8"),
        encoded_payload.encode("ascii"),
        hashlib.sha256,
    ).digest()
    return f"{encoded_payload}.{base64url_encode(signature)}"


def main() -> None:
    parser = argparse.ArgumentParser(
        description="Generate a test access token compatible with backend/shared/security.py"
    )
    parser.add_argument("--secret", required=True, help="AUTH_TOKEN_SECRET used by backend")
    parser.add_argument("--user-id", type=int, default=9001)
    parser.add_argument("--username", default="k6-performance-user")
    parser.add_argument("--expires-in", type=int, default=86400)
    args = parser.parse_args()

    print(
        create_access_token(
            user_id=args.user_id,
            username=args.username,
            secret=args.secret,
            expires_in_seconds=args.expires_in,
        )
    )


if __name__ == "__main__":
    main()
