FROM node:10.15.1-alpine

# Create user
USER ubuntu

# Copy package.json and package-lock.json
COPY package*.json ./

# Install
RUN npm install --production

# Copy app soruce
Copy . .

# Clean up
RUN rm "./env.production"

EXPOSE 8080

CMD ["node", "index.js"]