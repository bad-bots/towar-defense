FROM node:10.15.1-alpine

WORKDIR /usr/src/app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install
RUN npm install --production

# Copy app soruce
COPY . .

EXPOSE 8080

# Set user

CMD ["npm","run", "prod"]