import json,sys,statistics
from pathlib import Path
rows=[json.loads(s) for s in Path(sys.argv[1]).read_text().splitlines() if s.strip()]
if not rows:raise SystemExit('No results')
n=len(rows);lat=sorted(r['latency_s'] for r in rows);chars=sum(r.get('reference_chars',0) for r in rows)
print(json.dumps({'samples':n,'json_valid_rate':sum(r['json_valid'] for r in rows)/n,'task_exact_rate':sum(r['task_exact'] for r in rows)/n,'failed':sum('error' in r for r in rows),'cer_valid_responses_only':sum(r.get('char_edits',0) for r in rows)/chars if chars else None,'cer_scored_samples':sum('char_edits' in r for r in rows),'latency_p50_s':statistics.median(lat),'latency_p95_s':lat[min(n-1,__import__('math').ceil(.95*n)-1)]},indent=2))
