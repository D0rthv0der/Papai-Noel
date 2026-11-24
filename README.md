# Trabalho N3 — Problema do Papai Noel

**Versão desta entrega: Entrega 2 — duas soluções ingênuas e menu.**

**Aluno:** [preencha seu nome]  
**Disciplina:** Desenvolvimento para Concorrência (ESW240)  
**Linguagem:** C# / .NET 10  
**Tipo:** Aplicação de console

## 1. Descrição do problema

O cenário contém uma Task que representa o Papai Noel, nove Tasks que representam as renas e dez Tasks que representam os elfos. As renas retornam de suas viagens e os elfos procuram ajuda em grupos de exatamente três.

O Papai Noel é acordado quando as nove renas retornam ou quando três elfos estão esperando. A chegada das nove renas tem prioridade: quando todas retornam, o Papai Noel prepara o trenó e encerra o atendimento aos elfos. Por isso, como existem dez elfos, pode restar um elfo sem atendimento quando a simulação termina. Os elfos efetivamente atendidos sempre são liberados em grupos completos de três.

## 2. Threads/Tasks e recursos disputados

| Participante | Quantidade | Recursos/estado compartilhado |
|---|---:|---|
| Papai Noel | 1 | Contadores de renas e elfos, sinais de despertar e liberação dos grupos |
| Renas | 9 | Contador de renas retornadas e semáforo de liberação para o trenó |
| Elfos | 10 | Vagas de espera, contador de elfos aguardando e semáforo de atendimento |

Cada participante é representado por uma `Task`. Atrasos aleatórios com `Random.Shared` variam a ordem de execução.

## 3. Regras para acesso seguro

1. **Contadores protegidos:** toda leitura/modificação dos contadores compartilhados da solução ideal acontece dentro do mesmo `lock`.
2. **Grupo completo:** o Papai Noel só libera um grupo quando há exatamente três elfos aguardando.
3. **Limite da fila:** no máximo três elfos ocupam as vagas de espera ao mesmo tempo.
4. **Exclusividade do atendimento:** o Papai Noel conclui um grupo antes de iniciar outro; dois grupos não são atendidos simultaneamente.
5. **Prioridade das renas:** quando a nona rena retorna, o Papai Noel prioriza preparar o trenó.
6. **Sinalização persistente:** `SemaphoreSlim` guarda liberações até que a Task correspondente execute `WaitAsync`, evitando depender de um aviso momentâneo.

## 4. Soluções implementadas

### 4.1 Ingênua 1 — condição de corrida

O contador de renas é atualizado sem `lock` nem operação atômica. Cada Task lê o valor, aguarda e grava o valor incrementado. Uma `Barrier` é usada somente para alinhar a demonstração e fazer as nove Tasks lerem antes de qualquer gravação. Assim, várias gravações sobrescrevem umas às outras e atualizações são perdidas.

**Problema provocado:** o contador final não representa as nove renas; o Papai Noel não reconheceria a chegada de todas. A barreira não protege o contador: ela apenas torna a falha reproduzível.

### 4.2 Ingênua 2 — espera ocupada

O Papai Noel consulta repetidamente os contadores em um laço com `SpinWait`, sem aguardar um sinal. Os elfos também consultam continuamente se já foram atendidos.

**Problema provocado:** consumo desnecessário de CPU enquanto as Tasks aguardam. A versão usa operações atômicas para que a demonstração foque na espera ocupada, e não em uma segunda condição de corrida. O relatório registra a quantidade de iterações do laço, além dos contadores de renas, elfos e grupos atendidos.

### 4.3 Solução ideal — `SemaphoreSlim` e `lock`

A solução ideal usa:

- `lock` para proteger os contadores e as decisões sobre o estado compartilhado;
- `SemaphoreSlim` para acordar o Papai Noel, liberar as nove renas, liberar três elfos e aguardar a conclusão de um grupo;
- `SemaphoreSlim(3, 3)` para limitar a três as vagas de espera dos elfos;
- `WaitAsync` para suspender Tasks sem ficar consultando contadores continuamente.

O sinal de despertar é armazenado pelo semáforo, então a notificação não depende de o Papai Noel estar executando exatamente no instante em que o terceiro elfo ou a nona rena chega.

## 5. Verificações automáticas da solução ideal

1. As nove renas retornaram e foram liberadas para o trenó.
2. O total de elfos atendidos é múltiplo de três, portanto só houve grupos completos.
3. Nunca houve mais de três elfos aguardando e nunca houve mais de um grupo em atendimento simultâneo.

O relatório imprime `[OK]` ou `[ERRO]` para cada verificação.

## 6. Menu

Ao executar `dotnet run`, o menu oferece:

1. Ingênua 1 — condição de corrida.
2. Ingênua 2 — espera ocupada.
3. Solução ideal — `SemaphoreSlim` + `lock`.
4. Comparação de desempenho — tempo total da ingênua 2 e da ideal.
0. Sair.

## 7. Como executar

É necessário ter o SDK do .NET 10 instalado.

```bash
dotnet run
```

## 8. Comparação de desempenho

A opção 4 executa a versão com espera ocupada e a versão ideal e apresenta o tempo total de cada execução. Os tempos variam por causa dos atrasos aleatórios e do escalonamento do sistema. Tempo total não é uma medida direta de CPU; a demonstração da espera ocupada está no laço que consulta os contadores continuamente, ao contrário da versão ideal, que aguarda semáforos.

## 9. Evidências para completar na entrega

Depois de executar o programa, adicione nesta seção capturas reais do console:

- **Ingênua 1:** log mostrando o contador incorreto e a condição de corrida.
- **Ingênua 2:** log mostrando o Papai Noel/elfos aguardando por espera ocupada.
- **Solução ideal:** relatório com as três verificações em `[OK]`.
- **Comparação:** saída da opção 4.

Não foram incluídas capturas fictícias; elas devem ser feitas após executar o projeto no seu computador.

## 10. Ferramentas de IA

Foi utilizado o ChatGPT como apoio para estruturar o projeto, elaborar código inicial e organizar a documentação. O aluno deve revisar, executar, testar e ser capaz de explicar cada parte do código antes da entrega.

## 11. Referência

DOWNEY, Allen B. *The Little Book of Semaphores*. 2. ed. 2016. Problema clássico do Papai Noel (Santa Claus problem).
