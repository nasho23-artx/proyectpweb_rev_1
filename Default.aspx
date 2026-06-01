<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Vista Admin/Asesor -->
    <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
        <div class="row mb-4">
            <div class="col-12">
                <h2 class="fw-bold">Panel de Control</h2>
                <p class="text-muted">Bienvenido al sistema de evaluación digital INEA.</p>
            </div>
        </div>
        <div class="row g-4 mb-4">
            <div class="col-md-3">
                <div class="card card-hover bg-primary text-white h-100">
                    <div class="card-body d-flex align-items-center justify-content-between">
                        <div>
                            <h6 class="card-title text-uppercase fw-semibold mb-1">Total Alumnos</h6>
                            <h2 class="display-5 fw-bold mb-0"><asp:Label ID="lblTotalAlumnos" runat="server">0</asp:Label></h2>
                        </div>
                        <i class="bi bi-people display-4 opacity-50"></i>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card card-hover bg-success text-white h-100">
                    <div class="card-body d-flex align-items-center justify-content-between">
                        <div>
                            <h6 class="card-title text-uppercase fw-semibold mb-1">Exámenes Activos</h6>
                            <h2 class="display-5 fw-bold mb-0"><asp:Label ID="lblExamenesActivos" runat="server">0</asp:Label></h2>
                        </div>
                        <i class="bi bi-journal-check display-4 opacity-50"></i>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card card-hover bg-warning text-dark h-100">
                    <div class="card-body d-flex align-items-center justify-content-between">
                        <div>
                            <h6 class="card-title text-uppercase fw-semibold mb-1">Asignaciones Pendientes</h6>
                            <h2 class="display-5 fw-bold mb-0"><asp:Label ID="lblAsignaciones" runat="server">0</asp:Label></h2>
                        </div>
                        <i class="bi bi-calendar-event display-4 opacity-50"></i>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card card-hover bg-info text-white h-100">
                    <div class="card-body d-flex align-items-center justify-content-between">
                        <div>
                            <h6 class="card-title text-uppercase fw-semibold mb-1">Promedio General</h6>
                            <h2 class="display-5 fw-bold mb-0"><asp:Label ID="lblPromedio" runat="server">0.0</asp:Label></h2>
                        </div>
                        <i class="bi bi-graph-up display-4 opacity-50"></i>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Vista Alumno -->
    <asp:Panel ID="pnlAlumno" runat="server" Visible="false">
        <div class="row mb-4">
            <div class="col-12 text-center">
                <h2 class="fw-bold">Hola, <asp:Label ID="lblNombreAlumno" runat="server"></asp:Label></h2>
                <p class="text-muted fs-5">Este es tu panel de evaluaciones.</p>
            </div>
        </div>
        
        <div class="row mb-5">
            <div class="col-lg-8">
                <h4 class="fw-bold mb-3"><i class="bi bi-exclamation-circle text-warning me-2"></i>Mis Exámenes Pendientes</h4>
                <div class="row g-4">
                    <asp:Repeater ID="rptExamenesPendientes" runat="server">
                        <ItemTemplate>
                            <div class="col-md-6">
                                <div class="card card-hover h-100 border-start border-4 border-primary">
                                    <div class="card-body">
                                        <h5 class="card-title fw-bold text-primary"><%# Eval("TituloExamen") %></h5>
                                        <p class="card-text text-muted small"><i class="bi bi-calendar me-1"></i> Asignado: <%# Eval("FechaAsignacion", "{0:dd/MM/yyyy}") %></p>
                                        <a href="AplicacionExamen.aspx?id=<%# Eval("IdAsignacion") %>" class="btn btn-primary w-100 mt-2 rounded-pill">
                                            <i class="bi bi-play-circle me-1"></i> Iniciar Examen
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinExamenes" runat="server" Visible="false" CssClass="col-12 text-center text-muted">
                        <div class="p-5 bg-white rounded-3 shadow-sm">
                            <i class="bi bi-check-circle text-success" style="font-size: 3rem;"></i>
                            <h5 class="mt-3">No tienes exámenes pendientes.</h5>
                            <p>¡Buen trabajo! Has completado todas tus evaluaciones.</p>
                        </div>
                    </asp:Label>
                </div>
            </div>

            <div class="col-lg-4 mt-4 mt-lg-0">
                <h4 class="fw-bold mb-3"><i class="bi bi-geo-alt-fill text-danger me-2"></i>Ayuda y Ubicación</h4>
                <div class="card shadow-sm border-0 h-100">
                    <div class="card-body d-flex flex-column text-center">
                        <p class="mb-3 text-muted">Si tienes alguna duda con tus exámenes, puedes ir a nuestra ubicación o llamar al <br><a href="tel:8002024250" class="fw-bold fs-5 text-decoration-none">800 202 4250</a></p>
                        <div class="flex-grow-1" style="min-height: 200px;">
                            <iframe src="https://maps.google.com/maps?q=22.1444199,-100.9900483&t=&z=17&ie=UTF8&iwloc=&output=embed" width="100%" height="100%" frameborder="0" style="border:0; border-radius: 8px;" allowfullscreen></iframe>
                        </div>
                        <a href="https://maps.app.goo.gl/zGB8Tbu9V96V2jpSA" target="_blank" class="btn btn-outline-primary w-100 mt-3 rounded-pill">
                            <i class="bi bi-map me-1"></i> Abrir en Google Maps
                        </a>
                    </div>
                </div>
            </div>
        </div>

        <h4 class="fw-bold mb-3"><i class="bi bi-clock-history text-secondary me-2"></i>Mi Historial de Calificaciones</h4>
        <div class="card shadow-sm border-0">
            <div class="card-body p-0">
                <div class="table-responsive">
                    <table id="tblHistorial" class="table table-hover table-striped dt-responsive nowrap w-100 mb-0">
                        <thead class="table-light">
                            <tr>
                                <th>Examen</th>
                                <th>Fecha Aplicación</th>
                                <th>Calificación</th>
                                <th>Estado</th>
                                <th>Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptHistorial" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="fw-semibold"><%# Eval("TituloExamen") %></td>
                                        <td><%# Eval("FechaAplicacion", "{0:dd/MM/yyyy HH:mm}") %></td>
                                        <td><span class="fw-bold"><%# Eval("Calificacion", "{0:F2}") %></span> / 100</td>
                                        <td>
                                            <%# Convert.ToDecimal(Eval("Calificacion")) >= 70 
                                                ? "<span class='badge rounded-pill bg-success-subtle text-success'>Aprobado</span>" 
                                                : "<span class='badge rounded-pill bg-danger-subtle text-danger'>Reprobado</span>" %>
                                        </td>
                                        <td>
                                            <a href="ResultadosExamen.aspx?id=<%# Eval("IdCalificacion") %>" class="btn btn-sm btn-outline-info rounded-pill" title="Revisar Respuestas">
                                                <i class="bi bi-eye"></i> Revisar
                                            </a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr id="trSinHistorial" runat="server" visible="false">
                                <td colspan="5" class="text-center text-muted p-4">No hay calificaciones registradas.</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        $(document).ready(function () {
            if ($('#tblHistorial').length > 0 && $('#tblHistorial tbody tr').length > 0 && !$('#tblHistorial tbody tr td').hasClass('dataTables_empty') && !$('#tblHistorial tbody tr').attr('id')) {
                $('#tblHistorial').DataTable({
                    "language": { "url": "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json" },
                    "pageLength": 10,
                    "ordering": true,
                    "responsive": true,
                    "lengthChange": false,
                    "searching": false
                });
            }
        });
    </script>
</asp:Content>
