using PayerAccount.Dal.Local.Data;
using PayerAccount.Models.Remote;

namespace PayerAccount.BusinessLogic
{
    public class UserSessionState
    {
        public User User { get; private set; }
        public PayerState PayerState { get; private set; }

        public UserSessionState(User user, PayerState payerState)
        {
            User = user;
            PayerState = payerState;
        }
    }
}
