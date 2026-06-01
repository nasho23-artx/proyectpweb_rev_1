using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI.WebControls;

public partial class Reportes : System.Web.UI.Page
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
            CargarReportes();
        }
    }

    private void CargarReportes()
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = @"
                SELECT c.IdCalificacion, c.Calificacion, c.FechaAplicacion, 
                       a.NombreCompleto, a.CURP, 
                       e.Titulo, m.NombreMateria, n.NombreNivel
                FROM Calificaciones c
                INNER JOIN Alumnos a ON c.IdAlumno = a.IdAlumno
                INNER JOIN Examenes e ON c.IdExamen = e.IdExamen
                INNER JOIN Materias m ON e.IdMateria = m.IdMateria
                INNER JOIN NivelesEducativos n ON m.IdNivel = n.IdNivel
                ORDER BY c.FechaAplicacion DESC";

            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    
                    gvReportes.DataSource = dt;
                    gvReportes.DataBind();

                    // Calcular Resumen
                    int total = dt.Rows.Count;
                    int aprobados = 0;
                    int reprobados = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (Convert.ToDecimal(row["Calificacion"]) >= 70)
                            aprobados++;
                        else
                            reprobados++;
                    }

                    lblTotalEvaluaciones.Text = total.ToString();
                    lblAprobados.Text = aprobados.ToString();
                    lblReprobados.Text = reprobados.ToString();
                }
            }
        }
    }

    protected void gvReportes_PreRender(object sender, EventArgs e)
    {
        if (gvReportes.Rows.Count > 0)
        {
            gvReportes.UseAccessibleHeader = true;
            gvReportes.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}

