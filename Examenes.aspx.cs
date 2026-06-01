using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using OfficeOpenXml;

public partial class Examenes : System.Web.UI.Page
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
            CargarMaterias();
            CargarExamenes();
        }
    }

    private void CargarMaterias()
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT IdMateria, NombreMateria FROM Materias", conn))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlMateria.DataSource = dt;
                    ddlMateria.DataTextField = "NombreMateria";
                    ddlMateria.DataValueField = "IdMateria";
                    ddlMateria.DataBind();
                }
            }
        }
    }

    private void CargarExamenes()
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = @"SELECT e.IdExamen, e.Titulo, e.Activo, m.NombreMateria 
                             FROM Examenes e 
                             INNER JOIN Materias m ON e.IdMateria = m.IdMateria 
                             ORDER BY e.IdExamen DESC";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvExamenes.DataSource = dt;
                    gvExamenes.DataBind();
                }
            }
        }
    }

    protected void gvExamenes_PreRender(object sender, EventArgs e)
    {
        if (gvExamenes.Rows.Count > 0)
        {
            gvExamenes.UseAccessibleHeader = true;
            gvExamenes.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btnGuardarExamen_Click(object sender, EventArgs e)
    {
        string titulo = txtTituloExamen.Text.Trim();
        int idMateria = int.Parse(ddlMateria.SelectedValue);
        bool activo = chkActivo.Checked;

        if (string.IsNullOrEmpty(titulo))
        {
            MostrarMensaje("Ingrese el título del examen.", false);
            return;
        }

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            string query = "INSERT INTO Examenes (Titulo, Activo, IdMateria) VALUES (@tit, @act, @mat)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tit", titulo);
                cmd.Parameters.AddWithValue("@act", activo);
                cmd.Parameters.AddWithValue("@mat", idMateria);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MostrarMensaje("Examen creado exitosamente.", true);
                    txtTituloExamen.Text = "";
                    CargarExamenes();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al guardar: " + ex.Message, false);
                }
            }
        }
    }

    protected void gvExamenes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "VerPreguntas")
        {
            int idExamen = Convert.ToInt32(e.CommandArgument);
            hfIdExamenSeleccionado.Value = idExamen.ToString();
            CargarPreguntas(idExamen);
            pnlPreguntas.Visible = true;
        }
        else if (e.CommandName == "EliminarExamen")
        {
            int idExamen = Convert.ToInt32(e.CommandArgument);
            using (SQLiteConnection conn = new DBContext().GetConnection())
            {
                conn.Open();
                SQLiteTransaction tx = conn.BeginTransaction();
                try
                {
                    // Eliminar Opciones de las preguntas de este examen
                    using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM OpcionesRespuestas WHERE IdPregunta IN (SELECT IdPregunta FROM Preguntas WHERE IdExamen = @id)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", idExamen);
                        cmd.ExecuteNonQuery();
                    }
                    // Eliminar Preguntas
                    using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Preguntas WHERE IdExamen = @id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", idExamen);
                        cmd.ExecuteNonQuery();
                    }
                    // Eliminar asignaciones pendientes
                    using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM ExamenesAsignados WHERE IdExamen = @id AND Realizado = 0", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", idExamen);
                        cmd.ExecuteNonQuery();
                    }
                    // Eliminar Examen (sólo funcionará si no hay Calificaciones o Asignaciones finalizadas vinculadas)
                    using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Examenes WHERE IdExamen = @id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", idExamen);
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                    MostrarMensaje("Examen y sus preguntas eliminados exitosamente.", true);
                }
                catch (SQLiteException)
                {
                    tx.Rollback();
                    // Si hubo error de llave foránea (ej. un alumno ya hizo el examen)
                    using (SQLiteCommand cmd = new SQLiteCommand("UPDATE Examenes SET Activo = 0 WHERE IdExamen = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idExamen);
                        cmd.ExecuteNonQuery();
                    }
                    MostrarMensaje("El examen ya tiene calificaciones asociadas. Se ha cambiado su estado a Inactivo para no afectar el historial.", true);
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    MostrarMensaje("Error al eliminar: " + ex.Message, false);
                }
            }
            CargarExamenes();
            pnlPreguntas.Visible = false;
        }
    }

    private void CargarPreguntas(int idExamen)
    {
        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            // Título
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT Titulo FROM Examenes WHERE IdExamen = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", idExamen);
                conn.Open();
                object result = cmd.ExecuteScalar();
                lblExamenSeleccionado.Text = result != null ? result.ToString() : string.Empty;
                conn.Close();
            }

            // Preguntas
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT IdPregunta, TextoPregunta FROM Preguntas WHERE IdExamen = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", idExamen);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        rptPreguntas.DataSource = dt;
                        rptPreguntas.DataBind();
                        lblSinPreguntas.Visible = false;
                        rptPreguntas.Visible = true;
                    }
                    else
                    {
                        rptPreguntas.Visible = false;
                        lblSinPreguntas.Visible = true;
                    }
                }
            }
        }
    }

    protected void rptPreguntas_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Repeater rptOpciones = (Repeater)e.Item.FindControl("rptOpciones");
            int idPregunta = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "IdPregunta"));

            using (SQLiteConnection conn = new DBContext().GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT TextoOpcion, EsCorrecta FROM OpcionesRespuestas WHERE IdPregunta = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", idPregunta);
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        rptOpciones.DataSource = dt;
                        rptOpciones.DataBind();
                    }
                }
            }
        }
    }

    protected void btnGuardarPregunta_Click(object sender, EventArgs e)
    {
        int idExamen = 0;
        if (!int.TryParse(hfIdExamenSeleccionado.Value, out idExamen))
        {
            MostrarMensaje("Debe seleccionar un examen primero.", false);
            return;
        }

        string texto = txtTextoPregunta.Text.Trim();
        string op1 = txtOpcion1.Text.Trim();
        string op2 = txtOpcion2.Text.Trim();
        string op3 = txtOpcion3.Text.Trim();
        string optCorrecta = Request.Form["optCorrecta"];

        if (string.IsNullOrEmpty(texto) || string.IsNullOrEmpty(op1) || string.IsNullOrEmpty(op2) || string.IsNullOrEmpty(op3) || string.IsNullOrEmpty(optCorrecta))
        {
            MostrarMensaje("Complete todos los campos de la pregunta y opciones.", false);
            return;
        }

        using (SQLiteConnection conn = new DBContext().GetConnection())
        {
            conn.Open();
            SQLiteTransaction tx = conn.BeginTransaction();
            try
            {
                // Insertar pregunta
                string qPregunta = "INSERT INTO Preguntas (TextoPregunta, IdExamen) VALUES (@txt, @id); SELECT last_insert_rowid();";
                int idPregunta;
                using (SQLiteCommand cmd = new SQLiteCommand(qPregunta, conn, tx))
                {
                    cmd.Parameters.AddWithValue("@txt", texto);
                    cmd.Parameters.AddWithValue("@id", idExamen);
                    idPregunta = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Insertar opciones
                string qOpcion = "INSERT INTO OpcionesRespuestas (TextoOpcion, EsCorrecta, IdPregunta) VALUES (@txt, @corr, @idPreg)";
                using (SQLiteCommand cmd = new SQLiteCommand(qOpcion, conn, tx))
                {
                    // Opción 1
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@txt", op1);
                    cmd.Parameters.AddWithValue("@corr", optCorrecta == "1");
                    cmd.Parameters.AddWithValue("@idPreg", idPregunta);
                    cmd.ExecuteNonQuery();

                    // Opción 2
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@txt", op2);
                    cmd.Parameters.AddWithValue("@corr", optCorrecta == "2");
                    cmd.Parameters.AddWithValue("@idPreg", idPregunta);
                    cmd.ExecuteNonQuery();

                    // Opción 3
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@txt", op3);
                    cmd.Parameters.AddWithValue("@corr", optCorrecta == "3");
                    cmd.Parameters.AddWithValue("@idPreg", idPregunta);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                MostrarMensaje("Pregunta agregada exitosamente.", true);
                
                // Limpiar modal
                txtTextoPregunta.Text = "";
                txtOpcion1.Text = "";
                txtOpcion2.Text = "";
                txtOpcion3.Text = "";
                
                CargarPreguntas(idExamen);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                MostrarMensaje("Error al guardar: " + ex.Message, false);
            }
        }
    }

    private void MostrarMensaje(string msg, bool exito)
    {
        pnlMensaje.Visible = true;
        lblMensaje.Text = msg;
        pnlMensaje.CssClass = exito ? "alert alert-success alert-dismissible fade show mt-3" : "alert alert-danger alert-dismissible fade show mt-3";
    }

    protected void btnImportarCSV_Click(object sender, EventArgs e)
    {
        int idExamen = 0;
        if (!int.TryParse(hfIdExamenSeleccionado.Value, out idExamen))
        {
            MostrarMensaje("Debe seleccionar un examen primero.", false);
            return;
        }

        if (!fuPreguntasCSV.HasFile)
        {
            MostrarMensaje("Seleccione un archivo CSV o Excel (.xlsx) para importar.", false);
            return;
        }

        string ext = Path.GetExtension(fuPreguntasCSV.PostedFile.FileName).ToLower();
        if (ext != ".csv" && ext != ".xlsx")
        {
            MostrarMensaje("Formato no soportado. Por favor suba un archivo .csv o .xlsx", false);
            return;
        }

        try
        {
            int agregadas = 0;
            using (SQLiteConnection conn = new DBContext().GetConnection())
            {
                conn.Open();
                
                if (ext == ".csv")
                {
                    using (StreamReader sr = new StreamReader(fuPreguntasCSV.PostedFile.InputStream, System.Text.Encoding.GetEncoding("iso-8859-1")))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            string[] data = line.Split(',');
                            if (data.Length >= 4)
                            {
                                string pregunta = data[0].Trim();
                                string optCorr = data[1].Trim();
                                string opt2 = data[2].Trim();
                                string opt3 = data[3].Trim();

                                GuardarPreguntaBD(conn, idExamen, pregunta, optCorr, opt2, opt3);
                                agregadas++;
                            }
                        }
                    }
                }
                else if (ext == ".xlsx")
                {
                    using (ExcelPackage package = new ExcelPackage(fuPreguntasCSV.PostedFile.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        int rowCount = worksheet.Dimension.Rows;
                        for (int row = 1; row <= rowCount; row++)
                        {
                            string pregunta = worksheet.Cells[row, 1].Value != null ? worksheet.Cells[row, 1].Value.ToString().Trim() : "";
                            string optCorr = worksheet.Cells[row, 2].Value != null ? worksheet.Cells[row, 2].Value.ToString().Trim() : "";
                            string opt2 = worksheet.Cells[row, 3].Value != null ? worksheet.Cells[row, 3].Value.ToString().Trim() : "";
                            string opt3 = worksheet.Cells[row, 4].Value != null ? worksheet.Cells[row, 4].Value.ToString().Trim() : "";

                            if (!string.IsNullOrWhiteSpace(pregunta) && !string.IsNullOrWhiteSpace(optCorr) && !string.IsNullOrWhiteSpace(opt2) && !string.IsNullOrWhiteSpace(opt3))
                            {
                                GuardarPreguntaBD(conn, idExamen, pregunta, optCorr, opt2, opt3);
                                agregadas++;
                            }
                        }
                    }
                }
                
                MostrarMensaje(string.Format("Se importaron {0} preguntas correctamente.", agregadas), true);
                CargarPreguntas(idExamen);
            }
        }
        catch (Exception ex)
        {
            MostrarMensaje("Error al importar: " + ex.Message, false);
        }
    }

    private void GuardarPreguntaBD(SQLiteConnection conn, int idExamen, string pregunta, string optCorr, string opt2, string opt3)
    {
        SQLiteTransaction tx = conn.BeginTransaction();
        try
        {
            string qPreg = "INSERT INTO Preguntas (TextoPregunta, IdExamen) VALUES (@txt, @id); SELECT last_insert_rowid();";
            int idPreg;
            using (SQLiteCommand cmd = new SQLiteCommand(qPreg, conn, tx))
            {
                cmd.Parameters.AddWithValue("@txt", pregunta);
                cmd.Parameters.AddWithValue("@id", idExamen);
                idPreg = Convert.ToInt32(cmd.ExecuteScalar());
            }

            string qOpcion = "INSERT INTO OpcionesRespuestas (TextoOpcion, EsCorrecta, IdPregunta) VALUES (@txt, @corr, @idPreg)";
            using (SQLiteCommand cmd = new SQLiteCommand(qOpcion, conn, tx))
            {
                // Correcta
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@txt", optCorr);
                cmd.Parameters.AddWithValue("@corr", true);
                cmd.Parameters.AddWithValue("@idPreg", idPreg);
                cmd.ExecuteNonQuery();

                // Falsa 1
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@txt", opt2);
                cmd.Parameters.AddWithValue("@corr", false);
                cmd.Parameters.AddWithValue("@idPreg", idPreg);
                cmd.ExecuteNonQuery();

                // Falsa 2
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@txt", opt3);
                cmd.Parameters.AddWithValue("@corr", false);
                cmd.Parameters.AddWithValue("@idPreg", idPreg);
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}

