import json,os,subprocess,sys
from pathlib import Path
name=sys.argv[1] if len(sys.argv)>1 else 'qwen'
m=json.loads(Path(__file__).with_name('models.lock.json').read_text())[name]
root=Path(os.environ.get('TIMEBITE_MODEL_DIR',str(Path.home()/'Models/timebite-meta')))/name
runtime=os.environ.get('LLAMA_SERVER',str(Path.home()/'.local/share/timebite-meta/tools/llama/bin/llama-server'))
for f in m['files']:
    if not (root/f).is_file():raise SystemExit('Missing '+str(root/f))
cmd=[runtime,'-m',str(root/m['files'][0]),'--host','127.0.0.1','--port','8080','-ngl','99','-c','8192','-np','1','-a',name]
if name=='embeddinggemma':cmd+=['--embeddings','--pooling','mean']
else:cmd+=['--mmproj',str(root/m['files'][1]),'--jinja']
if name=='glimmer':
    if int(subprocess.check_output(['/usr/sbin/sysctl','-n','hw.memsize']))<24*1024**3:raise SystemExit('Use a machine with at least 24 GiB; 32 GiB preferred.')
subprocess.run(cmd,check=True)
