using System.Web.UI;

namespace Web_Form_Partial
{
	public partial class PartialBasePage : System.Web.UI.Page
	{
		/*
		 * The following are work arounds that prevent errors when attempting to render controls
		 * outside the normal page flow.
		 * */

		public override void VerifyRenderingInServerForm(System.Web.UI.Control control)
		{
			// Don't do anything

			// This is to work around a bug that occurs when attempting to render
			// controls when the page is not in the "Render Phase"
			// (see http://siderite.blogspot.com/2006/08/control-must-be-placed-ins_115589637882320466.html).

		}


		private bool out_of_order_render = false;
		/// <summary>
		/// When Out Of Order Render is enabled it disables EventValidation for the page so
		/// a control can be rendered outside the normal Render Phase.
		/// </summary>
		public bool EnableOutOfOrderRender
		{
			get
			{
				return this.out_of_order_render;
			}
			set
			{
				this.out_of_order_render = value;
			}
		}

		public override bool EnableEventValidation
		{
			get
			{
				if (this.out_of_order_render) return false;
				else return base.EnableEventValidation;
			}
			set
			{
				base.EnableEventValidation = value;
			}
		}
	}
}
