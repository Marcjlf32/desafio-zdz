export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',
  devtools: { enabled: true },
  ssr: false,

  modules: [
    'vuetify-nuxt-module',
    '@pinia/nuxt'
  ],

  vuetify: {
    moduleOptions: {
      styles: { configFile: 'assets/settings.scss' }
    },
    vuetifyOptions: {
      theme: {
        defaultTheme: 'light'
      }
    }
  },

  runtimeConfig: {
    public: {
      apiBaseUrl: 'http://localhost:5000'
    }
  },

  typescript: {
    strict: true,
    typeCheck: false
  }
})
