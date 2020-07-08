using IvA.Data;
using IvA.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Validation
{
    public class Helper
    {
        private readonly ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public Helper()
        {
            
        }

        public string[] CalculatePercentages(List<ArbeitsPaketModel> packages)
        {
            string[] percentages = new string[3];
            int packagesCount = packages.Count();
            if (packagesCount != 0)
            {
                int[] count = new int[3];
                foreach (ArbeitsPaketModel pack in packages)
                {
                    switch (pack.Status)
                    {
                        case "To do": count[0]++; break;
                        case "In Bearbeitung": count[1]++; break;
                        case "Fertig": count[2]++; break;
                    }
                }
                percentages[0] = Decimal.Round(Decimal.Multiply(Decimal.Divide(count[0], packagesCount), 100)).ToString() + "%";
                percentages[1] = Decimal.Round(Decimal.Multiply(Decimal.Divide(count[1], packagesCount), 100)).ToString() + "%";
                percentages[2] = Decimal.Round(Decimal.Multiply(Decimal.Divide(count[2], packagesCount), 100)).ToString() + "%";
            }
            else
            {
                percentages[0] = "0%";
                percentages[1] = "0%";
                percentages[2] = "0%";
            }
            return percentages;
        }
    }
}
