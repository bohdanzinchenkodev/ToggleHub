# Build stage
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

# Copy source
COPY . ./

# Build-time config for Vite
ARG VITE_API_URL
ENV VITE_API_URL=${VITE_API_URL}

# Build 
RUN npm run build

# Runtime stage
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx","-g","daemon off;"]
