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

# Build (Vite reads from ENV, or falls back to .env.production)
RUN npm run build

# Runtime stage
FROM node:20-alpine
WORKDIR /app

RUN npm install -g serve
COPY --from=build /app/dist ./dist

EXPOSE 3001
CMD ["serve", "-s", "dist", "-l", "3001", "--no-clipboard"]
