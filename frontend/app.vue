<template>
  <v-app>
    <v-app-bar color="primary" density="comfortable" flat>
      <v-app-bar-title>Catálogo Comercial</v-app-bar-title>
      <v-tabs align-tabs="end">
        <v-tab to="/categorias" prepend-icon="mdi-shape">Categorias</v-tab>
        <v-tab to="/produtos" prepend-icon="mdi-package-variant">Produtos</v-tab>
      </v-tabs>
    </v-app-bar>

    <v-main>
      <v-container fluid class="pa-6">
        <NuxtPage />
      </v-container>
    </v-main>

    <ErrorToast />
  </v-app>
</template>
