using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI.WebControls;

public partial class Alumnos : System.Web.UI.Page
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
            CargarNiveles();
            CargarAlumnos();
        }
    }

    private void CargarNiveles()
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT IdNivel, NombreNivel FROM NivelesEducativos", conn))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlNivel.DataSource = dt;
                    ddlNivel.DataTextField = "NombreNivel";
                    ddlNivel.DataValueField = "IdNivel";
                    ddlNivel.DataBind();
                }
            }
        }
    }

    private void CargarAlumnos()
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = @"SELECT a.IdAlumno, a.NombreCompleto, a.CURP, a.FechaRegistro, a.IdNivel, n.NombreNivel 
                             FROM Alumnos a 
                             INNER JOIN NivelesEducativos n ON a.IdNivel = n.IdNivel 
                             ORDER BY a.IdAlumno DESC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvAlumnos.DataSource = dt;
                    gvAlumnos.DataBind();
                }
            }
        }
    }

    protected void gvAlumnos_PreRender(object sender, EventArgs e)
    {
        if (gvAlumnos.Rows.Count > 0)
        {
            gvAlumnos.UseAccessibleHeader = true;
            gvAlumnos.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        string nombre = txtNombre.Text.Trim();
        string curp = txtCURP.Text.Trim().ToUpper();
        int idNivel = int.Parse(ddlNivel.SelectedValue);

        if (string.IsNullOrEmpty(nombre) || curp.Length != 18)
        {
            MostrarMensaje("Complete los datos correctamente. La CURP debe tener 18 caracteres.", false);
            return;
        }

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "INSERT INTO Alumnos (NombreCompleto, CURP, IdNivel) VALUES (@nom, @curp, @nivel)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@nom", nombre);
                cmd.Parameters.AddWithValue("@curp", curp);
                cmd.Parameters.AddWithValue("@nivel", idNivel);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MostrarMensaje("Alumno agregado exitosamente.", true);
                    txtNombre.Text = "";
                    txtCURP.Text = "";
                    CargarAlumnos();
                }
                catch (SQLiteException ex)
                {
                    if (ex.ResultCode == SQLiteErrorCode.Constraint) // Unique constraint
                        MostrarMensaje("Error: La CURP ya está registrada.", false);
                    else
                        MostrarMensaje("Error al guardar: " + ex.Message, false);
                }
            }
        }
    }

    protected void gvAlumnos_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvAlumnos.EditIndex = e.NewEditIndex;
        CargarAlumnos();

        DropDownList ddlEditNivel = (DropDownList)gvAlumnos.Rows[e.NewEditIndex].FindControl("ddlEditNivel");
        HiddenField hfIdNivel = (HiddenField)gvAlumnos.Rows[e.NewEditIndex].FindControl("hfIdNivel");

        if (ddlEditNivel != null && hfIdNivel != null)
        {
            using (SQLiteConnection conn = new DBContext().GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT IdNivel, NombreNivel FROM NivelesEducativos", conn))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        ddlEditNivel.DataSource = dt;
                        ddlEditNivel.DataTextField = "NombreNivel";
                        ddlEditNivel.DataValueField = "IdNivel";
                        ddlEditNivel.DataBind();
                        ddlEditNivel.SelectedValue = hfIdNivel.Value;
                    }
                }
            }
        }
    }

    protected void gvAlumnos_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvAlumnos.EditIndex = -1;
        CargarAlumnos();
    }

    protected void gvAlumnos_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int idAlumno = Convert.ToInt32(gvAlumnos.DataKeys[e.RowIndex].Value);
        TextBox txtNombre = (TextBox)gvAlumnos.Rows[e.RowIndex].FindControl("txtEditNombre");
        TextBox txtCURP = (TextBox)gvAlumnos.Rows[e.RowIndex].FindControl("txtEditCURP");
        DropDownList ddlNivel = (DropDownList)gvAlumnos.Rows[e.RowIndex].FindControl("ddlEditNivel");

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "UPDATE Alumnos SET NombreCompleto = @nom, CURP = @curp, IdNivel = @nivel WHERE IdAlumno = @id";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@nom", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@curp", txtCURP.Text.Trim().ToUpper());
                cmd.Parameters.AddWithValue("@nivel", int.Parse(ddlNivel.SelectedValue));
                cmd.Parameters.AddWithValue("@id", idAlumno);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    gvAlumnos.EditIndex = -1;
                    CargarAlumnos();
                    MostrarMensaje("Alumno actualizado exitosamente.", true);
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al actualizar: " + ex.Message, false);
                }
            }
        }
    }

    protected void gvAlumnos_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int idAlumno = Convert.ToInt32(gvAlumnos.DataKeys[e.RowIndex].Value);
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "DELETE FROM Alumnos WHERE IdAlumno = @id";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", idAlumno);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    CargarAlumnos();
                    MostrarMensaje("Alumno eliminado.", true);
                }
                catch (SQLiteException ex)
                {
                    if (ex.ResultCode == SQLiteErrorCode.Constraint)
                        MostrarMensaje("No se puede eliminar porque el alumno tiene exámenes asignados/realizados.", false);
                    else
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

