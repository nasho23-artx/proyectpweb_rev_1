<%@ Page Title="Gestión de Materias" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Materias.aspx.cs" Inherits="Materias" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold m-0"><i class="bi bi-journal-text text-primary me-2"></i>Gestión de Materias</h2>
        <button type="button" class="btn btn-primary rounded-pill px-4 shadow-sm" data-bs-toggle="modal" data-bs-target="#modalAgregar">
            <i class="bi bi-plus-lg me-1"></i> Agregar Materia
        </button>
    </div>

    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-success alert-dismissible fade show">
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </asp:Panel>

    <div class="card shadow-sm border-0">
        <div class="card-body">
            <div class="table-responsive">
                <asp:GridView ID="gvMaterias" runat="server" AutoGenerateColumns="False" DataKeyNames="IdMateria"
                    CssClass="table table-hover table-striped dt-responsive nowrap w-100 mb-0" ClientIDMode="Static"
                    OnRowEditing="gvMaterias_RowEditing" OnRowCancelingEdit="gvMaterias_RowCancelingEdit"
                    OnRowUpdating="gvMaterias_RowUpdating" OnRowDeleting="gvMaterias_RowDeleting"
                    OnPreRender="gvMaterias_PreRender">
                    <Columns>
                        <asp:BoundField DataField="IdMateria" HeaderText="ID" ReadOnly="True" />
                        <asp:TemplateField HeaderText="Nombre de Materia">
                            <ItemTemplate>
                                <%# Eval("NombreMateria") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditNombre" runat="server" CssClass="form-control form-control-sm" Text='<%# Bind("NombreMateria") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nivel Educativo">
                            <ItemTemplate>
                                <span class="badge bg-info text-dark"><%# Eval("NombreNivel") %></span>
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
                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar" OnClientClick="return confirm('¿Seguro que desea eliminar esta materia?');" CausesValidation="false"><i class="bi bi-trash"></i></asp:LinkButton>
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

    <!-- Modal Agregar -->
    <div class="modal fade" id="modalAgregar" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow">
                <div class="modal-header bg-primary text-white">
                    <h5 class="modal-title"><i class="bi bi-journal-plus me-2"></i>Nueva Materia</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <div class="form-floating mb-3">
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Nombre de la Materia"></asp:TextBox>
                        <label>Nombre de la Materia</label>
                    </div>
                    <div class="form-floating mb-3">
                        <asp:DropDownList ID="ddlNivel" runat="server" CssClass="form-select"></asp:DropDownList>
                        <label>Nivel Educativo</label>
                    </div>
                </div>
                <div class="modal-footer border-0">
                    <button type="button" class="btn btn-secondary rounded-pill" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar Materia" CssClass="btn btn-primary rounded-pill px-4" OnClick="btnGuardar_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            if ($('#gvMaterias').length > 0 && $('#gvMaterias tbody tr').length > 0 && !$('#gvMaterias tbody tr td').hasClass('dataTables_empty')) {
                $('#gvMaterias').DataTable({
                    "language": { "url": "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
                    "pageLength": 10,
                    "ordering": true,
                    "responsive": true
                });
            }
        });
    </script>
</asp:Content>
