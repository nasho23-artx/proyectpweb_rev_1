using System;
using System.Data.SQLite;
using System.Web.Security;

public partial class LoginAlumno : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (User.Identity.IsAuthenticated && Session["Rol"] != null && Session["Rol"].ToString() == "Alumno")
        {
            Response.Redirect("Default.aspx");
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string curp = txtCURP.Text.Trim().ToUpper();

        if (string.IsNullOrEmpty(curp) || curp.Length != 18)
        {
            MostrarError("Ingrese una CURP válida de 18 caracteres.");
            return;
        }

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "SELECT IdAlumno, NombreCompleto FROM Alumnos WHERE CURP = @curp";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@curp", curp);
                conn.Open();
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Session["IdAlumno"] = reader["IdAlumno"].ToString();
                        Session["Usuario"] = reader["NombreCompleto"].ToString();
                        Session["Rol"] = "Alumno";
                        
                        FormsAuthentication.SetAuthCookie(curp, false);
                        Response.Redirect("Default.aspx");
                    }
                    else
                    {
                        MostrarError("CURP no encontrada en el sistema.");
                    }
                }
            }
        }
    }

    private void MostrarError(string mensaje)
    {
        pnlError.Visible = true;
        lblError.Text = mensaje;
    }
}

