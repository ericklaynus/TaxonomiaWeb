﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using TaxonomiaWeb.ServiceBmvXbrl;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using TaxonomiaWeb.Utils;
using System.Text.RegularExpressions;

namespace TaxonomiaWeb.Forms
{
    public partial class Page800200 : Page
    {
        private List<string> listHiddenColumns = null;
        private Dictionary<int, List<int>> listTotal = null;
        private Service1Client servBmvXbrl = null;
        private ObservableCollection<Bmv800200> listaBmv = null;
        private ObservableCollection<Bmv800200> listaBmvAgrupada = null;
        private ObservableCollection<BmvDetalleSuma> listBmvSuma = null;
        private MainPage mainPage = null;
        private ObservableCollection<String> listContextoSinPresentar = null;

        public Page800200()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainPage = (MainPage)Application.Current.RootVisual;
            try
            {
                string parameterPageName = string.Empty;
                string trimestre = string.Empty;
                string ano = string.Empty;
                string compania = string.Empty;

                if (NavigationContext.QueryString.TryGetValue("trim", out trimestre))
                {
                    mainPage.NumTrimestre = Int32.Parse(trimestre);
                }
                if (NavigationContext.QueryString.TryGetValue("ejerc", out ano))
                {
                    mainPage.IdAno = Int32.Parse(ano);
                }
                if (NavigationContext.QueryString.TryGetValue("comp", out compania))
                {
                    mainPage.Compania = compania;
                }
                if (NavigationContext.QueryString.TryGetValue("name", out parameterPageName))
                {
                    Uri uri = new Uri(string.Format("/Home?trim={0}&ejerc={1}&comp={2}", mainPage.NumTrimestre.ToString(), mainPage.IdAno.ToString(), mainPage.Compania), UriKind.Relative);
                    mainPage.actualizarTituloContenidos(parameterPageName, "Regresar", uri);
                }
            }
            catch (Exception ex)
            {
                ChildWindow errorWin = new ErrorWindow("URL no válida", "La URL especificada no es válida. Se direccionará a la página de inicio");
                errorWin.Show();
                mainPage.navegarPagina(new Uri("/Inicio", UriKind.Relative));
            }
        }


        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            fillHiddenColumns();
            servBmvXbrl = new Service1Client();
            servBmvXbrl.GetPeriodoSinPresentarCompleted += servBmvXbrl_GetPeriodoSinPresentarCompleted;
            servBmvXbrl.GetPeriodoSinPresentarAsync(mainPage.NumTrimestre, "800200");
            //Agregamos los manejadores de eventos del datagrid
            //Se dispara cuando se comienza a editar una celda
            this.DgvTaxo.PreparingCellForEdit += DgvTaxo_PreparingCellForEdit;
            //Se dispara cuando se cargan las filas en el datagrid
            this.DgvTaxo.LoadingRow += DgvTaxo_LoadingRow;
            //Para que con un solo click o con el teclado entre en modo editar
            this.DgvTaxo.CurrentCellChanged += DgvTaxo_CurrentCellChanged;

            this.DgvTaxo.LayoutUpdated += DgvTaxo_LayoutUpdated;

        }


        #region Eventos del Datagrid

        void DgvTaxo_LayoutUpdated(object sender, EventArgs e)
        {
            DataGrid grid = this.DgvTaxo;
            if (grid != null)
            {
                grid.FrozenColumnCount = 1;
                ObservableCollection<DataGridColumn> listColumns = grid.Columns;
                foreach (var dgColumn in listColumns)
                {

                    string headerName = Regex.Replace(dgColumn.Header == null ? "" : dgColumn.Header.ToString(), @"\s+", "");
                    //Orden de la columnas mostradas
                    switch (headerName)
                    {
                        case AppConsts.COL_DESCRIPCION:
                            dgColumn.DisplayIndex = 0;
                            break;

                        case AppConsts.COL_TRIMESTREACTUAL:
                            dgColumn.DisplayIndex = 1;
                            break;

                        case AppConsts.COL_TRIMESTREANOANTERIOR:
                            dgColumn.DisplayIndex = 2;
                            break;

                        case AppConsts.COL_ACUMULADOANOACTUAL:
                            dgColumn.DisplayIndex = 3;
                            break;

                        case AppConsts.COL_ACUMULADOANOANTERIOR:
                            dgColumn.DisplayIndex = 4;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void DgvTaxo_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Solo buscamos las columnas visibles, las ocultas las ignoramos. 
            if (listHiddenColumns != null && listHiddenColumns.Contains(e.PropertyName) == false)
            {
                //Checamos que sea una columna tipo DataGridTextColumn
                DataGridTextColumn dgColumn = e.Column as DataGridTextColumn;
                if (dgColumn != null)
                {
                    DataGrid grid = sender as DataGrid;
                    grid.FrozenColumnCount = 1;
                    Style elementStyle = new Style(typeof(TextBlock));
                    elementStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                    Style editingElmentStyle = new Style(typeof(TextBox));
                    editingElmentStyle.Setters.Add(new Setter(TextBox.TextWrappingProperty, TextWrapping.Wrap));
                    editingElmentStyle.Setters.Add(new Setter(TextBox.AcceptsReturnProperty, true));
                    string headerName = string.Concat(e.Column.Header.ToString().Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                    dgColumn.EditingElementStyle = editingElmentStyle;
                    dgColumn.ElementStyle = elementStyle;
                    dgColumn.Header = headerName;
                    //Orden de la columnas mostradas
                    switch (e.PropertyName)
                    {
                        case AppConsts.COL_DESCRIPCION:
                            dgColumn.MaxWidth = AppConsts.MAXWIDTH_COL_DESCRIPCION;
                            break;

                        case AppConsts.COL_TRIMESTREACTUAL:
                            dgColumn.Width = DataGridLength.SizeToHeader;
                            break;

                        case AppConsts.COL_TRIMESTREANOANTERIOR:
                            dgColumn.Width = DataGridLength.SizeToHeader;
                            break;

                        case AppConsts.COL_ACUMULADOANOACTUAL:
                            dgColumn.Width = DataGridLength.SizeToHeader;
                            break;

                        case AppConsts.COL_ACUMULADOANOANTERIOR:
                            dgColumn.Width = DataGridLength.SizeToHeader;
                            break;
                    }
                    if (listContextoSinPresentar.Contains(e.PropertyName) == true)
                    {
                        dgColumn.Visibility = Visibility.Collapsed;
                    }
                    if (e.PropertyType == typeof(double))
                    {
                        DataGridBoundColumn obj = e.Column as DataGridBoundColumn;
                        if (obj != null && obj.Binding != null)
                        {
                            obj.Binding.StringFormat = "{0:n0}";
                        }
                    }
                }
            }
            //En caso de que sea una columna oculta, pues la ocultamos.
            else
            {
                hiddenColumn(e.Column);
            }

        }



        void DgvTaxo_CurrentCellChanged(object sender, EventArgs e)
        {

            DgvTaxo.BeginEdit();
        }


        private void DgvTaxo_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                DataGridRow dgr = e.Row;
                System.Reflection.PropertyInfo[] listProp = typeof(Bmv800200).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var prop in listProp)
                {
                    DataGridColumn column = DgvTaxo.Columns.SingleOrDefault(x => Regex.Replace(x.Header.ToString(), @"\s+", "").Equals(prop.Name));
                    if (column != null)
                    {
                        Bmv800200 row = dgr.DataContext as Bmv800200;
                        FrameworkElement cellElement = column.GetCellContent(dgr);
                        FrameworkElement eleCell = Utilerias.GetElementParent(cellElement, typeof(DataGridCell));
                        DataGridCell dgCell = (DataGridCell)eleCell;
                        //Necesario para ajustar los renglones de cada celda y no crezcan estas mismas.
                        if (cellElement != null)
                        {
                            cellElement.SizeChanged += CellElement_SizeChanged;
                        }
                        if (row.Lectura == true)
                        {
                            //Ocultamos las celdas donde no se debe de editar nada

                            dgCell.IsEnabled = false;
                            dgCell.Foreground = new SolidColorBrush(Colors.Transparent);
                            dgCell.Opacity = 0.2;
                        }
                        else
                        {
                            dgCell.IsEnabled = true;
                            dgCell.Foreground = new SolidColorBrush(Colors.Black);
                            dgCell.Opacity = 1;
                        }

                    }
                }
                DataGridColumn columnDesc = DgvTaxo.Columns.SingleOrDefault(x => Regex.Replace(x.Header.ToString(), @"\s+", "").Equals(AppConsts.COL_DESCRIPCION));
                if (columnDesc != null)
                {
                    Bmv800200 row = dgr.DataContext as Bmv800200;
                    FrameworkElement cellElement = columnDesc.GetCellContent(dgr);
                    FrameworkElement eleCell = Utilerias.GetElementParent(cellElement, typeof(DataGridCell));
                    DataGridCell dgCell = (DataGridCell)eleCell;
                    //Agregamos en negrita la sinopsis
                    if (row.Lectura == true)
                    {

                        dgCell.FontWeight = FontWeights.Bold;
                    }
                    //Agregamos en cursiva los totales
                    else if ((listTotal != null && listTotal.ContainsKey(row.IdTaxonomiaDetalle) == true))
                    {
                        dgCell.FontStyle = FontStyles.Italic;
                    }

                }

            }
            catch (Exception ex)
            {


            }
        }

        private void CellElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Fix issue: DataGrid Row Auto-Resize only grows row height but won't shrink
            var dataGrid = Utilerias.GetParentOf<DataGrid>((FrameworkElement)sender);
            if (dataGrid != null && double.IsNaN(dataGrid.RowHeight))
            {
                var row = DataGridRow.GetRowContainingElement((FrameworkElement)sender);

                //Fore recalculating row height
                try
                {
                    row.InvalidateMeasure();
                    row.Measure(row.RenderSize);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //Restore RowHeight
                    dataGrid.RowHeight = double.NaN;
                }
            }
        }


        private void DgvTaxo_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            try
            {
                Bmv800200 row = e.Row.DataContext as Bmv800200;
                TextBox textBox = (e.EditingElement as TextBox);
                //Si la celda a editar es lectura o es la columna de descripcion o es un total no se podra editar.
                if (row.Lectura == true || e.Column.Header.ToString().Equals(AppConsts.COL_DESCRIPCION) == true || (listTotal != null && listTotal.ContainsKey(row.IdTaxonomiaDetalle) == true))
                {
                    //textBox.Text = "";
                    textBox.IsReadOnly = true;
                    textBox.Background = new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    SharedEvents se = new SharedEvents();
                    //Dependiendo del formato de captura se aplica la plantilla
                    switch (row.FormatoCampo)
                    {
                        case AppConsts.FORMAT_TEXTBLOCK:
                        case AppConsts.FORMAT_TEXT:
                            break;
                        case AppConsts.FORMAT_YYMMDD:

                            break;
                        case AppConsts.FORMAT_X:
                        case AppConsts.FORMAT_X_NEGATIVE:
                        case AppConsts.FORMAT_SHARES:
                        case AppConsts.FORMAT_SUM:
                            textBox.KeyDown += se.NumericOnCellKeyDown;
                            textBox.TextChanged += se.NumericOnCellTextChanged;
                            break;
                        case AppConsts.FORMAT_XXX:
                            textBox.KeyDown += se.NumericDecimalOnCellKeyDown;
                            textBox.TextChanged += se.NumericDecimalOnCellTextChanged;
                            break;
                        default:
                            break;

                    }

                    // Binding customBinding = new Binding(e.Column.Header.ToString());
                    // customBinding.Source = row;
                    // customBinding.Mode = BindingMode.TwoWay;
                    // customBinding.ConverterParameter = "C2";
                    // customBinding.Converter = new NumberConverter();
                    // textBox.SetBinding(TextBox.TextProperty, customBinding);
                }
            }
            catch (Exception ex)
            {


            }

        }
        #endregion

        #region Eventos de otros controles

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (validarDatos() == true)
            {
                //Generamos nueva lista con los valores actualizados
                ObservableCollection<ReporteDetalle> sortedList = sortReport(listaBmvAgrupada, listaBmv);
                servBmvXbrl = new Service1Client();
                servBmvXbrl.SaveBmvReporteCompleted += servBmvXbrl_SaveBmvReporteCompleted;
                servBmvXbrl.SaveBmvReporteAsync(sortedList, mainPage.Compania,  mainPage.IdAno, mainPage.NumTrimestre);
                busyIndicator.IsBusy = true;
            }
        }
        void BtnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            string nameFile = Utilerias.ExportarDataGrid(DgvTaxo);
            if (nameFile != null && nameFile.Length > 0)
            {
                MessageBox.Show(string.Format("Archivo {0} exportado correctamente", nameFile));
            }
            else if (nameFile == null)
            {
                MessageBox.Show("Hubo un error al exportar el reporte");
            }
        }
        #endregion

        #region Llamadas a servicios asincronos WCF


        void servBmvXbrl_GetPeriodoSinPresentarCompleted(object sender, GetPeriodoSinPresentarCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                listContextoSinPresentar = e.Result;
            }
            servBmvXbrl.GetBmv800200Completed += servBmvXbrl_GetBmv800200Completed;
            servBmvXbrl.GetBmv800200Async(mainPage.NumTrimestre, mainPage.IdAno);
        }


        void servBmvXbrl_GetBmv800200Completed(object sender, GetBmv800200CompletedEventArgs e)
        {
            if (e.Result != null)
            {
                listaBmv = e.Result;
                listaBmvAgrupada = bindAndGroupList(listaBmv);
                foreach (var item in listaBmvAgrupada)
                {
                    item.PropertyChanged += new PropertyChangedEventHandler(bmv_PropertyChanged);
                }
                servBmvXbrl = new Service1Client();
                servBmvXbrl.GetBmvDetalleSumaCompleted += servBmvXbrl_GetBmvDetalleSumaCompleted;
                servBmvXbrl.GetBmvDetalleSumaAsync("800200");
            }
        }


        void servBmvXbrl_GetBmvDetalleSumaCompleted(object sender, GetBmvDetalleSumaCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                listBmvSuma = e.Result;
                listTotal = (from p in listBmvSuma
                             group p.IdTaxonomiaHijo by p.IdTaxonomiaPadre into g
                             select new { IdPadre = g.Key, ListaIdHijos = g.ToList() }).ToDictionary(p => p.IdPadre, p => p.ListaIdHijos);
                DgvTaxo.ItemsSource = listaBmvAgrupada;

            }
        }


        void servBmvXbrl_SaveBmvReporteCompleted(object sender, SaveBmvReporteCompletedEventArgs e)
        {
            busyIndicator.IsBusy = false;
            if (e.Error == null)
            {
                bool res = (bool)e.Result;
                if (res == true)
                {
                    MessageBox.Show("Datos almacenados correctamente");
                    //Para que se vuelvan a acomodar los renglones y columnas, cargamos de nuevo todo
                    MainPage m = (MainPage)Application.Current.RootVisual;
                    m.ContentFrame.Refresh();
                }
                else
                {
                    MessageBox.Show("Ocurrio un error al guardar los datos");
                }
            }
            else
            {
                MessageBox.Show(string.Format("Ocurrio un error al guardar los datos {0}", e.Error.Message));
            }
        }
        #endregion

        #region Evento cuando la celda cambia
        private void bmv_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var user = sender as Bmv800200;
            Bmv800200 result = listaBmvAgrupada.SingleOrDefault(s => s == user);
            System.Reflection.PropertyInfo[] listProp = typeof(Bmv800200).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (result != null)
            {
                foreach (KeyValuePair<int, List<int>> value in listTotal)
                {
                    List<int> list = value.Value as List<int>;
                    if (list != null)
                    {
                        foreach (int order in list)
                        {
                            //Si contiene order en lista
                            if (order == result.IdTaxonomiaDetalle)
                            {

                                var filteredList = from o in listaBmvAgrupada
                                                   where list.Contains(o.IdTaxonomiaDetalle)
                                                   select o;
                                double subtotalPositivo = 0;
                                double subtotalNegativo = 0;
                                if (filteredList != null)
                                {
                                    subtotalPositivo = filteredList.Where(x => x.FormatoCampo.Equals(AppConsts.FORMAT_X_NEGATIVE) == false).Sum(x => Convert.ToDouble(x.AcumuladoAnoActual));
                                    subtotalNegativo = filteredList.Where(x => x.FormatoCampo.Equals(AppConsts.FORMAT_X_NEGATIVE) == true).Sum(x => Convert.ToDouble(x.AcumuladoAnoActual));
                                    //Actualizamos el campo
                                    foreach (var u in listaBmv.Where(u => u.IdTaxonomiaDetalle == value.Key))
                                    {
                                        u.AcumuladoAnoActual = subtotalPositivo - subtotalNegativo;
                                    }
                                    subtotalPositivo = 0;
                                    subtotalNegativo = 0;
                                    subtotalPositivo = filteredList.Where(x => x.FormatoCampo.Equals(AppConsts.FORMAT_X_NEGATIVE) == false).Sum(x => Convert.ToDouble(x.AcumuladoAnoAnterior));
                                    subtotalNegativo = filteredList.Where(x => x.FormatoCampo.Equals(AppConsts.FORMAT_X_NEGATIVE) == true).Sum(x => Convert.ToDouble(x.AcumuladoAnoAnterior));
                                    //Actualizamos el campo
                                    foreach (var u in listaBmv.Where(u => u.IdTaxonomiaDetalle == value.Key))
                                    {
                                        u.AcumuladoAnoAnterior = subtotalPositivo - subtotalNegativo;
                                    }
                                    subtotalPositivo = 0;
                                    subtotalNegativo = 0;
                                    subtotalPositivo = filteredList.Where(x => x.FormatoCampo.Equals(AppConsts.FORMAT_X_NEGATIVE) == false).Sum(x => Convert.ToDouble(x.TrimestreActual));
                                    subtotalNegativo = filteredList.Where(x => x.FormatoCampo.Equals(AppConsts.FORMAT_X_NEGATIVE) == true).Sum(x => Convert.ToDouble(x.TrimestreActual));
                                    //Actualizamos el campo
                                    foreach (var u in listaBmv.Where(u => u.IdTaxonomiaDetalle == value.Key))
                                    {
                                        u.TrimestreActual = subtotalPositivo - subtotalNegativo;
                                    }
                                    subtotalPositivo = 0;
                                    subtotalNegativo = 0;
                                    subtotalPositivo = filteredList.Where(x => x.FormatoCampo.Equals(AppConsts.FORMAT_X_NEGATIVE) == false).Sum(x => Convert.ToDouble(x.TrimestreAnoAnterior));
                                    subtotalNegativo = filteredList.Where(x => x.FormatoCampo.Equals(AppConsts.FORMAT_X_NEGATIVE) == true).Sum(x => Convert.ToDouble(x.TrimestreAnoAnterior));
                                    //Actualizamos el campo
                                    foreach (var u in listaBmv.Where(u => u.IdTaxonomiaDetalle == value.Key))
                                    {
                                        u.TrimestreAnoAnterior = subtotalPositivo - subtotalNegativo;
                                    }
                                }

                            }

                        }
                    }
                }

            }
        }

        #endregion


        #region Metodos de clase

        private void fillHiddenColumns()
        {
            System.Reflection.PropertyInfo[] listProp = typeof(BmvBase).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            listHiddenColumns = new List<string>();
            foreach (var prop in listProp)
            {
                if (prop.Name.Equals(AppConsts.COL_DESCRIPCION) == false)
                {
                    listHiddenColumns.Add(prop.Name);
                }
            }
        }

        private void hiddenColumn(DataGridColumn dgColumn)
        {
            bool res = false;
            res = listHiddenColumns.Contains(dgColumn.Header.ToString());
            if (res == true)
            {
                dgColumn.Visibility = Visibility.Collapsed;
            }
        }


        private ObservableCollection<Bmv800200> bindAndGroupList(ObservableCollection<Bmv800200> listaBmv)
        {
            //Agrupamos por registro de taxonomia detalle y obtenemos solo un registro
            var tempListaBmvAgrupada = from element in listaBmv
                                       group element by element.IdTaxonomiaDetalle
                                           into groups
                                           select groups.OrderBy(p => p.IdReporte).First();
            ObservableCollection<Bmv800200> listaBmvAgrupada = new ObservableCollection<Bmv800200>(tempListaBmvAgrupada);
            //Actualizamos cada campo del registro
            foreach (var itemAgrupado in listaBmvAgrupada)
            {
                if (string.IsNullOrEmpty(itemAgrupado.FormatoCampo) == false)
                {
                    var itemsBmv = from o in listaBmv
                                   where o.IdTaxonomiaDetalle == itemAgrupado.IdTaxonomiaDetalle
                                   select o;
                    foreach (var subItems in itemsBmv)
                    {
                        switch (subItems.AtributoColumna)
                        {
                            case AppConsts.COL_ACUMULADOANOACTUAL:
                                itemAgrupado.AcumuladoAnoActual = string.IsNullOrEmpty(subItems.Valor) == true ? 0 : Convert.ToDouble(subItems.Valor);
                                break;

                            case AppConsts.COL_ACUMULADOANOANTERIOR:
                                itemAgrupado.AcumuladoAnoAnterior = string.IsNullOrEmpty(subItems.Valor) == true ? 0 : Convert.ToDouble(subItems.Valor);
                                break;

                            case AppConsts.COL_TRIMESTREACTUAL:
                                itemAgrupado.TrimestreActual = string.IsNullOrEmpty(subItems.Valor) == true ? 0 : Convert.ToDouble(subItems.Valor);
                                break;

                            case AppConsts.COL_TRIMESTREANOANTERIOR:
                                itemAgrupado.TrimestreAnoAnterior = string.IsNullOrEmpty(subItems.Valor) == true ? 0 : Convert.ToDouble(subItems.Valor);
                                break;
                            default:
                                break;
                        }
                    }
                }

            }

            return listaBmvAgrupada;
        }


        private ObservableCollection<ReporteDetalle> sortReport(ObservableCollection<Bmv800200> listaBmvAgrupada, ObservableCollection<Bmv800200> listaBmv)
        {
            ObservableCollection<ReporteDetalle> sortedList = new ObservableCollection<ReporteDetalle>();
            foreach (var itemAgrupado in listaBmvAgrupada)
            {
                if (string.IsNullOrEmpty(itemAgrupado.FormatoCampo) == false)
                {
                    var itemsBmv = from o in listaBmv
                                   where o.IdTaxonomiaDetalle == itemAgrupado.IdTaxonomiaDetalle
                                   select o;
                    foreach (var subItems in itemsBmv)
                    {
                        if (listContextoSinPresentar != null && listContextoSinPresentar.Contains(subItems.AtributoColumna) == false)
                        {
                            ReporteDetalle rd = new ReporteDetalle();
                            switch (subItems.AtributoColumna)
                            {
                                case AppConsts.COL_ACUMULADOANOACTUAL:
                                    rd.Valor = Convert.ToString(itemAgrupado.AcumuladoAnoActual);
                                    break;

                                case AppConsts.COL_ACUMULADOANOANTERIOR:
                                    rd.Valor = Convert.ToString(itemAgrupado.AcumuladoAnoAnterior);
                                    break;

                                case AppConsts.COL_TRIMESTREACTUAL:
                                    rd.Valor = Convert.ToString(itemAgrupado.TrimestreActual);
                                    break;

                                case AppConsts.COL_TRIMESTREANOANTERIOR:
                                    rd.Valor = Convert.ToString(itemAgrupado.TrimestreAnoAnterior);
                                    break;
                                default:
                                    break;
                            }
                            rd.IdReporte = subItems.IdReporte;
                            rd.IdReporteDetalle = subItems.IdReporteDetalle;
                            rd.Estado = true;
                            sortedList.Add(rd);
                        }
                    }
                }
            }
            return sortedList;


        }

        private bool validarDatos()
        {
            bool res = false;
            if (listaBmvAgrupada != null)
            {
                foreach (var renglon in listaBmvAgrupada)
                {
                    //Si son editables se tienen que validar los campos
                    if (renglon.Lectura == false)
                    {
                        //if (renglon.TrimestreActual == null)
                        //{
                        //    int indexOf = listaBmv.IndexOf(renglon);
                        //    DgvTaxo.SelectedItem = renglon;
                        //    DataGridColumn dgc = Utilerias.FindColumnByName(DgvTaxo.Columns, AppConsts.COL_TRIMESTREACTUAL);
                        //    if (dgc != null)
                        //    {
                        //        DgvTaxo.CurrentColumn = dgc;
                        //        DgvTaxo.Dispatcher.BeginInvoke(() => { DgvTaxo.ScrollIntoView(renglon, dgc); });
                        //    }
                        //    DgvTaxo.Focus();
                        //    DgvTaxo.BeginEdit();
                        //    return res;

                        //}

                    }
                }
            }
            else
            {
                return res;
            }
            res = true;
            return res;
        }

    }
        #endregion
}
