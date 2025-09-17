using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using TPVWPF.Commands;
using TPVWPF.Data;
using TPVWPF.Models;
using TPVWPF.Services;
using TPVWPF.ViewModels.Interfaces;

namespace TPVWPF.ViewModels
{
    public class OrderDetailViewModel : INotifyPropertyChanged, IOrderDetailViewModel
    {
        private Ticket _currentOrder;
        private ObservableCollection<TicketLine> _orderItems;
        private Customer _customer;
        private decimal _total;
        private readonly ApplicationDbContext _dbContext;

        public OrderDetailViewModel(ApplicationDbContext dbContext)
        {
            LoggingService.Information("OrderDetailViewModel inicializado.");
            _dbContext = dbContext;
            ExportInvoiceCommand = new RelayCommand(ExportInvoice);
        }

        public Ticket CurrentOrder
        {
            get { return _currentOrder; }
            set
            {
                _currentOrder = value;
                OnPropertyChanged(nameof(CurrentOrder));
                if (_currentOrder != null)
                {
                    LoggingService.Debug("Se ha asignado una orden para mostrar sus detalles. ID de la orden: {TicketId}", _currentOrder.Id);
                    LoadOrderDetails(_currentOrder.Id);
                }
                else
                {
                    LoggingService.Debug("Se ha deseleccionado la orden. Los detalles de la orden se limpiarán.");
                    OrderItems = null;
                    Customer = null;
                    Total = 0;
                }
            }
        }

        public ObservableCollection<TicketLine> OrderItems
        {
            get { return _orderItems; }
            set
            {
                _orderItems = value;
                OnPropertyChanged(nameof(OrderItems));
                LoggingService.Debug("Lista de items de la orden actualizada. Nuevo conteo: {OrderItemCount}", _orderItems?.Count);
            }
        }

        public Customer Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
                if (_customer != null)
                {
                    LoggingService.Debug("Cliente asociado a la orden cargado: {CustomerFullName} (ID: {CustomerId})", $"{_customer.FirstName} {_customer.LastName}", _customer.Id);
                }
                else
                {
                    LoggingService.Debug("No hay cliente asociado a esta orden o la información del cliente se ha limpiado.");
                }
            }
        }

        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged(nameof(Total));
                LoggingService.Debug("Total de la orden cargado o actualizado: {Total}", _total);
            }
        }

        public ICommand ExportInvoiceCommand { get; }

        private void LoadOrderDetails(int ticketId)
        {
            LoggingService.Information("Cargando detalles de la orden con ID: {TicketId}.", ticketId);
            try
            {
                // Cargar el ticket específico incluyendo el cliente y las líneas de ticket con sus productos
                var ticketFromDb = _dbContext.Tickets.Include(t => t.Customer)
                                                    .Include(t => t.TicketLines)
                                                    .ThenInclude(tl => tl.Product)
                                                    .FirstOrDefault(t => t.Id == ticketId);

                if (ticketFromDb != null)
                {
                    foreach (var line in ticketFromDb.TicketLines)
                    {
                        line.Ticket = null;
                        if (line.Product != null)
                            line.Product.TicketLines = null;
                    }

                    _currentOrder = ticketFromDb;  // Asigna directamente al campo privado

                    //CurrentOrder = null; // Evitar recursión innecesaria en el setter
                    Customer = ticketFromDb.Customer;
                    OrderItems = new ObservableCollection<TicketLine>(ticketFromDb.TicketLines);
                    Total = ticketFromDb.Total;
                    LoggingService.Debug("Detalles de la orden {TicketId} cargados exitosamente. Se encontraron {OrderItemCount} items.", ticketId, OrderItems.Count);
                }
                else
                {
                    // Manejar el caso en que no se encuentra el ticket
                    OrderItems = new ObservableCollection<TicketLine>();
                    Total = 0;
                    Customer = null;
                    LoggingService.Warning("No se encontraron detalles para la orden con ID: {TicketId}.", ticketId);
                }
            }
            catch (System.Exception ex)
            {
                LoggingService.Error("Error al cargar los detalles de la orden con ID: {TicketId}.", ticketId, ex);
                OrderItems = new ObservableCollection<TicketLine>();
                Total = 0;
                Customer = null;
            }
        }

        private void ExportInvoice(object parameter)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = $"Ticket_{CurrentOrder.Id}.pdf",
                Filter = "PDF Files (*.pdf)|*.pdf|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    byte[] pdfBytes = GenerateTicketPDF(CurrentOrder);
                    File.WriteAllBytes(sfd.FileName, pdfBytes);
                    System.Windows.MessageBox.Show("Factura exportada con éxito.", "Éxito", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error al exportar la factura: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    LoggingService.Error("Error al generar el PDF de la factura", ex);
                }
            }
        }

        private byte[] GenerateTicketPDF(Ticket ticket)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Header
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
                var subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var regularFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);

                Paragraph header = new Paragraph("Harmonix", titleFont);
                header.Alignment = Element.ALIGN_CENTER;
                document.Add(header);
                document.Add(new Paragraph(" "));
                document.Add(new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -2));

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph($"ID de Ticket: {ticket.Id}", regularFont));
                document.Add(new Paragraph($"Fecha: {ticket.Date.ToShortDateString()}", regularFont));
                document.Add(new Paragraph($"Cliente: {ticket.Customer.FirstName} {ticket.Customer.LastName}", regularFont));
                document.Add(new Paragraph($"Dirección: {ticket.Customer.Address}", regularFont));
                document.Add(new Paragraph($"Método de pago: {ticket.Customer.PaymentMethod}", regularFont));
   
                document.Add(new Paragraph(" "));

                // Tabla
                PdfPTable table = new PdfPTable(4)
                {
                    WidthPercentage = 100
                };
                table.SetWidths(new float[] { 4, 1, 2, 2 });

                //Headers de tabla
                table.AddCell(new PdfPCell(new Phrase("Producto", boldFont)));
                table.AddCell(new PdfPCell(new Phrase("Cantidad", boldFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("Precio Unitario", boldFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase("Subtotal", boldFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });

                // Filas con productos
                foreach (TicketLine item in ticket.TicketLines)
                {
                    var product = item.Product;
                    decimal subtotal = product.Price * item.Quantity;

                    table.AddCell(new PdfPCell(new Phrase(product.Name, regularFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(product.Price.ToString("C2"), regularFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    table.AddCell(new PdfPCell(new Phrase(subtotal.ToString("C2"), regularFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                document.Add(table);
                document.Add(new Paragraph(" "));

                // Total
                Paragraph total = new Paragraph($"Total: {ticket.Total.ToString("C2")}", totalFont);
                total.Alignment = Element.ALIGN_RIGHT;
                document.Add(total);

                // Footer
                document.Add(new Paragraph(" "));
                Paragraph thankYou = new Paragraph("¡Gracias por su compra!", subHeaderFont);
                thankYou.Alignment = Element.ALIGN_CENTER;
                document.Add(thankYou);

                document.Close();

                return memoryStream.ToArray();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}