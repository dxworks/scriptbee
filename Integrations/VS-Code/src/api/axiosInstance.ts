import axios from 'axios';
import { connectionService } from '../services/connectionService';

const axiosInstance = axios.create();

axiosInstance.interceptors.request.use(
  async (config) => {
    const activeConnection = await connectionService.getActiveConnection();
    if (activeConnection) {
      console.log('Active connection:', activeConnection);
      config.baseURL = activeConnection.url;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default axiosInstance;
