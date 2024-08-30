using OnlineOrders.Core;
using OnlineOrders.MVVM.Model;
using OnlineOrders.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OnlineOrders.MVVM.ViewModel
{
    class EditOrderViewModel : ObservableObject
    {
        #region ViewBindings
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }

        private ObservableCollection<Client> _clients;

        public ObservableCollection<Client> clients
        {
            get { return _clients; }
            set { _clients = value; OnPropertyChanged(nameof(clients)); }
        }

        private Client _selectedClient;

        public Client selectedClient
        {
            get { return _selectedClient; }
            set { _selectedClient = value; OnPropertyChanged(nameof(selectedClient)); }
        }

        private ObservableCollection<string> _statuses;

        public ObservableCollection<string> statuses
        {
            get { return _statuses; }
            set { _statuses = value; OnPropertyChanged(nameof(statuses)); }
        }

        private string _selectedStatus;

        public string selectedStatus
        {
            get { return _selectedStatus; }
            set { _selectedStatus = value; OnPropertyChanged(nameof(selectedStatus)); }
        }

        private Order _Order;

        public Order Order
        {
            get { return _Order; }
            set { _Order = value; OnPropertyChanged(nameof(Order)); }
        }

        private decimal _Total;

        public decimal Total
        {
            get { return _Total; }
            set { _Total = value; OnPropertyChanged(nameof(Total)); }
        }

        private ObservableCollection<Product> _products;

        public ObservableCollection<Product> products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged(nameof(products)); }
        }

        private OrderProduct _selectedProduct;

        public OrderProduct selectedProduct
        {
            get { return _selectedProduct; }
            set { _selectedProduct = value; OnPropertyChanged(nameof(selectedProduct)); }
        }
        #endregion

        #region Variables
        public string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public HelperMethods HelperMethods = new HelperMethods();
        private ObservableCollection<Order> orders;
        private int DirectionFlag;
        private bool isNullFlag;
        #endregion

        #region Methods
        public void SetViewData()
        {
            products = HelperMethods.GetProducts(connectionString);
            statuses = new ObservableCollection<string>
            {
                "Создан",
                "Отменен",
                "Собран",
                "В доставке",
                "Доставлен",
                "Закрыт",
                "Возврат",
            };

            if (Order == null)
            {
                Order = new Order 
                { 
                    ID = HelperMethods.CreateOrderID(orders),
                    IsCreated = true
                };
                selectedStatus = statuses[0];
            }
            else
            {
                selectedClient = clients.Where(c => c.FullName == Order.ClientFullName).FirstOrDefault();
                clients.Clear();
                clients.Add(selectedClient);
                selectedStatus = Order.OrderStatus;
                if (DirectionFlag == 1)
                {
                    Order.Products = HelperMethods.GetOrderProducts(connectionString, Order.ID);
                }

                decimal Total = 0;
                foreach (var product in Order.Products)
                {
                    product.Price = product.Quantity * products.Where(p => p.ID == product.ID).First().Price;
                    Total += product.Price;
                }

                Order.Total = Total;
                this.Total = Total;
            }
        }

        public void OpenEditOrderContentWindow()
        {
            if (Order.Products.Count < products.Count && products.Any(p => p.Quantity > 0) && products.Count > 0)
            {
                Order.OrderStatus = selectedStatus;
                Order.ClientFullName = selectedClient.FullName;
                EditOrderContent editOrderContentView = new EditOrderContent { DataContext = new EditOrderContentViewModel(selectedProduct, Order, orders, clients) };
                editOrderContentView.ShowDialog();
            }
            else
            {
                MessageBox.Show("Больше нет товаров, которые можно добавить");
            }
        }

        public void WindowsManage()
        {
            Application.Current.Windows[Application.Current.Windows.Count - 4].Close();
            Main MainWindow = new Main { DataContext = new MainViewModel() };
            MainWindow.Show();
            Application.Current.Windows[Application.Current.Windows.Count - 3].Close();
        }
        #endregion

        #region Contructor
        public EditOrderViewModel(Order selectedOrder, ObservableCollection<Order> orders, ObservableCollection<Client> clients, int DirectionFlag)
        {
            this.clients = new ObservableCollection<Client>();
            foreach (Client client in clients)
            {
                if (client.FullName != "Все")
                    this.clients.Add(client);
            }

            this.DirectionFlag = DirectionFlag;
            this.Order = selectedOrder;
            this.orders = orders;

            SetViewData();

            ConfirmCommand = new RelayCommand(_ =>
            {
                if (Order.Products.Count >= 1 && selectedClient != null)
                {
                    if (Order.IsCreated)
                    {
                        Order.ID = HelperMethods.CreateOrderID(orders);
                        HelperMethods.CreateOrder(connectionString, Order.ID, DateTime.Now.ToString(), Order.Total, clients.Where(c => c.FullName == selectedClient.FullName).FirstOrDefault().ID, selectedStatus, Order.Comment);
                        foreach (var Product in Order.Products)
                        {
                            HelperMethods.CreateOrderProduct(connectionString, Product.OrderID, Product.ID, Product.Quantity);
                            HelperMethods.DecreaseUpdateProduct(connectionString, Product.ID, Product.Quantity);
                        }
                    }
                    else
                    {
                        HelperMethods.UpdateOrder(connectionString, Order.ID, Order.Total, selectedStatus, Order.Comment);
                        if (selectedStatus == "Возврат")
                        {
                            foreach (var Product in Order.Products)
                            {
                                HelperMethods.IncreaseUpdateProduct(connectionString, Product.ID, Product.Quantity);
                            }
                        }
                        else
                        {
                            foreach (var Product in Order.Products)
                            {
                                if (Product.IsCreated)
                                {
                                    HelperMethods.CreateOrderProduct(connectionString, Product.OrderID, Product.ID, Product.Quantity);
                                    HelperMethods.DecreaseUpdateProduct(connectionString, Product.ID, Product.Quantity);
                                }
                            }
                        }
                    }

                    WindowsManage();
                }
                else
                {
                    MessageBox.Show("Заполните пустые поля!");
                }
            });

            AddCommand = new RelayCommand(_ =>
            {
                if (selectedClient != null)
                {
                    if (selectedStatus != "Возврат")
                        OpenEditOrderContentWindow();
                }
                else { MessageBox.Show("Выберите клиента!"); }
            });

            DeleteCommand = new RelayCommand(_ =>
            {
                if (selectedProduct != null && selectedStatus != "Возврат")
                {
                    HelperMethods.DeleteOrderProduct(connectionString, Order.ID, selectedProduct.ID);
                    HelperMethods.IncreaseUpdateProduct(connectionString, selectedProduct.ID, selectedProduct.Quantity);
                    this.Order.Products.Remove(selectedProduct);

                    Order.Total = 0;
                    Total = 0;

                    foreach (var product in Order.Products)
                    {
                        Order.Total += product.Price;
                        Total += product.Price;
                    }
                }
            });
        }
        #endregion
    }
}
