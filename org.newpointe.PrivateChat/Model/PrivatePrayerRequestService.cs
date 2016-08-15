using org.newpointe.PrivateChat.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.newpointe.PrivateChat.Model
{
    public class PrivatePrayerRequestService : PrivateChatService<PrivatePrayerRequest>
    {
        public PrivatePrayerRequestService(PrivateChatContext context) : base(context) { }
    }
}
