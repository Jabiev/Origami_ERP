const state = {
  apiBase: localStorage.getItem("origamiApiBase") || "http://localhost:5102/api",
  resourcesTab: "books",
};

const els = {
  apiForm: document.querySelector("#api-config"),
  apiBaseInput: document.querySelector("#apiBase"),
  apiStatus: document.querySelector("#apiStatus"),
  overviewGrid: document.querySelector("#overviewGrid"),
  signatureModels: document.querySelector("#signatureModels"),
  recognitionExample: document.querySelector("#recognitionExample"),
  featuredCreators: document.querySelector("#featuredCreators"),
  recognitionForm: document.querySelector("#recognitionForm"),
  recognitionFile: document.querySelector("#recognitionFile"),
  recognitionStatus: document.querySelector("#recognitionStatus"),
  recognitionResults: document.querySelector("#recognitionResults"),
  difficultyChart: document.querySelector("#difficultyChart"),
  topCreatorsChart: document.querySelector("#topCreatorsChart"),
  techniqueGrid: document.querySelector("#techniqueGrid"),
  imageCoverage: document.querySelector("#imageCoverage"),
  catalogForm: document.querySelector("#catalogForm"),
  catalogResults: document.querySelector("#catalogResults"),
  catalogDetails: document.querySelector("#catalogDetails"),
  creatorForm: document.querySelector("#creatorForm"),
  creatorResults: document.querySelector("#creatorResults"),
  creatorDetails: document.querySelector("#creatorDetails"),
  resourceResults: document.querySelector("#resourceResults"),
  resourceTabs: [...document.querySelectorAll(".tab")],
  useSampleImage: document.querySelector("#useSampleImage"),
  metricBarTemplate: document.querySelector("#metricBarTemplate"),
};

function setApiStatus(message, tone = "") {
  els.apiStatus.textContent = message;
  els.apiStatus.className = `api-status ${tone}`.trim();
}

function formatNumber(value) {
  return new Intl.NumberFormat().format(value || 0);
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;");
}

function normalizeBaseUrl(value) {
  return value.trim().replace(/\/+$/, "");
}

async function fetchJson(path, options) {
  const response = await fetch(`${state.apiBase}${path}`, options);

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `${response.status} ${response.statusText}`);
  }

  return response.json();
}

function renderOverview(overview) {
  const items = [
    ["Models", overview?.models],
    ["Creators", overview?.creators],
    ["Images", overview?.images],
    ["Publications", overview?.publications],
    ["ORC models", overview?.orcModels],
    ["CFC resources", overview?.cfcResources],
  ];

  els.overviewGrid.innerHTML = items
    .map(([label, value]) => `
      <article class="stat-card">
        <strong>${formatNumber(value)}</strong>
        <span>${label}</span>
      </article>
    `)
    .join("");
}

function renderSignatureModels(models = []) {
  if (!models.length) {
    els.signatureModels.innerHTML = `<div class="empty-state">No signature models were returned.</div>`;
    return;
  }

  els.signatureModels.innerHTML = models
    .map((model) => `
      <article class="showcase-card">
        ${model.primaryImageUrl ? `<img src="${escapeHtml(model.primaryImageUrl)}" alt="${escapeHtml(model.modelNameOriginal)}">` : ""}
        <h4>${escapeHtml(model.modelNameOriginal)}</h4>
        <p class="card-subtitle">${escapeHtml(model.creatorName || "Unknown creator")}</p>
        <div class="detail-meta">
          <span class="meta-pill">${escapeHtml(model.difficulty || "Unrated")}</span>
          <span class="meta-pill">${formatNumber(model.imageCount)} images</span>
        </div>
      </article>
    `)
    .join("");
}

function renderRecognitionExample(predictions = []) {
  if (!predictions.length) {
    els.recognitionExample.innerHTML = `<div class="empty-state">Recognition sample predictions are unavailable.</div>`;
    return;
  }

  els.recognitionExample.innerHTML = predictions
    .map((prediction) => `
      <div class="prediction">
        <strong>
          <span>${escapeHtml(prediction.label)}</span>
          <span>${Math.round((prediction.confidence || 0) * 100)}%</span>
        </strong>
      </div>
    `)
    .join("");
}

function renderFeaturedCreators(creators = []) {
  if (!creators.length) {
    els.featuredCreators.innerHTML = `<div class="empty-state">Featured creators are unavailable.</div>`;
    return;
  }

  els.featuredCreators.innerHTML = creators
    .map((creator) => `
      <article class="list-card">
        <h4>${escapeHtml(creator.nameOriginal)}</h4>
        <p class="card-meta">${formatNumber(creator.modelCount)} models</p>
      </article>
    `)
    .join("");
}

function renderMetricBars(container, items, labelKey, valueKey) {
  if (!items?.length) {
    container.innerHTML = `<div class="empty-state">No data returned.</div>`;
    return;
  }

  const max = Math.max(...items.map((item) => item[valueKey] || 0), 1);
  container.innerHTML = "";

  items.forEach((item) => {
    const node = els.metricBarTemplate.content.firstElementChild.cloneNode(true);
    node.querySelector(".metric-label").textContent = item[labelKey];
    node.querySelector(".metric-value").textContent = formatNumber(item[valueKey]);
    node.querySelector(".metric-fill").style.width = `${((item[valueKey] || 0) / max) * 100}%`;
    container.appendChild(node);
  });
}

function renderTechniqueCloud(items = []) {
  if (!items.length) {
    els.techniqueGrid.innerHTML = `<div class="empty-state">Technique data unavailable.</div>`;
    return;
  }

  els.techniqueGrid.innerHTML = items
    .map((item) => `<span class="technique-chip">${escapeHtml(item.name)} - ${formatNumber(item.count)}</span>`)
    .join("");
}

function renderImageCoverage(coverage) {
  if (!coverage) {
    els.imageCoverage.innerHTML = `<div class="empty-state">Image coverage data unavailable.</div>`;
    return;
  }

  const withImage = coverage.withImage || 0;
  const withoutImage = coverage.withoutImage || 0;
  const total = withImage + withoutImage;
  const ratio = total ? Math.round((withImage / total) * 100) : 0;

  els.imageCoverage.innerHTML = `
    <div class="coverage-ring" style="--coverage:${ratio}%;">
      <strong>${ratio}%</strong>
    </div>
    <div class="detail-meta">
      <span class="meta-pill">${formatNumber(withImage)} with image</span>
      <span class="meta-pill">${formatNumber(withoutImage)} without image</span>
    </div>
  `;
}

function renderCatalogResults(items = []) {
  if (!items.length) {
    els.catalogResults.innerHTML = `<div class="empty-state">No models matched the current filters.</div>`;
    return;
  }

  els.catalogResults.innerHTML = items
    .map((model) => `
      <article class="catalog-card">
        ${model.primaryImageUrl ? `<img class="catalog-thumb" src="${escapeHtml(model.primaryImageUrl)}" alt="${escapeHtml(model.modelNameOriginal)}">` : `<div class="catalog-thumb"></div>`}
        <div>
          <h4>${escapeHtml(model.modelNameOriginal)}</h4>
          <p class="card-meta">${escapeHtml(model.creatorName || "Unknown creator")}</p>
          <div class="detail-meta">
            <span class="meta-pill">${escapeHtml(model.difficulty || "Unrated")}</span>
            <span class="meta-pill">${escapeHtml(model.paperShape || "Paper shape unknown")}</span>
          </div>
        </div>
        <button class="button button-secondary" type="button" data-model-id="${model.modelId}">Details</button>
      </article>
    `)
    .join("");
}

function renderCatalogDetails(model) {
  if (!model) {
    els.catalogDetails.innerHTML = `<div class="empty-state">Model details could not be loaded.</div>`;
    return;
  }

  const images = (model.images || [])
    .slice(0, 4)
    .map((image) => `<img src="${escapeHtml(image.cloudinaryUrl || image.url || "")}" alt="${escapeHtml(model.modelNameOriginal)}">`)
    .join("");

  const publications = (model.publications || [])
    .slice(0, 5)
    .map((publication) => `
      <div class="details-item">
        <strong>${escapeHtml(publication.title)}</strong>
        <p class="details-copy">${escapeHtml(publication.type || "Publication")} ${publication.year ? `- ${publication.year}` : ""}</p>
      </div>
    `)
    .join("");

  els.catalogDetails.innerHTML = `
    <h3>${escapeHtml(model.modelNameOriginal)}</h3>
    <p class="details-copy">${escapeHtml(model.creatorName || "Unknown creator")}</p>
    <div class="details-tags">
      <span class="meta-pill">${escapeHtml(model.difficulty || "Unrated")}</span>
      <span class="meta-pill">${escapeHtml(model.paperShape || "Paper shape unknown")}</span>
      <span class="meta-pill">${model.pieces ?? "?"} pieces</span>
      <span class="meta-pill">${model.usesCutting ? "Uses cutting" : "No cutting"}</span>
      <span class="meta-pill">${model.usesGlue ? "Uses glue" : "No glue"}</span>
    </div>
    ${images ? `<div class="details-image-grid">${images}</div>` : ""}
    <div class="details-list">${publications || `<div class="empty-state">No publications returned for this model.</div>`}</div>
  `;
}

function renderCreatorResults(items = []) {
  if (!items.length) {
    els.creatorResults.innerHTML = `<div class="empty-state">No creators matched the search.</div>`;
    return;
  }

  els.creatorResults.innerHTML = items
    .map((creator) => `
      <article class="list-card">
        <h4>${escapeHtml(creator.nameOriginal)}</h4>
        <p class="card-meta">${escapeHtml(creator.country || "Country unknown")} - ${formatNumber(creator.modelCount)} models</p>
        <button class="button button-secondary" type="button" data-creator-id="${creator.creatorId}">Open profile</button>
      </article>
    `)
    .join("");
}

function renderCreatorDetails(creator) {
  if (!creator) {
    els.creatorDetails.innerHTML = `<div class="empty-state">Creator details could not be loaded.</div>`;
    return;
  }

  const aliases = (creator.aliases || [])
    .map((alias) => `<span class="meta-pill">${escapeHtml(alias.alias)}</span>`)
    .join("");

  const models = (creator.models || [])
    .slice(0, 6)
    .map((model) => `
      <div class="details-item">
        <strong>${escapeHtml(model.modelNameOriginal)}</strong>
        <p class="details-copy">${escapeHtml(model.difficulty || "Unrated")}</p>
      </div>
    `)
    .join("");

  els.creatorDetails.innerHTML = `
    <h3>${escapeHtml(creator.nameOriginal)}</h3>
    <p class="details-copy">${escapeHtml(creator.biography || "No biography available.")}</p>
    <div class="details-tags">
      <span class="meta-pill">${escapeHtml(creator.country || "Country unknown")}</span>
      <span class="meta-pill">${escapeHtml(creator.language || "Language unknown")}</span>
      ${creator.birthYear ? `<span class="meta-pill">Born ${creator.birthYear}</span>` : ""}
      ${creator.deathYear ? `<span class="meta-pill">Died ${creator.deathYear}</span>` : ""}
    </div>
    ${aliases ? `<div class="details-tags">${aliases}</div>` : ""}
    <div class="details-list">${models || `<div class="empty-state">No models returned for this creator.</div>`}</div>
  `;
}

function renderResourceCards(items = [], type) {
  if (!items.length) {
    els.resourceResults.innerHTML = `<div class="empty-state">No ${type} returned.</div>`;
    return;
  }

  els.resourceResults.innerHTML = items
    .map((item) => {
      const image = item.cloudinaryUrl || item.imageUrl;
      const summary = item.summary || item.author || item.creator || item.category || "Resource";
      const meta = item.publishedDate || item.postedOn || item.updatedDate || item.submissionDeadline || item.difficulty || "";

      return `
        <article class="resource-card">
          ${image ? `<img src="${escapeHtml(image)}" alt="${escapeHtml(item.title)}">` : ""}
          <h4>${escapeHtml(item.title)}</h4>
          <p>${escapeHtml(summary || "")}</p>
          ${meta ? `<div class="detail-meta"><span class="meta-pill">${escapeHtml(meta)}</span></div>` : ""}
          ${item.url ? `<p><a href="${escapeHtml(item.url)}" target="_blank" rel="noreferrer">Open source</a></p>` : ""}
        </article>
      `;
    })
    .join("");
}

function renderRecognitionResponse(result) {
  const predictions = result?.predictions || [];
  const matched = result?.matchedModels || [];

  const predictionHtml = predictions.length
    ? predictions.map((prediction) => `
        <div class="prediction">
          <strong>
            <span>${escapeHtml(prediction.label)}</span>
            <span>${Math.round((prediction.confidence || 0) * 100)}%</span>
          </strong>
        </div>
      `).join("")
    : `<div class="empty-state">No predictions returned.</div>`;

  const matchedHtml = matched.length
    ? matched.map((model) => `
        <article class="matched-card">
          <h4>${escapeHtml(model.modelNameOriginal)}</h4>
          <p class="card-meta">${escapeHtml(model.creatorName || "Unknown creator")}</p>
          <div class="detail-meta">
            <span class="meta-pill">${escapeHtml(model.difficulty || "Unrated")}</span>
            <span class="meta-pill">${escapeHtml(model.paperShape || "Paper shape unknown")}</span>
          </div>
        </article>
      `).join("")
    : `<div class="empty-state">No matched catalog models returned.</div>`;

  els.recognitionResults.innerHTML = `
    <div class="prediction-stack">${predictionHtml}</div>
    <div class="details-list">${matchedHtml}</div>
  `;
}

async function loadHeroData() {
  const [showcase, featured] = await Promise.all([
    fetchJson("/showcase/hero?limit=6"),
    fetchJson("/discovery/featured?limit=6"),
  ]);

  renderOverview(showcase.overview);
  renderSignatureModels(showcase.signatureModels);
  renderRecognitionExample(showcase.recognitionExample);
  renderFeaturedCreators(featured.topCreators);
}

async function loadAnalytics() {
  const [landscape, creators] = await Promise.all([
    fetchJson("/analytics/landscape"),
    fetchJson("/analytics/creators"),
  ]);

  renderMetricBars(els.difficultyChart, landscape.difficultyDistribution, "label", "count");
  renderMetricBars(els.topCreatorsChart, creators.topCreators, "name", "count");
  renderTechniqueCloud(landscape.techniqueUsage);
  renderImageCoverage(landscape.imageCoverage);
}

async function loadCatalog(query = "") {
  const params = new URLSearchParams();
  const formData = new FormData(els.catalogForm);

  for (const [key, value] of formData.entries()) {
    if (value) {
      params.set(key, value);
    }
  }

  if (query) {
    params.set("query", query);
  }

  params.set("limit", "12");

  const result = await fetchJson(`/catalog/models?${params.toString()}`);
  renderCatalogResults(result.items || []);
}

async function loadCatalogDetails(modelId) {
  els.catalogDetails.classList.add("is-loading");

  try {
    const model = await fetchJson(`/catalog/models/${modelId}`);
    renderCatalogDetails(model);
  } finally {
    els.catalogDetails.classList.remove("is-loading");
  }
}

async function loadCreators(query = "") {
  const params = new URLSearchParams();
  if (query) {
    params.set("query", query);
  }
  params.set("limit", "12");

  const result = await fetchJson(`/creators?${params.toString()}`);
  renderCreatorResults(result.items || []);
}

async function loadCreatorDetails(creatorId) {
  els.creatorDetails.classList.add("is-loading");

  try {
    const creator = await fetchJson(`/creators/${creatorId}`);
    renderCreatorDetails(creator);
  } finally {
    els.creatorDetails.classList.remove("is-loading");
  }
}

async function loadResources(type = state.resourcesTab) {
  const result = await fetchJson(`/resources/${type}?limit=9`);
  renderResourceCards(result.items || [], type);
}

async function refreshAll() {
  els.apiBaseInput.value = state.apiBase;
  setApiStatus(`Connecting to ${state.apiBase}`, "muted");

  try {
    await Promise.all([
      loadHeroData(),
      loadAnalytics(),
      loadCatalog(),
      loadCreators(),
      loadResources(),
    ]);
    setApiStatus(`Live data loaded from ${state.apiBase}`, "status-ok");
  } catch (error) {
    setApiStatus(`Live API unavailable at ${state.apiBase}. The UI is ready, but data requests failed.`, "status-error");
    console.error(error);
  }
}

els.apiForm.addEventListener("submit", async (event) => {
  event.preventDefault();
  state.apiBase = normalizeBaseUrl(els.apiBaseInput.value);
  localStorage.setItem("origamiApiBase", state.apiBase);
  await refreshAll();
});

els.catalogForm.addEventListener("submit", async (event) => {
  event.preventDefault();
  await loadCatalog();
});

els.catalogResults.addEventListener("click", async (event) => {
  const button = event.target.closest("[data-model-id]");
  if (!button) {
    return;
  }

  await loadCatalogDetails(button.dataset.modelId);
});

els.creatorForm.addEventListener("submit", async (event) => {
  event.preventDefault();
  const query = new FormData(els.creatorForm).get("query");
  await loadCreators(String(query || ""));
});

els.creatorResults.addEventListener("click", async (event) => {
  const button = event.target.closest("[data-creator-id]");
  if (!button) {
    return;
  }

  await loadCreatorDetails(button.dataset.creatorId);
});

els.resourceTabs.forEach((tab) => {
  tab.addEventListener("click", async () => {
    els.resourceTabs.forEach((item) => item.classList.toggle("is-active", item === tab));
    state.resourcesTab = tab.dataset.resource;
    await loadResources();
  });
});

els.useSampleImage.addEventListener("click", () => {
  els.recognitionStatus.textContent = "Sample image available in the repository at origami_sample_images/butterfly.jpg";
});

els.recognitionFile.addEventListener("change", () => {
  const file = els.recognitionFile.files?.[0];
  els.recognitionStatus.textContent = file ? `Selected: ${file.name}` : "No file selected.";
});

els.recognitionForm.addEventListener("submit", async (event) => {
  event.preventDefault();

  const file = els.recognitionFile.files?.[0];
  if (!file) {
    els.recognitionStatus.textContent = "Select an image before submitting.";
    return;
  }

  const formData = new FormData();
  formData.append("file", file);
  els.recognitionStatus.textContent = `Uploading ${file.name} for analysis...`;
  els.recognitionResults.innerHTML = `<div class="empty-state">Processing image...</div>`;

  try {
    const result = await fetchJson("/recognition/predict", {
      method: "POST",
      body: formData,
    });

    renderRecognitionResponse(result);
    els.recognitionStatus.textContent = `Analysis completed for ${result.fileName}.`;
  } catch (error) {
    els.recognitionStatus.textContent = "Recognition request failed. Check API runtime dependencies and model availability.";
    els.recognitionResults.innerHTML = `<div class="empty-state status-error">${escapeHtml(error.message)}</div>`;
  }
});

refreshAll();
