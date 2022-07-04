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

        public async Task RemoveAll(int id)
        {
            if (!_context.SalesRecords.Any(s => s.Seller.Id == id)) return;

            var sales = await _context.SalesRecords.Where(s => s.Seller.Id == id).ToListAsync();
            _context.SalesRecords.RemoveRange(sales);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var salesRecord = from obj in _context.SalesRecords select obj;
            if (minDate.HasValue)
            {
                salesRecord = salesRecord.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                salesRecord = salesRecord.Where(x => x.Date <= minDate.Value);
            }

            return await salesRecord
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<IGrouping<Department, SalesRecord>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecords select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            return await result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.Seller.Department)
                .ToListAsync();
        }
    }
}
