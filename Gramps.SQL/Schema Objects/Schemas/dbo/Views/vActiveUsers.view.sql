CREATE VIEW dbo.vActiveUsers
AS
SELECT     Catbert3.dbo.Permissions.UserID, Catbert3.dbo.Permissions.ApplicationID, Catbert3.dbo.Permissions.Inactive
FROM         Catbert3.dbo.Permissions INNER JOIN
                      dbo.vApplications ON Catbert3.dbo.Permissions.ApplicationID = dbo.vApplications.ApplicationID
WHERE     (Catbert3.dbo.Permissions.Inactive = 0)