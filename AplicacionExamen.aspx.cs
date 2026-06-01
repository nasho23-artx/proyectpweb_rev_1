using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Web.Script.Serialization;
using System.Web.Services;

public partial class AplicacionExamen : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Rol"] == null || Session["Rol"].ToString() != "Alumno")
        {
            Response.Redirect("Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            if (Request.QueryString["id"] != null)
            {
                int idAsignacion;
                if (int.TryParse(Request.QueryString["id"], out idAsignacion))
                {
                    hfIdAsignacion.Value = idAsignacion.ToString();
                    CargarDatosExamen(idAsignacion);
                }
                else
                {
                    Response.Redirect("Default.aspx");
                }
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }
    }

    private void CargarDatosExamen(int idAsignacion)
    {
        int idAlumno = Convert.ToInt32(Session["IdAlumno"]);
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = @"SELECT e.Titulo, m.NombreMateria, ea.Realizado 
                             FROM ExamenesAsignados ea 
                             INNER JOIN Examenes e ON ea.IdExamen = e.IdExamen 
                             INNER JOIN Materias m ON e.IdMateria = m.IdMateria
                             WHERE ea.IdAsignacion = @id AND ea.IdAlumno = @alumno";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", idAsignacion);
                cmd.Parameters.AddWithValue("@alumno", idAlumno);
                conn.Open();
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (Convert.ToBoolean(reader["Realizado"]))
                        {
                            Response.Redirect("Default.aspx"); // Ya fue realizado
                        }
                        lblTituloExamen.Text = reader["Titulo"].ToString();
                        lblMateria.Text = reader["NombreMateria"].ToString();
                    }
                    else
                    {
                        Response.Redirect("Default.aspx"); // No existe o no le pertenece
                    }
                }
            }
        }
    }

    [WebMethod(EnableSession = true)]
    public static string ObtenerPreguntas(int idAsignacion)
    {
        int idAlumno = Convert.ToInt32(System.Web.HttpContext.Current.Session["IdAlumno"]);
        List<PreguntaDto> preguntas = new List<PreguntaDto>();

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            // Validar asignacion y obtener idExamen
            int idExamen = 0;
            string qAsig = "SELECT IdExamen FROM ExamenesAsignados WHERE IdAsignacion = @id AND IdAlumno = @al AND Realizado = 0";
            using (SQLiteCommand cmd = new SQLiteCommand(qAsig, conn))
            {
                cmd.Parameters.AddWithValue("@id", idAsignacion);
                cmd.Parameters.AddWithValue("@al", idAlumno);
                conn.Open();
                object obj = cmd.ExecuteScalar();
                if (obj == null) return "[]";
                idExamen = Convert.ToInt32(obj);
            }

            // Obtener preguntas
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT IdPregunta, TextoPregunta FROM Preguntas WHERE IdExamen = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", idExamen);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        preguntas.Add(new PreguntaDto
                        {
                            IdPregunta = Convert.ToInt32(reader["IdPregunta"]),
                            TextoPregunta = reader["TextoPregunta"].ToString()
                        });
                    }
                }
            }

            // Obtener opciones (para simplificar, obtenemos todas las del examen)
            using (SQLiteCommand cmd = new SQLiteCommand(@"SELECT o.IdOpcion, o.TextoOpcion, o.IdPregunta 
                                                     FROM OpcionesRespuestas o 
                                                     INNER JOIN Preguntas p ON o.IdPregunta = p.IdPregunta 
                                                     WHERE p.IdExamen = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", idExamen);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idPreg = Convert.ToInt32(reader["IdPregunta"]);
                        var preg = preguntas.Find(p => p.IdPregunta == idPreg);
                        if (preg != null)
                        {
                            preg.Opciones.Add(new OpcionDto
                            {
                                IdOpcion = Convert.ToInt32(reader["IdOpcion"]),
                                TextoOpcion = reader["TextoOpcion"].ToString()
                            });
                        }
                    }
                }
            }
        }

        JavaScriptSerializer js = new JavaScriptSerializer();
        return js.Serialize(preguntas);
    }

    [WebMethod(EnableSession = true)]
    public static object ProcesarRespuestas(int idAsignacion, List<RespuestaDto> respuestas)
    {
        int idAlumno = Convert.ToInt32(System.Web.HttpContext.Current.Session["IdAlumno"]);
        decimal calificacion = 0;
        int idCalificacion = 0;

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            conn.Open();
            int idExamen = 0;
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT IdExamen FROM ExamenesAsignados WHERE IdAsignacion = @id AND IdAlumno = @al", conn))
            {
                cmd.Parameters.AddWithValue("@id", idAsignacion);
                cmd.Parameters.AddWithValue("@al", idAlumno);
                object obj = cmd.ExecuteScalar();
                if (obj == null) throw new Exception("Asignación inválida");
                idExamen = Convert.ToInt32(obj);
            }

            int totalPreguntas = 0;
            int respuestasCorrectas = 0;

            // Contar total de preguntas
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM Preguntas WHERE IdExamen = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", idExamen);
                totalPreguntas = Convert.ToInt32(cmd.ExecuteScalar());
            }

            if (totalPreguntas > 0)
            {
                // Verificar respuestas
                foreach (var resp in respuestas)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT EsCorrecta FROM OpcionesRespuestas WHERE IdOpcion = @idOp AND IdPregunta = @idPreg", conn))
                    {
                        cmd.Parameters.AddWithValue("@idOp", resp.IdOpcion);
                        cmd.Parameters.AddWithValue("@idPreg", resp.IdPregunta);
                        object esCorr = cmd.ExecuteScalar();
                        if (esCorr != null && Convert.ToBoolean(esCorr))
                        {
                            respuestasCorrectas++;
                        }
                    }
                }
                calificacion = Math.Round(((decimal)respuestasCorrectas / totalPreguntas) * 100, 2);
            }

            // Guardar calificación y marcar realizado
            SqlTransaction tx = conn.BeginTransaction();
            try
            {
                string qInsert = "INSERT INTO Calificaciones (Calificacion, IdAlumno, IdExamen) VALUES (@calif, @al, @ex); SELECT SCOPE_IDENTITY();";
                using (SQLiteCommand cmd = new SQLiteCommand(qInsert, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@calif", calificacion);
                    cmd.Parameters.AddWithValue("@al", idAlumno);
                    cmd.Parameters.AddWithValue("@ex", idExamen);
                    idCalificacion = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach(var r in respuestas) {
                    string qResp = "INSERT INTO RespuestasAlumno (IdCalificacion, IdPregunta, IdOpcionSeleccionada) VALUES (@idCal, @idPreg, @idOpt)";
                    using(SQLiteCommand cmdR = new SQLiteCommand(qResp, conn, tx)){
                        cmdR.Parameters.AddWithValue("@idCal", idCalificacion);
                        cmdR.Parameters.AddWithValue("@idPreg", r.IdPregunta);
                        cmdR.Parameters.AddWithValue("@idOpt", r.IdOpcion);
                        cmdR.ExecuteNonQuery();
                    }
                }

                string qUpdate = "UPDATE ExamenesAsignados SET Realizado = 1 WHERE IdAsignacion = @id";
                using (SQLiteCommand cmd = new SQLiteCommand(qUpdate, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", idAsignacion);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }

        return new { Calificacion = calificacion, IdCalificacion = idCalificacion };
    }

    public class PreguntaDto
    {
        public int IdPregunta { get; set; }
        public string TextoPregunta { get; set; }
        public List<OpcionDto> Opciones { get; set; }
        
        public PreguntaDto() {
            Opciones = new List<OpcionDto>();
        }
    }

    public class OpcionDto
    {
        public int IdOpcion { get; set; }
        public string TextoOpcion { get; set; }
    }

    public class RespuestaDto
    {
        public int IdPregunta { get; set; }
        public int IdOpcion { get; set; }
    }
}

