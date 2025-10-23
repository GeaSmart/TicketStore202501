using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Persistence.Configurations
{
    public class StoredProcedureConfiguration
    {
        public static string CreateSP = @"
        create or alter procedure usp_ListConcerts 
	        @title nvarchar(50)
        as
        begin
	        SELECT [c].[Id], 
		        [c].[Title], 
		        [c].[Description], 
		        [c].[Place], 
		        [c].[UnitPrice], 
		        [g].[Name] 'Genre', 
		        [c].[GenreId], 
		        CONVERT(VARCHAR, [c].[DateEvent], 3) 'DateEvent', 
		        CONVERT(VARCHAR, [c].[DateEvent], 8) 'TimeEvent', 
		        [c].[ImageUrl], 
		        [c].[TicketsQuantity], 
		        [c].[Finalized], 
		        CASE
                  WHEN [c].[Status] = CAST(1 AS bit) 
		          THEN N'Activo'
                  ELSE N'Inactivo'
		        END 'Status'
              FROM [Musicales].[Concert] AS [c]
              INNER JOIN [Musicales].[Genre] AS [g] ON [c].[GenreId] = [g].[Id]
              WHERE ([c].[Title] LIKE '%'+@title+'%')	  
        end";

        public static string DropSP = "DROP PROCEDURE IF EXISTS usp_ListConcerts";
    }
}
