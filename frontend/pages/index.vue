<template>
  <v-row justify="center" align="center" class="text-center">
    <v-col cols="12" md="8">
      <h1 class="text-h3 mb-4">Plataforma de Inventário Comercial</h1>
      <p class="text-body-1 mb-6">
        Gerencie categorias e produtos com integridade referencial garantida.
      </p>
      <v-row justify="center">
        <v-col cols="12" sm="5">
          <v-card to="/categorias" hover>
            <v-card-item>
              <v-icon size="48" color="primary">mdi-shape</v-icon>
              <v-card-title>Categorias</v-card-title>
              <v-card-subtitle>Cadastro e manutenção de categorias</v-card-subtitle>
            </v-card-item>
          </v-card>
        </v-col>
        <v-col cols="12" sm="5">
          <v-card to="/produtos" hover>
            <v-card-item>
              <v-icon size="48" color="primary">mdi-package-variant</v-icon>
              <v-card-title>Produtos</v-card-title>
              <v-card-subtitle>Cadastro e manutenção de produtos</v-card-subtitle>
            </v-card-item>
          </v-card>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>
