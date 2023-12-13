import 'package:flutter/material.dart';
import 'package:leamoscolombiamobile/main.dart';
import 'package:leamoscolombiamobile/carteras.dart';
import 'package:leamoscolombiamobile/productos.dart';
import 'UsuarioModel.dart';

void main() => runApp(const PrincipalApp());

class PrincipalApp extends StatelessWidget {
  const PrincipalApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Leamos Colombia Movil',
      theme: ThemeData(
        primaryColor: const Color(0xFF0F0E0E), // Color de la barra superior
      ),
      debugShowCheckedModeBanner: false, // Set this to false
      home: PrincipalList(),
    );
  }
}

class PrincipalList extends StatefulWidget {
  const PrincipalList({super.key});

  @override
  _PrincipalListState createState() => _PrincipalListState();
}

class _PrincipalListState extends State<PrincipalList> {
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

@override
  Widget build(BuildContext context) {
    // Obtén el email del usuario (puedes pasarlo como parámetro al constructor o utilizar un servicio de autenticación)

    return Scaffold(
      appBar: AppBar(
        title: const Text('Principal'),
        centerTitle: true,
        backgroundColor: const Color(0xFF0F0E0E),
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
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Image.asset(
              'assets/logo.png',
              height: 300,
            ),
            SizedBox(height: 20),
            Text(
              'Bienvenido al aplicativo móvil de la librería Leamos Colombia',
              style: TextStyle(
                fontSize: 25,
                fontWeight: FontWeight.bold,
                fontFamily: 'Montserrat',
              ),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      ),
    );
  }
}
