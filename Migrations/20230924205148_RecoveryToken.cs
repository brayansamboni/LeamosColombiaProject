using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeamosColombiaProject.Migrations
{
    /// <inheritdoc />
    public partial class RecoveryToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    idCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__8A3D240C4B1D4A68", x => x.idCategoria);
                });

            migrationBuilder.CreateTable(
                name: "Editorial",
                columns: table => new
                {
                    idEditorial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEditorial = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Editoria__9DF182DB7A03A6D3", x => x.idEditorial);
                });

            migrationBuilder.CreateTable(
                name: "Permiso",
                columns: table => new
                {
                    idPermiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Modulo = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Permiso__06A584866F0BB324", x => x.idPermiso);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    idProveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Encargado = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Proveedo__A3FA8E6B0F4DC42B", x => x.idProveedor);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    idRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rol__3C872F7674DBA5C2", x => x.idRol);
                });

            migrationBuilder.CreateTable(
                name: "TipoDocumento",
                columns: table => new
                {
                    idTipoDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Documento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoDocu__61FDF9F5302A2DD3", x => x.idTipoDocumento);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    idProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: true),
                    Autor = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Anio = table.Column<DateTime>(type: "date", nullable: true),
                    nPaginas = table.Column<int>(type: "int", nullable: true),
                    Sinopsis = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    Imagen = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Precio = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true),
                    idCategoria = table.Column<int>(type: "int", nullable: true),
                    ISBN = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    Editorial = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Producto__07F4A132274BAC27", x => x.idProducto);
                    table.ForeignKey(
                        name: "FK_Producto_Categoria",
                        column: x => x.idCategoria,
                        principalTable: "Categoria",
                        principalColumn: "idCategoria");
                    table.ForeignKey(
                        name: "FK_Producto_Editorial",
                        column: x => x.Editorial,
                        principalTable: "Editorial",
                        principalColumn: "idEditorial");
                });

            migrationBuilder.CreateTable(
                name: "Compra",
                columns: table => new
                {
                    idCompra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Total = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true),
                    idProveedor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Compra__48B99DB73FEC2016", x => x.idCompra);
                    table.ForeignKey(
                        name: "FK_Compra_Proveedor",
                        column: x => x.idProveedor,
                        principalTable: "Proveedor",
                        principalColumn: "idProveedor");
                });

            migrationBuilder.CreateTable(
                name: "PermisoRol",
                columns: table => new
                {
                    idPermisoRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPermiso = table.Column<int>(type: "int", nullable: true),
                    idRol = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PermisoR__BBE208F9851EEFF4", x => x.idPermisoRol);
                    table.ForeignKey(
                        name: "FK_PermisoRol_Permiso",
                        column: x => x.IdPermiso,
                        principalTable: "Permiso",
                        principalColumn: "idPermiso");
                    table.ForeignKey(
                        name: "FK_PermisoRol_Rol",
                        column: x => x.idRol,
                        principalTable: "Rol",
                        principalColumn: "idRol");
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    idUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: true),
                    Correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Contraseña = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true),
                    idRol = table.Column<int>(type: "int", nullable: true),
                    RecoveryToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecoveryTokenExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__645723A6A44FA6A1", x => x.idUsuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol",
                        column: x => x.idRol,
                        principalTable: "Rol",
                        principalColumn: "idRol");
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    idCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Documento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Nombre = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: true),
                    Correo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Direccion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Telefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Ciudad = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Universidad = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Facultad = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: true),
                    Semestre = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true),
                    idTipoDocumento = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cliente__885457EEE2A79888", x => x.idCliente);
                    table.ForeignKey(
                        name: "FK_Cliente_TipoDocumento",
                        column: x => x.idTipoDocumento,
                        principalTable: "TipoDocumento",
                        principalColumn: "idTipoDocumento");
                });

            migrationBuilder.CreateTable(
                name: "DetalleCompra",
                columns: table => new
                {
                    idDetalleCompra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    Precio = table.Column<int>(type: "int", nullable: true),
                    idCompra = table.Column<int>(type: "int", nullable: true),
                    idProducto = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetalleC__62C252C15EFF07EC", x => x.idDetalleCompra);
                    table.ForeignKey(
                        name: "FK_DetalleCompra_Compra",
                        column: x => x.idCompra,
                        principalTable: "Compra",
                        principalColumn: "idCompra");
                    table.ForeignKey(
                        name: "FK_DetalleCompra_Producto",
                        column: x => x.idProducto,
                        principalTable: "Producto",
                        principalColumn: "idProducto");
                });

            migrationBuilder.CreateTable(
                name: "Referencia",
                columns: table => new
                {
                    idReferencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Documento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Nombre = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: true),
                    Correo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Direccion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Telefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Ciudad = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true),
                    idCliente = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Referenc__22743578261528D7", x => x.idReferencia);
                    table.ForeignKey(
                        name: "FK_Referencia_Cliente",
                        column: x => x.idCliente,
                        principalTable: "Cliente",
                        principalColumn: "idCliente");
                });

            migrationBuilder.CreateTable(
                name: "Venta",
                columns: table => new
                {
                    idVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime", nullable: true),
                    TipoVenta = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Total = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true),
                    idCliente = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Venta__077D56149D7D76B5", x => x.idVenta);
                    table.ForeignKey(
                        name: "FK_Venta_Cliente",
                        column: x => x.idCliente,
                        principalTable: "Cliente",
                        principalColumn: "idCliente");
                });

            migrationBuilder.CreateTable(
                name: "Cartera",
                columns: table => new
                {
                    idCartera = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fechaInicio = table.Column<DateTime>(type: "datetime", nullable: true),
                    fechaFinal = table.Column<DateTime>(type: "datetime", nullable: true),
                    Saldo = table.Column<int>(type: "int", nullable: true),
                    Monto = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true),
                    idVenta = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cartera__CEF28AEAF7D72FC8", x => x.idCartera);
                    table.ForeignKey(
                        name: "FK_Cartera_Venta",
                        column: x => x.idVenta,
                        principalTable: "Venta",
                        principalColumn: "idVenta");
                });

            migrationBuilder.CreateTable(
                name: "DetalleVenta",
                columns: table => new
                {
                    idDetalleVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cantidad = table.Column<int>(type: "int", nullable: true),
                    Precio = table.Column<int>(type: "int", nullable: true),
                    idVenta = table.Column<int>(type: "int", nullable: true),
                    idProducto = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DetalleV__BFE2843FF27D49AC", x => x.idDetalleVenta);
                    table.ForeignKey(
                        name: "FK_DetalleVenta_Producto",
                        column: x => x.idProducto,
                        principalTable: "Producto",
                        principalColumn: "idProducto");
                    table.ForeignKey(
                        name: "FK_DetalleVenta_Venta",
                        column: x => x.idVenta,
                        principalTable: "Venta",
                        principalColumn: "idVenta");
                });

            migrationBuilder.CreateTable(
                name: "AbonoCartera",
                columns: table => new
                {
                    idAbonoCartera = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cuotas = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime", nullable: true),
                    Abono = table.Column<int>(type: "int", nullable: true),
                    idCartera = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AbonoCar__C3B643B0FE6314D9", x => x.idAbonoCartera);
                    table.ForeignKey(
                        name: "FK_AbonoCartera_Cartera",
                        column: x => x.idCartera,
                        principalTable: "Cartera",
                        principalColumn: "idCartera");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbonoCartera_idCartera",
                table: "AbonoCartera",
                column: "idCartera");

            migrationBuilder.CreateIndex(
                name: "IX_Cartera_idVenta",
                table: "Cartera",
                column: "idVenta");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_idTipoDocumento",
                table: "Cliente",
                column: "idTipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Compra_idProveedor",
                table: "Compra",
                column: "idProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCompra_idCompra",
                table: "DetalleCompra",
                column: "idCompra");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCompra_idProducto",
                table: "DetalleCompra",
                column: "idProducto");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVenta_idProducto",
                table: "DetalleVenta",
                column: "idProducto");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVenta_idVenta",
                table: "DetalleVenta",
                column: "idVenta");

            migrationBuilder.CreateIndex(
                name: "IX_PermisoRol_IdPermiso",
                table: "PermisoRol",
                column: "IdPermiso");

            migrationBuilder.CreateIndex(
                name: "IX_PermisoRol_idRol",
                table: "PermisoRol",
                column: "idRol");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Editorial",
                table: "Producto",
                column: "Editorial");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_idCategoria",
                table: "Producto",
                column: "idCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Referencia_idCliente",
                table: "Referencia",
                column: "idCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_idRol",
                table: "Usuario",
                column: "idRol");

            migrationBuilder.CreateIndex(
                name: "IX_Venta_idCliente",
                table: "Venta",
                column: "idCliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbonoCartera");

            migrationBuilder.DropTable(
                name: "DetalleCompra");

            migrationBuilder.DropTable(
                name: "DetalleVenta");

            migrationBuilder.DropTable(
                name: "PermisoRol");

            migrationBuilder.DropTable(
                name: "Referencia");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Cartera");

            migrationBuilder.DropTable(
                name: "Compra");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Permiso");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Venta");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Editorial");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "TipoDocumento");
        }
    }
}
