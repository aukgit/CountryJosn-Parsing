﻿USE [E:\WORKING\DEVELOPERS ORGANISM\CODE EXAMPLES\COUNTRYJOSN PARSING\JSONCOUNTRYPARSING\JSONCOUNTRYPARSING\BIN\DEBUG\PARSING-DB.MDF]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[DeleteAllWithoutLanguages]

SELECT	@return_value as 'Return Value'

GO