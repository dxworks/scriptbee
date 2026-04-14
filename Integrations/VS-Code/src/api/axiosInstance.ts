import axios from 'axios';
import { ClientIdService } from '../services/clientIdService';

const axiosInstance = axios.create();

axiosInstance.interceptors.request.use((config) => {
  config.headers = config.headers || {};
  config.headers['X-Client-Id'] = ClientIdService.clientId;
  return config;
});

export default axiosInstance;
