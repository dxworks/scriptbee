FROM nginx

ARG DIST_FOLDER="./dist/scriptbeewebapp"

COPY ${DIST_FOLDER} /etc/nginx/html/

COPY ./nginx/default.conf /etc/nginx/conf.d/default.conf
