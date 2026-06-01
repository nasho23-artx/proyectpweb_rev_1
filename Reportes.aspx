<%@ Page Title="Reporte de Calificaciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Reportes.aspx.cs" Inherits="Reportes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold m-0"><i class="bi bi-bar-chart text-primary me-2"></i>Reporte de Calificaciones</h2>
    </div>

    <!-- Tarjetas de Resumen -->
    <div class="row g-4 mb-4">
        <div class="col-md-4">
            <div class="card shadow-sm border-0 border-start border-5 border-primary h-100">
                <div class="card-body">
                    <h6 class="text-muted text-uppercase fw-semibold mb-1">Total Evaluaciones</h6>
                    <h2 class="fw-bold mb-0"><asp:Label ID="lblTotalEvaluaciones" runat="server">0</asp:Label></h2>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card shadow-sm border-0 border-start border-5 border-success h-100">
                <div class="card-body">
                    <h6 class="text-muted text-uppercase fw-semibold mb-1">Total Aprobados</h6>
                    <h2 class="fw-bold text-success mb-0"><asp:Label ID="lblAprobados" runat="server">0</asp:Label></h2>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card shadow-sm border-0 border-start border-5 border-danger h-100">
                <div class="card-body">
                    <h6 class="text-muted text-uppercase fw-semibold mb-1">Total Reprobados</h6>
                    <h2 class="fw-bold text-danger mb-0"><asp:Label ID="lblReprobados" runat="server">0</asp:Label></h2>
                </div>
            </div>
        </div>
    </div>

    <!-- Filtros y Tabla -->
    <div class="card shadow-sm border-0">
        <div class="card-header bg-white border-bottom-0 pt-4 pb-0 d-flex justify-content-between align-items-center">
            <h5 class="fw-bold m-0"><i class="bi bi-table me-2"></i>Historial Detallado</h5>
        </div>
        <div class="card-body">
            <div class="table-responsive">
                <asp:GridView ID="gvReportes" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-hover table-striped dt-responsive nowrap w-100 mb-0" ClientIDMode="Static"
                    OnPreRender="gvReportes_PreRender">
                    <Columns>
                        <asp:BoundField DataField="NombreCompleto" HeaderText="Alumno" />
                        <asp:BoundField DataField="CURP" HeaderText="CURP" />
                        <asp:BoundField DataField="NombreNivel" HeaderText="Nivel" />
                        <asp:BoundField DataField="NombreMateria" HeaderText="Materia" />
                        <asp:BoundField DataField="Titulo" HeaderText="Examen" />
                        <asp:BoundField DataField="FechaAplicacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                        <asp:TemplateField HeaderText="Calificación">
                            <ItemTemplate>
                                <span class="fw-bold"><%# Eval("Calificacion", "{0:F2}") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <%# Convert.ToDecimal(Eval("Calificacion")) >= 70 ? "<span class='badge bg-success rounded-pill px-3'>Aprobado</span>" : "<span class='badge bg-danger rounded-pill px-3'>Reprobado</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <!-- Buttons extension para exportar a PDF/Excel -->
    <link rel="stylesheet" href="https://cdn.datatables.net/buttons/2.4.1/css/buttons.bootstrap5.min.css">
    <script src="https://cdn.datatables.net/buttons/2.4.1/js/dataTables.buttons.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.4.1/js/buttons.bootstrap5.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.10.1/jszip.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/pdfmake.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/vfs_fonts.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.4.1/js/buttons.html5.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/2.4.1/js/buttons.print.min.js"></script>

    <script>
        $(document).ready(function () {
            if ($('#gvReportes').length > 0 && $('#gvReportes tbody tr').length > 0 && !$('#gvReportes tbody tr td').hasClass('dataTables_empty')) {
                $('#gvReportes').DataTable({
                    "language": { "url": "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
                    "pageLength": 15,
                    "ordering": true,
                    "responsive": true,
                    dom: '<"row"<"col-sm-12 col-md-6"B><"col-sm-12 col-md-6"f>>rt<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                    buttons: [
                        { extend: 'excel', className: 'btn btn-sm btn-success', text: '<i class="bi bi-file-earmark-excel"></i> Excel' },
                        { extend: 'pdf', className: 'btn btn-sm btn-danger', text: '<i class="bi bi-file-earmark-pdf"></i> PDF' },
                        { extend: 'print', className: 'btn btn-sm btn-secondary', text: '<i class="bi bi-printer"></i> Imprimir' }
                    ]
                });
            }
        });
    </script>
</asp:Content>
