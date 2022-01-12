
namespace Laborator_6_PSSC.Domain.Models
{
    //    public record PublishedStudentGrade(StudentRegistrationNumber StudentRegistrationNumber, Grade ExamGrade, Grade ActivityGrade, Grade FinalGrade);

    public record PaidShoppingCart(ProductCode productCode, Quantity quantity, Address address, Price price, Price finalPrice);
}
