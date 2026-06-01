<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Login Personal - INEA</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700&display=swap" rel="stylesheet" />
    <style>
        body { font-family: 'Inter', sans-serif; background-color: #f8f9fa; height: 100vh; display: flex; align-items: center; justify-content: center; }
        .card { border: none; border-radius: 15px; box-shadow: 0 10px 30px rgba(0,0,0,0.1); overflow: hidden; }
        .card-header { background: linear-gradient(135deg, #0d6efd 0%, #0b5ed7 100%); color: white; padding: 2rem; text-align: center; }
        .btn-gradient { background: linear-gradient(135deg, #0d6efd 0%, #0b5ed7 100%); color: white; border: none; padding: 0.8rem; font-weight: 600; }
        .btn-gradient:hover { opacity: 0.9; color: white; }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="w-100" style="max-width: 450px;">
        <div class="card fade-in-up mx-3">
            <div class="card-header">
                <h3 class="mb-0">INEA</h3>
                <p class="mb-0 text-white-50">Acceso de Personal</p>
            </div>
            <div class="card-body p-4">
                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </asp:Panel>
                <div class="form-floating mb-3">
                    <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Usuario"></asp:TextBox>
                    <label for="txtUsuario">Usuario</label>
                </div>
                <div class="form-floating mb-4">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
                    <label for="txtPassword">Contraseña</label>
                </div>
                <div class="d-grid gap-2">
                    <asp:Button ID="btnLogin" runat="server" Text="Iniciar Sesión" CssClass="btn btn-gradient rounded-pill" OnClick="btnLogin_Click" />
                    <a href="LoginAlumno.aspx" class="btn btn-outline-secondary rounded-pill mt-2">Soy Alumno</a>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
