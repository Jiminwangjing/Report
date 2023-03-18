using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPrinterName
    {
        IQueryable<PrinterName> PrintNames();
        PrinterName GetbyId(int id);
        Task AddorEdit(PrinterName printer);      
        Task Delete(int id);
       
    }
    public class PrinterNameRepository : IPrinterName
    {
        private readonly DataContext _context;
        public PrinterNameRepository(DataContext dataContext)
        {
            _context = dataContext;
        }
        public async Task AddorEdit(PrinterName printer)
        {
            if (printer.ID == 0)
            {
                await _context.PrinterNames.AddAsync(printer);
            }
            else
            {
                _context.PrinterNames.Update(printer);
            }
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var prin = await _context.PrinterNames.FirstAsync(x => x.ID == id);
            prin.Delete = true;
            _context.PrinterNames.Update(prin);
            await _context.SaveChangesAsync();
        }

        public PrinterName GetbyId(int id) => _context.PrinterNames.Find(id);
        public IQueryable<PrinterName> PrintNames()
        {
            IQueryable<PrinterName> list = _context.PrinterNames.Where(x => x.Delete == false);
            return list;

        }
    }
}
