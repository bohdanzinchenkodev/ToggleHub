// src/config/env.js
class EnvConfig {
  constructor() {
    // Use Vite's environment variable handling
    this.API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5160';
  }

  getApiUrl() {
    return this.API_URL.replace(/\/$/, ''); // Remove trailing slash
  }
}

export const envConfig = new EnvConfig();
export default envConfig;
