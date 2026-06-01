using System;
using System.Collections.Generic;

public class NivelEducativo {
    public int IdNivel { get; set; }
    public string NombreNivel { get; set; }
}

public class Materia {
    public int IdMateria { get; set; }
    public string NombreMateria { get; set; }
    public int IdNivel { get; set; }
    public string NombreNivel { get; set; }
}

public class Alumno {
    public int IdAlumno { get; set; }
    public string NombreCompleto { get; set; }
    public string CURP { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int IdNivel { get; set; }
    public string NombreNivel { get; set; }
}

public class Usuario {
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; }
    public string PasswordHash { get; set; }
    public string Rol { get; set; }
}

public class Examen {
    public int IdExamen { get; set; }
    public string Titulo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
    public int IdMateria { get; set; }
    public string NombreMateria { get; set; }
}

public class Pregunta {
    public int IdPregunta { get; set; }
    public string TextoPregunta { get; set; }
    public int Puntaje { get; set; }
    public int IdExamen { get; set; }
    public List<OpcionRespuesta> Opciones { get; set; }
    
    public Pregunta() {
        Opciones = new List<OpcionRespuesta>();
    }
}

public class OpcionRespuesta {
    public int IdOpcion { get; set; }
    public string TextoOpcion { get; set; }
    public bool EsCorrecta { get; set; }
    public int IdPregunta { get; set; }
}

public class CalificacionModel {
    public int IdCalificacion { get; set; }
    public decimal Calificacion { get; set; }
    public DateTime FechaAplicacion { get; set; }
    public int IdAlumno { get; set; }
    public string NombreAlumno { get; set; }
    public int IdExamen { get; set; }
    public string TituloExamen { get; set; }
}

public class ExamenAsignado {
    public int IdAsignacion { get; set; }
    public int IdAlumno { get; set; }
    public string NombreAlumno { get; set; }
    public string CURP { get; set; }
    public int IdExamen { get; set; }
    public string TituloExamen { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public bool Realizado { get; set; }
}
