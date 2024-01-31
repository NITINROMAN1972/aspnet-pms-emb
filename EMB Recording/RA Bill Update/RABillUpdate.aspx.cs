using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RA_Bill_RABill : System.Web.UI.Page
{
    string connectionString = ConfigurationManager.ConnectionStrings["Ginie"].ConnectionString;

    string projectRefID;
    string workOrderRefID;
    string vendorRefID;

    string selectedProjectMasterRefID;
    string selectedWorkOrderRefID;
    string selectedVendorRefID;
    string selectedAbstractNO;

    // Step #1: Create a Class named ApiPara with 4 properties
    //public class Api
    //{
    //    public string Command { get; set; }
    //    public Dictionary<string, string> Parameters { get; set; }
    //    public string Connection { get; set; }
    //    public string AccessKey { get; set; }
    //}

    //private DataTable getApiCall(string sql, Dictionary<string, string> para)
    //{
    //    Api mPara = new Api
    //    {
    //        Command = sql,
    //        Parameters = para,
    //        Connection = "Ginie"
    //    };

    //    string jsonContent = JsonConvert.SerializeObject(mPara);
    //    StringContent stringContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

    //    string apiUrl = "http://101.53.144.92/wms/api/Get/Table";

    //    using (HttpClient client = new HttpClient())
    //    {
    //        HttpResponseMessage response = client.PostAsync(apiUrl, stringContent).Result;

    //        if (response.IsSuccessStatusCode)
    //        {
    //            string jsonResponse = response.Content.ReadAsStringAsync().Result;

    //            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonResponse);

    //            return dt;
    //        }
    //        else
    //        {
    //            return new DataTable();
    //        }
    //    }
    //}

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Bind_RaHeader();
            Bind_Role_Project();
        }
    }

    //=============================={ Sweet Alert }============================================

    // sweet alert - success only
    private void getSweetAlertSuccessOnly()
    {
        string title = "Saved!";
        string message = "Record saved successfully!";
        string icon = "success";
        string confirmButtonText = "OK";

        string sweetAlertScript =
            $@"<script>
                Swal.fire({{ 
                    title: '{title}', 
                    text: '{message}', 
                    icon: '{icon}', 
                    confirmButtonText: '{confirmButtonText}' 
                }});
            </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - success redirect
    private void getSweetAlertSuccessRedirect(string redirectUrl)
    {
        string title = "Saved!";
        string message = "Record saved successfully!";
        string icon = "success";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false";

        string sweetAlertScript =
            $@"<script>
                Swal.fire({{ 
                    title: '{title}', 
                    text: '{message}', 
                    icon: '{icon}', 
                    confirmButtonText: '{confirmButtonText}',
                    allowOutsideClick: {allowOutsideClick}
                }}).then((result) => {{
                    if (result.isConfirmed) {{
                        window.location.href = '{redirectUrl}';
                    }}
                }});
            </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - success redirect block
    private void getSweetAlertSuccessRedirectMandatory(string titles, string mssg, string redirectUrl)
    {
        string title = titles;
        string message = mssg;
        string icon = "success";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false"; // Prevent closing on outside click

        string sweetAlertScript =
        $@"<script>
            Swal.fire({{ 
                title: '{title}', 
                text: '{message}', 
                icon: '{icon}', 
                confirmButtonText: '{confirmButtonText}', 
                allowOutsideClick: {allowOutsideClick}
            }}).then((result) => {{
                if (result.isConfirmed) {{
                    window.location.href = '{redirectUrl}';
                }}
            }});
        </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - error only block
    private void getSweetAlertErrorMandatory(string titles, string mssg)
    {
        string title = titles;
        string message = mssg;
        string icon = "error";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false"; // Prevent closing on outside click

        string sweetAlertScript =
        $@"<script>
            Swal.fire({{ 
                title: '{title}', 
                text: '{message}', 
                icon: '{icon}', 
                confirmButtonText: '{confirmButtonText}', 
                allowOutsideClick: {allowOutsideClick}
            }});
        </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    //=========================={ Drop Down Binding }==========================

    private void Bind_RaHeader()
    {
        //string sql = "select * from RaHeader874";
        //Dictionary<string, string> para = new Dictionary<string, string>();
        //DataTable dt = getApiCall(sql, para);

        //gridRaHeader.DataSource = dt;
        //gridRaHeader.DataBind();


        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from RaHeader874";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
        }
    }

    private void Bind_Role_Project()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from ProjectMaster874";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddProject.DataSource = dt;
            ddProject.DataTextField = "ProjectName";
            ddProject.DataValueField = "RefID";
            ddProject.DataBind();
            ddProject.Items.Insert(0, new ListItem("------Select Project------", "0"));
        }
    }

    private void Bind_Role_WorkOrder(string projectRefID)
    {
        DataTable projectDT = getProject(projectRefID);

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from WorkOrder874 where woProject = @woProject";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@woProject", projectDT.Rows[0]["RefID"].ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddWOName.DataSource = dt;
            ddWOName.DataTextField = "woTitle";
            ddWOName.DataValueField = "RefID";
            ddWOName.DataBind();
            ddWOName.Items.Insert(0, new ListItem("------Select Work Order------", "0"));
        }
    }

    private void Bind_Role_Vendor(string projectRefID, string workOrderRefID)
    {
        DataTable projectDT = getProject(projectRefID);
        DataTable workOrderDT = getWorkOrder(workOrderRefID);

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from VendorMaster874";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddVendorName.DataSource = dt;
            ddVendorName.DataTextField = "vName";
            ddVendorName.DataValueField = "RefID";
            ddVendorName.DataBind();
            ddVendorName.Items.Insert(0, new ListItem("------Select Vendor------", "0"));
        }
    }

    public void Bind_Role_DocType()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM DocType874";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddDocType.DataSource = dt;
            ddDocType.DataTextField = "DocType";
            ddDocType.DataValueField = "DocType";
            ddDocType.DataBind();
            ddDocType.Items.Insert(0, new ListItem("------Select Doc------", "0"));
        }
    }

    public void Bind_Role_Stages()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM WorkStage874";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddStage.DataSource = dt;
            ddStage.DataTextField = "StageLevel";
            ddStage.DataValueField = "StageLevel";
            ddStage.DataBind();
            ddStage.Items.Insert(0, new ListItem("------Select Stage------", "0"));
        }
    }

    //=========================={ Drop Down Event }==========================

    protected void ddProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        projectRefID = ddProject.SelectedValue;

        if (ddProject.SelectedValue != "0")
        {
            Bind_Role_WorkOrder(projectRefID);
        }
        else
        {
            gridRaUpdateDiv.Visible = false;

            // Clear the values of follwing dropdowns on the server side
            ddWOName.Items.Clear();
            ddVendorName.Items.Clear();
            //ddWorkOrder.Items.Insert(0, new ListItem("------Select Vendor------", "0"));

            // Clear the values of ddWorkOrder & ddVender on the client side using JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "ClearWorkOrderDropdown", "ClearWorkOrderDropdown();", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "ClearVendorDropdown", "ClearVendorDropdown();", true);
        }
    }

    protected void ddWOName_SelectedIndexChanged(object sender, EventArgs e)
    {
        projectRefID = ddProject.SelectedValue;
        workOrderRefID = ddWOName.SelectedValue;

        if (ddWOName.SelectedValue != "0")
        {
            Bind_Role_Vendor(projectRefID, workOrderRefID);
        }
        else
        {
            gridRaUpdateDiv.Visible = false;

            // Clear the values of follwing dropdowns on the server side
            ddVendorName.Items.Clear();

            // Clear the values of ddWorkOrder & ddVender on the client side using JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "ClearVendorDropdown", "ClearVendorDropdown();", true);
        }
    }

    protected void ddVendorName_SelectedIndexChanged(object sender, EventArgs e)
    {
        projectRefID = ddProject.SelectedValue;
        workOrderRefID = ddWOName.SelectedValue;
        vendorRefID = ddVendorName.SelectedValue;
    }

    //=========================={ Fetching Data }==========================

    private DataTable getProject(string projectRefID)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM ProjectMaster874 where RefID=@RefID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefID", projectRefID.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getWorkOrder(string workOrderRefID)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM WorkOrder874 where RefID=@RefID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefID", workOrderRefID.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getVendor(string vendorRefID)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM VendorMaster874 where RefID=@RefID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefID", vendorRefID.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getAbstractNo(string selectedAbstractNO)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM AbstApproval874 where AbsNo=@AbsNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@AbsNo", selectedAbstractNO.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getSearchedEmbHeader(string project, string workOrder, string vendor)
    {
        DataTable dt = new DataTable();

        if (project != "" && workOrder != "" && vendor != "")
        {
            DataTable projectDT = getProject(project); // project dt
            DataTable workOrderDT = getWorkOrder(workOrder); // work order dt
            DataTable vendorDT = getVendor(vendor); // vendor dt

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT * FROM RaHeader874 where RaProj = @RaProj and RaWO = @RaWO and RaVendor = @RaVendor";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@RaProj", projectDT.Rows[0]["ProjectName"].ToString());
                cmd.Parameters.AddWithValue("@RaWO", workOrderDT.Rows[0]["woTendrNo"].ToString());
                cmd.Parameters.AddWithValue("@RaVendor", vendorDT.Rows[0]["vName"].ToString());
                cmd.ExecuteNonQuery();

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                con.Close();
                return dt;
            }
        }
        else if (project != "" && workOrder != "")
        {
            DataTable projectDT = getProject(project); // project dt
            DataTable workOrderDT = getWorkOrder(workOrder); // work order dt

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT * FROM RaHeader874 where RaProj = @RaProj and RaWO = @RaWO";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@RaProj", projectDT.Rows[0]["ProjectName"].ToString());
                cmd.Parameters.AddWithValue("@RaWO", workOrderDT.Rows[0]["woTendrNo"].ToString());
                cmd.ExecuteNonQuery();

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                con.Close();
                return dt;
            }
        }
        else if (project != "")
        {
            DataTable projectDT = getProject(project); // project dt

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT * FROM RaHeader874 where RaProj = @RaProj";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@RaProj", projectDT.Rows[0]["ProjectName"].ToString());
                cmd.ExecuteNonQuery();

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(dt);
                con.Close();
                return dt;
            }
        }

        return dt;
    }

    protected void GridEmbHeader_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //binding GridView to PageIndex object
        gridRaHeader.PageIndex = e.NewPageIndex;
        Bind_RaHeader();
    }

    private DataTable getRaHeader(int rowId)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM RaHeader874 where RefID=@RefID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefID", rowId.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getRaDetails(int rowId)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM RaDetails874 where RaHeaderID=@RaHeaderID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RaHeaderID", rowId.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    private DataTable getAccountHead(int RaHeaderID)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM RaTax874 WHERE RaHeaderID = @RaHeaderID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RaHeaderID", RaHeaderID.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();
            return dt;
        }
    }

    //=============================={ Fill BoQ & Tax Head }============================================

    protected void GridTax_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) > 0)
        {
            // Set the row in edit mode
            e.Row.RowState = e.Row.RowState ^ DataControlRowState.Edit;
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // fetching acount head or taxes
            int RaHeaderID = Convert.ToInt32(Session["RowID"]);
            DataTable accountHeadDT = getAccountHead(RaHeaderID);

            // adding + / - dropdowns
            DropDownList ddlAddLess = (DropDownList)e.Row.FindControl("AddLess");
            if (ddlAddLess != null)
            {
                // Add options dynamically
                ddlAddLess.Items.Add(new ListItem("+", "Add"));
                ddlAddLess.Items.Add(new ListItem("-", "Less"));

                // Set selected value based on the "AddLess" column in the DataTable
                string addLessValue = accountHeadDT.Rows[e.Row.RowIndex]["AddLess"].ToString();

                // Set the selected value in the DropDownList
                ListItem selectedListItem = ddlAddLess.Items.FindByValue(addLessValue);
                if (selectedListItem != null)
                {
                    selectedListItem.Selected = true;
                }
            }

            // adding % / ₹ dropdowns
            DropDownList ddlPerOrAmnt = (DropDownList)e.Row.FindControl("PerOrAmnt");
            if (ddlPerOrAmnt != null)
            {
                // Add options dynamically
                ddlPerOrAmnt.Items.Add(new ListItem("%", "Percentage"));
                ddlPerOrAmnt.Items.Add(new ListItem("₹", "Amount"));

                // Set selected value based on the "PerOrAmnt" column in the DataTable
                string perOrAmntValue = accountHeadDT.Rows[e.Row.RowIndex]["PerOrAmnt"].ToString();

                // Set the selected value in the DropDownList
                ListItem selectedListItem = ddlPerOrAmnt.Items.FindByValue(perOrAmntValue);
                if (selectedListItem != null)
                {
                    selectedListItem.Selected = true;
                }
            }
        }
    }

    protected void btnReCalTax_Click(object sender, EventArgs e)
    {
        // Account Head DataTable
        DataTable dt = (DataTable)Session["AccountHeadDT"];

        // Perform calculations or other logic based on the updated values
        double basicAmount = Convert.ToDouble(txtBasicAmt.Text);
        double totalDeduction = 0.00;
        double totalAddition = 0.00;
        double netAmount = 0.00;

        foreach (GridViewRow row in GridTax.Rows)
        {
            // Accessing the updated dropdown values from the grid
            string updatedAddLessValue = ((DropDownList)row.FindControl("AddLess")).SelectedValue;
            string updatedPerOrAmntValue = ((DropDownList)row.FindControl("PerOrAmnt")).SelectedValue;

            int rowIndex = row.RowIndex;

            // reading parameters from gridview
            TextBox DeductionHeadStr = row.FindControl("DeductionHead") as TextBox;
            TextBox FactorInPercentage = row.FindControl("FactorInPer") as TextBox;
            DropDownList perOrAmntDropDown = row.FindControl("PerOrAmnt") as DropDownList;
            DropDownList AddLessDropown = row.FindControl("AddLess") as DropDownList;
            TextBox TaxAccountHeadAmount = row.FindControl("TaxAmount") as TextBox;

            string DeductionHead = (DeductionHeadStr.Text).ToString();
            double factorInPer = Convert.ToDouble(FactorInPercentage.Text);
            string perOrAmnt = perOrAmntDropDown.SelectedValue;
            string addLess = AddLessDropown.SelectedValue;
            double taxAmount = Convert.ToDouble(TaxAccountHeadAmount.Text);

            if (perOrAmnt == "Amount")
            {
                taxAmount = factorInPer;

                // setting tax head amount
                TaxAccountHeadAmount.Text = Math.Abs(taxAmount).ToString("N2");

                if (addLess == "Add")
                {
                    totalAddition = totalAddition + taxAmount;
                }
                else
                {
                    totalDeduction = totalDeduction + taxAmount;
                }
            }
            else
            {
                // tax amount (based on add or less)
                taxAmount = (basicAmount * factorInPer) / 100;

                // setting tax head amount
                TaxAccountHeadAmount.Text = Math.Abs(taxAmount).ToString("N2");

                if (addLess == "Add")
                {
                    totalAddition = totalAddition + taxAmount;
                }
                else
                {
                    totalDeduction = totalDeduction + taxAmount;
                }
            }
        }

        // setting total deduction
        txtTotalDeduct.Text = Math.Abs(totalDeduction).ToString("N2");

        // setting total addition
        txtTotalAdd.Text = totalAddition.ToString("N2");

        // Net Amount after tax deductions or addition
        netAmount = (basicAmount + totalAddition) - Math.Abs(totalDeduction);
        txtNetAmnt.Text = netAmount.ToString("N2");
    }

    //========================================================================

    private void Bind_Role_Update_Dropdowns(int rowId)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from RaHeader874 where RaHeaderID = @RaHeaderID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RaHeaderID", rowId.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddProjectMaster.DataSource = dt;
            ddProjectMaster.DataTextField = "RaProj";
            ddProjectMaster.DataValueField = "RefID";
            ddProjectMaster.DataBind();
            ddProjectMaster.Items.Insert(0, new ListItem("------Select Project------", "0"));

            ddWorkOrder.DataSource = dt;
            ddWorkOrder.DataTextField = "RaWO";
            ddWorkOrder.DataValueField = "RefID";
            ddWorkOrder.DataBind();
            ddWorkOrder.Items.Insert(0, new ListItem("------Select Work Order------", "0"));

            ddVender.DataSource = dt;
            ddVender.DataTextField = "RaVendor";
            ddVender.DataValueField = "RefID";
            ddVender.DataBind();
            ddVender.Items.Insert(0, new ListItem("------Select Vendor------", "0"));

            ddAbstractNo.DataSource = dt;
            ddAbstractNo.DataTextField = "RaAbstNo";
            ddAbstractNo.DataValueField = "RefID";
            ddAbstractNo.DataBind();
            ddAbstractNo.Items.Insert(0, new ListItem("------Select Abstract No.------", "0"));
        }
    }

    //=========================={ Button Click Event }==========================

    protected void btnNewRaBill_Click(object sender, EventArgs e)
    {
        Response.Redirect("../RABillNew.aspx");
    }

    protected void btnTruncate_Click(object sender, EventArgs e)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            string sql = "truncate table RaHeader874";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            string sql1 = "truncate table RaDetails874";
            SqlCommand cmd1 = new SqlCommand(sql1, con);
            cmd1.ExecuteNonQuery();

            string sql2 = "truncate table RaTax874";
            SqlCommand cmd2 = new SqlCommand(sql2, con);
            cmd2.ExecuteNonQuery();

            string sql3 = "truncate table DocUpload874";
            SqlCommand cmd3 = new SqlCommand(sql3, con);
            cmd3.ExecuteNonQuery();

            con.Close();
        }

        string message = "RA Header & RA Details truncated";
        string script = $"alert('{message}');";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }

    private void BindGridView()
    {
        DataTable searchedEmbDT = new DataTable();

        if (ddProject.SelectedValue.ToString() == "0" && string.IsNullOrEmpty(ddWOName.SelectedValue) && string.IsNullOrEmpty(ddVendorName.SelectedValue))
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "select * from RaHeader874";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();

                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(searchedEmbDT);
                con.Close();

                // binding the grid
                gridRaHeader.DataSource = searchedEmbDT;
                gridRaHeader.DataBind();
            }

            gridRaUpdateDiv.Visible = true;
        }
        else if (ddProject.SelectedValue.ToString() != "0" && ddWOName.SelectedValue.ToString() == "0" && string.IsNullOrEmpty(ddVendorName.SelectedValue))
        {
            // only project is selceted
            projectRefID = ddProject.SelectedValue; // Ref ID

            searchedEmbDT = getSearchedEmbHeader(projectRefID, "", "");

            gridRaUpdateDiv.Visible = true;

            // binding the grid
            gridRaHeader.DataSource = searchedEmbDT;
            gridRaHeader.DataBind();
        }
        else if (ddProject.SelectedValue.ToString() != "0" && ddWOName.SelectedValue.ToString() != "0" && ddVendorName.SelectedValue.ToString() == "0")
        {
            // only project and work order are selceted

            projectRefID = ddProject.SelectedValue; // Ref ID
            workOrderRefID = ddWOName.SelectedValue; // Ref ID

            searchedEmbDT = getSearchedEmbHeader(projectRefID, workOrderRefID, "");

            gridRaUpdateDiv.Visible = true;

            // binding the grid
            gridRaHeader.DataSource = searchedEmbDT;
            gridRaHeader.DataBind();
        }
        else if (ddProject.SelectedValue.ToString() != "0" && ddWOName.SelectedValue.ToString() != "0" && ddVendorName.SelectedValue.ToString() != "0")
        {
            // project, work order and vendor are selceted

            projectRefID = ddProject.SelectedValue; // Ref ID
            workOrderRefID = ddWOName.SelectedValue; // Ref ID
            vendorRefID = ddVendorName.SelectedValue; // Ref ID

            searchedEmbDT = getSearchedEmbHeader(projectRefID, workOrderRefID, vendorRefID);

            gridRaUpdateDiv.Visible = true;

            // binding the grid
            gridRaHeader.DataSource = searchedEmbDT;
            gridRaHeader.DataBind();
        }
        else
        {
            //redirect with only message
            string message = "Please select properly !";
            string script = $"alert('{message}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);
        }
    }

    protected void btnBasicAmount_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)Session["BoQDTUpdate"];

        double basicAmount = 0;

        if (dt != null)
        {
            // Iterate through each row in the GridView
            foreach (GridViewRow row in gridDynamicBOQ.Rows)
            {
                // code to read values from row-bound grid columns
                //TextBox boqQty = row.FindControl("BoqQtyMeas") as TextBox;
                //double qtyMeasuredValue = Convert.ToDouble(boqQty.Text);

                int rowIndex = row.RowIndex;

                double qtyMeasuredValue = Convert.ToDouble(dt.Rows[rowIndex]["RaAbstQty"]);
                double boqUnitRate = Convert.ToDouble(dt.Rows[rowIndex]["RaItemRate"]);

                double prod = (qtyMeasuredValue * boqUnitRate);
                basicAmount = basicAmount + prod;
            }

            string basicAmountStr = basicAmount.ToString("N0");

            txtBasicAmt.CssClass = "form-control fw-normal border border-2";
            txtBasicAmt.Text = basicAmountStr;
        }
    }

    protected void GrdUser_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "lnkView")
        {
            int rowId = Convert.ToInt32(e.CommandArgument);
            Session["RowID"] = rowId;

            gridRaUpdateDiv.Visible = false;
            divRAUpdate.Visible = true;
            btnReCalTax.Enabled = true;
            //raDetailsUpdate.Visible = true;

            divTopSearch.Visible = false;
            //btnSubmitBasicAmount.Enabled = true;

            // getting all dropdowns
            Bind_Role_Update_Dropdowns(rowId);

            // fill RA header
            updateRAHeader(rowId);

            // fill EMB Details
            updateEmbDetails(rowId);

            // binding document dropdowns
            Bind_Role_DocType();
            Bind_Role_Stages();

            // binding doc gridview
            updateDocUploadGrid(rowId);
        }
    }

    protected void GridDocument_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "lnkView")
        {
            int docRefID = Convert.ToInt32(e.CommandArgument); 
            Session["DocRefID"] = docRefID;

            // alert pop-up with only message
            string message = "docRefID  " + docRefID;
            string script = $"alert('{message}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);
        }
    }

    private void updateRAHeader(int rowId)
    {
        // fetching EMB header
        DataTable raHeaderDT = getRaHeader(rowId);

        if (raHeaderDT.Rows.Count > 0)
        {
            // drop downs
            ddProjectMaster.SelectedIndex = 1;
            ddWorkOrder.SelectedIndex = 1;
            ddVender.SelectedIndex = 1;
            ddAbstractNo.SelectedIndex = 1;

            // header non-dropdowns
            txtWoAmnt.Text = raHeaderDT.Rows[0]["RaWoAmount"].ToString();
            txtUpToTotalRaAmnt.Text = raHeaderDT.Rows[0]["RaBillBookAmnt"].ToString();
            txtRemarks.Value = raHeaderDT.Rows[0]["RaRemarks"].ToString();

            // date
            DateTime billDate = DateTime.Parse(raHeaderDT.Rows[0]["RaBillDate"].ToString());
            DateTime payDueDate = DateTime.Parse(raHeaderDT.Rows[0]["RaPayDueDate"].ToString());

            dateBillDate.Text = billDate.ToString("yyyy-MM-dd");
            datePayDueDate.Text = payDueDate.ToString("yyyy-MM-dd");

            txtBillNo.Text = raHeaderDT.Rows[0]["RaBillNo"].ToString();
            txtBasicAmt.Text = raHeaderDT.Rows[0]["RaBasicAmount"].ToString();

            // tax heads data
            txtTotalDeduct.Text = raHeaderDT.Rows[0]["RaTotalDeduct"].ToString();
            txtTotalAdd.Text = raHeaderDT.Rows[0]["RaTotalAdd"].ToString();
            txtNetAmnt.Text = raHeaderDT.Rows[0]["RaNetAmount"].ToString();

            gridEmbDiv.Visible = true;
        }
        else
        {
            string message = "No RA Header data Found !";
            string script = $"alert('{message}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);
        }
    }

    private void updateEmbDetails(int rowId)
    {
        // fetching EMB details
        DataTable dt = getRaDetails(rowId);
        Session["BoQDTUpdate"] = dt;

        if (dt.Rows.Count > 0)
        {
            // BoQ Grid
            gridDynamicBOQ.DataSource = dt;
            gridDynamicBOQ.DataBind();

            // Tax Head Grid
            DataTable accountHeadDT = getAccountHead(rowId);
            Session["AccountHeadDT"] = accountHeadDT;

            GridTax.DataSource = accountHeadDT;
            GridTax.DataBind();

        }
        else
        {
            string message1 = "No RA Details Found !";
            string script1 = $"alert('{message1}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script1, true);
        }
    }

    private void updateDocUploadGrid(int rowId)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from DocUpload874 where RaHeaderID = @RaHeaderID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RaHeaderID", rowId);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            if (dt != null)
            {
                docGrid.Visible = true;

                // binding document details gridview
                GridDocument.DataSource = dt;
                GridDocument.DataBind();

                // Saving DataTable to ViewState for further use to additon doc upload
                ViewState["DocDetailsDataTable"] = dt;
                Session["DocuUploadDTUpte"] = dt;
            }
        }
    }

    //=========================={ File Upload Event }==========================

    protected void btnDocUpload_Click(object sender, EventArgs e)
    {
        // setting the file size in web.config file (web.config should not be read only)
        //settingHttpRuntimeForFileSize();

        string docTypeCode = ddDocType.SelectedValue;
        string stageCode = ddStage.SelectedValue;

        if (fileDoc.HasFile)
        {
            string FileExtension = System.IO.Path.GetExtension(fileDoc.FileName);

            if (FileExtension == ".xlsx" || FileExtension == ".xls")
            {

            }

            // file name
            string onlyFileNameWithExtn = fileDoc.FileName.ToString();

            // getting unique file name
            string strFileName = GenerateUniqueId(onlyFileNameWithExtn);

            // saving and getting file path
            string filePath = getServerFilePath(strFileName);

            // Retrieve DataTable from ViewState or create a new one
            DataTable dt = ViewState["DocDetailsDataTable"] as DataTable ?? CreateDocDetailsDataTable();

            // filling document details datatable
            AddRowToDocDetailsDataTable(dt, docTypeCode, stageCode, onlyFileNameWithExtn, filePath);

            // Save DataTable to ViewState
            ViewState["DocDetailsDataTable"] = dt;
            Session["DocuUploadDTUpte"] = dt;

            if (dt.Rows.Count > 0)
            {
                // binding document details gridview
                GridDocument.DataSource = dt;
                GridDocument.DataBind();
            }
        }
    }

    private string GenerateUniqueId(string strFileName)
    {
        long timestamp = DateTime.Now.Ticks;
        //string guid = Guid.NewGuid().ToString("N"); //N to remove hypen "-" from GUIDs
        string guid = Guid.NewGuid().ToString(); //N to remove hypen "-" from GUIDs
        string uniqueID = timestamp + "_" + guid + "_" + strFileName;
        return uniqueID;
    }

    private string getServerFilePath(string strFileName)
    {
        string orgFilePath = Server.MapPath("~/Portal/Public/" + strFileName);

        // saving file
        fileDoc.SaveAs(orgFilePath);

        //string filePath = Server.MapPath("~/Portal/Public/" + strFileName);
        //file:///C:/HostingSpaces/PAWAN/cdsmis.in/wwwroot/Pms2/Portal/Public/638399011215544557_926f9320-275e-49ad-8f59-32ecb304a9f1_EMB%20Recording.pdf

        // replacing server-specific path with the desired URL
        string baseUrl = "http://101.53.144.92/pms2/Ginie/External?url=..";
        string relativePath = orgFilePath.Replace(Server.MapPath("~/Portal/Public/"), "Portal/Public/");

        // Full URL for the hyperlink
        string fullUrl = $"{baseUrl}/{relativePath}";

        return fullUrl;
    }

    private DataTable CreateDocDetailsDataTable()
    {
        DataTable dt = new DataTable();

        // document type
        DataColumn docType = new DataColumn("docType", typeof(string));
        dt.Columns.Add(docType);

        // stage level
        DataColumn stageLevel = new DataColumn("stageLevel", typeof(string));
        dt.Columns.Add(stageLevel);

        // file name
        DataColumn DocName = new DataColumn("DocName", typeof(string));
        dt.Columns.Add(DocName);

        // Doc uploaded path
        DataColumn DocPath = new DataColumn("DocPath", typeof(string));
        dt.Columns.Add(DocPath);

        return dt;
    }

    private void AddRowToDocDetailsDataTable(DataTable dt, string docTypeCode, string stageCode, string onlyFileNameWithExtn, string filePath)
    {
        // Create a new row
        DataRow row = dt.NewRow();

        // Set values for the new row
        row["docType"] = docTypeCode;
        row["stageLevel"] = stageCode;
        row["DocName"] = onlyFileNameWithExtn;
        row["DocPath"] = filePath;

        // Add the new row to the DataTable
        dt.Rows.Add(row);
    }

    //=========================={ Submit Button Event }==========================

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("RABillUpdate.aspx");
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        int RaHeaderID = Convert.ToInt32(Session["RowID"]);

        // Update Tax Account Head Grid
        updateRaTaxHeadGrid(RaHeaderID);

        // Update Tax Details
        updateRaDetails(RaHeaderID);

        // update Document Upload Details
        updateRaDocumentDetails(RaHeaderID);

        // sweet alert - success redirect block
        getSweetAlertSuccessRedirectMandatory("RA Bill updated", "the following RA Bill has been updated successfully", "");
    }

    private void updateRaTaxHeadGrid(int RaHeaderID)
    {
        // Account Head DataTable
        DataTable dt = (DataTable)Session["AccountHeadDT"];

        try
        {
            if (dt != null)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    foreach (GridViewRow row in GridTax.Rows)
                    {
                        // to get the current row index
                        int rowIndex = row.RowIndex;

                        // parameters of textbox
                        TextBox DeductionHeadStr = row.FindControl("DeductionHead") as TextBox;
                        string DeductionHead = (DeductionHeadStr.Text).ToString();

                        TextBox FactorInPercentage = row.FindControl("FactorInPer") as TextBox;
                        double FactorInPer = Convert.ToDouble(FactorInPercentage.Text);

                        TextBox TaxAccountHeadAmount = row.FindControl("TaxAmount") as TextBox;
                        double TaxAmount = Convert.ToDouble(TaxAccountHeadAmount.Text);

                        // parameters of dropdown list
                        DropDownList perOrAmntDropDown = row.FindControl("PerOrAmnt") as DropDownList;
                        string PerOrAmnt = perOrAmntDropDown.SelectedValue;

                        DropDownList AddLessDropown = row.FindControl("AddLess") as DropDownList;
                        string AddLess = AddLessDropown.SelectedValue;

                        // inserting into Ra Tax
                        string sql = "UPDATE RaTax874 SET FactorInPer=@FactorInPer, PerOrAmnt=@PerOrAmnt, AddLess=@AddLess, TaxAmount=@TaxAmount WHERE RefID=@RefID";

                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@FactorInPer", FactorInPer);
                        cmd.Parameters.AddWithValue("@PerOrAmnt", PerOrAmnt);
                        cmd.Parameters.AddWithValue("@AddLess", AddLess);
                        cmd.Parameters.AddWithValue("@TaxAmount", TaxAmount);
                        cmd.Parameters.AddWithValue("@RefID", dt.Rows[rowIndex]["RefID"].ToString());
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    private void updateRaDetails(int RaHeaderID)
    {
        // RA Header update total deduction, addition and Net Amount
        double totalAddition = Convert.ToDouble(txtTotalAdd.Text);
        double totalDeduction = Convert.ToDouble(txtTotalDeduct.Text);
        double netAmount = Convert.ToDouble(txtNetAmnt.Text);

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            // inserting into Ra Tax
            string sql = "UPDATE RaHeader874 SET RaTotalAdd=@RaTotalAdd, RaTotalDeduct=@RaTotalDeduct, RaNetAmount=@RaNetAmount WHERE RaHeaderID=@RaHeaderID";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RaTotalAdd", totalAddition);
            cmd.Parameters.AddWithValue("@RaTotalDeduct", totalDeduction);
            cmd.Parameters.AddWithValue("@RaNetAmount", netAmount);
            cmd.Parameters.AddWithValue("@RaHeaderID", RaHeaderID);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }

    private void updateRaDocumentDetails(int RaHeaderID)
    {
        DataTable dt = (DataTable)Session["DocuUploadDTUpte"];

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            foreach (GridViewRow row in GridDocument.Rows)
            {
                // to get the current row index
                int rowIndex = row.RowIndex;

                // dropdown values
                string docType = dt.Rows[rowIndex]["DocType"].ToString();
                string stageLevel = dt.Rows[rowIndex]["StageLevel"].ToString();
                string docName = dt.Rows[rowIndex]["DocName"].ToString();

                // Find the HyperLink control in the current row
                HyperLink hypDocPath = (HyperLink)row.FindControl("DocPath");
                // Get the NavigateUrl property from the HyperLink control
                string navigateUrl = hypDocPath.NavigateUrl;

                // document ref id
                string docRefID = dt.Rows[rowIndex]["RefID"].ToString();

                //string message = "docRefID" + ": " + docRefID;
                //string script = $"alert('{message}');";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);

                // check for existing documents to update
                bool isDocExist = checkForDocuUploadedExist(docRefID);

                if (isDocExist)
                {
                    string sql = "UPDATE DocUpload874 SET DocType=@DocType, StageLevel=@StageLevel, DocName=@DocName, DocPath=@DocPath WHERE RefID=@RefID";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@DocType", docType);
                    cmd.Parameters.AddWithValue("@StageLevel", stageLevel);
                    cmd.Parameters.AddWithValue("@DocName", docName);
                    cmd.Parameters.AddWithValue("@DocPath", navigateUrl);
                    cmd.Parameters.AddWithValue("@RefID", docRefID);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // getting new doc ref id
                    int newDocRefID = getDocUploadedRefID();

                    string sql = "insert into DocUpload874 (RefID, RaHeaderID, DocType, StageLevel, DocName, DocPath) values (@RefID, @RaHeaderID, @DocType, @StageLevel, @DocName, @DocPath)";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@RefID", newDocRefID);
                    cmd.Parameters.AddWithValue("@RaHeaderID", RaHeaderID);
                    cmd.Parameters.AddWithValue("@DocType", docType);
                    cmd.Parameters.AddWithValue("@StageLevel", stageLevel);
                    cmd.Parameters.AddWithValue("@DocName", docName);
                    cmd.Parameters.AddWithValue("@DocPath", navigateUrl);
                    cmd.ExecuteNonQuery();
                }
            }

            con.Close();
        }
    }

    //===============================================================================

    private bool checkForDocuUploadedExist(string docRefID)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM DocUpload874 WHERE RefID=@RefID";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefID", docRefID);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            if (dt.Rows.Count > 0) return true;
            else return false;
        }
    }

    private int getDocUploadedRefID()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefID AS INT)), 1000000) + 1 AS NextRefID FROM DocUpload874";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
    }
}