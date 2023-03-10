# ASP.NET CORE


- Uma requisição é direcionada a uma ACTION

- Classe Controller -> Fornece métodos ACTION -> Que respondem as requisições HTTP

- View(): Implementado na classe controller - possui sobrecargar
	- Sem argumentos: retorna uma visão com o mesmo nome da ACTION
	- Com Argumentos:
		- Pode representar o nome da View, ou mesmo dados que devem ser enviados a View
		
# ShortCuts do VSCode

	Duplica o cursor para a linha de baixo:
		CTRL + Shift + Alt + Seta para baixo (+vezes = +linhas)
		
	Seleciona palavras iguais:
		1x:
			Ctrl + D
		todas iguais de uma vez só:
			Ctrl + Shift + L
			
	Comandos Ctrl + P - Linha de Comando.
		Digitar o > (caso já não apareça:
		
			Abrir o Nuget:
			
			Lowercase:
				> Transforma do Lowercase
				
				

==========================================================================================================================		
# EF Core - Entity Framework

	================================
	CONTEXTO

	Classe de contexto
		- Pode ter qualquer nome, mas será utilizado por toda a solução - Indicado conter a palavra "Context";
		- É a classe onde estarão declarados quais classes da solução serão tratadas como entidades
		- Onde estarão as configurações de acesso a base de dados
		- IMPORTANTE: É importante criar uma pasta separada no projeto onde ficará a classe de contexto (Ex: Pasta Data)
		
		A classe de contexto deve herdar DbContext (Microsoft.EntityFrameworkCore)
			- Caso seja utilizado o Identity para autenticação, será utilizada IdentityDbContext<ClasseDeUsuáriodaAplicação>
			
				

	Bibliotecas necessárias (Nuget)
	(using)
	Microsoft.EntityFrameworkCore; 				**** Nesta está a tratativa do contexto Contem Microsoft.EntityFrameworkCore.DbContext;   
	Microsoft.EntityFrameworkCore.Relational;  	**** Entidades Relacionais
	Microsoft.EntityFrameworkCore.SqlServer;   	**** Microsoft Sql Server
	
	Construtor:
		public IESContext(DbContextOptions<IESContext> options) : base(options)
        {

        }
	
	
	Tipo de dados DbSet
		- Serão as propriedados do contexto que identificação as classes de entidade.
		- public DbSet<Classe> NomeDaEntidade {get;set;}
		
		public DbSet<Instituicao> Instituicoes { get; set; }
		
		- Para sobreescrever o nome da tabela do banco de dados para a Entidade do modelo.
		- No método OnModelCreating (override) 
		
			<codigo>
				protected override void OnModelCreating(ModelBuilder modelBuilder)
				{
					base.OnModelCreating(modelBuilder);
					modelBuilder.Entity<Departamento>().ToTable("Departamento");
				}
			</codigo>
			
			
	Exemplo classe de contexto:
	<codigo>
			using Microsoft.EntityFrameworkCore;
			using System;
			using System.Collections.Generic;
			using System.Linq;
			using System.Threading.Tasks;
			using _20210728Capitulo01.Models.Infra;
			using Modelo.Models;
			using Modelo.Discente;
			using Modelo.Docente;
			using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

			namespace _20210728Capitulo01.Data
			{
				public class IESContext : IdentityDbContext<UsuarioDaAplicacao>
				{
					public IESContext(DbContextOptions<IESContext> options) : base(options)
					{

					}

					protected override void OnModelCreating(ModelBuilder modelBuilder)
					{
						base.OnModelCreating(modelBuilder);
						modelBuilder.Entity<Departamento>().ToTable("Departamentos");

						modelBuilder.Entity<CursoDisciplina>().HasKey(cd => new { cd.CursoID, cd.DisciplinaID });
						modelBuilder.Entity<CursoDisciplina>().HasOne(c => c.Curso).WithMany(cd => cd.CursoDisciplinas).HasForeignKey(c => c.CursoID);
						modelBuilder.Entity<CursoDisciplina>().HasOne(d => d.Disciplina).WithMany(cd => cd.CursoDisciplinas).HasForeignKey(d => d.DisciplinaID);    

						modelBuilder.Entity<CursoProfessor>().HasKey(cp => new { cp.CursoID, cp.ProfessorID });
						modelBuilder.Entity<CursoProfessor>().HasOne(c => c.Curso).WithMany(cp => cp.CursoProfessores).HasForeignKey(c => c.CursoID);
						modelBuilder.Entity<CursoProfessor>().HasOne(p => p.Professor).WithMany(cp => cp.CursoProfessores).HasForeignKey(p => p.ProfessorID);

					}

					public DbSet<Instituicao> Instituicoes { get; set; }
					public DbSet<Departamento> Departamentos { get; set; }

					public DbSet<Curso> Cursos { get; set; }
					public DbSet<Disciplina> Disciplinas { get; set; }        

					public DbSet<Academico> Academicos { get; set; }

					public DbSet<Professor> Professores { get; set; }

				}

			}
	</codigo>

	=============================
	CONFIGURANDO O ACESSO AO BANCO DE DADOS PARA A SOLUÇÃO:
	
	
		++++++++++
			ConnectionString no appsettings.json(Antes de Logging):
			
			 "ConnectionStrings": {
				"IESConnection": "Server=LAPTOP-MQIANRJV\\SQL2008R2;Database=IESUtfpr;Trusted_Connection=True;MultipleActiveResultSets=true"
			  },

		++++++++++
		Adicionar no startup.cs o Contexto do Banco de dados:
		
		no método ConfigureServices(IServiceCollection services):
		
			services.AddDbContext<NomeDaClasseDeContexto>(options => options.UseSqlServer(Configuration.GetConnectionString("IESConnection"))); //Nome da ConnectionString configurada no arquivo appsettings.json
				
		
		+++++++++++
			
		
		Caso desejado, se for configurar os dados de conexão na classe de contexto, pode ser feito do seguinte modo:
		<codigo>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;D
			atabase=IESCasaDoCodigo;Trusted_Connection=True;MultipleActiveRes
			ultSets=true");
		}
		</codigo>
		
		O ASP.NET Core implementa a Injeção de Dependência
		(Dependency Injection, ou DI) por padrão. Injeção de Dependência
		é um pattern que traz um recurso necessário para a aplicação,
		assim, ela não precisa buscar por este serviço. O código anterior
		não trabalha a injeção de dependências, pois nossa aplicação, com
		ele, vai buscar o serviço.
		
		Porém podemos fazer essa configuração no startup do sistema, com o ConnectionString no arquivo de configurações appsettings.json
		
=============================
## MIGRATIONS
		Ativando o MIGRATIONS:
		
		Dependencias (Nuget):
			Microsoft.EntityFrameworkCore.Design
			
		Executar o Command Prompt(Cmd) na pasta da solução (botão direito no projeto -> Open Folder in File Explorer)
		Para desinstalar a versão atual:
			dotnet tool uninstall dotnet-ef -g
			
		Para instalar a versão mais atual:
			dotnet tool install --global dotnet-ef --version 3.1.4
		
		Para iniciar a base de dados	
			dotnet ef migrations add InitialCreate
		
		Após isto, será criada a pasta Migrations dentro da solução.
		Para executar a alteração no banco de dados
			dotnet ef database update
		
		Parq que as alterações que ocorrerem nas classes de entidade sejam refletidas no banco de dados, executar:
		A criação da migration:
			dotnet ef migration add [Descrição da alteração]
			
		Database Update:
			dotnet ef database update	
			
			Caso a soluções tenha varios projetos associados
				e o projeto de persistencia estiver separado do projeto da API (do framework), deve ser informado no database update o projeto source
				
				dotnet ef database update -s ProEventos.API 
					(Eu acho que isso é para o SQLite, para definir onde será criado o db do Sqlite, porque no SQL Server, os arquivos de banco de dados estão no servidor)
					
					
		
		
		=== COMO executar Migrations quando o projeto de persistencia está separado do projeto onde está o framework (WebApi, MVC, etc)
		
			Deveremos informar ao migrations qual o projeto onde está o contexto
				-p NomeDoProjeto
			Deveremos informar ao migrations qual o projeto onde o startUp está (projeto onde o Framework é executado):
				-s NomeDoProjeto
				
			Então ficamos assim:
			
				Adicionando nova:
					dotnet ef migrations add Initial -p ProEventos.Persistence -s ProEventos.API
														NomedoProjetoClassLib     NomeDoProjetoWebAPI-MVC
														
					dotnet ef migrations add Adicionando-Identity -p ProEventos.Persistence -s ProEventos.API
				
				Atualizando o banco de dados:
					dotnet ef database update -s ProEventos.API
			
			
												
												
												
			
				
=============================			
## RELACIONAMENTOS
	
		Nas classes de entidades é possível identificar relacionamentos a outras classes.
		Deve se definir se o relacionamento será 1 para muitos, ou muitos para muitos.

		Quando é feito um relacionamento de um para muitos
			Na classe onde é vinculada 1 deve ser criadas as propriedades do ID (ForeigKey e relacionamento com a classe relacionada)
			Ex: 
				public long? PedidoID {get;set;}
				public Pedido Pedido {get;set;}
				
				Neste Exemplo, supomos uma Classe ItemDoPedido que se vincula ao pedido, pelo PedidoID. O relacionamento do tipo Pedido nos permite trazer os dados do pedido na consulta.
				
			Na classe onde será vinculada o relacionamento para Muitos, é criado um virtual ICollecion<Classe>
			Ex:
				public virtual ICollection<Item> Itens {get;set;}
				
				Neste exemplo, suponde uma classe Pedido.cs, temos a coleção do tipo ItensDoPedido.cs que pode ser carregada para listar os itens do pedido. 
	
		Quando temos uma relação de muitas para muitos:
			Por exemplo,
			Curso se relaciona a muitas disciplinas
			Disciplinas se relaciona a muitos cursos. 
			
			Deve existir uma classe intermediaria (CursoDisciplina) que faz o relacionamento entre as classes.
			
			Em curso teremos:
				public virtual ICollection<CursoDisciplina> CursoDisciplinas { get; set; }
				
			Em Disciplinas teremos:
				public virtual ICollection<CursoDisciplina> CursoDisciplinas { get; set; }
				
			E teremos a Classe CursoDisciplina.cs, onde teremos:
				public long? CursoID {get;set;};
				public Curso Curso {get;set;}
				
				public long? DisciplinaID {get;set;}
				public Disciplina Disciplina {get;set;}
				
			Para este caso, não vinculamos no contexto um DbSet para a classe que faz o vinculo, ao invés disso, dizemos ao contexto no metodo OnModelCreating que há um vinculo de muitos para muitos, do seguinte modo:
			
			modelBuilder.Entity<CursoDisciplina>().HasKey(cd => new { cd.CursoID, cd.DisciplinaID });			//Entidade CursoDisciplina tem chave X
            modelBuilder.Entity<CursoDisciplina>().HasOne(c => c.Curso).WithMany(cd => cd.CursoDisciplinas).HasForeignKey(c => c.CursoID); // CursoDisciplina se vincula a um curso, com muitas CursoDisciplina, e tem chave estrangeira
            modelBuilder.Entity<CursoDisciplina>().HasOne(d => d.Disciplina).WithMany(cd => cd.CursoDisciplinas).HasForeignKey(d => d.DisciplinaID); //CursoDisciplina se vincula a uma disciplina, com muitos CursoDisciplina, com chave estrangeira
============================
# LINQ
		O LINQ faz parte do EF Core para nos carregar os dados gravados no banco de dados para nossos objetos criados a partir das nossas classes de Entidade.
		
		Temos um padrão chamada DAL (Data Access Layer) onde criamos em uma pasta separada nossos códigos para efetuar a busca de nossos dados, ou salvar novos dados.
		
		Retornar uma lista de dados do tipos IQueryable (ordenada pelo campo Nome e incluindo o relacionamento):
			_context.Departamentos.Include(i => i.instituicoes).OrderBy(d => d.Nome);
			
		Retornar um objeto buscando pelo valor de um campo (ID no exemplo):
			Departamento departamento = await _context.Departamentos.SingleOrDefaultAsync(d => d.DepartamentoID == id);
			
			Adendo:
			No caso de departamento, podemos carregar as instituição que esta relaciona ao departamento:
				_context.Instituicoes.Where(i => departamento.InstituicaoID == i.InstituicaoID).Load();
				
			return Departamento; //(Já vai carregado com a instituição vinculada)
	
		Adicionar ao contexto (banco de dados):
			_context.Departamentos.Add(departamento);			
			await _context.SaveChangesAsync(); //Commit
			
		Alterar um entrada existente:
			_context.Update(departamento);
			await _context.SaveChangesAsync();	//Commit
	
		Excluir um registro:
			Departamento departamento = await ObterDepartamentoPorID(id);
            _context.Departamentos.Remove(departamento);
            await _context.SaveChangesAsync();
			
		Obter um registro buscando pela chave estrangeira vinculada a este mesmo:
		//Exemplo: Obter uma lista de departamentos que estão vinculadas a uma certa instituição:
			IQueryable<Departamento> departamentos = _context.Departamentos.Where(d => d.InstituicaoID == InstituicaoID).OrderBy(d => d.Nome); //Ordenado por Nome
			
			
		//Adendo:
			Como buscar com operador de negação.
			Ex: Curso é vinculado de muitos-para-muitos com Professor.
				Preciso saber, quais os professores que não estão vinculados ao curso, e listar estes professores.
				
				//Listar o curso desejado - incluindo o relacionamento com CursoProfessor (muitos para muitos)
				Curso curso = context.Cursos.Where(c => c.CursoID == cursoID).Include(cp => cp.CursoProfessores).First();
				//obter os professores que estão vinculados a este curso
				//Em forma de array ? Devemos testar se da pra ser em formato de List
				long?[] professoresDoCurso = curso.CursoProfessores.Select(cp => cp.ProfessorID).ToArray();
				//buscar na entidade professor os registros que não estão incluidos acima
				var professoresForaDoCurso = context.Professores.Where(p => !professoresDoCurso.Contains(p.ProfessorID)); //Atenção ao operador ! negando o methodo Contains
				
		//Encontra pelo ID		
		context.Users.FindAsync(id);
		//Busca com Where sem Where por string 
		context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.UserName == username.ToLower());
		
		//Retorna todos
		var query = context.Users;
        return await query.AsNoTracking().ToListAsync();
		
		//IQueryable
		IQueryable<Lote> query = context.Lotes.Where(e => e.EventoId == eventoId);            
        return await query.AsNoTracking().OrderBy(lote => lote.Id).ToArrayAsync();
		
		IQueryable<Lote> query = context.Lotes;
		query = query.AsNoTracking().Where(lote => lote.EventoId == eventoId && lote.Id == id);
        return await query.FirstOrDefaultAsync();
		
		//Incluindo entidades filhas
		IQueryable<Evento> query = context.Eventos.Include(evento => evento.Lotes).Include(evento => evento.RedesSociais);
		if (includePalestrantes){
			query = query.Include(evento => evento.PalestrantesEventos).ThenInclude(pe => pe.Palestrante);
		}
		if (includePalestrantes){
			query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(p => p.Palestrante);
		}
		query = query.AsNoTracking().OrderBy(e => e.Id).Where(e => e.Id == eventoId);
		return await query.FirstOrDefaultAsync();        
				
				
		
=============================
# DATA ANNOTATIONS
	
		[Required(ErrorMessage = "O campo {0} é obrigatório")]
		[EmailAddress(ErrorMessage = "O campo {0} deve ser um endereço de e-mail válido")]
		
		String Validations
		
		MinLength(3, ErrorMessage = "{0} deve ter no mínimo 3 caracteres")
		MaxLength(50, ErrorMessage = "{0} pode ter no máximo 50 caracteres")
		StringLength(50, MinimumLength = 3, ErrorMessage = "{0} deve ter entre 3 e 50 caracteres")
		
		Apresenta o nome do campo deste modo em tela/mensagens de erro
		[Display(Name = "e-mail")]
		
		Valida se um número de telefone contém somente números ( e não letras)
		Aceita + - () e espaço
		[Phone(ErrorMessage = "O número de {0} é inválido")]
		
		Regular Expression
		Validar se é nome de arquivo de imagem
		[RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "O nome do arquivo não é uma imagem válida (gif, jpg, )")]
		
		Uma sequencia de Data Annotations podem ficar dentro do mesmo colchete
		
		
		EF-CORE
		
			Definir nome de tabela diferente do nome da classe
			[Table("NomeTabela")]

			Definir que um campo é chave (que não seja com nome Id)
			[Key]
			
			Definir ForeigKey
			[ForeigKey("NomeDaTabela")]
			
			Para campos notnull no banco de dados
			[Required]
			
			
			Para campos que não devam estar no banco de dados (para campos que calculem alguma formula)
			[NotMapped]
			
			Definir tamanho do campo no banco de dados
			[MaxLength(50)]
=============================		
# USERIDENTITY - Autenticação e Autorizações

	/* Com o userIdentity podemos incluir autenticação e trabalhar com roles, e autorização do usuário para a execução de funções*/
	Como podemos implementar o UserIdentity
	
		Criar uma classe que herda IdentityUser
			- Dependência: Microsoft.AspNetCore.Identity;
			
		Na classe Startup.cs do projeto, no método
			- ConfigureServices
				- Inserir a instrução:
				
				services.AddIdentity<UsuarioDaAplicacao, IdentityRole>().AddEntityFrameworkStores<IESContext>().AddDefaultTokenProviders();
				
				services.ConfigureApplicationCookie(options => {
					options.LoginPath = "/Infra/Acessar";
					options.AccessDeniedPath = "/Infra/AcessoNegado";
				});
				
			- Configure(IApplicationBuilder app, IWebHostingEnvironment ent)
				- Inserir a instrução:
				
				app.UseAuthentication();
				app.UseAuthorization(); //Se não não tiver
				
		Na sua classe de contexto.cs:
			- Substituir a herança
				- De: DbContext
				- Para: IdentityDbContext<ClasseDeUsuáriodaAplicação>
				
				
		Nos controllers:
			Utilizar: [Authorize]
				- Deve estar após o nome do namespace, sobre o nome da classe.
				
				[Authorize] Pode estar sobre qualquer Action do controller, indicando que a Action precisa de authorização
				[AllowAnonymous] é o contrário de [Authorize] - Permite a execução da Action sem autenticação
				
				
				



=================================================
# Curso UDEMY
FULL STACK - AspNet e Angular




.Net5
Visual Studio Code
node
Angular 


Aula 4
	Problemas no C# for Visual Studio Code
		instalar a versão v.1.23.17
		
		
Visual Studio Code 
	Compact Folders ( Mostra o nome de subpastas como Pasta\Subpasta ai inves de arvore)
	Ctrl + Shift + P
		Preferences:Open Settings UI
			Compact Folders
			Se estiver marcado, o nome dos folders filhos podem aparecer como:
			Front\ProEventos-App
			
			Se não estiver Marcado, vai aparecer abaixo:
			Front
				ProEventos-App
				
Para instalar o Entity Framework pela linha de comando:
	cd c:\...ir até a pasta do projeto
	dotnet tool install --global dotnet-ef --version 5.0.2
	
	
Passo a passo da criação do projeto
	1. Criar pasta no diretório desejado
	2. Abrir a pasta no VScode
		a. Criar subpasta: Back
			i. Criar subpasta: src
		a. Criar subpasta: Front
		
		
		1. abrir Terminal apontando para a pasta back/src
			a. Versão do dotnet
				dotnet --version
			
			b. Help para os comandos disponiveis / opções do sdk:
				dotnet -h
				
			c. informações do dotnet instalado
				dotnet --info
				
			d. Listar sdks instalados
				dotnet --list-sdks
				
			
			GLOBAL.JSON	
				O arquivo global.json é onde fica definido qual a versão do dotnet em que a aplicação será criada/utilizada.
				
				Criar o arquivo dentro da pasta raiz Back (antes de src)
					global.json
					
					O conteudo do global.json define o dotnet utilizado no projeto:
					
					{
						"sdk": {
							"version": "5.0.411"
						}
					}
				
				Para criar o global.json via prompt de comando:
					dotnet new global.json --sdk-version 5.0.411
					
	CRIANDO O PROJETO
		Comando 
			dotnet new
				- (lista os templates de projetos que o sdk pode gerar)
				
			dotnet new webapi
				- Cria um projeto do tipo webapi
				
			dotnet new webapi -n ProEvento.API
				- A opção -n possibilita de darmos um nome para o projeto
				O nome vem logo em seguida ao -n
				
				
			ATENÇÃO: O projeto será criado na pasta referenciada (cd c:\projeto...
			
		Executando
			(colocar na pasta dentro do projeto criado)
			(cd ProEventos.API)
			dotnet run
			
			
			Para executar https em dev, o seguinte comando cria o certficado para o ambiente localhost
				dotnet dev-certs https --trust
				
			Watch
				- O comando watch mantém o servidor sendo executado, e monitora por alterações.
				Caso reconheça alterações no projeto, ele automaticamente recompila e executa o projeto				
				
				dotnet watch run
				dotnet watch run -p ProEventos.API
	
Criar uma solução e projetos ClassLib para separar o projeto em camadas
	Aula 55

	Criando uma solução para o projeto 
		dotnet new -h
		dotnet new sln -n ProEventos 
		
		Uma solução serve para que sejam associados todos os projetos envolvidos na solução
		
		
	Criando projetos do tipo classlib (Library of Classes)
	
		dotnet new classlib -n ProEventos.Application
							   ProEventos.Domain
							   ProEventos.Persistence
							   
	Adicionar os projetos a solução
		dotnet sln ProEventos.sln add ProEventos.Application
		
	Adicionar a referencia de um projeto a outro
		dotnet add ProEventos.API/ProEventos.API.csproj reference ProEventos.Application
			- Adiciona a referencia do projeto referenciado ao csproj do projeto informado após o add 
			
Build 
	dotnet build
		- Executa a compilação dos projetos e soluções
		
		
			
		
GIT
	Ignorar arquivos 
	dotnet new gitignore
	
	O dotnet cria automaticamente com esse comando o arquivo gitignore já com os tipos de arquivos que ele julga necessário não serem enviados ao git
	
	
Entity Framework Core
	Entity Relational Mapping
	
	Instalando:
	dotnet tool install --global dotnet-ef
	
	dotnet-ef <enter>
		Apresenta os comandos disponiveis para a ferramenta
		
		
	Referênciando no projeto:
		As referencias do projeto ficam no arquivo *.csproj
		
		As referencias podem ser feitas pelo Nuget Gallery
			Digitar F1 ou Ctrl + Shift + P
			
			Nuget: Open Nuget Gallery
			
			Procurar e instalar:
				Microsoft.EntityFrameworkCore
				Microsoft.EntityFrameworkCore.Design
				Microsoft.EntityFrameworkCore.Tools
				Microsoft.EntityFrameworkCore.Sqlite
				
				
			
		DbContext - Contexto do Banco de dados
		
			Criar classe herdando DbContext do EntityFrameworkCore
			
			Para associar a classe que deverá ser persistida no banco de dados, deve ser definido um DbSet do tipo da classe onde está definida a estrutura
			
				public DbSet<Evento> Eventos { get; set;}
				
			//
			Informar ao Service (no StartUp) que há um DbContext para o banco de dados
				(A variavel configuration consegue acessar o appsettings do projeto
				
				No startup, adicionar ao service o dbcontext
				
					service.AddDbContext<DataContext>(
						context => context.UseSqlite(configuration.GetConnectionString("NomeDaConexão"));																		
					);
					
					
				Na classe do DbContext (DataContext) deve ser definido o construtor onde receberá as opções de configuração do banco de dados 
				Dica: ctor vira
				
				public DataContext(DbContextOptions<DataContext> options) : base(){}
				
			Como definir a string de conexão:
				No arquivo appsettings:
				Inserir:
					"ConnectionStrings":{
						"NomeDaConexão" : "String da Conexão"
					}
					
					Ex:
					Para o Sqlite:
					"ConnectionStrings":{
						"Default" : "Data source=ProEventos.db"
					}
			
			
		Migrations
			IMPORTANTE: Colocar a referência dentro da pasta do projeto ASP.NET 
			
			dotnet ef -h (helper)
			
			dotnet ef migrations -h (Helper)
			
			dotnet ef migrations add Initial -o Data/Migrations
			
			Depois de criar a migration:
				dotnet ef database update
				
				Esse comando criara o banco de dados/novas tabelas 
	
	
		Paginação
			No projeto de persistencia
				ProEventos.Persistence
				
				Criar pasta nova
					Models
					
				Criar novas classes
					PageParams 
					PageList
					
				Basicamente, ao buscar os dados com o EF, deve ser feito utilizando o programa de paginação, que vai definir qual a página deve ser retornada 
					Simplificando, o comando pula os registros que vc não quer.
					Por exemplo. 
						Se o pageSize é de 10.
						Na página 1, são retornados somente os primeiros 10 registros
						Na pagína 2, pula-se os primeiros 10 registros, e retornam-se mais 10. 
						E assim por diante, calculando o número de páginas com o total de registros.
						
				Codigo segue abaixo:			
				
				Classe PageParams:
					A classe pageParams é responsavel pela estrutura do request
					Ela traz os parâmetros através da query do request para definir qual o tamanho da página que deve ser retornada ao Requester:
					
					public class PageParams
					{
						public const int MaxPageSize = 50;
						public int PageNumber { get; set; } = 1;
						public int pageSize = 10;
						public int PageSize { 
							get { return pageSize; }
							set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
						}
						public string Term { get; set; } = String.Empty;
					}
				
					Comentario: Notar o get e set de PageSize
					Os valores setados são os valores padrão 
					Term será usado como termo de filtragem.
					
					
				Classe PageList
					A classe PageList calcula e retorna os registros em lista paginada
					
				public class PageList<T> : List<T>
				{
					public int CurrentPage { get; set; }
					public int TotalPages { get; set; }
					public int PageSize { get; set; }
					public int TotalCount { get; set; }

					public PageList() {}

					public PageList(List<T> items, int count, int pageNumber, int pageSize){
						TotalCount = count;
						PageSize = pageSize;
						CurrentPage = pageNumber;
						TotalPages = (int)Math.Ceiling(count/(double)pageSize);
						AddRange(items);
					}

					public static async Task<PageList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize){
						var count = await source.CountAsync();
						var items = await source.Skip((pageNumber-2) * pageSize).Take(pageSize).ToListAsync();

						return new PageList<T>(items, count, pageNumber, pageSize); 
					}
					
				}

				As persistencias não retornam mais seus tipos em lista ou array. Eles deverão retornar uma lista de páginas, ou PageList
					Exemplo: Task<Evento[]> se torna Task<PageList<Evento>>


					Código:
					
					public async Task<PageList<Evento>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false)
					{
						IQueryable<Evento> query = context.Eventos.Include(evento => evento.Lotes).Include(evento => evento.RedesSociais);
						if (includePalestrantes){
							query = query.Include(evento => evento.PalestrantesEventos).ThenInclude(pe => pe.Palestrante);
						}

						query = query.AsNoTracking().Where(e => e.UserId == userId && e.Tema.ToLower().Contains(pageParams.Term.ToLower()));

						return await PageList<Evento>.CreateAsync(query, pageParams.PageNumber, pageParams.PageSize);

						//return await query.AsNoTracking().Where(e => e.UserId == userId).OrderBy(e => e.Id).ToArrayAsync();
					}
				
					Notar que o filtro por tema foi uncluido no retorno de todos os eventos.
						Isso se deve pois podemos filtrar ou não, e retornará a mesma coisa. 
						
				
				O mapper (para mapeamento de dominio origem para DTO) funciona sem alteração, porém, a classe PageList precisa de um construtor vazio:
						public PageList() {}										
				
					E após o mapeamento, devem ser setados os parâmetros:
					
					var resultado = mapper.Map<PageList<EventoDTO>>(eventos);

					resultado.CurrentPage = eventos.CurrentPage;
					resultado.PageSize = eventos.PageSize;
					resultado.TotalPages = eventos.TotalPages;
					resultado.TotalCount = eventos.TotalCount;
					
					Sem fazer o instanciamento, pode causar erro: Error Mapping Types
					
					
				
				
				O service fica assim:
					 public async Task<PageList<EventoDTO>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false)
					{
						try
						{
							//Evento[] eventos = await eventosPersist.GetAllEventosAsync( userId, includePalestrantes);
							var eventos = await eventosPersist.GetAllEventosAsync( userId, pageParams, includePalestrantes);

							if (eventos == null) return null;

							var resultado = mapper.Map<PageList<EventoDTO>>(eventos);

							resultado.CurrentPage = eventos.CurrentPage;
							resultado.PageSize = eventos.PageSize;
							resultado.TotalPages = eventos.TotalPages;
							resultado.TotalCount = eventos.TotalCount;

							return resultado;
						}
						catch (Exception ex)
						{                
							throw new Exception(ex.Message);
						}
					}	
					
				Controller:
					Para recebermos os parâmetros, utilizaremos a classe PageParams, que receberá os dados via query:
					
					[HttpGet]
					[Authorize]
					public async Task<IActionResult> Get([FromQuery]PageParams pageParams)
					{
						try
						{
							 var eventos = await eventoService.GetAllEventosAsync(User.GetUserId(), pageParams, true);
							 //if (eventos == null) return NotFound("Nenhum Evento Encontrado");
							 if (eventos == null) return NoContent();                

							 return Ok(eventos);
						}
						catch (Exception ex)
						{                
							return this.StatusCode(StatusCodes.Status500InternalServerError, 
													$"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
						}
					}
					
				Para fazer a chamada e passar a query, é feito na propria URL, utilizando ? (interrogação) e preenchendo os valores após , e fica assim:
					https://localhost:5001/api/eventos/getuser?pagesize=10&pagenumber=1&term=qualquercoisa
					
			
			Response Header
				Como retornar um Header no Response da requisição.
				No caso, retornaremos informações de paginação, para o requester receber a informação de quantas paginas de registros existem. 
				
				Criamos uma classe de extensão, que estende o HttpResponse, passando os dados de paginação:
					Criado diretório Models dentro de ProEventos.API.
					Criar classe para definir a estrutura do retorno:
					
						public class PaginationHeader
						{
							public int CurrentPage { get; set; }
							public int ItemsPerPage { get; set; }
							public int TotalItems { get; set; }
							public int TotalPages { get; set; }
							
						}
					
					No diretório Extensions, definir classe de extensão de HttpResponse, que recebe as informações de paginação, e insere no Header do Response:
					
						public static class Pagination
						{
							public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int TotalPages)
							{
								var pagination = new PaginationHeader{
									CurrentPage = currentPage,
									ItemsPerPage = itemsPerPage, 
									TotalPages = TotalPages,
									TotalItems = totalItems
								};
								
								var options = new JsonSerializerOptions{
									PropertyNamingPolicy = JsonNamingPolicy.CamelCase
								};
								response.Headers.Add("Pagination", JsonSerializer.Serialize(pagination, options));
								response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

							}
						}
						
					No controller, fazer a chamada deste modo:
						Response.AddPagination(eventos.CurrentPage, eventos.PageSize, eventos.TotalCount, eventos.TotalPages);
				
				
			
			
			
ANGULAR
	Extensões / Extensions
	
		Angular Essentials
		Angular Files
		Angular Language Service
		Auto Close Tag (O VSCode já tem algo parecido - Não instalei)
		Auto Rename Tag
		Bracket Pair Colorizer (Deprecated - Built-in VsCode)
		Color Highlight
		GitLens
		Path Intellisense - Autocompleta caminhos de arquivos
		
		MySQL
		
	
Node version Manager 
	nvm -list
	
	
			
CORS
	Como configurar o CORS na API
	
	Na api do Asp.Net, 
		- No arquivo startup
		incluir as configurações para permissão do CORS policy
		
		em: 
		 public void ConfigureServices(IServiceCollection services)
			{
			...
			services.AddControllers();
			services.AddCors();
			...
			}
		
		na configuração do app, no método Configure:
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
			...
			app.UseAuthorization();
			//Libera os acessos bloqueados por CORS policy
            app.UseCors(cors => cors.AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowAnyOrigin());
			...
		}
		
dotnet restore
	???
	Restaura o projeto de acordo com os relacionamentos ?!

Como planejar projetos	
	Entidade Principal

	Sobre a divisão de camadas do projeto
	Segundo o instrutor, isso pode ser um pouco de Domain Driven Design
		- Verificar sobre.
		
	O projeto foi dividido em 
		ProEventos.API
			- webappi application
			- Aqui ficam os controllers, o startup do projeto (aqui é onde o framework é executado).
		ProEventos.Application
			- Class library
			- Ficarão os serviços e as interfaces dos serviços
		ProEventos.Domain
			- Class Libray
			- Dominios - Onde estão definidas as classes de entidades
		ProEventos.Persistence
			- Classes de Banco de dados
			- Contexto
			- Migrations
			- Contratos (Interfaces) de manipulação dos dados. 
			
	

Entidade Persistencia
	
	Interface com a criação de todos os métodos necessários para a implementação da persistencia
	
	
Como trabalhar com injeção de dependencia de classes - services
	Para respeitarmos os conceitos de Design Patterns de SOLID onde é dito que devemos depender de abstrações e não implementações, temos o método de Injeção de Dependencia.
	Pode utilizar a injeção de dependencia de nossas proprias classes, criando interfaces e implementações dessas interfaces.
	
	Para que isso seja possível, basta informar para o services do arquivos Startup.cs quais são nossas Interfaces e sua respectiva implementação.
	Sendo assim, ficamos assim:
	
	<coding>
	
	public void ConfigureServices(IServiceCollection services)
        {

			// ************ DEPENDENCY INJECTION PARA O DBCONTEXT ******************************
			// AQUI DEFINIMOS A CONEXÃO COM O BANCO DE DADOS 
            services.AddDbContext<ProEventosContext>(
                context => context.UseSqlite(Configuration.GetConnectionString("Default"))
            );           

            //Ignorar o loop - resolver a erro de cycle quando temos entidade filha com o Id do pai
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


			//***************************DEPENDENCY INJECTION***********************************
            //Serviços são inseridos aqui
            services.AddScoped<IGeralPersist, GeralPersist>();
            services.AddScoped<IEventosPersist, EventoPersist>();
            services.AddScoped<IEventoService, EventoService>();
			//***************************DEPENDENCY INJECTION***********************************


            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProEventos.API", Version = "v1" });
            });
        }
	
	</coding>
	
	
	Deste modo, para usar esses recursos, só precisamos injetar (atraves dos contrutores) essas classes nos objetos que necessitam utilizá-las.
	
	
Como criar uma interface de persistencia
	A interface define o contrato da persistencia dos dados de modo genérico
	
	<coding>
		public interface IGeralPersist
		{
			//GERAL
			void Add<T>(T entity) where T: class;
			void Update<T>(T entity) where T: class;
			void Delete<T>(T entity) where T: class;
			void DeleteRange<T>(T[] entity) where T: class;
			Task<bool> SaveChangesAsync();               
		}
	</coding>
	
Como criar a implementação de uma interface de CRUD genérico:

	<coding>
		public class GeralPersist : IGeralPersist
		{
			private readonly ProEventosContext context;
			
			public GeralPersist(ProEventosContext _context)
			{
				context = _context;            
			}

			public void Add<T>(T entity) where T : class
			{
				context.Add(entity);
			}

			public void Update<T>(T entity) where T : class
			{
				context.Update(entity);
			}

			public void Delete<T>(T entity) where T : class
			{
				context.Remove(entity);
			}

			public void DeleteRange<T>(T[] entities) where T : class
			{
				context.RemoveRange(entities);
			}

			public async Task<bool> SaveChangesAsync()
			{
				return (await context.SaveChangesAsync()) > 0;
			}                        
			
		}
	</coding>

	Deste modo, o C# já sabe que ele deve tratar genericamente a entidade recebida. 
	
	Uma interface de persistencia específica pode também herdar de IGeralPersist, assim não precisaria termos um serviço separado para o CRUD comum entre as interfaces. 
	
	IMPLEMENTANDO INTERFACES:
	
	
	
	
Problema de Ciclo em entidades que se auto-referenciam - Tag: NewtonsoftJson
	Erro: 
		System.Text.Json.JsonException: A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32. Consider using ReferenceHandler.Preserve on JsonSerializerOptions to support cycles.
		
		Isso se deve a entidades se referenciando entre si em modo de Loop.
			Uma entidade pode ser 1 para N por exemplo
			Dentro da entidade podemos ter uma referencia a um item de uma outra entidade, e na outra entidade, se referencia o código da entidade "Pai"
			
		Ao listar a entidade pai, ela pode listar a entidade filha.
			Em webapi, ao tentar enviar um objeto json da entidade pai, realizando ali o insert da entidade filha, o problema ocorre (o EF entra em loop). Ele adiciona o filho, mas ao tentar buscar, fica em loop por que o filho tem o Id do Pai. 
			
		Como resolver:
			Newtonsoft json
			
			Instalar via Nuget a instancia de: Microsoft.AspNetCore.MVC.NewtonsoftJson;
			
			Alteração no StartUp.cs
				Onde temos:
					services.AddControllers();
					
					Alterar para:
					
					services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
					
					
Delete Cascade
	Entidades com mais de 1 chave estrangeira
	
	Quando temos uma tabela (entidade) com somente 1 chave estrangeira, ao deletar, é feito delete cascade e o registro filho também é deletado
	
	Porém
		Quando temos tabelas com mais de 1 chave estrangeira, o comportamento padrão do EF é somente dissociar o filho do Id do pai, deixando o registro na base de dados (teoricamente, fica como lixo)
		
		Devemos dizer ao EF que o registro deve ser excluído.
		
		Como: 
		
		Dentro do contexto, dizer para o modelbuilder que uma entidade que tem muitos elementos, e que cada elemento que tem 1 só elemento pai, deve ser deletado em cascade:
			 
			//Um Evento, tem muitas redesSociais, e uma RedeSocial tem 1 evento. Ao deletar, execute Cascade
			modelBuilder.Entity<Evento>().HasMany(e => e.RedesSociais).WithOne(rs => rs.Evento).OnDelete(DeleteBehavior.Cascade);
					
			//Um palestrante tem muitas redes sociais, uma redesocial tem um palestrante. Ao deletar, execute Cascade. 
            modelBuilder.Entity<Palestrante>().HasMany(p => p.RedesSociais).WithOne(rs => rs.Palestrante).OnDelete(DeleteBehavior.Cascade);

DTOs
	Data Transfer Objects
	
	Pelo que entendi, os DTOs são usados para que o dominio não fique totalmente exposto. 
	ou seja, se temos um dominio de eventos mais seus campos, com lotes, redes sociais, palestrantes, mas vc quer enviar um evento reduzido (sem expor seus lotes, redes sociais, palestrantes.
	Vc cria uma classe DTO (Data transfer object), enviando somente os campos que vc julga serem necessários. 
	
	Um DTO é uma classe com os mesmos campos da classe de dominio, porém com menos informações (menos campos)
	
	
Mapeamento de Transferencia de Dados
	O mapeamento de transferencia de dados automatiza a criação de DTOs para que a exposição de dominios da aplicação seja limitada de acordo com a necessidade.
	Ao invês de referenciar as classes de DTO e inserir em seus objetos, os objetos obtidos via persistencia, num modo mais verboso
	o AutoMapper faz isso de modo mais automatico. 
	
	AutoMapper
	
	NuGet:
		AutoMapper.Extensions.Microsoft.DependencyInjection
		
		
	Adicionar ao services do StartUp.cs:
	
		public void ConfigureServices(...){
			...
			services.AddControllers(...);
			
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());		
			
		}
		
	Criar uma classe para definir os mapeamentos:
		Herdar Profile de AutoMapper
		(Criar uma pasta Helpers, e criar a nova classe dentro desta pasta)
		
			<code>
				using AutoMapper;
				using ProEventos.Domain;
				using ProEventos.API.DTOs;

				namespace ProEventos.API.Helpers
				{
					public class ProEventosProfile: Profile
					{
						public ProEventosProfile()
						{
							CreateMap<Evento, EventoDTO>();
						}
					}
				}			
			</code>
			
			Para que o Map reverso também seja possível, basta encadear com ReverseMap, assim é possível mapear o contrario:
			CreateMap<Evento, EventoDTO>().ReverseMap()
		
		DTO e Profile em Application ?!
			
			Para não haver uma quebra de paradigma, o código de mapeamento não pode estar dentro do controller. 
			Ele deve estar dentro do serviço, que está no projeto Application.
				Se precisar de alteração, não se altera o Controller
				
				
	Usando o Mapper
		using AutoMapper;
		
		========
		var resultado = mapper.Map<EventoDTO>(evento);
		===========
		Evento[] eventos = await eventosPersist.GetAllEventosByTemaAsync(tema, includePalestrantes);
		if (eventos == null) return Array.Empty<EventoDTO>();

		var resultado = mapper.Map<EventoDTO[]>(eventos);
		===========	
			
			
		Mapear um objeto origem para um outro objeto:
			mapper.Map(model, evento);      
			
			Os valores dos campos no objeto model serão mapeados para o objeto evento. 
			
			
		Para cada Data Transfer Object, precisa ser declarado no Mapper
			Quando tivermos 1 para muitos, precisa ser informado para o mapper

Debugando no VS Code
	No menu esquerdo, temos a opção Debug do VS Code
	Dentro deste menu, temos dois tipos de Launch
	
		1) .Net Core Launch (web)
			Deste modo o próprio VS Code tenta inicializar o projeto.
			Após o inicio, basta criar "break points" no código que ao executar, o debug é acionado, e podemos ir pulando de passos 
			
			(Pode não funcionar caso tenhamos mais de 1 projeto dentro da pasta do projeto - Front e Back)
			
		2) .Net Core Attach
			Neste caso, precisamos executar primeiramente nosso projeto (dotnet watch run)
			
			Apos isso, ao acionar o play do Attach, o VS Code solicita encontrar qual o projeto que deve ser assistido
				basta digitar o nome do projeto que o debug começa a assistir, e irá detectar os break points 
				
				
IDENTITY
	Instalado no projeto Domain
		ProEventos.Domain
		
		Microsoft.AspNetCore.Identity.EntityFrameworkCore
			A versão do Identity deve ser a mesma do EntityFrameworkCore instalado  
			Pode ocorrer erro (inconsistencia) se tivermos versões diferentes
			
		Instalando as dependências do JWT (Json Web Token)
			Microsoft.AspNetCore.Authentication.JwtBearer
			
		Dentro do Application.Domain Criar diretório			
			Identity
				Para separar as classes de Identity dos outros tipos de Domain
			
		Criado Enums
			Titulo
			Funcao
			
			Foi criada uma pasta separada para organizar os Enums dentro do Application.Domain
			
		Criado Classes
			User
				Criado campos em User 
				Herdar de IdentityUser<int> (instrutor sugeriu mudar para guid para a release)
				
				Properties de User
				
				<code>
					public class User : IdentityUser<int>
					{
						public string PrimeiroNome { get; set; }
						public string UltimoNome { get; set; }
						public Titulo Titulo { get; set; }
						public string Descricao {get ; set; }
						public Funcao Funcao { get; set; }
						public string ImagemURL { get; set; }

						public IEnumerable<UserRole> UserRoles { get; set; }
					}
							
				</code>
				
				
			Role
				Criar property
					UserRoles
				Herdar da IdentityRole<int> (instrutor sugeriu mudar para guid para a release)
				
				<code>
					public class Role : IdentityRole<int>
					{
						public IEnumerable<UserRole> UserRoles { get; set; }
					}
}
				</code>
				
				
				
			UserRole
				Criar Property
					User
					Role
				Herdar de IdentityUserRole<int> (instrutor sugeriu mudar para guid para a release)
				
				<code>
					public class UserRole : IdentityUserRole<int>
					{
						public User User { get; set; }
						public Role Role { get; set; }
					}
				
				</code>
			
			
		DbContext
			Alterações do Contexto
			
			Ao invés de trabalhar com DbContext, deveremos trabalhar com IdentityDbContext
			
				public class ProEventosContext: IdentityDbContext<User, Role, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
				{...
			
				IdentityUserRole<int> - Foi alterado para UserRole porque temos a configuração de UserRole nas nossas classes
		
				//base.OnModelCreating(modelBuilder); // Obrigatório
				
				protected override void OnModelCreating(ModelBuilder modelBuilder){
					base.OnModelCreating(modelBuilder); // Obrigatório
					...
	
		ATENÇÃO: Também é necessário instalar a referencia do Microsoft.AspNetCore.Identity.EntityFrameworkCore no projeto onde está o StartUp.cs (API)
		
		As classes de Identity não são incluidas no DbSet dentro da classe de Contexto.
			Ao invés disso, elas são definidas na herança da classe:
				public class ProEventosContext: IdentityDbContext<User, Role, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
				{...
				
			O EntityFrameworkCore consegue buscar a referencia das entidades 
			
			
		Classes de Conta de Usuário
			IAccountService
			AccountService
				(Aqui é implementada a lógica de tratativa de usuário - criar, atualizar, obter)
			
			UserDto
			UserUpdateDto
			UserLogin
			
			Controller
				AccountController
				
				
		
		UserManager
			Busca usuário por query
			.SingleOrDefaultAsync(user => user.UserName == userUpdateDto.UserName)
			
			Cria novo usuário
			.CreateAsync(user, password); // Retorna o usuário criado 
			
			Reset da senha
			.GeneratePasswordResetTokenAsync(user); //retorna novo Token para reset
			.ResetPasswordAsync(user, token, password); //faz o reset da senha
			
		SignInManager
			Checa a senha
			.CheckPasswordSignInAsync(user, password, false)
			SignInResult
			
		Token Service
			Dependencia:
				System.IdentityModels.Tokens.Jwt
											 Json Web Token
				Microsoft.IdentityModels.Tokens
				
					
		
			Constructor
				IConfiguration
				UserManager<User>
				IMapper
			
			Claims: Afirmações/Alegações
				System.Security.Claims
				
				Roles: Responsabilidades
				
				
		Referencias do Identity no Startup.cs
			
			Para retornar as descrições dos Enums no Response dos serviços
				Na classe StartUp.cs:
				
				public void ConfigureServices(IServiceCollection services)
				{...
				 services.AddControllers()
					.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))    
					
					
			Configurar o Identity no services:
				
				services.AddIdentityCore<User>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            })
            .AddRoles<Role>()
            .AddRoleManager<RoleManager<Role>>()
            .AddSignInManager<SignInManager<User>>()
            .AddRoleValidator<RoleValidator<Role>>()
            .AddEntityFrameworkStores<ProEventosContext>()
            .AddDefaultTokenProviders();     
			
		Configuração do JWT
		
				 services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters{
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                }); 
		
			
		
		
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
			...
			app.UseAuthentication();
            app.UseAuthorization();
		
		
		
		Swagger
			Ativar o token via swagger
			No StartUp.cs, onde temos a configuração do Swagger no services, incluir os comandos AddSecurityDefinition e AddSecurityRequirement:
			
			
			 services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ProEventos.API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                    Description = @"JWT Authorization header usando bearer.
                                    Entre com 'Bearer ' [espaço] então coloque seu token.
                                    Exemplo: 'Bearer 121231ksfskdjdfks'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }
		
Servindo Arquivos Estaticos
	Startup.cs
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
			...
			
			app.UseCors();
			
			app.UseStaticFiles(new StaticFileOptions(){
					FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
					RequestPath = new PathString("/Resources")
			});
	
	
			app.UseEndpoints(endpoints =>
            {...});
		}
		
Upload de Arquivos

	[NonAction]
	public async Task<string> SaveImage(IFormFile imageFile)
	{
		var imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');

		imageName = $"{imageName}{DateTime.UtcNow.ToString("yyyymmddssfff")}{Path.GetExtension(imageFile.FileName)}";

		var imagePath = Path.Combine(hostEnvironment.ContentRootPath, @"Resources/images", imageName);

		using( var fileStream = new FileStream(imagePath, FileMode.Create))
		{
			await imageFile.CopyToAsync(fileStream);
		}

		return imageName;
	}

	[NonAction]
	public void DeleteImage(string imageName){
		var imagePath = Path.Combine(hostEnvironment.ContentRootPath, @"Resources/images", imageName);
		if (System.IO.File.Exists(imagePath)) 
			System.IO.File.Delete(imagePath);
	}
	

			
	
Design Patterns
	DDD - Domain Driven Design
	
Bootsnipp
	bootsnipp.com
	
	Proporciona alguns templates e layouts usando bootstrap 
	
Bootswatch
	bootswatch.com
	Temas para bootstrap 
		Instalando o bootswatch, é possível alterar um pouco o layout do bootstrap, somente referenciando o css do boots
		Entrar na opção GitHub no footer da página para instruções de como instalar. 
	
Bootstrap - Dicas
	
	Responsividade
		Como esconder colunas de uma tabela responsivamente
		class="d-none d-md-table-cell"
		
		Esconder o texto de um botão
		<a class="d-flex btn btn-outline-primary" href="#">
			<i class="fa fa-plus-circle my-1"></i>
			<b class="ml-1 d-none d-sm-block">Novo</b>        
		</a>
		
	Deixar que divs sejam posicionadas a direita (e não abaixo)
		<div class="d-flex">
		<div class="flex-fill">
	
	
==========================
{
  "local": "São Paulo",
  "dataEvento": "2022-10-31T17:06:58.624",
  "tema": "Angular 11 e suas novidades",
  "qtdPessoas": 150,
  "imagemURL": "foto_2.png",
  "email": "angular@empresa.com.br",
  "telefone": "0319283019",
  "lotes": [
    {
      "nome": "1º Lote",
      "preco": 100.50,
      "dataInicio": "2022-10-31T17:52:53.664",
      "dataFim": "2022-10-31T17:52:53.664",
      "quantidade": 90
    },
    {
      "nome": "2º Lote",
      "preco": 150.96,
      "dataInicio": "2022-11-01T17:52:53.664",
      "dataFim": "2022-11-01T17:52:53.664",
      "quantidade": 120
    }
  ],
  "redesSociais": [
    {
      "nome": "Facebook",
      "url": "https://www.facebook.com/eventoAngularSP"
    }, 
    {
      "nome": "LinkedIn",
      "url": "https://www.linkedin.com/SPANgular"
    }
  ],
  "palestrantesEventos": []
}
					
			
			
			
			
	
			