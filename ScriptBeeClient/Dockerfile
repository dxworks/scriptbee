FROM nginx AS base
EXPOSE 80

FROM node:16 as build

WORKDIR /src

COPY ./package*.json ./

RUN npm install

COPY ./src ./src
COPY ./angular.json .
COPY ./tsconfig*.json ./
COPY ./webpack*.config.js ./

RUN npm run build-prod

FROM base as final

COPY ./nginx/default.conf /etc/nginx/conf.d/default.conf

COPY --from=build /src/dist/scriptbeewebapp /etc/nginx/html/
