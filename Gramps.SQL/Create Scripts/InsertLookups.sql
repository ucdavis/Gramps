 USE Gramps
 
 -- //////////////////////////////////////////////////////////////////////////////////////
 -- Insert the question types
 -- //////////////////////////////////////////////////////////////////////////////////////
 
 IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Text Box' )
 begin
	 INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	 VALUES ('Text Box', 0, 1)
 end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Text Area' )
begin
	 INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Text Area', 0, 0)
end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Boolean' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Boolean', 0, 0)
end

IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Radio Buttons' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Radio Buttons', 1, 0)
end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Checkbox List' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Checkbox List', 1, 0)
end
 
IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Drop Down' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Drop Down', 1, 0)
end

IF NOT EXISTS ( select * from QuestionTypes where [name] = 'Date' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('Date', 0, 1)
end

IF NOT EXISTS ( select * from QuestionTypes where [name] = 'No Answer' )
begin
	INSERT INTO QuestionTypes ([Name], hasOptions, ExtendedProperty)
	VALUES ('No Answer', 0, 0)
end
GO

-- //////////////////////////////////////////////////////////////////////////////////////
-- Insert the default validators
-- //////////////////////////////////////////////////////////////////////////////////////
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Required', 'required', '^.+$', '{0} is a required field.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Email', 'email', '(^((([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&''\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$){1}|^$', '{0} is not a valid email.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Url', 'url', '(^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&''\(\)\*\+,;=]|:|@)|\/|\?)*)?$){1}|^$', '{0} is not a valid url.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Date', 'date', '(^(((0?[1-9]|1[012])[-\/](0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])[-\/](29|30)|(0?[13578]|1[02])[-\/]31)[-\/](19|[2-9]\d)\d{2}|0?2[-\/]29[-\/]((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$){1}|^$', '{0} is not a valid date.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Phone Number', 'phoneUS', '(^\(?[\d]{3}\)?[\s-]?[\d]{3}[\s-]?[\d]{4}$){1}|^$', '{0} is not a valid phone number.')
GO
INSERT INTO [dbo].[Validators]([Name], [Class], [RegEx], [ErrorMessage])
  VALUES('Zip Code', 'zipUS', '^\d{5}(-\d{4})?$', '{0} is not a valid zip code.')
GO


