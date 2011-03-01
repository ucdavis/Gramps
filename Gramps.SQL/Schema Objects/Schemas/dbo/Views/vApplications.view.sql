CREATE VIEW dbo.vApplications
AS
SELECT     ApplicationID, Name, Abbr, Location, IconLocation, Inactive, WebServiceHash, Salt
FROM         Catbert3.dbo.Applications
WHERE     (Abbr = 'Gramps')