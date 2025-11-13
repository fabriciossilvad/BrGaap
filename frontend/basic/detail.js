// detail.js - mostra detalhes de uma tarefa
// Note: backend exposes routes at /todos (not /api/todos)
const API_BASE = "http://localhost:5188/todos";
function el(id){ return document.getElementById(id); }
function showBusy(show){ const b = el('busy'); if(b){ b.classList.toggle('hidden', !show); } }

function getParam(name){ const url = new URL(location.href); return url.searchParams.get(name); }

async function fetchDetail(id){
  showBusy(true);
  try{
    const res = await fetch(`${API_BASE}/${id}`);
    if(res.status===404){ el('notFound').classList.remove('hidden'); el('detail').classList.add('hidden'); return; }
    if(!res.ok) throw new Error('HTTP ' + res.status);
    const t = await res.json(); renderDetail(t);
  }catch(err){ console.error(err); alert('Erro ao carregar detalhe: '+err.message); }
  finally{ showBusy(false); }
}

function renderDetail(t){ el('detail').classList.remove('hidden'); el('notFound').classList.add('hidden');
  const container = el('detail'); container.innerHTML = '';
  const fields = [ ['ID', t.id], ['Título', t.title], ['UserId', t.userId], ['Concluída', t.completed ? 'Sim' : 'Não'] ];
  for(const f of fields){
    const div = document.createElement('div'); div.className='field';
    const label = document.createElement('div'); label.className='label'; label.textContent = f[0];
    const value = document.createElement('div'); value.className='value'; value.textContent = f[1];
    div.appendChild(label); div.appendChild(value); container.appendChild(div);
  }
  const btn = document.createElement('button'); btn.className='button'; btn.textContent = t.completed ? 'Marcar como Pendente' : 'Marcar como Concluída';
  btn.addEventListener('click', ()=> toggleCompleted(t.id, !t.completed));
  container.appendChild(btn);
}

async function toggleCompleted(id, completed){ showBusy(true); try{
  const res = await fetch(`${API_BASE}/${id}`, { method:'PUT', headers:{'Content-Type':'application/json'}, body: JSON.stringify({ completed }) });
  if(!res.ok) throw new Error('HTTP ' + res.status);
  fetchDetail(id);
  alert('Tarefa atualizada');
}catch(err){ alert('Erro: '+err.message); console.error(err);} finally{ showBusy(false); }}

document.addEventListener('DOMContentLoaded', ()=>{
  const back = el('backBtn'); if(back) back.addEventListener('click', ()=> history.back());
  const id = getParam('id'); if(!id){ el('notFound').classList.remove('hidden'); return; }
  fetchDetail(id);
});