import argparse, hashlib, json, os, shutil, subprocess
from pathlib import Path
p=argparse.ArgumentParser();p.add_argument('model',choices=['qwen','glimmer','embeddinggemma']);p.add_argument('--download',action='store_true');p.add_argument('--accept-gemma-terms',action='store_true');p.add_argument('--allow-large-model',action='store_true');a=p.parse_args()
m=json.loads(Path(__file__).with_name('models.lock.json').read_text())[a.model]
root=Path(os.environ.get('TIMEBITE_MODEL_DIR',str(Path.home()/'Models/timebite-meta'))).expanduser().resolve()
repo=Path(__file__).resolve().parents[1]
if root==repo or repo in root.parents: raise SystemExit('Model storage must be outside this Git repository')
print(json.dumps(m,indent=2));print('Destination:',root/a.model)
if not a.download: raise SystemExit('Plan only. Add --download to fetch these exact files.')
if a.model=='embeddinggemma' and not a.accept_gemma_terms: raise SystemExit('Review https://ai.google.dev/gemma/terms and acknowledge with --accept-gemma-terms')
if a.model=='glimmer':
    memory=int(subprocess.check_output(['/usr/sbin/sysctl','-n','hw.memsize']))
    if memory<24*1024**3 and not a.allow_large_model: raise SystemExit('Glimmer vision exceeds this Mac memory; use a larger machine or explicit --allow-large-model for storage only.')
root.mkdir(parents=True,exist_ok=True)
if shutil.disk_usage(root).free < m['bytes']+10*1024**3: raise SystemExit('Insufficient disk space with 10 GiB reserve')
os.environ['HF_HOME']=str(root/'hf-cache')
from huggingface_hub import hf_hub_download
for name,expected in zip(m['files'],m['sha256']):
    path=Path(hf_hub_download(m['repo'],name,revision=m['revision'],local_dir=root/a.model))
    h=hashlib.sha256()
    with path.open('rb') as f:
        for block in iter(lambda:f.read(8*1024*1024),b''):h.update(block)
    if h.hexdigest()!=expected:raise SystemExit('Checksum mismatch: '+name)
    print('Verified',name)
