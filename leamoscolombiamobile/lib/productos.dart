import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:intl/intl.dart';
import 'package:leamoscolombiamobile/main.dart';
import 'package:leamoscolombiamobile/carteras.dart';
import 'package:leamoscolombiamobile/principal.dart';



void main() => runApp(const ProductosApp());

class ProductosApp extends StatelessWidget {
  const ProductosApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Lista de Libros',
      theme: ThemeData(
        primaryColor: const Color(0xFF0F0E0E), // Color de la barra superior
      ),
      home: const ProductsList(),
      debugShowCheckedModeBanner: false, // Set this to false
    );
  }
}

class ProductsList extends StatefulWidget {
  const ProductsList({super.key});

  @override
  _ProductsListState createState() => _ProductsListState();
}

class _ProductsListState extends State<ProductsList> {
  List<Map<String, dynamic>> _productos = [];
  List<Map<String, dynamic>> _productosFiltrados = [];
  bool _isLoading = false;
  final TextEditingController _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _refreshProductos();
  }

  Future<void> _refreshProductos() async {
    setState(() {
      _isLoading = true;
    });

    final productos = await obtenerProductos();

    setState(() {
      _productos = productos;
      _productosFiltrados = _productos;
      _isLoading = false;
    });
  }

  Future<List<Map<String, dynamic>>> obtenerProductos() async {
    final response = await http.get(
      Uri.parse('http://leamoscolombia12-001-site1.atempurl.com/api/Api/Productos'),
    );

    if (response.statusCode == 200) {
      final List<dynamic> data = json.decode(response.body);
      return List<Map<String, dynamic>>.from(data);
    } else {
      print('Error: ${response.reasonPhrase}');
      return [];
    }
  }

 Widget _decodificarImagen(String imageUrl, {double height = 400.0}) {
  if (imageUrl != null && imageUrl.isNotEmpty) {
    return Image.network(
      imageUrl,
      height: height,
      width: double.infinity,
      fit: BoxFit.contain,
      loadingBuilder: (BuildContext context, Widget child, ImageChunkEvent? loadingProgress) {
        if (loadingProgress == null) {
          return child;
        } else {
          return Center(
            child: CircularProgressIndicator(
              value: loadingProgress.expectedTotalBytes != null
                  ? loadingProgress.cumulativeBytesLoaded / (loadingProgress.expectedTotalBytes ?? 1)
                  : null,
            ),
          );
        }
      },
      errorBuilder: (BuildContext context, Object error, StackTrace? stackTrace) {
        return Center(
          child: Icon(Icons.error),
        );
      },
    );
  } else {
    return Container(
      height: height,
      padding: const EdgeInsets.all(10),
      child: const Center(
        child: Text(
          'No hay imagen para este libro',
          textAlign: TextAlign.center,
        ),
      ),
    );
  }
}


  void _mostrarImagen(String imageUrl) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return Dialog(
          child: Container(
            width: MediaQuery.of(context).size.width * 0.8,
            margin: const EdgeInsets.all(16.0),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start, // Align content to the left
              children: [
                const SizedBox(height: 10),
                const Text(
                  'Portada del libro',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 10),
                _decodificarImagen(imageUrl, height: 300.0),
                const SizedBox(height: 10),
                Align(
                  alignment: Alignment.centerRight, // Align the button to the right
                  child: TextButton(
                    onPressed: () {
                      Navigator.pop(context);
                    },
                    style: TextButton.styleFrom(
                      foregroundColor: Colors.red, // Set the text color to red
                    ),
                    child: const Text('Cerrar'),
                  ),
                ),
                const SizedBox(height: 10),
              ],
            ),
          ),
        );
      },
    );
  }

  void _mostrarDetalles(
    String titulo,
    String precio,
    String anio,
    String nPaginas,
    String sinopsis,
    String autor,
    String isbn,
    bool estado,
    String categoria,
    String editorial,
  ) {
    final formatoMoneda = NumberFormat.currency(locale: 'es', symbol: '\$');

    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: Text(titulo),
          content: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                _buildDetalleRow('Autor', autor),
                _buildDetalleRow('ISBN', isbn),
                _buildDetalleRow('Año', anio),
                _buildDetalleRow('Número de Páginas', nPaginas),
                _buildDetalleRow(
                  'Precio',
                  formatoMoneda.format(double.parse(precio)),
                ),
                _buildDetalleRow('Categoría', categoria),
                _buildDetalleRow('Editorial', editorial),
                _buildDetalleRow(
                  'Sinopsis',
                  sinopsis ?? 'No hay sinopsis',
                ),
                _buildDetalleRow(
                  'Estado',
                  estado ? 'Disponible' : 'No disponible',
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.pop(context);
              },
              style: TextButton.styleFrom(
                foregroundColor: Colors.red, // Set the text color to red
              ),
              child: const Text('Cerrar'),
            ),
          ],
        );
      },
    );
  }

  Widget _buildDetalleRow(String label, String value) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '$label:',
          style: const TextStyle(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 3),
        Text(value),
        const SizedBox(height: 10),
        const Divider(), // Línea separadora
        const SizedBox(height: 10),
      ],
    );
  }

  void _cerrarSesion() {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Cerrar sesión'),
          content: const Text('¿Está seguro de que desea cerrar sesión?'),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.pop(context); // Cerrar el modal de confirmación
              },
              style: TextButton.styleFrom(
                foregroundColor: Theme.of(context).primaryColor, // Color del botón
              ),
              child: const Text('Cancelar'),
            ),
            TextButton(
              onPressed: () {
                Navigator.pop(context); // Cerrar el modal de confirmación
                Navigator.pushReplacement(
                  context,
                  MaterialPageRoute(builder: (context) => const MyApp()),
                );
              },
              style: TextButton.styleFrom(
                foregroundColor: Colors.red, // Color del botón "Aceptar"
              ),
              child: const Text('Aceptar'),
            ),
          ],
        );
      },
    );
  }

  void _filtrarProductos(String query) {
    setState(() {
      _productosFiltrados = _productos
          .where((producto) =>
              producto['titulo'].toLowerCase().contains(query.toLowerCase()))
          .toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Productos'),
        centerTitle: true,
        backgroundColor: const Color(0xFF0F0E0E), // Color de la barra superior
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            onPressed: _cerrarSesion,
          ),
        ],
      ),
    drawer: Drawer(
        child: ListView(
          children: [
UserAccountsDrawerHeader(
  accountName: const Text('Señor Usuario'),
  accountEmail: Text('Gracias por visitar el aplicativo'),
  currentAccountPicture: CircleAvatar(
    backgroundColor: Colors.white,
    child: Icon(
      Icons.book,
      color: Colors.black, // Establece el color del ícono a negro
    ),
  ),
  decoration: BoxDecoration(
    color: Colors.black, // Establece el color de fondo a negro
  ),
),


           ListTile(
  title: const Text('Cartera'),
  leading: Icon(Icons.shopping_bag), // Añade el ícono de bolsa de compras
  onTap: () {
    Navigator.pop(context);
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => CarterasApp()),
    );
  },
),
ListTile(
  title: const Text('Productos'),
  leading: Icon(Icons.book), // Añade el ícono de libro
  onTap: () {
    Navigator.pop(context);
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => ProductosApp()),
    );
  },
),
ListTile(
  title: const Text('Bienvenido'),
  leading: Icon(Icons.lightbulb_outline), // Añade el ícono de bombilla
  onTap: () {
    Navigator.pop(context);
    Navigator.push(
      context,
      MaterialPageRoute(builder: (context) => PrincipalApp()),
    );
  },
),

          ],
        ),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : Padding(
              padding: const EdgeInsets.all(8.0),
              child: Column(
                children: [
                  TextFormField(
                    controller: _searchController,
                    decoration: InputDecoration(
                      labelText: 'Buscar por título',
                      suffixIcon: IconButton(
                        icon: const Icon(Icons.clear),
                        onPressed: () {
                          _searchController.clear();
                          _filtrarProductos('');
                        },
                      ),
                    ),
                    onChanged: _filtrarProductos,
                  ),
                  const SizedBox(height: 10),
                   Expanded(
                    child: SingleChildScrollView(
                      child: Wrap(
                        spacing: 10.0,
                        runSpacing: 10.0,
                        alignment: WrapAlignment.spaceEvenly,
                        children: _productosFiltrados.map((producto) {
                          return Container(
                            width: MediaQuery.of(context).size.width / 2 - 15,
                            height: 350.0,
                            padding: const EdgeInsets.all(10),
                            decoration: BoxDecoration(
                              border: Border.all(color: Colors.grey),
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Container(
                                  alignment: Alignment.center,
                                  child: _decodificarImagen(
                                    producto['imagen'] ?? '',
                                    height: 150,
                                  ),
                                ),
                                const SizedBox(height: 10),
                                Text(
                                  '${producto['titulo']}',
                                  maxLines: 2,
                                  overflow: TextOverflow.ellipsis,
                                  style: const TextStyle(
                                    fontSize: 18,
                                    fontWeight: FontWeight.bold,
                                    color: Colors.black,
                                  ),
                                ),
                                const SizedBox(height: 3),
                                Text(
                                  NumberFormat.currency(locale: 'es', symbol: '\$').format(producto['precio'].toDouble()),
                                  style: const TextStyle(fontSize: 16),
                                ),
                                const SizedBox(height: 3),
                                Text(
                                  '${producto['categoria'] ?? 'Sin categoría'}',
                                  style: const TextStyle(fontSize: 16),
                                ),
                                const SizedBox(height: 10),
                                Row(
                                  mainAxisAlignment:
                                      MainAxisAlignment.spaceEvenly,
                                  children: [
                                    IconButton(
                                      onPressed: () {
                                        if (producto['imagen'] != null) {
                                          _mostrarImagen(producto['imagen']);
                                        } else {
                                          ScaffoldMessenger.of(context)
                                              .showSnackBar(
                                            const SnackBar(
                                              content: Text(
                                                  'No hay imagen para este producto.'),
                                              duration: Duration(seconds: 2),
                                            ),
                                          );
                                        }
                                      },
                                      icon: const Icon(Icons.image),
                                      tooltip: 'Ver Imagen',
                                    ),
                                    IconButton(
                                      onPressed: () {
                                        _mostrarDetalles(
                                          producto['titulo'],
                                          producto['precio'].toString(),
                                          producto['anio'],
                                          producto['nPaginas'].toString(),
                                          producto['sinopsis'] ??
                                              'Sin sinopsis',
                                          producto['autor'],
                                          producto['isbn'].toString(),
                                          producto['estado'] ?? false,
                                          producto['categoria'] ??
                                              'Sin categoría',
                                          producto['editorial'] ??
                                              'Sin editorial',
                                        );
                                      },
                                      icon: const Icon(Icons.info),
                                      tooltip: 'Ver Detalles',
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          );
                        }).toList(),
                      ),
                    ),
                  ),
                ],
              ),
            ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _refreshProductos(),
        child: const Icon(Icons.refresh),
        backgroundColor: const Color(0xFF0F0E0E),
      ),
    );
  }
}