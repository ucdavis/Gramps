ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [GrantManagement], FILENAME = '$(DefaultDataPath)GrantManagement.mdf', FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

