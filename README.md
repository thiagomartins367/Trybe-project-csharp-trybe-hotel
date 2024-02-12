# Trybe Hotel

Boas-vindas ao repositório do projeto `Trybe Hotel`

Nesse projeto foi desenvolvido uma API de booking de várias redes de hotéis, trata-se de um software completo contando com recurso de **autenticação e autorização por token**, reserva de quartos e CRUD de hotéis, cidades, quartos e usuários. Além disso conta com um recurso especial de **geolocalização** sendo possível obter os hotéis mais próximos de um determinado endereço através do consumo de dados da API [nominatim](https://nominatim.org/release-docs/latest).

Desenvolvido durante o período de **Aceleração C#** da Trybe 🚀

Tem por objetivo a avaliação e prática dos conhecimentos adquiridos durante a aceleração, visando o cumprimento dos requisitos solicitados!

## Fases do projeto 🔨
O projeto foi construído em 4 fases em que cada fase consistia em desenvolver um novo recurso da aplicação no repositório correspondente daquela fase e adicionar o código finalizado da fase anterior na atual e assim por diante até que a API estivesse completa na última fase. As fases foram nomeadas seguindo a ordem das letras do alfabeto.

<br />

- **Fase A - Recursos básicos 🏁** 
  - Nessa fase foi desenvolvido o CRUD inicial de cidades, hotéis e quartos disponíveis. Além disso, foram criados testes que cobrissem no mínimo 40% das linhas de código.
<br />

- **Fase B - Segurança e rotas de usuários 🔐**
  - Nessa fase foi desenvolvido o recurso de segurança da API utilizando tokens _Bearer_ para autenticação e autorização do usuário, além de criar as rotas de cadastro e login. Também foram criadas novas rotas dos CRUDs iniciados na fase anterior, bem como o recurso de **booking** (reservas) de quartos de hotéis. Além disso, foram criados testes que cobrissem no mínimo 40% das linhas de código.
<br />

- **Fase C - Geolocalização 📍🌎**
  - Nessa fase foi desenvolvido um recurso especial de **geolocalização** que permite buscar os hotéis mais próximos baseando-se em um endereço que o usuário forneça ordenando o resultado dessa consulta do hotel mais próximo para o menos próximo. Esse recurso utiliza o serviço externo [nominatim](https://nominatim.org/release-docs/latest) para obter os dados de latitude e longitude do endereço informado. Foi nescessário refatorar alguns recursos anteriormente desenvolvidos para que essa nova funcionalidade pudesse ser implementada.
<br />

- **Fase D - Status e deploy 🚀**
  - Nessa fase foi desenvolvido um recurso para obter o status da API, nescessário para saber se ela está ou não online. Também foi criado um Dockerfile da aplicação para o deploy no [Railway](https://railway.app/).

## Uso no Docker 🐋
Se você possuir o [Docker](https://www.docker.com) e o [Docker compose](https://docs.docker.com/compose/install) instalados, você pode economizar muito trabalho na configuração do ambiente de produção.

1. Para iniciar todo o ambiente de produção no Docker execute o comando:
```
docker-compose -f docker-compose.prod.yml up -d
```
- Após esse processo a API estará executando no container `trybe-hotel`, o banco de dados no `db-trybe-hotel` e um container auxiliar também será iniciado para executar alguns comandos nescessários do _Entity Framework (EF)_ antes de começar a utilizar a aplicação.
<br />

2. Em seguida acesse o CLI do container `entity-framework-trybe-hotel` e execute os seguintes comandos:
- Visualizar _Migrations_ pendentes
```
dotnet ef migrations list
```

- Criar e/ou atualizar o banco de dados com as _Migrations_ pendentes
```
dotnet ef database update
```
<br />

3. Após todo esse processo o banco de dados, bem como suas tabelas, estarão criados e prontos. A partir desse momento o container `entity-framework-trybe-hotel` não será mais nescessário, logo, ele pode ser removido com o comando:
```
docker rm -fv entity-framework-trybe-hotel
```

- Para remover a imagem execute:
```
docker image rm entity-framework-trybe-hotel
```

- Caso precise executar novos comandos do _Entity Framework (EF)_ no ambiente de produção utilize esse container podendo removê-lo logo em seguida, ou não, sempre que nescessário.

## Instalação e Uso 🖥️

⚠️ É necessário ter instalado o [.NET Framework](https://dotnet.microsoft.com/pt-br) (Windows) ou [.NET Core](https://dotnet.microsoft.com/pt-br/) (Linux/ Mac) em sua máquina para executar a API.

⚠️ É nescessário possuir o [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) instalado e configurado ou possuir o mesmo em nuvem para que a aplicação possa se conectar e manipular os dados.
- O SQL Server só funciona no sistema operacional Windows, para que ele possa ser usado em outro SO será nescessário utilizar o Docker como ambiente de execução do banco de dados.

⚠️ É nescessário instalar também o [CLI do Entity Framework](https://www.nuget.org/packages/dotnet-ef) em sua máquina para executar as _Migrations_ do banco de dados.

**Na raiz do projeto execute os comandos abaixo no seu terminal:**

1. Instale as dependências
```
dotnet restore ./src
```
<br />

2. Execute os seguintes comandos do _Entity Framework (EF)_:

&nbsp;&nbsp;&nbsp;⚠️ No **Windows** em vez de usar `export ASPNETCORE_ENVIRONMENT=Production` utilize `set ASPNETCORE_ENVIRONMENT=Production`.

&nbsp;&nbsp;&nbsp;⚠️ Sempre que executar comandos do _Entity Framework (EF)_ no ambiente local defina a variável `ASPNETCORE_ENVIRONMENT` junto do comando.

- Visualizar _Migrations_ pendentes
```
export ASPNETCORE_ENVIRONMENT=Production && dotnet ef migrations list --project ./src/TrybeHotel/TrybeHotel.csproj
```

- Criar e/ou atualizar o banco de dados com as _Migrations_ pendentes
```
export ASPNETCORE_ENVIRONMENT=Production && dotnet ef database update --project ./src/TrybeHotel/TrybeHotel.csproj
```
<br />

3. Execute a aplicação

&nbsp;&nbsp;&nbsp;⚠️ Certifique-se de que o banco de dados, bem como suas tabelas, estejam criados.
```
dotnet run --project ./src/TrybeHotel/TrybeHotel.csproj --environment Production
```

## Desenvolvimento 🧑‍💻
Para desenvolver novos recursos ou refatorar é recomendado o uso do [Docker](https://www.docker.com) e do [Docker compose](https://docs.docker.com/compose/install), pois eles fornecem um ambiente isolado e devidamente configurado no arquivo `docker-compose.dev.yml`.

⚠️ É necessário ter o [Git](https://git-scm.com) instalado em sua máquina para o controle de versão da API.

**Na raiz do projeto execute os comandos abaixo no seu terminal:**
1. Crie e entre em uma nova *branch* de desenvolvimento
```
git checkout -b nome-da-branch
```
<br />

2. Crie o ambiente Docker de desenvolvimento
```shell
docker-compose -f docker-compose.dev.yml up -d
```
- Após esse processo a API estará disponível em seu `http://localhost:5057` e executando no container `dev_trybe-hotel`.
<br />

3. É possível executar os testes da API no container `test_trybe-hotel` com o comando `dotnet test` na CLI do container ou exeutar testes específicos junto do parâmetro `--filter`, veja alguns exemplos desses comandos no arquivo `Makefile`.
<br />

<details>
  <summary><strong>:open_file_folder: Estrutura de arquivos</strong></summary>
  <br />

O sistema está dividido em diretórios específicos para auxiliar na organização e desenvolvimento do projeto.

- `Controllers/`: Este diretório armazena os arquivos com as lógicas dos controllers da aplicação que gerenciam as requisições recebidas e as respostas enviadas pela API.
<br />

- `Models/`: Este diretório armazena os arquivos com as models do banco de dados. As models `City`, `Hotel`, `Room`, `User` e `Booking` são os modelos usados para as tabelas `Cities`, `Hotels`, `Rooms`, `Users` e `Bookings`.
<br />

- `DTO/`: Este diretório armazena as classes de DTO. Responsáveis pela transferência de dados entre camadas da aplicação evitando acessos indevidos a dados sigilosos.
<br />

- `Repository/`: Este diretório armazena as lógicas que farão a interação com o banco de dados. Além disso, existe nesse mesmo diretório, o arquivo `TrybeHotelContext` com o contexto para a conexão com o banco de dados. Todos os `repository` e o `context` possuem interfaces que estão nesse diretório e fornecem o contrato para essas classes.
<br />

- `Services`: Este diretório armazena os serviços responsáveis pela geração de token e pelo serviço de geolocalização.

---
</details>

<details>
  <summary><strong>🎲 Banco de Dados</strong></summary>
  <br />

  Esse projeto conta com o *Diagrama de Entidade-Relacionamento (DER)* usado na modelagem do banco de dados.
    
![banco de dados](img/der.png)

  O diagrama infere 05 tabelas:
  - ***Cities***: tabela que armazena um conjunto de cidades nas quais os hotéis estão localizados.
  - ***Hotels***: tabela que armazena os hotéis da aplicação.
  - ***Rooms***: tabela que armazena os quartos de cada hotel da aplicação.
  - ***Users***: tabela que armazena as pessoas usuárias do sistema.
  - ***Bookings***: tabela que armazena as reservas de quartos de hotéis.

  Acerca dos relacionamentos, pelo diagrama de entidade-relacionamento temos:
  - Uma cidade pode ter vários hotéis.
  - Um hotel pode ter vários quartos.
  - Uma pessoa usuária pode ter várias reservas.
  - Um quarto pode ter várias reservas.

---
</details>

Para adicionar as alterações da nova branch de desenvolvimento na branch principal ```main``` é nescessário criar um *Pull Request* neste repositório.

🚫 Alterações diretas na branch ```main``` estão bloqueadas.

✅ O uso dos comandos `make` listados no arquivo `Makefile` é recomendado para acelerar o processo de criação e remoção dos containers dev e/ou produção ou na execução de scripts `dotnet` no caso de não usar o Docker como ambiente de desenvolvimento.

## Contribuidores 🤝

- [THIAGO MARTINS](https://github.com/thiagomartins367) - criador e mantenedor
