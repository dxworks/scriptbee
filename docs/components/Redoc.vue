<template>
  <div ref="container" class="redoc-container"></div>
</template>

<script setup>
import {nextTick, onBeforeUnmount, onMounted, ref, watch} from 'vue';

const props = defineProps({
  specUrl: {
    type: String,
    required: true,
  },
});

const container = ref(null);
let redocRoot;
let redocModule;
let themeObserver;

function isDark() {
  return document.documentElement.classList.contains('dark');
}

function loadRedoc() {
  redocModule ??= import('redoc/bundles/redoc.standalone.js');
  return redocModule;
}

async function renderRedoc() {
  if (!container.value) return;

  redocRoot?.unmount();
  container.value.innerHTML = '';

  const Redoc = await loadRedoc();

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

  redocRoot = Redoc.init(
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

  let observedDarkMode = isDark();
  themeObserver = new MutationObserver(async () => {
    const darkMode = isDark();
    if (darkMode === observedDarkMode) return;

    observedDarkMode = darkMode;
    await renderRedoc();
  });

  themeObserver.observe(document.documentElement, {
    attributes: true,
    attributeFilter: ['class'],
  });
});

onBeforeUnmount(() => {
  themeObserver?.disconnect();
  redocRoot?.unmount();
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
