# Trybe Hotel

Boas-vindas ao repositório do projeto `Trybe Hotel`

Nesse projeto foi desenvolvido uma API de booking de várias redes de hotéis, trata-se de um software completo contando com recurso de autenticação e autorização por token, reserva de quartos, CRUD de hotéis, cidades, quartos e usuários. Além disso conta com um recurso especial de geolocalização sendo possível obter os hotéis mais próximos de um determinado endereço, consumindo dados da API [nominatim](https://nominatim.org/release-docs/latest)

Desenvolvido durante o período de **Aceleração C#** da Trybe 🚀

Tem por objetivo a avaliação e prática dos conhecimentos adquiridos durante a aceleração, visando o cumprimento do requisitos solicitados!

## Requisitos do projeto

Você está desenvolvendo uma API que será utilizada em uma aplicação de booking de várias redes de hotéis.

Na primeira fase deste projeto, você desenvolveu algumas rotas de entidades acerca de cidades, hotéis e quartos. Na segunda fase, você construiu rotas para o cadastro e login de pessoas clientes e o cadastro de reservas. Na terceira fase, você adicionou novas funcionalidades em rotas e adicionou serviços externos. **Agora, você irá desenvolver uma funcionalidade preparar a sua aplicação para deploy.**

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

<details id='der'>
  <summary><strong>🎲 Banco de Dados</strong></summary>
  <br/>

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

</details>

<details>
<summary><strong>🐳 Docker</strong></summary><br />

Para auxiliar no desenvolvimento, este projeto possui um arquivo do docker compose para subir um serviço do banco de dados `Azure Data Studio`. Este banco de dados possui a mesma arquitetura do `SQL Server`.

Para subir o serviço, utilize o comando:

```shell
docker-compose up -d --build
```

Para conectar ao seu sistema de gerenciamento de banco de dados, utilize as seguintes credenciais:

- `Server`: localhost
- `User`: sa
- `Password`: TrybeHotel12!
- `Database`: TrybeHotel
- `Trust server certificate`: true

Para criar o contexto do banco de dados na sua aplicação, utilize como connection string:

```csharp
var connectionString = "Server=localhost;Database=TrybeHotel;User=SA;Password=TrybeHotel12!;TrustServerCertificate=True";
```

⚠️ ** Essa connection string poderá ser utilizada no requisito 1 **

</details>

<details id='refatorando'>
  <summary>Continuando o projeto Trybe Hotel</summary>

Você já iniciou o projeto da nossa aplicação e portanto, todas as funcionalidades podem ser trazidas para não duplicar o funcionamento. Isso será muito importante, especialmente no que diz respeito ao banco de dados. Algumas models do seu banco de dados anterior serão referenciadas nas models agora, portanto, vamos trazer as funcionalidades anteriores.

Mas como fazemos isso:

Após clonar o repositório deste projeto, apenas copie e cole as funcionalidades que você construiu anteriormente:

- `Controllers`: copie todos os arquivos do diretório `Controllers` do projeto anterior e cole no diretório `Controllers` deste projeto.
- `Dto`: copie todos os arquivos do diretório `Dto` do projeto anterior e cole no diretório `Dto` deste projeto.
- `Models`: copie os arquivos referentes às models `City`, `Hotel`, `Room`, `User` e `Booking` do projeto anterior e cole no diretório `Models` deste projeto.
- `Repository`: copie os arquivos `RoomRepository`, `HotelRepository`, `CityRepository`, `UserRepository` e `BookingRepository` do projeto anterior e cole no diretório `Repository` deste projeto. Não copie as interfaces. Para o arquivo `TrybeHotelContext`, não o substitua. Apenas adicione os `DBSets` e implemente os métodos `OnConfiguring()` e `OnModelCreating()`.
- `Migrations`: Se você possui um diretório de migrations, significa que você criou migrations no projeto anterior. Não copie este diretório e crie migrations novas porque a instância do banco de dados no container não será o mesmo.
- `Services`: copie todos os arquivos do diretório `Services` do projeto anterior e cole no diretório `Services` deste projeto.
- `Testes`: No projeto de testes, você pode copiar a funcionalidade do arquivo `src/TrybeHotel.Test/IntegrationTest.cs`.

</details>
