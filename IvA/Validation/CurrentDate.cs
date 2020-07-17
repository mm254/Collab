using System;
using System.ComponentModel.DataAnnotations;

namespace IvA.Validation
{

    // Validierungsmodell zur Überprüfung, ob ein Datum in der Zukunft oder Vergangenheit liegt. 
    public class CurrentDateAttribute : ValidationAttribute
    {
        public CurrentDateAttribute()
        {
        }

        //Erhält als Parameter (object value) ein Datum. Die Methode überprüft, ob das Datum in der Zukunft liegt (true) oder nicht (false).
        public override bool IsValid(object value)
        {
            var dt = (DateTime)value;
            if (dt >= DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
