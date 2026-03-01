using Domain.Exceptions;

namespace Domain.Exceptions
{
    public class CouponNotFoundException : NotFoundException
    {
        public CouponNotFoundException(string code)
            : base($"Coupon with code '{code}' was not found")
        {
        }

        public CouponNotFoundException(Guid id)
            : base($"Coupon with ID '{id}' was not found")
        {
        }
    }
}
