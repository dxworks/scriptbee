---
layout: page
---

<script setup>

const specUrl = import.meta.env.BASE_URL + 'analysis_swagger.json'
</script>

<ClientOnly>
  <Redoc :specUrl="specUrl" />
</ClientOnly>
