using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI.WebControls;

public partial class Materias : System.Web.UI.Page
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
            CargarMaterias();
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

    private void CargarMaterias()
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = @"SELECT m.IdMateria, m.NombreMateria, m.IdNivel, n.NombreNivel 
                             FROM Materias m 
                             INNER JOIN NivelesEducativos n ON m.IdNivel = n.IdNivel 
                             ORDER BY m.IdMateria DESC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvMaterias.DataSource = dt;
                    gvMaterias.DataBind();
                }
            }
        }
    }

    protected void gvMaterias_PreRender(object sender, EventArgs e)
    {
        if (gvMaterias.Rows.Count > 0)
        {
            gvMaterias.UseAccessibleHeader = true;
            gvMaterias.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        string nombre = txtNombre.Text.Trim();
        int idNivel = int.Parse(ddlNivel.SelectedValue);

        if (string.IsNullOrEmpty(nombre))
        {
            MostrarMensaje("Ingrese el nombre de la materia.", false);
            return;
        }

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "INSERT INTO Materias (NombreMateria, IdNivel) VALUES (@nom, @nivel)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@nom", nombre);
                cmd.Parameters.AddWithValue("@nivel", idNivel);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MostrarMensaje("Materia agregada exitosamente.", true);
                    txtNombre.Text = "";
                    CargarMaterias();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al guardar: " + ex.Message, false);
                }
            }
        }
    }

    protected void gvMaterias_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvMaterias.EditIndex = e.NewEditIndex;
        CargarMaterias();

        DropDownList ddlEditNivel = (DropDownList)gvMaterias.Rows[e.NewEditIndex].FindControl("ddlEditNivel");
        HiddenField hfIdNivel = (HiddenField)gvMaterias.Rows[e.NewEditIndex].FindControl("hfIdNivel");

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

    protected void gvMaterias_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvMaterias.EditIndex = -1;
        CargarMaterias();
    }

    protected void gvMaterias_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int idMateria = Convert.ToInt32(gvMaterias.DataKeys[e.RowIndex].Value);
        TextBox txtNombre = (TextBox)gvMaterias.Rows[e.RowIndex].FindControl("txtEditNombre");
        DropDownList ddlNivel = (DropDownList)gvMaterias.Rows[e.RowIndex].FindControl("ddlEditNivel");

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "UPDATE Materias SET NombreMateria = @nom, IdNivel = @nivel WHERE IdMateria = @id";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@nom", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@nivel", int.Parse(ddlNivel.SelectedValue));
                cmd.Parameters.AddWithValue("@id", idMateria);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    gvMaterias.EditIndex = -1;
                    CargarMaterias();
                    MostrarMensaje("Materia actualizada exitosamente.", true);
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al actualizar: " + ex.Message, false);
                }
            }
        }
    }

    protected void gvMaterias_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int idMateria = Convert.ToInt32(gvMaterias.DataKeys[e.RowIndex].Value);
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "DELETE FROM Materias WHERE IdMateria = @id";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", idMateria);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    CargarMaterias();
                    MostrarMensaje("Materia eliminada.", true);
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547)
                        MostrarMensaje("No se puede eliminar porque hay exámenes o registros asociados a esta materia.", false);
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

