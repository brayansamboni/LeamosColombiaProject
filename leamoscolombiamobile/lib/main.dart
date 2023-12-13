import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:leamoscolombiamobile/principal.dart';

void main() {
  runApp(const MyApp());
}

class CredencialesInicioSesion {
  final String correo;
  final String contrasena;

  CredencialesInicioSesion(this.correo, this.contrasena);

  Map<String, dynamic> toJson() {
    return {'correo': correo, 'contraseña': contrasena};
  }
}

class AuthService {
  final String baseUrl;

  AuthService(this.baseUrl);

  Future<void> iniciarSesion(CredencialesInicioSesion credenciales) async {
    try {
      final response = await http.post(
        Uri.parse('http://leamoscolombia12-001-site1.atempurl.com/api/Api/IniciarSesion/'),
        headers: {'Content-Type': 'application/json'},
        body: json.encode(credenciales.toJson()),
      );

      if (response.statusCode == 200) {
        // Autenticación exitosa
        print('Usuario autenticado exitosamente');
        // Puedes devolver información adicional sobre el usuario o el token si es necesario
      } else if (response.statusCode == 401) {
        // Credenciales inválidas
        throw Exception('Correo electrónico o contraseña incorrectos');
      } else {
        // Otro código de estado, manejar según sea necesario
        throw Exception('Error en la autenticación: ${response.statusCode}');
      }
    } catch (e) {
      // Manejar excepciones, por ejemplo, problemas de conexión
      throw Exception(
          'Error en la autenticación. Por favor, inténtelo de nuevo.');
    }

    Future<void> cerrarSesion() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/api/Api/CerrarSesion'),
      );

      if (response.statusCode == 200) {
        // Sesión cerrada exitosamente
        print('Sesión cerrada exitosamente');
      } else {
        // Manejar otros códigos de estado según sea necesario
        throw Exception('Error al cerrar sesión: ${response.statusCode}');
      }
    } catch (e) {
      // Manejar excepciones, por ejemplo, problemas de conexión
      throw Exception('Error al cerrar sesión. Por favor, inténtelo de nuevo.');
    }
  }
  }
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Leamos Colombia',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: const Color.fromARGB(255, 0, 0, 0)),
        useMaterial3: true,
      ),
      home: const LoginPage(),
                  debugShowCheckedModeBanner: false, // Set this to false
    );
  }
}

class LoginPage extends StatefulWidget {
  const LoginPage({Key? key}) : super(key: key);

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF0F0E0E),
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(20.0),
          child: SingleChildScrollView(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: <Widget>[
                Container(
                  width: double.infinity,
                  decoration: BoxDecoration(
                    color: const Color(0xFF186F65),
                    borderRadius: const BorderRadius.only(
                      topLeft: Radius.circular(20),
                      topRight: Radius.circular(20),
                    ),
                  ),
                  padding: const EdgeInsets.all(20.0),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        'Bienvenidos',
                        style: TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.bold,
                          color: const Color(0xFFF6FBF4),
                        ),
                      ),
                      const SizedBox(height: 20),
                      Text(
                        'Si no tienes cuenta, comunícate con el administrador',
                        style: TextStyle(
                          color: const Color(0xFFF6FBF4),
                        ),
                        textAlign: TextAlign.center,
                      ),
                    ],
                  ),
                ),
                Container(
                  width: double.infinity,
                  height: 400,
                  decoration: BoxDecoration(
                    color: const Color(0xFFF6FBF4),
                    borderRadius: const BorderRadius.only(
                      bottomLeft: Radius.circular(20),
                      bottomRight: Radius.circular(20),
                    ),
                  ),
                  padding: const EdgeInsets.all(20.0),
                  child: Form(
                    key: _formKey,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.center,
                      children: [
                        const Text(
                          'Iniciar sesión',
                          style: TextStyle(
                            fontSize: 24,
                            fontWeight: FontWeight.bold,
                            color: Color(0xFF186F65),
                          ),
                        ),
                        const SizedBox(height: 20),
                        Container(
                          width: double.infinity,
                          child: TextFormField(
                            controller: _emailController,
                            decoration: const InputDecoration(
                              labelText: 'Correo electrónico',
                              fillColor: const Color(0xFFF6FBF4),
                              filled: true,
                            ),
                            keyboardType: TextInputType.emailAddress,
                            validator: (value) {
                              if (value == null || value.isEmpty) {
                                return 'Por favor, ingresa tu correo';
                              } else if (!RegExp(
                                      r'^[a-zA-Z0-9.]+@[a-zA-Z0-9]+\.[a-zA-Z]+')
                                  .hasMatch(value)) {
                                return 'Por favor, ingresa un correo válido';
                              }
                              return null;
                            },
                          ),
                        ),
                        const SizedBox(height: 20),
                        Container(
                          width: double.infinity,
                          child: TextFormField(
                            controller: _passwordController,
                            obscureText: true,
                            decoration: const InputDecoration(
                              labelText: 'Contraseña',
                              fillColor: const Color(0xFFF6FBF4),
                              filled: true,
                            ),
                            validator: (value) {
                              if (value == null || value.isEmpty) {
                                return 'Por favor, ingresa tu contraseña';
                              }
                              return null;
                            },
                          ),
                        ),
                        const SizedBox(height: 40),
                        ElevatedButton(
                          onPressed: () {
                            if (_formKey.currentState!.validate()) {
                              _loginUser(context);
                            }
                          },
                          style: ElevatedButton.styleFrom(
                            backgroundColor: const Color(0xFF186F65),
                          ),
                          child: const Text(
                            'Ingresar',
                            style: TextStyle(
                              color: const Color(0xFFF6FBF4),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
Future<void> _loginUser(BuildContext context) async {
  String email = _emailController.text;
  String password = _passwordController.text;
  final AuthService authService =
      AuthService('http://leamoscolombia12-001-site1.atempurl.com');

  final credenciales = CredencialesInicioSesion(email, password);

  try {
    await authService.iniciarSesion(credenciales);
    // La autenticación fue exitosa
    Navigator.pushAndRemoveUntil(
      context,
      MaterialPageRoute(builder: (context) => PrincipalApp()),
      (Route<dynamic> route) => false,
    );
  } catch (e) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Correo y/o contraseña incorrectos'),
        duration: const Duration(seconds: 2),
      ),
    );
  }
}
}
