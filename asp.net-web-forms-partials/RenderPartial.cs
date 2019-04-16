namespace Web_Form_Partial
{
	public static class RenderPartial
	{
		/// <summary>
		/// Renders a control to the Response Stream and ends the response.
		/// Do not catch the ThreadAbortException or the response will not end
		/// </summary>
		/// <param name="control"></param>
		public static bool EmitHTML(Control control)
		{
			HttpResponse response = HttpContext.Current.Response;
			
			using (HtmlTextWriter html_writer = new HtmlTextWriter(response.Output))
			{
				try
				{
					response.Clear();
					response.ContentType = "text/html";

					SetupPage(control.Page, true);
					control.RenderControl(html_writer);
					SetupPage(control.Page, false);
				}
				catch
				{
					return false;
				}

				try
				{
					response.End();
				}
				catch (ThreadAbortException) {
					throw;
				}
				catch
				{
					return false;
				}

			}

			return true;			
		}

		public static string EmitHTML(Control control, bool send_on_response)
		{
			if (send_on_response)
			{
				return Booleans.ToString(AjaxServices.EmitHTML(control));
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				HtmlTextWriter hw = new HtmlTextWriter(new StringWriter(sb));

				SetupPage(control.Page, true);
				control.RenderControl(hw);
				SetupPage(control.Page, false);
				return sb.ToString();
			}
		}

		private static void SetupPage(Page page, bool enable_out_of_order_render)
		{
			if (page is AjaxPage)
			{
				((AjaxPage)page).EnableOutOfOrderRender = enable_out_of_order_render;
			}
		}
	}
}
