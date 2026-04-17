"""JSON-oriented bridge for ASP.NET recognition requests."""

from __future__ import annotations

import json
import os
import sys
from pathlib import Path

os.environ.setdefault("TF_ENABLE_ONEDNN_OPTS", "0")
os.environ.setdefault("TF_CPP_MIN_LOG_LEVEL", "3")

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
ROOT_DIR = os.path.dirname(BASE_DIR)
if ROOT_DIR not in sys.path:
    sys.path.insert(0, ROOT_DIR)

MODEL_PATH = os.path.join(BASE_DIR, "origami_model.h5")
FALLBACK_MODEL_PATH = os.path.join(BASE_DIR, "checkpoints", "best_model.keras")
TEMPLATE_DIR = os.path.join(ROOT_DIR, "origami_sample_images")


def resolve_model_path() -> str:
    explicit_path = os.getenv("ORIGAMI_MODEL_PATH", "").strip()
    if explicit_path:
        return explicit_path
    if os.path.exists(FALLBACK_MODEL_PATH):
        return FALLBACK_MODEL_PATH
    return MODEL_PATH


def model_exists(model_path: str) -> bool:
    return os.path.exists(model_path)


def build_template_predictions(image_path: str) -> list[dict[str, float | int | str]]:
    import cv2
    import numpy as np

    input_image = cv2.imread(image_path, cv2.IMREAD_GRAYSCALE)
    if input_image is None:
        raise ValueError(f"Could not read image: {image_path}")

    input_image = cv2.resize(input_image, (224, 224), interpolation=cv2.INTER_AREA)
    predictions: list[dict[str, float | int | str]] = []

    for class_index, template_path in enumerate(sorted(Path(TEMPLATE_DIR).glob("*"))):
        if not template_path.is_file():
            continue

        template = cv2.imread(str(template_path), cv2.IMREAD_GRAYSCALE)
        if template is None:
            continue

        template = cv2.resize(template, (224, 224), interpolation=cv2.INTER_AREA)
        diff = cv2.absdiff(input_image, template)
        mae = float(np.mean(diff)) / 255.0
        confidence = max(0.0, min(100.0, (1.0 - mae) * 100.0))
        predictions.append(
            {
                "class_index": class_index,
                "label": template_path.stem.replace("_", " ").replace("-", " ").title(),
                "confidence": round(confidence, 2),
            }
        )

    if not predictions:
        raise FileNotFoundError(
            "No trained model file was found and no fallback templates exist in origami_sample_images."
        )

    return sorted(predictions, key=lambda item: item["confidence"], reverse=True)[:3]


def main() -> int:
    if len(sys.argv) < 2:
        print(json.dumps({"error": "Image path argument is required."}))
        return 1

    image_path = sys.argv[1]

    try:
        from ai.predict_image import get_top_predictions, load_and_preprocess_image, load_labels_dict, load_model

        model_path = resolve_model_path()
        if model_exists(model_path):
            model = load_model(model_path)
            index_to_label = load_labels_dict()
            _, image_batch = load_and_preprocess_image(image_path)
            predictions = model.predict(image_batch, verbose=0)
            top_predictions = get_top_predictions(predictions, index_to_label=index_to_label, top_k=3)

            payload = {
                "predictions": [
                    {
                        "class_index": class_index,
                        "label": label,
                        "confidence": confidence,
                    }
                    for class_index, label, confidence in top_predictions
                ]
            }
        else:
            payload = {
                "predictions": build_template_predictions(image_path),
                "fallback": "template_match",
                "model_missing": model_path,
            }

        print(json.dumps(payload))
        return 0
    except ModuleNotFoundError as exc:
        module_name = exc.name or "unknown"
        dependency_hint = " Install the required Python package and retry."
        if module_name == "cv2":
            dependency_hint = " Install 'opencv-python' in the Python environment used by the API and retry."

        print(json.dumps({"error": f"Missing Python dependency '{module_name}'.{dependency_hint}"}))
        return 1
    except Exception as exc:
        print(json.dumps({"error": str(exc)}))
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
