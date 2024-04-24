namespace Dokana.Services
{
    public interface ICheckoutService
    {
        bool PhoneWalet(string email, string password);


        bool Paypal(string email, string password);


        bool VisaCard(string email, string password);
    }
}
