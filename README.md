# BankAccountAPI

API REST desenvolvida em .NET 8.0 para simular um sistema de contas bancárias, integrando um banco de dados SQL Lite, permitindo operações de depósito, saque, transferência entre contas, consulta de extrato e um CRUD completo de contas bancárias.

Essa API foi desenvolvida simulando operações bancárias seguindo as principais boas práticas de desenvolvimento, como:

- Uso de arquitetura limpa e separação de responsabilidades;
- Operações assíncronas (`async/await`) para acesso eficiente aos dados;
- Manipulação de valores em **centavos** para evitar imprecisão com ponto flutuante;
- Validação de saldo considerando o valor da operação **mais as taxas**;
- Cálculo de saldo sempre baseado nas transações (não é um campo salvo no banco);
- Uso correto do ciclo de vida das dependências (`Scoped`) para evitar concorrência e inconsistências nas operações.

Este projeto apresenta:

- **CRUD de Conta Bancária**
  - Criar conta
  - Consultar conta
  - Atualizar conta
  - Deletar conta
  - Consultar saldo
  - Consultar extrato bancário (lista de transações com descrição e valores)

- **Transações Bancárias**
  - Depósito
  - Saque
  - Transferência entre contas


**Regras de Negócio**

- Não é permitido saldo negativo.
- O cliente **só pode sacar ou transferir se tiver saldo suficiente considerando a taxa da operação.**
- O **saldo é sempre calculado** com base nas transações e seu tipo (créditos e débitos).


**Observação:** Todos os valores são manipulados em **centavos**.

## Arquitetura

- **Camadas separadas:**
  - `Entities`: Entidades do domínio
  - `Requests`: Modelos para entrada de dados
  - `Responses`: Modelos para saída de dados (não expõem dados sensíveis para o Front-end)

- **Saldo:**  
O saldo (**CurrentBalance**) não é um campo fixo, e sim um campo calculado a partir de transações feitas na conta.

- **Ciclo de vida das dependências:**  
Os repositórios são registrados como `Scoped` no `Program.cs`, garantindo uma única instância por requisição e evitando concorrência e inconsistências nas transações bancárias.

- **Operações assíncronas:**  
Todas as operações que acessam dados (banco de dados) são implementadas com `async/await` para garantir performance e escalabilidade.


## Endpoints 

### **Contas Bancárias**
- `GET /api/accounts`: Listar todas as contas
- `POST /api/accounts`: Criar conta
- `GET /api/accounts/{accountId}`: Buscar dados de uma conta por AccountId
- `PUT /api/accounts/{accountId}`: Atualizar dados da conta
- `DELETE /api/accounts/{accountId}`: Deletar conta (somente é permitido quando não há saldo na conta)
- `GET /api/accounts/{accountId}/statement`: Obtém extrato (lista de lançamentos realizados na conta)
- `GET /api/accounts/{accountId}/balance`: Obtém saldo atual da conta

### **Transações**
- `POST /api/transactions/deposit`: Realizar depósito
- `POST /api/transactions/withdraw`: Realizar saque
- `POST /api/transactions/transfer`: Realizar transferência

