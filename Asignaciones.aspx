<%@ Page Title="Asignar Exámenes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Asignaciones.aspx.cs" Inherits="Asignaciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold m-0"><i class="bi bi-calendar-check text-primary me-2"></i>Asignar Exámenes</h2>
    </div>

    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-success alert-dismissible fade show">
        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </asp:Panel>

    <div class="row g-4 mb-5">
        <!-- Seleccionar Alumno y Examen -->
        <div class="col-md-4">
            <div class="card shadow-sm border-0 h-100">
                <div class="card-header bg-white border-bottom-0 pt-4 pb-0">
                    <h5 class="fw-bold"><i class="bi bi-plus-circle-dotted me-2"></i>Nueva Asignación</h5>
                </div>
                <div class="card-body">
                    <div class="form-floating mb-3">
                        <asp:DropDownList ID="ddlAlumno" runat="server" CssClass="form-select"></asp:DropDownList>
                        <label>Seleccionar Alumno</label>
                    </div>
                    <div class="form-floating mb-4">
                        <asp:DropDownList ID="ddlExamen" runat="server" CssClass="form-select"></asp:DropDownList>
                        <label>Seleccionar Examen</label>
                    </div>
                    <asp:Button ID="btnAsignar" runat="server" Text="Asignar Examen" CssClass="btn btn-primary w-100 rounded-pill py-2 fw-semibold" OnClick="btnAsignar_Click" />
                </div>
            </div>
        </div>

        <!-- Lista de Asignaciones -->
        <div class="col-md-8">
            <div class="card shadow-sm border-0 h-100">
                <div class="card-header bg-white border-bottom-0 pt-4 pb-0">
                    <h5 class="fw-bold"><i class="bi bi-list-check me-2"></i>Asignaciones Actuales</h5>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <asp:GridView ID="gvAsignaciones" runat="server" AutoGenerateColumns="False" DataKeyNames="IdAsignacion"
                            CssClass="table table-hover table-striped dt-responsive nowrap w-100 mb-0" ClientIDMode="Static"
                            OnRowDeleting="gvAsignaciones_RowDeleting" OnPreRender="gvAsignaciones_PreRender">
                            <Columns>
                                <asp:BoundField DataField="IdAsignacion" HeaderText="ID" />
                                <asp:BoundField DataField="NombreCompleto" HeaderText="Alumno" />
                                <asp:BoundField DataField="CURP" HeaderText="CURP" />
                                <asp:BoundField DataField="Titulo" HeaderText="Examen" />
                                <asp:BoundField DataField="FechaAsignacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <%# Convert.ToBoolean(Eval("Realizado")) ? "<span class='badge bg-success rounded-pill'>Realizado</span>" : "<span class='badge bg-warning text-dark rounded-pill'>Pendiente</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="btn btn-sm btn-outline-danger" ToolTip="Eliminar Asignación" OnClientClick="return confirm('¿Seguro que desea eliminar esta asignación?');" Visible='<%# !Convert.ToBoolean(Eval("Realizado")) %>'><i class="bi bi-trash"></i></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            if ($('#gvAsignaciones').length > 0 && $('#gvAsignaciones tbody tr').length > 0 && !$('#gvAsignaciones tbody tr td').hasClass('dataTables_empty')) {
                $('#gvAsignaciones').DataTable({
                    "language": { "url": "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
                    "pageLength": 10,
                    "ordering": true,
                    "responsive": true
                });
            }
        });
    </script>
</asp:Content>
