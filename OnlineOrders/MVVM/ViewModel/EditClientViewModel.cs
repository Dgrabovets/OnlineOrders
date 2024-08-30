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
    class EditClientViewModel : ObservableObject
    {
        #region ViewBindings
        public ICommand ConfirmCommand { get; set; }

        private Client _client;

        public Client client
        {
            get { return _client; }
            set { _client = value; OnPropertyChanged(nameof(client)); }
        }

        private ObservableCollection<char> _genders;

        public ObservableCollection<char> genders
        {
            get { return _genders; }
            set { _genders = value; OnPropertyChanged(nameof(genders)); }
        }

        private char _selectedGender;

        public char selectedGender
        {
            get { return _selectedGender; }
            set { _selectedGender = value; OnPropertyChanged(nameof(selectedGender)); }
        }
        #endregion

        #region Variables
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private HelperMethods HelperMethods = new HelperMethods();
        private bool isNullFlag;
        private ObservableCollection<Client> clients;
        #endregion

        #region Methods
        public void SetViewData()
        {
            genders = new ObservableCollection<char>
            {
                'М',
                'Ж'
            };

            if (client != null)
            {
                selectedGender = genders.Where(s => s == client.Sex).FirstOrDefault();
            }
            else { selectedGender = genders.FirstOrDefault(); }
        }

        public void WindowsManage()
        {
            Application.Current.Windows[Application.Current.Windows.Count - 4].Close();
            Clients clientsWindow = new Clients { DataContext = new ClientsViewModel(clients) };
            clientsWindow.Show();
            Application.Current.Windows[Application.Current.Windows.Count - 3].Close();
        }
        #endregion

        #region Contructor
        public EditClientViewModel(Client client, ObservableCollection<Client> clients)
        {
            this.clients = clients;

            if (client == null)
            {
                this.client = new Client();
                isNullFlag = true;
            }
            else { this.client = client; }

            SetViewData();

            ConfirmCommand = new RelayCommand(_ =>
            {
                if (this.client.Surname != null && this.client.FirstName != null && this.client.PhoneNumber != null && this.client.Email != null && (selectedGender == 'М' || selectedGender == 'Ж'))
                {
                    DateTime bufferDateTime;
                    bool isDateTime = DateTime.TryParse(this.client.BirthDate, out bufferDateTime);

                    long bufferNumber;
                    bool isNumber = long.TryParse(this.client.PhoneNumber, out bufferNumber);

                    if (isDateTime && isNumber && HelperMethods.IsValidEmail(this.client.Email))
                    {
                        this.client.Email = this.client.Email.ToLower();

                        if (!isNullFlag)
                        {
                            this.clients.Remove(this.clients.First(c => c.ID == this.client.ID));
                            this.clients.Add(this.client);
                            HelperMethods.UpdateClient(connectionString, this.client.ID, this.client.Surname, this.client.FirstName, this.client.MiddleName, selectedGender, this.client.BirthDate, this.client.PhoneNumber, this.client.Email);
                            WindowsManage();
                        }
                        else
                        {
                            if (!clients.Any(c => c.PhoneNumber == this.client.PhoneNumber) && !clients.Any(c => c.Email == this.client.Email))
                            {
                                this.clients.Add(this.client);
                                HelperMethods.CreateClient(connectionString, this.client.Surname, this.client.FirstName, this.client.MiddleName, selectedGender, this.client.BirthDate, this.client.PhoneNumber, this.client.Email);
                                WindowsManage();
                            }
                            else { MessageBox.Show("Введены дубликаты имеющихся данных!"); }
                        }
                    }
                    else { MessageBox.Show("Некорреткно введенные данные!"); }
                }
                else { MessageBox.Show("Заполните все поля!"); }
            });
        }
        #endregion
    }
}
