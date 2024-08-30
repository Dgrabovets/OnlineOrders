using OnlineOrders.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Configuration;
using OnlineOrders.MVVM.Model;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OnlineOrders.MVVM.View;

namespace OnlineOrders.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        #region ViewBindings
        public ICommand UpdateCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditClientCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ClientChanged { get; set; }
        public ICommand StatusChanged { get; set; }

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
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(selectedClient));
            }
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
            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(selectedStatus));
            }
        }

        private ObservableCollection<Order> _orders;

        public ObservableCollection<Order> orders
        {
            get { return _orders; }
            set { _orders = value; OnPropertyChanged(nameof(orders)); }
        }

        private Order _selectedOrder;

        public Order selectedOrder
        {
            get { return _selectedOrder; }
            set { _selectedOrder = value; OnPropertyChanged(nameof(selectedOrder)); }
        }
        #endregion

        #region Variables
        public string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public HelperMethods HelperMethods = new HelperMethods();
        #endregion

        #region Methods
        public void SetViewData()
        {
            clients = HelperMethods.GetClients(connectionString);
            selectedClient = clients[0];

            statuses = new ObservableCollection<string>
            {
                "Все",
                "Создан",
                "Отменен",
                "Собран",
                "В доставке",
                "Доставлен",
                "Закрыт",
                "Возврат",
            };

            selectedStatus = statuses[0];
            orders = HelperMethods.GetOrders(connectionString, clients, 0, "Все");
        }

        public void OpenEditOrderWindow()
        {
            EditOrder editOrderView = new EditOrder { DataContext = new EditOrderViewModel(selectedOrder, orders, clients, 1) };
            editOrderView.ShowDialog();
            SetViewData();
        }
        #endregion

        #region Contstructor
        public MainViewModel()
        {
            SetViewData();

            #region Binding Commands
            EditClientCommand = new RelayCommand(_ =>
            {
                Clients clientsView = new Clients { DataContext = new ClientsViewModel(clients)};
                bool? result = clientsView.ShowDialog();
                if (result == false)
                {
                    SetViewData();
                }
            });

            UpdateCommand = new RelayCommand(_ =>
            {
                if (selectedOrder != null && selectedOrder.OrderStatus != "Возврат")
                {
                    OpenEditOrderWindow();
                }
            });

            AddCommand = new RelayCommand(_ =>
            {
                OpenEditOrderWindow();
            });

            DeleteCommand = new RelayCommand(_ =>
            {
                if (selectedOrder != null)
                {
                    selectedOrder.Products = HelperMethods.GetOrderProducts(connectionString, selectedOrder.ID);
                    foreach (var product in selectedOrder.Products)
                    {
                        HelperMethods.IncreaseUpdateProduct(connectionString, product.ID, product.Quantity);
                    }
                    selectedOrder.Products.Clear();
                    HelperMethods.DeleteOrder(connectionString, selectedOrder.ID);
                    this.orders.Remove(selectedOrder);
                }
            });

            ResetCommand = new RelayCommand(_ => SetViewData());

            ClientChanged = new RelayCommand(_ =>
            {
                if (selectedClient != null)
                {
                    orders = HelperMethods.GetOrders(connectionString, clients, selectedClient.ID, selectedStatus);
                }
            });

            StatusChanged = new RelayCommand(_ =>
            {
                if (selectedStatus != null)
                {
                    orders = HelperMethods.GetOrders(connectionString, clients, selectedClient.ID, selectedStatus);
                }
            });
            #endregion
        }
        #endregion
    }
}
