# Origami Frontend

Static frontend for the origami recognition system.

## Run

Serve this folder with any static server. For example:

```powershell
cd frontend
python -m http.server 4173
```

Then open:

```text
http://localhost:4173
```

## API

The app expects the API base URL:

```text
http://localhost:5102/api
```

You can change the base URL from the page header without editing code.

## Notes

- No files under `origami_api` were modified.
- The UI consumes these endpoints:
  - `/showcase/hero`
  - `/discovery/featured`
  - `/analytics/landscape`
  - `/analytics/creators`
  - `/catalog/models`
  - `/catalog/models/{id}`
  - `/creators`
  - `/creators/{id}`
  - `/resources/books`
  - `/resources/diagrams`
  - `/resources/articles`
  - `/resources/calls`
  - `/recognition/predict`
