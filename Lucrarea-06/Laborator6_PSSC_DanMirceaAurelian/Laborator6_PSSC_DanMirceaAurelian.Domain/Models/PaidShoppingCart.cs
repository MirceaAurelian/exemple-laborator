using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator6_PSSC_DanMirceaAurelian.Domain.Models
{
    //    public record PublishedStudentGrade(StudentRegistrationNumber StudentRegistrationNumber, Grade ExamGrade, Grade ActivityGrade, Grade FinalGrade);

    public record PaidShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price, Price finalPrice)
    {
        public int OrderLineId { get; set; }
        public int OrderId { get; set; }
    }
}
