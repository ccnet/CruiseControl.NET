using System;
using System.Collections;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard
{
	public class Controller : Page
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl ParentControl;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;

		private void Page_Load(object sender, EventArgs e)
		{
			RequestController controller = new RequestController(new ConfiguredActionFactory(new CruiseConfiguredActionFactoryConfiguration(), new CruiseActionInstantiator()));
			controller.Do(ParentControl, new NameValueCollectionRequest(Request.Form));

			DisplayFormVariables();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void DisplayFormVariables()
		{
			ArrayList list = new ArrayList();
			foreach (string s in Request.Form.Keys)
			{
				list.Add(new Tuple(s, Request.Form[s]));
			}
			DataGrid1.DataSource = list;
			DataGrid1.DataBind();
		}
	}

	public class Tuple
	{
		private readonly object value;
		private readonly object key;

		public object Value
		{
			get { return this.value; }
		}

		public object Key
		{
			get { return key; }
		}

		public Tuple(object Key, object Value)
		{
			key = Key;
			value = Value;
		}
	}

}
