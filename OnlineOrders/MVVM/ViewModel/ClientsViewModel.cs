using OnlineOrders.Core;
using OnlineOrders.MVVM.Model;
using OnlineOrders.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static OnlineOrders.MVVM.ViewModel.MainViewModel;

namespace OnlineOrders.MVVM.ViewModel
{
    class ClientsViewModel : ObservableObject
    {
        #region ViewBindings
        public ICommand DeleteCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand AddCommand { get; set; }

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
        #endregion

        #region Variables
        public string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public HelperMethods HelperMethods = new HelperMethods();
        #endregion

        #region Methods
        public void OpenEditClientWindow(int flag)
        {
            if (flag == 0)
            {
                EditClient editClientView = new EditClient { DataContext = new EditClientViewModel(selectedClient, clients) };
                editClientView.ShowDialog();
            }
            else
            {
                EditClient editClientView = new EditClient { DataContext = new EditClientViewModel(null, clients) };
                editClientView.ShowDialog();
            }
        }
        #endregion

        #region Contructor
        public ClientsViewModel(ObservableCollection<Client> clients)
        {
            this.clients = new ObservableCollection<Client>();
            foreach (Client client in clients)
            {
                if (client.FullName != "Все")
                    this.clients.Add(client);
            }

            #region Binding Commands
            DeleteCommand = new RelayCommand(_ =>
            {
                if (selectedClient != null)
                {
                    HelperMethods.DeleteClient(connectionString, selectedClient.ID);
                    this.clients.Remove(selectedClient);
                }
            });

            UpdateCommand = new RelayCommand(_ =>
            {
                if (selectedClient != null)
                {
                    OpenEditClientWindow(0);
                }
            });

            AddCommand = new RelayCommand(_ => OpenEditClientWindow(1));
            #endregion
        }
        #endregion
    }
}
