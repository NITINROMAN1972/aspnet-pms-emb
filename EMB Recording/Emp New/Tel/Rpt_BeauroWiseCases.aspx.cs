using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class Rpt_BeauroWiseCases : System.Web.UI.Page
{

    string connectionString = ConfigurationManager.ConnectionStrings["Ginie"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //BindCustomersAndGrid();
            BindDropDownCountry();
            BindDropDownState();
            BindDropDownCity();
        }
    }

    protected void Radgrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        BindCustomersAndGrid();
    }

    protected void Radgrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {

    }

    protected void RadGrid1_ItemCreated(object sender, GridItemEventArgs e)
    {
        // Your event handler code here
    }

    //The Grid will change when the button is being clicked
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Radgrid1.Visible = true;
        BindCustomersAndGrid();
        Radgrid1.DataBind();

    }

    private void BindDropDownCountry()
    {
        // connection string  
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            using (SqlCommand com = new SqlCommand("SELECT OANo FROM CaseCreation864", con))
            {
                // table name   
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                da.Fill(dt);  // fill dataset  
                ddlOANo.DataSource = dt;      //assigning datasource to the dropdownlist 
                ddlOANo.DataTextField = "OANo";
                ddlOANo.DataValueField = "OANo";
                ddlOANo.DataBind();  //binding dropdownlist 
                ddlOANo.Items.Insert(0, new ListItem("--select--", ""));
                con.Close();
            }
        }

    }


    private void BindDropDownState()
    {
        //  string connectionString = ConfigurationManager.ConnectionStrings["Ginie"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            //con.Open();
            SqlCommand com = new SqlCommand("SELECT BuroID, BuroCity FROM Buro864", con);
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);
            ddlBureau.DataSource = ds.Tables[0]; // Assigning datasource to the dropdownlist  
            ddlBureau.DataTextField = "BuroCity"; // Text field name of table displayed in the dropdown       
            ddlBureau.DataValueField = "BuroID";  // To retrieve the specific textfield name   
            ddlBureau.DataBind(); // Binding dropdownlist 
            ddlBureau.Items.Insert(0, new ListItem("--select--", ""));
            //con.Close();
        }
    }

    private void BindDropDownCity()
    {
        //string connectionString = ConfigurationManager.ConnectionStrings["Ginie"].ConnectionString;
        // connection string  
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            // con.Open();
            SqlCommand com = new SqlCommand("SELECT CircleID, CircleName FROM Circle864", con);
            // table name   
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            da.Fill(ds);  // fill dataset  
            ddlCircle.DataSource = ds.Tables[0]; // Assigning datasource to the dropdownlist  
            ddlCircle.DataTextField = "CircleName"; // Text field name of table displayed in the dropdown       
            ddlCircle.DataValueField = "CircleID";  // To retrieve the specific textfield name   
            ddlCircle.DataBind(); // Binding dropdownlist 
            ddlCircle.Items.Insert(0, new ListItem("--select--", ""));
            //con.Close();
        }
    }

    private void BindCustomersAndGrid()
    {
        // Retrieve connection string from Web.config
        string connectionString = ConfigurationManager.ConnectionStrings["Ginie"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = @"SELECT a.RefID, a.CaseDate, r.BuroCity, a.OANo, a.ApplicantName, STRING_AGG(y.ApplicantCaty, ', ') WITHIN GROUP (ORDER BY y.ApplicantCaty) AS [ApplicantsCategory], e.ConOfficeName, x.CircleName, t.CourtName,rt.RespoType AS RespondantDesignation1,rt2.RespoType AS RespondantDesignation2,rt3.RespoType AS RespondantDesignation3,rt4.RespoType AS RespondantDesignation4,rt5.RespoType AS RespondantDesignation5,a.CaseBrief,a.KeyWord,a.HearDate,a.CaseCreDHO,a.CaseCreCSDist,a.MentHosp,a.AdhsLeprosy,a.HealthLab,a.RegWork,a.WingOff,a.TrainCent,a.AdhsMalaria,a.DmoOff,a.DocUpload   
                        FROM CaseCreation864 a
                        JOIN Buro864 r ON a.CaseCreBeuro = r.BuroID
                        JOIN Circle864 x ON a.CaseCreCircle = x.CircleID
                        JOIN ConcernOffices864 e ON a.ConOffice = e.ConOfficeID
                        JOIN TypeCourt864 t ON a.CourtName = t.CourtID
                        JOIN CatyApplicant864 y ON CHARINDEX(',' + CAST(y.ApplicantID AS NVARCHAR(MAX)) + ',', ',' + a.ApplicantCaty + ',') > 0 
                        JOIN RespondentType864 rt ON a.RespoDesig = rt.RespoID 
                        JOIN RespondentType864 rt2 ON a.RespoDesig2 = rt2.RespoID
                        JOIN RespondentType864 rt3 ON a.RespoDesig3 = rt3.RespoID
                        JOIN RespondentType864 rt4 ON a.RespoDesig4 = rt4.RespoID
                        JOIN RespondentType864 rt5 ON a.RespoDesig5 = rt5.RespoID
                        WHERE 1=1";

            // Check if Employee ID is provided for filtering
            //if (!string.IsNullOrEmpty(TxtEMPID.Text.Trim()))
            //{
            //    sql += " AND EmployeeID = @EmployeeID";
            //}           
            // Check if a value is selected in the dropdown
            if (ddlOANo.SelectedIndex > 0)
            {
                sql += " AND a.OANo = @DropdownCountry";
            }

            if (ddlBureau.SelectedIndex > 0)
            {
                sql += " AND r.BuroCity = @DropdownState";
            }

            if (ddlCircle.SelectedIndex > 0)
            {
                sql += " AND x.CircleName = @DropdownCity";
            }

            if (!string.IsNullOrEmpty(txtFromDate.Value))
            {
               
                sql += " AND CONVERT(datetime, a.CaseDate, 103) >= @FromDate";
            }

            // Check if To Date is provided for filtering
            if (!string.IsNullOrEmpty(txtToDate.Value))
            {
                sql += "  AND CONVERT(datetime, a.CaseDate, 103) <= @ToDate";
            }
            
            
            sql += " GROUP BY a.RefID, a.CaseDate, r.BuroCity, a.OANo, a.ApplicantName, e.ConOfficeName, x.CircleName, t.CourtName, rt.RespoType, rt2.RespoType,rt3.RespoType,rt4.RespoType,rt5.RespoType,a.CaseBrief,a.KeyWord,a.HearDate,a.CaseCreDHO,a.CaseCreCSDist,a.MentHosp,a.AdhsLeprosy,a.HealthLab,a.RegWork,a.WingOff,a.TrainCent,a.AdhsMalaria,a.DmoOff,a.DocUpload";

            SqlCommand cmd = new SqlCommand(sql, con);

            // Add parameters for Employee ID filtering
            //if (!string.IsNullOrEmpty(TxtEMPID.Text.Trim()))
            //{
            //    cmd.Parameters.AddWithValue("@EmployeeID", TxtEMPID.Text.Trim());
            //}
            //else
            //{
            //    cmd.Parameters.AddWithValue("@EmployeeID", DBNull.Value);
            //}

            // Add parameters for Dropdown filtering
            if (ddlOANo.SelectedIndex > 0)
            {
                cmd.Parameters.AddWithValue("@DropdownCountry", ddlOANo.SelectedItem.Text);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DropdownCountry", DBNull.Value);
            }


            if (ddlBureau.SelectedIndex > 0)
            {
                cmd.Parameters.AddWithValue("@DropdownState", ddlBureau.SelectedItem.Text);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DropdownState", DBNull.Value);
            }

            if (ddlCircle.SelectedIndex > 0)
            {
                cmd.Parameters.AddWithValue("@DropdownCity", ddlCircle.SelectedItem.Text);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DropdownCity", DBNull.Value);
            }

            if (!string.IsNullOrEmpty(txtFromDate.Value))
            {
                cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(txtFromDate.Value, CultureInfo.GetCultureInfo("en-GB")));
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
            }

            if (!string.IsNullOrEmpty(txtToDate.Value))
            {
                cmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(txtToDate.Value, CultureInfo.GetCultureInfo("en-GB")));
            }
            else
            {
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
            }
            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            Radgrid1.DataSource = dt;
            // Set the current page index to the first page
            //Radgrid1.DataBind();
        }
    }

    protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (ddlCountry.SelectedIndex > 0)
        //{
        //    BindDropDownState(ddlCountry.SelectedValue.ToString());
        //}
        //else
        //{
        //    // No value selected in Country dropdown, clear State dropdown
        //    ddlState.Items.Clear();
        //    ddlCity.Items.Clear();
        //}

        BindCustomersAndGrid();
        Radgrid1.DataBind();
    }

    protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (ddlState.SelectedIndex > 0)
        //{
        //    BindDropDownCity(ddlState.SelectedValue.ToString());
        //}
        //else
        //{
        //    // No value selected in Country dropdown, clear State dropdown
        //    ddlCity.Items.Clear();
        //}
        BindCustomersAndGrid();
        Radgrid1.DataBind();
    }
    protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindCustomersAndGrid();
        Radgrid1.DataBind();
    }
}