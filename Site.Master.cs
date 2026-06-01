using System;
using System.Web.Security;

public partial class Site : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Rol"] != null)
        {
            string rol = Session["Rol"].ToString();
            if (rol == "Admin" || rol == "Asesor")
            {
                menuAdmin.Visible = true;
                menuAlumno.Visible = false;
            }
            else if (rol == "Alumno")
            {
                menuAdmin.Visible = false;
                menuAlumno.Visible = true;
            }
        }
    }

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        FormsAuthentication.SignOut();
        Session.Clear();
        Session.Abandon();
        Response.Redirect("Login.aspx");
    }
}
