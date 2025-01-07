

using LoyaltyCouponsSystem.DAL.DB;
using LoyaltyCouponsSystem.DAL.Entity;
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

        public long GetNextNumInYear()
        {
            int currentYear = DateTime.Now.Year;

            // Check if the year exists
            var globalCounter = _context.GlobalCounters
                .SingleOrDefault(gc => gc.YearNotId == currentYear);

            if (globalCounter == null)
            {
                // If not exists, create a new row for the current year
                globalCounter = new GlobalCounter
                {
                    YearNotId = currentYear,
                    MaXNumberInYear=1
                    
                     
                };
                _context.GlobalCounters.Add(globalCounter);
            }
            else
            {
                // Increment the counter
                globalCounter.MaXNumberInYear++;
            }

            _context.SaveChanges(); // Save changes to the database

            return globalCounter.MaXNumberInYear;
        }
        public long GetNextSerialNumInYear()
        {
            int currentYear = DateTime.Now.Year;

            // Check if the year exists
            var globalCounter = _context.GlobalCounters
                .SingleOrDefault(gc => gc.YearNotId == currentYear);

            if (globalCounter == null)
            {
                // If not exists, create a new row for the current year
                globalCounter = new GlobalCounter
                {
                    YearNotId = currentYear,
              
                    MaxSerialNumber = 1

                };
                _context.GlobalCounters.Add(globalCounter);
            }
            else
            {
                // Increment the counter
                globalCounter.MaxSerialNumber++;
            }

            _context.SaveChanges(); // Save changes to the database

            return globalCounter.MaxSerialNumber;
        }
        public long UpdateMaxSerialNum(int Count)
        {
            int currentYear = DateTime.Now.Year;

            // Check if the year exists
            var globalCounter = _context.GlobalCounters
                .SingleOrDefault(gc => gc.YearNotId == currentYear);

           // Increment the counter
              globalCounter.MaxSerialNumber+=Count;
           

            _context.SaveChanges(); // Save changes to the database

            return globalCounter.MaxSerialNumber;
        }


    }
}
