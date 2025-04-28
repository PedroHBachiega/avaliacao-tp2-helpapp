# 📘 Avaliação Técnica – Clean Architecture + Azure SQL

Este repositório contém minha entrega referente à avaliação técnica baseada no repositório original do professor:  
[https://github.com/victoricoma/avaliacao-tp2-helpapp](https://github.com/victoricoma/avaliacao-tp2-helpapp)

---

## ✅ Objetivo

Implementar os repositórios `Category` e `Product` seguindo os padrões da Clean Architecture, aplicar a migration `Initial` e conectar a aplicação com uma instância de SQL Server no Azure.

---

## 🚀 Funcionalidades implementadas

- [x] Repositórios `CategoryRepository` e `ProductRepository`
- [x] Configurações com `EntityTypeConfiguration` para `Category` e `Product`
- [x] Injeção de dependência configurada (`DependencyInjectionAPI`)
- [x] Migration `Initial` criada com `HasData()` para categorias
- [x] Banco de dados SQL Server criado no Azure
- [x] Migration aplicada com sucesso no Azure via `dotnet ef database update`

---

# Criação da Branch

![minhabranch](https://github.com/user-attachments/assets/b109a744-8253-4d8c-b987-85d312e97035)


# 🔧 Comandos utilizados
## Criação da migration
dotnet ef migrations add Initial --project Infra.Data --startup-project WebAPI

## Aplicação no banco de dados (Azure)
dotnet ef database update --project Infra.Data --startup-project WebAPI

![comandomigration](https://github.com/user-attachments/assets/ab4bde1a-f3c1-4936-b607-db4190a6ed60)


# 🔗 String de conexão (mascarada)  Usando User Secrets (recomendado para desenvolvimento) Demonstração via API para ver a string sendo mascarada

  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR_AQUI;Database=NOME_BANCO_AQUI;iser ID=SEU_USUARIO_AQUI;password=SUA_SENHA_AQUI;Encrypt=True;TrustServerCertificate=False;ConnectionTimeout=30"},
    
![conexaomascarada](https://github.com/user-attachments/assets/35835852-dd03-4df6-bb6c-a722799cd010)

# ☁️ Configuração no Azure
SQL Server criado no portal Azure

Banco de dados nomeado: avaliacao_tp2_pedrobachiega

IP local autorizado no firewall

![meuip](https://github.com/user-attachments/assets/1dc4b695-3519-4b1f-b6f4-3b3a0944fbdd)


Tabelas e dados populados

![resultadossql](https://github.com/user-attachments/assets/5cfbe988-bb5c-4fef-a5f6-56c02026ced6)


Testes Unitários do projeto

![testes](https://github.com/user-attachments/assets/0b2d10a8-fb85-4443-a1dc-7398891a4a0d)


# 👨‍💻 Dados do aluno
Nome: Pedro Henrique Bachiega
Curso: Desenvolvimento de Sistemas – 3º Semestre

Professor: Victor Icoma

Branch da entrega: avaliacao-PedroHBachiega

## 🧱 Estrutura da aplicação

```bash
📦 src
 ┣ 📂 Domain
 ┣ 📂 Application
 ┣ 📂 Infra
 ┃ ┣ 📂 Data
 ┃ ┃ ┣ 📂 Migrations
 ┃ ┃ ┣ 📂 Repositories
 ┃ ┃ ┗ 📂 EntityConfiguration
 ┗ 📂 WebAPI

# 📜 Review Tradicional - Estrutura do Projeto (Baseada na Figura e no Commit)
🔹 1. HelpApp.Domain/Entities
O que vi:

Category.cs e Product.cs modelados.

As entidades contêm apenas propriedades simples (Id, Name, etc.).

Não notei acoplamento direto com EF ([Key], [ForeignKey]), ou seja, estão limpas como devem ser.

Ponto Positivo:

Separação correta: Entidades dentro do domínio, sem saber nada da infraestrutura.

Simples e claras: Focadas em atributos, sem métodos de negócios pesados.

Ponto a Melhorar:

Validações internas: Poderiam ter regras básicas de negócio (por exemplo: "Nome não pode ser nulo", "Preço precisa ser maior que zero", etc.).
Clean Architecture prega que as entidades devem guardar a sanidade do seu próprio estado.

📜 Comentário clássico:
"Entidade que aceita qualquer valor é como igreja sem porta: entra até quem não deveria."

🔹 2. HelpApp.Domain.Test
O que vi:

Dois arquivos de teste: CategoryUnitTest.cs e ProductUnitTest.cs.

Ponto Positivo:

Boas práticas presentes: Testes unitários separados por entidade.

Ponto a Melhorar:

Foco nos testes: Sem ver o código de dentro, não sei se testam apenas a entidade ou se misturam persistência e lógica externa.
Ideal: testes focados só no domínio, sem bater no banco ou precisar de infra.

📜 Comentário clássico:
"Teste que precisa do banco é como receita de bolo que pede farinha de marte: complicado sem necessidade."

🔹 3. HelpApp.Infra.Data/Repositories
O que vi:

CategoryRepository.cs e ProductRepository.cs implementados aqui.

Ponto Positivo:

Repositórios segregados: Cada entidade tem seu próprio repositório.

Provavelmente estão implementando uma camada concreta de acesso a dados.

Ponto a Melhorar:

Interface de abstração:
Não encontrei as interfaces (ICategoryRepository, IProductRepository) no domínio ou em outra camada.
A Clean Architecture exige que o domínio defina o contrato (a interface) e a infraestrutura implemente.
Aqui, parece que o domínio ainda depende direto da infra, ou pelo menos não impõe suas regras.

📜 Comentário clássico:
"Quem implementa sem contrato é como quem constrói casa sem planta: vai sair, mas vai cair."

🔹 4. HelpApp.Infra.IoC
O que vi:

DependencyInjectionAPI.cs para configurar as dependências.

Ponto Positivo:

Configuração centralizada: Lugar único para registrar os serviços no contêiner de IoC.

Ponto a Melhorar:

Possível acoplamento forte: Se as classes registradas no IoC conhecem o Entity Framework ou outra infraestrutura diretamente e se não usam interfaces para abstração, ainda há risco de fuga de responsabilidade.

📜 Comentário clássico:
"Quem injeta dependência sem filtro é como convidar todo mundo para o casamento: depois não reclama do tumulto."

🏛️ Resumo Tradicional da Avaliação

Critério	Avaliação
Entidades (pureza e independência)	8/10
Testes (estrutura e foco)	7/10
Repositórios (abstração e isolamento)	6/10
IoC (configuração e isolamento)	7/10
🎯 Nota Final: 7,0/10
Justificativa tradicional:

Projeto bem organizado e separado em pastas, respeitando o espírito da Clean Architecture.

Falta o uso de interfaces no domínio para garantir total independência de infraestrutura.

Testes poderiam reforçar as regras internas das entidades em vez de depender de frameworks externos.

IoC bem posicionado, mas precisa amarrar tudo via abstrações, não concreto diretamente.
