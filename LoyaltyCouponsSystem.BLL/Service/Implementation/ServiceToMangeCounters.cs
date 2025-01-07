using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.BLL.Service.Implementation
{
    public class ServiceToMangeCounters
    {
        private readonly ApplicationDbContext _context;

        public ServiceToMangeCounters(ApplicationDbContext context)
        {
            _context = context;
        }

        // تحويل هذه الدالة لتعمل بشكل غير متزامن
        public async Task<long> GetNextNumInYearAsync()
        {
            int currentYear = DateTime.Now.Year;

            // Check if the year exists
            var globalCounter = await _context.GlobalCounters
                .SingleOrDefaultAsync(gc => gc.YearNotId == currentYear);

            if (globalCounter == null)
            {
                // If not exists, create a new row for the current year
                globalCounter = new GlobalCounter
                {
                    YearNotId = currentYear,
                    MaXNumberInYear = 1
                };
                await _context.GlobalCounters.AddAsync(globalCounter);
            }
            else
            {
                // Increment the counter
                globalCounter.MaXNumberInYear++;
            }

            await _context.SaveChangesAsync(); // Save changes to the database asynchronously

            return globalCounter.MaXNumberInYear;
        }

        // تحويل هذه الدالة لتعمل بشكل غير متزامن
        public async Task<long> GetNextSerialNumInYearAsync()
        {
            int currentYear = DateTime.Now.Year;

            // Check if the year exists
            var globalCounter = await _context.GlobalCounters
                .SingleOrDefaultAsync(gc => gc.YearNotId == currentYear);

            if (globalCounter == null)
            {
                // If not exists, create a new row for the current year
                globalCounter = new GlobalCounter
                {
                    YearNotId = currentYear,
                    MaxSerialNumber = 1
                };
                await _context.GlobalCounters.AddAsync(globalCounter);
            }
            else
            {
                // Increment the counter
                globalCounter.MaxSerialNumber++;
            }

            await _context.SaveChangesAsync(); // Save changes to the database asynchronously

            return globalCounter.MaxSerialNumber;
        }

        // تحويل هذه الدالة لتعمل بشكل غير متزامن
        public async Task<long> UpdateMaxSerialNumAsync(int count)
        {
            int currentYear = DateTime.Now.Year;

            // Check if the year exists
            var globalCounter = await _context.GlobalCounters
                .SingleOrDefaultAsync(gc => gc.YearNotId == currentYear);

            if (globalCounter != null)
            {
                // Increment the counter
                globalCounter.MaxSerialNumber += count;
                await _context.SaveChangesAsync(); // Save changes to the database asynchronously
            }

            return globalCounter?.MaxSerialNumber ?? 0; // Ensure we return a valid number
        }
    }
}
