using System;
using System.Data;
using System.Data.SQLite;

public partial class ResultadosExamen : System.Web.UI.Page
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
                int idCalificacion;
                if (int.TryParse(Request.QueryString["id"], out idCalificacion))
                {
                    CargarResultados(idCalificacion);
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

    private void CargarResultados(int idCalificacion)
    {
        int idAlumno = Convert.ToInt32(Session["IdAlumno"]);
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string qCabecera = @"SELECT c.Calificacion, e.Titulo, a.NombreCompleto 
                                 FROM Calificaciones c
                                 INNER JOIN Examenes e ON c.IdExamen = e.IdExamen
                                 INNER JOIN Alumnos a ON c.IdAlumno = a.IdAlumno
                                 WHERE c.IdCalificacion = @idCal AND c.IdAlumno = @idAlum";
            using (SQLiteCommand cmd = new SQLiteCommand(qCabecera, conn))
            {
                cmd.Parameters.AddWithValue("@idCal", idCalificacion);
                cmd.Parameters.AddWithValue("@idAlum", idAlumno);
                conn.Open();
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblTituloExamen.Text = reader["Titulo"].ToString();
                        lblAlumno.Text = "Alumno: " + reader["NombreCompleto"].ToString();
                        decimal calif = Convert.ToDecimal(reader["Calificacion"]);
                        lblCalificacion.Text = calif.ToString("F2") + " / 100";
                        if (calif >= 70)
                            lblCalificacion.CssClass += " text-success";
                        else
                            lblCalificacion.CssClass += " text-danger";
                    }
                    else
                    {
                        Response.Redirect("Default.aspx");
                        return;
                    }
                }
            }

            // Cargar el detalle de respuestas
            string qDetalle = @"SELECT p.TextoPregunta, 
                                       os.TextoOpcion AS OpcionSeleccionada, 
                                       os.EsCorrecta,
                                       (SELECT TextoOpcion FROM OpcionesRespuestas WHERE IdPregunta = p.IdPregunta AND EsCorrecta = 1) AS OpcionCorrecta
                                FROM RespuestasAlumno ra
                                INNER JOIN Preguntas p ON ra.IdPregunta = p.IdPregunta
                                INNER JOIN OpcionesRespuestas os ON ra.IdOpcionSeleccionada = os.IdOpcion
                                WHERE ra.IdCalificacion = @idCal";
            using (SQLiteCommand cmd = new SQLiteCommand(qDetalle, conn))
            {
                cmd.Parameters.AddWithValue("@idCal", idCalificacion);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    rptRespuestas.DataSource = dt;
                    rptRespuestas.DataBind();
                }
            }
        }
    }
}

