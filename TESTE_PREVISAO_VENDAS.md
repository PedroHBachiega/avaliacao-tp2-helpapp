# Guia de Teste da Funcionalidade de Previsão de Vendas

## Descrição

Este documento descreve como testar a funcionalidade de análise preditiva de vendas implementada usando ML.NET. A funcionalidade permite prever vendas futuras de produtos com base em dados históricos simulados.

## Pré-requisitos

1. Visual Studio 2022 (ou versão compatível)
2. .NET 6.0 SDK instalado
3. Pacote ML.NET instalado (já configurado no projeto)

## Como Testar no Visual Studio

### Passo 1: Abrir a Solução

1. Abra o Visual Studio 2022
2. Selecione "Arquivo" > "Abrir" > "Projeto/Solução"
3. Navegue até a pasta do projeto e selecione o arquivo `StockApp.sln`
4. Clique em "Abrir"

### Passo 2: Restaurar Pacotes NuGet

1. Clique com o botão direito na solução no Gerenciador de Soluções
2. Selecione "Restaurar Pacotes NuGet"
3. Aguarde a conclusão do processo

### Passo 3: Compilar a Solução

1. Pressione `Ctrl+Shift+B` ou selecione "Compilar" > "Compilar Solução" no menu
2. Verifique se não há erros de compilação

### Passo 4: Executar o Projeto

1. Defina o projeto `StockApp.API` como projeto de inicialização (clique com o botão direito no projeto e selecione "Definir como Projeto de Inicialização")
2. Pressione `F5` ou clique no botão "Iniciar" para executar o projeto
3. O navegador será aberto automaticamente com o Swagger UI ou a página inicial da API

### Passo 5: Testar a API de Previsão de Vendas

#### Opção 1: Usando o Swagger UI

1. No Swagger UI, localize o endpoint `/api/SalesPrediction/{productId}/{month}/{year}`
2. Clique em "Try it out"
3. Preencha os parâmetros:
   - `productId`: ID do produto (ex: 1)
   - `month`: Mês (1-12)
   - `year`: Ano atual ou futuro
4. Clique em "Execute"
5. Verifique a resposta da API com a previsão de vendas

#### Opção 2: Usando a Página de Teste HTML

1. No navegador, navegue até `https://localhost:{porta}/sales-prediction-test.html`
   (substitua `{porta}` pela porta em que a aplicação está rodando)
2. Preencha o formulário com:
   - ID do Produto
   - Mês
   - Ano
3. Clique em "Obter Previsão"
4. Verifique o resultado exibido na página

## Detalhes da Implementação

### Arquivos Criados/Modificados

1. `ISalesPredictionService.cs` - Interface para o serviço de previsão de vendas
2. `SalesPredictionDTO.cs` - DTO para transferência de dados de previsão
3. `SalesPredictionService.cs` - Implementação do serviço usando ML.NET
4. `SalesPredictionController.cs` - Controlador da API para expor a funcionalidade
5. `sales-prediction-test.html` - Página HTML para testar a funcionalidade
6. Atualização do `Program.cs` para registrar o serviço
7. Atualização do `StockApp.Application.csproj` para incluir a referência ao ML.NET

### Notas Importantes

- A implementação atual usa dados simulados para demonstração. Em um ambiente de produção, você usaria dados reais de vendas históricas.
- O modelo de machine learning é treinado em tempo real para fins de demonstração. Em um cenário real, você treinaria o modelo offline e salvaria para uso posterior.
- A API requer autenticação e autorização com a política "CanManageStock" para acessar o endpoint de previsão.

## Possíveis Melhorias Futuras

1. Implementar persistência do modelo treinado
2. Adicionar mais features ao modelo (promoções, sazonalidade, etc.)
3. Implementar retreinamento automático do modelo com novos dados
4. Adicionar visualizações gráficas das previsões
5. Implementar comparação entre vendas previstas e reais