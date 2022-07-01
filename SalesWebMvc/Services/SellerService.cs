using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exception;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private readonly SalesWebMvcContext _context;
        private readonly SalesRecordService _salesRecordService;

        public SellerService(SalesWebMvcContext context, SalesRecordService salesRecordService)
        {
            _context = context;
            _salesRecordService = salesRecordService;
        }

        public async Task<List<Seller>> FindAllAsync()
        {
            return await _context.Seller.ToListAsync();
        }

        public async Task<Seller> FindByIdAsync(int id)
        {
            return await _context.Seller.Include(s => s.Department).SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task RemoveAsync(int id)
        {
            var seller = _context.Seller.Find(id);
            var taskSales = _salesRecordService.RemoveAll(id);
            taskSales.Wait();
            _context.Remove(seller);
            await _context.SaveChangesAsync();
        }

        public async Task Insert(Seller seller)
        {
            _context.Add(seller);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Seller seller)
        {
            try
            {
                var hasAny = await _context.Seller.AnyAsync(s => s.Id == seller.Id);
                if (!hasAny) throw new NotFoundException("Não existe esse Id");

                _context.Update(seller);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
