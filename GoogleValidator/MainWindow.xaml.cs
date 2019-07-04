using GemBox.Spreadsheet;
using Interoute.P2O.Olo.SupplierModel;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GoogleValidator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
        private Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
        private string workbookPath = "";
        ExcelFile inputWorkbook;
        ExcelFile workbook;
        ExcelWorksheet inputexcelWorksheet;
        ExcelWorksheet excelWorksheet;
        public MainWindow()
        {
            InitializeComponent();
            SpreadsheetInfo.SetLicense("EPAS-VI3T-QDHK-P60D");
        }
        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            var config = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            CustomChannelFactory<AddressValidatorServiceProxy.IAddressValidatorService> factory = new CustomChannelFactory<AddressValidatorServiceProxy.IAddressValidatorService>("BasicHttpBinding_IAddressValidatorService", config);
            var client = factory.CreateChannel();

            var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var fullFileName = System.IO.Path.Combine(desktopFolder, "ValidatedAddresses_" + String.Format("{0:yyyy/MM/dd}", DateTime.Now).ToString().Replace("/", "") + System.IO.Path.GetExtension(workbookPath));

            workbook = new ExcelFile();
            workbook.Worksheets.Add("Sheet1");
            excelWorksheet = workbook.Worksheets[0];
            excelWorksheet.Cells[0, 0].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 1].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 2].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 3].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 4].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 5].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 0].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 3].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 4].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 5].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            excelWorksheet.Cells[0, 0].Style.Font.Weight = ExcelFont.BoldWeight;
            excelWorksheet.Cells[0, 1].Style.Font.Weight = ExcelFont.BoldWeight;
            excelWorksheet.Cells[0, 2].Style.Font.Weight = ExcelFont.BoldWeight;
            excelWorksheet.Cells[0, 3].Style.Font.Weight = ExcelFont.BoldWeight;
            excelWorksheet.Cells[0, 4].Style.Font.Weight = ExcelFont.BoldWeight;
            excelWorksheet.Cells[0, 5].Style.Font.Weight = ExcelFont.BoldWeight;
            excelWorksheet.Cells[0, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            excelWorksheet.Cells[0, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            excelWorksheet.Cells[0, 2].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            excelWorksheet.Cells[0, 3].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            excelWorksheet.Cells[0, 4].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            excelWorksheet.Cells[0, 5].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Aqua, System.Drawing.Color.Aqua);
            excelWorksheet.Cells[0, 0].Value = "Country";
            excelWorksheet.Cells[0, 1].Value = "State";
            excelWorksheet.Cells[0, 2].Value = "City";
            excelWorksheet.Cells[0, 3].Value = "Postcode";
            excelWorksheet.Cells[0, 4].Value = "Street";
            excelWorksheet.Cells[0, 5].Value = "House";

            try
            {
                for (int i = 0; i < inputWorkbook.Worksheets.Count; i++)
                {
                    inputexcelWorksheet = inputWorkbook.Worksheets[i];
                    pbValidation.Value = 0;
                    pbValidation.Maximum = inputexcelWorksheet.Rows.Count;
                    Task.Run(() =>
                    {
                        for (int r = 0; r < inputexcelWorksheet.Rows.Count; r++)
                        {
                            if (inputexcelWorksheet.Cells[r, 0].Value != null)
                            {
                                Thread.Sleep(50);
                                this.Dispatcher.Invoke(() => //Use Dispather to Update UI Immediately  
                                {
                                    pbValidation.Value = r + 1;
                                });
                                var address = inputexcelWorksheet.Cells[r, 0].Value;
                                try
                                {
                                    var validatedAddress = client.ValidateAndGetAddressText(address.ToString(), AddressValidatorServiceProxy.GoogleServicelanguages.en);
                                    excelWorksheet.Cells[r + 1, 0].Value = String.IsNullOrEmpty(validatedAddress.Country) ? "" : validatedAddress.Country;
                                    excelWorksheet.Cells[r + 1, 1].Value = String.IsNullOrEmpty(validatedAddress.State) ? "" : validatedAddress.State;
                                    excelWorksheet.Cells[r + 1, 2].Value = String.IsNullOrEmpty(validatedAddress.City) ? "" : validatedAddress.City;
                                    excelWorksheet.Cells[r + 1, 3].Value = String.IsNullOrEmpty(validatedAddress.Postcode) ? "" : validatedAddress.Postcode;
                                    excelWorksheet.Cells[r + 1, 4].Value = String.IsNullOrEmpty(validatedAddress.Street) ? "" : validatedAddress.Street;
                                    excelWorksheet.Cells[r + 1, 5].Value = String.IsNullOrEmpty(validatedAddress.House) ? "" : validatedAddress.House;
                                }
                                catch (Exception ex)
                                {
                                    excelWorksheet.Cells[r + 1, 0].Comment.Text = "Validation for address: " + address.ToString() + " failed. " + ex.Message;
                                    excelWorksheet.Cells[r + 1, 0].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Red, System.Drawing.Color.Red);
                                    excelWorksheet.Cells[r + 1, 1].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Red, System.Drawing.Color.Red);
                                    excelWorksheet.Cells[r + 1, 2].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Red, System.Drawing.Color.Red);
                                    excelWorksheet.Cells[r + 1, 3].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Red, System.Drawing.Color.Red);
                                    excelWorksheet.Cells[r + 1, 4].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Red, System.Drawing.Color.Red);
                                    excelWorksheet.Cells[r + 1, 5].Style.FillPattern.SetPattern(FillPatternStyle.Solid, System.Drawing.Color.Red, System.Drawing.Color.Red);
                                    continue;
                                }
                            }
                            else
                            {
                                MessageBox.Show("An error has occurred", "Error", MessageBoxButton.OK);
                                return;
                            }
                        }
                    }).ContinueWith(t =>
                    {
                        saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.FileName = "ValidatedAddresses_" + String.Format("{0:yyyy/MM/dd}", DateTime.Now).ToString().Replace("/", "");
                        saveFileDialog.DefaultExt = System.IO.Path.GetExtension(workbookPath);
                        saveFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                        // Show save file dialog box
                        Nullable<bool> result = saveFileDialog.ShowDialog();
                        if (result == true)
                        {
                            // Save document
                            string filename = saveFileDialog.FileName;
                            if (System.IO.Path.GetExtension(filename).Equals(".xls", StringComparison.InvariantCultureIgnoreCase))
                            {
                                workbook.SaveXls(filename);
                            }
                            else
                            {
                                workbook.SaveXlsx(filename);
                            }
                            
                            var mainMessage = MessageBox.Show("Validation completed! The file has been saved to: " + filename + "\n Do you want to open the file?", "Validation completed",
              MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                            if (mainMessage == MessageBoxResult.Yes)
                            {
                                System.Diagnostics.Process.Start(@filename);
                            }
                        }
                    }
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK);
            }
        }
        private void menuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
            openFileDialog.ShowDialog();
            inputWorkbook = new ExcelFile();
            if (String.IsNullOrEmpty(openFileDialog.FileName))
            {
                MessageBox.Show("Please open the excel file", "Warning", MessageBoxButton.OK);
                return;
            }
            if (System.IO.Path.GetExtension(openFileDialog.FileName).Equals(".xls", StringComparison.InvariantCultureIgnoreCase))
            {
                inputWorkbook.LoadXls(openFileDialog.FileName, XlsOptions.PreserveWorksheetRecords);
            }
            else if (System.IO.Path.GetExtension(openFileDialog.FileName).Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase))
            {
                inputWorkbook.LoadXlsx(openFileDialog.FileName, XlsxOptions.PreserveMakeCopy);
            }
            else
            {
                MessageBox.Show("Only files of type XLS and XLSX can be imported", "Warning", MessageBoxButton.OK);
                return;
            }

            workbookPath = openFileDialog.FileName;
            inputexcelWorksheet = inputWorkbook.Worksheets[0];
            pbValidation.Value = 0;
            pbValidation.Maximum = inputexcelWorksheet.Rows.Count;
            spValidateButton.Visibility = Visibility.Visible;
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
