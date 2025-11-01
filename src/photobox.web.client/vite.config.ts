import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    server: {
        port: 60005,
      proxy: {
        // Proxy all API requests starting with /api to the backend
        '/api': {
          target: 'https://localhost', // your backend origin
          changeOrigin: true,
          secure: false, // allow self-signed certs if using local HTTPS
        },
      }
    }
})
