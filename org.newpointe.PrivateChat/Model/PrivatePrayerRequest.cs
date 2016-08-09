using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;

using Rock.Data;
using Rock.Model;

using org.newpointe.PrivateChat.Data;

namespace org.newpointe.PrivateChat.Model
{
    [Table( "_org_newpointe_PrivateChat_PrivatePrayerRequest")]
    [DataContract]
    public class PrivatePrayerRequest : Rock.Data.Model<PrivatePrayerRequest>, Rock.Security.ISecured
    {
        
        //public virtual Person Person { get; set; }

        [DataMember]
        public int? Person_Id { get; set; }
        
        [Required( ErrorMessage = "RoomId is required")]
        [DataMember]
        public string RoomId { get; set; }

        [Required]
        [DataMember]
        public bool Answered { get; set; }

        [Required]
        [DataMember]
        public string Name { get; set; }
    }

    public partial class PrivatePrayerRequestConfiguration : EntityTypeConfiguration<PrivatePrayerRequest>
    {
        public PrivatePrayerRequestConfiguration()
        {
            //this.HasOptional(r => r.Person).WithMany().HasForeignKey(r => r.Person_Id).WillCascadeOnDelete(false);
        }
    }
}
