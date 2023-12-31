USE [master]
GO
/****** Object:  Database [LeamosColombiaProject]    Script Date: 18/10/2023 1:10:53 a. m. ******/
CREATE DATABASE [LeamosColombiaProject]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'LeamosColombiaProject', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\LeamosColombiaProject.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'LeamosColombiaProject_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\LeamosColombiaProject_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [LeamosColombiaProject] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [LeamosColombiaProject].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [LeamosColombiaProject] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET ARITHABORT OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [LeamosColombiaProject] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [LeamosColombiaProject] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET  ENABLE_BROKER 
GO
ALTER DATABASE [LeamosColombiaProject] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [LeamosColombiaProject] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET RECOVERY FULL 
GO
ALTER DATABASE [LeamosColombiaProject] SET  MULTI_USER 
GO
ALTER DATABASE [LeamosColombiaProject] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [LeamosColombiaProject] SET DB_CHAINING OFF 
GO
ALTER DATABASE [LeamosColombiaProject] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [LeamosColombiaProject] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [LeamosColombiaProject] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [LeamosColombiaProject] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'LeamosColombiaProject', N'ON'
GO
ALTER DATABASE [LeamosColombiaProject] SET QUERY_STORE = ON
GO
ALTER DATABASE [LeamosColombiaProject] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [LeamosColombiaProject]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AbonoCartera]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AbonoCartera](
	[idAbonoCartera] [int] IDENTITY(1,1) NOT NULL,
	[Cuotas] [int] NULL,
	[Fecha] [datetime] NULL,
	[Abono] [int] NULL,
	[idCartera] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idAbonoCartera] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cartera]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cartera](
	[idCartera] [int] IDENTITY(1,1) NOT NULL,
	[fechaInicio] [datetime] NULL,
	[fechaUltimoAbono] [datetime] NULL,
	[fechaFinal] [datetime] NULL,
	[Saldo] [int] NULL,
	[Monto] [int] NULL,
	[Estado] [bit] NULL,
	[idVenta] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idCartera] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categoria]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categoria](
	[idCategoria] [int] IDENTITY(1,1) NOT NULL,
	[Categoria] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[idCategoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cliente]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente](
	[idCliente] [int] IDENTITY(1,1) NOT NULL,
	[Documento] [varchar](50) NULL,
	[Nombre] [varchar](80) NULL,
	[Correo] [varchar](50) NULL,
	[Direccion] [varchar](50) NULL,
	[Telefono] [varchar](50) NULL,
	[Ciudad] [varchar](200) NULL,
	[Universidad] [varchar](200) NULL,
	[Facultad] [varchar](80) NULL,
	[Semestre] [int] NULL,
	[Estado] [bit] NULL,
	[idTipoDocumento] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Compra]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Compra](
	[idCompra] [int] IDENTITY(1,1) NOT NULL,
	[Total] [int] NULL,
	[Fecha] [datetime] NULL,
	[Estado] [bit] NULL,
	[idProveedor] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idCompra] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleCompra]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleCompra](
	[idDetalleCompra] [int] IDENTITY(1,1) NOT NULL,
	[Cantidad] [int] NULL,
	[Precio] [int] NULL,
	[idCompra] [int] NULL,
	[idProducto] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalleCompra] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleVenta]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleVenta](
	[idDetalleVenta] [int] IDENTITY(1,1) NOT NULL,
	[Cantidad] [int] NULL,
	[Precio] [int] NULL,
	[idVenta] [int] NULL,
	[idProducto] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalleVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Editorial]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Editorial](
	[idEditorial] [int] IDENTITY(1,1) NOT NULL,
	[NombreEditorial] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[idEditorial] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permiso]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permiso](
	[idPermiso] [int] IDENTITY(1,1) NOT NULL,
	[Modulo] [varchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[idPermiso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PermisoRol]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermisoRol](
	[idPermisoRol] [int] IDENTITY(1,1) NOT NULL,
	[IdPermiso] [int] NULL,
	[idRol] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idPermisoRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Producto]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Producto](
	[idProducto] [int] IDENTITY(1,1) NOT NULL,
	[Titulo] [varchar](80) NULL,
	[Autor] [varchar](100) NULL,
	[Anio] [varchar](10) NULL,
	[nPaginas] [int] NULL,
	[Sinopsis] [varchar](1000) NULL,
	[ImagenUrl] [varchar](max) NULL,
	[Precio] [int] NULL,
	[Estado] [bit] NULL,
	[idCategoria] [int] NULL,
	[ISBN] [varchar](20) NULL,
	[Cantidad] [int] NULL,
	[Editorial] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Proveedor]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Proveedor](
	[idProveedor] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
	[numeroIdentificacion] [varchar](50) NULL,
	[Identificacion] [varchar](50) NULL,
	[Encargado] [varchar](50) NULL,
	[Correo] [varchar](100) NULL,
	[Direccion] [varchar](100) NULL,
	[Telefono] [varchar](100) NULL,
	[Estado] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[idProveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Referencia]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Referencia](
	[idReferencia] [int] IDENTITY(1,1) NOT NULL,
	[Documento] [varchar](50) NULL,
	[Nombre] [varchar](80) NULL,
	[Correo] [varchar](50) NULL,
	[Direccion] [varchar](50) NULL,
	[Telefono] [varchar](50) NULL,
	[Ciudad] [varchar](200) NULL,
	[Estado] [bit] NULL,
	[idCliente] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idReferencia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rol]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rol](
	[idRol] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
	[Estado] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[idRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoDocumento]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoDocumento](
	[idTipoDocumento] [int] IDENTITY(1,1) NOT NULL,
	[Documento] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[idTipoDocumento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuario]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuario](
	[idUsuario] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](80) NULL,
	[Correo] [varchar](100) NULL,
	[Contraseña] [varchar](100) NULL,
	[Estado] [bit] NULL,
	[idRol] [int] NULL,
	[RecoveryToken] [nvarchar](max) NULL,
	[RecoveryTokenExpirationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[idUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Venta]    Script Date: 18/10/2023 1:10:53 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Venta](
	[idVenta] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [datetime] NULL,
	[TipoVenta] [varchar](10) NULL,
	[Total] [int] NULL,
	[Estado] [bit] NULL,
	[idCliente] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Categoria] ON 

INSERT [dbo].[Categoria] ([idCategoria], [Categoria]) VALUES (1, N'Anatomía y Fisiología')
INSERT [dbo].[Categoria] ([idCategoria], [Categoria]) VALUES (2, N'Cirugía')
INSERT [dbo].[Categoria] ([idCategoria], [Categoria]) VALUES (3, N'Medicina Interna')
INSERT [dbo].[Categoria] ([idCategoria], [Categoria]) VALUES (4, N'Pediatría')
INSERT [dbo].[Categoria] ([idCategoria], [Categoria]) VALUES (5, N'Farmacología')
SET IDENTITY_INSERT [dbo].[Categoria] OFF
GO
SET IDENTITY_INSERT [dbo].[Editorial] ON 

INSERT [dbo].[Editorial] ([idEditorial], [NombreEditorial]) VALUES (1, N'Editorial Médica Colombiana')
INSERT [dbo].[Editorial] ([idEditorial], [NombreEditorial]) VALUES (2, N'Libros Médicos SA')
INSERT [dbo].[Editorial] ([idEditorial], [NombreEditorial]) VALUES (3, N'Editorial Salud y Ciencia')
INSERT [dbo].[Editorial] ([idEditorial], [NombreEditorial]) VALUES (4, N'Ediciones Médicas Colombianas')
INSERT [dbo].[Editorial] ([idEditorial], [NombreEditorial]) VALUES (5, N'Librería Médica Nacional')
SET IDENTITY_INSERT [dbo].[Editorial] OFF
GO
SET IDENTITY_INSERT [dbo].[Permiso] ON 

INSERT [dbo].[Permiso] ([idPermiso], [Modulo]) VALUES (1, N'Proveedores')
INSERT [dbo].[Permiso] ([idPermiso], [Modulo]) VALUES (2, N'Compras')
INSERT [dbo].[Permiso] ([idPermiso], [Modulo]) VALUES (3, N'Productos')
INSERT [dbo].[Permiso] ([idPermiso], [Modulo]) VALUES (4, N'Carteras')
INSERT [dbo].[Permiso] ([idPermiso], [Modulo]) VALUES (5, N'Clientes')
INSERT [dbo].[Permiso] ([idPermiso], [Modulo]) VALUES (6, N'Ventas')
INSERT [dbo].[Permiso] ([idPermiso], [Modulo]) VALUES (7, N'Roles')
INSERT [dbo].[Permiso] ([idPermiso], [Modulo]) VALUES (8, N'Usuarios')
SET IDENTITY_INSERT [dbo].[Permiso] OFF
GO
SET IDENTITY_INSERT [dbo].[PermisoRol] ON 

INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (1, 1, 1)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (2, 2, 1)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (3, 3, 1)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (4, 4, 1)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (5, 5, 1)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (6, 6, 1)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (7, 7, 1)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (8, 8, 1)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (9, 3, 2)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (10, 4, 2)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (11, 5, 2)
INSERT [dbo].[PermisoRol] ([idPermisoRol], [IdPermiso], [idRol]) VALUES (12, 6, 2)
SET IDENTITY_INSERT [dbo].[PermisoRol] OFF
GO
SET IDENTITY_INSERT [dbo].[Rol] ON 

INSERT [dbo].[Rol] ([idRol], [Nombre], [Estado]) VALUES (1, N'Administrador', 1)
INSERT [dbo].[Rol] ([idRol], [Nombre], [Estado]) VALUES (2, N'Empleado', 1)
SET IDENTITY_INSERT [dbo].[Rol] OFF
GO
SET IDENTITY_INSERT [dbo].[TipoDocumento] ON 

INSERT [dbo].[TipoDocumento] ([idTipoDocumento], [Documento]) VALUES (1, N'TI')
INSERT [dbo].[TipoDocumento] ([idTipoDocumento], [Documento]) VALUES (2, N'CC')
INSERT [dbo].[TipoDocumento] ([idTipoDocumento], [Documento]) VALUES (3, N'CE')
SET IDENTITY_INSERT [dbo].[TipoDocumento] OFF
GO
SET IDENTITY_INSERT [dbo].[Usuario] ON 

INSERT [dbo].[Usuario] ([idUsuario], [Nombre], [Correo], [Contraseña], [Estado], [idRol], [RecoveryToken], [RecoveryTokenExpirationDate]) VALUES (1, N'Carlos Velásquez', N'leamoscolombia1999@gmail.com', N'aeec7e57b75916c8e1fe330039567795cc7068ef1e96a3e1fc0d8a2a3be613f6', 1, 1, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Usuario] OFF
GO
ALTER TABLE [dbo].[AbonoCartera]  WITH CHECK ADD  CONSTRAINT [FK_AbonoCartera_Cartera] FOREIGN KEY([idCartera])
REFERENCES [dbo].[Cartera] ([idCartera])
GO
ALTER TABLE [dbo].[AbonoCartera] CHECK CONSTRAINT [FK_AbonoCartera_Cartera]
GO
ALTER TABLE [dbo].[Cartera]  WITH CHECK ADD  CONSTRAINT [FK_Cartera_Venta] FOREIGN KEY([idVenta])
REFERENCES [dbo].[Venta] ([idVenta])
GO
ALTER TABLE [dbo].[Cartera] CHECK CONSTRAINT [FK_Cartera_Venta]
GO
ALTER TABLE [dbo].[Cliente]  WITH CHECK ADD  CONSTRAINT [FK_Cliente_TipoDocumento] FOREIGN KEY([idTipoDocumento])
REFERENCES [dbo].[TipoDocumento] ([idTipoDocumento])
GO
ALTER TABLE [dbo].[Cliente] CHECK CONSTRAINT [FK_Cliente_TipoDocumento]
GO
ALTER TABLE [dbo].[Compra]  WITH CHECK ADD  CONSTRAINT [FK_Compra_Proveedor] FOREIGN KEY([idProveedor])
REFERENCES [dbo].[Proveedor] ([idProveedor])
GO
ALTER TABLE [dbo].[Compra] CHECK CONSTRAINT [FK_Compra_Proveedor]
GO
ALTER TABLE [dbo].[DetalleCompra]  WITH CHECK ADD  CONSTRAINT [FK_DetalleCompra_Compra] FOREIGN KEY([idCompra])
REFERENCES [dbo].[Compra] ([idCompra])
GO
ALTER TABLE [dbo].[DetalleCompra] CHECK CONSTRAINT [FK_DetalleCompra_Compra]
GO
ALTER TABLE [dbo].[DetalleCompra]  WITH CHECK ADD  CONSTRAINT [FK_DetalleCompra_Producto] FOREIGN KEY([idProducto])
REFERENCES [dbo].[Producto] ([idProducto])
GO
ALTER TABLE [dbo].[DetalleCompra] CHECK CONSTRAINT [FK_DetalleCompra_Producto]
GO
ALTER TABLE [dbo].[DetalleVenta]  WITH CHECK ADD  CONSTRAINT [FK_DetalleVenta_Producto] FOREIGN KEY([idProducto])
REFERENCES [dbo].[Producto] ([idProducto])
GO
ALTER TABLE [dbo].[DetalleVenta] CHECK CONSTRAINT [FK_DetalleVenta_Producto]
GO
ALTER TABLE [dbo].[DetalleVenta]  WITH CHECK ADD  CONSTRAINT [FK_DetalleVenta_Venta] FOREIGN KEY([idVenta])
REFERENCES [dbo].[Venta] ([idVenta])
GO
ALTER TABLE [dbo].[DetalleVenta] CHECK CONSTRAINT [FK_DetalleVenta_Venta]
GO
ALTER TABLE [dbo].[PermisoRol]  WITH CHECK ADD  CONSTRAINT [FK_PermisoRol_Permiso] FOREIGN KEY([IdPermiso])
REFERENCES [dbo].[Permiso] ([idPermiso])
GO
ALTER TABLE [dbo].[PermisoRol] CHECK CONSTRAINT [FK_PermisoRol_Permiso]
GO
ALTER TABLE [dbo].[PermisoRol]  WITH CHECK ADD  CONSTRAINT [FK_PermisoRol_Rol] FOREIGN KEY([idRol])
REFERENCES [dbo].[Rol] ([idRol])
GO
ALTER TABLE [dbo].[PermisoRol] CHECK CONSTRAINT [FK_PermisoRol_Rol]
GO
ALTER TABLE [dbo].[Producto]  WITH CHECK ADD  CONSTRAINT [FK_Producto_Categoria] FOREIGN KEY([idCategoria])
REFERENCES [dbo].[Categoria] ([idCategoria])
GO
ALTER TABLE [dbo].[Producto] CHECK CONSTRAINT [FK_Producto_Categoria]
GO
ALTER TABLE [dbo].[Producto]  WITH CHECK ADD  CONSTRAINT [FK_Producto_Editorial] FOREIGN KEY([Editorial])
REFERENCES [dbo].[Editorial] ([idEditorial])
GO
ALTER TABLE [dbo].[Producto] CHECK CONSTRAINT [FK_Producto_Editorial]
GO
ALTER TABLE [dbo].[Referencia]  WITH CHECK ADD  CONSTRAINT [FK_Referencia_Cliente] FOREIGN KEY([idCliente])
REFERENCES [dbo].[Cliente] ([idCliente])
GO
ALTER TABLE [dbo].[Referencia] CHECK CONSTRAINT [FK_Referencia_Cliente]
GO
ALTER TABLE [dbo].[Usuario]  WITH CHECK ADD  CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY([idRol])
REFERENCES [dbo].[Rol] ([idRol])
GO
ALTER TABLE [dbo].[Usuario] CHECK CONSTRAINT [FK_Usuario_Rol]
GO
ALTER TABLE [dbo].[Venta]  WITH CHECK ADD  CONSTRAINT [FK_Venta_Cliente] FOREIGN KEY([idCliente])
REFERENCES [dbo].[Cliente] ([idCliente])
GO
ALTER TABLE [dbo].[Venta] CHECK CONSTRAINT [FK_Venta_Cliente]
GO
USE [master]
GO
ALTER DATABASE [LeamosColombiaProject] SET  READ_WRITE 
GO
