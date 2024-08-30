using OnlineOrders.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OnlineOrders.Core
{
    public class HelperMethods
    {
        #region UtilityMethods
        public bool IsAnyNullOrEmpty(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public int CreateOrderID(ObservableCollection<Order> orders)
        {
            int ID = 0;

            for (int i = 1; i < int.MaxValue; i++)
            {
                if (!orders.Any(o => o.ID == i))
                {
                    ID = i;
                    break;
                }
            }

            return ID;
        }
        #endregion

        #region Products
        public ObservableCollection<Product> GetProducts(string connectionString)
        {
            ObservableCollection<Product> _products = new ObservableCollection<Product>();

            string sqlQuery = "SELECT * FROM Products";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Price = Convert.ToDecimal(reader["Price"])
                            };

                            _products.Add(product);
                        }
                    }
                }
            }

            return _products;
        }

        public async void DecreaseUpdateProduct(string connectionString, int ID, int Quantity)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Products SET Quantity = Quantity - {Quantity} WHERE ID = {ID}";

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async void IncreaseUpdateProduct(string connectionString, int ID, int Quantity)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Products SET Quantity = Quantity + {Quantity} WHERE ID = {ID}";

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        #endregion

        #region OrderProducts
        public ObservableCollection<OrderProduct> GetOrderProducts(string connectionString, int ID)
        {
            ObservableCollection<OrderProduct> _products = new ObservableCollection<OrderProduct>();

            string sqlQuery = $"SELECT * FROM Orders_Products WHERE OrderID = {ID}";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            OrderProduct product = new OrderProduct
                            {
                                ID = Convert.ToInt32(reader["ProductID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                OrderID = Convert.ToInt32(reader["OrderID"])
                            };

                            _products.Add(product);
                        }
                    }
                }
            }

            return _products;
        }

        public async void DeleteOrderProduct(string connectionString, int OrderID, int ProductID)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM Orders_Products WHERE OrderID = {OrderID} AND ProductID = {ProductID}";

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async void CreateOrderProduct(string connectionString, int OrderID, int ProductID, int Quantity)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = $"INSERT INTO Orders_Products (OrderID, ProductID, Quantity) VALUES ({OrderID}, {ProductID}, {Quantity})";

                        await connection.OpenAsync();

                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Введены неверные данные!");
                    }
                }
            }
        }

        public async void UpdateOrderProdcut(string connectionString, int OrderID, int ProductID, int Quantity)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Orders_Products SET Quantity = {Quantity} WHERE OrderID = {OrderID} AND ProductID = {ProductID}";

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        #endregion

        #region Orders
        public ObservableCollection<Order> GetOrders(string connectionString, ObservableCollection<Client> clients, int ClientID, string Status)
        {
            ObservableCollection<Order> _orders = new ObservableCollection<Order>();

            string sqlQuery = "SELECT * FROM Orders";

            if (ClientID == 0)
            {
                if (Status == "Все")
                {

                }
                else
                {
                    sqlQuery = $"SELECT * FROM Orders WHERE OrderStatus = '{Status}'";
                }
            }
            else
            {
                if (Status == "Все")
                {
                    sqlQuery = $"SELECT * FROM Orders WHERE ClientID = {ClientID}";
                }
                else
                {
                    sqlQuery = $"SELECT * FROM Orders WHERE ClientID = {ClientID} AND OrderStatus = '{Status}'";
                }
            }

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Order order = new Order
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                Total = Convert.ToInt32(reader["Total"]),
                                ClientID = Convert.ToInt32(reader["ClientID"]),
                                OrderStatus = Convert.ToString(reader["OrderStatus"]),
                                Comment = Convert.ToString(reader["Comment"])
                            };

                            order.ClientFullName = clients.Where(c => c.ID == order.ClientID).FirstOrDefault().FullName;

                            _orders.Add(order);
                        }
                    }
                }
            }

            return _orders;
        }

        public async void DeleteOrder(string connectionString, int ID)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM Orders WHERE ID = {ID}";

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async void CreateOrder(string connectionString, int ID, string CreatedAt, decimal Total, int ClientID, string OrderStatus, string Comment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = $"INSERT INTO Orders (ID, CreatedAt, Total, ClientID, OrderStatus, Comment) VALUES ({ID}, '{CreatedAt}', {Total}, '{ClientID}', '{OrderStatus}', '{Comment}')";

                        await connection.OpenAsync();

                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Введены неверные данные!");
                    }
                }
            }
        }

        public async void UpdateOrder(string connectionString, int ID, decimal Total, string OrderStatus, string Comment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Orders SET Total = {Total}, OrderStatus = '{OrderStatus}', Comment = '{Comment}' WHERE ID = {ID}";

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        #endregion

        #region Clients
        public ObservableCollection<Client> GetClients(string connectionString)
        {
            ObservableCollection<Client> _Clients = new ObservableCollection<Client>()
            {
                new Client
                {
                    ID = 0,
                    FullName = "Все"
                }
            };

            string sqlQuery = "SELECT * FROM Clients";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Client client = new Client
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Surname = Convert.ToString(reader["Surname"]),
                                FirstName = Convert.ToString(reader["FirstName"]),
                                MiddleName = Convert.ToString(reader["MiddleName"]),
                                Sex = Convert.ToChar(reader["Sex"]),
                                BirthDate = Convert.ToDateTime(reader["BirthDate"]).ToString("dd.MM.yyyy"),
                                PhoneNumber = Convert.ToString(reader["PhoneNumber"]),
                                Email = Convert.ToString(reader["Email"])
                            };

                            if (client.MiddleName.Length > 0)
                            {
                                client.FullName = $"{client.Surname} {client.FirstName[0]}. {client.MiddleName[0]}.";
                            }
                            else
                            {
                                client.FullName = $"{client.Surname} {client.FirstName[0]}.";
                            }
                            _Clients.Add(client);
                        }
                    }
                }
            }

            return _Clients;
        }

        public async void DeleteClient(string connectionString, int ID)
        {   
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM Clients WHERE ID = {ID}";

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async void UpdateClient(string connectionString, int ID, string Surname, string FirstName, string MiddleName, char Sex, string BirthDate, string PhoneNumber, string Email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Clients SET Surname = '{Surname}', FirstName = '{FirstName}', MiddleName = '{MiddleName}', Sex = '{Sex}', BirthDate = '{BirthDate}', PhoneNumber = '{PhoneNumber}', Email = '{Email}' WHERE ID = {ID}";

                    await connection.OpenAsync();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async void CreateClient(string connectionString, string Surname, string FirstName, string MiddleName, char Sex, string BirthDate, string PhoneNumber, string Email)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = $"INSERT INTO Clients (Surname, FirstName, MiddleName, Sex, BirthDate, PhoneNumber, Email) VALUES ('{Surname}', '{FirstName}', '{MiddleName}', '{Sex}', '{BirthDate}', '{PhoneNumber}', '{Email}')";

                        await connection.OpenAsync();

                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Введены неверные данные!");
                    }
                }
            }
        }
        #endregion
    }
}
