import axios, { InternalAxiosRequestConfig } from 'axios';
import { ClientIdService } from '../services/clientIdService';
import { connectionService } from '../services/connectionService';
import { authService } from '../services/authService';

const axiosInstance = axios.create();

axiosInstance.interceptors.request.use(async (config: InternalAxiosRequestConfig) => {
  config.headers = config.headers || {};
  config.headers['X-Client-Id'] = ClientIdService.clientId;

  if (!config.headers['Authorization']) {
    const activeConnection = await connectionService.getActiveConnection();
    if (activeConnection) {
      const token = await authService.getAccessToken(activeConnection);
      if (token) {
        config.headers['Authorization'] = `Bearer ${token}`;
      }
    }
  }

  return config;
});

export default axiosInstance;
