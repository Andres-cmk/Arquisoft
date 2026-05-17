FROM nginx:1.27-alpine

RUN apk add --no-cache openssl

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY docker-entrypoint.d/10-generate-local-cert.sh /docker-entrypoint.d/10-generate-local-cert.sh

RUN chmod +x /docker-entrypoint.d/10-generate-local-cert.sh
