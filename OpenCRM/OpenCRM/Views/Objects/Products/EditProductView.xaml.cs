﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCRM.Models.Objects.Products;

namespace OpenCRM.Views.Objects.Products
{
    /// <summary>
    /// Interaction logic for EditProductView.xaml
    /// </summary>
    public partial class EditProductView : Page
    {
        ProductsModel _productModel;

        public EditProductView()
        {
            InitializeComponent();
            _productModel = new ProductsModel();
            //_productModel.LoadEditProduct(this);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //_productModel.Save(
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            PageSwitcher.Switch("/Views/Objects/Products/ProductsView.xaml");
        }
    }
}
