// Copyright (c) 2016, SolidCP
// SolidCP is distributed under the Creative Commons Share-alike license
// 
// SolidCP is a fork of WebsitePanel:
// Copyright (c) 2015, Outercurve Foundation.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must  retain  the  above copyright notice, this
//   list of conditions and the following disclaimer.
//
// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  the  Outercurve Foundation  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

namespace SolidCP.Portal
{
    public partial class SpaceNestedSpaces : SolidCPModuleBase
    {
        private int columChangedDate = 0; 
        protected void Page_Load(object sender, EventArgs e)
        {
            // set display preferences
            gvPackages.PageSize = UsersHelper.GetDisplayItemsPerPage();

            if (!IsPostBack)
            {
                searchBox.AddCriteria("Username", GetLocalizedString("SearchField.Username"));
                searchBox.AddCriteria("FullName", GetLocalizedString("SearchField.Name"));
                searchBox.AddCriteria("Email", GetLocalizedString("SearchField.EMail"));

                // set inital controls state from request
                if (Request["FilterColumn"] != null)
                    searchBox.FilterColumn = Request["FilterColumn"];
                if (Request["FilterValue"] != null)
                    searchBox.FilterValue = Request["FilterValue"];
                if (Request["StatusID"] != null)
                    Utils.SelectListItem(ddlStatus, Request["StatusID"]);
            }
            gvPackages.Columns[columChangedDate].Visible = false;
            //if (ddlStatus.SelectedValue != "1")
            //gvPackages.Columns[gvPackages.Columns.Count - 2].Visible = true;
            if (ddlStatus.SelectedItem.Value != "1")
                gvPackages.Columns[columChangedDate].Visible = true;

            searchBox.AjaxData = this.GetSearchBoxAjaxData();
        }

        public string GetUserHomePageUrl(int userId)
        {
            return PortalUtils.GetUserHomePageUrl(userId);
        }

        public string GetSpaceHomePageUrl(int spaceId)
        {
            return PortalUtils.GetSpaceHomePageUrl(spaceId);
        }

        public string GetNestedSpacesPageUrl(string parameterName, string parameterValue)
        {
            return NavigateURL(PortalUtils.SPACE_ID_PARAM, PanelSecurity.PackageId.ToString(),
                parameterName + "=" + parameterValue);
        }

        protected void odsNestedPackages_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                ProcessException(e.Exception.InnerException);
                //this.DisableControls = true;
                e.ExceptionHandled = true;
            }
        }

        public string GetSearchBoxAjaxData()
        {
            StringBuilder res = new StringBuilder();
            res.Append("PagedStored: 'NestedPackages'");
            res.Append(", RedirectUrl: '" + NavigateURL(PortalUtils.SPACE_ID_PARAM, "{0}").Substring(2) + "'");

            res.Append(", PackageID: " + (String.IsNullOrEmpty(Request["SpaceID"]) ? "0" : Request["SpaceID"]));
            res.Append(", StatusID: $('#" + ddlStatus.ClientID + "').val()");
            res.Append(", PlanID: " + (String.IsNullOrEmpty(Request["PlanID"]) ? "0" : Request["PlanID"]));
            res.Append(", ServerID: " + (String.IsNullOrEmpty(Request["ServerID"]) ? "0" : Request["ServerID"]));
            return res.ToString();
        }

        protected void gvPackages_DataBound(object sender, EventArgs e)
        {
            if (Request["StatusID"] == "1")
            {
                gvPackages.Columns[columChangedDate].Visible = false;
            }                
        }

        protected void gvPackages_Init(object sender, EventArgs e)
        {
            for (int i = 0; i < gvPackages.Columns.Count; i++) //get Index of Column gvPackagesStatusIDchangeDate
            {
                if (gvPackages.Columns[i].HeaderText == "gvPackagesStatusIDchangeDate")
                {
                    columChangedDate = i;
                    break;
                }
            }
        }
    }
}
