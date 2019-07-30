using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ProdFloor.Infrastructure
{

    #region oldPagination
    //The previous pages were duplicated, therefore, we made two unique cool paginations for greater standardization.
    /*
    [HtmlTargetElement("page-doorop", Attributes = "page-model")]
    public class PageLinkTagHelperDoorOp : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelperDoorOp(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                PageUrlValues["page"] = i;
                tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage
                    ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    public class PageLinkTagReasons : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagReasons(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }
        public int reasonNumber { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                PageUrlValues["page"] = i;
                PageUrlValues["reasonNumber"] = reasonNumber;
                tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage
                    ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    [HtmlTargetElement("select-countries", Attributes = "page-model")]
    public class PageLinkTagHelperSelectCountries : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        private IItemRepository repository;

        public PageLinkTagHelperSelectCountries(IUrlHelperFactory helperFactory, IItemRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public string CurrentCountry { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("select");
            foreach (Country country in repository.Countries)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["Value"] = country.Name;
                if (country.Name == CurrentCountry)
                {
                    tag.InnerHtml.Append("Selected");
                }
                result.InnerHtml.Append(tag.ToString());
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    [HtmlTargetElement("page-all", Attributes = "page-model")]
    public class PageLinkTagHelperAll : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelperAll(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                PageUrlValues["page"] = i;
                tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage
                    ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public bool DashBoardEnabled { get; set; } = false;
        public PagingInfo AdditionalPageModel { get; set; }
        public string MainUrlValue { get; set; }
        public string AddUrlValue { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            if (DashBoardEnabled)
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                TagBuilder result = new TagBuilder("div");
                for (int i = 1; i <= PageModel.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    PageUrlValues[MainUrlValue] = i;
                    PageUrlValues[AddUrlValue] = AdditionalPageModel.CurrentPage;
                    tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                    if (PageClassesEnabled)
                    {
                        tag.AddCssClass(PageClass);
                        tag.AddCssClass(i == PageModel.CurrentPage
                        ? PageClassSelected : PageClassNormal);
                    }
                    tag.InnerHtml.Append(i.ToString());
                    result.InnerHtml.AppendHtml(tag);
                }
                output.Content.AppendHtml(result.InnerHtml);
            }
            else
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                TagBuilder result = new TagBuilder("div");
                for (int i = 1; i <= PageModel.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    PageUrlValues["jobPage"] = i;
                    tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                    if (PageClassesEnabled)
                    {
                        tag.AddCssClass(PageClass);
                        tag.AddCssClass(i == PageModel.CurrentPage
                        ? PageClassSelected : PageClassNormal);
                    }
                    tag.InnerHtml.Append(i.ToString());
                    result.InnerHtml.AppendHtml(tag);
                }
                output.Content.AppendHtml(result.InnerHtml);
            }
        }
    }

    public class PageLinkDashBoardTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkDashBoardTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }



        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public PagingInfo MyJobsPageModel { get; set; }
        public PagingInfo OnCrossPageModel { get; set; }
        public PagingInfo PendingToCrossPageModel { get; set; }
        public string MyJobsUrlValue { get; set; }
        public string OnCrossUrlValue { get; set; }
        public string PendingToCrossUrlValue { get; set; }
        public string Sort { get; set; }
        public string CurrentModel { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(CurrentModel))
            {
                switch (CurrentModel)
                {
                    case "MyJobs":

                        IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result = new TagBuilder("div");
                        for (int i = 1; i <= MyJobsPageModel.TotalPages; i++)
                        {
                            TagBuilder tag = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = i;
                            PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                            PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                            PageUrlValues[Sort] = MyJobsPageModel.sort;
                            tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag.AddCssClass(PageClass);
                                tag.AddCssClass(i == MyJobsPageModel.CurrentPage
                                ? PageClassSelected : PageClassNormal);
                            }
                            tag.InnerHtml.Append(i.ToString());
                            result.InnerHtml.AppendHtml(tag);
                        }
                        output.Content.AppendHtml(result.InnerHtml);
                        break;
                    case "OncrossJobs":
                        IUrlHelper urlHelper2 = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result2 = new TagBuilder("div");
                        for (int i = 1; i <= OnCrossPageModel.TotalPages; i++)
                        {
                            TagBuilder tag = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                            PageUrlValues[OnCrossUrlValue] = i;
                            PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                            PageUrlValues[Sort] = OnCrossPageModel.sort;
                            tag.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag.AddCssClass(PageClass);
                                tag.AddCssClass(i == OnCrossPageModel.CurrentPage
                                ? PageClassSelected : PageClassNormal);
                            }
                            tag.InnerHtml.Append(i.ToString());
                            result2.InnerHtml.AppendHtml(tag);
                        }
                        output.Content.AppendHtml(result2.InnerHtml);
                        break;
                    case "PendingJobs":
                        IUrlHelper urlHelper3 = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result3 = new TagBuilder("div");
                        for (int i = 1; i <= PendingToCrossPageModel.TotalPages; i++)
                        {
                            TagBuilder tag = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                            PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                            PageUrlValues[PendingToCrossUrlValue] = i;
                            PageUrlValues[Sort] = OnCrossPageModel.sort;
                            tag.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag.AddCssClass(PageClass);
                                tag.AddCssClass(i == PendingToCrossPageModel.CurrentPage
                                ? PageClassSelected : PageClassNormal);
                            }
                            tag.InnerHtml.Append(i.ToString());
                            result3.InnerHtml.AppendHtml(tag);
                        }
                        output.Content.AppendHtml(result3.InnerHtml);
                        break;
                }

            }
        }
    }

    public class PageLinkStepsTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkStepsTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }


        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public PagingInfo ElmHydroPagingInfo { get; set; }
        public PagingInfo ElmTractionPagingInfo { get; set; }
        public PagingInfo M2000PagingInfo { get; set; }
        public PagingInfo M4000PagingInfo { get; set; }
        public string ElmHydroUrlValue { get; set; }
        public string ElmTractionUrlValue { get; set; }
        public string M2000UrlValue { get; set; }
        public string M4000UrlValue { get; set; }
        public string JobTypeSelected { get; set; }
        public string CurrentModel { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            switch (CurrentModel)
            {
                case "Hydro":

                    IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                    TagBuilder result = new TagBuilder("div");
                    for (int i = 1; i <= ElmHydroPagingInfo.TotalPages; i++)
                    {
                        TagBuilder tag = new TagBuilder("a");
                        PageUrlValues[ElmHydroUrlValue] = i;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmHydroPagingInfo.JobTypeName;
                        tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag.AddCssClass(PageClass);
                            tag.AddCssClass(i == ElmHydroPagingInfo.CurrentPage
                            ? PageClassSelected : PageClassNormal);
                        }
                        tag.InnerHtml.Append(i.ToString());
                        result.InnerHtml.AppendHtml(tag);
                    }
                    output.Content.AppendHtml(result.InnerHtml);
                    break;
                case "Traction":
                    IUrlHelper urlHelper2 = urlHelperFactory.GetUrlHelper(ViewContext);
                    TagBuilder result2 = new TagBuilder("div");
                    for (int i = 1; i <= ElmTractionPagingInfo.TotalPages; i++)
                    {
                        TagBuilder tag = new TagBuilder("a");
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = i;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmTractionPagingInfo.JobTypeName;
                        tag.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag.AddCssClass(PageClass);
                            tag.AddCssClass(i == ElmTractionPagingInfo.CurrentPage
                            ? PageClassSelected : PageClassNormal);
                        }
                        tag.InnerHtml.Append(i.ToString());
                        result2.InnerHtml.AppendHtml(tag);
                    }
                    output.Content.AppendHtml(result2.InnerHtml);
                    break;
                case "M2000":
                    IUrlHelper urlHelper3 = urlHelperFactory.GetUrlHelper(ViewContext);
                    TagBuilder result3 = new TagBuilder("div");
                    for (int i = 1; i <= M2000PagingInfo.TotalPages; i++)
                    {
                        TagBuilder tag = new TagBuilder("a");
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = i;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = M2000PagingInfo.JobTypeName;
                        tag.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag.AddCssClass(PageClass);
                            tag.AddCssClass(i == M2000PagingInfo.CurrentPage
                            ? PageClassSelected : PageClassNormal);
                        }
                        tag.InnerHtml.Append(i.ToString());
                        result3.InnerHtml.AppendHtml(tag);
                    }
                    output.Content.AppendHtml(result3.InnerHtml);
                    break;
                case "M4000":
                    IUrlHelper urlHelper4 = urlHelperFactory.GetUrlHelper(ViewContext);
                    TagBuilder result4 = new TagBuilder("div");
                    for (int i = 1; i <= M4000PagingInfo.TotalPages; i++)
                    {
                        TagBuilder tag = new TagBuilder("a");
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = i;
                        PageUrlValues[JobTypeSelected] = M4000PagingInfo.JobTypeName;
                        tag.Attributes["href"] = urlHelper4.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag.AddCssClass(PageClass);
                            tag.AddCssClass(i == M4000PagingInfo.CurrentPage
                            ? PageClassSelected : PageClassNormal);
                        }
                        tag.InnerHtml.Append(i.ToString());
                        result4.InnerHtml.AppendHtml(tag);
                    }
                    output.Content.AppendHtml(result4.InnerHtml);
                    break;
                            {
                                tag.AddCssClass(PageClass);
                                tag.AddCssClass(i == MyJobsPageModel.CurrentPage
                                ? PageClassSelected : PageClassNormal);
                            }
                            tag.InnerHtml.Append(i.ToString());
                            result.InnerHtml.AppendHtml(tag);
                        }
                        output.Content.AppendHtml(result.InnerHtml);
                        break;
                    case "OncrossJobs":
                        IUrlHelper urlHelper2 = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result2 = new TagBuilder("div");
                        for (int i = 1; i <= OnCrossPageModel.TotalPages; i++)
                        {
                            TagBuilder tag = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                            PageUrlValues[OnCrossUrlValue] = i;
                            PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                            PageUrlValues[Sort] = OnCrossPageModel.sort;
                            tag.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag.AddCssClass(PageClass);
                                tag.AddCssClass(i == OnCrossPageModel.CurrentPage
                                ? PageClassSelected : PageClassNormal);
                            }
                            tag.InnerHtml.Append(i.ToString());
                            result2.InnerHtml.AppendHtml(tag);
                        }
                        output.Content.AppendHtml(result2.InnerHtml);
                        break;
                    case "PendingJobs":
                        IUrlHelper urlHelper3 = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result3 = new TagBuilder("div");
                        for (int i = 1; i <= PendingToCrossPageModel.TotalPages; i++)
                        {
                            TagBuilder tag = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                            PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                            PageUrlValues[PendingToCrossUrlValue] = i;
                            PageUrlValues[Sort] = OnCrossPageModel.sort;
                            tag.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag.AddCssClass(PageClass);
                                tag.AddCssClass(i == PendingToCrossPageModel.CurrentPage
                                ? PageClassSelected : PageClassNormal);
                            }
                            tag.InnerHtml.Append(i.ToString());
                            result3.InnerHtml.AppendHtml(tag);
                        }
                        output.Content.AppendHtml(result3.InnerHtml);
                        break;
                }

            }
        }
    }
    */
    #endregion

    public class CooolPaginationAll : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public CooolPaginationAll(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            TagBuilder tag = new TagBuilder("a");
            TagBuilder tag1 = new TagBuilder("a");
            TagBuilder tag4 = new TagBuilder("a");
            TagBuilder tag5 = new TagBuilder("a");
            /*************************************************************************/
            PageUrlValues["page"] = 1;
            tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            if (PageClassesEnabled)
            {
                tag.AddCssClass(PageClass);
                tag.AddCssClass(PageClassSelected);
            }
            tag.InnerHtml.Append("<<");
            result.InnerHtml.AppendHtml(tag);
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            PageUrlValues["page"] = (PageModel.CurrentPage - 1) != 0 ? (PageModel.CurrentPage - 1) : 1;
            tag1.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            if (PageClassesEnabled)
            {
                tag1.AddCssClass(PageClass);
                tag1.AddCssClass(PageClassSelected);
            }
            tag1.InnerHtml.Append("<");
            result.InnerHtml.AppendHtml(tag1);
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            List<int> Pages = new List<int>();
            int countPages = PageModel.CurrentPage;
            while ((countPages >= PageModel.CurrentPage - 3 && countPages != 0))
            {
                Pages.Add(countPages);
                countPages--;
            }
            Pages.Reverse();
            int pagesRest = 4 - Pages.Count;
            for (int i = Pages[0]; i <= Pages.Last(); i++)
            {
                TagBuilder tag2 = new TagBuilder("a");
                PageUrlValues["page"] = i;
                tag2.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tag2.AddCssClass(PageClass);
                    tag2.AddCssClass(i == PageModel.CurrentPage
                    ? "btn-info" : PageClassNormal);
                }
                tag2.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag2);
            }
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            for (int i = PageModel.CurrentPage + 1; (i <= PageModel.CurrentPage + 3 + pagesRest && i <= PageModel.TotalPages); i++)
            {
                TagBuilder tag3 = new TagBuilder("a");
                PageUrlValues["page"] = i;
                tag3.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tag3.AddCssClass(PageClass);
                    tag3.AddCssClass(i == PageModel.CurrentPage
                    ? "btn-info" : PageClassNormal);
                }
                tag3.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag3);
            }
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            PageUrlValues["page"] = (PageModel.CurrentPage + 1) <= PageModel.TotalPages ? (PageModel.CurrentPage + 1) : PageModel.TotalPages;
            tag4.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            if (PageClassesEnabled)
            {
                tag4.AddCssClass(PageClass);
                tag4.AddCssClass(PageClassSelected);
            }
            tag4.InnerHtml.Append(">");
            result.InnerHtml.AppendHtml(tag4);
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            PageUrlValues["page"] = PageModel.TotalPages;
            tag5.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            if (PageClassesEnabled)
            {
                tag5.AddCssClass(PageClass);
                tag5.AddCssClass(PageClassSelected);
            }
            tag5.InnerHtml.Append(">>");
            result.InnerHtml.AppendHtml(tag5);
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    public class CooolPaginationTest : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public CooolPaginationTest(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }

        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageID { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");
            TagBuilder tag = new TagBuilder("a");
            TagBuilder tag1 = new TagBuilder("a");
            TagBuilder tag4 = new TagBuilder("a");
            TagBuilder tag5 = new TagBuilder("a");
            /*************************************************************************/
            PageUrlValues["ID"] = PageID;
            PageUrlValues["page"] = 1;
            tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            if (PageClassesEnabled)
            {
                tag.AddCssClass(PageClass);
                tag.AddCssClass(PageClassSelected);
            }
            tag.InnerHtml.Append("<<");
            result.InnerHtml.AppendHtml(tag);
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            PageUrlValues["ID"] = PageID;
            PageUrlValues["page"] = (PageModel.CurrentPage - 1) != 0 ? (PageModel.CurrentPage - 1) : 1;
            tag1.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            if (PageClassesEnabled)
            {
                tag1.AddCssClass(PageClass);
                tag1.AddCssClass(PageClassSelected);
            }
            tag1.InnerHtml.Append("<");
            result.InnerHtml.AppendHtml(tag1);
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            List<int> Pages = new List<int>();
            int countPages = PageModel.CurrentPage;
            while ((countPages >= PageModel.CurrentPage - 3 && countPages != 0))
            {
                Pages.Add(countPages);
                countPages--;
            }
            Pages.Reverse();
            int pagesRest = 4 - Pages.Count;
            for (int i = Pages[0]; i <= Pages.Last(); i++)
            {
                TagBuilder tag2 = new TagBuilder("a");
                PageUrlValues["ID"] = PageID;
                PageUrlValues["page"] = i;
                tag2.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tag2.AddCssClass(PageClass);
                    tag2.AddCssClass(i == PageModel.CurrentPage
                    ? "btn-info" : PageClassNormal);
                }
                tag2.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag2);
            }
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            for (int i = PageModel.CurrentPage + 1; (i <= PageModel.CurrentPage + 3 + pagesRest && i <= PageModel.TotalPages); i++)
            {
                TagBuilder tag3 = new TagBuilder("a");
                PageUrlValues["ID"] = PageID;
                PageUrlValues["page"] = i;
                tag3.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tag3.AddCssClass(PageClass);
                    tag3.AddCssClass(i == PageModel.CurrentPage
                    ? "btn-info" : PageClassNormal);
                }
                tag3.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag3);
            }
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            PageUrlValues["ID"] = PageID;
            PageUrlValues["page"] = (PageModel.CurrentPage + 1) <= PageModel.TotalPages ? (PageModel.CurrentPage + 1) : PageModel.TotalPages;
            tag4.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            if (PageClassesEnabled)
            {
                tag4.AddCssClass(PageClass);
                tag4.AddCssClass(PageClassSelected);
            }
            tag4.InnerHtml.Append(">");
            result.InnerHtml.AppendHtml(tag4);
            output.Content.AppendHtml(result.InnerHtml);
            /*************************************************************************/
            PageUrlValues["ID"] = PageID;
            PageUrlValues["page"] = PageModel.TotalPages;
            tag5.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            if (PageClassesEnabled)
            {
                tag5.AddCssClass(PageClass);
                tag5.AddCssClass(PageClassSelected);
            }
            tag5.InnerHtml.Append(">>");
            result.InnerHtml.AppendHtml(tag5);
            output.Content.AppendHtml(result.InnerHtml);
        }
    }

    public class CoolPaginationDashBoard : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public CoolPaginationDashBoard(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }



        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public PagingInfo MyJobsPageModel { get; set; }
        public PagingInfo OnCrossPageModel { get; set; }
        public PagingInfo PendingToCrossPageModel { get; set; }
        public string MyJobsUrlValue { get; set; }
        public string OnCrossUrlValue { get; set; }
        public string PendingToCrossUrlValue { get; set; }
        public string Sort { get; set; }
        public string CurrentModel { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(CurrentModel))
            {
                switch (CurrentModel)
                {
                    case "MyJobs":
                        IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result = new TagBuilder("div");
                        TagBuilder tag = new TagBuilder("a");
                        TagBuilder tag1 = new TagBuilder("a");
                        TagBuilder tag4 = new TagBuilder("a");
                        TagBuilder tag5 = new TagBuilder("a");
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = 1;
                        PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                        PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                        PageUrlValues[Sort] = MyJobsPageModel.sort;
                        tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag.AddCssClass(PageClass);
                            tag.AddCssClass(PageClassSelected);
                        }
                        tag.InnerHtml.Append("<<");
                        result.InnerHtml.AppendHtml(tag);
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = (MyJobsPageModel.CurrentPage - 1) != 0 ? (MyJobsPageModel.CurrentPage - 1) : 1;
                        PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                        PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                        tag1.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag1.AddCssClass(PageClass);
                            tag1.AddCssClass(PageClassSelected);
                        }
                        tag1.InnerHtml.Append("<");
                        result.InnerHtml.AppendHtml(tag1);
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        List<int> Pages = new List<int>();
                        int countPages = MyJobsPageModel.CurrentPage;
                        while ((countPages >= MyJobsPageModel.CurrentPage - 3 && countPages != 0))
                        {
                            Pages.Add(countPages);
                            countPages--;
                        }
                        Pages.Reverse();
                        int pagesRest = 4 - Pages.Count;
                        for (int i = Pages[0]; i <= Pages.Last(); i++)
                        {
                            TagBuilder tag2 = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = i;
                            PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                            PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                            PageUrlValues[Sort] = MyJobsPageModel.sort;
                            tag2.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag2.AddCssClass(PageClass);
                                tag2.AddCssClass(i == MyJobsPageModel.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag2.InnerHtml.Append(i.ToString());
                            result.InnerHtml.AppendHtml(tag2);
                        }
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        for (int i = MyJobsPageModel.CurrentPage + 1; (i <= MyJobsPageModel.CurrentPage + 3 + pagesRest && i <= MyJobsPageModel.TotalPages); i++)
                        {
                            TagBuilder tag3 = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = i;
                            PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                            PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                            PageUrlValues[Sort] = MyJobsPageModel.sort;
                            tag3.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag3.AddCssClass(PageClass);
                                tag3.AddCssClass(i == MyJobsPageModel.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag3.InnerHtml.Append(i.ToString());
                            result.InnerHtml.AppendHtml(tag3);
                        }
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = (MyJobsPageModel.CurrentPage + 1) <= MyJobsPageModel.TotalPages ? (MyJobsPageModel.CurrentPage + 1) : MyJobsPageModel.TotalPages;
                        PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                        PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                        PageUrlValues[Sort] = MyJobsPageModel.sort;
                        tag4.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag4.AddCssClass(PageClass);
                            tag4.AddCssClass(PageClassSelected);
                        }
                        tag4.InnerHtml.Append(">");
                        result.InnerHtml.AppendHtml(tag4);
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] =  MyJobsPageModel.TotalPages;
                        PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                        PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                        PageUrlValues[Sort] = MyJobsPageModel.sort;
                        tag5.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag5.AddCssClass(PageClass);
                            tag5.AddCssClass(PageClassSelected);
                        }
                        tag5.InnerHtml.Append(">>");
                        result.InnerHtml.AppendHtml(tag5);
                        output.Content.AppendHtml(result.InnerHtml);
                        break;
                        
                    case "OncrossJobs":
                        IUrlHelper urlHelper2 = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result2 = new TagBuilder("div");
                        TagBuilder tag6 = new TagBuilder("a");
                        TagBuilder tag7 = new TagBuilder("a");
                        TagBuilder tag8 = new TagBuilder("a");
                        TagBuilder tag9 = new TagBuilder("a");
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                        PageUrlValues[OnCrossUrlValue] = 1;
                        PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                        PageUrlValues[Sort] = OnCrossPageModel.sort;
                        tag6.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag6.AddCssClass(PageClass);
                            tag6.AddCssClass(PageClassSelected);
                        }
                        tag6.InnerHtml.Append("<<");
                        result2.InnerHtml.AppendHtml(tag6);
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                        PageUrlValues[OnCrossUrlValue] = (OnCrossPageModel.CurrentPage - 1) != 0 ? (OnCrossPageModel.CurrentPage - 1) : 1;
                        PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                        tag7.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag7.AddCssClass(PageClass);
                            tag7.AddCssClass(PageClassSelected);
                        }
                        tag7.InnerHtml.Append("<");
                        result2.InnerHtml.AppendHtml(tag7);
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        List<int> Pages2 = new List<int>();
                        int countPages2 = OnCrossPageModel.CurrentPage;
                        while ((countPages2 >= OnCrossPageModel.CurrentPage - 3 && countPages2 != 0))
                        {
                            Pages2.Add(countPages2);
                            countPages2--;
                        }
                        Pages2.Reverse();
                        int pagesRest2 = 4 - Pages2.Count;
                        for (int i = Pages2[0]; i <= Pages2.Last(); i++)
                        {
                            TagBuilder tag2 = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                            PageUrlValues[OnCrossUrlValue] = i;
                            PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                            PageUrlValues[Sort] = OnCrossPageModel.sort;
                            tag2.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag2.AddCssClass(PageClass);
                                tag2.AddCssClass(i == OnCrossPageModel.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag2.InnerHtml.Append(i.ToString());
                            result2.InnerHtml.AppendHtml(tag2);
                        }
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        for (int i = OnCrossPageModel.CurrentPage + 1; (i <= OnCrossPageModel.CurrentPage + 3 + pagesRest2 && i <= OnCrossPageModel.TotalPages); i++)
                        {
                            TagBuilder tag3 = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                            PageUrlValues[OnCrossUrlValue] = i;
                            PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                            PageUrlValues[Sort] = OnCrossPageModel.sort;
                            tag3.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag3.AddCssClass(PageClass);
                                tag3.AddCssClass(i == OnCrossPageModel.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag3.InnerHtml.Append(i.ToString());
                            result2.InnerHtml.AppendHtml(tag3);
                        }
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                        PageUrlValues[OnCrossUrlValue] = (OnCrossPageModel.CurrentPage + 1) <= OnCrossPageModel.TotalPages ? (OnCrossPageModel.CurrentPage + 1) : OnCrossPageModel.TotalPages;
                        PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                        PageUrlValues[Sort] = OnCrossPageModel.sort;
                        tag8.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag8.AddCssClass(PageClass);
                            tag8.AddCssClass(PageClassSelected);
                        }
                        tag8.InnerHtml.Append(">");
                        result2.InnerHtml.AppendHtml(tag8);
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                        PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.TotalPages;
                        PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.CurrentPage;
                        PageUrlValues[Sort] = OnCrossPageModel.sort;
                        tag9.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag9.AddCssClass(PageClass);
                            tag9.AddCssClass(PageClassSelected);
                        }
                        tag9.InnerHtml.Append(">>");
                        result2.InnerHtml.AppendHtml(tag9);
                        output.Content.AppendHtml(result2.InnerHtml);
                        break; 

                    case "PendingJobs":

                        IUrlHelper urlHelper3 = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result3 = new TagBuilder("div");
                        TagBuilder tag10 = new TagBuilder("a");
                        TagBuilder tag11 = new TagBuilder("a");
                        TagBuilder tag12 = new TagBuilder("a");
                        TagBuilder tag13 = new TagBuilder("a");
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                        PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                        PageUrlValues[PendingToCrossUrlValue] = 1;
                        PageUrlValues[Sort] = PendingToCrossPageModel.sort;
                        tag10.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag10.AddCssClass(PageClass);
                            tag10.AddCssClass(PageClassSelected);
                        }
                        tag10.InnerHtml.Append("<<");
                        result3.InnerHtml.AppendHtml(tag10);
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                        PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                        PageUrlValues[PendingToCrossUrlValue] = (PendingToCrossPageModel.CurrentPage - 1) != 0 ? (PendingToCrossPageModel.CurrentPage - 1) : 1;
                        tag11.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag11.AddCssClass(PageClass);
                            tag11.AddCssClass(PageClassSelected);
                        }
                        tag11.InnerHtml.Append("<");
                        result3.InnerHtml.AppendHtml(tag11);
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        List<int> Pages3 = new List<int>();
                        int countPages3 = PendingToCrossPageModel.CurrentPage;
                        while ((countPages3 >= PendingToCrossPageModel.CurrentPage - 3 && countPages3 != 0))
                        {
                            Pages3.Add(countPages3);
                            countPages3--;
                        }
                        Pages3.Reverse();
                        int pagesRest3 = 4 - Pages3.Count;
                        for (int i = Pages3[0]; i <= Pages3.Last(); i++)
                        {
                            TagBuilder tag2 = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                            PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                            PageUrlValues[PendingToCrossUrlValue] = i;
                            PageUrlValues[Sort] = PendingToCrossPageModel.sort;
                            tag2.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag2.AddCssClass(PageClass);
                                tag2.AddCssClass(i == PendingToCrossPageModel.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag2.InnerHtml.Append(i.ToString());
                            result3.InnerHtml.AppendHtml(tag2);
                        }
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        for (int i = PendingToCrossPageModel.CurrentPage + 1; (i <= PendingToCrossPageModel.CurrentPage + 3 + pagesRest3 && i <= PendingToCrossPageModel.TotalPages); i++)
                        {
                            TagBuilder tag3 = new TagBuilder("a");
                            PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                            PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                            PageUrlValues[PendingToCrossUrlValue] = i;
                            PageUrlValues[Sort] = PendingToCrossPageModel.sort;
                            tag3.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag3.AddCssClass(PageClass);
                                tag3.AddCssClass(i == PendingToCrossPageModel.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag3.InnerHtml.Append(i.ToString());
                            result3.InnerHtml.AppendHtml(tag3);
                        }
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                        PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                        PageUrlValues[PendingToCrossUrlValue] = (PendingToCrossPageModel.CurrentPage + 1) <= PendingToCrossPageModel.TotalPages ? (PendingToCrossPageModel.CurrentPage + 1) : PendingToCrossPageModel.TotalPages;
                        PageUrlValues[Sort] = PendingToCrossPageModel.sort;
                        tag12.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag12.AddCssClass(PageClass);
                            tag12.AddCssClass(PageClassSelected);
                        }
                        tag12.InnerHtml.Append(">");
                        result3.InnerHtml.AppendHtml(tag12);
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[MyJobsUrlValue] = MyJobsPageModel.CurrentPage;
                        PageUrlValues[OnCrossUrlValue] = OnCrossPageModel.CurrentPage;
                        PageUrlValues[PendingToCrossUrlValue] = PendingToCrossPageModel.TotalPages;
                        PageUrlValues[Sort] = PendingToCrossPageModel.sort;
                        tag13.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag13.AddCssClass(PageClass);
                            tag13.AddCssClass(PageClassSelected);
                        }
                        tag13.InnerHtml.Append(">>");
                        result3.InnerHtml.AppendHtml(tag13);
                        output.Content.AppendHtml(result3.InnerHtml);
                        break;
                }

            }
        }
    }

    public class CoolPaginationSteps : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public CoolPaginationSteps(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }



        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public PagingInfo ElmHydroPagingInfo { get; set; }
        public PagingInfo ElmTractionPagingInfo { get; set; }
        public PagingInfo M2000PagingInfo { get; set; }
        public PagingInfo M4000PagingInfo { get; set; }
        public string ElmHydroUrlValue { get; set; }
        public string ElmTractionUrlValue { get; set; }
        public string M2000UrlValue { get; set; }
        public string M4000UrlValue { get; set; }
        public string JobTypeSelected { get; set; }
        public string CurrentModel { get; set; }

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(CurrentModel))
            {
                switch (CurrentModel)
                {
                    case "Hydro":
                        IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result = new TagBuilder("div");
                        TagBuilder tag = new TagBuilder("a");
                        TagBuilder tag1 = new TagBuilder("a");
                        TagBuilder tag4 = new TagBuilder("a");
                        TagBuilder tag5 = new TagBuilder("a");
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = 1;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmHydroPagingInfo.JobTypeName;
                        tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag.AddCssClass(PageClass);
                            tag.AddCssClass(PageClassSelected);
                        }
                        tag.InnerHtml.Append("<<");
                        result.InnerHtml.AppendHtml(tag);
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = (ElmHydroPagingInfo.CurrentPage - 1) != 0 ? (ElmHydroPagingInfo.CurrentPage - 1) : 1;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmHydroPagingInfo.JobTypeName;
                        tag1.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag1.AddCssClass(PageClass);
                            tag1.AddCssClass(PageClassSelected);
                        }
                        tag1.InnerHtml.Append("<");
                        result.InnerHtml.AppendHtml(tag1);
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        List<int> Pages = new List<int>();
                        int countPages = ElmHydroPagingInfo.CurrentPage;
                        while ((countPages >= ElmHydroPagingInfo.CurrentPage - 3 && countPages != 0))
                        {
                            Pages.Add(countPages);
                            countPages--;
                        }
                        Pages.Reverse();
                        int pagesRest = 4 - Pages.Count;
                        for (int i = Pages[0]; i <= Pages.Last(); i++)
                        {
                            TagBuilder tag2 = new TagBuilder("a");
                            PageUrlValues[ElmHydroUrlValue] = i;
                            PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                            PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                            PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                            PageUrlValues[JobTypeSelected] = ElmHydroPagingInfo.JobTypeName;
                            tag2.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag2.AddCssClass(PageClass);
                                tag2.AddCssClass(i == ElmHydroPagingInfo.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag2.InnerHtml.Append(i.ToString());
                            result.InnerHtml.AppendHtml(tag2);
                        }
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        for (int i = ElmHydroPagingInfo.CurrentPage + 1; (i <= ElmHydroPagingInfo.CurrentPage + 3 + pagesRest && i <= ElmHydroPagingInfo.TotalPages); i++)
                        {
                            TagBuilder tag3 = new TagBuilder("a");
                            PageUrlValues[ElmHydroUrlValue] = i;
                            PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                            PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                            PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                            PageUrlValues[JobTypeSelected] = ElmHydroPagingInfo.JobTypeName;
                            tag3.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag3.AddCssClass(PageClass);
                                tag3.AddCssClass(i == ElmHydroPagingInfo.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag3.InnerHtml.Append(i.ToString());
                            result.InnerHtml.AppendHtml(tag3);
                        }
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = (ElmHydroPagingInfo.CurrentPage + 1) <= ElmHydroPagingInfo.TotalPages ? (ElmHydroPagingInfo.CurrentPage + 1) : ElmHydroPagingInfo.TotalPages;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmHydroPagingInfo.JobTypeName;
                        tag4.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag4.AddCssClass(PageClass);
                            tag4.AddCssClass(PageClassSelected);
                        }
                        tag4.InnerHtml.Append(">");
                        result.InnerHtml.AppendHtml(tag4);
                        output.Content.AppendHtml(result.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.TotalPages;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmHydroPagingInfo.JobTypeName;
                        tag5.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag5.AddCssClass(PageClass);
                            tag5.AddCssClass(PageClassSelected);
                        }
                        tag5.InnerHtml.Append(">>");
                        result.InnerHtml.AppendHtml(tag5);
                        output.Content.AppendHtml(result.InnerHtml);
                        break;

                    case "Traction":
                        IUrlHelper urlHelper2 = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result2 = new TagBuilder("div");
                        TagBuilder tag6 = new TagBuilder("a");
                        TagBuilder tag7 = new TagBuilder("a");
                        TagBuilder tag8 = new TagBuilder("a");
                        TagBuilder tag9 = new TagBuilder("a");
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = 1;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmTractionPagingInfo.JobTypeName;
                        tag6.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag6.AddCssClass(PageClass);
                            tag6.AddCssClass(PageClassSelected);
                        }
                        tag6.InnerHtml.Append("<<");
                        result2.InnerHtml.AppendHtml(tag6);
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = (ElmTractionPagingInfo.CurrentPage - 1) != 0 ? (ElmTractionPagingInfo.CurrentPage - 1) : 1;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmTractionPagingInfo.JobTypeName;
                        tag7.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag7.AddCssClass(PageClass);
                            tag7.AddCssClass(PageClassSelected);
                        }
                        tag7.InnerHtml.Append("<");
                        result2.InnerHtml.AppendHtml(tag7);
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        List<int> Pages2 = new List<int>();
                        int countPages2 = ElmTractionPagingInfo.CurrentPage;
                        while ((countPages2 >= ElmTractionPagingInfo.CurrentPage - 3 && countPages2 != 0))
                        {
                            Pages2.Add(countPages2);
                            countPages2--;
                        }
                        Pages2.Reverse();
                        int pagesRest2 = 4 - Pages2.Count;
                        for (int i = Pages2[0]; i <= Pages2.Last(); i++)
                        {
                            TagBuilder tag2 = new TagBuilder("a");
                            PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                            PageUrlValues[ElmTractionUrlValue] = i;
                            PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                            PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                            PageUrlValues[JobTypeSelected] = ElmTractionPagingInfo.JobTypeName;
                            tag2.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag2.AddCssClass(PageClass);
                                tag2.AddCssClass(i == ElmTractionPagingInfo.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag2.InnerHtml.Append(i.ToString());
                            result2.InnerHtml.AppendHtml(tag2);
                        }
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        for (int i = ElmTractionPagingInfo.CurrentPage + 1; (i <= ElmTractionPagingInfo.CurrentPage + 3 + pagesRest2 && i <= ElmTractionPagingInfo.TotalPages); i++)
                        {
                            TagBuilder tag3 = new TagBuilder("a");
                            PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                            PageUrlValues[ElmTractionUrlValue] = i;
                            PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                            PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                            PageUrlValues[JobTypeSelected] = ElmTractionPagingInfo.JobTypeName;
                            tag3.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag3.AddCssClass(PageClass);
                                tag3.AddCssClass(i == ElmTractionPagingInfo.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag3.InnerHtml.Append(i.ToString());
                            result2.InnerHtml.AppendHtml(tag3);
                        }
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = (ElmTractionPagingInfo.CurrentPage + 1) <= ElmTractionPagingInfo.TotalPages ? (ElmTractionPagingInfo.CurrentPage + 1) : ElmTractionPagingInfo.TotalPages;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmTractionPagingInfo.JobTypeName;
                        tag8.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag8.AddCssClass(PageClass);
                            tag8.AddCssClass(PageClassSelected);
                        }
                        tag8.InnerHtml.Append(">");
                        result2.InnerHtml.AppendHtml(tag8);
                        output.Content.AppendHtml(result2.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.TotalPages;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = ElmTractionPagingInfo.JobTypeName;
                        tag9.Attributes["href"] = urlHelper2.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag9.AddCssClass(PageClass);
                            tag9.AddCssClass(PageClassSelected);
                        }
                        tag9.InnerHtml.Append(">>");
                        result2.InnerHtml.AppendHtml(tag9);
                        output.Content.AppendHtml(result2.InnerHtml);
                        break;

                    case "M2000":

                        IUrlHelper urlHelper3 = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result3 = new TagBuilder("div");
                        TagBuilder tag10 = new TagBuilder("a");
                        TagBuilder tag11 = new TagBuilder("a");
                        TagBuilder tag12 = new TagBuilder("a");
                        TagBuilder tag13 = new TagBuilder("a");
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = 1;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = M2000PagingInfo.JobTypeName;
                        tag10.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag10.AddCssClass(PageClass);
                            tag10.AddCssClass(PageClassSelected);
                        }
                        tag10.InnerHtml.Append("<<");
                        result3.InnerHtml.AppendHtml(tag10);
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = (M2000PagingInfo.CurrentPage - 1) != 0 ? (M2000PagingInfo.CurrentPage - 1) : 1;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = M2000PagingInfo.JobTypeName;
                        tag11.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag11.AddCssClass(PageClass);
                            tag11.AddCssClass(PageClassSelected);
                        }
                        tag11.InnerHtml.Append("<");
                        result3.InnerHtml.AppendHtml(tag11);
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        List<int> Pages3 = new List<int>();
                        int countPages3 = M2000PagingInfo.CurrentPage;
                        while ((countPages3 >= M2000PagingInfo.CurrentPage - 3 && countPages3 != 0))
                        {
                            Pages3.Add(countPages3);
                            countPages3--;
                        }
                        Pages3.Reverse();
                        int pagesRest3 = 4 - Pages3.Count;
                        for (int i = Pages3[0]; i <= Pages3.Last(); i++)
                        {
                            TagBuilder tag2 = new TagBuilder("a");
                            PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                            PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                            PageUrlValues[M2000UrlValue] = i;
                            PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                            PageUrlValues[JobTypeSelected] = M2000PagingInfo.JobTypeName;
                            tag2.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag2.AddCssClass(PageClass);
                                tag2.AddCssClass(i == M2000PagingInfo.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag2.InnerHtml.Append(i.ToString());
                            result3.InnerHtml.AppendHtml(tag2);
                        }
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        for (int i = M2000PagingInfo.CurrentPage + 1; (i <= M2000PagingInfo.CurrentPage + 3 + pagesRest3 && i <= M2000PagingInfo.TotalPages); i++)
                        {
                            TagBuilder tag3 = new TagBuilder("a");
                            PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                            PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                            PageUrlValues[M2000UrlValue] = i;
                            PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                            PageUrlValues[JobTypeSelected] = M2000PagingInfo.JobTypeName;
                            tag3.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag3.AddCssClass(PageClass);
                                tag3.AddCssClass(i == M2000PagingInfo.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag3.InnerHtml.Append(i.ToString());
                            result3.InnerHtml.AppendHtml(tag3);
                        }
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = (M2000PagingInfo.CurrentPage + 1) <= M2000PagingInfo.TotalPages ? (M2000PagingInfo.CurrentPage + 1) : M2000PagingInfo.TotalPages;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = M2000PagingInfo.JobTypeName;
                        tag12.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag12.AddCssClass(PageClass);
                            tag12.AddCssClass(PageClassSelected);
                        }
                        tag12.InnerHtml.Append(">");
                        result3.InnerHtml.AppendHtml(tag12);
                        output.Content.AppendHtml(result3.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.TotalPages;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.CurrentPage;
                        PageUrlValues[JobTypeSelected] = M2000PagingInfo.JobTypeName;
                        tag13.Attributes["href"] = urlHelper3.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag13.AddCssClass(PageClass);
                            tag13.AddCssClass(PageClassSelected);
                        }
                        tag13.InnerHtml.Append(">>");
                        result3.InnerHtml.AppendHtml(tag13);
                        output.Content.AppendHtml(result3.InnerHtml);
                        break;

                    case "M4000":

                        IUrlHelper urlHelper4 = urlHelperFactory.GetUrlHelper(ViewContext);
                        TagBuilder result4 = new TagBuilder("div");
                        TagBuilder tag14 = new TagBuilder("a");
                        TagBuilder tag15 = new TagBuilder("a");
                        TagBuilder tag16 = new TagBuilder("a");
                        TagBuilder tag17 = new TagBuilder("a");
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = 1;
                        PageUrlValues[JobTypeSelected] = M4000PagingInfo.JobTypeName;
                        tag14.Attributes["href"] = urlHelper4.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag14.AddCssClass(PageClass);
                            tag14.AddCssClass(PageClassSelected);
                        }
                        tag14.InnerHtml.Append("<<");
                        result4.InnerHtml.AppendHtml(tag14);
                        output.Content.AppendHtml(result4.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = (M4000PagingInfo.CurrentPage - 1) != 0 ? (M4000PagingInfo.CurrentPage - 1) : 1;
                        PageUrlValues[JobTypeSelected] = M4000PagingInfo.JobTypeName;
                        tag15.Attributes["href"] = urlHelper4.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag15.AddCssClass(PageClass);
                            tag15.AddCssClass(PageClassSelected);
                        }
                        tag15.InnerHtml.Append("<");
                        result4.InnerHtml.AppendHtml(tag15);
                        output.Content.AppendHtml(result4.InnerHtml);
                        /*************************************************************************/
                        List<int> Pages4 = new List<int>();
                        int countPages4 = M4000PagingInfo.CurrentPage;
                        while ((countPages4 >= M4000PagingInfo.CurrentPage - 3 && countPages4 != 0))
                        {
                            Pages4.Add(countPages4);
                            countPages4--;
                        }
                        Pages4.Reverse();
                        int pagesRest4 = 4 - Pages4.Count;
                        for (int i = Pages4[0]; i <= Pages4.Last(); i++)
                        {
                            TagBuilder tag2 = new TagBuilder("a");
                            PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                            PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                            PageUrlValues[M2000UrlValue] =  M2000PagingInfo.CurrentPage;
                            PageUrlValues[M4000UrlValue] = i;
                            PageUrlValues[JobTypeSelected] = M4000PagingInfo.JobTypeName;
                            tag2.Attributes["href"] = urlHelper4.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag2.AddCssClass(PageClass);
                                tag2.AddCssClass(i == M4000PagingInfo.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag2.InnerHtml.Append(i.ToString());
                            result4.InnerHtml.AppendHtml(tag2);
                        }
                        output.Content.AppendHtml(result4.InnerHtml);
                        /*************************************************************************/
                        for (int i = M4000PagingInfo.CurrentPage + 1; (i <= M4000PagingInfo.CurrentPage + 3 + pagesRest4 && i <= M4000PagingInfo.TotalPages); i++)
                        {
                            TagBuilder tag3 = new TagBuilder("a");
                            PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                            PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                            PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                            PageUrlValues[M4000UrlValue] = i;
                            PageUrlValues[JobTypeSelected] = M4000PagingInfo.JobTypeName;
                            tag3.Attributes["href"] = urlHelper4.Action(PageAction, PageUrlValues);
                            if (PageClassesEnabled)
                            {
                                tag3.AddCssClass(PageClass);
                                tag3.AddCssClass(i == M4000PagingInfo.CurrentPage
                                ? "btn-info" : PageClassNormal);
                            }
                            tag3.InnerHtml.Append(i.ToString());
                            result4.InnerHtml.AppendHtml(tag3);
                        }
                        output.Content.AppendHtml(result4.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] = M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = (M4000PagingInfo.CurrentPage + 1) <= M4000PagingInfo.TotalPages ? (M4000PagingInfo.CurrentPage + 1) : M4000PagingInfo.TotalPages;
                        PageUrlValues[JobTypeSelected] = M4000PagingInfo.JobTypeName;
                        tag16.Attributes["href"] = urlHelper4.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag16.AddCssClass(PageClass);
                            tag16.AddCssClass(PageClassSelected);
                        }
                        tag16.InnerHtml.Append(">");
                        result4.InnerHtml.AppendHtml(tag16);
                        output.Content.AppendHtml(result4.InnerHtml);
                        /*************************************************************************/
                        PageUrlValues[ElmHydroUrlValue] = ElmHydroPagingInfo.CurrentPage;
                        PageUrlValues[ElmTractionUrlValue] = ElmTractionPagingInfo.CurrentPage;
                        PageUrlValues[M2000UrlValue] =  M2000PagingInfo.CurrentPage;
                        PageUrlValues[M4000UrlValue] = M4000PagingInfo.TotalPages;
                        PageUrlValues[JobTypeSelected] = M4000PagingInfo.JobTypeName;
                        tag17.Attributes["href"] = urlHelper4.Action(PageAction, PageUrlValues);
                        if (PageClassesEnabled)
                        {
                            tag17.AddCssClass(PageClass);
                            tag17.AddCssClass(PageClassSelected);
                        }
                        tag17.InnerHtml.Append(">>");
                        result4.InnerHtml.AppendHtml(tag17);
                        output.Content.AppendHtml(result4.InnerHtml);
                        break;
                }

            }
        }
    }

    public class MySelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public MySelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = null;
            m_tag.InnerHtml.Append("Please select one");
            result.InnerHtml.AppendHtml(m_tag);
            foreach (string style in itemsrepository.DoorOperators.Select(d => d.Style).Distinct())
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = style;
                if (style == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(style);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class CustomSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;
        private IJobRepository jobrepository;
        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public CustomSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo, IJobRepository jobrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
            jobrepository = jobrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        private IQueryable<string> CaseFor(string value)
        {
            switch (value)
            {
                case "JobType":
                    return itemsrepository.JobTypes.OrderBy(s => s.Name).Select(d => d.Name).Distinct();
                case "Style":
                    return itemsrepository.DoorOperators.OrderBy(s => s.Name).Select(d => d.Style).Distinct();
                case "SwitchStyle":
                    return new List<string> { "2-Position", "3-Position" }.AsQueryable();
                case "Stage":
                    return new List<string> { "Beginning", "Program", "Logic", "Ending", "Complete" }.AsQueryable();
                case "Label":
                    return new List<string> { "-", "A", "B", "C", "D", "E", "F", "G", "H" }.AsQueryable();
                case "Station":
                    return new List<string> { "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "S10", "S11", "S12", "ELEM1", "ELEM2" }.AsQueryable();
                case "CarCode":
                    return new List<string> { "IMonitor", "Key" }.AsQueryable();
                case "SPH":
                    return new List<string> { "80", "120" }.AsQueryable();
                case "INA":
                    return new List<string> { "Top & Bottom", "Top","bottom", "None"}.AsQueryable();
                case "LoadWeigher":
                    return new List<string> {"Discrete", "EMCO" }.AsQueryable();
                case "MachineLocation":
                    return new List<string> { "Overhead", "Basement"}.AsQueryable();
                case "VVVF":
                    return new List<string> { "HPV900", "KEB","Yaskawa" }.AsQueryable();
                case "Contact":
                    return new List<string> { "N/C", "N/O" }.AsQueryable();
                case "Starter":
                    return new List<string> { "Siemens SS : 6/12", "Siemens SS : 3/9", "Sprecher SS : 6/12", "Sprecher SS : 3/9", "ATL", "YD" }.AsQueryable();
                case "StarterForElements":
                    return new List<string> { "Siemens SS : 6/12", "Siemens SS : 3/9", "Sprecher SS : 6/12", "Sprecher SS : 3/9"}.AsQueryable();
                case "Status":
                    return new List<string> { "Working on it", "Cross Approval Pending", "On Cross Approval", "Cross Approval Complete" }.AsQueryable();
                case "MorningStatus":
                    return new List<string> { "Not reviewed", "Working on it",  "Missing Data", "On Sales", "Cross Approval", "Released" }.AsQueryable();
                case "Valve Brand":
                    List<string> ValveInHydro = jobrepository.HydroSpecifics.Where(d => d.ValveBrand != null).Select(d => d.ValveBrand).Distinct().ToList();
                    List<string> ValveList = new List<string> { "Blain", "Bucher", "EECO", "Maxton", "TKE | Dover", "Other" };
                    if (ValveInHydro.Count > 0) ValveList.AddRange(ValveInHydro);
                    return ValveList.Distinct().AsQueryable();
                case "Battery Brand":
                    List<string> BatteryInHydro = jobrepository.HydroSpecifics.Where(d => d.BatteryBrand != null).Select(d => d.BatteryBrand).Distinct().ToList();
                    List<string> BatteryList = new List<string> { "HAPS", "R&R", "Other" };
                    if (BatteryInHydro.Count > 0) BatteryList.AddRange(BatteryInHydro);

                    return BatteryList.Distinct().AsQueryable();
                default:
                    return new List<string> { "Beginning", "Program", "Logic", "Ending", "Complete" }.AsQueryable();
                    return itemsrepository.JobTypes.OrderBy(s => s.Name).Select(d => d.Name).Distinct();
            }
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            if(SelectFor == "LoadWeigher")
            {
                m_tag.Attributes["value"] = "N/A";
                m_tag.InnerHtml.Append("N/A");
            }else
            {
                m_tag.Attributes["value"] = "";
                m_tag.InnerHtml.Append("Please select one");
            }
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> options2 = CaseFor(SelectFor);
            foreach (string option in options2)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = option.ToString();
                if (!string.IsNullOrEmpty(SelectedValue))
                {
                    if (option.ToString() == SelectedValue)
                    {
                        tag.Attributes["selected"] = "selected";
                    }
                }
                tag.InnerHtml.Append(option.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class IndicatorsTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public IndicatorsTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        private IQueryable<string> CaseFor(string value)
        {
            switch (value)
            {
                case "Volts_Value":
                    return new List<string> { "24", "48", "120", "Other" }.AsQueryable();
                case "Volts_Type":
                    return new List<string> { "AC", "DC" }.AsQueryable();
                case "CallType":
                    return new List<string> { "Incandescent", "LED" }.AsQueryable();
                case "EPContact":
                    return new List<string> { "NC", "NO", }.AsQueryable();
                case "EPCars":
                    return new List<string> { "1", "2", "3", "4" }.AsQueryable();
                case "PIDriver":
                    return new List<string> { "CE Electronics", "Emotive", "Discrete" }.AsQueryable();
                case "CarPIDiscreteType":
                    return new List<string> { "Binary 00", "Binary 01", "Multi-light", "One line per floor", }.AsQueryable();
                case "Monitoring":
                    return new List<string> { "IDS Liftnet","MView Interface & IMonitor Complete","MView Interface & IMonitor Interface", "MView Complete", "MView Interface", "IMonitor Complete", "IMonitor Interface",
                        "MView Complete & IMonitor Complete","MView Complete & IMonitor Interface" }.AsQueryable();
                case "PIType":
                    return new List<string> { "Chime", "Gong" }.AsQueryable();
                case "AccessSWLocation":
                    return new List<string> { "Front", "Rear" }.AsQueryable();
                case "INCPButtons":
                    return new List<string> { "Using top/bottom car calls", "Using up/down buttons" }.AsQueryable();
                case "JobType":
                    return new List<string> { "Simplex", "Duplex", "Group" }.AsQueryable();
                case "JobType2":
                    return new List<string> { "Selective Collective", "SAPB Single Automatic Pushbutton", "SBC Single Button Collective", "Duplex Operation", "Group Operation" }.AsQueryable();
                default:
                    return new List<string> { "Chime", "Gong" }.AsQueryable();
            }
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append(" --- Please Select one--- ");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> options = CaseFor(SelectFor);
            foreach (string option in options)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = option;
                if (option == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(option);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class VoltsTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public VoltsTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        private IQueryable<int> CaseFor(string value)
        {
            switch (value)
            {
                case "Volts_Value":
                    return new List<int> { 24, 48, 120 }.AsQueryable();
                default:
                    return new List<int> { 24, 48, 120 }.AsQueryable();
            }
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("---");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<int> options = CaseFor(SelectFor);
            foreach (int option in options)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = option.ToString();
                if (option == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(option.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class CitySelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public CitySelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select a City");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<City> city = itemsrepository.Cities.AsQueryable();
            if (SelectedValue != 0)
            {
                int SelectedStateInCityID = itemsrepository.Cities.FirstOrDefault(n => n.CityID == SelectedValue).StateID;
                city = itemsrepository.Cities.Where(m => m.StateID == SelectedStateInCityID).OrderBy(s => s.Name);
            }
            foreach (City cities in city)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = cities.CityID.ToString();
                if (cities.CityID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(cities.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class StateSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public StateSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("-- Select State --");
            result.InnerHtml.AppendHtml(m_tag);
            int stateID = 0;
            IQueryable<State> state = itemsrepository.States.AsQueryable();
            if (SelectedValue != 0)
            {
                City selectedCity = itemsrepository.Cities.FirstOrDefault(c => c.CityID == SelectedValue);
                State selectedState = itemsrepository.States.FirstOrDefault(s => s.StateID == selectedCity.StateID);
                stateID = selectedCity.StateID;
                state = itemsrepository.States.Where(m => m.CountryID == selectedState.CountryID).OrderBy(s => s.Name).AsQueryable();
            }
            foreach (State states in state)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = states.StateID.ToString();
                if (states.StateID == stateID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(states.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class CountrySelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public CountrySelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        public int SelectedDefaultValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("-- Select Country --");
            result.InnerHtml.AppendHtml(m_tag);
            int countryID = 0;
            if (SelectedValue != 0)
            {
                City selectedCity = itemsrepository.Cities.FirstOrDefault(c => c.CityID == SelectedValue);
                State selectedState = itemsrepository.States.FirstOrDefault(s => s.StateID == selectedCity.StateID);
                countryID = selectedState.CountryID;
            }
            if (SelectedDefaultValue != 0)
            {
                Country selectedCountry = itemsrepository.Countries.FirstOrDefault(s => s.CountryID == SelectedDefaultValue);
                countryID = selectedCountry.CountryID;
            }
            IQueryable<Country> country = itemsrepository.Countries.OrderBy(s => s.Name).AsQueryable();
            foreach (Country countries in country)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = countries.CountryID.ToString();
                if (countries.CountryID == countryID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(countries.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class FireCodeSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public FireCodeSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }
        public int SelectedCurrentFireCode { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (SelectedCurrentFireCode != 0)
            {
                int FireCodeOnCity = itemsrepository.Cities.FirstOrDefault(m => m.CityID == SelectedCurrentFireCode).FirecodeID;
                SelectedValue = FireCodeOnCity;
            }
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select a FireCode");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<FireCode> fireCode = itemsrepository.FireCodes.OrderBy(s => s.Name).AsQueryable();
            foreach (FireCode fireCodes in fireCode)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = fireCodes.FireCodeID.ToString();
                if (fireCodes.FireCodeID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(fireCodes.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class FireCodeInJobsSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public FireCodeInJobsSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select a FireCode");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<FireCode> fireCode = itemsrepository.FireCodes.FromSql("select * from dbo.FireCodes " +
                "Where dbo.FireCodes.FireCodeID IN (Select dbo.Jobs.FireCodeID From dbo.Jobs)").OrderBy(s => s.Name).AsQueryable();
            foreach (FireCode fireCodes in fireCode)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = fireCodes.FireCodeID.ToString();
                if (fireCodes.FireCodeID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(fireCodes.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class LandingSysSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public LandingSysSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        public int SelectFor { get; set; }


        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {


            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select one");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<LandingSystem> landigSystems = itemsrepository.LandingSystems.OrderBy(s => s.Name).AsQueryable();
            if (SelectFor != 0)
            {
                var jobtypeName = itemsrepository.JobTypes.FirstOrDefault(m => m.JobTypeID == SelectFor).Name;
                landigSystems = itemsrepository.LandingSystems.OrderBy(s => s.Name).Where(m => m.UsedIn == jobtypeName).AsQueryable();
            }
            foreach (LandingSystem landingSys in landigSystems)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = landingSys.LandingSystemID.ToString();
                if (landingSys.LandingSystemID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(landingSys.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class DoorOperatorSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public DoorOperatorSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select one");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<DoorOperator> doors = itemsrepository.DoorOperators.AsQueryable();
            if (SelectedValue != 0)
            {
                string AuxDoor = itemsrepository.DoorOperators.FirstOrDefault(m => m.DoorOperatorID == SelectedValue).Brand;
                doors = itemsrepository.DoorOperators.Where(m => m.Brand == AuxDoor).OrderBy(s => s.Name).AsQueryable();
            }
            foreach (DoorOperator uniqueDoor in doors)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = uniqueDoor.DoorOperatorID.ToString();
                if (uniqueDoor.DoorOperatorID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(uniqueDoor.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class DoorOperatorStyleSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public DoorOperatorStyleSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }
        public bool isElement { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select a Style");
            if (!isElement) result.InnerHtml.AppendHtml(m_tag);
            int doorOperatorID = 0;
            string doorStyle = "";
            if (SelectedValue != 0)
            {
                DoorOperator selectedDoor = itemsrepository.DoorOperators.FirstOrDefault(c => c.DoorOperatorID == SelectedValue);
                doorOperatorID = selectedDoor.DoorOperatorID;
                doorStyle = selectedDoor.Style;
            }
            IQueryable<DoorOperator> door = itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where dbo.DoorOperators.DoorOperatorID " +
                "in (Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Style)").AsQueryable();
            if (!isElement)
            {
                foreach (DoorOperator doors in door)
                {
                    TagBuilder tag = new TagBuilder("option");
                    tag.Attributes["value"] = doors.Style;
                    if (doors.Style == doorStyle)
                    {
                        tag.Attributes["selected"] = doors.Style;
                    }
                    tag.InnerHtml.Append(doors.Style);
                    result.InnerHtml.AppendHtml(tag);
                }
                output.Content.AppendHtml(result.InnerHtml);
                if (IsDisabled)
                {
                    var d = new TagHelperAttribute("disabled", "disabled");
                    output.Attributes.Add(d);
                }
                base.Process(context, output);
            }
            else
            {
                DoorOperator doorUnique = itemsrepository.DoorOperators.FirstOrDefault(m => m.Style == "Automatic");

                    TagBuilder tag = new TagBuilder("option");
                    tag.Attributes["value"] = doorUnique.Style;
                    if (doorUnique.Style == doorStyle)
                    {
                        tag.Attributes["selected"] = doorUnique.Style;
                    }
                    tag.InnerHtml.Append(doorUnique.Style);
                    result.InnerHtml.AppendHtml(tag);
                output.Content.AppendHtml(result.InnerHtml);
                if (IsDisabled)
                {
                    var d = new TagHelperAttribute("disabled", "disabled");
                    output.Attributes.Add(d);
                }
                base.Process(context, output);
            }
            
        }
    }

    public class DoorOperatorBrandSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public DoorOperatorBrandSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select a Brand");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<DoorOperator> door = itemsrepository.DoorOperators.AsQueryable();
            int doorOperatorID = 0;
            string doorBrand = "";
            if (SelectedValue != 0)
            {
                DoorOperator selectedDoor = itemsrepository.DoorOperators.FirstOrDefault(c => c.DoorOperatorID == SelectedValue);
                doorOperatorID = selectedDoor.DoorOperatorID;
                doorBrand = selectedDoor.Brand;
                door = itemsrepository.DoorOperators.FromSql("select * from dbo.DoorOperators where Style = {0} AND dbo.DoorOperators.DoorOperatorID in " +
                "(Select max(dbo.DoorOperators.DoorOperatorID) FROM dbo.DoorOperators group by dbo.DoorOperators.Brand)", selectedDoor.Style).OrderBy(s => s.Brand).AsQueryable();
            }
            foreach (DoorOperator doors in door)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = doors.DoorOperatorID.ToString();
                if (doors.Brand == doorBrand)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(doors.Brand);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class JobTypeSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public JobTypeSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select a JobType");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<JobType> jobType = itemsrepository.JobTypes.OrderBy(s => s.Name).AsQueryable();
            foreach (JobType jobTypes in jobType)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = jobTypes.JobTypeID.ToString();
                if (jobTypes.JobTypeID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(jobTypes.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class JobTypeInJobSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public JobTypeInJobSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select a JobType");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<JobType> jobType = itemsrepository.JobTypes.FromSql("select * from dbo.JobTypes " +
                "Where dbo.JobTypes.JobTypeID IN (Select dbo.Jobs.JobTypeID From dbo.Jobs)").OrderBy(s => s.Name).AsQueryable();
            foreach (JobType jobTypes in jobType)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = jobTypes.JobTypeID.ToString();
                if (jobTypes.JobTypeID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(jobTypes.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class JobTypeAddInJobSelectTagHelper : TagHelper
    {
        private IJobRepository ijobrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public JobTypeAddInJobSelectTagHelper(IUrlHelperFactory helperFactory, IJobRepository jobRepository)
        {
            urlHelperFactory = helperFactory;
            ijobrepository = jobRepository;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Select a type of operation");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> jobTypeAdd = ijobrepository.JobsExtensions.OrderBy(s => s.JobTypeAdd).Select(j => j.JobTypeAdd).Distinct().AsQueryable();
            foreach (string jobTypesAdd in jobTypeAdd)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = jobTypesAdd;
                if (jobTypesAdd == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(jobTypesAdd);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class JobTypeMainiInJobSelectTagHelper : TagHelper
    {
        private IJobRepository ijobrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public JobTypeMainiInJobSelectTagHelper(IUrlHelperFactory helperFactory, IJobRepository jobRepository)
        {
            urlHelperFactory = helperFactory;
            ijobrepository = jobRepository;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Select a Type of operation #2");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> jobTypeMain = ijobrepository.JobsExtensions.OrderBy(s => s.JobTypeMain).Select(j => j.JobTypeMain).Distinct().AsQueryable();
            foreach (string jobTypesMain in jobTypeMain)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = jobTypesMain;
                if (jobTypesMain == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(jobTypesMain);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class ValveBrandniInJobSelectTagHelper : TagHelper
    {
        private IJobRepository ijobrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public ValveBrandniInJobSelectTagHelper(IUrlHelperFactory helperFactory, IJobRepository jobRepository)
        {
            urlHelperFactory = helperFactory;
            ijobrepository = jobRepository;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Select a Valve Brand");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> valveBrand = ijobrepository.HydroSpecifics.Select(h => h.ValveBrand).Distinct().AsQueryable();
            foreach (string valveBrands in valveBrand)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = valveBrands;
                if (valveBrands == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(valveBrands);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class LandingSysInJobSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public LandingSysInJobSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select a Landing System");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<LandingSystem> landingSys = itemsrepository.LandingSystems.FromSql("select * from dbo.LandingSystems " +
                "Where dbo.LandingSystems.LandingSystemID IN (Select dbo.HoistWayDatas.LandingSystemID From dbo.HoistWayDatas)").OrderBy(s => s.Name).AsQueryable();
            foreach (LandingSystem landingsSys in landingSys)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = landingsSys.LandingSystemID.ToString();
                if (landingsSys.LandingSystemID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(landingsSys.Name.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);

            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class StarterTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public StarterTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        private IQueryable<string> CaseFor(string value)
        {
            switch (value)
            {
                case "Volts":
                    return new List<string> { "208", "240", "380/415", "460", "480", "575" }.AsQueryable();
                case "Brand":
                    return new List<string> { "Siemens", "Sprecher Schuh" }.AsQueryable();
                case "Type":
                    return new List<string> { "ATL", "YD", "3/9", "6/12" }.AsQueryable();
                case "OverloadTable":
                    return new List<string> { "1", "2", "3", "4", "5", "N/A" }.AsQueryable();
                default:
                    return new List<string> { "Error" }.AsQueryable();
            }
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Select a option");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> options = CaseFor(SelectFor);
            foreach (string option in options)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = option;
                if (option == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(option);
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons1SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons1SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public int SelectedValue { get; set; }
        public int SelectedInR3Value { get; set; }
        public int SelectedInR2Value { get; set; }
        public int SelectedInR4Value { get; set; }
        public int SelectedInR5Value { get; set; }


        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("Select a Reason 1");
            result.InnerHtml.AppendHtml(m_tag);
            int reason1ID = 0;
            if (SelectedInR5Value != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedInR5Value);
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == selectedR5.Reason4ID);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                Reason1 selectedR1 = repository.Reasons1.FirstOrDefault(c => c.Reason1ID == selectedR2.Reason1ID);
                reason1ID = selectedR1.Reason1ID;
            }
            else if (SelectedInR4Value != 0)
            {
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == SelectedInR4Value);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                Reason1 selectedR1 = repository.Reasons1.FirstOrDefault(c => c.Reason1ID == selectedR2.Reason1ID);
                reason1ID = selectedR1.Reason1ID;
            }
            else if (SelectedInR3Value != 0)
            {
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == SelectedInR3Value);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                Reason1 selectedR1 = repository.Reasons1.FirstOrDefault(c => c.Reason1ID == selectedR2.Reason1ID);
                reason1ID = selectedR1.Reason1ID;
            }
            else if (SelectedInR2Value != 0)
            {
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == SelectedInR2Value);
                Reason1 selectedR1 = repository.Reasons1.FirstOrDefault(c => c.Reason1ID == selectedR2.Reason1ID);
                reason1ID = selectedR1.Reason1ID;
            }
            else
            {
                reason1ID = SelectedValue;
            }
            IQueryable<Reason1> reason1s = repository.Reasons1.Where(m => m.Description != "-").AsQueryable();
            foreach (Reason1 item in reason1s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason1ID.ToString();
                if (item.Reason1ID == reason1ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons2SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons2SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedInR5Value { get; set; }
        public int SelectedInR4Value { get; set; }
        public int SelectedInR3Value { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("Select a Reason 2");
            result.InnerHtml.AppendHtml(m_tag);
            int reason2ID = 0;
            if (SelectedInR5Value != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedInR5Value);
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == selectedR5.Reason4ID);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                reason2ID = selectedR2.Reason2ID;
            }
            if (SelectedInR4Value != 0)
            {
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == SelectedInR4Value);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                reason2ID = selectedR2.Reason2ID;
            }
            if (SelectedInR3Value != 0)
            {
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == SelectedInR3Value);
                Reason2 selectedR2 = repository.Reasons2.FirstOrDefault(c => c.Reason2ID == selectedR3.Reason2ID);
                reason2ID = selectedR2.Reason2ID;
            }
            IQueryable<Reason2> reason2s = repository.Reasons2.Where(m => m.Description != "-").AsQueryable();
            foreach (Reason2 item in reason2s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason2ID.ToString();
                if (item.Reason2ID == reason2ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons3SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons3SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedInR4Value { get; set; }
        public int SelectedInR5Value { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("Select a Reason 3");
            result.InnerHtml.AppendHtml(m_tag);
            int reason3ID = 0;
            if (SelectedInR5Value != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedInR5Value);
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == selectedR5.Reason4ID);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                reason3ID = selectedR3.Reason3ID;
            }
            if (SelectedInR4Value != 0)
            {
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == SelectedInR4Value);
                Reason3 selectedR3 = repository.Reasons3.FirstOrDefault(c => c.Reason3ID == selectedR4.Reason3ID);
                reason3ID = selectedR3.Reason3ID;
            }
            IQueryable<Reason3> reason3s = repository.Reasons3.Where(m => m.Description != "-").AsQueryable();
            foreach (Reason3 item in reason3s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason3ID.ToString();
                if (item.Reason3ID == reason3ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons4SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons4SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedInR5Value { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("Select a Reason 4");
            result.InnerHtml.AppendHtml(m_tag);
            int reason4ID = 0;
            if (SelectedInR5Value != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedInR5Value);
                Reason4 selectedR4 = repository.Reasons4.FirstOrDefault(c => c.Reason4ID == selectedR5.Reason4ID);
                reason4ID = selectedR4.Reason4ID;
            }
            IQueryable<Reason4> reason4s = repository.Reasons4.Where(m => m.Description != "-").AsQueryable();
            foreach (Reason4 item in reason4s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason4ID.ToString();
                if (item.Reason4ID == reason4ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Reasons5SelectTagHelper : TagHelper
    {
        private ITestingRepository repository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public Reasons5SelectTagHelper(IUrlHelperFactory helperFactory, ITestingRepository repo)
        {
            urlHelperFactory = helperFactory;
            repository = repo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "default";
            m_tag.InnerHtml.Append("Select a Reason 5");
            result.InnerHtml.AppendHtml(m_tag);
            int reason5ID = 0;
            if (SelectedValue != 0)
            {
                Reason5 selectedR5 = repository.Reasons5.FirstOrDefault(c => c.Reason5ID == SelectedValue);
                reason5ID = selectedR5.Reason5ID;
            }
            IQueryable<Reason5> reason5s = repository.Reasons5.Where(m => m.Description != "-").AsQueryable();
            foreach (Reason5 item in reason5s)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = item.Reason5ID.ToString();
                if (item.Reason5ID == reason5ID)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(item.Description.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class Trigger2SelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public string SelectFor { get; set; }

        public Trigger2SelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
        }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        private IQueryable<string> CaseFor(string value)
        {
            switch (value)
            {
                case "TriggerFeature":
                    return new List<string> {"Overlay", "Group","PC de Cliente", "Brake Coil Voltage > 10","EMBrake Module","EMCO Board","R6 Regen Unit","Local","Short Floor"
                                            ,"Custom","MRL","CTL2","Tarjeta CPI Incluida","Door Control en Cartop","Canada","Ontario","Manual Doors","Duplex","Serial Halls Calls"
                                            ,"Edge-LS","Rail-LS", "mView","iMonitor","HAPS Battery","2+ Starters", "MOD Door Operator"}.AsQueryable();
                case "TriggerCustom":
                    return new List<string> { "Contractor", "Fire Code", "City", "VCI", "Valve Brand", "Switch Style", "Landing System", "State" }.AsQueryable();
                default:
                    return new List<string> {"Overlay", "Group","PC de Cliente", "Brake Coil Voltage > 10","EMBrake Module","EMCO Board","R6 Regen Unit","Local","ShortFloor"
                                            ,"Custom","MRL","CTL2","Tarjeta CPI Incluida","Door Control en Cartop","Canada","Ontario","Manual Doors","Duplex","Serial Halls Calls"
                                            ,"Edge-LS","Rail-LS", "mView","iMonitor","HAPS Battery","2+ Starters", "MOD Door Operator"}.AsQueryable();
            }
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string SelectedValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select one");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<string> options2 = CaseFor(SelectFor);
            foreach (string option in options2)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = option.ToString();
                if (option.ToString() == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(option.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class UserSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;

        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        private UserManager<AppUser> userManager;

        public UserSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo, UserManager<AppUser> userMrg)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
            userManager = userMrg;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }

        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select one");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<AppUser> users = userManager.Users.AsQueryable();
            foreach (AppUser user in users)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = user.EngID.ToString();
                if (user.EngID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(user.FullName.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

    public class StationSelectTagHelper : TagHelper
    {
        private IItemRepository itemsrepository;
        private ITestingRepository testingRepository;


        private IUrlHelperFactory urlHelperFactory;

        public ModelExpression AspFor { get; set; }

        public StationSelectTagHelper(IUrlHelperFactory helperFactory, IItemRepository itemsrepo, ITestingRepository testRepo)
        {
            urlHelperFactory = helperFactory;
            itemsrepository = itemsrepo;
            testingRepository = testRepo;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public int SelectedValue { get; set; }
        public int SelectFor { get; set; }


        [HtmlAttributeName("asp-is-disabled")]
        public bool IsDisabled { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "select";
            TagBuilder result = new TagBuilder("select");
            string name = this.AspFor.Name;
            if (!String.IsNullOrEmpty(name))
            {
                output.Attributes.Add("id", name);
                output.Attributes.Add("name", name);
            }
            TagBuilder m_tag = new TagBuilder("option");
            m_tag.Attributes["value"] = "";
            m_tag.InnerHtml.Append("Please select one");
            result.InnerHtml.AppendHtml(m_tag);
            IQueryable<Station> stations = testingRepository.Stations.Where(m => m.StationID != 0).OrderBy(s => s.Label).AsQueryable();
            if (SelectFor != 0) stations = testingRepository.Stations.OrderBy(s => s.Label).Where(m => m.JobTypeID == SelectFor && m.StationID != 0).AsQueryable();
            foreach (Station station in stations)
            {
                TagBuilder tag = new TagBuilder("option");
                tag.Attributes["value"] = station.StationID.ToString();
                if (station.StationID == SelectedValue)
                {
                    tag.Attributes["selected"] = "selected";
                }
                tag.InnerHtml.Append(station.Label.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
            if (IsDisabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            base.Process(context, output);
        }
    }

}
