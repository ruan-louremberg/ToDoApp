import http from 'k6/http';
import { sleep, check } from 'k6';

// Configuração de carga
export const options = {
  vus: 100,         // Simula 100 usuários simultâneos
  duration: '10s', // Duração total do teste: 10  segundos
};

export default function () {
  const data = {
    title: 'Nova Tarefa',
    description: 'Descrição da nova tarefa',
  }
  // Executa uma requisição GET para sua API
  const res = http.post('http://localhost:5080/api/tasks', JSON.stringify(data), {
    headers: { 'Content-Type': 'application/json' },
  });

  // Validação (check): garante que a API respondeu 200 OK
  check(res, {
    'status é 200': (r) => r.status === 200,
  });

  // Pausa curta entre requisições para simular navegação real
  sleep(1);
}