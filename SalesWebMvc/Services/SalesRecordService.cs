using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebMvcContext _context;

        public SalesRecordService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<SalesRecord>> FindAll(int id)
        {
            try
            {
                var salesRecords = await _context.SalesRecords.Where(s => s.Seller.Id == id).ToListAsync();

                return salesRecords;
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }

        public async Task Remove(int id)
        {
            var sales = await _context.SalesRecords.Where(s => s.Seller.Id == id).ToListAsync();
            _context.SalesRecords.RemoveRange(sales);
            await _context.SaveChangesAsync();
        }
    }
}
