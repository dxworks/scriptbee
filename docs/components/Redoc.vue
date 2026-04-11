<template>
  <div ref="container" class="redoc-container"></div>
</template>

<script setup>
import { ref, onMounted, watch, nextTick } from 'vue';

const props = defineProps({
  specUrl: {
    type: String,
    required: true,
  },
});

const container = ref(null);

function isDark() {
  return document.documentElement.classList.contains('dark');
}

function loadRedocScript() {
  return new Promise((resolve, reject) => {
    if (window.Redoc) return resolve(window.Redoc);

    const script = document.createElement('script');
    script.src = 'https://cdn.jsdelivr.net/npm/redoc@latest/bundles/redoc.standalone.js';
    script.async = true;

    script.onload = () => resolve(window.Redoc);
    script.onerror = reject;

    document.head.appendChild(script);
  });
}

async function renderRedoc() {
  if (!container.value) return;

  container.value.innerHTML = '';

  const Redoc = await loadRedocScript();

  const theme = isDark()
    ? {
        colors: {
          primary: { main: '#60a5fa' },
          text: { primary: '#e5e7eb' },
          http: {
            get: '#4ade80',
            post: '#60a5fa',
            put: '#facc15',
            delete: '#f87171',
          },
          responses: {
            success: { color: '#4ade80' },
            error: { color: '#f87171' },
          },
          background: { main: '#0f172a' },
        },
        typography: {
          fontSize: '14px',
          lineHeight: '1.6em',
        },
      }
    : {};

  Redoc.init(
    props.specUrl,
    {
      scrollYOffset: 60,
      hideDownloadButton: false,
      disableSearch: false,
      theme,
    },
    container.value,
  );
}

onMounted(async () => {
  await renderRedoc();

  // react to dark mode changes
  const observer = new MutationObserver(async () => {
    await renderRedoc();
  });

  observer.observe(document.documentElement, {
    attributes: true,
    attributeFilter: ['class'],
  });
});

watch(
  () => props.specUrl,
  async () => {
    await nextTick();
    await renderRedoc();
  },
);
</script>

<style>
.redoc-container {
  width: 100%;
  min-height: 80vh;
}
</style>
