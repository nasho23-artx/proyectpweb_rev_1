<%@ Page Title="Resultados del Examen" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="ResultadosExamen.aspx.cs" Inherits="ResultadosExamen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold m-0"><i class="bi bi-clipboard-check text-primary me-2"></i>Resultados del Examen</h2>
        <button type="button" class="btn btn-danger rounded-pill px-4 shadow-sm" onclick="descargarPDF()">
            <i class="bi bi-file-earmark-pdf me-1"></i> Descargar PDF
        </button>
    </div>

    <div id="pdfContent" class="p-3 bg-white shadow-sm rounded">
        <div class="text-center mb-4">
            <h3 class="fw-bold"><asp:Label ID="lblTituloExamen" runat="server"></asp:Label></h3>
            <h5 class="text-muted"><asp:Label ID="lblAlumno" runat="server"></asp:Label></h5>
            <div class="mt-3">
                <span class="fs-4">Calificación Final: </span>
                <asp:Label ID="lblCalificacion" runat="server" CssClass="fs-3 fw-bold"></asp:Label>
            </div>
        </div>

        <hr />
        <h5 class="fw-bold mb-3">Detalle de Respuestas:</h5>
        
        <asp:Repeater ID="rptRespuestas" runat="server">
            <ItemTemplate>
                <div class="card mb-3 border-0 shadow-sm <%# Convert.ToBoolean(Eval("EsCorrecta")) ? "border-start border-success border-4" : "border-start border-danger border-4" %>">
                    <div class="card-body">
                        <h6 class="fw-bold"><%# Container.ItemIndex + 1 %>. <%# Eval("TextoPregunta") %></h6>
                        <div class="row mt-2">
                            <div class="col-md-6">
                                <p class="mb-1 text-muted small">Tu respuesta:</p>
                                <p class="mb-0 <%# Convert.ToBoolean(Eval("EsCorrecta")) ? "text-success fw-bold" : "text-danger text-decoration-line-through" %>">
                                    <i class="bi <%# Convert.ToBoolean(Eval("EsCorrecta")) ? "bi-check-circle" : "bi-x-circle" %> me-1"></i>
                                    <%# Eval("OpcionSeleccionada") %>
                                </p>
                            </div>
                            <div class="col-md-6" style='display: <%# Convert.ToBoolean(Eval("EsCorrecta")) ? "none" : "block" %>;'>
                                <p class="mb-1 text-muted small">Respuesta Correcta:</p>
                                <p class="mb-0 text-success fw-bold">
                                    <i class="bi bi-check-circle-fill me-1"></i> <%# Eval("OpcionCorrecta") %>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function descargarPDF() {
            var element = document.getElementById('pdfContent');
            var opt = {
                margin:       0.5,
                filename:     'Resultados_Examen.pdf',
                image:        { type: 'jpeg', quality: 0.98 },
                html2canvas:  { scale: 2 },
                jsPDF:        { unit: 'in', format: 'letter', orientation: 'portrait' }
            };
            
            html2pdf().set(opt).from(element).save();
        }
    </script>
</asp:Content>
