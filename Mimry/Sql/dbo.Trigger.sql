CREATE TRIGGER [UpdateLastModifiedTrigger]
	ON [dbo].[Mims]
	AFTER INSERT, UPDATE
	AS
	BEGIN
		SET NOCOUNT ON
		UPDATE [dbo].[Mims] Set LastModifiedDate = GetDate() where MimID in (SELECT MimID FROM Inserted)
	END

CREATE TRIGGER [UpdateCreatedDateTriggerMim]
	ON [dbo].[Mims]
	AFTER INSERT
	AS
	BEGIN
		SET NOCOUNT ON
		UPDATE [dbo].[Mims] Set CreatedDate = GetDate() where MimID in (SELECT MimID FROM Inserted)
	END

CREATE TRIGGER [UpdateCreatedDateTriggerMimSeq]
	ON [dbo].[Mims]
	AFTER INSERT
	AS
	BEGIN
		SET NOCOUNT ON
		UPDATE [dbo].[Mims] Set CreatedDate = GetDate() where MimID in (SELECT MimID FROM Inserted)
	END
