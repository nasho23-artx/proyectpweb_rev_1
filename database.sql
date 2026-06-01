CREATE DATABASE INEA_EvaluacionesDigitalV2;
GO
USE INEA_EvaluacionesDigitalV2;
GO

CREATE TABLE NivelesEducativos (
    IdNivel INT IDENTITY(1,1) PRIMARY KEY,
    NombreNivel VARCHAR(100) NOT NULL
);

CREATE TABLE Materias (
    IdMateria INT IDENTITY(1,1) PRIMARY KEY,
    NombreMateria VARCHAR(150) NOT NULL,
    IdNivel INT FOREIGN KEY REFERENCES NivelesEducativos(IdNivel)
);

CREATE TABLE Alumnos (
    IdAlumno INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto VARCHAR(200) NOT NULL,
    CURP CHAR(18) UNIQUE NOT NULL,
    FechaRegistro DATE DEFAULT GETDATE(),
    IdNivel INT FOREIGN KEY REFERENCES NivelesEducativos(IdNivel)
);

CREATE TABLE Usuarios (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Rol VARCHAR(20) CHECK (Rol IN ('Admin','Asesor')) NOT NULL
);

CREATE TABLE Examenes (
    IdExamen INT IDENTITY(1,1) PRIMARY KEY,
    Titulo VARCHAR(200) NOT NULL,
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    Activo BIT DEFAULT 1,
    IdMateria INT FOREIGN KEY REFERENCES Materias(IdMateria)
);

CREATE TABLE Preguntas (
    IdPregunta INT IDENTITY(1,1) PRIMARY KEY,
    TextoPregunta TEXT NOT NULL,
    Puntaje INT DEFAULT 1,
    IdExamen INT FOREIGN KEY REFERENCES Examenes(IdExamen)
);

CREATE TABLE OpcionesRespuestas (
    IdOpcion INT IDENTITY(1,1) PRIMARY KEY,
    TextoOpcion TEXT NOT NULL,
    EsCorrecta BIT NOT NULL,
    IdPregunta INT FOREIGN KEY REFERENCES Preguntas(IdPregunta)
);

CREATE TABLE Calificaciones (
    IdCalificacion INT IDENTITY(1,1) PRIMARY KEY,
    Calificacion DECIMAL(5,2) NOT NULL,
    FechaAplicacion DATETIME2 DEFAULT GETDATE(),
    IdAlumno INT FOREIGN KEY REFERENCES Alumnos(IdAlumno),
    IdExamen INT FOREIGN KEY REFERENCES Examenes(IdExamen)
);

CREATE TABLE AsesoresMaterias (
    IdUsuario INT FOREIGN KEY REFERENCES Usuarios(IdUsuario),
    IdMateria INT FOREIGN KEY REFERENCES Materias(IdMateria),
    PRIMARY KEY (IdUsuario, IdMateria)
);

CREATE TABLE ExamenesAsignados (
    IdAsignacion INT IDENTITY(1,1) PRIMARY KEY,
    IdAlumno INT FOREIGN KEY REFERENCES Alumnos(IdAlumno),
    IdExamen INT FOREIGN KEY REFERENCES Examenes(IdExamen),
    FechaAsignacion DATETIME2 DEFAULT GETDATE(),
    Realizado BIT DEFAULT 0
);

CREATE TABLE RespuestasAlumno (
    IdRespuesta INT IDENTITY(1,1) PRIMARY KEY,
    IdCalificacion INT FOREIGN KEY REFERENCES Calificaciones(IdCalificacion),
    IdPregunta INT FOREIGN KEY REFERENCES Preguntas(IdPregunta),
    IdOpcionSeleccionada INT FOREIGN KEY REFERENCES OpcionesRespuestas(IdOpcion)
);

-- Insert Test Data
INSERT INTO NivelesEducativos (NombreNivel) VALUES ('Inicial'), ('Primaria'), ('Secundaria');

INSERT INTO Materias (NombreMateria, IdNivel) VALUES ('Matemáticas Básicas', 2), ('Español', 2), ('Ciencias', 3);

-- admin / admin123 (SHA256: 240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9)
INSERT INTO Usuarios (NombreUsuario, PasswordHash, Rol) VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Admin');
INSERT INTO Usuarios (NombreUsuario, PasswordHash, Rol) VALUES ('asesor1', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Asesor');

INSERT INTO Alumnos (NombreCompleto, CURP, IdNivel) VALUES ('Juan Pérez', 'TESTCURP1234567890', 2);

INSERT INTO Examenes (Titulo, IdMateria, Activo) VALUES ('Examen Diagnóstico Matemáticas', 1, 1);

DECLARE @IdExamen INT = SCOPE_IDENTITY();

INSERT INTO Preguntas (TextoPregunta, Puntaje, IdExamen) VALUES ('¿Cuánto es 2 + 2?', 1, @IdExamen);
DECLARE @IdPregunta1 INT = SCOPE_IDENTITY();
INSERT INTO OpcionesRespuestas (TextoOpcion, EsCorrecta, IdPregunta) VALUES ('3', 0, @IdPregunta1), ('4', 1, @IdPregunta1), ('5', 0, @IdPregunta1);

INSERT INTO Preguntas (TextoPregunta, Puntaje, IdExamen) VALUES ('¿Cuál es la raíz cuadrada de 16?', 1, @IdExamen);
DECLARE @IdPregunta2 INT = SCOPE_IDENTITY();
INSERT INTO OpcionesRespuestas (TextoOpcion, EsCorrecta, IdPregunta) VALUES ('4', 1, @IdPregunta2), ('8', 0, @IdPregunta2), ('2', 0, @IdPregunta2);

INSERT INTO ExamenesAsignados (IdAlumno, IdExamen, Realizado) VALUES (1, @IdExamen, 0);
GO
