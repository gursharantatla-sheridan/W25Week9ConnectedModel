using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace W25Week9ConnectedModel;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    string connStr = ConfigurationManager.ConnectionStrings["Northwind"].ConnectionString;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void LoadGrid()
    {
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string query = "select EmployeeID, FirstName, LastName City, Country from Employees";
            SqlCommand cmd = new SqlCommand(query, conn);

            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            DataTable tbl = new DataTable();
            tbl.Load(reader);
            grdEmployees.ItemsSource = tbl.DefaultView;
        }
        //conn.Close();
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
        LoadGrid();
    }

    private void btnSearch_Click(object sender, RoutedEventArgs e)
    {
        // string concatination - bad practice
        //string query = "select EmployeeID, FirstName, LastName City, Country from Employees where FirstName='" + txtFirstname.Text + "'";

        // parameterized query - good practice
        string query = "select EmployeeID, FirstName, LastName City, Country from Employees where FirstName=@firstname";

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@firstname", txtFirstname.Text);

            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            DataTable tbl = new DataTable();
            tbl.Load(reader);

            grdEmployees.ItemsSource = tbl.DefaultView;
        }
    }

    private void btnCount_Click(object sender, RoutedEventArgs e)
    {
        string query = "select count(*) from Employees";

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            int rows = (int)cmd.ExecuteScalar();

            MessageBox.Show("Total rows: " + rows);
        }
    }

    private void btnInsert_Click(object sender, RoutedEventArgs e)
    {
        string query = "insert into Employees(Firstname, Lastname) values(@fn, @ln)";

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@fn", txtFirstname.Text);
            cmd.Parameters.AddWithValue("@ln", txtLastname.Text);

            conn.Open();

            int result = cmd.ExecuteNonQuery();

            if (result == 1)
            {
                LoadGrid();
                MessageBox.Show("New employee added");
            }
            else
                MessageBox.Show("Employee not added");
        }
    }
}