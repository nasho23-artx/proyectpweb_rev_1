<%@ Page Title="Gestión de Alumnos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Alumnos.aspx.cs" Inherits="Alumnos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold m-0"><i class="bi bi-people text-primary me-2"></i>Gestión de Alumnos</h2>
        <button type="button" class="btn btn-primary rounded-pill px-4 shadow-sm" data-bs-toggle="modal" data-bs-target="#modalAgregar">
            <i class="bi bi-person-plus me-1"></i> Agregar Alumno
        </button>
    </div>

    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-success alert-dismissible fade show">
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </asp:Panel>

    <div class="row">
        <div class="col-lg-8 mb-4">
            <div class="card shadow-sm border-0 h-100">
        <div class="card-body">
            <div class="table-responsive">
                <asp:GridView ID="gvAlumnos" runat="server" AutoGenerateColumns="False" DataKeyNames="IdAlumno"
                    CssClass="table table-hover table-striped dt-responsive nowrap w-100 mb-0" ClientIDMode="Static"
                    OnRowEditing="gvAlumnos_RowEditing" OnRowCancelingEdit="gvAlumnos_RowCancelingEdit"
                    OnRowUpdating="gvAlumnos_RowUpdating" OnRowDeleting="gvAlumnos_RowDeleting"
                    OnPreRender="gvAlumnos_PreRender">
                    <Columns>
                        <asp:BoundField DataField="IdAlumno" HeaderText="ID" ReadOnly="True" />
                        <asp:TemplateField HeaderText="Nombre Completo">
                            <ItemTemplate>
                                <%# Eval("NombreCompleto") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditNombre" runat="server" CssClass="form-control form-control-sm" Text='<%# Bind("NombreCompleto") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CURP">
                            <ItemTemplate>
                                <span class="badge bg-secondary"><%# Eval("CURP") %></span>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditCURP" runat="server" CssClass="form-control form-control-sm text-uppercase" MaxLength="18" Text='<%# Bind("CURP") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="FechaRegistro" HeaderText="Fecha Registro" ReadOnly="True" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:TemplateField HeaderText="Nivel">
                            <ItemTemplate>
                                <%# Eval("NombreNivel") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlEditNivel" runat="server" CssClass="form-select form-select-sm">
                                </asp:DropDownList>
                                <asp:HiddenField ID="hfIdNivel" runat="server" Value='<%# Eval("IdNivel") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CssClass="btn btn-sm btn-outline-primary" ToolTip="Editar" CausesValidation="false"><i class="bi bi-pencil"></i></asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar" OnClientClick="return confirm('¿Seguro que desea eliminar este alumno?');" CausesValidation="false"><i class="bi bi-trash"></i></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn btn-sm btn-success" ToolTip="Guardar"><i class="bi bi-check-lg"></i></asp:LinkButton>
                                <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn btn-sm btn-secondary" ToolTip="Cancelar" CausesValidation="false"><i class="bi bi-x-lg"></i></asp:LinkButton>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
            </div>
        </div>
        <div class="col-lg-4 mb-4">
            <div class="card shadow-sm border-0 h-100">
                <div class="card-header bg-white border-0 pt-4 pb-0">
                    <h5 class="fw-bold"><i class="bi bi-geo-alt-fill text-danger me-2"></i>Ubicación INEA</h5>
                </div>
                <div class="card-body d-flex flex-column">
                    <div class="flex-grow-1" style="min-height: 250px;">
                        <iframe src="https://maps.google.com/maps?q=22.1444199,-100.9900483&t=&z=17&ie=UTF8&iwloc=&output=embed" width="100%" height="100%" frameborder="0" style="border:0; border-radius: 8px;" allowfullscreen></iframe>
                    </div>
                    <a href="https://maps.app.goo.gl/zGB8Tbu9V96V2jpSA" target="_blank" class="btn btn-outline-primary w-100 mt-3 rounded-pill">
                        <i class="bi bi-map me-1"></i> Abrir en Google Maps
                    </a>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Agregar -->
    <div class="modal fade" id="modalAgregar" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title"><i class="bi bi-person-plus me-2"></i>Nuevo Alumno</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <div class="form-floating mb-3">
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Nombre"></asp:TextBox>
                        <label>Nombre Completo</label>
                    </div>
                    <div class="form-floating mb-3">
                        <asp:TextBox ID="txtCURP" runat="server" CssClass="form-control text-uppercase" placeholder="CURP" MaxLength="18"></asp:TextBox>
                        <label>CURP (18 caracteres)</label>
                    </div>
                    <div class="form-floating mb-3">
                        <asp:DropDownList ID="ddlNivel" runat="server" CssClass="form-select"></asp:DropDownList>
                        <label>Nivel Educativo</label>
                    </div>
                </div>
                <div class="modal-footer border-0">
                    <button type="button" class="btn btn-secondary rounded-pill" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar Alumno" CssClass="btn btn-primary rounded-pill px-4" OnClick="btnGuardar_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            if ($('#gvAlumnos').length > 0 && $('#gvAlumnos tbody tr').length > 0 && !$('#gvAlumnos tbody tr td').hasClass('dataTables_empty')) {
                $('#gvAlumnos').DataTable({
                    "language": { "url": "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
                    "pageLength": 10,
                    "ordering": true,
                    "responsive": true
                });
            }
        });
    </script>
</asp:Content>
