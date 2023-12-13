using Microsoft.EntityFrameworkCore;

namespace LeamosColombiaProject.Models;

public partial class LeamosColombiaProjectContext : DbContext
{
    public LeamosColombiaProjectContext()
    {
    }

    public LeamosColombiaProjectContext(DbContextOptions<LeamosColombiaProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AbonoCartera> AbonoCarteras { get; set; }

    public virtual DbSet<Cartera> Carteras { get; set; }

    public virtual DbSet<Categorias> Categoria { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<DetalleCompra> DetalleCompras { get; set; }

    public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }

    public virtual DbSet<Editorial> Editorial { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<PermisoRol> PermisoRols { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<Referencia> Referencia { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<TipoDocumento> TipoDocumentos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Venta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=SQL5097.site4now.net;Initial Catalog=db_aa2b03_leamoscolombia12;User Id=db_aa2b03_leamoscolombia12_admin;Password=Leamos12345");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AbonoCartera>(entity =>
        {
            entity.HasKey(e => e.IdAbonoCartera).HasName("PK__AbonoCar__C3B643B0FE6314D9");

            entity.ToTable("AbonoCartera");

            entity.Property(e => e.IdAbonoCartera).HasColumnName("idAbonoCartera");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.IdCartera).HasColumnName("idCartera");

            entity.HasOne(d => d.IdCarteraNavigation).WithMany(p => p.AbonoCarteras)
                .HasForeignKey(d => d.IdCartera)
                .HasConstraintName("FK_AbonoCartera_Cartera");
        });

        modelBuilder.Entity<Cartera>(entity =>
        {
            entity.HasKey(e => e.IdCartera).HasName("PK__Cartera__CEF28AEAF7D72FC8");

            entity.ToTable("Cartera");

            entity.Property(e => e.IdCartera).HasColumnName("idCartera");
            entity.Property(e => e.FechaFinal)
                .HasColumnType("datetime")
                .HasColumnName("fechaFinal");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.fechaUltimoAbono)
    .HasColumnType("datetime")
    .HasColumnName("fechaUltimoAbono");
            entity.Property(e => e.IdVenta).HasColumnName("idVenta");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.Carteras)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK_Cartera_Venta");
        });

        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__8A3D240C4B1D4A68");


            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);



        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Cliente__885457EEE2A79888");

            entity.ToTable("Cliente");

            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Documento)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Facultad)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.IdTipoDocumento).HasColumnName("idTipoDocumento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Universidad)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTipoDocumentoNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdTipoDocumento)
                .HasConstraintName("FK_Cliente_TipoDocumento");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__Compra__48B99DB73FEC2016");

            entity.ToTable("Compra");

            entity.Property(e => e.IdCompra).HasColumnName("idCompra");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdProveedor)
                .HasConstraintName("FK_Compra_Proveedor");
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCompra).HasName("PK__DetalleC__62C252C15EFF07EC");

            entity.ToTable("DetalleCompra");

            entity.Property(e => e.IdDetalleCompra).HasColumnName("idDetalleCompra");
            entity.Property(e => e.IdCompra).HasColumnName("idCompra");
            entity.Property(e => e.IdProducto).HasColumnName("idProducto");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdCompra)
                .HasConstraintName("FK_DetalleCompra_Compra");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleCompras)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_DetalleCompra_Producto");
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.IdDetalleVenta).HasName("PK__DetalleV__BFE2843FF27D49AC");

            entity.Property(e => e.IdDetalleVenta).HasColumnName("idDetalleVenta");
            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.IdVenta).HasColumnName("idVenta");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_DetalleVenta_Producto");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK_DetalleVenta_Venta");
        });

        modelBuilder.Entity<Editorial>(entity =>
        {
            entity.HasKey(e => e.IdEditorial).HasName("PK__Editoria__9DF182DB7A03A6D3");

            entity.ToTable("Editorial");

            entity.Property(e => e.IdEditorial).HasColumnName("idEditorial");
            entity.Property(e => e.NombreEditorial)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PK__Permiso__06A584866F0BB324");

            entity.ToTable("Permiso");

            entity.Property(e => e.IdPermiso).HasColumnName("idPermiso");
            entity.Property(e => e.Modulo)
                .HasMaxLength(80)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PermisoRol>(entity =>
        {
            entity.HasKey(e => e.IdPermisoRol).HasName("PK__PermisoR__BBE208F9851EEFF4");

            entity.ToTable("PermisoRol");

            entity.Property(e => e.IdPermisoRol).HasColumnName("idPermisoRol");
            entity.Property(e => e.IdRol).HasColumnName("idRol");

            entity.HasOne(d => d.IdPermisoNavigation).WithMany(p => p.PermisoRols)
                .HasForeignKey(d => d.IdPermiso)
                .HasConstraintName("FK_PermisoRol_Permiso");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.PermisoRols)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK_PermisoRol_Rol");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__07F4A132274BAC27");

            entity.ToTable("Producto");

            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.Anio)
                            .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.Autor)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.Isbn)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.NPaginas).HasColumnName("nPaginas");
            entity.Property(e => e.Sinopsis)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Titulo)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(255)  // Ajusta la longitud según tus necesidades
                .IsUnicode(false);

            entity.HasOne(d => d.EditorialNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.Editorial)
                .HasConstraintName("FK_Producto_Editorial");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK_Producto_Categoria");

        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedo__A3FA8E6B0F4DC42B");

            entity.ToTable("Proveedor");

            entity.Property(e => e.IdProveedor).HasColumnName("idProveedor");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Encargado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Identificacion)
    .HasMaxLength(50)
    .IsUnicode(false);
            entity.Property(e => e.numeroIdentificacion)
            .HasMaxLength(50)
            .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Referencia>(entity =>
        {
            entity.HasKey(e => e.IdReferencia).HasName("PK__Referenc__22743578261528D7");

            entity.Property(e => e.IdReferencia).HasColumnName("idReferencia");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Documento)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Referencia)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_Referencia_Cliente");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__3C872F7674DBA5C2");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.HasKey(e => e.IdTipoDocumento).HasName("PK__TipoDocu__61FDF9F5302A2DD3");

            entity.ToTable("TipoDocumento");

            entity.Property(e => e.IdTipoDocumento).HasColumnName("idTipoDocumento");
            entity.Property(e => e.Documento)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__645723A6A44FA6A1");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Contraseña)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.RecoveryToken)
                .IsUnicode(false);
            entity.Property(e => e.RecoveryTokenExpirationDate)
                .HasColumnType("datetime");
            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK_Usuario_Rol");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__Venta__077D56149D7D76B5");

            entity.Property(e => e.IdVenta).HasColumnName("idVenta");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.IdCliente).HasColumnName("idCliente");
            entity.Property(e => e.TipoVenta)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK_Venta_Cliente");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}