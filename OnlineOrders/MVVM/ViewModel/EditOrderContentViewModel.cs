using OnlineOrders.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OnlineOrders.MVVM.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Configuration;
using System.Windows;
using OnlineOrders.MVVM.View;
using System.Text.RegularExpressions;

namespace OnlineOrders.MVVM.ViewModel
{
    class EditOrderContentViewModel : ObservableObject
    {
        #region ViewBindings
        public ICommand ConfirmCommand { get; set; }
        public ICommand MinusCommand { get; set; }
        public ICommand PlusCommand { get; set; }

        private ObservableCollection<Product> _products;

        public ObservableCollection<Product> products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged(nameof(products)); }
        }

        private Product _selectedProduct;

        public Product selectedProduct
        {
            get { return _selectedProduct; }
            set { _selectedProduct = value; OnPropertyChanged(nameof(selectedProduct)); }
        }

        private int _Quantity;

        public int Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; OnPropertyChanged(nameof(Quantity)); }
        }
        #endregion

        #region Variables
        public HelperMethods HelperMethods = new HelperMethods();
        public string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public Order selectedOrder;
        public ObservableCollection<Order> orders = new ObservableCollection<Order>();
        public ObservableCollection<Client> clients = new ObservableCollection<Client>();
        private bool isNullFlag = false;
        private Product outerProduct;
        #endregion

        #region Methods
        public void SetViewData()
        {
            products = HelperMethods.GetProducts(connectionString);

            foreach (var Product in selectedOrder.Products)
            {
                products.Remove(products.Where(p => p.ID == Product.ID).First());
            }

            for (int i = products.Count - 1; i > -1; i--)
            {
                if (products[i].Quantity == 0)
                {
                    products.Remove(products[i]);
                }
            }

            if (products.Count > 0)
            {
                selectedProduct = products[0];
                Quantity = 1;
            }
        }

        public void WindowsManage()
        {
            Application.Current.Windows[Application.Current.Windows.Count - 4].Close();
            EditOrder EditOrderWindow = new EditOrder { DataContext = new EditOrderViewModel(selectedOrder, orders, clients, 0) };
            EditOrderWindow.Show();
            Application.Current.Windows[Application.Current.Windows.Count - 3].Close();
        }
        #endregion

        #region Contructor
        public EditOrderContentViewModel(Product selectedProduct, Order selectedOrder, ObservableCollection<Order> orders, ObservableCollection<Client> clients)
        {
            this.outerProduct = selectedProduct;
            this.selectedOrder = selectedOrder;
            this.orders = orders;
            this.clients = clients;

            SetViewData();

            #region Binding Commands
            MinusCommand = new RelayCommand(_ =>
            {
                if (Quantity > 1)
                    Quantity -= 1;
            });

            PlusCommand = new RelayCommand(_ =>
            {
                if (Quantity + 1 <= this.selectedProduct.Quantity)
                    Quantity += 1;
            });

            ConfirmCommand = new RelayCommand(_ =>
            {
                if (this.selectedProduct != null)
                {
                    this.selectedOrder.Products.Add(new OrderProduct
                    {
                        ID = this.selectedProduct.ID,
                        OrderID = this.selectedOrder.ID,
                        Quantity = Quantity,
                        Price = Quantity * this.selectedProduct.Price,
                        IsCreated = true
                    });
                }
                else { MessageBox.Show("Больше нет товаров, которые можно добавить"); }

                WindowsManage();
            });
            #endregion
        }
        #endregion
    }
}
