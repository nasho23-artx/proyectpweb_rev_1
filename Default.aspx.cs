using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Rol"] == null)
        {
            Response.Redirect("Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            string rol = Session["Rol"].ToString();
            if (rol == "Admin" || rol == "Asesor")
            {
                pnlAdmin.Visible = true;
                CargarDashboardAdmin();
            }
            else if (rol == "Alumno")
            {
                pnlAlumno.Visible = true;
                lblNombreAlumno.Text = Session["Usuario"].ToString();
                CargarDashboardAlumno();
            }
        }
    }

    private void CargarDashboardAdmin()
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            conn.Open();
            // Total Alumnos
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM Alumnos", conn))
            {
                lblTotalAlumnos.Text = cmd.ExecuteScalar().ToString();
            }
            // Examenes Activos
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM Examenes WHERE Activo = 1", conn))
            {
                lblExamenesActivos.Text = cmd.ExecuteScalar().ToString();
            }
            // Asignaciones Pendientes
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM ExamenesAsignados WHERE Realizado = 0", conn))
            {
                lblAsignaciones.Text = cmd.ExecuteScalar().ToString();
            }
            // Promedio General
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT ISNULL(AVG(Calificacion), 0) FROM Calificaciones", conn))
            {
                lblPromedio.Text = Convert.ToDecimal(cmd.ExecuteScalar()).ToString("F2");
            }
        }
    }

    private void CargarDashboardAlumno()
    {
        int idAlumno = Convert.ToInt32(Session["IdAlumno"]);
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            conn.Open();
            // Examenes pendientes
            string queryPendientes = @"
                SELECT ea.IdAsignacion, e.Titulo as TituloExamen, ea.FechaAsignacion
                FROM ExamenesAsignados ea
                INNER JOIN Examenes e ON ea.IdExamen = e.IdExamen
                WHERE ea.IdAlumno = @IdAlumno AND ea.Realizado = 0 AND e.Activo = 1";
            
            using (SQLiteCommand cmd = new SQLiteCommand(queryPendientes, conn))
            {
                cmd.Parameters.AddWithValue("@IdAlumno", idAlumno);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        rptExamenesPendientes.DataSource = dt;
                        rptExamenesPendientes.DataBind();
                    }
                    else
                    {
                        lblSinExamenes.Visible = true;
                    }
                }
            }

            // Historial
            string queryHistorial = @"
                SELECT c.IdCalificacion, c.Calificacion, c.FechaAplicacion, e.Titulo as TituloExamen
                FROM Calificaciones c
                INNER JOIN Examenes e ON c.IdExamen = e.IdExamen
                WHERE c.IdAlumno = @IdAlumno
                ORDER BY c.FechaAplicacion DESC";

            using (SQLiteCommand cmd = new SQLiteCommand(queryHistorial, conn))
            {
                cmd.Parameters.AddWithValue("@IdAlumno", idAlumno);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        rptHistorial.DataSource = dt;
                        rptHistorial.DataBind();
                    }
                    else
                    {
                        trSinHistorial.Visible = true;
                    }
                }
            }
        }
    }
}

