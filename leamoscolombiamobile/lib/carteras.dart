import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:intl/intl.dart';
import 'package:leamoscolombiamobile/main.dart';
import 'package:leamoscolombiamobile/productos.dart';
import 'package:leamoscolombiamobile/principal.dart';

void main() => runApp(const CarterasApp());

class CarterasApp extends StatelessWidget {
  const CarterasApp({Key? key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Cartera',
      theme: ThemeData(
        primaryColor: const Color(0xFF0F0E0E),
      ),
      home: const CarterasList(),
      debugShowCheckedModeBanner: false,
    );
  }
}

class CarterasList extends StatefulWidget {
  const CarterasList({Key? key});

  @override
  _CarterasListState createState() => _CarterasListState();
}

class _CarterasListState extends State<CarterasList> {
  List<Map<String, dynamic>> _carteras = [];
  List<Map<String, dynamic>> _carterasFiltradas = [];
  bool _isLoading = false;
  final TextEditingController _searchController = TextEditingController();
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>(); // Añadido

  int? _selectedCarteraId;

  @override
  void initState() {
    super.initState();
    _refreshCarteras();
  }

  Future<void> _refreshCarteras() async {
    setState(() {
      _isLoading = true;
    });

    final carteras = await obtenerCarteras();

    setState(() {
      _carteras = carteras;
      _carterasFiltradas = _carteras;
      _isLoading = false;
    });
  }

  Future<List<Map<String, dynamic>>> obtenerCarteras() async {
    final response = await http.get(
      Uri.parse('http://leamoscolombia12-001-site1.atempurl.com/api/Api/Carteras'),
    );

    if (response.statusCode == 200) {
      final List<dynamic> data = json.decode(response.body);

      List<Map<String, dynamic>> carterasConAbonos = data.map((cartera) {
        List<Map<String, dynamic>> abonos = (cartera['abonoCarteras'] as List?)
                ?.map((abono) => {
                      'idabonocartera': abono['idabonocartera'],
                      'cuotas': abono['cuotas'],
                      'fecha': abono['fecha'],
                      'abono': abono['abono'],
                    })
                .toList() ??
            [];

        // Calcular la fecha del último abono
DateTime? fechaUltimoAbono;
if (abonos.isNotEmpty) {
  fechaUltimoAbono = abonos
      .where((abono) => abono['fecha'] != null)  // Filtrar aquellos con fecha no nula
      .map((abono) => DateTime.parse(abono['fecha']))
      .reduce((value, element) => value.isAfter(element) ? value : element);
}


        return {
          'idcartera': cartera['idcartera'],
          'fechainicio': cartera['fechainicio'],
          'fechafinal': cartera['fechafinal'],
          'saldo': cartera['saldo'],
          'monto': cartera['monto'],
          'estado': cartera['estado'],
          'fechaultimoabono': fechaUltimoAbono,
          'AbonoCarteras': abonos,
 'clienteNombre': cartera['idVentaNavigation'] != null
            ? cartera['idVentaNavigation']['idventa'] // Aquí se obtiene el nombre del cliente
            : null,
        'AbonoCarteras': abonos,
              'progreso': _calcularProgreso(cartera),

        };
      }).toList();

      return carterasConAbonos;
    } else {
      print('Error: ${response.reasonPhrase}');
      return [];
    }
  }
  String _calcularProgreso(Map<String, dynamic> cartera) {
    int saldo = cartera['saldo'] ?? 0;
    bool estado = cartera['estado'] ?? false;

    if (saldo == 0) {
      return 'Completado';
    } else if (saldo > 0 && estado) {
      return 'Pendiente';
    } else {
      return 'Cancelado';
    }
  }

  Color _getColorByProgreso(String progreso) {
    switch (progreso) {
      case 'Completado':
        return Colors.green;
      case 'Pendiente':
        return Colors.amber;
      case 'Cancelado':
        return Colors.red;
      default:
        return Colors.black;
    }
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
                Navigator.pop(context);
              },
              style: TextButton.styleFrom(
                foregroundColor: Theme.of(context).primaryColor,
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

void _filtrarCarteras(String query) {
  setState(() {
    _carterasFiltradas = _carteras
        .where((cartera) =>
            cartera['idcartera'].toString().contains(query.toLowerCase()) ||
            (cartera['clienteNombre']?.toLowerCase() ?? '').contains(query.toLowerCase()))
        .toList();
  });
}


Future<void> _agregarAbono(int idCartera, int abono) async {
  try {
    // Obtener la fecha y hora actual en formato ISO 8601
    String fechaActual = DateTime.now().toIso8601String();

    final response = await http.post(
      Uri.parse('http://leamoscolombia12-001-site1.atempurl.com/api/Api/AgregarAbono'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'IdCartera': idCartera,
        'Abono': abono,
        'Fecha': fechaActual,
      }),
    );

    if (response.statusCode == 200) {
      _refreshCarteras();
    } else {
      print('Error al agregar abono. Status code: ${response.statusCode}');
      print('Respuesta del servidor: ${response.body}');
    }
  } catch (e) {
    print('Error en la solicitud HTTP: $e');
  }
}


void _mostrarFormularioAgregarAbono() {
  final _abonoController = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  showDialog(
    context: context,
    builder: (BuildContext context) {
      return Center(
        child: Container(
          width: MediaQuery.of(context).size.width * 0.8,
          child: AlertDialog(
            title: const Text('Agregar Abono'),
            content: Form(
              key: _formKey,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min, // Ajusta el tamaño del contenido
                children: [
                  DropdownButtonFormField<int>(
                    value: _selectedCarteraId,
                    decoration: const InputDecoration(labelText: 'Número de Cartera'),
                    items: _carterasFiltradas
                        .where((cartera) => (cartera['estado'] ?? false) && (cartera['saldo'] ?? 0) > 0)
                        .map((cartera) {
                      return DropdownMenuItem<int>(
                        value: cartera['idcartera'],
                        child: Text('${cartera['idcartera']} - ${cartera['clienteNombre'] ?? 'Sin cliente'}'),
                      );
                    }).toList(),
                    onChanged: (value) {
                      setState(() {
                        _selectedCarteraId = value;
                      });
                    },
                  ),
                  const SizedBox(height: 10),
                  TextFormField(
                    controller: _abonoController,
                    keyboardType: TextInputType.number,
                    decoration: const InputDecoration(labelText: 'Abono'),
                    validator: (value) {
                      if (value == null || value.isEmpty) {
                        return 'Ingrese un valor';
                      }

                      final abono = int.tryParse(value);

                      if (abono == null || abono <= 0) {
                        return 'Ingrese un valor válido';
                      }

                      if (abono > _getSelectedCarteraSaldo()) {
                        return 'Abono mayor que saldo';
                      }

                          // Validación de longitud
    if (value.length < 5 || value.length > 10) {
      return 'Debe ser de 5 a 10 caracteres';
    }

                      return null;
                    },
                  ),
                ],
              ),
            ),
            actions: [
              TextButton(
                onPressed: () {
                  Navigator.pop(context);
                  setState(() {
                    _selectedCarteraId = null;
                  });
                },
                style: TextButton.styleFrom(
                  foregroundColor: Theme.of(context).primaryColor,
                ),
                child: const Text('Cancelar'),
              ),
              ElevatedButton(
                onPressed: () async {
                  if (_formKey.currentState != null && _formKey.currentState!.validate()) {
                    final abono = int.parse(_abonoController.text);
                    await _agregarAbono(_selectedCarteraId!, abono);
                    Navigator.pop(context);
                    setState(() {
                      _selectedCarteraId = null;
                    });
                  }
                },
                child: const Text('Agregar'),
                style: ElevatedButton.styleFrom(
                  primary: Colors.blue,
                ),
              ),
            ],
          ),
        ),
      );
    },
  );
}


int _getSelectedCarteraSaldo() {
  if (_selectedCarteraId != null) {
    final selectedCartera = _carteras.firstWhere((cartera) => cartera['idcartera'] == _selectedCarteraId);
    return selectedCartera['saldo'] ?? 0;
  }
  return 0;
}


  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Cartera'),
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
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : Padding(
              padding: const EdgeInsets.all(8.0),
              child: Column(
                children: [
                  TextFormField(
                    controller: _searchController,
                    decoration: InputDecoration(
                      labelText: 'Buscar por id o nombre',
                      suffixIcon: IconButton(
                        icon: const Icon(Icons.clear),
                        onPressed: () {
                          _searchController.clear();
                          _filtrarCarteras('');
                        },
                      ),
                    ),
                    onChanged: _filtrarCarteras,
                  ),
                  ElevatedButton(
      onPressed: () {
        _mostrarFormularioAgregarAbono();
      },
      child: const Text('Agregar Abono'),
    ),
                  const SizedBox(height: 10),
                  Expanded(
                    child: SingleChildScrollView(
                      child: Wrap(
                        spacing: 10.0,
                        runSpacing: 10.0,
                        alignment: WrapAlignment.spaceEvenly,
                    children: _carterasFiltradas.map((cartera) {
  List<Widget> abonosWidgets = (cartera['AbonoCarteras'] as List)
      .map((abono) {
        return Container(
          padding: const EdgeInsets.all(10),
          decoration: BoxDecoration(
            color: Colors.grey[200],
            borderRadius: BorderRadius.circular(10),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const SizedBox(height: 5),
              Text(
                'Cuotas: ${abono['cuotas']}',
                style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 5),
              Text(
                'Fecha: ${abono['fecha']}',
                style: const TextStyle(fontSize: 14),
              ),
              const SizedBox(height: 5),
              Text(
                'Abono: ${NumberFormat.currency(locale: 'es', symbol: '\$').format(abono['abono'])}',
                style: const TextStyle(fontSize: 14),
              ),
            ],
          ),
        );
      })
      .toList();

  return Container(
    width: double.infinity, // Ancho máximo
    padding: const EdgeInsets.all(10),
    margin: const EdgeInsets.only(bottom: 10),
    decoration: BoxDecoration(
      border: Border.all(color: Colors.grey),
      borderRadius: BorderRadius.circular(10),
    ),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const SizedBox(height: 10),
        Text.rich(
          TextSpan(
            children: [
              const TextSpan(
                text: 'Cliente: ',
                style: TextStyle(
                  fontSize: 19,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextSpan(
                text: '${cartera['clienteNombre'] ?? 'Sin cliente'}',
                style: const TextStyle(fontSize: 16),
              ),
            ],
          ),
        ),
        const SizedBox(height: 3),
        Text.rich(
          TextSpan(
            children: [
              const TextSpan(
                text: 'Número de Cartera: ',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextSpan(
                text: '${cartera['idcartera']}',
                style: const TextStyle(fontSize: 16),
              ),
            ],
          ),
        ),
        const SizedBox(height: 3),
        Text.rich(
          TextSpan(
            children: [
              const TextSpan(
                text: 'Fecha de Inicio: ',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextSpan(
                text: '${cartera['fechainicio'] != null ? DateFormat('dd/MM/yyyy').format(DateTime.parse(cartera['fechainicio'])) : 'Sin fecha de inicio'}',
                style: const TextStyle(fontSize: 16),
              ),
            ],
          ),
        ),
        const SizedBox(height: 5),
        Text.rich(
          TextSpan(
            children: [
              const TextSpan(
                text: 'Fecha del último abono: ',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextSpan(
                text: '${cartera['fechaultimoabono'] != null ? DateFormat('dd/MM/yyyy').format(cartera['fechaultimoabono']) : 'Sin fecha'}',
                style: const TextStyle(fontSize: 16),
              ),
            ],
          ),
        ),
        const SizedBox(height: 5),
        Text.rich(
          TextSpan(
            children: [
              const TextSpan(
                text: 'Fecha final: ',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextSpan(
                text: '${cartera['fechafinal'] != null ? DateFormat('dd/MM/yyyy').format(DateTime.parse(cartera['fechafinal'])) : 'Sin fecha final'}',
                style: const TextStyle(fontSize: 16),
              ),
            ],
          ),
        ),
        const SizedBox(height: 5),
        Text.rich(
          TextSpan(
            children: [
              const TextSpan(
                text: 'Saldo: ',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextSpan(
                text: '${NumberFormat.currency(locale: 'es', symbol: '\$').format(cartera['saldo'].toDouble())}',
                style: const TextStyle(fontSize: 16),
              ),
            ],
          ),
        ),
        const SizedBox(height: 5),
        Text.rich(
          TextSpan(
            children: [
              const TextSpan(
                text: 'Monto: ',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextSpan(
                text: '${NumberFormat.currency(locale: 'es', symbol: '\$').format(cartera['monto'].toDouble())}',
                style: const TextStyle(fontSize: 16),
              ),
            ],
          ),
        ),
        const SizedBox(height: 5),
        Text.rich(
          TextSpan(
            children: [
              const TextSpan(
                text: 'Estado: ',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextSpan(
                text: '${cartera['estado'] ?? false ? 'Disponible' : 'No disponible'}',
                style: const TextStyle(fontSize: 16),
              ),
            ],
          ),
        ),
        const SizedBox(height: 5),
        Text.rich(
          TextSpan(
            children: [
              const TextSpan(
                text: 'Progreso: ',
                style: TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextSpan(
                text: cartera['progreso'] ?? 'N/A',
                style: TextStyle(
                  fontSize: 16,
                  color: _getColorByProgreso(cartera['progreso'] ?? 'N/A'),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 5),
        SizedBox(
          width: double.infinity,
          child: ElevatedButton.icon(
            onPressed: cartera['estado'] ?? false
                ? () {
                    showDialog(
                      context: context,
                      builder: (BuildContext context) {
                        return AlertDialog(
                          title: const Text('Información de Abonos'),
                          content: SingleChildScrollView(
                            child: Column(
                              children: abonosWidgets,
                            ),
                          ),
                          actions: [
                            TextButton(
                              onPressed: () {
                                Navigator.pop(context);
                              },
                              style: TextButton.styleFrom(
                                foregroundColor: Colors.red,
                              ),
                              child: const Text('Cerrar'),
                            ),
                          ],
                        );
                      },
                    );
                  }
                : null,
            icon: const Icon(Icons.info),
            label: const Text('Ver Abonos'),
            style: ElevatedButton.styleFrom(
              primary: Colors.blue,
            ),
          ),
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
        onPressed: () => _refreshCarteras(),
        child: const Icon(Icons.refresh),
        backgroundColor: const Color(0xFF0F0E0E),
      ),
    );
  }
}
