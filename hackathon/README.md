# TimeBite Meta handwriting benchmark

Prepared 2026-10-13. Isolated from timebite-official and the glasses client. No app code or existing repository changes are modified.

## Machine and tools

Apple M1 MacBookPro17,1, 16 GiB unified memory, about 157 GiB disk free at inspection. Qwen 4B Q4 plus F16 vision projector is the local baseline (~3.33 GB weights). Run models sequentially. Glimmer needs roughly 19 GB for vision inference; this Mac is too small. Use a 32 GiB+ Mac or remote machine for Glimmer. EmbeddingGemma is text embeddings, not OCR or task extraction.

Hugging Face CLI: ~/.local/share/timebite-meta/tools/venv/bin/hf
Python: ~/.local/share/timebite-meta/tools/venv/bin/python
llama.cpp: ~/.local/share/timebite-meta/tools/llama/bin/llama-server (official ARM64 release b11429; Metal supported).
Homebrew installation failed because the installed Homebrew does not recognize macOS 26.6.2; official release used instead.
Model storage: ~/Models/timebite-meta, outside Git. Set TIMEBITE_MODEL_DIR to change it. Raw handwriting and results are ignored here.

## Verified models

Exact filenames, immutable revisions and SHA-256 values are in models.lock.json. Downloads select only listed files, verify checksums, reserve 10 GiB free space, and never execute model repository code.

- Muse: https://huggingface.co/meta-models/Muse-Glimmer-30B-GGUF — Apache 2.0; Q4_K_M 17GB + Q4 vision projector; no optional drafter. Runtime needs b10353 or later, plus --jinja. https://dev.meta.ai/docs/muse-glimmer/get-the-model
- Qwen: https://huggingface.co/Qwen/Qwen3-VL-4B-Instruct-GGUF — Apache 2.0; Q4_K_M + F16 projector.
- EmbeddingGemma: https://huggingface.co/ggml-org/embeddinggemma-300M-GGUF — Q8_0 GGUF conversion of https://huggingface.co/google/embeddinggemma-300m. Google’s Gemma terms apply: https://ai.google.dev/gemma/terms . Review and accept the upstream conditions yourself; the script never accepts account agreements for you.

No model weights are downloaded during setup. Preview and fetch:

```sh
cd /Users/erinjerri/Documents/GH-Repos-Main/timebite-meta
PY="$HOME/.local/share/timebite-meta/tools/venv/bin/python"
"$PY" hackathon/download.py qwen
"$PY" hackathon/download.py qwen --download
# After personally reviewing Google’s terms:
"$PY" hackathon/download.py embeddinggemma --download --accept-gemma-terms
# On a larger machine:
"$PY" hackathon/download.py glimmer --download
"$PY" hackathon/serve.py qwen
```

Glimmer download has a memory guard; --allow-large-model permits storage-only download on this 16 GiB Mac. serve.py still refuses local Glimmer execution. Servers bind to 127.0.0.1. No automatic downloads occur in serve.py.

## Dataset and evaluation

Copy manifest.example.jsonl to a private manifest and replace the placeholder image/ground truth. One JSONL row per photographed sample; assign stable id, writer_id and page_id. Photos of the same page share page_id. Keep whole writers/pages in one split. Collect consented, non-sensitive handwriting. Start with 20–30 development pages; freeze a separate 100–150-page test set if feasible. Your own handwriting is useful for personal performance; multiple writers are needed for cross-writer evidence. Sample count alone does not guarantee significance.

Expected contract: transcription string; tasks array of objects with title string and due string or null. Dates must be explicit; do not infer them without a defined reference date. Task array order is significant for the strict exact-match score.

```sh
"$PY" hackathon/benchmark.py hackathon/manifest.jsonl --model qwen --output hackathon/results/qwen-run1.jsonl
# Add Apple Vision’s verbatim output as ocr_text in each row to compare OCR + LLM:
"$PY" hackathon/benchmark.py hackathon/manifest.jsonl --model qwen --ocr-text --output hackathon/results/vision-qwen-run1.jsonl
"$PY" hackathon/summarize.py hackathon/results/qwen-run1.jsonl
```

The scaffold records full response, failures, latency, schema validity, exact tasks and character edit counts. Invalid JSON and request failures remain in the denominator. It does not execute extracted tasks. Apple Vision integration is an input adapter slot via ocr_text, not an implemented Swift OCR engine. Compare the same held-out images and ground truth across methods. Temperature=0, seed=42 and 4096 output tokens are fixed; record runtime, revision, context, prompt and sampling settings with each report. Glimmer may need a larger generation budget; report truncations and any differing settings. Latency includes request time, not model startup.

For paired comparisons, align sample IDs, use exact McNemar only for independent binary outcomes, and use writer/page cluster bootstrap for repeated pages or writers. CER = total edits / total reference characters; no semantic embeddings as a substitute for OCR accuracy. Latency p50/p95 and failures accompany accuracy. No significance or performance claims before actual data collection and model runs.

Hackathon rules and paid hosted-model eligibility are not supplied; verify organizer rules on pre-existing work, permitted models and APIs before submission. This scaffold does not establish eligibility.
