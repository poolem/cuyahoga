using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Cuyahoga.Core;
using Cuyahoga.Core.DAL;
using Cuyahoga.Core.Collections;

namespace Cuyahoga.Web.Admin
{
	/// <summary>
	/// Summary description for Roles.
	/// </summary>
	public class Roles : Cuyahoga.Web.Admin.UI.AdminBasePage
	{
		protected System.Web.UI.WebControls.Button btnNew;
		protected System.Web.UI.WebControls.Repeater rptRoles;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			this.Title = "Roles";
			if (! this.IsPostBack)
			{
				BindRoles();
			}
		}

		private void BindRoles()
		{
			RoleCollection roles = new RoleCollection();
			CmsDataFactory.GetInstance().GetAllRoles(roles);
			this.rptRoles.DataSource = roles;
			this.rptRoles.DataBind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.rptRoles.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.rptRoles_ItemDataBound);
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void rptRoles_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			Role role = e.Item.DataItem as Role;
			if (role != null)
			{
				// Permissions
				Label lblPermissions = (Label)e.Item.FindControl("lblPermissions");
				lblPermissions.Text = role.PermissionsString;
				
				HyperLink hplEdit = (HyperLink)e.Item.FindControl("hplEdit");

				// HACK: as long as ~/ doesn't work properly in mono we have to use a relative path from the Controls
				// directory due to the template construction.
				hplEdit.NavigateUrl = String.Format("../RoleEdit.aspx?RoleId={0}", role.Id);
			}
		}

		private void btnNew_Click(object sender, System.EventArgs e)
		{
			Context.Response.Redirect("RoleEdit.aspx?RoleId=-1");
		}
	}
}
