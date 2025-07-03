< !DOCTYPE html >
< html lang = "pt" >
< head >
    < meta charset = "utf-8" />
    < title > @ViewBag.Title - MS Premium Solutions</title>
    <meta name = "viewport" content= "width=device-width, initial-scale=1.0" />
    < link rel= "stylesheet" href= "~/lib/bootstrap/dist/css/bootstrap.min.css" />
    < link href= "https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel= "stylesheet" />
    < link rel= "stylesheet" href= "~/css/site.css" asp-append-version= "true" >


    < style >
        /* Estilos para o submenu. Pode mover isto para o seu ficheiro site.css se preferir. */
        .submenu {
            display: none; /* Escondido por defeito */
padding - left: 20px; /* Indentação */
background - color: rgba(0, 0, 0, 0.1);
border - left: 3px solid #ffc107; /* Destaque amarelo */
            margin-left: 10px;
border - radius: 0 5px 5px 0;
padding - bottom: 10px;
        }

        .submenu.show {
display: block; /* Torna o submenu visível */
}

        .submenu.nav - link {
    font - size: 0.9em;
    padding - top: .4rem;
    padding - bottom: .4rem;
}
        
        /* Círculos para as etapas do submenu */
        .submenu - item - circle {
display: inline - block;
width: 16px;
height: 16px;
border: 2px solid #fff;
            border - radius: 50 %;
    margin - right: 10px;
    vertical - align: middle;
transition: background - color 0.2s;
}

        /* Estilo para o link da etapa ativa */
        .submenu a.nav - link.active.submenu - item - circle {
    background - color: #ffc107; /* Preenche o círculo quando a página está ativa */
        }
    </ style >
</ head >
< body >
    < !--Sidebar-- >
    < div class= "sidebar d-flex flex-column" >
        < div class= "logo" >
            < img src = "~/images/Logo.JPG" alt = "MS Premium Solutions" class= "img-fluid" />
        </ div >
        < nav class= "nav flex-column mt-3" >
            @if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                < a class= "nav-link" asp - controller = "Home" asp - action = "Index" >< i class= "bi bi-house-door-fill" ></ i > Página Inicial </ a >

                @* SEU LINK ORIGINAL PARA CLIENTES - RESTAURADO *@
                < a class= "nav-link" asp - controller = "Home" asp - action = "ClientPainel" >< i class= "bi bi-bar-chart" ></ i > Clientes </ a >


                < a class= "nav-link" asp - controller = "Budget" asp - action = "Index" >< i class= "bi bi-calculator-fill" ></ i > Orçamento </ a >

                < !--SUBMENU PARA QUALIDADE DO AR - A única adição funcional -->
                <div id="qualidadeArSubmenu" class= "submenu" >
                    < div class= "d-flex justify-content-between align-items-center ps-3 pe-2 pt-2" >
                         < h6 class= "text-white mb-0" > Qualidade do ar </ h6 >
                         < button id = "closeSubmenuBtn" class= "btn btn-sm btn-link text-white p-0" title = "Fechar menu de orçamento" >< i class= "bi bi-x-circle" ></ i ></ button >
                    </ div >
                    < a class= "nav-link" asp - controller = "Budget" asp - action = "TipologiaConstrutiva" >
                        < span class= "submenu-item-circle" ></ span > Tipologia construtiva
                    </ a >
                    < a class= "nav-link" asp - controller = "Budget" asp - action = "ColecaoDados" >
                        < span class= "submenu-item-circle" ></ span > Coleção de dados
                    </a>
                    <a class= "nav-link" asp - controller = "Budget" asp - action = "Objetivos" >
                        < span class= "submenu-item-circle" ></ span > Objetivos
                    </ a >
                    < a class= "nav-link" asp - controller = "Budget" asp - action = "Volumes" >
                        < span class= "submenu-item-circle" ></ span > Volumes
                    </ a >
                    < a class= "nav-link" asp - action = "DetalheOrcamento" asp - controller = "Budget" >
                        < span class= "submenu-item-circle" ></ span > Detalhe do orçamento
                    </ a >
                    < a class= "nav-link" asp - action = "ResumoOrcamento" asp - controller = "Budget" >
                        < span class= "submenu-item-circle" ></ span > Resumo do orçamento
                    </ a >
                </ div >

                @if(User.IsInRole("Admin"))
                {
                    < a class= "nav-link" asp - controller = "Admin" asp - action = "AdminMenu" >< i class= "bi bi-gear-fill" ></ i > Settings(Admin) </ a >
                }
            }
            else
{
                < a class= "nav-link" asp - controller = "Account" asp - action = "Login" >< i class= "bi bi-box-arrow-in-right" ></ i > Login </ a >
            }
        </ nav >
    </ div >

    < !--Topbar(Seu código original, intacto)-- >
    < div class= "topbar d-flex align-items-center p-2" >
        < button class= "menu-toggle btn btn-link text-white me-auto" id = "menuToggleBtn" type = "button" title = "Alternar menu" >
            < i class= "bi bi-list" style = "font-size: 1.5rem;" ></ i >
        </ button >

        @if(User.Identity != null && User.Identity.IsAuthenticated)
        {
            @* SEU BLOCO ORIGINAL PARA O NOME DO UTILIZADOR - RESTAURADO *@
            < span class= "me-3 text-white" > Olá, < strong > @User.FindFirst("FullName")?.Value </ strong ></ span >
            < form asp - controller = "Account" asp - action = "Logout" method = "post" id = "logoutForm" class= "d-inline" >
                @Html.AntiForgeryToken()
                < button type = "submit" class= "btn btn-outline-light btn-sm" title = "Logout" >
                    < i class= "bi bi-box-arrow-right" ></ i > Sair
                </ button >
            </ form >
        }
        else
{
            < span > </ span >
        }
    </ div >

    < !--Main Content-- >
    < div class= "content" >
        @RenderBody()
    </ div >

    < script src = "~/lib/jquery/dist/jquery.min.js" ></ script >
    < script src = "~/lib/bootstrap/dist/js/bootstrap.bundle.min.js" ></ script >
    < script src = "~/js/site.js" asp - append - version = "true" ></ script >

    @* SCRIPT PARA O TOGGLE DO MENU PRINCIPAL (Seu código original) *@
    < script >
        document.addEventListener('DOMContentLoaded', function() {
    const menuToggleBtn = document.getElementById('menuToggleBtn');
    const sidebar = document.querySelector('.sidebar');

    if (menuToggleBtn && sidebar)
    {
        menuToggleBtn.addEventListener('click', function() {
            sidebar.classList.toggle('active');
        });
    }
});
    </ script >

    @* SCRIPT PARA CONTROLAR O SUBMENU (A adição necessária) *@
    < script >
        document.addEventListener('DOMContentLoaded', function() {
    const submenu = document.getElementById('qualidadeArSubmenu');
    const closeSubmenuBtn = document.getElementById('closeSubmenuBtn');

    // Expor as funções para que possam ser chamadas de outras views
    window.showQualidadeArSubmenu = function() {
        if (submenu) submenu.classList.add('show');
    }

    window.hideQualidadeArSubmenu = function() {
        if (submenu) submenu.classList.remove('show');
    }

    // Evento para o botão de fechar do submenu
    if (closeSubmenuBtn)
    {
        closeSubmenuBtn.addEventListener('click', function() {
            window.hideQualidadeArSubmenu();
        });
    }

    // Lógica para manter o submenu aberto se estivermos numa das suas páginas
    const currentPath = window.location.pathname.toLowerCase();
    const submenuPaths = [
        '/budget/tipologiaconstrutiva',
                '/budget/colecaodados',
                '/budget/objetivos',
                '/budget/volumes',
                '/budget/detalheorcamento',
                '/budget/resumoorcamento'
    ];

    // Verifica se a URL atual contém algum dos caminhos do submenu
    const isInSubmenu = submenuPaths.some(path => currentPath.includes(path));

    if (isInSubmenu)
    {
        window.showQualidadeArSubmenu();

        // Tenta encontrar o link ativo e marcar o círculo
        try
        {
            const activeLink = document.querySelector(`.submenu a.nav - link[href *= '${currentPath.split(' / ').pop()}']`);
            if (activeLink)
            {
                activeLink.classList.add('active');
            }
        }
        catch (e)
        {
            console.error("Erro ao tentar encontrar o link ativo do submenu:", e);
        }
    }
});
    </ script >

    @await RenderSectionAsync("Scripts", required: false)
</ body >
</ html >