import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from "node:path";
import vuetify from 'vite-plugin-vuetify';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vuetify({autoImport: true}),
  ],
  server: {
      port: 60005,
      proxy: {
        // Proxy all API requests starting with /api to the backend
        '/api': {
          target: 'https://localhost', // your backend origin
          changeOrigin: true,
          secure: false, // allow self-signed certs if using local HTTPS
        },
        '/users': {
          target: 'https://localhost',
          changeOrigin: true,
          secure: false,
        },
      }
    },
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src'), // <-- this makes '@' point to /src
      },
    },
})
