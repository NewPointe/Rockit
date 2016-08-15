using Rock.Data;

namespace org.newpointe.PrivateChat.Data
{
    public class PrivateChatService<T> : Rock.Data.Service<T> where T : Rock.Data.Entity<T>, new()
    {
        public PrivateChatService(PrivateChatContext context)
            : base(context)
        {

        }

        public virtual bool CanDelete(T item, out string errorMessage)
        {
            errorMessage = string.Empty;
            return true;
        }
    }
}
