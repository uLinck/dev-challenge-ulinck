# Desafio Umbler

Esta é uma aplicação web que recebe um domínio e mostra suas informações de DNS.

Este é um exemplo real de sistema que utilizamos na Umbler.

Ex: Consultar os dados de registro do dominio `umbler.com`

**Retorno:**
- Name servers (ns254.umbler.com)
- IP do registro A (177.55.66.99)
- Empresa que está hospedado (Umbler)

Essas informações são descobertas através de consultas nos servidores DNS e de WHOIS.

*Obs: WHOIS (pronuncia-se "ruís") é um protocolo específico para consultar informações de contato e DNS de domínios na internet.*

Nesta aplicação, os dados obtidos são salvos em um banco de dados, evitando uma segunda consulta desnecessaria, caso seu TTL ainda não tenha expirado.

*Obs: O TTL é um valor em um registro DNS que determina o número de segundos antes que alterações subsequentes no registro sejam efetuadas. Ou seja, usamos este valor para determinar quando uma informação está velha e deve ser renovada.*

Tecnologias Backend utilizadas:

- C#
- Asp.Net Core
- MySQL
- Entity Framework

Tecnologias Frontend utilizadas:

- Webpack
- Babel
- ES7

Para rodar o projeto você vai precisar instalar:

- dotnet Core SDK (https://www.microsoft.com/net/download/windows dotnet Core 6.0.201 SDK)
- Um editor de código, acoselhamos o Visual Studio ou VisualStudio Code. (https://code.visualstudio.com/)
- NodeJs v17.6.0 para "buildar" o FrontEnd (https://nodejs.org/en/)
- Um banco de dados MySQL (vc pode rodar localmente ou criar um site PHP gratuitamente no app da Umbler https://app.umbler.com/ que lhe oferece o banco Mysql adicionemente)

Com as ferramentas devidamente instaladas, basta executar os seguintes comandos:

Para "buildar" o javascript basta executar:

`npm install`
`npm run build`

Para Rodar o projeto:

Execute a migration no banco mysql:

`dotnet tool update --global dotnet-ef`
`dotnet tool ef database update`

E após: 

`dotnet run` (ou clique em "play" no editor do vscode)

# Objetivos:

Se você rodar o projeto e testar um domínio, verá que ele já está funcionando. Porém, queremos melhorar varios pontos deste projeto:

# FrontEnd

 - Os dados retornados não estão formatados, e devem ser apresentados de uma forma legível.
 - Não há validação no frontend permitindo que seja submetido uma requsição inválida para o servidor (por exemplo, um domínio sem extensão).
 - Está sendo utilizado "vanilla-js" para fazer a requisição para o backend, apesar de já estar configurado o webpack. O ideal seria utilizar algum framework mais moderno como ReactJs ou Blazor.  

# BackEnd

 - Não há validação no backend permitindo que uma requisição inválida prossiga, o que ocasiona exceptions (erro 500).
 - A complexidade ciclomática do controller está muito alta, o ideal seria utilizar uma arquitetura em camadas.
 - O DomainController está retornando a própria entidade de domínio por JSON, o que faz com que propriedades como Id, Ttl e UpdatedAt sejam mandadas para o cliente web desnecessariamente. Retornar uma ViewModel (DTO) neste caso seria mais aconselhado.

# Testes

 - A cobertura de testes unitários está muito baixa, e o DomainController está impossível de ser testado pois não há como "mockar" a infraestrutura.
 - O Banco de dados já está sendo "mockado" graças ao InMemoryDataBase do EntityFramework, mas as consultas ao Whois e Dns não. 

# Dica

- Este teste não tem "pegadinha", é algo pensado para ser simples. Aconselhamos a ler o código, e inclusive algumas dicas textuais deixadas nos testes unitários. 
- Há um teste unitário que está comentado, que obrigatoriamente tem que passar.
- Diferencial: criar mais testes.

# Entrega

- Enviei o link do seu repositório com o código atualizado.
- O repositório deve estar público para que possamos acessar..
- Modifique Este readme adicionando informações sobre os motivos das mudanças realizadas.

---

# Modificações Realizadas no Projeto

Minhas mudanças no projeto foram focadas em:
- definir uma arquitetura que faça sentido com o escopo do projeto
- organização do código
- separação de responsabilidades
- validação de dados
- padronização de respostas da API
- implementação de testes e aprimoramento do frontend utilizando Blazor Server.

Abaixo estão os detalhes das modificações realizadas:

---

## Arquitetura

1. **Abordagem Vertical Slice modular**
   - Estruturei o projeto para uma abordagem de arquitetura mais voltada ao Vertical Slice modular
   - Essa abordagem é mais condizente ao porte do projeto e facilita a manutenção
   - Organizei por features ao invés de camadas tradicionais (Controllers, Services, Models)
   - Cada feature contém tudo relacionado ao seu contexto: DTOs, Validators, Extensions, Services e etc.

2. **Organização visando separação de responsabilidades**
   - `Features/DomainContext/`: Toda lógica relacionada a domínios
   - `Persistence/`: Camada de persistência de dados isolada
   - `Controllers/`: Apenas roteamento de requisições
   - `Shared/`: Componentes compartilhados (Enums, Layouts, Services)
   - `Components/`: Componentes Blazor do frontend
   - `Pages/`: Páginas Blazor do frontend

---

## Controllers

1. **Redefinição da lógica do controller**
   - O controller da aplicação estava com muita lógica implementada diretamente nele, tornando difícil a manutenção e a testabilidade.
   - Para resolver isso, eu criei um serviço que agora contém toda a lógica de negócio relacionada ao domínio.
   - O papel da `Controller` foi reduzido para apenas receber as requisições que chegam na API e delegar para a serviço processar.
   - Essa mudança permite realizar testes mais simples (possibilitando mock) e maior controle sobre as respostas da API.
   - Diminuiu também a carga cognitiva caso seja feita alguma manutenção.

2. **Criação de DTOs**
   - Criei uma classe (`DomainDto`) pra retornar pro frontend só os dados necessários do domínio
   - Além de facilitar para o frontend, isso isola a entidade do banco de dados da API e evita expor dados desnecessários

3. **Padronização de respostas da API**
   - Criei a classe `BaseController` com método `ToResponse()` para padronizar retornos
   - Implementei a classe `ApiResponse<T>` para garantir que todas as respostas da API sigam o mesmo formato
   - Isso facilita o tratamento de respostas no frontend e torna a API mais previsível

4. **Validação em múltiplas camadas**
   - Implementei validadores robustos (`DomainValidator` e `TldValidator`) que validam o domínio antes de chegar no serviço
   - A validação acontece antes de consultar os dados no banco
   - O domínio não chega no serviço com dados inválidos
   - Validações incluem: formato, caracteres especiais, TLD válido segundo IANA, comprimento, hífens, etc

---

## Models

1. **Organização da estrutura de pastas**
   - Organizei a estrutura de pastas das models, separando em responsabilidades:
     - `Features/DomainContext/Dto/`: DTOs para transferência de dados
     - `Persistence/Models/`: Entidades do banco de dados
   - Isso deixa claro o que é retornado pela API versus o que é armazenado no banco

---

## Persistence

1. **Organização da camada de persistência**
   - Guardei aqui tudo relacionado a acesso a banco de dados, para deixar mais fácil de entender a arquitetura do projeto
   - Estrutura criada:
     - `Persistence/Models/`: Entidades
     - `Persistence/DatabaseContext.cs`: Contexto do EF Core
     - `Persistence/Migrations/`: Migrações do banco

2. **Melhorias na entidade Domain**
   - Adicionei construtor específico para criação
   - Criei método `Update()` para atualização dos dados
   - Separei responsabilidades de criação e atualização

---

## Services

1. **Criação do DomainService**
   - Criei uma serviço para armazenar toda a lógica de buscar, cadastrar e alterar domínios
   - Ele é responsável por:
     - Validar domínios usando `DomainValidator`
     - Normalizar domínio para lowercase antes de consultar cache
     - Consultar cache no banco de dados
     - Verificar TTL para saber se cache é válido
     - Fazer consultas DNS (usando `DnsClient`)
     - Fazer consultas WHOIS (usando `WhoisClient`)
     - Salvar/atualizar dados no banco

2. **Injeção de dependências para facilitar testes**
   - Injetei os serviços necessários para que eu possa mockar eles nos testes unitários do `DomainService`
   - Configurei lifetimes corretos

3. **Abstração do WhoisClient**
   - O `WhoisClient` da biblioteca `Whois.NET` é estático, impossibilitando mock direto
   - Portanto, criei um wrapper que implementa a interface, permitindo injeção de dependência
   - Isso torna o `DomainService` completamente testável sem depender de chamadas reais ao WHOIS
   - Registrado no container de DI como Singleton

4. **Implementação de logging**
   - Adicionei `ILogger` no `DomainService` para logar qualquer erro que aconteça na execução
   - Logs incluem:
     - Validações que falharam
     - Erros ao buscar dados externos (DNS/WHOIS)
     - Informações de domínios consultados
   - Esses erros poderiam ser consultados caso fosse uma aplicação real em: Um arquivo, DB ou sistema de observabilidade

5. **Refatoração e isolamento de código**
   - Refatorei a lógica de consulta do domínio e isolei numa função `FetchWhoIsAsync()`
   - Posso usar ela várias vezes sem duplicar código
   - Deixa o código mais fácil de entender e manter
   - Implementei método `IsDomainStale()` para verificar se cache expirou

6. **Padronização de resultados com Result Pattern**
   - Criei `ServiceResult<T>` para padronizar retornos do serviço de forma tipada e segura
   - Implementei classe estática `Result` com métodos auxiliares para facilitar criação:
     - `Ok(T data)`: sucesso com dados
     - `ValidationError(string)`: erro de validação
     - `Error(string)`: erro de servidor
     - `NotFound(string)`: recurso não encontrado
   - Incluí `ServiceErrorTypeEnum` para diferenciar tipos de erro de forma clara
   - Pattern permite melhor tratamento de erros e propagação de resultados sem exceções

---

## Extensions

1. **DomainExtension**
   - Criei uma extension para converter entidade `Domain` em `DomainDto`
   - Facilita a conversão e mantém o código limpo
   - Centraliza a lógica de mapeamento em um único lugar
   - Evita repetição de código ao converter entidades para DTOs
   - Decidi não utilizar AutoMapper para manter o projeto simples e evitar dependências desnecessárias

---

## Validators

1. **DomainValidator**
   - As validações seguem o formato tradicional de domínios conforme as RFCs 1035 e 3696, garantindo que o nome seja sintaticamente válido para uso em DNS.
   - O validador não está barrando o uso de acentuação (ex: café.com, müller.de), pois a lib utilizada converte automaticamente o domínio para o formato ASCII compatível (punycode) antes de realizar as consultas DNS e WHOIS.
   - Implementei validações robustas:
     - Verifica se está vazio ou contém espaços
     - Valida comprimento (3-253 caracteres)
     - Verifica se tem extensão válida (.com, .br, etc)
     - Valida cada parte (label) do domínio:
       - Máximo 63 caracteres por label
       - Não pode começar/terminar com hífen
       - Não pode ter hífens consecutivos (--)
       - Não pode conter apenas números
     - Usa regex para validar formato geral
   - Retorna tupla `(bool isValid, string error)` com mensagens claras em português visando retornar-las na UI

2. **TldValidator**
   - Implementei validador usando lista oficial da IANA
   - Carrega arquivo `tlds-alpha-by-domain.txt` (1500+ extensões)
   - Usa `HashSet<string>` para performance O(1)
   - Validação case-insensitive
   - Garante que apenas TLDs reais são aceitos (rejeita .fake, .invalid, etc)

---

## Tests

**Observação:** Implementei novos testes focados em validar as regras de negócio e validações implementadas, priorizando testes que realmente verificam comportamentos esperados da aplicação.

1. **Testes unitários para validadores**
   - Adicionei `DomainValidatorTests.cs` com cobertura completa:
     - Testa domínios válidos: comuns, com hífens, com acentos, subdomínios
     - Testa domínios inválidos: sem extensão, com espaços, caracteres especiais, TLD inválido
     - Testa regras específicas: apenas números, hífens inválidos, comprimento
   - Adicionei `TldValidatorTests.cs`:
     - Verifica carregamento da lista IANA (>1000 TLDs)
     - Testa TLDs comuns (com, br, net, org, io)
     - Testa case insensitive
     - Testa TLDs inválidos

2. **Testes de integração para controllers**
   - Adicionei `ControllersTests.cs`:
     - Testa cache válido (retorna do banco sem chamar DNS)
     - Testa domínio inválido (retorna erro 422)
     - Testa mock do WhoisClient para consultas WHOIS - Teste Obrigatório
     (tive que fazer algumas alterações para adaptar ao código, mas o objetivo do teste segue o mesmo)
     - Usa `InMemoryDatabase` para não depender de MySQL
     - Faz mocks das dependências da service

3. **Implementação de mocks testáveis**
   - Todos os testes conseguem mockar completamente as dependências externas (DNS e WHOIS)
   - Testes isolados não fazem chamadas reais para servidores externos
   - Verificação de que os mocks são chamados corretamente usando `Moq.Verify()`

---

## Frontend

1. **Migração para Blazor Server**
   - Migrei o frontend para Blazor e adicionei configurações necessárias
   - Utilizei Blazor para chamar o serviço de domínio diretamente através de injeção de dependência visando abordagem moderna e prática
   - Isolei componentes da aplicação, deixando a estrutura mais moderna
   - Criei `Index.razor` como componente principal de busca
   - Separei responsabilidades utilizando uma estrutura intuitiva dentro do contexto de componentes
   - Implementei as configurações necessárias no Startup.cs

2. **Estilização**
   - Estilizei o frontend para manter a identidade visual da Umbler
   - Input arredondado com estilo moderno
   - Cards com informações bem organizadas
   - Layout responsivo

3. **Melhorias de UX**
   - Quando pesquisar um domínio, enquanto realiza a pesquisa o usuário não pode atualizar a resposta
   - Adicionei estado de loading com spinner
   - Botões desabilitados durante carregamento
   - Feedback visual claro para o usuário

4. **Apresentação de dados**
   - Exibe: Nome do Domínio, IP e Onde está Hospedado

5. **Funcionalidades adicionais**
   - Adicionei função para pesquisar o domínio ao apertar Enter
   - Botões de extensão rápida (.com, .br, .net, .org, .app)
   - Validação de entrada no frontend

6. **Tratamento de erros**
   - Erros de validação aparecem em amarelo (400)
   - Erros de servidor aparecem em vermelho (500)
   - Mensagens claras em português para o usuário

7. **Preservação de compatibilidade**
   - Preservei o endpoint da API caso ele seja utilizado por outras aplicações (app mobile, por exemplo)
   - API e Frontend funcionam independentemente

8. **Observação**
   - Nunca havia mexido em blazor anteriormente, mas tentei dar o meu melhor já que adorei a experiência e espero ter oportunidade de mexer novamente com essa tech :)

---

## Documentação da API (Swagger)

- Configurei para termos acesso ao Swagger da aplicação
- Adicionei Swagger para documentação automática da API
- Disponível em `/swagger` quando rodando em desenvolvimento
- Facilita testes e entendimento dos endpoints
- Configurado em `Startup.cs` com suporte a XML comments

---

## Resumo das Melhorias Implementadas

### Arquitetura ✅
- Vertical Slice modular para melhor organização
- Estrutura por features ao invés de camadas tradicionais
- Separação clara de responsabilidades

### Backend ✅
- Separação de responsabilidades com `DomainService`
- DTOs dedicados separados das entidades do banco (`DomainDto`, `WhoIsResult`)
- Padronização de respostas da API (`ApiResponse<T>`)
- Implementação do Result Pattern (`ServiceResult<T>` e `Result`)
- Extension methods para conversão de entidades (`DomainExtension`)
- Validação de domínios (`DomainValidator`)
- Validação de TLD usando lista oficial IANA (`TldValidator`)
- Validação antes de consultar banco de dados
- Normalização de domínios para lowercase (cache consistente)
- Injeção de dependências correta
- Abstração do WhoisClient com `IWhoIsClient` para testabilidade
- Logging estruturado com `ILogger`
- Tratamento de erros tipado com enums (Validation, ServerError, NotFound)

### Estrutura ✅
- Organização por features (`Features/DomainContext/`)
- Separação clara de responsabilidades
- Pasta `Persistence` para persistência de dados
- Enums em pasta dedicada (`Shared/Enum/`)
- Services compartilhados (`Shared/Services/`)
- Estrutura de pastas intuitiva

### Testes ✅
- Testes unitários de validadores
- Testes de integração de controllers
- Uso de InMemoryDatabase
- Mocks completos de dependências externas (DNS, WHOIS, Logger)
- Cobertura ampla de cenários

### Frontend ✅
- Migração completa para Blazor Server
- Identidade visual da Umbler preservada
- Estados de loading, sucesso e erro
- Validação de entrada
- Busca ao pressionar Enter
- Botões de extensão rápida
- Layout moderno e responsivo
- Configurações Blazor adequadas

### Observabilidade ✅
- Swagger para documentação da API
- Logging estruturado
- Mensagens de erro claras
- Rastreamento de operações

---

