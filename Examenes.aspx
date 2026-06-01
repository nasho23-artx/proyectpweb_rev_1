<%@ Page Title="Banco de Exámenes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Examenes.aspx.cs" Inherits="Examenes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .pregunta-card { border-left: 4px solid #0d6efd; margin-bottom: 15px; }
        .opcion-correcta { background-color: #d1e7dd; border-color: #badbcc; color: #0f5132; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold m-0"><i class="bi bi-card-checklist text-primary me-2"></i>Banco de Exámenes</h2>
        <button type="button" class="btn btn-primary rounded-pill px-4 shadow-sm" data-bs-toggle="modal" data-bs-target="#modalAgregarExamen">
            <i class="bi bi-plus-lg me-1"></i> Crear Examen
        </button>
    </div>

    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-success alert-dismissible fade show">
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </asp:Panel>

    <div class="row">
        <!-- Lista de Exámenes -->
        <div class="col-md-5">
            <div class="card shadow-sm border-0">
                <div class="card-header bg-white border-bottom-0 pt-4 pb-0">
                    <h5 class="fw-bold"><i class="bi bi-list-ul me-2"></i>Exámenes Disponibles</h5>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <asp:GridView ID="gvExamenes" runat="server" AutoGenerateColumns="False" DataKeyNames="IdExamen"
                            CssClass="table table-hover table-striped dt-responsive nowrap w-100 mb-0" ClientIDMode="Static"
                            OnRowCommand="gvExamenes_RowCommand" OnPreRender="gvExamenes_PreRender">
                            <Columns>
                                <asp:BoundField DataField="IdExamen" HeaderText="ID" />
                                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                                <asp:BoundField DataField="NombreMateria" HeaderText="Materia" />
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <%# Convert.ToBoolean(Eval("Activo")) ? "<span class='badge bg-success rounded-pill'>Activo</span>" : "<span class='badge bg-danger rounded-pill'>Inactivo</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnVer" runat="server" CommandName="VerPreguntas" CommandArgument='<%# Eval("IdExamen") %>' CssClass="btn btn-sm btn-outline-info rounded-circle me-1" ToolTip="Ver Preguntas"><i class="bi bi-eye"></i></asp:LinkButton>
                                        <asp:LinkButton ID="btnEliminar" runat="server" CommandName="EliminarExamen" CommandArgument='<%# Eval("IdExamen") %>' CssClass="btn btn-sm btn-outline-danger rounded-circle" OnClientClick="return confirm('¿Está seguro que desea eliminar este examen de forma permanente?');" ToolTip="Eliminar"><i class="bi bi-trash"></i></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>

        <!-- Panel de Preguntas -->
        <div class="col-md-7">
            <asp:Panel ID="pnlPreguntas" runat="server" Visible="false">
                <div class="card shadow-sm border-0 h-100">
                    <div class="card-header bg-white border-bottom-0 pt-4 pb-0 d-flex justify-content-between align-items-center">
                        <h5 class="fw-bold text-primary m-0">Preguntas: <asp:Label ID="lblExamenSeleccionado" runat="server"></asp:Label></h5>
                        <div class="d-flex gap-2">
                            <button type="button" class="btn btn-sm btn-outline-secondary rounded-pill px-3" data-bs-toggle="modal" data-bs-target="#modalImportarCSV">
                                <i class="bi bi-file-earmark-spreadsheet me-1"></i> Importar CSV
                            </button>
                            <button type="button" class="btn btn-sm btn-success rounded-pill px-3" data-bs-toggle="modal" data-bs-target="#modalAgregarPregunta">
                                <i class="bi bi-plus-circle me-1"></i> Agregar Pregunta
                            </button>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="accordion" id="accordionPreguntas">
                            <asp:Repeater ID="rptPreguntas" runat="server" OnItemDataBound="rptPreguntas_ItemDataBound">
                                <ItemTemplate>
                                    <div class="accordion-item border-0 mb-2 shadow-sm rounded">
                                        <h2 class="accordion-header" id="heading<%# Eval("IdPregunta") %>">
                                            <button class="accordion-button collapsed fw-semibold rounded" type="button" data-bs-toggle="collapse" data-bs-target="#collapse<%# Eval("IdPregunta") %>">
                                                <%# Container.ItemIndex + 1 %>. <%# Eval("TextoPregunta") %>
                                            </button>
                                        </h2>
                                        <div id="collapse<%# Eval("IdPregunta") %>" class="accordion-collapse collapse" data-bs-parent="#accordionPreguntas">
                                            <div class="accordion-body bg-light rounded-bottom">
                                                <ul class="list-group list-group-flush rounded">
                                                    <asp:Repeater ID="rptOpciones" runat="server">
                                                        <ItemTemplate>
                                                            <li class='list-group-item bg-transparent <%# Convert.ToBoolean(Eval("EsCorrecta")) ? "text-success fw-bold" : "" %>'>
                                                                <i class='bi <%# Convert.ToBoolean(Eval("EsCorrecta")) ? "bi-check-circle-fill" : "bi-circle" %> me-2'></i>
                                                                <%# Eval("TextoOpcion") %>
                                                            </li>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Label ID="lblSinPreguntas" runat="server" Visible="false" CssClass="d-block text-center text-muted p-4">Este examen no tiene preguntas registradas.</asp:Label>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>

    <!-- Modal Agregar Examen -->
    <div class="modal fade" id="modalAgregarExamen" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title">Nuevo Examen</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <div class="form-floating mb-3">
                        <asp:TextBox ID="txtTituloExamen" runat="server" CssClass="form-control" placeholder="Título"></asp:TextBox>
                        <label>Título del Examen</label>
                    </div>
                    <div class="form-floating mb-3">
                        <asp:DropDownList ID="ddlMateria" runat="server" CssClass="form-select"></asp:DropDownList>
                        <label>Materia</label>
                    </div>
                    <div class="form-check form-switch">
                        <asp:CheckBox ID="chkActivo" runat="server" CssClass="form-check-input" Checked="true" />
                        <label class="form-check-label">Examen Activo</label>
                    </div>
                </div>
                <div class="modal-footer border-0">
                    <button type="button" class="btn btn-secondary rounded-pill" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnGuardarExamen" runat="server" Text="Guardar Examen" CssClass="btn btn-primary rounded-pill px-4" OnClick="btnGuardarExamen_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Agregar Pregunta -->
    <div class="modal fade" id="modalAgregarPregunta" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content border-0 shadow">
                <div class="modal-header bg-success text-white">
                    <h5 class="modal-title">Nueva Pregunta</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <asp:HiddenField ID="hfIdExamenSeleccionado" runat="server" />
                    <div class="form-floating mb-4">
                        <asp:TextBox ID="txtTextoPregunta" runat="server" CssClass="form-control" TextMode="MultiLine" Height="100px" placeholder="Pregunta"></asp:TextBox>
                        <label>Texto de la Pregunta</label>
                    </div>
                    <h6 class="fw-bold mb-3">Opciones de Respuesta:</h6>
                    <div class="alert alert-info py-2 small"><i class="bi bi-info-circle me-1"></i> Seleccione el radio button de la respuesta correcta.</div>
                    
                    <div class="input-group mb-3">
                        <div class="input-group-text">
                            <input class="form-check-input mt-0" type="radio" name="optCorrecta" value="1" checked>
                        </div>
                        <asp:TextBox ID="txtOpcion1" runat="server" CssClass="form-control" placeholder="Opción 1"></asp:TextBox>
                    </div>
                    <div class="input-group mb-3">
                        <div class="input-group-text">
                            <input class="form-check-input mt-0" type="radio" name="optCorrecta" value="2">
                        </div>
                        <asp:TextBox ID="txtOpcion2" runat="server" CssClass="form-control" placeholder="Opción 2"></asp:TextBox>
                    </div>
                    <div class="input-group mb-3">
                        <div class="input-group-text">
                            <input class="form-check-input mt-0" type="radio" name="optCorrecta" value="3">
                        </div>
                        <asp:TextBox ID="txtOpcion3" runat="server" CssClass="form-control" placeholder="Opción 3"></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer border-0">
                    <button type="button" class="btn btn-secondary rounded-pill" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnGuardarPregunta" runat="server" Text="Guardar Pregunta" CssClass="btn btn-success rounded-pill px-4" OnClick="btnGuardarPregunta_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Importar CSV -->
    <div class="modal fade" id="modalImportarCSV" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow">
                <div class="modal-header bg-info text-white">
                    <h5 class="modal-title">Importar Preguntas (CSV)</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <div class="alert alert-light border small text-muted">
                        <i class="bi bi-info-circle text-primary"></i> <strong>Formato requerido:</strong><br />
                        Columna A: Pregunta<br />
                        Columna B: Opcion Correcta<br />
                        Columna C, D...: Otras Opciones<br />
                        <em>Sin encabezados. En CSV separadas por coma.</em>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Seleccionar archivo (.csv, .xlsx):</label>
                        <asp:FileUpload ID="fuPreguntasCSV" runat="server" CssClass="form-control" accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel" />
                    </div>
                </div>
                <div class="modal-footer border-0">
                    <button type="button" class="btn btn-secondary rounded-pill" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnImportarCSV" runat="server" Text="Importar" CssClass="btn btn-info text-white rounded-pill px-4" OnClick="btnImportarCSV_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            if ($('#gvExamenes').length > 0 && $('#gvExamenes tbody tr').length > 0 && !$('#gvExamenes tbody tr td').hasClass('dataTables_empty')) {
                $('#gvExamenes').DataTable({
                    "language": { "url": "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
                    "pageLength": 5,
                    "lengthChange": false,
                    "ordering": true,
                    "responsive": true
                });
            }
        });
    </script>
</asp:Content>
