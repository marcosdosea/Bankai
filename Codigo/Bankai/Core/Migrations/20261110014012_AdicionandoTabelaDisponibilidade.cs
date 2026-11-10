using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoTabelaDisponibilidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "especialidade",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pessoa",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    cpfCnpj = table.Column<string>(type: "char(14)", fixedLength: true, maxLength: 14, nullable: false),
                    nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    telefone = table.Column<string>(type: "char(11)", fixedLength: true, maxLength: 11, nullable: true),
                    ehProfissional = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cep = table.Column<string>(type: "char(8)", fixedLength: true, maxLength: 8, nullable: false),
                    rua = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    bairro = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    cidade = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    estado = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    numeroCasa = table.Column<string>(type: "char(10)", fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "servico",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    descricao = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "agendamento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    descricaoServico = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    tipoCobranca = table.Column<string>(type: "enum('F','O')", nullable: false),
                    statusAgendamento = table.Column<string>(type: "enum('Execucao','Concluido','Cancelado')", nullable: false),
                    nota = table.Column<string>(type: "enum('MR','R','B','MB','E')", nullable: false),
                    comentario = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    idProfissional = table.Column<int>(type: "int", nullable: false),
                    idCliente = table.Column<int>(type: "int", nullable: false),
                    horarioInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    horarioFim = table.Column<TimeSpan>(type: "time", nullable: false),
                    diaSemana = table.Column<string>(type: "enum('D','SF','TF','QF','QIF','SXF','S')", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Agendamento_Pessoa1",
                        column: x => x.idProfissional,
                        principalTable: "pessoa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_Agendamento_Pessoa2",
                        column: x => x.idCliente,
                        principalTable: "pessoa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "disponibilidade",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    statusDisponibilidade = table.Column<string>(type: "enum('D','ND')", nullable: false),
                    diaSemana = table.Column<string>(type: "enum('D','SF','TF','QF','QIF','SXF','S')", nullable: false),
                    horarioInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    horarioFim = table.Column<TimeSpan>(type: "time", nullable: false),
                    idPessoa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Disponibilidade_Pessoa1",
                        column: x => x.idPessoa,
                        principalTable: "pessoa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "profissionalespecialidade",
                columns: table => new
                {
                    idPessoa = table.Column<int>(type: "int", nullable: false),
                    idEspecialidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "fk_ProfissionalEspecialidade_Especialidade1",
                        column: x => x.idEspecialidade,
                        principalTable: "especialidade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ProfissionalEspecialidade_Pessoa1",
                        column: x => x.idPessoa,
                        principalTable: "pessoa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "profissionalservico",
                columns: table => new
                {
                    idPessoa = table.Column<int>(type: "int", nullable: false),
                    idServico = table.Column<int>(type: "int", nullable: false),
                    preco = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idPessoa, x.idServico });
                    table.ForeignKey(
                        name: "fk_ProfissionalServiço_Pessoa1",
                        column: x => x.idPessoa,
                        principalTable: "pessoa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ProfissionalServiço_Servico1",
                        column: x => x.idServico,
                        principalTable: "servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "agendamentoservico",
                columns: table => new
                {
                    idAgendamento = table.Column<int>(type: "int", nullable: false),
                    idServico = table.Column<int>(type: "int", nullable: false),
                    valorProposto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    statusServico = table.Column<string>(type: "enum('E','C','CA')", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idAgendamento, x.idServico });
                    table.ForeignKey(
                        name: "fk_AgendamentoServiço_Agendamento1",
                        column: x => x.idAgendamento,
                        principalTable: "agendamento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_AgendamentoServiço_Serviço1",
                        column: x => x.idServico,
                        principalTable: "servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "chamadogarantia",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    dataChamado = table.Column<DateTime>(type: "date", nullable: false),
                    statusGarantia = table.Column<string>(type: "enum('S','A','R','C','E')", nullable: false),
                    idAgendamento = table.Column<int>(type: "int", nullable: false),
                    descricao = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_ChamadoGarantia_Agendamento1",
                        column: x => x.idAgendamento,
                        principalTable: "agendamento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pagamento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    formaPagamento = table.Column<string>(type: "enum('P','CC')", nullable: false),
                    statusPagamento = table.Column<string>(type: "enum('AP','P','R','A','C')", nullable: false),
                    valorTransacao = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    idAgendamento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Pagamento_Agendamento1",
                        column: x => x.idAgendamento,
                        principalTable: "agendamento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "fk_Agendamento_Pessoa1_idx",
                table: "agendamento",
                column: "idProfissional");

            migrationBuilder.CreateIndex(
                name: "fk_Agendamento_Pessoa2_idx",
                table: "agendamento",
                column: "idCliente");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "agendamento",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_AgendamentoServiço_Agendamento1_idx",
                table: "agendamentoservico",
                column: "idAgendamento");

            migrationBuilder.CreateIndex(
                name: "fk_AgendamentoServiço_Serviço1_idx",
                table: "agendamentoservico",
                column: "idServico");

            migrationBuilder.CreateIndex(
                name: "fk_ChamadoGarantia_Agendamento1_idx",
                table: "chamadogarantia",
                column: "idAgendamento");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE1",
                table: "chamadogarantia",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Disponibilidade_Pessoa1_idx",
                table: "disponibilidade",
                column: "idPessoa");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE2",
                table: "disponibilidade",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Pagamento_Agendamento1_idx",
                table: "pagamento",
                column: "idAgendamento");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE3",
                table: "pagamento",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "cpf_UNIQUE",
                table: "pessoa",
                column: "cpfCnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "email_UNIQUE",
                table: "pessoa",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE4",
                table: "pessoa",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "telefone_UNIQUE",
                table: "pessoa",
                column: "telefone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_ProfissionalEspecialidade_Especialidade1_idx",
                table: "profissionalespecialidade",
                column: "idEspecialidade");

            migrationBuilder.CreateIndex(
                name: "fk_ProfissionalEspecialidade_Pessoa1_idx",
                table: "profissionalespecialidade",
                column: "idPessoa");

            migrationBuilder.CreateIndex(
                name: "fk_ProfissionalServiço_Pessoa1_idx",
                table: "profissionalservico",
                column: "idPessoa");

            migrationBuilder.CreateIndex(
                name: "fk_ProfissionalServiço_Servico1_idx",
                table: "profissionalservico",
                column: "idServico");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE5",
                table: "servico",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "nome_UNIQUE",
                table: "servico",
                column: "nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agendamentoservico");

            migrationBuilder.DropTable(
                name: "chamadogarantia");

            migrationBuilder.DropTable(
                name: "disponibilidade");

            migrationBuilder.DropTable(
                name: "pagamento");

            migrationBuilder.DropTable(
                name: "profissionalespecialidade");

            migrationBuilder.DropTable(
                name: "profissionalservico");

            migrationBuilder.DropTable(
                name: "agendamento");

            migrationBuilder.DropTable(
                name: "especialidade");

            migrationBuilder.DropTable(
                name: "servico");

            migrationBuilder.DropTable(
                name: "pessoa");
        }
    }
}
