#pragma checksum "C:\Users\Ancowi\Desktop\AnotherShit\project\vs\Himitsu\Views\Shared\error403.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f495c83c6b6643fcb245441cb7aa23dc3824a566"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_error403), @"mvc.1.0.view", @"/Views/Shared/error403.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Ancowi\Desktop\AnotherShit\project\vs\Himitsu\Views\Shared\error403.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f495c83c6b6643fcb245441cb7aa23dc3824a566", @"/Views/Shared/error403.cshtml")]
    public class Views_Shared_error403 : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<!DOCTYPE html>\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f495c83c6b6643fcb245441cb7aa23dc3824a5662929", async() => {
                WriteLiteral(@"
    <link rel=""shortcut icon"" href="".ico"">
    <link rel=""stylesheet"" href=""/css/main.css"">
    <link rel=""stylesheet"" href=""/css/content-1.css"">
    <script type=""text/javascript"" src=""https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js""></script>
    <script type=""text/javascript"" src=""/js/global.js""></script>
    <link href=""https://fonts.googleapis.com/css2?family=Exo+2:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap"" rel=""stylesheet"">
    <meta charset=""utf-8"">
    <title>Himitsu</title>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n<header id=\"header\">\r\n    <div id=\"header-top\">\r\n        <a title=\"Открыть меню\"><img src=\"/resources/header/logo.png\" id=\"header-logo\" onclick=\"headerLogoButton()\" /></a>\r\n        <div id=\"header-profile-container\">\r\n        </div>\r\n");
#nullable restore
#line 20 "C:\Users\Ancowi\Desktop\AnotherShit\project\vs\Himitsu\Views\Shared\error403.cshtml"
           if (Context.Session.Keys.Contains("userid"))
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <a");
            BeginWriteAttribute("href", " href=\"", 988, "\"", 1033, 2);
            WriteAttributeValue("", 995, "/u/", 995, 3, true);
#nullable restore
#line 21 "C:\Users\Ancowi\Desktop\AnotherShit\project\vs\Himitsu\Views\Shared\error403.cshtml"
WriteAttributeValue("", 998, Context.Session.GetInt32("userid"), 998, 35, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"header-button-a\"><img src=\"../resources/header/user.svg\" class=\"header-button\" /><text class=\"header-button-text\">Профиль</text></a> ");
#nullable restore
#line 21 "C:\Users\Ancowi\Desktop\AnotherShit\project\vs\Himitsu\Views\Shared\error403.cshtml"
                                                                                                                                                                                                           } 

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        <a href=""/"" class=""header-button-a""><img src=""../resources/header/home-button.png"" class=""header-button"" /><text class=""header-button-text"">Главная</text></a>
        <a href=""/leaderboard"" class=""header-button-a""><img src=""../resources/header/leaderboard-button.png"" class=""header-button"" /><text class=""header-button-text"">Рекорды</text></a>
        <a href=""https://hentai.ninja/beatmaps"" class=""header-button-a""><img src=""../resources/header/library-button.png"" class=""header-button"" /><text class=""header-button-text"">Библиотека</text></a>
    </div>
    <div id=""header-bottom"">
        <a href=""/about"" class=""header-button-a""><img src=""../resources/header/about.png"" class=""header-button"" /><text class=""header-button-text"">О&nbsp;сервере</text></a>
        <a href=""/settings"" class=""header-button-a""><img src=""../resources/header/settings.png"" class=""header-button"" /><text class=""header-button-text"">Настройки</text></a>
");
#nullable restore
#line 29 "C:\Users\Ancowi\Desktop\AnotherShit\project\vs\Himitsu\Views\Shared\error403.cshtml"
           if (Context.Session.Keys.Contains("userid"))
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <a href=\"/logout\" class=\"header-button-a\"><img src=\"../resources/header/logout.png\" class=\"header-button\" /><text class=\"header-button-text\">Выйти</text></a> ");
#nullable restore
#line 30 "C:\Users\Ancowi\Desktop\AnotherShit\project\vs\Himitsu\Views\Shared\error403.cshtml"
                                                                                                                                                                            } 

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\r\n</header>\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f495c83c6b6643fcb245441cb7aa23dc3824a5667898", async() => {
                WriteLiteral(@"
    <div id=""content"">
        <div id=""error-panel"">
            <img src=""/resources/error/error403-tyan.png"" height=""350px"" />
            <div id=""error"">
                <p class=""ops"">Тебе сюда нельзя! -з-</p>
                <p class=""error"">403</p>
                <p class=""error-description"">");
#nullable restore
#line 41 "C:\Users\Ancowi\Desktop\AnotherShit\project\vs\Himitsu\Views\Shared\error403.cshtml"
                                        Write(ViewBag.Error);

#line default
#line hidden
#nullable disable
                WriteLiteral("</p>\r\n            </div>\r\n        </div>\r\n    </div>\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
