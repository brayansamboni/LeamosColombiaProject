class UsuarioModel {
  final int idUsuario;
  final String nombre;
  final String correo;
  final bool estado;

  UsuarioModel({
    required this.idUsuario,
    required this.nombre,
    required this.correo,
    required this.estado,
  });

  factory UsuarioModel.fromJson(Map<String, dynamic> json) {
    return UsuarioModel(
      idUsuario: json['idUsuario'],
      nombre: json['nombre'],
      correo: json['correo'],
      estado: json['estado'],
    );
  }
}
