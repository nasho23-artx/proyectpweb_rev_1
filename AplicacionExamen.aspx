<%@ Page Title="Examen" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AplicacionExamen.aspx.cs" Inherits="AplicacionExamen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .pregunta-texto { font-size: 1.8rem; font-weight: 600; line-height: 1.4; color: #333; }
        .opcion-label { display: block; padding: 15px 20px; margin-bottom: 10px; border: 2px solid #e9ecef; border-radius: 10px; cursor: pointer; transition: all 0.2s; font-size: 1.2rem; }
        .opcion-label:hover { border-color: #0d6efd; background-color: #f8f9fa; }
        .opcion-radio { display: none; }
        .opcion-radio:checked + .opcion-label { border-color: #0d6efd; background-color: #e7f1ff; font-weight: bold; }
        .opcion-radio:checked + .opcion-label::before { content: '\F26A'; font-family: 'Bootstrap Icons'; color: #0d6efd; float: right; }
        #panelResultados { display: none; text-align: center; }
        .progress-bar-animated { transition: width 0.5s ease; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center" id="panelExamen">
        <div class="col-md-8">
            <asp:HiddenField ID="hfIdAsignacion" runat="server" ClientIDMode="Static" />
            
            <div class="card shadow border-0 rounded-4 overflow-hidden mb-4">
                <div class="card-header bg-primary text-white p-4">
                    <h3 class="mb-0 fw-bold"><asp:Label ID="lblTituloExamen" runat="server"></asp:Label></h3>
                    <p class="mb-0 opacity-75">Materia: <asp:Label ID="lblMateria" runat="server"></asp:Label></p>
                </div>
                
                <div class="progress" style="height: 10px; border-radius: 0;">
                    <div class="progress-bar progress-bar-striped progress-bar-animated bg-success" id="progressBar" role="progressbar" style="width: 0%;" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"></div>
                </div>

                <div class="card-body p-5">
                    <div id="contenedorPreguntas">
                        <!-- Preguntas cargadas por JS -->
                    </div>
                    
                    <div class="d-flex justify-content-between mt-5">
                        <button type="button" class="btn btn-outline-secondary rounded-pill px-4" id="btnAnterior" style="display: none;">
                            <i class="bi bi-arrow-left me-1"></i> Anterior
                        </button>
                        <button type="button" class="btn btn-primary rounded-pill px-5 fw-bold ms-auto" id="btnSiguiente">
                            Siguiente <i class="bi bi-arrow-right ms-1"></i>
                        </button>
                        <button type="button" class="btn btn-success rounded-pill px-5 fw-bold ms-auto" id="btnFinalizar" style="display: none;">
                            <i class="bi bi-check-circle me-1"></i> Finalizar Examen
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        let preguntas = [];
        let indexActual = 0;
        let respuestas = {};

        $(document).ready(function () {
            cargarPreguntas();

            $('#btnSiguiente').click(function () {
                guardarRespuestaLocal();
                if (indexActual < preguntas.length - 1) {
                    indexActual++;
                    mostrarPregunta(indexActual);
                }
            });

            $('#btnAnterior').click(function () {
                guardarRespuestaLocal();
                if (indexActual > 0) {
                    indexActual--;
                    mostrarPregunta(indexActual);
                }
            });

            $('#btnFinalizar').click(function () {
                guardarRespuestaLocal();
                if (Object.keys(respuestas).length < preguntas.length) {
                    Swal.fire('Atención', 'Por favor responde todas las preguntas antes de finalizar.', 'warning');
                    return;
                }

                Swal.fire({
                    title: '¿Terminar examen?',
                    text: "Una vez finalizado no podrás cambiar tus respuestas.",
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonColor: '#198754',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: 'Sí, finalizar',
                    cancelButtonText: 'Cancelar'
                }).then((result) => {
                    if (result.isConfirmed) {
                        enviarResultados();
                    }
                });
            });
        });

        function cargarPreguntas() {
            let idAsig = $('#hfIdAsignacion').val();
            if (!idAsig) {
                Swal.fire('Error', 'No se especificó un examen.', 'error');
                return;
            }

            $.ajax({
                type: "POST",
                url: "AplicacionExamen.aspx/ObtenerPreguntas",
                data: JSON.stringify({ idAsignacion: parseInt(idAsig) }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    preguntas = JSON.parse(response.d);
                    if (preguntas.length > 0) {
                        mostrarPregunta(0);
                    } else {
                        $('#contenedorPreguntas').html('<div class="alert alert-warning">Este examen no tiene preguntas.</div>');
                        $('#btnSiguiente').hide();
                    }
                },
                error: function (error) {
                    console.log(error);
                    Swal.fire('Error', 'Hubo un problema al cargar el examen.', 'error');
                }
            });
        }

        function mostrarPregunta(index) {
            let p = preguntas[index];
            let html = `
                <div class="mb-4 text-muted fw-bold text-uppercase small">Pregunta ${index + 1} de ${preguntas.length}</div>
                <div class="pregunta-texto mb-5">${p.TextoPregunta}</div>
                <div class="opciones-lista">
            `;

            p.Opciones.forEach(op => {
                let check = (respuestas[p.IdPregunta] === op.IdOpcion) ? 'checked' : '';
                html += `
                    <div>
                        <input type="radio" name="optPregunta" id="opt_${op.IdOpcion}" value="${op.IdOpcion}" class="opcion-radio" ${check}>
                        <label for="opt_${op.IdOpcion}" class="opcion-label shadow-sm">${op.TextoOpcion}</label>
                    </div>
                `;
            });

            html += `</div>`;
            $('#contenedorPreguntas').html(html);

            // Actualizar botones
            if (index === 0) $('#btnAnterior').hide(); else $('#btnAnterior').show();
            if (index === preguntas.length - 1) {
                $('#btnSiguiente').hide();
                $('#btnFinalizar').show();
            } else {
                $('#btnSiguiente').show();
                $('#btnFinalizar').hide();
            }

            // Barra de progreso
            let pct = ((index + 1) / preguntas.length) * 100;
            $('#progressBar').css('width', pct + '%').attr('aria-valuenow', pct);
        }

        function guardarRespuestaLocal() {
            let selected = $('input[name="optPregunta"]:checked').val();
            if (selected) {
                respuestas[preguntas[indexActual].IdPregunta] = parseInt(selected);
            }
        }

        function enviarResultados() {
            let idAsig = $('#hfIdAsignacion').val();
            let listaResp = [];
            for (let idPreg in respuestas) {
                listaResp.push({ IdPregunta: parseInt(idPreg), IdOpcion: respuestas[idPreg] });
            }

            $.ajax({
                type: "POST",
                url: "AplicacionExamen.aspx/ProcesarRespuestas",
                data: JSON.stringify({ idAsignacion: parseInt(idAsig), respuestas: listaResp }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    let res = response.d;
                    let esAprobado = res.Calificacion >= 70;
                    
                    Swal.fire({
                        title: esAprobado ? '¡Felicidades!' : 'Examen Terminado',
                        html: `Tu calificación es: <strong style="font-size: 2rem; color: ${esAprobado ? '#198754' : '#dc3545'};">${res.Calificacion}</strong>`,
                        icon: esAprobado ? 'success' : 'info',
                        confirmButtonText: 'Ver Resultados',
                        allowOutsideClick: false
                    }).then((result) => {
                        window.location.href = "ResultadosExamen.aspx?id=" + res.IdCalificacion;
                    });
                },
                error: function (error) {
                    console.log(error);
                    Swal.fire('Error', 'Hubo un problema al procesar el examen.', 'error');
                }
            });
        }
    </script>
</asp:Content>
