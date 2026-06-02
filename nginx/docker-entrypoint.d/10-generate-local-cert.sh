#!/bin/sh
set -eu

mkdir -p /etc/nginx/certs

if [ -f /etc/nginx/certs/local.crt ] && [ -f /etc/nginx/certs/local.key ]; then
    exit 0
fi

cat >/tmp/local-cert.conf <<'EOF'
[req]
default_bits = 2048
prompt = no
default_md = sha256
distinguished_name = dn
x509_extensions = v3_req

[dn]
CN = localhost

[v3_req]
subjectAltName = @alt_names

[alt_names]
DNS.1 = localhost
IP.1 = 127.0.0.1
EOF

openssl req -x509 -nodes -days 3650 -newkey rsa:2048 \
    -keyout /etc/nginx/certs/local.key \
    -out /etc/nginx/certs/local.crt \
    -config /tmp/local-cert.conf
