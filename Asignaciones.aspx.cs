using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI.WebControls;

public partial class Asignaciones : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Rol"] == null || (Session["Rol"].ToString() != "Admin" && Session["Rol"].ToString() != "Asesor"))
        {
            Response.Redirect("Login.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CargarSelects();
            CargarAsignaciones();
        }
    }

    private void CargarSelects()
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            // Alumnos
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT IdAlumno, NombreCompleto + ' (' + CURP + ')' as DisplayName FROM Alumnos ORDER BY NombreCompleto", conn))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlAlumno.DataSource = dt;
                    ddlAlumno.DataTextField = "DisplayName";
                    ddlAlumno.DataValueField = "IdAlumno";
                    ddlAlumno.DataBind();
                }
            }

            // Exámenes Activos
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT IdExamen, Titulo FROM Examenes WHERE Activo = 1 ORDER BY Titulo", conn))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlExamen.DataSource = dt;
                    ddlExamen.DataTextField = "Titulo";
                    ddlExamen.DataValueField = "IdExamen";
                    ddlExamen.DataBind();
                }
            }
        }
    }

    private void CargarAsignaciones()
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = @"SELECT ea.IdAsignacion, a.NombreCompleto, a.CURP, e.Titulo, ea.FechaAsignacion, ea.Realizado 
                             FROM ExamenesAsignados ea 
                             INNER JOIN Alumnos a ON ea.IdAlumno = a.IdAlumno 
                             INNER JOIN Examenes e ON ea.IdExamen = e.IdExamen 
                             ORDER BY ea.IdAsignacion DESC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvAsignaciones.DataSource = dt;
                    gvAsignaciones.DataBind();
                }
            }
        }
    }

    protected void gvAsignaciones_PreRender(object sender, EventArgs e)
    {
        if (gvAsignaciones.Rows.Count > 0)
        {
            gvAsignaciones.UseAccessibleHeader = true;
            gvAsignaciones.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btnAsignar_Click(object sender, EventArgs e)
    {
        if (ddlAlumno.Items.Count == 0 || ddlExamen.Items.Count == 0)
        {
            MostrarMensaje("Debe haber al menos un alumno y un examen para asignar.", false);
            return;
        }

        int idAlumno = int.Parse(ddlAlumno.SelectedValue);
        int idExamen = int.Parse(ddlExamen.SelectedValue);

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            // Validar si ya está asignado
            string checkQuery = "SELECT COUNT(*) FROM ExamenesAsignados WHERE IdAlumno = @alumno AND IdExamen = @examen AND Realizado = 0";
            using (SQLiteCommand cmdCheck = new SQLiteCommand(checkQuery, conn))
            {
                cmdCheck.Parameters.AddWithValue("@alumno", idAlumno);
                cmdCheck.Parameters.AddWithValue("@examen", idExamen);
                conn.Open();
                int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
                if (count > 0)
                {
                    MostrarMensaje("El alumno ya tiene este examen asignado y pendiente.", false);
                    return;
                }
            }

            string query = "INSERT INTO ExamenesAsignados (IdAlumno, IdExamen) VALUES (@alumno, @examen)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@alumno", idAlumno);
                cmd.Parameters.AddWithValue("@examen", idExamen);
                try
                {
                    cmd.ExecuteNonQuery();
                    MostrarMensaje("Examen asignado exitosamente.", true);
                    CargarAsignaciones();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al asignar: " + ex.Message, false);
                }
            }
        }
    }

    protected void gvAsignaciones_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int idAsignacion = Convert.ToInt32(gvAsignaciones.DataKeys[e.RowIndex].Value);
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "DELETE FROM ExamenesAsignados WHERE IdAsignacion = @id AND Realizado = 0";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", idAsignacion);
                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        CargarAsignaciones();
                        MostrarMensaje("Asignación eliminada.", true);
                    }
                    else
                    {
                        MostrarMensaje("No se puede eliminar una asignación ya realizada.", false);
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error: " + ex.Message, false);
                }
            }
        }
    }

    private void MostrarMensaje(string msg, bool exito)
    {
        pnlMensaje.Visible = true;
        lblMensaje.Text = msg;
        pnlMensaje.CssClass = exito ? "alert alert-success alert-dismissible fade show mt-3" : "alert alert-danger alert-dismissible fade show mt-3";
    }
}

