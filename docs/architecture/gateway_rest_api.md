---
layout: page
---

<script setup>

const specUrl = import.meta.env.BASE_URL + 'gateway_swagger.json'
</script>

<ClientOnly>
  <Redoc :specUrl="specUrl" />
</ClientOnly>
