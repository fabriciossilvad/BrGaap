const API_BASE = "http://localhost:5188/todos";

let state = {
  page: 1,
  pageSize: 10,
  total: 0,
  items: [],
  q: ''
};
let searchTimer = null;

function el(id){ return document.getElementById(id); }

/* ===============================
   Busy Indicator Controls
=============================== */
function showBusy() {
  const b = el('busy');
  if (b) b.classList.remove('hidden');
}

function hideBusy() {
  const b = el('busy');
  if (b) b.classList.add('hidden');
}

/* ===============================
   Fetch Tarefas
=============================== */
async function fetchTodos(){
  showBusy();
  try{
    const params = new URLSearchParams({ page: state.page, pageSize: state.pageSize });

    // Campo de busca dinâmico
    if (state.q) {
      switch (state.searchField) {
        case 'userId':
          params.append('userId', state.q);
          break;
        case 'completed':
          const val = state.q.toLowerCase();
          const boolVal = (val === 'true' || val === 'sim' || val === '1');
          params.append('completed', boolVal);
          break;
        default:
          params.append('title', state.q);
          break;
      }
    }

    const res = await fetch(`${API_BASE}?${params.toString()}`);
    if(!res.ok) throw new Error('HTTP ' + res.status);
    const data = await res.json();
    state.total = data.total || 0;
    state.items = data.items || [];
    renderList();
  }catch(err){
    console.error(err);
    alert('Erro ao carregar tarefas: ' + err.message);
    el('tasksBody').innerHTML = '';
    el('empty').classList.remove('hidden');
  }finally{
    hideBusy();
  }
}

/* ===============================
   Renderização da Lista
=============================== */
function renderList(){
  const tbody = el('tasksBody');
  tbody.innerHTML = '';
  if(!state.items || state.items.length===0){ 
    el('empty').classList.remove('hidden'); 
    el('tasksTable').classList.add('hidden');
    el('empty').innerHTML = 'Nenhuma tarefa cadastrada.';
  }
  else{
    el('empty').classList.add('hidden'); 
    el('tasksTable').classList.remove('hidden');
    
    for(const t of state.items){
      const tr = document.createElement('tr');

      // ID
      const tdId = document.createElement('td'); 
      tdId.textContent = t.id; 
      tr.appendChild(tdId);
      
      // Title
      const tdTitle = document.createElement('td');
      const spanTitle = document.createElement('span'); 
      spanTitle.textContent = t.title; 
      if(t.completed) spanTitle.classList.add('completed'); 
      tdTitle.appendChild(spanTitle); 
      tr.appendChild(tdTitle);
      
      // userId
      const tdUser = document.createElement('td'); 
      tdUser.textContent = t.userId; 
      tr.appendChild(tdUser);
      
      // completed icon 
      const tdChk = document.createElement('td');
      tdChk.className = 'status-cell';
      tdChk.dataset.concluido = t.completed;
      tdChk.innerHTML = t.completed ? '✅' : '❌';
      tdChk.addEventListener('click', async () => {
        const novoValor = !t.completed;
        await onToggleCompleted(t.id, novoValor);
        t.completed = novoValor;
        tdChk.dataset.concluido = novoValor;
        tdChk.innerHTML = novoValor ? '✔️' : '❌';
      });
      tr.appendChild(tdChk);
      
      // actions
      const tdAct = document.createElement('td'); 
      tdAct.className='actions';
      const btn = document.createElement('button'); 
      btn.className='button'; 
      btn.textContent='Detalhes'; 
      btn.addEventListener('click', ()=> openDetail(t.id)); 
      tdAct.appendChild(btn);
      tr.appendChild(tdAct);

      tbody.appendChild(tr);
    }
  }
  updatePaginationInfo();
}

/* ===============================
   Paginação
=============================== */
function updatePaginationInfo(){
  const info = el('pageInfo');
  const start = (state.page-1)*state.pageSize + 1;
  const end = Math.min(state.page*state.pageSize, state.total);
  info.textContent = `Mostrando ${start} a ${end} de ${state.total}`;
  el('prevBtn').disabled = state.page<=1;
  el('nextBtn').disabled = state.page*state.pageSize >= state.total;
}

function onPrev(){ if(state.page>1){ state.page--; fetchTodos(); } }
function onNext(){ if(state.page*state.pageSize < state.total){ state.page++; fetchTodos(); } }

/* ===============================
   Busca
=============================== */
function onSearch(e) {
  const v = e.target.value.trim();
  const field = el('searchField').value;
  state.q = v;
  state.searchField = field;

  if (searchTimer) clearTimeout(searchTimer);
  searchTimer = setTimeout(() => {
    state.page = 1;
    fetchTodos();
  }, 500);
}

/* ===============================
   Atualizar Status
=============================== */
async function onToggleCompleted(id, completed){
  showBusy();
  try{
    const res = await fetch(`${API_BASE}/${id}`, { 
      method:'PUT', 
      headers:{'Content-Type':'application/json'}, 
      body: JSON.stringify({ completed }) 
    });
    if(!res.ok){
      const t = await res.text(); throw new Error('Status ' + res.status + ' - ' + t);
    }
  }catch(err){ 
    alert('Erro ao atualizar: ' + err.message); 
    console.error(err);
  }finally{
    hideBusy();
  }
}

/* ===============================
   Detalhes
=============================== */
function openDetail(id){ 
  location.href = `detail.html?id=${encodeURIComponent(id)}`;
}

/* ===============================
   Paginação e Sincronização
=============================== */
function onPageSizeChange(e){ 
  state.pageSize = parseInt(e.target.value,10)||10; 
  state.page=1; 
  fetchTodos(); 
}

async function syncTodos(){
  showBusy();
  try{
    const res = await fetch(`${API_BASE}/sync`, { method: 'POST' });
    if(!res.ok){ 
      const t = await res.text(); 
      throw new Error('Status ' + res.status + ' - ' + t); 
    }
    state.page = 1;
    await fetchTodos();
    alert('Sincronização concluída.');
  }catch(err){
    console.error(err);
    alert('Erro ao sincronizar: ' + err.message);
  }finally{
    hideBusy();
  }
}

/* ===============================
   Inicialização
=============================== */
document.addEventListener('DOMContentLoaded', ()=>{
  el('prevBtn').addEventListener('click', onPrev);
  el('nextBtn').addEventListener('click', onNext);
  el('search').addEventListener('input', onSearch);
  el('searchField').addEventListener('change', () => fetchTodos());
  el('pageSize').addEventListener('change', onPageSizeChange);
  const sync = el('syncBtn'); if(sync) sync.addEventListener('click', syncTodos);

  /* Modal - Criar Tarefa */
  const createBtn = el('createTask');
  if(createBtn) createBtn.addEventListener('click', ()=>{
    el('taskModal').classList.remove('hidden');
  });
  const cancelBtn = el('cancelTask');
  if(cancelBtn) cancelBtn.addEventListener('click', ()=>{
    el('taskForm').reset();
    el('taskModal').classList.add('hidden');
  });
  const closeBtn = el('closeTaskModal');
  if(closeBtn) closeBtn.addEventListener('click', ()=>{
    el('taskForm').reset();
    el('taskModal').classList.add('hidden');
  });

  /* Envio do Formulário */
  const form = el('taskForm');
  if(form) form.addEventListener('submit', async (e)=>{
    e.preventDefault();
    const title = el('taskTitle').value.trim();
    const userId = el('taskUserId').value.trim();
    const completed = el('taskCompleted').checked;
    if(!title || !userId){
      alert('Preencha todos os campos obrigatórios.');
      return;
    }
    showBusy();
    try{
      const res = await fetch(`${API_BASE}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, userId, completed })
      });
      if(!res.ok){
        // tenta interpretar JSON
        let bodyMessage = '';
        try{
          const json = await res.json();
          if(json && json.message) bodyMessage = json.message;
        }catch(_){
          try{ bodyMessage = await res.text(); }catch(__){ bodyMessage = ''; }
        }

        // regra de negócio
        if(bodyMessage && bodyMessage.includes('5 tarefas incompletas')){
          throw new Error('Usuário atingiu limite de 5 tarefas incompletas, não é possível cadastrar mais.');
        }

        const text = bodyMessage || (`Status ${res.status}`);
        throw new Error(text);
      }
      el('taskForm').reset();
      el('taskModal').classList.add('hidden');
      await fetchTodos();
      alert('Tarefa cadastrada com sucesso!');
    }catch(err){
      alert(err.message || 'Erro ao cadastrar');
      console.error(err);
    }finally{
      hideBusy();
    }
  });

  // inicializa a página
  state.pageSize = parseInt(el('pageSize').value,10)||10;
  fetchTodos();
});
