SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author: Alan Lai
-- Create date: 4/1/2011
-- Description: Processes mail objects in the email queue to be sent
-- =============================================
CREATE PROCEDURE usp_ProcessMailing
@immediate bit = 0
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;

declare @subject varchar(100), @body varchar(max), @emails varchar(max), @queueId int
declare @automatedEmail varchar(50) = 'automatedemail@caes.ucdavis.edu'
declare @queue cursor

set @queue = cursor for
select emailqueue.id, [subject], [body], emailaddress
from emailqueue
where Pending = 1
and [immediate] = @immediate

open @queue

fetch next from @queue into @queueId, @subject, @body, @emails

while (@@FETCH_STATUS = 0)
begin

exec msdb.dbo.sp_send_dbmail
@recipients = @emails,
@blind_copy_recipients = @automatedEmail,
@subject = @subject,
@body = @body,
@body_format = 'HTML';

update emailqueue
set errorcode = @@ERROR, SentDateTime = GETDATE(), Pending = 0
where id = @queueId

fetch next from @queue into @queueId, @subject, @body, @emails
end

close @queue
deallocate @queue

END
GO

