import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin()],
    server: {
      port: 65088,
      proxy: {
        '/api': {
          target: 'https://localhost:7096/api/tasks',
          changeOrigin: true,
          rewrite: (path) => path.replace(/^\/api/, ''),
          secure: false
        },
      },
    }
})
