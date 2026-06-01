using System;
using System.Data;
using System.Data.SQLite;
using System.Configuration;
using System.IO;
using System.Web;

public class DBContext {
    private string connectionString = ConfigurationManager.ConnectionStrings["SQLiteConnection"].ConnectionString;

    public SQLiteConnection GetConnection() {
        return new SQLiteConnection(connectionString);
    }

    public static void InitializeDatabase() {
        string appDataPath = HttpContext.Current.Server.MapPath("~/App_Data");
        if (!Directory.Exists(appDataPath)) {
            Directory.CreateDirectory(appDataPath);
        }

        string dbPath = Path.Combine(appDataPath, "proyecto_escolar.db");
        if (!File.Exists(dbPath)) {
            SQLiteConnection.CreateFile(dbPath);
            using (var conn = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;")) {
                conn.Open();
                string script = @"
                    CREATE TABLE NivelesEducativos (
                        IdNivel INTEGER PRIMARY KEY AUTOINCREMENT,
                        NombreNivel TEXT NOT NULL
                    );

                    CREATE TABLE Materias (
                        IdMateria INTEGER PRIMARY KEY AUTOINCREMENT,
                        NombreMateria TEXT NOT NULL,
                        IdNivel INTEGER,
                        FOREIGN KEY(IdNivel) REFERENCES NivelesEducativos(IdNivel)
                    );

                    CREATE TABLE Alumnos (
                        IdAlumno INTEGER PRIMARY KEY AUTOINCREMENT,
                        NombreCompleto TEXT NOT NULL,
                        CURP TEXT UNIQUE NOT NULL,
                        FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
                        IdNivel INTEGER,
                        FOREIGN KEY(IdNivel) REFERENCES NivelesEducativos(IdNivel)
                    );

                    CREATE TABLE Usuarios (
                        IdUsuario INTEGER PRIMARY KEY AUTOINCREMENT,
                        NombreUsuario TEXT UNIQUE NOT NULL,
                        PasswordHash TEXT NOT NULL,
                        Rol TEXT CHECK(Rol IN ('Admin','Asesor')) NOT NULL
                    );

                    CREATE TABLE Examenes (
                        IdExamen INTEGER PRIMARY KEY AUTOINCREMENT,
                        Titulo TEXT NOT NULL,
                        FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
                        Activo INTEGER DEFAULT 1,
                        IdMateria INTEGER,
                        FOREIGN KEY(IdMateria) REFERENCES Materias(IdMateria)
                    );

                    CREATE TABLE Preguntas (
                        IdPregunta INTEGER PRIMARY KEY AUTOINCREMENT,
                        TextoPregunta TEXT NOT NULL,
                        Puntaje INTEGER DEFAULT 1,
                        IdExamen INTEGER,
                        FOREIGN KEY(IdExamen) REFERENCES Examenes(IdExamen)
                    );

                    CREATE TABLE OpcionesRespuestas (
                        IdOpcion INTEGER PRIMARY KEY AUTOINCREMENT,
                        TextoOpcion TEXT NOT NULL,
                        EsCorrecta INTEGER NOT NULL,
                        IdPregunta INTEGER,
                        FOREIGN KEY(IdPregunta) REFERENCES Preguntas(IdPregunta)
                    );

                    CREATE TABLE Calificaciones (
                        IdCalificacion INTEGER PRIMARY KEY AUTOINCREMENT,
                        Calificacion REAL NOT NULL,
                        FechaAplicacion DATETIME DEFAULT CURRENT_TIMESTAMP,
                        IdAlumno INTEGER,
                        IdExamen INTEGER,
                        FOREIGN KEY(IdAlumno) REFERENCES Alumnos(IdAlumno),
                        FOREIGN KEY(IdExamen) REFERENCES Examenes(IdExamen)
                    );

                    CREATE TABLE AsesoresMaterias (
                        IdUsuario INTEGER,
                        IdMateria INTEGER,
                        PRIMARY KEY (IdUsuario, IdMateria),
                        FOREIGN KEY(IdUsuario) REFERENCES Usuarios(IdUsuario),
                        FOREIGN KEY(IdMateria) REFERENCES Materias(IdMateria)
                    );

                    CREATE TABLE ExamenesAsignados (
                        IdAsignacion INTEGER PRIMARY KEY AUTOINCREMENT,
                        IdAlumno INTEGER,
                        IdExamen INTEGER,
                        FechaAsignacion DATETIME DEFAULT CURRENT_TIMESTAMP,
                        Realizado INTEGER DEFAULT 0,
                        FOREIGN KEY(IdAlumno) REFERENCES Alumnos(IdAlumno),
                        FOREIGN KEY(IdExamen) REFERENCES Examenes(IdExamen)
                    );

                    CREATE TABLE RespuestasAlumno (
                        IdRespuesta INTEGER PRIMARY KEY AUTOINCREMENT,
                        IdCalificacion INTEGER,
                        IdPregunta INTEGER,
                        IdOpcionSeleccionada INTEGER,
                        FOREIGN KEY(IdCalificacion) REFERENCES Calificaciones(IdCalificacion),
                        FOREIGN KEY(IdPregunta) REFERENCES Preguntas(IdPregunta),
                        FOREIGN KEY(IdOpcionSeleccionada) REFERENCES OpcionesRespuestas(IdOpcion)
                    );

                    INSERT INTO NivelesEducativos (NombreNivel) VALUES ('Inicial'), ('Primaria'), ('Secundaria');
                    INSERT INTO Materias (NombreMateria, IdNivel) VALUES ('Matemáticas Básicas', 2), ('Español', 2), ('Ciencias', 3);
                    INSERT INTO Usuarios (NombreUsuario, PasswordHash, Rol) VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Admin');
                    INSERT INTO Usuarios (NombreUsuario, PasswordHash, Rol) VALUES ('asesor1', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Asesor');
                    INSERT INTO Alumnos (NombreCompleto, CURP, IdNivel) VALUES ('Juan Pérez', 'TESTCURP1234567890', 2);
                    INSERT INTO Examenes (Titulo, IdMateria, Activo) VALUES ('Examen Diagnóstico Matemáticas', 1, 1);
                ";
                using (var cmd = new SQLiteCommand(script, conn)) {
                    cmd.ExecuteNonQuery();
                }

                // Insert questions
                string preguntasScript = @"
                    INSERT INTO Preguntas (TextoPregunta, Puntaje, IdExamen) VALUES ('¿Cuánto es 2 + 2?', 1, 1);
                    INSERT INTO OpcionesRespuestas (TextoOpcion, EsCorrecta, IdPregunta) VALUES ('3', 0, 1), ('4', 1, 1), ('5', 0, 1);
                    INSERT INTO Preguntas (TextoPregunta, Puntaje, IdExamen) VALUES ('¿Cuál es la raíz cuadrada de 16?', 1, 1);
                    INSERT INTO OpcionesRespuestas (TextoOpcion, EsCorrecta, IdPregunta) VALUES ('4', 1, 2), ('8', 0, 2), ('2', 0, 2);
                    INSERT INTO ExamenesAsignados (IdAlumno, IdExamen, Realizado) VALUES (1, 1, 0);
                ";
                using (var cmd = new SQLiteCommand(preguntasScript, conn)) {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
