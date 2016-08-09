using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock.Plugin;

namespace org.newpointe.PrivateChat.Migrations
{
    [MigrationNumber(1, "1.0.8")]
    public class CreateDb : Migration
    {
        public override void Up()
        {
            Sql(@"
            CREATE TABLE [dbo].[_org_newpointe_PrivateChat_PrivatePrayerRequest](
                [Id] [int] IDENTITY(1,1) NOT NULL,
                [Person_Id] [int] NULL,
                [Name]  [nvarchar](max) NOT NULL,
                [RoomId] [nvarchar](max) NOT NULL,
                [Answered] [bit] NOT NULL,
                [Guid] [uniqueidentifier] NOT NULL,
	            [CreatedDateTime] [datetime] NULL,
	            [ModifiedDateTime] [datetime] NULL,
	            [CreatedByPersonAliasId] [int] NULL,
	            [ModifiedByPersonAliasId] [int] NULL,
	            [ForeignId] [nvarchar](50) NULL,
                [ForeignKey] [nvarchar](50) NULL,
                [ForeignGuid] [nvarchar](50) NULL
                CONSTRAINT [PK_dbo._org_newpointe_PrivateChat_PrivatePrayerRequest] PRIMARY KEY CLUSTERED
                (
                  [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
            )"
            );
        }

        public override void Down()
        {

            Sql(@"
                DROP TABLE [dbo].[_org_newpointe_PrivateChat_PrivatePrayerRequest]"
            );
        }

    }
}
