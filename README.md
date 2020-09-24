![Banner](https://dadgroupdiag380.blob.core.windows.net/76561198061011337/csgoclubbanner2.png)  
## Introdução  
CSGO Club consiste em uma aplicação web em que jogadores de Counter-Strike: Global Offensive (CSGO) se juntam em um lobby para criar partidas em modo competitivo privado. Também é possível fazer download de replays de partidas anteriores, ver logs das partidas, além de ver o desempenho do jogador de maneira geral e em cada partida. O projeto irá funcionar com os seguintes componentes implantados na nuvem do Azure:  
* VM com Aplicação Web  
* VM com REST API e servidor(es) do jogo CSGO  
* Blob  
* Banco de Dados SQL SERVER  

## VM com Aplicação Web
A aplicação será desenvolvida no ecossistema .NET Core 3.1 usando a tecnologia ASP NET CORE para criar as páginas webs dinâmicas (Razor) interfaciadas por controllers que representarão o endpoint de cada página. As principais funcionalidades da aplicação incluem:  
* Usuário logar através da steam  
* Acessar histórico de partidas e suas informações (log, replay, estatísticas)  
* Criar salas privadas e convidar amigos  
* Iniciar partidas

## VM com REST API 
Aqui será implantada uma API que possibilitará a criação de partidas, iniciando o servidor do jogo na máquina virtual. Além disso, haverá métodos para consultar o estado da partida, ou encerrá-la. Todos esses métodos serão usados a princípio pela aplicação web, mas também podem ser usados, eventualmente, por serviços de terceiros.

## Blob 
Aqui serão armazenados arquivos mais extensos, como replay/log das partidas, imagem do perfil de usuários etc.

## Banco de Dados SQL SERVER
Aqui serão armazenados dados como: 
* Informação do usuário (Nome, URL da foto, steamId, rank, KDA , ADR)  
* Metadados da infraestrutura (IP dos servidores por exemplo)  
* Informações da partida (Id do jogador, Placar final, URL do replay/log, Status, Nome do mapa).

## Infraestrutura
![Infraestrutura](https://daddiag204.blob.core.windows.net/images/infraestruturaimg.jpeg)  
1. Usuário acessa a aplicação web.
2. Aplicação web utiliza a api para iniciar e checar o andamento das partidas
3. API se comunica com blob para salvar replays/logs de partidas
4. Aplicação web salva e recupera arquivos relacionados aos usuários e recupera arquivos relacionados às partidas
5. API salva dados da partida no servidor SQL
6. Aplicação web acessa o banco de dados com as informações do usuário
7. Outras aplicações podem se comunicar com API para iniciar suas partidas
