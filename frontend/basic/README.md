# Frontend simples (HTML/CSS/JS)

Arquivos criados em `frontend/basic`:
- `index.html` - lista com busca, paginação e checkbox para marcar concluído
- `detail.html` - visualização completa de uma tarefa
- `app.js` - lógica do frontend (fetch, render, debounce, paginação)
- `detail.js` - lógica da página de detalhes
- `styles.css` - estilos e spinner

Requisitos:
- O backend deve estar rodando em `http://localhost:5188`.
- O backend já possui CORS liberado para `http://localhost:8080` (ver `Program.cs`).

Executando (opções):
1) Usando `npx http-server` (recomendado)

```powershell
cd frontend\basic
npx http-server -p 8080
# abra http://localhost:8080
```

2) Usando `python` (se instalado):

```powershell
cd frontend\basic
python -m http.server 8080
# abra http://localhost:8080
```

3) Abrindo direto o arquivo com `file://` pode causar bloqueio CORS; prefira servir via HTTP.

Testes rápidos:
- Abra `http://localhost:8080` e verifique se a lista carrega.
- Pesquise um termo do título para ver filtro por query param.
- Use os botões Anterior/Próxima para paginação.
- Clique em "Detalhes" para abrir `detail.html?id={id}`.
- Altere o checkbox para marcar concluído; ele chama o endpoint PUT `/api/todos/{id}`.

Observações:
- Se houver erro de CORS, verifique `Program.cs` para permitir a origem correta (ex: `http://localhost:8080`).
- Se preferir outra porta, atualize tanto o servidor estático quanto o `WithOrigins(...)` no backend.
