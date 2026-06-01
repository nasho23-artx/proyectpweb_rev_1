using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (User.Identity.IsAuthenticated && Session["Rol"] != null && (Session["Rol"].ToString() == "Admin" || Session["Rol"].ToString() == "Asesor"))
        {
            Response.Redirect("Default.aspx");
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string usuario = txtUsuario.Text.Trim();
        string password = txtPassword.Text;

        if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
        {
            MostrarError("Ingrese usuario y contraseña.");
            return;
        }

        string hash = ComputeSha256Hash(password);

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "SELECT IdUsuario, Rol FROM Usuarios WHERE NombreUsuario = @user AND PasswordHash = @hash";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@user", usuario);
                cmd.Parameters.AddWithValue("@hash", hash);
                conn.Open();
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Session["IdUsuario"] = reader["IdUsuario"].ToString();
                        Session["Usuario"] = usuario;
                        Session["Rol"] = reader["Rol"].ToString();
                        
                        FormsAuthentication.SetAuthCookie(usuario, false);
                        Response.Redirect("Default.aspx");
                    }
                    else
                    {
                        MostrarError("Credenciales incorrectas.");
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

    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}

